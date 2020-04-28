// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using Microsoft.CodeDom;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading;

    internal class ClientClassGenerator : IServiceContractGenerationExtension
    {
        private bool _tryAddHelperMethod = false;
        private bool _generateEventAsyncMethods = false;

        internal ClientClassGenerator(bool tryAddHelperMethod)
            : this(tryAddHelperMethod, false)
        {
        }

        internal ClientClassGenerator(bool tryAddHelperMethod, bool generateEventAsyncMethods)
        {
            _tryAddHelperMethod = tryAddHelperMethod;
            _generateEventAsyncMethods = generateEventAsyncMethods;
        }

        private static Type s_clientBaseType = typeof(ClientBase<>);
        private static Type s_duplexClientBaseType = typeof(DuplexClientBase<>);
        private static Type s_instanceContextType = typeof(InstanceContext);
        private static Type s_objectType = typeof(object);
        private static Type s_objectArrayType = typeof(object[]);
        private static Type s_exceptionType = typeof(Exception);
        private static Type s_boolType = typeof(bool);
        private static Type s_stringType = typeof(string);
        private static Type s_endpointAddressType = typeof(EndpointAddress);
        private static Type s_uriType = typeof(Uri);
        private static Type s_bindingType = typeof(Binding);
        private static Type s_sendOrPostCallbackType = typeof(SendOrPostCallback);
        private static Type s_asyncCompletedEventArgsType = typeof(AsyncCompletedEventArgs);
        private static Type s_eventHandlerType = typeof(EventHandler<>);
        private static Type s_voidType = typeof(void);
        private static Type s_asyncResultType = typeof(IAsyncResult);
        private static Type s_asyncCallbackType = typeof(AsyncCallback);

        private static CodeTypeReference s_voidTypeRef = new CodeTypeReference(typeof(void));
        private static CodeTypeReference s_asyncResultTypeRef = new CodeTypeReference(typeof(IAsyncResult));

        private static string s_inputInstanceName = "callbackInstance";
        private static string s_invokeAsyncCompletedEventArgsTypeName = "InvokeAsyncCompletedEventArgs";
        private static string s_invokeAsyncMethodName = "InvokeAsync";
        private static string s_raiseExceptionIfNecessaryMethodName = "RaiseExceptionIfNecessary";
        private static string s_beginOperationDelegateTypeName = "BeginOperationDelegate";
        private static string s_endOperationDelegateTypeName = "EndOperationDelegate";
        private static string s_getDefaultValueForInitializationMethodName = "GetDefaultValueForInitialization";

        // IMPORTANT: this table tracks the set of .ctors in ClientBase and DuplexClientBase. 
        // This table must be kept in sync
        // for DuplexClientBase, the initial InstanceContext param is assumed; ctor overloads must match between ClientBase and DuplexClientBase
        private static Type[][] s_clientCtorParamTypes = new Type[][]
            {
                new Type[] { },
                new Type[] { s_stringType, },
                new Type[] { s_stringType, s_stringType, },
                new Type[] { s_stringType, s_endpointAddressType, },
                new Type[] { s_bindingType, s_endpointAddressType, },
            };

        private static string[][] s_clientCtorParamNames = new string[][]
            {
                new string[] { },
                new string[] { "endpointConfigurationName", },
                new string[] { "endpointConfigurationName", "remoteAddress", },
                new string[] { "endpointConfigurationName", "remoteAddress", },
                new string[] { "binding", "remoteAddress", },
            };

        private static Type[] s_eventArgsCtorParamTypes = new Type[]
        {
            s_objectArrayType,
            s_exceptionType,
            s_boolType,
            s_objectType
        };

        private static string[] s_eventArgsCtorParamNames = new string[]
        {
            "results",
            "exception",
            "cancelled",
            "userState"
        };

        private static string[] s_eventArgsPropertyNames = new string[]
        {
            "Results",
            "Error",
            "Cancelled",
            "UserState"
        };

#if DEBUG
        private static BindingFlags s_ctorBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private static string s_debugCheckTable_errorString = "Client code generation table out of sync with ClientBase and DuplexClientBase ctors. Please investigate.";

        // check the table against what we would get from reflection
        private static void DebugCheckTable()
        {
            Fx.Assert(s_clientCtorParamNames.Length == s_clientCtorParamTypes.Length, s_debugCheckTable_errorString);

            for (int i = 0; i < s_clientCtorParamTypes.Length; i++)
            {
                DebugCheckTable_ValidateCtor(s_clientBaseType.GetConstructor(s_ctorBindingFlags, null, s_clientCtorParamTypes[i], null), s_clientCtorParamNames[i]);

                Type[] duplexCtorTypes1 = DebugCheckTable_InsertAtStart(s_clientCtorParamTypes[i], s_objectType);
                Type[] duplexCtorTypes2 = DebugCheckTable_InsertAtStart(s_clientCtorParamTypes[i], s_instanceContextType);
                string[] duplexCtorNames = DebugCheckTable_InsertAtStart(s_clientCtorParamNames[i], s_inputInstanceName);

                DebugCheckTable_ValidateCtor(s_duplexClientBaseType.GetConstructor(s_ctorBindingFlags, null, duplexCtorTypes1, null), duplexCtorNames);
                DebugCheckTable_ValidateCtor(s_duplexClientBaseType.GetConstructor(s_ctorBindingFlags, null, duplexCtorTypes2, null), duplexCtorNames);
            }

            // ClientBase<> has extra InstanceContext overloads that we do not call directly from the generated code, but which we 
            // need to account for in this assert
            Fx.Assert(s_clientBaseType.GetConstructors(s_ctorBindingFlags).Length == s_clientCtorParamTypes.Length * 2, s_debugCheckTable_errorString);

            // DuplexClientBase<> also has extra object/InstanceContext overloads (but we call these)
            Fx.Assert(s_duplexClientBaseType.GetConstructors(s_ctorBindingFlags).Length == s_clientCtorParamTypes.Length * 2, s_debugCheckTable_errorString);
        }

        private static T[] DebugCheckTable_InsertAtStart<T>(T[] arr, T item)
        {
            T[] newArr = new T[arr.Length + 1];
            newArr[0] = item;
            Array.Copy(arr, 0, newArr, 1, arr.Length);
            return newArr;
        }

        private static void DebugCheckTable_ValidateCtor(ConstructorInfo ctor, string[] paramNames)
        {
            Fx.Assert(ctor != null, s_debugCheckTable_errorString);

            ParameterInfo[] parameters = ctor.GetParameters();
            Fx.Assert(parameters.Length == paramNames.Length, s_debugCheckTable_errorString);
            for (int i = 0; i < paramNames.Length; i++)
            {
                Fx.Assert(parameters[i].Name == paramNames[i], s_debugCheckTable_errorString);
            }
        }
#endif

        void IServiceContractGenerationExtension.GenerateContract(ServiceContractGenerationContext context)
        {
#if DEBUG
            // DebugCheckTable();
#endif
            CodeTypeDeclaration clientType = context.TypeFactory.CreateClassType();
            // Have to make sure that client name does not match any methods: member names can not be the same as their enclosing type (CSharp only)
            clientType.Name = NamingHelper.GetUniqueName(GetClientClassName(context.ContractType.Name), DoesMethodNameExist, context.Operations);
            CodeTypeReference contractTypeRef = context.ContractTypeReference;
            if (context.DuplexCallbackType == null)
                clientType.BaseTypes.Add(new CodeTypeReference(context.ServiceContractGenerator.GetCodeTypeReference(typeof(ClientBase<>)).BaseType, context.ContractTypeReference));
            else
                clientType.BaseTypes.Add(new CodeTypeReference(context.ServiceContractGenerator.GetCodeTypeReference(typeof(DuplexClientBase<>)).BaseType, context.ContractTypeReference));

            clientType.BaseTypes.Add(context.ContractTypeReference);

            if (!(s_clientCtorParamNames.Length == s_clientCtorParamTypes.Length))
            {
                Fx.Assert("Invalid client generation constructor table initialization");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Invalid client generation constructor table initialization")));
            }

            for (int i = 0; i < s_clientCtorParamNames.Length; i++)
            {
                if (!(s_clientCtorParamNames[i].Length == s_clientCtorParamTypes[i].Length))
                {
                    Fx.Assert("Invalid client generation constructor table initialization");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Invalid client generation constructor table initialization")));
                }

                CodeConstructor ctor = new CodeConstructor();
                ctor.Attributes = MemberAttributes.Public;
                if (context.DuplexCallbackType != null)
                {
                    ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(InstanceContext), s_inputInstanceName));
                    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(s_inputInstanceName));
                }
                for (int j = 0; j < s_clientCtorParamNames[i].Length; j++)
                {
                    ctor.Parameters.Add(new CodeParameterDeclarationExpression(s_clientCtorParamTypes[i][j], s_clientCtorParamNames[i][j]));
                    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(s_clientCtorParamNames[i][j]));
                }
                clientType.Members.Add(ctor);
            }

            foreach (OperationContractGenerationContext operationContext in context.Operations)
            {
                // Note that we generate all the client-side methods, even inherited ones.
                if (operationContext.Operation.IsServerInitiated()) continue;
                CodeTypeReference declaringContractTypeRef = operationContext.DeclaringTypeReference;
                GenerateClientClassMethod(clientType, contractTypeRef, operationContext.SyncMethod, _tryAddHelperMethod, declaringContractTypeRef);

                if (operationContext.IsAsync)
                {
                    CodeMemberMethod beginMethod = GenerateClientClassMethod(clientType, contractTypeRef, operationContext.BeginMethod, _tryAddHelperMethod, declaringContractTypeRef);
                    CodeMemberMethod endMethod = GenerateClientClassMethod(clientType, contractTypeRef, operationContext.EndMethod, _tryAddHelperMethod, declaringContractTypeRef);

                    if (_generateEventAsyncMethods)
                    {
                        GenerateEventAsyncMethods(context, clientType, operationContext.SyncMethod.Name, beginMethod, endMethod);
                    }
                }

                if (operationContext.IsTask)
                {
                    GenerateClientClassMethod(clientType, contractTypeRef, operationContext.TaskMethod, !operationContext.Operation.HasOutputParameters && _tryAddHelperMethod, declaringContractTypeRef);
                }
            }

            context.Namespace.Types.Add(clientType);
            context.ClientType = clientType;
            context.ClientTypeReference = ServiceContractGenerator.NamespaceHelper.GetCodeTypeReference(context.Namespace, clientType);
        }

        private static CodeMemberMethod GenerateClientClassMethod(CodeTypeDeclaration clientType, CodeTypeReference contractTypeRef, CodeMemberMethod method, bool addHelperMethod, CodeTypeReference declaringContractTypeRef)
        {
            CodeMemberMethod methodImpl = GetImplementationOfMethod(contractTypeRef, method);
            AddMethodImpl(methodImpl);
            int methodPosition = clientType.Members.Add(methodImpl);
            CodeMemberMethod helperMethod = null;

            if (addHelperMethod)
            {
                helperMethod = GenerateHelperMethod(declaringContractTypeRef, methodImpl);
                if (helperMethod != null)
                {
                    clientType.Members[methodPosition].CustomAttributes.Add(CreateEditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    clientType.Members.Add(helperMethod);
                }
            }

            return (helperMethod != null) ? helperMethod : methodImpl;
        }

        internal static CodeAttributeDeclaration CreateEditorBrowsableAttribute(EditorBrowsableState editorBrowsableState)
        {
            CodeAttributeDeclaration browsableAttribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(EditorBrowsableAttribute)));
            CodeTypeReferenceExpression browsableAttributeState = new CodeTypeReferenceExpression(typeof(EditorBrowsableState));
            CodeAttributeArgument browsableAttributeValue = new CodeAttributeArgument(new CodeFieldReferenceExpression(browsableAttributeState, editorBrowsableState.ToString()));
            browsableAttribute.Arguments.Add(browsableAttributeValue);

            return browsableAttribute;
        }

        private static CodeMemberMethod GenerateHelperMethod(CodeTypeReference ifaceType, CodeMemberMethod method)
        {
            CodeMemberMethod helperMethod = new CodeMemberMethod();
            helperMethod.Name = method.Name;
            helperMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            CodeMethodInvokeExpression invokeMethod = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeCastExpression(ifaceType, new CodeThisReferenceExpression()), method.Name));
            bool hasTypedMessage = false;
            foreach (CodeParameterDeclarationExpression param in method.Parameters)
            {
                CodeTypeDeclaration paramTypeDecl = ServiceContractGenerator.NamespaceHelper.GetCodeType(param.Type);
                if (paramTypeDecl != null)
                {
                    hasTypedMessage = true;
                    CodeVariableReferenceExpression inValue = new CodeVariableReferenceExpression("inValue");
                    helperMethod.Statements.Add(new CodeVariableDeclarationStatement(param.Type, inValue.VariableName, new CodeObjectCreateExpression(param.Type)));
                    invokeMethod.Parameters.Add(inValue);
                    GenerateParameters(helperMethod, paramTypeDecl, inValue, FieldDirection.In);
                }
                else
                {
                    helperMethod.Parameters.Add(new CodeParameterDeclarationExpression(param.Type, param.Name));
                    invokeMethod.Parameters.Add(new CodeArgumentReferenceExpression(param.Name));
                }
            }
            if (method.ReturnType.BaseType == s_voidTypeRef.BaseType)
                helperMethod.Statements.Add(invokeMethod);
            else
            {
                CodeTypeDeclaration returnTypeDecl = ServiceContractGenerator.NamespaceHelper.GetCodeType(method.ReturnType);
                if (returnTypeDecl != null)
                {
                    hasTypedMessage = true;
                    CodeVariableReferenceExpression outVar = new CodeVariableReferenceExpression("retVal");

                    helperMethod.Statements.Add(new CodeVariableDeclarationStatement(method.ReturnType, outVar.VariableName, invokeMethod));
                    CodeMethodReturnStatement returnStatement = GenerateParameters(helperMethod, returnTypeDecl, outVar, FieldDirection.Out);
                    if (returnStatement != null)
                        helperMethod.Statements.Add(returnStatement);
                }
                else
                {
                    helperMethod.Statements.Add(new CodeMethodReturnStatement(invokeMethod));
                    helperMethod.ReturnType = method.ReturnType;
                }
            }
            if (hasTypedMessage)
                method.PrivateImplementationType = ifaceType;
            return hasTypedMessage ? helperMethod : null;
        }

        private static CodeMethodReturnStatement GenerateParameters(CodeMemberMethod helperMethod, CodeTypeDeclaration codeTypeDeclaration, CodeExpression target, FieldDirection dir)
        {
            CodeMethodReturnStatement returnStatement = null;
            foreach (CodeTypeMember member in codeTypeDeclaration.Members)
            {
                CodeMemberField field = member as CodeMemberField;
                if (field == null)
                    continue;
                CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(target, field.Name);
                CodeTypeDeclaration bodyTypeDecl = ServiceContractGenerator.NamespaceHelper.GetCodeType(field.Type);
                if (bodyTypeDecl != null)
                {
                    if (dir == FieldDirection.In)
                        helperMethod.Statements.Add(new CodeAssignStatement(fieldRef, new CodeObjectCreateExpression(field.Type)));
                    returnStatement = GenerateParameters(helperMethod, bodyTypeDecl, fieldRef, dir);
                    continue;
                }
                CodeParameterDeclarationExpression param = GetRefParameter(helperMethod.Parameters, dir, field);
                if (param == null && dir == FieldDirection.Out && helperMethod.ReturnType.BaseType == s_voidTypeRef.BaseType)
                {
                    helperMethod.ReturnType = field.Type;
                    returnStatement = new CodeMethodReturnStatement(fieldRef);
                }
                else
                {
                    if (param == null)
                    {
                        param = new CodeParameterDeclarationExpression(field.Type, NamingHelper.GetUniqueName(field.Name, DoesParameterNameExist, helperMethod));
                        param.Direction = dir;
                        helperMethod.Parameters.Add(param);
                    }
                    if (dir == FieldDirection.Out)
                        helperMethod.Statements.Add(new CodeAssignStatement(new CodeArgumentReferenceExpression(param.Name), fieldRef));
                    else
                        helperMethod.Statements.Add(new CodeAssignStatement(fieldRef, new CodeArgumentReferenceExpression(param.Name)));
                }
            }
            return returnStatement;
        }

        private static CodeParameterDeclarationExpression GetRefParameter(CodeParameterDeclarationExpressionCollection parameters, FieldDirection dir, CodeMemberField field)
        {
            foreach (CodeParameterDeclarationExpression p in parameters)
            {
                if (p.Name == field.Name)
                {
                    if (p.Direction != dir && p.Type.BaseType == field.Type.BaseType)
                    {
                        p.Direction = FieldDirection.Ref;
                        return p;
                    }
                    return null;
                }
            }
            return null;
        }

        internal static bool DoesMemberNameExist(string name, object typeDeclarationObject)
        {
            CodeTypeDeclaration typeDeclaration = (CodeTypeDeclaration)typeDeclarationObject;

            if (string.Compare(typeDeclaration.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            foreach (CodeTypeMember member in typeDeclaration.Members)
            {
                if (string.Compare(member.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool DoesTypeNameExists(string name, object codeTypeDeclarationCollectionObject)
        {
            CodeTypeDeclarationCollection codeTypeDeclarations = (CodeTypeDeclarationCollection)codeTypeDeclarationCollectionObject;
            foreach (CodeTypeDeclaration codeTypeDeclaration in codeTypeDeclarations)
            {
                if (string.Compare(codeTypeDeclaration.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool DoesTypeAndMemberNameExist(string name, object nameCollection)
        {
            object[] nameCollections = (object[])nameCollection;

            if (DoesTypeNameExists(name, nameCollections[0]))
            {
                return true;
            }
            if (DoesMemberNameExist(name, nameCollections[1]))
            {
                return true;
            }

            return false;
        }

        internal static bool DoesMethodNameExist(string name, object operationsObject)
        {
            Collection<OperationContractGenerationContext> operations = (Collection<OperationContractGenerationContext>)operationsObject;
            foreach (OperationContractGenerationContext operationContext in operations)
            {
                if (String.Compare(operationContext.SyncMethod.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
                if (operationContext.IsAsync)
                {
                    if (String.Compare(operationContext.BeginMethod.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                    if (String.Compare(operationContext.EndMethod.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }
                if (operationContext.IsTask)
                {
                    if (String.Compare(operationContext.TaskMethod.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }
            }
            return false;
        }

        internal static bool DoesParameterNameExist(string name, object methodObject)
        {
            CodeMemberMethod method = (CodeMemberMethod)methodObject;
            if (String.Compare(method.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
            CodeParameterDeclarationExpressionCollection parameters = method.Parameters;
            foreach (CodeParameterDeclarationExpression p in parameters)
            {
                if (String.Compare(p.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }
            return false;
        }

        private static void AddMethodImpl(CodeMemberMethod method)
        {
            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(GetChannelReference(), method.Name);
            foreach (CodeParameterDeclarationExpression parameter in method.Parameters)
            {
                methodInvoke.Parameters.Add(new CodeDirectionExpression(parameter.Direction, new CodeVariableReferenceExpression(parameter.Name)));
            }
            if (IsVoid(method))
                method.Statements.Add(methodInvoke);
            else
                method.Statements.Add(new CodeMethodReturnStatement(methodInvoke));
        }

        private static CodeMemberMethod GetImplementationOfMethod(CodeTypeReference ifaceType, CodeMemberMethod method)
        {
            CodeMemberMethod m = new CodeMemberMethod();
            m.Name = method.Name;
            m.ImplementationTypes.Add(ifaceType);
            m.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            foreach (CodeParameterDeclarationExpression parameter in method.Parameters)
            {
                CodeParameterDeclarationExpression newParam = new CodeParameterDeclarationExpression(parameter.Type, parameter.Name);
                newParam.Direction = parameter.Direction;
                m.Parameters.Add(newParam);
            }
            m.ReturnType = method.ReturnType;
            return m;
        }

        private static void GenerateEventAsyncMethods(ServiceContractGenerationContext context, CodeTypeDeclaration clientType,
            string syncMethodName, CodeMemberMethod beginMethod, CodeMemberMethod endMethod)
        {
            CodeTypeDeclaration operationCompletedEventArgsType = CreateOperationCompletedEventArgsType(context, syncMethodName, endMethod);
            CodeMemberEvent operationCompletedEvent = CreateOperationCompletedEvent(context, clientType, syncMethodName, operationCompletedEventArgsType);

            CodeMemberField beginOperationDelegate = CreateBeginOperationDelegate(context, clientType, syncMethodName);
            CodeMemberMethod beginOperationMethod = CreateBeginOperationMethod(context, clientType, syncMethodName, beginMethod);

            CodeMemberField endOperationDelegate = CreateEndOperationDelegate(context, clientType, syncMethodName);
            CodeMemberMethod endOperationMethod = CreateEndOperationMethod(context, clientType, syncMethodName, endMethod);

            CodeMemberField operationCompletedDelegate = CreateOperationCompletedDelegate(context, clientType, syncMethodName);
            CodeMemberMethod operationCompletedMethod = CreateOperationCompletedMethod(context, clientType, syncMethodName, operationCompletedEventArgsType, operationCompletedEvent);

            CodeMemberMethod eventAsyncMethod = CreateEventAsyncMethod(context, clientType, syncMethodName, beginMethod,
                beginOperationDelegate, beginOperationMethod, endOperationDelegate, endOperationMethod, operationCompletedDelegate, operationCompletedMethod);

            CreateEventAsyncMethodOverload(clientType, eventAsyncMethod);

            // hide the normal async methods from intellisense
            beginMethod.CustomAttributes.Add(CreateEditorBrowsableAttribute(EditorBrowsableState.Advanced));
            endMethod.CustomAttributes.Add(CreateEditorBrowsableAttribute(EditorBrowsableState.Advanced));
        }

        private static CodeTypeDeclaration CreateOperationCompletedEventArgsType(ServiceContractGenerationContext context,
            string syncMethodName, CodeMemberMethod endMethod)
        {
            if ((endMethod.Parameters.Count == 1) && (endMethod.ReturnType.BaseType == s_voidTypeRef.BaseType))
            {
                // no need to create new event args type, use AsyncCompletedEventArgs
                return null;
            }

            CodeTypeDeclaration argsType = context.TypeFactory.CreateClassType();
            argsType.BaseTypes.Add(new CodeTypeReference(s_asyncCompletedEventArgsType));

            // define object[] results field.
            CodeMemberField resultsField = new CodeMemberField();
            resultsField.Type = new CodeTypeReference(s_objectArrayType);

            CodeFieldReferenceExpression resultsFieldReference = new CodeFieldReferenceExpression();
            resultsFieldReference.TargetObject = new CodeThisReferenceExpression();

            // create constructor, that assigns the results field.
            CodeConstructor ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            for (int i = 0; i < s_eventArgsCtorParamTypes.Length; i++)
            {
                ctor.Parameters.Add(new CodeParameterDeclarationExpression(s_eventArgsCtorParamTypes[i], s_eventArgsCtorParamNames[i]));
                if (i > 0)
                {
                    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(s_eventArgsCtorParamNames[i]));
                }
            }
            argsType.Members.Add(ctor);
            ctor.Statements.Add(new CodeAssignStatement(resultsFieldReference, new CodeVariableReferenceExpression(s_eventArgsCtorParamNames[0])));

            // create properties for the out parameters
            int asyncResultParamIndex = GetAsyncResultParamIndex(endMethod);
            int count = 0;
            for (int i = 0; i < endMethod.Parameters.Count; i++)
            {
                if (i != asyncResultParamIndex)
                {
                    CreateEventAsyncCompletedArgsTypeProperty(argsType,
                        endMethod.Parameters[i].Type,
                        endMethod.Parameters[i].Name,
                        new CodeArrayIndexerExpression(resultsFieldReference, new CodePrimitiveExpression(count++)));
                }
            }

            // create the property for the return type
            if (endMethod.ReturnType.BaseType != s_voidTypeRef.BaseType)
            {
                CreateEventAsyncCompletedArgsTypeProperty(
                    argsType,
                    endMethod.ReturnType,
                    NamingHelper.GetUniqueName("Result", DoesMemberNameExist, argsType),
                    new CodeArrayIndexerExpression(resultsFieldReference,
                        new CodePrimitiveExpression(count)));
            }

            // Name the "results" field after generating the properties to make sure it does 
            // not conflict with the property names.
            resultsField.Name = NamingHelper.GetUniqueName("results", DoesMemberNameExist, argsType);
            resultsFieldReference.FieldName = resultsField.Name;
            argsType.Members.Add(resultsField);

            // Name the type making sure that it does not conflict with its members and types already present in
            // the namespace. 
            argsType.Name = NamingHelper.GetUniqueName(GetOperationCompletedEventArgsTypeName(syncMethodName),
                DoesTypeAndMemberNameExist, new object[] { context.Namespace.Types, argsType });
            context.Namespace.Types.Add(argsType);

            return argsType;
        }

        private static int GetAsyncResultParamIndex(CodeMemberMethod endMethod)
        {
            int index = endMethod.Parameters.Count - 1;
            if (endMethod.Parameters[index].Type.BaseType != s_asyncResultTypeRef.BaseType)
            {
                // workaround for CSD Dev Framework:10826, the unwrapped end method has IAsyncResult as first param. 
                index = 0;
            }

            return index;
        }

        private static CodeMemberProperty CreateEventAsyncCompletedArgsTypeProperty(CodeTypeDeclaration ownerTypeDecl,
            CodeTypeReference propertyType, string propertyName, CodeExpression propertyValueExpr)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            property.Type = propertyType;
            property.Name = propertyName;
            property.HasSet = false;
            property.HasGet = true;

            CodeCastExpression castExpr = new CodeCastExpression(propertyType, propertyValueExpr);
            CodeMethodReturnStatement returnStmt = new CodeMethodReturnStatement(castExpr);

            property.GetStatements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), s_raiseExceptionIfNecessaryMethodName));
            property.GetStatements.Add(returnStmt);
            ownerTypeDecl.Members.Add(property);

            return property;
        }

        private static CodeMemberEvent CreateOperationCompletedEvent(ServiceContractGenerationContext context,
            CodeTypeDeclaration clientType, string syncMethodName, CodeTypeDeclaration operationCompletedEventArgsType)
        {
            CodeMemberEvent operationCompletedEvent = new CodeMemberEvent();
            operationCompletedEvent.Attributes = MemberAttributes.Public;
            operationCompletedEvent.Type = new CodeTypeReference(s_eventHandlerType);

            if (operationCompletedEventArgsType == null)
            {
                operationCompletedEvent.Type.TypeArguments.Add(s_asyncCompletedEventArgsType);
            }
            else
            {
                operationCompletedEvent.Type.TypeArguments.Add(operationCompletedEventArgsType.Name);
            }

            operationCompletedEvent.Name = NamingHelper.GetUniqueName(GetOperationCompletedEventName(syncMethodName),
                DoesMethodNameExist, context.Operations);

            clientType.Members.Add(operationCompletedEvent);
            return operationCompletedEvent;
        }

        private static CodeMemberField CreateBeginOperationDelegate(ServiceContractGenerationContext context,
            CodeTypeDeclaration clientType, string syncMethodName)
        {
            CodeMemberField beginOperationDelegate = new CodeMemberField();
            beginOperationDelegate.Attributes = MemberAttributes.Private;
            beginOperationDelegate.Type = new CodeTypeReference(s_beginOperationDelegateTypeName);
            beginOperationDelegate.Name = NamingHelper.GetUniqueName(GetBeginOperationDelegateName(syncMethodName),
                DoesMethodNameExist, context.Operations);

            clientType.Members.Add(beginOperationDelegate);
            return beginOperationDelegate;
        }

        private static CodeMemberMethod CreateBeginOperationMethod(ServiceContractGenerationContext context, CodeTypeDeclaration clientType,
            string syncMethodName, CodeMemberMethod beginMethod)
        {
            CodeMemberMethod onBeginOperationMethod = new CodeMemberMethod();
            onBeginOperationMethod.Attributes = MemberAttributes.Private;
            onBeginOperationMethod.ReturnType = new CodeTypeReference(s_asyncResultType);
            onBeginOperationMethod.Name = NamingHelper.GetUniqueName(GetBeginOperationMethodName(syncMethodName),
                DoesMethodNameExist, context.Operations);

            CodeParameterDeclarationExpression inValuesParam = new CodeParameterDeclarationExpression();
            inValuesParam.Type = new CodeTypeReference(s_objectArrayType);
            inValuesParam.Name = NamingHelper.GetUniqueName("inValues", DoesParameterNameExist, beginMethod);
            onBeginOperationMethod.Parameters.Add(inValuesParam);

            CodeMethodInvokeExpression invokeBegin = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), beginMethod.Name);
            CodeExpression inValuesRef = new CodeVariableReferenceExpression(inValuesParam.Name);

            for (int i = 0; i < beginMethod.Parameters.Count - 2; i++)
            {
                CodeVariableDeclarationStatement variableDecl = new CodeVariableDeclarationStatement();
                variableDecl.Type = beginMethod.Parameters[i].Type;
                variableDecl.Name = beginMethod.Parameters[i].Name;
                variableDecl.InitExpression = new CodeCastExpression(variableDecl.Type,
                    new CodeArrayIndexerExpression(inValuesRef, new CodePrimitiveExpression(i)));

                onBeginOperationMethod.Statements.Add(variableDecl);
                invokeBegin.Parameters.Add(new CodeDirectionExpression(beginMethod.Parameters[i].Direction,
                    new CodeVariableReferenceExpression(variableDecl.Name)));
            }

            for (int i = beginMethod.Parameters.Count - 2; i < beginMethod.Parameters.Count; i++)
            {
                onBeginOperationMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                    beginMethod.Parameters[i].Type, beginMethod.Parameters[i].Name));
                invokeBegin.Parameters.Add(new CodeVariableReferenceExpression(beginMethod.Parameters[i].Name));
            }

            onBeginOperationMethod.Statements.Add(new CodeMethodReturnStatement(invokeBegin));
            clientType.Members.Add(onBeginOperationMethod);
            return onBeginOperationMethod;
        }

        private static CodeMemberField CreateEndOperationDelegate(ServiceContractGenerationContext context,
            CodeTypeDeclaration clientType, string syncMethodName)
        {
            CodeMemberField endOperationDelegate = new CodeMemberField();
            endOperationDelegate.Attributes = MemberAttributes.Private;
            endOperationDelegate.Type = new CodeTypeReference(s_endOperationDelegateTypeName);
            endOperationDelegate.Name = NamingHelper.GetUniqueName(GetEndOperationDelegateName(syncMethodName),
                DoesMethodNameExist, context.Operations);

            clientType.Members.Add(endOperationDelegate);
            return endOperationDelegate;
        }

        private static CodeMemberMethod CreateEndOperationMethod(ServiceContractGenerationContext context, CodeTypeDeclaration clientType, string syncMethodName, CodeMemberMethod endMethod)
        {
            CodeMemberMethod onEndOperationMethod = new CodeMemberMethod();
            onEndOperationMethod.Attributes = MemberAttributes.Private;
            onEndOperationMethod.ReturnType = new CodeTypeReference(s_objectArrayType);
            onEndOperationMethod.Name = NamingHelper.GetUniqueName(GetEndOperationMethodName(syncMethodName), DoesMethodNameExist, context.Operations);

            int asyncResultParamIndex = GetAsyncResultParamIndex(endMethod);
            CodeMethodInvokeExpression invokeEnd = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), endMethod.Name);
            CodeArrayCreateExpression retArray = new CodeArrayCreateExpression();
            retArray.CreateType = new CodeTypeReference(s_objectArrayType);
            for (int i = 0; i < endMethod.Parameters.Count; i++)
            {
                if (i == asyncResultParamIndex)
                {
                    onEndOperationMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                        endMethod.Parameters[i].Type, endMethod.Parameters[i].Name));
                    invokeEnd.Parameters.Add(new CodeVariableReferenceExpression(endMethod.Parameters[i].Name));
                }
                else
                {
                    CodeVariableDeclarationStatement variableDecl = new CodeVariableDeclarationStatement(
                        endMethod.Parameters[i].Type, endMethod.Parameters[i].Name);
                    CodeMethodReferenceExpression getDefaultValueMethodRef = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), s_getDefaultValueForInitializationMethodName, endMethod.Parameters[i].Type);
                    variableDecl.InitExpression = new CodeMethodInvokeExpression(getDefaultValueMethodRef);
                    onEndOperationMethod.Statements.Add(variableDecl);

                    invokeEnd.Parameters.Add(new CodeDirectionExpression(endMethod.Parameters[i].Direction,
                            new CodeVariableReferenceExpression(variableDecl.Name)));

                    retArray.Initializers.Add(new CodeVariableReferenceExpression(variableDecl.Name));
                }
            }

            if (endMethod.ReturnType.BaseType != s_voidTypeRef.BaseType)
            {
                CodeVariableDeclarationStatement retValDecl = new CodeVariableDeclarationStatement();
                retValDecl.Type = endMethod.ReturnType;
                retValDecl.Name = NamingHelper.GetUniqueName("retVal", DoesParameterNameExist, endMethod);
                retValDecl.InitExpression = invokeEnd;
                retArray.Initializers.Add(new CodeVariableReferenceExpression(retValDecl.Name));

                onEndOperationMethod.Statements.Add(retValDecl);
            }
            else
            {
                onEndOperationMethod.Statements.Add(invokeEnd);
            }

            if (retArray.Initializers.Count > 0)
            {
                onEndOperationMethod.Statements.Add(new CodeMethodReturnStatement(retArray));
            }
            else
            {
                onEndOperationMethod.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
            }

            clientType.Members.Add(onEndOperationMethod);
            return onEndOperationMethod;
        }

        private static CodeMemberField CreateOperationCompletedDelegate(ServiceContractGenerationContext context,
            CodeTypeDeclaration clientType, string syncMethodName)
        {
            CodeMemberField operationCompletedDelegate = new CodeMemberField();
            operationCompletedDelegate.Attributes = MemberAttributes.Private;
            operationCompletedDelegate.Type = new CodeTypeReference(s_sendOrPostCallbackType);
            operationCompletedDelegate.Name = NamingHelper.GetUniqueName(GetOperationCompletedDelegateName(syncMethodName),
                DoesMethodNameExist, context.Operations);

            clientType.Members.Add(operationCompletedDelegate);
            return operationCompletedDelegate;
        }

        private static CodeMemberMethod CreateOperationCompletedMethod(ServiceContractGenerationContext context, CodeTypeDeclaration clientType,
            string syncMethodName, CodeTypeDeclaration operationCompletedEventArgsType, CodeMemberEvent operationCompletedEvent)
        {
            CodeMemberMethod operationCompletedMethod = new CodeMemberMethod();
            operationCompletedMethod.Attributes = MemberAttributes.Private;
            operationCompletedMethod.Name = NamingHelper.GetUniqueName(GetOperationCompletedMethodName(syncMethodName),
                DoesMethodNameExist, context.Operations);

            operationCompletedMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(s_objectType), "state"));
            operationCompletedMethod.ReturnType = new CodeTypeReference(s_voidType);

            CodeVariableDeclarationStatement eventArgsDecl =
                new CodeVariableDeclarationStatement(s_invokeAsyncCompletedEventArgsTypeName, "e");

            eventArgsDecl.InitExpression = new CodeCastExpression(s_invokeAsyncCompletedEventArgsTypeName,
                new CodeArgumentReferenceExpression(operationCompletedMethod.Parameters[0].Name));

            CodeObjectCreateExpression newEventArgsExpr;
            CodeVariableReferenceExpression eventArgsRef = new CodeVariableReferenceExpression(eventArgsDecl.Name);
            if (operationCompletedEventArgsType != null)
            {
                newEventArgsExpr = new CodeObjectCreateExpression(operationCompletedEventArgsType.Name,
                    new CodePropertyReferenceExpression(eventArgsRef, s_eventArgsPropertyNames[0]),
                    new CodePropertyReferenceExpression(eventArgsRef, s_eventArgsPropertyNames[1]),
                    new CodePropertyReferenceExpression(eventArgsRef, s_eventArgsPropertyNames[2]),
                    new CodePropertyReferenceExpression(eventArgsRef, s_eventArgsPropertyNames[3]));
            }
            else
            {
                newEventArgsExpr = new CodeObjectCreateExpression(s_asyncCompletedEventArgsType,
                    new CodePropertyReferenceExpression(eventArgsRef, s_eventArgsPropertyNames[1]),
                    new CodePropertyReferenceExpression(eventArgsRef, s_eventArgsPropertyNames[2]),
                    new CodePropertyReferenceExpression(eventArgsRef, s_eventArgsPropertyNames[3]));
            }

            CodeEventReferenceExpression completedEvent = new CodeEventReferenceExpression(new CodeThisReferenceExpression(), operationCompletedEvent.Name);

            CodeDelegateInvokeExpression raiseEventExpr = new CodeDelegateInvokeExpression(
                completedEvent,
                new CodeThisReferenceExpression(),
                newEventArgsExpr);

            CodeConditionStatement ifEventHandlerNotNullBlock = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    completedEvent,
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePrimitiveExpression(null)),
                eventArgsDecl,
                new CodeExpressionStatement(raiseEventExpr));

            operationCompletedMethod.Statements.Add(ifEventHandlerNotNullBlock);

            clientType.Members.Add(operationCompletedMethod);
            return operationCompletedMethod;
        }

        private static CodeMemberMethod CreateEventAsyncMethod(ServiceContractGenerationContext context, CodeTypeDeclaration clientType,
            string syncMethodName, CodeMemberMethod beginMethod,
            CodeMemberField beginOperationDelegate, CodeMemberMethod beginOperationMethod,
            CodeMemberField endOperationDelegate, CodeMemberMethod endOperationMethod,
            CodeMemberField operationCompletedDelegate, CodeMemberMethod operationCompletedMethod)
        {
            CodeMemberMethod eventAsyncMethod = new CodeMemberMethod();
            eventAsyncMethod.Name = NamingHelper.GetUniqueName(GetEventAsyncMethodName(syncMethodName),
                DoesMethodNameExist, context.Operations);
            eventAsyncMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            eventAsyncMethod.ReturnType = new CodeTypeReference(s_voidType);

            CodeArrayCreateExpression invokeAsyncInValues = new CodeArrayCreateExpression(new CodeTypeReference(s_objectArrayType));
            for (int i = 0; i < beginMethod.Parameters.Count - 2; i++)
            {
                CodeParameterDeclarationExpression beginMethodParameter = beginMethod.Parameters[i];
                CodeParameterDeclarationExpression eventAsyncMethodParameter = new CodeParameterDeclarationExpression(
                    beginMethodParameter.Type, beginMethodParameter.Name);

                eventAsyncMethodParameter.Direction = FieldDirection.In;
                eventAsyncMethod.Parameters.Add(eventAsyncMethodParameter);
                invokeAsyncInValues.Initializers.Add(new CodeVariableReferenceExpression(eventAsyncMethodParameter.Name));
            }

            string userStateParamName = NamingHelper.GetUniqueName("userState", DoesParameterNameExist, eventAsyncMethod);
            eventAsyncMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(s_objectType), userStateParamName));

            eventAsyncMethod.Statements.Add(CreateDelegateIfNotNull(beginOperationDelegate, beginOperationMethod));
            eventAsyncMethod.Statements.Add(CreateDelegateIfNotNull(endOperationDelegate, endOperationMethod));
            eventAsyncMethod.Statements.Add(CreateDelegateIfNotNull(operationCompletedDelegate, operationCompletedMethod));

            CodeMethodInvokeExpression invokeAsync = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), s_invokeAsyncMethodName);
            invokeAsync.Parameters.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), beginOperationDelegate.Name));
            if (invokeAsyncInValues.Initializers.Count > 0)
            {
                invokeAsync.Parameters.Add(invokeAsyncInValues);
            }
            else
            {
                invokeAsync.Parameters.Add(new CodePrimitiveExpression(null));
            }
            invokeAsync.Parameters.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), endOperationDelegate.Name));
            invokeAsync.Parameters.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), operationCompletedDelegate.Name));
            invokeAsync.Parameters.Add(new CodeVariableReferenceExpression(userStateParamName));

            eventAsyncMethod.Statements.Add(new CodeExpressionStatement(invokeAsync));

            clientType.Members.Add(eventAsyncMethod);
            return eventAsyncMethod;
        }

        private static CodeMemberMethod CreateEventAsyncMethodOverload(CodeTypeDeclaration clientType, CodeMemberMethod eventAsyncMethod)
        {
            CodeMemberMethod eventAsyncMethodOverload = new CodeMemberMethod();
            eventAsyncMethodOverload.Attributes = eventAsyncMethod.Attributes;
            eventAsyncMethodOverload.Name = eventAsyncMethod.Name;
            eventAsyncMethodOverload.ReturnType = eventAsyncMethod.ReturnType;

            CodeMethodInvokeExpression invokeEventAsyncMethod = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), eventAsyncMethod.Name);

            for (int i = 0; i < eventAsyncMethod.Parameters.Count - 1; i++)
            {
                eventAsyncMethodOverload.Parameters.Add(new CodeParameterDeclarationExpression(
                    eventAsyncMethod.Parameters[i].Type,
                    eventAsyncMethod.Parameters[i].Name));

                invokeEventAsyncMethod.Parameters.Add(new CodeVariableReferenceExpression(
                    eventAsyncMethod.Parameters[i].Name));
            }
            invokeEventAsyncMethod.Parameters.Add(new CodePrimitiveExpression(null));

            eventAsyncMethodOverload.Statements.Add(invokeEventAsyncMethod);

            int eventAsyncMethodPosition = clientType.Members.IndexOf(eventAsyncMethod);
            Fx.Assert(eventAsyncMethodPosition != -1,
                "The eventAsyncMethod must be added to the clientType before calling CreateEventAsyncMethodOverload");

            clientType.Members.Insert(eventAsyncMethodPosition, eventAsyncMethodOverload);
            return eventAsyncMethodOverload;
        }

        private static CodeStatement CreateDelegateIfNotNull(CodeMemberField delegateField, CodeMemberMethod delegateMethod)
        {
            return new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), delegateField.Name),
                    CodeBinaryOperatorType.IdentityEquality,
                    new CodePrimitiveExpression(null)),
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), delegateField.Name),
                    new CodeDelegateCreateExpression(delegateField.Type,
                        new CodeThisReferenceExpression(), delegateMethod.Name)));
        }

        private static string GetClassName(string interfaceName)
        {
            // maybe strip a leading 'I'
            if (interfaceName.Length >= 2 &&
                String.Compare(interfaceName, 0, Strings.InterfaceTypePrefix, 0, Strings.InterfaceTypePrefix.Length, StringComparison.Ordinal) == 0 &&
                Char.IsUpper(interfaceName, 1))
                return interfaceName.Substring(1);
            else
                return interfaceName;
        }

        private static string GetEventAsyncMethodName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}Async", syncMethodName);
        }

        private static string GetBeginOperationDelegateName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "onBegin{0}Delegate", syncMethodName);
        }

        private static string GetBeginOperationMethodName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "OnBegin{0}", syncMethodName);
        }

        private static string GetEndOperationDelegateName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "onEnd{0}Delegate", syncMethodName);
        }

        private static string GetEndOperationMethodName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "OnEnd{0}", syncMethodName);
        }

        private static string GetOperationCompletedDelegateName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "on{0}CompletedDelegate", syncMethodName);
        }

        private static string GetOperationCompletedMethodName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "On{0}Completed", syncMethodName);
        }

        private static string GetOperationCompletedEventName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}Completed", syncMethodName);
        }

        private static string GetOperationCompletedEventArgsTypeName(string syncMethodName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}CompletedEventArgs", syncMethodName);
        }

        static internal string GetClientClassName(string interfaceName)
        {
            return GetClassName(interfaceName) + Strings.ClientTypeSuffix;
        }

        private static bool IsVoid(CodeMemberMethod method)
        {
            return method.ReturnType == null || String.Compare(method.ReturnType.BaseType, typeof(void).FullName, StringComparison.Ordinal) == 0;
        }

        private static CodeExpression GetChannelReference()
        {
            return new CodePropertyReferenceExpression(new CodeBaseReferenceExpression(), Strings.ClientBaseChannelProperty);
        }

        private static class Strings
        {
            public const string ClientBaseChannelProperty = "Channel";
            public const string ClientTypeSuffix = "Client";
            public const string InterfaceTypePrefix = "I";
        }
    }
}

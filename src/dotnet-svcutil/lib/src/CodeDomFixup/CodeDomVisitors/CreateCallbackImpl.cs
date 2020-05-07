// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class CreateCallbackImpl : ClientClassVisitor
    {
        private bool _taskBasedAsync;
        private List<CodeTypeDeclaration> _eventArgsList = new List<CodeTypeDeclaration>();
        private ServiceContractGenerator _generator;
        private MetadataConversionError _requestReplyError;
        private CodeTypeDeclaration _eventBasedDuplexClass;

        public CreateCallbackImpl(bool taskBasedAsync, ServiceContractGenerator generator)
        {
            _taskBasedAsync = taskBasedAsync;
            _generator = generator;
        }

        protected override bool IsSpecificType(CodeTypeDeclaration type)
        {
            // Check if the current client class is for duplex service.
            return _taskBasedAsync &&
                base.IsSpecificType(type) &&
                CodeDomHelpers.MatchGenericBaseType(type.BaseTypes[0], typeof(DuplexClientBase<>));
        }

        protected override void VisitClientClass(CodeTypeDeclaration type)
        {
            base.VisitClientClass(type);
            CodeTypeDeclaration serviceContractInterface = CodeDomHelpers.ResolveTypeReference(type.BaseTypes[0].TypeArguments[0]);
            CodeTypeDeclaration callbackInterface = GetCallbackContractType(serviceContractInterface);
            RemoveAsyncMethods(callbackInterface);
            CreateEventBasedDuplexClass(type, callbackInterface);
        }

        private void CreateEventBasedDuplexClass(CodeTypeDeclaration type, CodeTypeDeclaration callbackInterface)
        {
            _eventBasedDuplexClass = new CodeTypeDeclaration(type.Name);
            _eventBasedDuplexClass.IsPartial = true;
            type.Name += "Base";
            _eventBasedDuplexClass.BaseTypes.Add(type.Name);
            CodeTypeDeclaration callbackImpl;
            using (CodeTypeNameScope nameScope = new CodeTypeNameScope(_eventBasedDuplexClass))
            {
                Dictionary<string, string> methodNames = GenerateEventAsyncMethods(nameScope, _eventBasedDuplexClass, callbackInterface, _eventArgsList);
                callbackImpl = CreateCallbackImplClass(nameScope, _eventBasedDuplexClass, callbackInterface, methodNames);
                _eventBasedDuplexClass.Members.Add(callbackImpl);
            }
            // Create ctor
            CreateCtorOverload(_eventBasedDuplexClass, callbackImpl);
        }

        private MetadataConversionError RequestReplyError
        {
            get
            {
                if (_requestReplyError == null)
                {
                    _requestReplyError = new MetadataConversionError(SR.RequestReplyCallbackContractNotSupported, false);
                }
                return _requestReplyError;
            }
        }

        private static void RemoveAsyncMethods(CodeTypeDeclaration callbackInterface)
        {
            CollectionHelpers.MapList<CodeMemberMethod>(
                callbackInterface.Members,
                delegate (CodeMemberMethod method)
                {
                    return !CodeDomHelpers.IsTaskAsyncMethod(method);
                },
                null
            );
        }

        protected override void FinishVisit(CodeCompileUnit cu)
        {
            base.FinishVisit(cu);

            foreach (CodeTypeDeclaration type in _eventArgsList)
            {
                cu.Namespaces[0].Types.Add(type);
            }

            if (_eventBasedDuplexClass != null)
            {
                cu.Namespaces[0].Types.Add(_eventBasedDuplexClass);
            }
        }

        private static void CreateCtorOverload(CodeTypeDeclaration parent, CodeTypeDeclaration callbackImpl)
        {
            CodeConstructor ctor;

            //public HelloServiceClient(string endpointConfiguration) :
            //    this(new HelloServiceClientCallback(), endpointConfiguration)
            //{
            //}
            ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            ctor.ChainedConstructorArgs.Add(new CodeObjectCreateExpression(new CodeTypeReference(callbackImpl.Name)));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "endpointConfiguration"));
            ctor.ChainedConstructorArgs.Add(new CodeVariableReferenceExpression("endpointConfiguration"));
            parent.Members.Add(ctor);

            //private HelloServiceClient(HelloServiceClientCallback callbackImpl, string endpointConfiguration) :
            //    base(new System.ServiceModel.InstanceContext(callbackImpl), endpointConfiguration)
            //{
            //    callbackImpl.Initialize(this);
            //}
            ctor = new CodeConstructor();
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(callbackImpl.Name), "callbackImpl"));
            ctor.BaseConstructorArgs.Add(new CodeObjectCreateExpression(new CodeTypeReference(typeof(System.ServiceModel.InstanceContext)), new CodeArgumentReferenceExpression("callbackImpl")));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "endpointConfiguration"));
            ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("endpointConfiguration"));

            ctor.Statements.Add(
                new CodeMethodInvokeExpression()
                {
                    Method = new CodeMethodReferenceExpression()
                    {
                        TargetObject = new CodeVariableReferenceExpression("callbackImpl"),
                        MethodName = "Initialize",
                    },
                    Parameters = { new CodeThisReferenceExpression() }
                }
            );
            parent.Members.Add(ctor);

            //public PollingDuplexEchoClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            //    this(new PollingDuplexEchoCallback(), binding, remoteAddress)
            //{
            //}
            ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            ctor.ChainedConstructorArgs.Add(new CodeObjectCreateExpression(new CodeTypeReference(callbackImpl.Name)));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.ServiceModel.Channels.Binding), "binding"));
            ctor.ChainedConstructorArgs.Add(new CodeVariableReferenceExpression("binding"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.ServiceModel.EndpointAddress), "remoteAddress"));
            ctor.ChainedConstructorArgs.Add(new CodeVariableReferenceExpression("remoteAddress"));
            parent.Members.Add(ctor);

            //private PollingDuplexEchoClient(PollingDuplexEchoCallback callback, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            //    base(new System.ServiceModel.InstanceContext(callback), binding, remoteAddress)
            //{
            //    callback.Initialize(this);
            //}
            ctor = new CodeConstructor();
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(callbackImpl.Name), "callbackImpl"));
            ctor.BaseConstructorArgs.Add(new CodeObjectCreateExpression(new CodeTypeReference(typeof(System.ServiceModel.InstanceContext)), new CodeArgumentReferenceExpression("callbackImpl")));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.ServiceModel.Channels.Binding), "binding"));
            ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("binding"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(System.ServiceModel.EndpointAddress), "remoteAddress"));
            ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("remoteAddress"));

            ctor.Statements.Add(
                new CodeMethodInvokeExpression()
                {
                    Method = new CodeMethodReferenceExpression()
                    {
                        TargetObject = new CodeVariableReferenceExpression("callbackImpl"),
                        MethodName = "Initialize",
                    },
                    Parameters = { new CodeThisReferenceExpression() }
                }
            );
            parent.Members.Add(ctor);

            //public HelloServiceClient() :
            //    this(new HelloServiceClientCallback())
            //{
            //}
            ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            ctor.ChainedConstructorArgs.Add(new CodeObjectCreateExpression(new CodeTypeReference(callbackImpl.Name)));
            parent.Members.Add(ctor);

            //private HelloServiceClient(HelloServiceClientCallback callbackImpl) :
            //    base(new System.ServiceModel.InstanceContext(callbackImpl))
            //{
            //    callbackImpl.Initialize(this);
            //}
            ctor = new CodeConstructor();
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(callbackImpl.Name), "callbackImpl"));
            ctor.BaseConstructorArgs.Add(new CodeObjectCreateExpression(new CodeTypeReference(typeof(System.ServiceModel.InstanceContext)), new CodeArgumentReferenceExpression("callbackImpl"))); ;

            ctor.Statements.Add(
                new CodeMethodInvokeExpression()
                {
                    Method = new CodeMethodReferenceExpression()
                    {
                        TargetObject = new CodeVariableReferenceExpression("callbackImpl"),
                        MethodName = "Initialize",
                    },
                    Parameters = { new CodeThisReferenceExpression() }
                }
            );
            parent.Members.Add(ctor);
        }

        private static Dictionary<string, string> GenerateEventAsyncMethods(CodeTypeNameScope nameScope, CodeTypeDeclaration parent, CodeTypeDeclaration callbackInterface, List<CodeTypeDeclaration> eventArgsList)
        {
            Dictionary<string, string> methodNames = new Dictionary<string, string>();
            List<CodeMemberEvent> receivedEvents = new List<CodeMemberEvent>();
            foreach (CodeMemberMethod method in callbackInterface.Members)
            {
                if (IsSyncOperationContract(method))
                {
                    // public partial class UploadFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
                    CodeTypeDeclaration eventArgs = CreateOperationReceivedEventArgsType(nameScope, method);
                    if (eventArgs != null)
                    {
                        eventArgsList.Add(eventArgs);
                    }

                    // public event System.EventHandler<UploadFileCompletedEventArgs> UploadFileCompleted;
                    CodeMemberEvent evt = CreateOperationReceivedEvent(nameScope, method, eventArgs);
                    parent.Members.Add(evt);
                    receivedEvents.Add(evt);

                    CodeMemberMethod member = CreateOperationReceivedMethod(nameScope, method, eventArgs, evt);
                    parent.Members.Add(member);
                    System.Diagnostics.Debug.Assert(!methodNames.ContainsKey(method.Name), $"Key '{method.Name}' already added to dictionary!");
                    methodNames[method.Name] = member.Name;
                }
            }
            return methodNames;
        }

        private static CodeMemberMethod CreateOperationReceivedMethod(CodeTypeNameScope nameScope, CodeMemberMethod syncMethod,
            CodeTypeDeclaration operationReceivedEventArgsType, CodeMemberEvent operationCompletedEvent)
        {
            CodeMemberMethod operationCompletedMethod = new CodeMemberMethod();
            operationCompletedMethod.Name = nameScope.UniqueMemberName("On" + syncMethod.Name + "Received");
            operationCompletedMethod.Attributes = MemberAttributes.Private;

            operationCompletedMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "state"));
            operationCompletedMethod.ReturnType = new CodeTypeReference(typeof(void));

            // object[] results = ((object[])(state));
            CodeVariableDeclarationStatement results = new CodeVariableDeclarationStatement()
            {
                Name = "results",
                Type = new CodeTypeReference(typeof(object[])),
                InitExpression = new CodeCastExpression()
                {
                    TargetType = new CodeTypeReference(typeof(object[])),
                    Expression = new CodeVariableReferenceExpression("state"),
                }
            };

            // new OnEcho2CallbackEventArgs(results, null, false, null)
            CodeObjectCreateExpression newEventArgsExpr = null;
            if (operationReceivedEventArgsType != null)
            {
                newEventArgsExpr = new CodeObjectCreateExpression(operationReceivedEventArgsType.Name,
                    new CodeVariableReferenceExpression(results.Name),
                    new CodePrimitiveExpression(null),
                    new CodePrimitiveExpression(false),
                    new CodePrimitiveExpression(null)
                    );
            }
            else
            {
                newEventArgsExpr = new CodeObjectCreateExpression(typeof(AsyncCompletedEventArgs),
                    new CodePrimitiveExpression(null),
                    new CodePrimitiveExpression(false),
                    new CodePrimitiveExpression(null)
                    );
            }

            // this.OnEcho2Callback(this, new OnEcho2CallbackEventArgs(results, null, false, null));
            CodeEventReferenceExpression completedEvent = new CodeEventReferenceExpression(new CodeThisReferenceExpression(), operationCompletedEvent.Name);

            CodeDelegateInvokeExpression raiseEventExpr = new CodeDelegateInvokeExpression(
                completedEvent,
                new CodeThisReferenceExpression(),
                newEventArgsExpr);


            //if ((this.OnEcho2Callback != null))
            //{
            //    object[] results = ((object[])(state));
            //    this.OnEcho2Callback(this, new OnEcho2CallbackEventArgs(results, null, false, null));
            //}
            CodeConditionStatement ifEventHandlerNotNullBlock = new CodeConditionStatement()
            {
                Condition = new CodeBinaryOperatorExpression(completedEvent, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
                TrueStatements =
                            {
                                results,
                                raiseEventExpr,
                            }
            };

            operationCompletedMethod.Statements.Add(ifEventHandlerNotNullBlock);

            return operationCompletedMethod;
        }

        private static CodeMemberEvent CreateOperationReceivedEvent(CodeTypeNameScope nameScope, CodeMemberMethod syncMethod,
            CodeTypeDeclaration operationCompletedEventArgsType)
        {
            // public event System.EventHandler<OnEcho2CallbackEventArgs> OnEcho2Callback;
            CodeMemberEvent operationCompletedEvent = new CodeMemberEvent();
            operationCompletedEvent.Name = nameScope.UniqueMemberName(syncMethod.Name + "Received");
            operationCompletedEvent.Attributes = MemberAttributes.Public;
            operationCompletedEvent.Type = new CodeTypeReference(typeof(EventHandler));

            if (operationCompletedEventArgsType == null)
            {
                operationCompletedEvent.Type.TypeArguments.Add(typeof(AsyncCompletedEventArgs));
            }
            else
            {
                operationCompletedEvent.Type.TypeArguments.Add(operationCompletedEventArgsType.Name);
            }

            return operationCompletedEvent;
        }

        private static CodeTypeDeclaration CreateOperationReceivedEventArgsType(CodeTypeNameScope nameScope, CodeMemberMethod syncMethod)
        {
            if (syncMethod.Parameters.Count <= 0)
            {
                // no need to create new event args type, use AsyncCompletedEventArgs
                return null;
            }

            CodeTypeDeclaration evtArg = new CodeTypeDeclaration();
            evtArg.Name = nameScope.UniqueMemberName(syncMethod.Name + "ReceivedEventArgs");
            evtArg.BaseTypes.Add(new CodeTypeReference(typeof(AsyncCompletedEventArgs)));

            // define object[] results field.
            CodeMemberField resultsField = new CodeMemberField(typeof(object[]), "results");
            evtArg.Members.Add(resultsField);

            // create constructor, that assigns the results field.
            CodeConstructor ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object[]), "results"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Exception), "exception"));
            ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("exception"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "cancelled"));
            ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("cancelled"));
            ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "userState"));
            ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("userState"));
            evtArg.Members.Add(ctor);

            CodeFieldReferenceExpression resultsFieldReference = new CodeFieldReferenceExpression();
            resultsFieldReference.TargetObject = new CodeThisReferenceExpression();
            resultsFieldReference.FieldName = "results";
            ctor.Statements.Add(new CodeAssignStatement(resultsFieldReference, new CodeVariableReferenceExpression("results")));

            using (PropertyFieldNameScope typeNamescope = new PropertyFieldNameScope(typeof(AsyncCompletedEventArgs)))
            {
                for (int paramIndex = 0; paramIndex < syncMethod.Parameters.Count; paramIndex++)
                {
                    CodeParameterDeclarationExpression param = syncMethod.Parameters[paramIndex];
                    CodeMemberProperty property = new CodeMemberProperty();
                    property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    property.Type = param.Type;
                    property.Name = typeNamescope.UniqueMemberName(param.Name);
                    property.HasSet = false;
                    property.HasGet = true;

                    // base.RaiseExceptionIfNecessary();
                    property.GetStatements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "RaiseExceptionIfNecessary"));

                    // return ((string)(this.results[0]));
                    CodeCastExpression castExpr = new CodeCastExpression(param.Type, new CodeArrayIndexerExpression(resultsFieldReference, new CodePrimitiveExpression(paramIndex)));
                    CodeMethodReturnStatement returnStmt = new CodeMethodReturnStatement(castExpr);
                    property.GetStatements.Add(returnStmt);

                    evtArg.Members.Add(property);
                }
            }
            return evtArg;
        }


        private static CodeTypeDeclaration CreateCallbackImplClass(CodeTypeNameScope nameScope, CodeTypeDeclaration parent, CodeTypeDeclaration callbackInterface, Dictionary<string, string> methodNames)
        {
            CodeTypeDeclaration callbackImpl = new CodeTypeDeclaration();
            callbackImpl.Name = nameScope.UniqueMemberName(parent.Name + "Callback");
            callbackImpl.TypeAttributes &= ~TypeAttributes.VisibilityMask;
            callbackImpl.TypeAttributes |= TypeAttributes.NestedPrivate;
            callbackImpl.BaseTypes.Add(new CodeTypeReference(typeof(object)));
            callbackImpl.BaseTypes.Add(new CodeTypeReference(callbackInterface.Name));

            AddMembers(callbackImpl, parent);
            AddInitialize(callbackImpl, parent);
            AddMethods(callbackImpl, callbackInterface, methodNames);
            return callbackImpl;
        }

        private static void AddMembers(CodeTypeDeclaration callbackImpl, CodeTypeDeclaration parent)
        {
            callbackImpl.Members.Add(
                new CodeMemberField()
                {
                    Name = "proxy",
                    Type = new CodeTypeReference(parent.Name)
                }
            );
        }

        private static void AddInitialize(CodeTypeDeclaration callbackImpl, CodeTypeDeclaration parent)
        {
            // public void Initialize(PollingDuplexEchoClient proxy)
            CodeMemberMethod m = new CodeMemberMethod();
            m.Name = "Initialize";
            m.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            m.Parameters.Add(
                new CodeParameterDeclarationExpression()
                {
                    Name = "proxy",
                    Type = new CodeTypeReference(parent.Name)
                }
            );

            // this.synchronizationContext = System.ComponentModel.AsyncOperationManager.SynchronizationContext;
            m.Statements.Add(
                new CodeAssignStatement()
                {
                    Left = new CodeFieldReferenceExpression()
                    {
                        TargetObject = new CodeThisReferenceExpression(),
                        FieldName = "proxy"
                    },
                    Right = new CodeVariableReferenceExpression("proxy")
                }
            );
            callbackImpl.Members.Add(m);
        }

        private static void AddMethods(CodeTypeDeclaration callbackImpl, CodeTypeDeclaration callbackInterface, Dictionary<string, string> methodNames)
        {
            foreach (CodeMemberMethod method in callbackInterface.Members)
            {
                System.Diagnostics.Debug.Assert((IsSyncOperationContract(method)), "Only support sync callback on immersive project");
                CodeMemberMethod m = CodeDomHelpers.GetImplementationOfMethod(callbackImpl.BaseTypes[1], method);

                // new object[] { msg, timestamp });
                CodeArrayCreateExpression arr = new CodeArrayCreateExpression();
                arr.CreateType = new CodeTypeReference(typeof(object));
                foreach (CodeParameterDeclarationExpression p in method.Parameters)
                {
                    arr.Initializers.Add(new CodeVariableReferenceExpression(p.Name));
                }

                // proxy.OnOnEchoReceived(new object[] { msg, timestamp});
                m.Statements.Add(
                    new CodeMethodInvokeExpression()
                    {
                        Method = new CodeMethodReferenceExpression()
                        {
                            TargetObject = new CodeFieldReferenceExpression()
                            {
                                TargetObject = new CodeThisReferenceExpression(),
                                FieldName = "proxy"
                            },
                            MethodName = methodNames[method.Name],
                        },
                        Parameters =
                        {
                            arr
                        },
                    }
                );

                if (method.ReturnType.BaseType != typeof(void).FullName)
                {
                    m.Statements.Add(new CodeMethodReturnStatement(
                        new CodeDefaultValueExpression(
                            method.ReturnType)
                        ));
                }

                callbackImpl.Members.Add(m);
            }
        }

        private static CodeTypeDeclaration GetCallbackContractType(CodeTypeDeclaration iface)
        {
            CodeAttributeDeclaration serviceContractAttribute = null;
            foreach (CodeAttributeDeclaration attr in iface.CustomAttributes)
            {
                if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(ServiceContractAttribute)))
                {
                    serviceContractAttribute = attr;
                    break;
                }
            }
            if (serviceContractAttribute != null)
            {
                foreach (CodeAttributeArgument arg in serviceContractAttribute.Arguments)
                {
                    if (arg.Name == "CallbackContract")
                    {
                        CodeTypeOfExpression exp = (CodeTypeOfExpression)arg.Value;
                        return CodeDomHelpers.ResolveTypeReference(exp.Type);
                    }
                }
            }
            return null;
        }

        private static bool IsSyncOperationContract(CodeMemberMethod method)
        {
            CodeAttributeDeclaration operationContractAttribute = null;
            foreach (CodeAttributeDeclaration attr in method.CustomAttributes)
            {
                if (attr.Name == typeof(System.ServiceModel.OperationContractAttribute).FullName)
                {
                    operationContractAttribute = attr;
                    break;
                }
            }
            if (operationContractAttribute == null)
                return false;

            foreach (CodeAttributeArgument arg in operationContractAttribute.Arguments)
            {
                if (arg.Name == "AsyncPattern")
                {
                    return !(bool)((CodePrimitiveExpression)arg.Value).Value;
                }
            }
            return true;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using Microsoft.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ArrayOfXElementTypeHelper
    {
        private const string ArrayOfXElementTypeName = "ArrayOfXElement";

        private static CodeTypeDeclaration s_arrayOfXElements = null;
        private static CodeTypeReference s_arrayOfXElementRef = null;

        private List<string> _addedToCodeCompileUnit = new List<string>();
        private List<string> _needToAdd = new List<string>();

        private const string xelementType = "System.Xml.Linq.XElement";
        internal string SpecialNamespace = "";

        private static bool s_isInternal = false;

        internal ArrayOfXElementTypeHelper(bool isInternal, CodeCompileUnit cu)
        {
            ArrayOfXElementTypeHelper.s_isInternal = isInternal;

            //ArrayOfXElement will be in the global namespace
            CodeNamespace targetNamespace = cu.Namespaces.Count > 0 ? cu.Namespaces[0] : new CodeNamespace();
            SpecialNamespace = targetNamespace.Name;
            s_arrayOfXElements = CreateArrayOfXmlElementClass(targetNamespace);
            s_arrayOfXElementRef = new CodeTypeReference(GetUniqueClassName(targetNamespace));
        }

        internal static CodeTypeDeclaration ArrayOfXElements
        {
            get
            {
                return s_arrayOfXElements;
            }
        }

        internal static CodeTypeReference ArrayOfXElementRef
        {
            get
            {
                return s_arrayOfXElementRef;
            }
        }

        internal void CheckToAdd(string namespaceToAdd)
        {
            if (!_needToAdd.Contains(namespaceToAdd))
            {
                _needToAdd.Add(namespaceToAdd);
            }
        }

        internal static string GetUniqueClassName(CodeNamespace ns)
        {
            string uniqueName = ArrayOfXElementTypeName;
            Dictionary<string, CodeTypeDeclaration> nameTable = new Dictionary<string, CodeTypeDeclaration>();

            foreach (CodeTypeDeclaration type in ns.Types)
            {
                if (!nameTable.ContainsKey(type.Name))
                {
                    nameTable.Add(type.Name, type);
                }
            }

            int i = 0;
            while (nameTable.ContainsKey(uniqueName))
            {
                uniqueName = ArrayOfXElementTypeName + (++i).ToString(CultureInfo.InvariantCulture);
            }
            return uniqueName;
        }

        private static void AddGeneratedCodeAttribute(CodeTypeDeclaration codeType)
        {
            CodeAttributeDeclaration generatedCodeAttribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)));

            AssemblyName assemblyName = typeof(ArrayOfXElementTypeHelper).GetTypeInfo().Assembly.GetName();
            generatedCodeAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(assemblyName.Name)));
            generatedCodeAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(assemblyName.Version.ToString())));

            codeType.CustomAttributes.Add(generatedCodeAttribute);
        }

        internal void AddToCompileUnit(CodeCompileUnit codeCompileUnit, string namespaceToAdd)
        {
            if (!_addedToCodeCompileUnit.Contains(namespaceToAdd) && _needToAdd.Contains(namespaceToAdd))
            {
                //if it is special namespace and there is only one namespace, add the special type to the first ns because that's when -n is used
                if (codeCompileUnit.Namespaces.Count > 0 && namespaceToAdd.Equals(SpecialNamespace))
                {
                    _needToAdd.Remove(namespaceToAdd);
                    namespaceToAdd = codeCompileUnit.Namespaces[0].Name;
                    _needToAdd.Add(namespaceToAdd);
                }

                foreach (CodeNamespace ns in codeCompileUnit.Namespaces)
                {
                    if (namespaceToAdd.Equals(ns.Name))
                    {
                        ns.Types.Add(ArrayOfXElements);
                        _addedToCodeCompileUnit.Add(namespaceToAdd);
                        break;
                    }
                }
            }
        }

        internal static CodeTypeReference CreateTypeReference(string typeName, params string[] typeArguments)
        {
            CodeTypeReference[] typeRefArgs = new CodeTypeReference[typeArguments.Length];
            for (int i = 0; i < typeArguments.Length; i++)
            {
                typeRefArgs[i] = new CodeTypeReference(typeArguments[i]);
            }
            return new CodeTypeReference(typeName, typeRefArgs);
        }

        private static CodeTypeDeclaration CreateArrayOfXmlElementClass(CodeNamespace ns)
        {
            CodeTypeDeclaration classToGen = new CodeTypeDeclaration(GetUniqueClassName(ns));
            classToGen.IsClass = true;
            classToGen.IsPartial = true;
            classToGen.TypeAttributes = s_isInternal ? TypeAttributes.NotPublic : TypeAttributes.Public;

            CodeAttributeDeclaration xmlSchemaProviderAttribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(Microsoft.Xml.Serialization.XmlSchemaProviderAttribute)),
                new CodeAttributeArgument(new CodePrimitiveExpression(null)),
                new CodeAttributeArgument("IsAny", new CodePrimitiveExpression(true)));
            classToGen.CustomAttributes.Add(xmlSchemaProviderAttribute);

            classToGen.BaseTypes.Add(new CodeTypeReference(typeof(Object)));
            classToGen.BaseTypes.Add(new CodeTypeReference(typeof(Microsoft.Xml.Serialization.IXmlSerializable)));

            CodeConstructor ctor = new CodeConstructor();
            ctor.Attributes = MemberAttributes.Public;
            classToGen.Members.Add(ctor);

            AddField(classToGen);
            AddProperty(classToGen);
            AddGetSchemaMethod(classToGen);
            AddWriteXml(classToGen);
            AddReadXml(classToGen);
            AddGeneratedCodeAttribute(classToGen);

            return classToGen;
        }

        private static void AddField(CodeTypeDeclaration classToGen)
        {
            CodeMemberField nodesField = new CodeMemberField();
            nodesField.Attributes = MemberAttributes.Private;
            nodesField.Name = "nodesList";

            CodeTypeReference listOfXElement = CreateTypeReference("System.Collections.Generic.List`1", xelementType);
            nodesField.Type = listOfXElement;
            nodesField.InitExpression = new CodeObjectCreateExpression(listOfXElement);

            classToGen.Members.Add(nodesField);
        }

        private static void AddProperty(CodeTypeDeclaration classToGen)
        {
            CodeMemberProperty nodesProperty = new CodeMemberProperty();
            nodesProperty.Attributes = MemberAttributes.Public;
            nodesProperty.Name = "Nodes";

            nodesProperty.HasGet = true;
            CodeTypeReference listOfXElement = CreateTypeReference("System.Collections.Generic.List`1", xelementType);
            nodesProperty.Type = listOfXElement;

            nodesProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), "nodesList")));

            classToGen.Members.Add(nodesProperty);
        }
        private static void AddGetSchemaMethod(CodeTypeDeclaration classToGen)
        {
            CodeMemberMethod getSchemaMethod = new CodeMemberMethod();
            getSchemaMethod.Attributes = MemberAttributes.Public;
            getSchemaMethod.Name = "GetSchema";
            getSchemaMethod.ImplementationTypes.Add(classToGen.BaseTypes[1]);
            getSchemaMethod.ReturnType = new CodeTypeReference(typeof(Microsoft.Xml.Schema.XmlSchema));

            CodeThrowExceptionStatement throwException = new CodeThrowExceptionStatement(
                new CodeObjectCreateExpression(
                new CodeTypeReference(typeof(NotImplementedException)),
                new CodeExpression[] { }));

            getSchemaMethod.Statements.Add(throwException);

            classToGen.Members.Add(getSchemaMethod);
        }
        private static void AddWriteXml(CodeTypeDeclaration classToGen)
        {
            CodeMemberMethod writeXml = new CodeMemberMethod();
            writeXml.Name = "WriteXml";
            writeXml.Attributes = MemberAttributes.Public;
            writeXml.ImplementationTypes.Add(classToGen.BaseTypes[1]);

            writeXml.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(Microsoft.Xml.XmlWriter)), "writer"));
            CodeVariableDeclarationStatement enumeratorDec =
                new CodeVariableDeclarationStatement(
                    CreateTypeReference("System.Collections.Generic.IEnumerator`1", xelementType), "e",
                    new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("nodesList"), "GetEnumerator"));

            writeXml.Statements.Add(enumeratorDec);

            CodeVariableReferenceExpression eRef = new CodeVariableReferenceExpression("e");
            CodePropertyReferenceExpression eCurrent = new CodePropertyReferenceExpression(eRef, "Current");
            CodeCastExpression iXmlSerCast = new CodeCastExpression(new CodeTypeReference(typeof(Microsoft.Xml.Serialization.IXmlSerializable)), eCurrent);
            CodeMethodInvokeExpression codeWrite = new CodeMethodInvokeExpression(iXmlSerCast, "WriteXml", new CodeVariableReferenceExpression("writer"));

            CodeIterationStatement codeFor = new CodeIterationStatement();
            codeFor.TestExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("e"), "MoveNext"));
            codeFor.Statements.Add(codeWrite);
            codeFor.IncrementStatement = new CodeSnippetStatement("");
            codeFor.InitStatement = new CodeSnippetStatement("");

            writeXml.Statements.Add(codeFor);

            classToGen.Members.Add(writeXml);
        }

        private static void AddReadXml(CodeTypeDeclaration classToGen)
        {
            CodeMemberMethod readXml = new CodeMemberMethod();
            readXml.Name = "ReadXml";
            readXml.Attributes = MemberAttributes.Public;
            readXml.ImplementationTypes.Add(classToGen.BaseTypes[1]);

            readXml.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(Microsoft.Xml.XmlReader)), "reader"));

            CodeVariableReferenceExpression reader = new CodeVariableReferenceExpression("reader");
            CodePropertyReferenceExpression readerNodeType = new CodePropertyReferenceExpression(reader, "NodeType");
            CodeTypeReferenceExpression xmlNodeType = new CodeTypeReferenceExpression(typeof(Microsoft.Xml.XmlNodeType));
            CodePropertyReferenceExpression xmlNodeTypeEndElement = new CodePropertyReferenceExpression(xmlNodeType, "EndElement");
            CodeBinaryOperatorExpression notEq = new CodeBinaryOperatorExpression(readerNodeType, CodeBinaryOperatorType.IdentityInequality, xmlNodeTypeEndElement);

            CodeIterationStatement codeFor = new CodeIterationStatement();
            codeFor.TestExpression = notEq;
            codeFor.IncrementStatement = new CodeSnippetStatement("");
            codeFor.InitStatement = new CodeSnippetStatement("");

            CodePropertyReferenceExpression xmlNodeTypeElement = new CodePropertyReferenceExpression(xmlNodeType, "Element");
            CodeBinaryOperatorExpression eQElement = new CodeBinaryOperatorExpression(readerNodeType, CodeBinaryOperatorType.ValueEquality, xmlNodeTypeElement);

            CodeVariableDeclarationStatement element =
                new CodeVariableDeclarationStatement(
                    new CodeTypeReference(xelementType), "elem",
                    new CodeObjectCreateExpression(new CodeTypeReference(xelementType), new CodePrimitiveExpression("default")));

            CodeCastExpression iXmlSerCast = new CodeCastExpression(new CodeTypeReference(typeof(Microsoft.Xml.Serialization.IXmlSerializable)), new CodeVariableReferenceExpression("elem"));

            CodeMethodInvokeExpression codeRead = new CodeMethodInvokeExpression(iXmlSerCast, "ReadXml", new CodeVariableReferenceExpression("reader"));

            CodeMethodInvokeExpression addNode =
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("Nodes"),
                        "Add"), new CodeVariableReferenceExpression("elem"));

            CodeConditionStatement codeIfElse = new CodeConditionStatement(eQElement);
            codeIfElse.TrueStatements.Add(element);
            codeIfElse.TrueStatements.Add(codeRead);
            codeIfElse.TrueStatements.Add(addNode);

            CodeMethodInvokeExpression skip = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("reader"), "Skip"));
            codeIfElse.FalseStatements.Add(skip);

            codeFor.Statements.Add(codeIfElse);

            readXml.Statements.Add(codeFor);

            classToGen.Members.Add(readXml);
        }
    }
}

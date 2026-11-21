// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Microsoft.CodeDom;
using Microsoft.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Xunit;

namespace SvcutilTest
{
    public class AsyncMessageContractTests
    {
        [Fact]
        public void GenerateHelperMethod_UnwrapsTaskMessageContractResponse()
        {
            CodeMemberMethod helper = CreateHelperMethod();

            Assert.Equal(typeof(Task<>).FullName, helper.ReturnType.BaseType);
            Assert.Equal("Test.AccountResponse", Assert.Single(helper.ReturnType.TypeArguments.Cast<CodeTypeReference>()).BaseType);
            Assert.True((bool)helper.UserData[CodeAwaitExpression.AsyncMethodUserDataKey]);

            CodeVariableDeclarationStatement responseDeclaration = Assert.IsType<CodeVariableDeclarationStatement>(helper.Statements[2]);
            Assert.Equal("Test.GetAccountResponse", responseDeclaration.Type.BaseType);
            Assert.IsType<CodeAwaitExpression>(responseDeclaration.InitExpression);

            CodeMethodReturnStatement returnStatement = Assert.IsType<CodeMethodReturnStatement>(helper.Statements[3]);
            CodeFieldReferenceExpression returnedField = Assert.IsType<CodeFieldReferenceExpression>(returnStatement.Expression);
            Assert.Equal("SingleResponse", returnedField.FieldName);
        }

        [Fact]
        public void GenerateHelperMethod_PreservesTaskMessageContractResponseWithMultipleOutputs()
        {
            var codeNamespace = new CodeNamespace("Test");
            var requestMessage = CreateMessageType("GetAccountRequest", "Request", new CodeTypeReference("Test.AccountRequest"));
            var responseMessage = CreateMessageType("GetAccountResponse", "Result", new CodeTypeReference(typeof(string)));
            responseMessage.Members.Add(new CodeMemberField(typeof(string), "Header"));
            var requestType = ServiceContractGenerator.NamespaceHelper.GetCodeTypeReference(codeNamespace, requestMessage);
            var responseType = ServiceContractGenerator.NamespaceHelper.GetCodeTypeReference(codeNamespace, responseMessage);
            var method = new CodeMemberMethod
            {
                Name = "GetAccountAsync",
                ReturnType = new CodeTypeReference(typeof(Task<>).FullName, responseType)
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(requestType, "request"));

            CodeMemberMethod helper = ClientClassGenerator.GenerateHelperMethod(new CodeTypeReference("Test.IAccountService"), method);

            Assert.NotNull(helper);
            Assert.Equal(typeof(Task<>).FullName, helper.ReturnType.BaseType);
            Assert.Equal("Test.GetAccountResponse", Assert.Single(helper.ReturnType.TypeArguments.Cast<CodeTypeReference>()).BaseType);
            Assert.Null(helper.UserData[CodeAwaitExpression.AsyncMethodUserDataKey]);
            Assert.All(helper.Parameters.Cast<CodeParameterDeclarationExpression>(), parameter => Assert.Equal(FieldDirection.In, parameter.Direction));
            Assert.IsType<CodeMethodReturnStatement>(helper.Statements[2]);
        }

        [Fact]
        public void CSharpCodeProvider_GeneratesAsyncAwaitHelper()
        {
            string code = GenerateCode(new CSharpCodeProvider(), CreateHelperMethod());

            Assert.Contains("public async System.Threading.Tasks.Task<Test.AccountResponse> GetAccountAsync", code);
            Assert.Contains("await ((Test.IAccountService)(this)).GetAccountAsync(inValue).ConfigureAwait(false)", code);
        }

        [Fact]
        public void VisualBasicCodeProvider_GeneratesAsyncAwaitHelper()
        {
            string code = GenerateCode(new VBCodeProvider(), CreateHelperMethod());

            Assert.Contains("Public Async Function GetAccountAsync", code);
            Assert.Contains("Await CType(Me,Test.IAccountService).GetAccountAsync(inValue).ConfigureAwait(false)", code);
        }

        private static CodeMemberMethod CreateHelperMethod()
        {
            var codeNamespace = new CodeNamespace("Test");
            var requestMessage = CreateMessageType("GetAccountRequest", "Request", new CodeTypeReference("Test.AccountRequest"));
            var responseMessage = CreateMessageType("GetAccountResponse", "SingleResponse", new CodeTypeReference("Test.AccountResponse"));
            var requestType = ServiceContractGenerator.NamespaceHelper.GetCodeTypeReference(codeNamespace, requestMessage);
            var responseType = ServiceContractGenerator.NamespaceHelper.GetCodeTypeReference(codeNamespace, responseMessage);
            var method = new CodeMemberMethod
            {
                Name = "GetAccountAsync",
                ReturnType = new CodeTypeReference(typeof(Task<>).FullName, responseType)
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression(requestType, "request"));

            CodeMemberMethod helper = ClientClassGenerator.GenerateHelperMethod(new CodeTypeReference("Test.IAccountService"), method);

            Assert.NotNull(helper);
            return helper;
        }

        private static string GenerateCode(CodeDomProvider provider, CodeMemberMethod helper)
        {
            var compileUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace("Test");
            var clientType = new CodeTypeDeclaration("AccountClient");
            clientType.Members.Add(helper);
            codeNamespace.Types.Add(clientType);
            compileUnit.Namespaces.Add(codeNamespace);

            using (provider)
            using (var writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
                return writer.ToString();
            }
        }

        private static CodeTypeDeclaration CreateMessageType(string typeName, string fieldName, CodeTypeReference fieldType)
        {
            var messageType = new CodeTypeDeclaration(typeName);
            messageType.Members.Add(new CodeMemberField(fieldType, fieldName));
            return messageType;
        }
    }
}

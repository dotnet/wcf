// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeDom;
using System.Collections;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class CodeDomVisitor
    {
        public static void Visit(CodeDomVisitor[] visitors, CodeCompileUnit code)
        {
            new Enumerator(visitors).Enumerate(code);
        }

        private class Enumerator : CodeDomVisitor
        {
            private CodeDomVisitor[] _visitors;
            public Enumerator(CodeDomVisitor[] visitors)
            {
                _visitors = visitors;
            }

            public void Enumerate(CodeCompileUnit code)
            {
                Visit((object)code);
                CallFinishVisit(code);
            }

            private void Enumerate(object o)
            {
                Visit(o);
            }

            private void Enumerate(IEnumerable coll)
            {
                foreach (object o in coll)
                {
                    Visit(o);
                }
            }

            private void CallVisitors(object obj)
            {
                for (int i = 0; i < _visitors.Length; i++)
                {
                    _visitors[i].Visit(obj);
                }
            }

            private void CallFinishVisit(CodeCompileUnit cu)
            {
                for (int i = 0; i < _visitors.Length; i++)
                {
                    _visitors[i].FinishVisit(cu);
                }
            }

            protected override void Visit(object obj)
            {
                CallVisitors(obj);

                base.Visit(obj);
            }

            #region enumerating visitor overrides
            protected override void Visit(CodeArgumentReferenceExpression expr)
            {
                base.Visit(expr);
            }

            protected override void Visit(CodeArrayCreateExpression expr)
            {
                Enumerate(expr.CreateType);
                Enumerate(expr.Initializers);
                Enumerate(expr.SizeExpression);
                base.Visit(expr);
            }

            protected override void Visit(CodeArrayIndexerExpression expr)
            {
                Enumerate(expr.Indices);
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodeAssignStatement statement)
            {
                Enumerate(statement.Right);
                Enumerate(statement.Left);
                base.Visit(statement);
            }

            protected override void Visit(CodeAttachEventStatement statement)
            {
                Enumerate(statement.Listener);
                Enumerate(statement.Event);
                base.Visit(statement);
            }

            protected override void Visit(CodeAttributeArgument attrarg)
            {
                Enumerate(attrarg.Value);
                base.Visit(attrarg);
            }

            protected override void Visit(CodeAttributeDeclaration attr)
            {
                Enumerate(attr.Arguments);
                Enumerate(attr.AttributeType);
                base.Visit(attr);
            }

            protected override void Visit(CodeBaseReferenceExpression expr)
            {
                base.Visit(expr);
            }

            protected override void Visit(CodeBinaryOperatorExpression expr)
            {
                Enumerate(expr.Right);
                Enumerate(expr.Left);
                base.Visit(expr);
            }

            protected override void Visit(CodeCastExpression expr)
            {
                Enumerate(expr.Expression);
                Enumerate(expr.TargetType);
                base.Visit(expr);
            }

            protected override void Visit(CodeCatchClause clause)
            {
                Enumerate(clause.CatchExceptionType);
                Enumerate(clause.Statements);
                base.Visit(clause);
            }

            protected override void Visit(CodeChecksumPragma directive)
            {
                base.Visit(directive);
            }

            protected override void Visit(CodeComment comment)
            {
                base.Visit(comment);
            }

            protected override void Visit(CodeCommentStatement statement)
            {
                Enumerate(statement.Comment);
                base.Visit(statement);
            }

            protected override void Visit(CodeCompileUnit cu)
            {
                Enumerate(cu.StartDirectives);
                Enumerate(cu.AssemblyCustomAttributes);
                Enumerate(cu.Namespaces);
                Enumerate(cu.EndDirectives);
                base.Visit(cu);
            }

            protected override void Visit(CodeConditionStatement statement)
            {
                Enumerate(statement.Condition);
                Enumerate(statement.TrueStatements);
                Enumerate(statement.FalseStatements);
                base.Visit(statement);
            }

            protected override void Visit(CodeConstructor ctor)
            {
                Enumerate(ctor.BaseConstructorArgs);
                Enumerate(ctor.ChainedConstructorArgs);
                base.Visit(ctor);
            }

            protected override void Visit(CodeDefaultValueExpression expr)
            {
                Enumerate(expr.Type);
                base.Visit(expr);
            }

            protected override void Visit(CodeDelegateCreateExpression expr)
            {
                Enumerate(expr.DelegateType);
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodeDelegateInvokeExpression expr)
            {
                Enumerate(expr.Parameters);
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodeDirectionExpression expr)
            {
                Enumerate(expr.Expression);
                base.Visit(expr);
            }

            protected override void Visit(CodeDirective directive)
            {
                base.Visit(directive);
            }

            protected override void Visit(CodeEntryPointMethod method)
            {
                base.Visit(method);
            }

            protected override void Visit(CodeEventReferenceExpression expr)
            {
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodeExpression expr)
            {
                base.Visit(expr);
            }

            protected override void Visit(CodeExpressionStatement statement)
            {
                Enumerate(statement.Expression);
                base.Visit(statement);
            }

            protected override void Visit(CodeFieldReferenceExpression expr)
            {
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodeGotoStatement statement)
            {
                base.Visit(statement);
            }

            protected override void Visit(CodeIndexerExpression expr)
            {
                Enumerate(expr.Indices);
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodeIterationStatement statement)
            {
                Enumerate(statement.InitStatement);
                Enumerate(statement.TestExpression);
                Enumerate(statement.Statements);
                Enumerate(statement.TestExpression);
                base.Visit(statement);
            }

            protected override void Visit(CodeLabeledStatement statement)
            {
                Enumerate(statement.Statement);
                base.Visit(statement);
            }

            protected override void Visit(CodeLinePragma pragma)
            {
                base.Visit(pragma);
            }

            protected override void Visit(CodeMemberEvent evt)
            {
                Enumerate(evt.Type);
                Enumerate(evt.ImplementationTypes);
                Enumerate(evt.PrivateImplementationType);
                base.Visit(evt);
            }

            protected override void Visit(CodeMemberField field)
            {
                Enumerate(field.InitExpression);
                Enumerate(field.Type);
                base.Visit(field);
            }

            protected override void Visit(CodeMemberMethod method)
            {
                Enumerate(method.TypeParameters);
                Enumerate(method.Parameters);
                Enumerate(method.ReturnType);
                Enumerate(method.ReturnTypeCustomAttributes);
                Enumerate(method.Statements);
                Enumerate(method.ImplementationTypes);
                Enumerate(method.PrivateImplementationType);
                base.Visit(method);
            }

            protected override void Visit(CodeMemberProperty prop)
            {
                Enumerate(prop.Parameters);
                Enumerate(prop.Type);
                Enumerate(prop.GetStatements);
                Enumerate(prop.SetStatements);
                Enumerate(prop.ImplementationTypes);
                Enumerate(prop.PrivateImplementationType);
                base.Visit(prop);
            }

            protected override void Visit(CodeMethodInvokeExpression expr)
            {
                Enumerate(expr.Parameters);
                Enumerate(expr.Method);
                base.Visit(expr);
            }

            protected override void Visit(CodeMethodReferenceExpression expr)
            {
                Enumerate(expr.TypeArguments);
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodeMethodReturnStatement statement)
            {
                Enumerate(statement.Expression);
                base.Visit(statement);
            }

            protected override void Visit(CodeNamespace ns)
            {
                Enumerate(ns.Comments);
                Enumerate(ns.Imports);
                EnumerateTypes(ns);
                base.Visit(ns);
            }

            private void EnumerateTypes(CodeNamespace ns)
            {
                foreach (CodeObject codeObj in ns.Types)
                {
                    codeObj.UserData["Namespace"] = ns;
                }
                Enumerate(ns.Types);
            }


            protected override void Visit(CodeNamespaceImport ns)
            {
                Enumerate(ns.LinePragma);
                base.Visit(ns);
            }

            protected override void Visit(CodeObject obj)
            {
                // CONSIDER: UserData
                base.Visit(obj);
            }

            protected override void Visit(CodeObjectCreateExpression expr)
            {
                Enumerate(expr.Parameters);
                Enumerate(expr.CreateType);
                base.Visit(expr);
            }

            protected override void Visit(CodeParameterDeclarationExpression expr)
            {
                Enumerate(expr.Type);
                Enumerate(expr.CustomAttributes);
                base.Visit(expr);
            }

            protected override void Visit(CodePrimitiveExpression expr)
            {
                base.Visit(expr);
            }

            protected override void Visit(CodePropertyReferenceExpression expr)
            {
                Enumerate(expr.TargetObject);
                base.Visit(expr);
            }

            protected override void Visit(CodePropertySetValueReferenceExpression expr)
            {
                base.Visit(expr);
            }

            protected override void Visit(CodeRegionDirective directive)
            {
                base.Visit(directive);
            }

            protected override void Visit(CodeRemoveEventStatement statement)
            {
                Enumerate(statement.Listener);
                Enumerate(statement.Event);
                base.Visit(statement);
            }

            protected override void Visit(CodeSnippetCompileUnit cu)
            {
                Enumerate(cu.LinePragma);
                base.Visit(cu);
            }

            protected override void Visit(CodeSnippetExpression expr)
            {
                base.Visit(expr);
            }

            protected override void Visit(CodeSnippetStatement statement)
            {
                base.Visit(statement);
            }

            protected override void Visit(CodeSnippetTypeMember snippet)
            {
                base.Visit(snippet);
            }

            protected override void Visit(CodeStatement statement)
            {
                Enumerate(statement.StartDirectives);
                Enumerate(statement.LinePragma);
                Enumerate(statement.EndDirectives);
                base.Visit(statement);
            }

            protected override void Visit(CodeThisReferenceExpression expr)
            {
                base.Visit(expr);
            }

            protected override void Visit(CodeThrowExceptionStatement statement)
            {
                Enumerate(statement.ToThrow);
                base.Visit(statement);
            }

            protected override void Visit(CodeTryCatchFinallyStatement statement)
            {
                Enumerate(statement.TryStatements);
                Enumerate(statement.CatchClauses);
                Enumerate(statement.FinallyStatements);
                base.Visit(statement);
            }

            protected override void Visit(CodeTypeConstructor cctor)
            {
                base.Visit(cctor);
            }

            protected override void Visit(CodeTypeDeclaration type)
            {
                Enumerate(type.BaseTypes);
                Enumerate(type.TypeParameters);
                Enumerate(type.Members);
                base.Visit(type);
            }

            protected override void Visit(CodeTypeDelegate del)
            {
                Enumerate(del.Parameters);
                Enumerate(del.ReturnType);
                base.Visit(del);
            }

            protected override void Visit(CodeTypeMember member)
            {
                Enumerate(member.StartDirectives);
                Enumerate(member.Comments);
                Enumerate(member.CustomAttributes);
                Enumerate(member.LinePragma);
                Enumerate(member.EndDirectives);
                base.Visit(member);
            }

            protected override void Visit(CodeTypeOfExpression expr)
            {
                Enumerate(expr.Type);
                base.Visit(expr);
            }

            protected override void Visit(CodeTypeParameter typeparam)
            {
                Enumerate(typeparam.Constraints);
                Enumerate(typeparam.CustomAttributes);
                base.Visit(typeparam);
            }

            protected override void Visit(CodeTypeReference typeref)
            {
                Enumerate(typeref.ArrayElementType);
                Enumerate(typeref.TypeArguments);
                base.Visit(typeref);
            }

            protected override void Visit(CodeTypeReferenceExpression expr)
            {
                Enumerate(expr.Type);
                base.Visit(expr);
            }

            protected override void Visit(CodeVariableDeclarationStatement statement)
            {
                Enumerate(statement.Type);
                Enumerate(statement.InitExpression);
                base.Visit(statement);
            }

            protected override void Visit(CodeVariableReferenceExpression expr)
            {
                base.Visit(expr);
            }

            #endregion // enumerating visitor overrides
        }

        protected virtual void Visit(object obj)
        {
            if (obj is CodeAttributeArgument)
                Visit((CodeAttributeArgument)obj);
            else if (obj is CodeAttributeDeclaration)
                Visit((CodeAttributeDeclaration)obj);
            else if (obj is CodeCatchClause)
                Visit((CodeCatchClause)obj);
            else if (obj is CodeLinePragma)
                Visit((CodeLinePragma)obj);
            else if (obj is CodeObject)
                Visit((CodeObject)obj);
        }

        #region descendants of object
        protected virtual void Visit(CodeAttributeArgument attrarg) { }
        protected virtual void Visit(CodeAttributeDeclaration attr) { }
        protected virtual void Visit(CodeCatchClause clause) { }
        protected virtual void Visit(CodeLinePragma pragma) { }
        protected virtual void Visit(CodeObject obj)
        {
            if (obj is CodeComment)
                Visit((CodeComment)obj);
            else if (obj is CodeCompileUnit)
                Visit((CodeCompileUnit)obj);
            else if (obj is CodeDirective)
                Visit((CodeDirective)obj);
            else if (obj is CodeExpression)
                Visit((CodeExpression)obj);
            else if (obj is CodeNamespace)
                Visit((CodeNamespace)obj);
            else if (obj is CodeNamespaceImport)
                Visit((CodeNamespaceImport)obj);
            else if (obj is CodeStatement)
                Visit((CodeStatement)obj);
            else if (obj is CodeTypeMember)
                Visit((CodeTypeMember)obj);
            else if (obj is CodeTypeParameter)
                Visit((CodeTypeParameter)obj);
            else if (obj is CodeTypeReference)
                Visit((CodeTypeReference)obj);
        }

        #region descendants of CodeObject
        protected virtual void Visit(CodeComment comment) { }
        protected virtual void Visit(CodeCompileUnit cu)
        {
            if (cu is CodeSnippetCompileUnit)
                Visit((CodeSnippetCompileUnit)cu);
        }
        #region descendants of CodeCompileUnit
        protected virtual void Visit(CodeSnippetCompileUnit cu) { }
        #endregion // descendants of CodeCompileUnit
        protected virtual void Visit(CodeDirective directive)
        {
            if (directive is CodeChecksumPragma)
                Visit((CodeChecksumPragma)directive);
            else if (directive is CodeRegionDirective)
                Visit((CodeRegionDirective)directive);
        }

        #region descendants of CodeDirective
        protected virtual void Visit(CodeChecksumPragma directive) { }
        protected virtual void Visit(CodeRegionDirective directive) { }

        #endregion // descendants of CodeDirective
        protected virtual void Visit(CodeExpression expr)
        {
            if (expr is CodeArgumentReferenceExpression)
                Visit((CodeArgumentReferenceExpression)expr);
            else if (expr is CodeArrayCreateExpression)
                Visit((CodeArrayCreateExpression)expr);
            else if (expr is CodeArrayIndexerExpression)
                Visit((CodeArrayIndexerExpression)expr);
            else if (expr is CodeBaseReferenceExpression)
                Visit((CodeBaseReferenceExpression)expr);
            else if (expr is CodeBinaryOperatorExpression)
                Visit((CodeBinaryOperatorExpression)expr);

            else if (expr is CodeCastExpression)
                Visit((CodeCastExpression)expr);
            else if (expr is CodeDefaultValueExpression)
                Visit((CodeDefaultValueExpression)expr);
            else if (expr is CodeDelegateCreateExpression)
                Visit((CodeDelegateCreateExpression)expr);
            else if (expr is CodeDelegateInvokeExpression)
                Visit((CodeDelegateInvokeExpression)expr);
            else if (expr is CodeDirectionExpression)
                Visit((CodeDirectionExpression)expr);

            else if (expr is CodeEventReferenceExpression)
                Visit((CodeEventReferenceExpression)expr);
            else if (expr is CodeFieldReferenceExpression)
                Visit((CodeFieldReferenceExpression)expr);
            else if (expr is CodeIndexerExpression)
                Visit((CodeIndexerExpression)expr);
            else if (expr is CodeMethodInvokeExpression)
                Visit((CodeMethodInvokeExpression)expr);
            else if (expr is CodeMethodReferenceExpression)
                Visit((CodeMethodReferenceExpression)expr);

            else if (expr is CodeObjectCreateExpression)
                Visit((CodeObjectCreateExpression)expr);
            else if (expr is CodeParameterDeclarationExpression)
                Visit((CodeParameterDeclarationExpression)expr);
            else if (expr is CodePrimitiveExpression)
                Visit((CodePrimitiveExpression)expr);
            else if (expr is CodePropertyReferenceExpression)
                Visit((CodePropertyReferenceExpression)expr);
            else if (expr is CodePropertySetValueReferenceExpression)
                Visit((CodePropertySetValueReferenceExpression)expr);

            else if (expr is CodeSnippetExpression)
                Visit((CodeSnippetExpression)expr);
            else if (expr is CodeThisReferenceExpression)
                Visit((CodeThisReferenceExpression)expr);
            else if (expr is CodeTypeOfExpression)
                Visit((CodeTypeOfExpression)expr);
            else if (expr is CodeTypeReferenceExpression)
                Visit((CodeTypeReferenceExpression)expr);
            else if (expr is CodeVariableReferenceExpression)
                Visit((CodeVariableReferenceExpression)expr);
        }
        #region descendants of CodeExpression
        protected virtual void Visit(CodeArgumentReferenceExpression expr) { }
        protected virtual void Visit(CodeArrayCreateExpression expr) { }
        protected virtual void Visit(CodeArrayIndexerExpression expr) { }
        protected virtual void Visit(CodeBaseReferenceExpression expr) { }
        protected virtual void Visit(CodeBinaryOperatorExpression expr) { }

        protected virtual void Visit(CodeCastExpression expr) { }
        protected virtual void Visit(CodeDefaultValueExpression expr) { }
        protected virtual void Visit(CodeDelegateCreateExpression expr) { }
        protected virtual void Visit(CodeDelegateInvokeExpression expr) { }
        protected virtual void Visit(CodeDirectionExpression expr) { }

        protected virtual void Visit(CodeEventReferenceExpression expr) { }
        protected virtual void Visit(CodeFieldReferenceExpression expr) { }
        protected virtual void Visit(CodeIndexerExpression expr) { }
        protected virtual void Visit(CodeMethodInvokeExpression expr) { }
        protected virtual void Visit(CodeMethodReferenceExpression expr) { }

        protected virtual void Visit(CodeObjectCreateExpression expr) { }
        protected virtual void Visit(CodeParameterDeclarationExpression expr) { }
        protected virtual void Visit(CodePrimitiveExpression expr) { }
        protected virtual void Visit(CodePropertyReferenceExpression expr) { }
        protected virtual void Visit(CodePropertySetValueReferenceExpression expr) { }

        protected virtual void Visit(CodeSnippetExpression expr) { }
        protected virtual void Visit(CodeThisReferenceExpression expr) { }
        protected virtual void Visit(CodeTypeOfExpression expr) { }
        protected virtual void Visit(CodeTypeReferenceExpression expr) { }
        protected virtual void Visit(CodeVariableReferenceExpression expr) { }
        #endregion // descendants of CodeExpression

        protected virtual void Visit(CodeNamespace ns) { }
        protected virtual void Visit(CodeNamespaceImport ns) { }
        protected virtual void Visit(CodeStatement statement)
        {
            if (statement is CodeAssignStatement)
                Visit((CodeAssignStatement)statement);
            else if (statement is CodeAttachEventStatement)
                Visit((CodeAttachEventStatement)statement);
            else if (statement is CodeCommentStatement)
                Visit((CodeCommentStatement)statement);
            else if (statement is CodeConditionStatement)
                Visit((CodeConditionStatement)statement);
            else if (statement is CodeExpressionStatement)
                Visit((CodeExpressionStatement)statement);

            else if (statement is CodeGotoStatement)
                Visit((CodeGotoStatement)statement);
            else if (statement is CodeIterationStatement)
                Visit((CodeIterationStatement)statement);
            else if (statement is CodeLabeledStatement)
                Visit((CodeLabeledStatement)statement);
            else if (statement is CodeMethodReturnStatement)
                Visit((CodeMethodReturnStatement)statement);
            else if (statement is CodeRemoveEventStatement)
                Visit((CodeRemoveEventStatement)statement);

            else if (statement is CodeSnippetStatement)
                Visit((CodeSnippetStatement)statement);
            else if (statement is CodeThrowExceptionStatement)
                Visit((CodeThrowExceptionStatement)statement);
            else if (statement is CodeTryCatchFinallyStatement)
                Visit((CodeTryCatchFinallyStatement)statement);
            else if (statement is CodeVariableDeclarationStatement)
                Visit((CodeVariableDeclarationStatement)statement);
        }

        #region descendants of CodeStatement
        protected virtual void Visit(CodeAssignStatement statement) { }
        protected virtual void Visit(CodeAttachEventStatement statement) { }
        protected virtual void Visit(CodeCommentStatement statement) { }
        protected virtual void Visit(CodeConditionStatement statement) { }
        protected virtual void Visit(CodeExpressionStatement statement) { }

        protected virtual void Visit(CodeGotoStatement statement) { }
        protected virtual void Visit(CodeIterationStatement statement) { }
        protected virtual void Visit(CodeLabeledStatement statement) { }
        protected virtual void Visit(CodeMethodReturnStatement statement) { }
        protected virtual void Visit(CodeRemoveEventStatement statement) { }

        protected virtual void Visit(CodeSnippetStatement statement) { }
        protected virtual void Visit(CodeThrowExceptionStatement statement) { }
        protected virtual void Visit(CodeTryCatchFinallyStatement statement) { }
        protected virtual void Visit(CodeVariableDeclarationStatement statement) { }
        #endregion // descendants of CodeStatement

        protected virtual void Visit(CodeTypeMember member)
        {
            if (member is CodeMemberEvent)
                Visit((CodeMemberEvent)member);
            else if (member is CodeMemberField)
                Visit((CodeMemberField)member);
            else if (member is CodeMemberMethod)
                Visit((CodeMemberMethod)member);
            else if (member is CodeMemberProperty)
                Visit((CodeMemberProperty)member);
            else if (member is CodeSnippetTypeMember)
                Visit((CodeSnippetTypeMember)member);
            else if (member is CodeTypeDeclaration)
                Visit((CodeTypeDeclaration)member);
        }

        #region descendants of CodeTypeMember
        protected virtual void Visit(CodeMemberEvent evt) { }
        protected virtual void Visit(CodeMemberField field) { }
        protected virtual void Visit(CodeMemberMethod method)
        {
            if (method is CodeConstructor)
                Visit((CodeConstructor)method);
            else if (method is CodeEntryPointMethod)
                Visit((CodeEntryPointMethod)method);
            else if (method is CodeTypeConstructor)
                Visit((CodeTypeConstructor)method);
        }

        #region descendants of CodeMemberMethod
        protected virtual void Visit(CodeConstructor ctor) { }
        protected virtual void Visit(CodeEntryPointMethod method) { }
        protected virtual void Visit(CodeTypeConstructor cctor) { }
        #endregion // descendants of CodeMemberMethod

        protected virtual void Visit(CodeMemberProperty prop) { }
        protected virtual void Visit(CodeSnippetTypeMember snippet) { }
        protected virtual void Visit(CodeTypeDeclaration type)
        {
            if (type is CodeTypeDelegate)
                Visit((CodeTypeDelegate)type);
        }

        #region descendants of CodeTypeDeclaration
        protected virtual void Visit(CodeTypeDelegate del) { }
        #endregion // descendants of CodeTypeDeclaration
        #endregion // descendants of CodeTypeMember

        protected virtual void Visit(CodeTypeParameter typeparam) { }
        protected virtual void Visit(CodeTypeReference typeref) { }

        #endregion // descendants of CodeObject
        #endregion // descendants of object

        protected virtual void FinishVisit(CodeCompileUnit cu) { }
    }
}

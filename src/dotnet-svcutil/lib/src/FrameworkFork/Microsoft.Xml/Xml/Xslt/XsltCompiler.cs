// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// Not needed in dotnet-svcutil scenario. 
// //------------------------------------------------------------------------------
// // <copyright file="XsltCompiler.cs" company="Microsoft">
// //     Copyright (c) Microsoft Corporation.  All rights reserved.
// // </copyright>
// // <owner current="true" primary="true">antonl</owner>
// //------------------------------------------------------------------------------
// 
// using Microsoft.CodeDom.Compiler;
// using System.Diagnostics;
// using System.Reflection.Emit;
// using Microsoft.Xml.Xsl.Qil;
// // using Microsoft.Xml.Xsl.Runtime;
// using Microsoft.Xml.Xsl.Xslt;
// 
// namespace Microsoft.Xml.Xsl {
// 				using System;
// 				using Microsoft.Xml;
// 
// 
//     //----------------------------------------------------------------------------------------------------
//     //  Clarification on null values in this API:
//     //      stylesheet              - cannot be null
//     //      settings                - if null, XsltSettings.Default will be used
//     //      stylesheetResolver      - if null, XmlNullResolver will be used for includes/imports
//     //      typeBuilder             - cannot be null
//     //----------------------------------------------------------------------------------------------------
// 
//     /// <summary>
//     /// Compiles XSLT stylesheet into a TypeBuilder
//     /// </summary>
//     public class XsltCompiler {
//         public static CompilerErrorCollection CompileToType(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver, TypeBuilder typeBuilder) {
//             if (stylesheet == null)
//                 throw new ArgumentNullException("stylesheet");
// 
//             if (typeBuilder == null)
//                 throw new ArgumentNullException("typeBuilder");
// 
//             if (settings == null)
//                 settings = XsltSettings.Default;
// 
//             CompilerErrorCollection errors;
//             QilExpression qil;
// 
//             // Get DebuggableAttribute of the assembly. If there are many of them, JIT seems to pick up a random one.
//             // I could not discover any pattern here, so let's take the first attribute found.
//             object[] debuggableAttrs = typeBuilder.Assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);
//             bool debug = debuggableAttrs.Length > 0 && ((DebuggableAttribute) debuggableAttrs[0]).IsJITTrackingEnabled;
// 
//             errors = new Compiler(settings, debug).Compile(stylesheet, stylesheetResolver, out qil).Errors;
// 
//             if (!errors.HasErrors) {
//                 new XmlILGenerator().Generate(qil, typeBuilder);
//             }
// 
//             return errors;
//         }
//     }
// }

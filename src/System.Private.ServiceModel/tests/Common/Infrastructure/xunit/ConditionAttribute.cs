// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace Infrastructure.Common
{
    // The [Condition] attribute can be applied to any test method that
    // is marked with [WcfFact] or [WcfTheory].  It provides zero or more
    // member names to be evaluated at runtime to determine whether the
    // test should be run or skipped.
    // Examples: 
    //  [Condition(nameof(Root_Certificate_Installed))]
    //  [Condition(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ConditionAttribute : WcfSkippableAttribute
    {
        public string[] Conditions { get; private set; }

        public ConditionAttribute(params string[] conditions)
        {
            Conditions = conditions;
        }

        public override string GetSkipReason(ITestMethod testMethod)
        {
            // A null or empty list of conditions is treated as "no conditions",
            // and the test cases will not be skipped.
            // Example: [Condition()] or [Condition((string[]) null)]
            int conditionCount = Conditions == null ? 0 : Conditions.Length;
            if (conditionCount == 0)
            {
                return null;
            }

            MethodInfo testMethodInfo = testMethod.Method.ToRuntimeMethod();
            Type testMethodDeclaringType = testMethodInfo.DeclaringType;

            List<string> falseConditions = new List<string>(conditionCount);

            foreach (string entry in Conditions)
            {
                string conditionMemberName = entry;

                // Null condition member names are silently tolerated
                if (string.IsNullOrWhiteSpace(conditionMemberName))
                {
                    continue;
                }

                string[] symbols = conditionMemberName.Split('.');
                Type declaringType = testMethodDeclaringType;

                if (symbols.Length == 2)
                {
                    conditionMemberName = symbols[1];
                    ITypeInfo type = testMethod.TestClass.Class.Assembly.GetTypes(false).Where(t => t.Name.Contains(symbols[0])).SingleOrDefault();
                    if (type != null)
                    {
                        declaringType = type.ToRuntimeType();
                    }
                }

                MethodInfo conditionMethodInfo;
                if ((conditionMethodInfo = LookupConditionalMethod(declaringType, conditionMemberName)) == null)
                {
                    falseConditions.Add(String.Format("Condition \"{0}\" not found in type \"{1}\".",
                                                      conditionMemberName, testMethodDeclaringType.FullName));
                    continue;
                }

                // In the case of multiple conditions, collect the results of all
                // of them to produce a summary skip reason.
                try
                {
                    if (!(bool)conditionMethodInfo.Invoke(null, null))
                    {
                        falseConditions.Add(conditionMemberName);
                    }
                }
                catch (Exception exc)
                {
                    falseConditions.Add(String.Format("Condition \"{0}\" threw exception {1}: \"{2}\".",
                                                      conditionMemberName, 
                                                      exc.GetType().Name,
                                                      exc.Message));
                }
            }

            if (falseConditions.Count == 0)
            {
                return null;
            }

            return String.Format("{0}", String.Join(", ", falseConditions));
        }


        internal static MethodInfo LookupConditionalMethod(Type t, string name)
        {
            if (t == null || name == null)
                return null;

            TypeInfo ti = t.GetTypeInfo();

            MethodInfo mi = ti.GetDeclaredMethod(name);
            if (mi != null && mi.IsStatic && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(bool))
                return mi;

            PropertyInfo pi = ti.GetDeclaredProperty(name);
            if (pi != null && pi.PropertyType == typeof(bool) && pi.GetMethod != null && pi.GetMethod.IsStatic && pi.GetMethod.GetParameters().Length == 0)
                return pi.GetMethod;

            return LookupConditionalMethod(ti.BaseType, name);
        }
    }
}

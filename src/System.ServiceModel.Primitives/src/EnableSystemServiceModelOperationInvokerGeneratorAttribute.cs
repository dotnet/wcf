// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class EnableSystemServiceModelOperationInvokerGeneratorAttribute : Attribute
    {
        public string Value { get; }

        public EnableSystemServiceModelOperationInvokerGeneratorAttribute(string value)
        {
            Value = value;
        }
    }
}

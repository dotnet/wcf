// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreDataMemberAttribute : Attribute
    {
        public IgnoreDataMemberAttribute()
        {
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
    public sealed class KnownTypeAttribute : Attribute
    {
        private string _methodName;
        private Type _type;

        private KnownTypeAttribute()
        {
            // Disallow default constructor
        }

        public KnownTypeAttribute(Type type)
        {
            _type = type;
        }

        public KnownTypeAttribute(string methodName)
        {
            _methodName = methodName;
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}


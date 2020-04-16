// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.ServiceContract | ServiceModelAttributeTargets.OperationContract, Inherited = true, AllowMultiple = true)]
    public sealed class ServiceKnownTypeAttribute : Attribute
    {
        private Type _declaringType;
        private string _methodName;
        private Type _type;

        private ServiceKnownTypeAttribute()
        {
            // Disallow default constructor
        }

        public ServiceKnownTypeAttribute(Type type)
        {
            _type = type;
        }

        public ServiceKnownTypeAttribute(string methodName)
        {
            _methodName = methodName;
        }

        public ServiceKnownTypeAttribute(string methodName, Type declaringType)
        {
            _methodName = methodName;
            _declaringType = declaringType;
        }

        public Type DeclaringType
        {
            get { return _declaringType; }
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


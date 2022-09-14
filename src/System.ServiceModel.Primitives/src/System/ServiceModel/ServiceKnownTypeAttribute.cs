// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.ServiceContract | ServiceModelAttributeTargets.OperationContract, Inherited = true, AllowMultiple = true)]
    public sealed class ServiceKnownTypeAttribute : Attribute
    {
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
            MethodName = methodName;
        }

        public ServiceKnownTypeAttribute(string methodName, Type declaringType)
        {
            MethodName = methodName;
            DeclaringType = declaringType;
        }

        public Type DeclaringType { get; }

        public string MethodName { get; }

        public Type Type
        {
            get { return _type; }
        }
    }
}


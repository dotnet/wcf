// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace System.ServiceModel.Description
{
    internal class CustomAttributeProvider
    {
        private enum AttributeProviderType
        {
            Unknown,
            Type,
            MethodInfo,
            MemberInfo,
            ParameterInfo,
        };

        private object _attrProvider;

        private CustomAttributeProvider(object attrProvider)
        {
            _attrProvider = attrProvider;
            if (attrProvider is Type)
            {
                this.Type = (Type)attrProvider;
                ProviderType = AttributeProviderType.Type;
            }
            else if (attrProvider is MethodInfo)
            {
                this.MethodInfo = (MethodInfo)attrProvider;
                ProviderType = AttributeProviderType.MethodInfo;
            }
            else if (attrProvider is MemberInfo)
            {
                this.MemberInfo = (MemberInfo)attrProvider;
                ProviderType = AttributeProviderType.MemberInfo;
            }
            else if (attrProvider is ParameterInfo)
            {
                this.ParameterInfo = (ParameterInfo)attrProvider;
                ProviderType = AttributeProviderType.ParameterInfo;
            }
            else
            {
                throw ExceptionHelper.AsError(new ArgumentException());
            }
        }

        private AttributeProviderType ProviderType { get; set; }
        internal Type Type { get; private set; }
        internal MemberInfo MemberInfo { get; private set; }
        internal MethodInfo MethodInfo { get; private set; }
        internal ParameterInfo ParameterInfo { get; private set; }

        public object[] GetCustomAttributes(bool inherit)
        {
            switch (this.ProviderType)
            {
                case AttributeProviderType.Type:
                    return this.Type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
                case AttributeProviderType.MethodInfo:
                    return this.MethodInfo.GetCustomAttributes(inherit).ToArray();
                case AttributeProviderType.MemberInfo:
                    return this.MemberInfo.GetCustomAttributes(inherit).ToArray();
                case AttributeProviderType.ParameterInfo:
                    return this.ParameterInfo.GetCustomAttributes(inherit).ToArray();
            }
            Contract.Assert(false, "This should never execute.");
            throw ExceptionHelper.PlatformNotSupported();
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            switch (this.ProviderType)
            {
                case AttributeProviderType.Type:
                    return this.Type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
                case AttributeProviderType.MethodInfo:
                    return this.MethodInfo.GetCustomAttributes(attributeType, inherit).ToArray();
                case AttributeProviderType.MemberInfo:
                    return this.MemberInfo.GetCustomAttributes(attributeType, inherit).ToArray();
                case AttributeProviderType.ParameterInfo:
                    //GetCustomAttributes could return null instead of empty collection for a known System.Relection issue, workaround the issue by explicitly checking the null
                    var customAttributes = this.ParameterInfo.GetCustomAttributes(attributeType, inherit);
                    return customAttributes == null ? null : customAttributes.ToArray();
            }
            Contract.Assert(false, "This should never execute.");
            throw ExceptionHelper.PlatformNotSupported();
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            switch (this.ProviderType)
            {
                case AttributeProviderType.Type:
                    return this.Type.GetTypeInfo().IsDefined(attributeType, inherit);
                case AttributeProviderType.MethodInfo:
                    return this.MethodInfo.IsDefined(attributeType, inherit);
                case AttributeProviderType.MemberInfo:
                    return this.MemberInfo.IsDefined(attributeType, inherit);
                case AttributeProviderType.ParameterInfo:
                    return this.ParameterInfo.IsDefined(attributeType, inherit);
            }
            Contract.Assert(false, "This should never execute.");
            throw ExceptionHelper.PlatformNotSupported();
        }

        public static implicit operator CustomAttributeProvider(MemberInfo attrProvider)
        {
            return new CustomAttributeProvider(attrProvider);
        }

        public static implicit operator CustomAttributeProvider(MethodInfo attrProvider)
        {
            return new CustomAttributeProvider(attrProvider);
        }

        public static implicit operator CustomAttributeProvider(ParameterInfo attrProvider)
        {
            return new CustomAttributeProvider(attrProvider);
        }

        public static implicit operator CustomAttributeProvider(Type attrProvider)
        {
            return new CustomAttributeProvider(attrProvider);
        }
    }
}

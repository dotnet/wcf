//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.Svcutil
{

    internal static class ConfigToCodeConstants
    {
        internal const string EndpointConfigurationParameter = "endpointConfiguration";
        internal const string BindingTypeParameter = "bindingType";

        internal const string GetEndpointAddressMethod = "GetEndpointAddress";
        internal const string GetBindingMethod = "GetBindingForEndpoint";

        internal const string GetDefaultBindingMethod = "GetDefaultBinding";
        internal const string GetDefaultEndpointAddressMethod = "GetDefaultEndpointAddress";

        internal const string GeneratedDuplexCtorCallbackName = "callbackImpl";
        internal const string ClientBaseOfTBaseName = "System.ServiceModel.ClientBase`1";
        internal const string DuplexClientBaseOfTBaseName = "System.ServiceModel.DuplexClientBase`1";

        internal const string EndpointConfigurationEnumTypeName = "EndpointConfiguration";

        internal const string EndpointPropertyName = "Endpoint";
        internal const string ToStringMethod = "ToString";
        internal const string ClientCredentialsPropertyName = "ClientCredentials";
        internal const string EndpointNamePropertyName = "Name";
    }
}

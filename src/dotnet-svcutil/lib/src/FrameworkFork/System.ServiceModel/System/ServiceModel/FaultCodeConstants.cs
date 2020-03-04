// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    internal class FaultCodeConstants
    {
        public static class Namespaces
        {
            public const string NetDispatch = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher";
            public const string Transactions = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/transactions";
        }

        public static class Codes
        {
            public const string DeserializationFailed = "DeserializationFailed";
            public const string SessionTerminated = "SessionTerminated";
            public const string InternalServiceFault = "InternalServiceFault";
        }

        public static class Actions
        {
            public const string NetDispatcher = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher/fault";
            public const string Transactions = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/transactions/fault";
        }
    }
}



// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum TransactionFlowOption
    {
        NotAllowed,
        Allowed,
        Mandatory,
    }

    internal static class TransactionFlowOptionHelper
    {
        public static bool IsDefined(TransactionFlowOption option)
        {
            return (option == TransactionFlowOption.NotAllowed ||
                    option == TransactionFlowOption.Allowed ||
                    option == TransactionFlowOption.Mandatory);
            //option == TransactionFlowOption.Ignore);
        }
        internal static bool AllowedOrRequired(TransactionFlowOption option)
        {
            return (option == TransactionFlowOption.Allowed ||
                    option == TransactionFlowOption.Mandatory);
        }
    }
}

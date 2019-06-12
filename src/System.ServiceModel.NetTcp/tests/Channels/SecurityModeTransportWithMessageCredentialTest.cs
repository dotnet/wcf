// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

namespace System.ServiceModel.NetTcp.Tests
{
    public static class SecurityModeTransportWithMessageCredentialTest
    {
        [WcfFact]
        public static void Init_TransportWithMessageCredentialSecurityMode()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.TransportWithMessageCredential);
            MessageSecurityOverTcp message = new MessageSecurityOverTcp();
            message.ClientCredentialType = MessageCredentialType.UserName;
            binding.Security.Message = message;
            BindingElementCollection bindingElements = binding.CreateBindingElements();
            SecurityBindingElement secutityBindingElement = bindingElements.Find<SecurityBindingElement>();

            Assert.StrictEqual(binding.Security.Mode, SecurityMode.TransportWithMessageCredential);
            Assert.True(secutityBindingElement != null, "SecurityBindingElement should not be null.");
            
            Assert.True(binding.Security.Message.AlgorithmSuite == SecurityAlgorithmSuite.Default, "AlgorithmSuite should be Default.");
            Assert.True(binding.Security.Message.ClientCredentialType == MessageCredentialType.UserName, "ClientCredentialType should be UserName.");

            message.ClientCredentialType = MessageCredentialType.Certificate;
            binding.Security.Message = message;
            Assert.True(binding.Security.Message.ClientCredentialType == MessageCredentialType.Certificate, "ClientCredentialType should be Certifiacte.");
        }
    }
}

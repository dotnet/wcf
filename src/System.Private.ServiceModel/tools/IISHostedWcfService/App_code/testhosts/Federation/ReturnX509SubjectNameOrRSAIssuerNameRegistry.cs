// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.IdentityModel.Tokens;
#else
using System.IdentityModel.Tokens;
#endif

namespace WcfService
{
    public class ReturnX509SubjectNameOrRSAIssuerNameRegistry : IssuerNameRegistry
    {
        public override string GetIssuerName(SecurityToken securityToken)
        {
            var x509Token = securityToken as X509SecurityToken;
            var rsaToken = securityToken as RsaSecurityToken;
            if (x509Token != null)
            {
                return x509Token.Certificate.SubjectName.Name;
            }
            else if (rsaToken != null)
            {
                return string.Format("RSA-token-{0}", rsaToken.Rsa.ToXmlString(false));
            }

            return null;
        }
    }
}

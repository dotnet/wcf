// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal sealed class RequestSecurityTokenResponseCollection : BodyWriter
    {
        private SecurityStandardsManager _standardsManager;

        public RequestSecurityTokenResponseCollection(IEnumerable<RequestSecurityTokenResponse> rstrCollection)
            : this(rstrCollection, SecurityStandardsManager.DefaultInstance)
        { }

        public RequestSecurityTokenResponseCollection(IEnumerable<RequestSecurityTokenResponse> rstrCollection, SecurityStandardsManager standardsManager)
            : base(true)
        {
            if (rstrCollection == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rstrCollection));
            }

            int index = 0;
            foreach (RequestSecurityTokenResponse rstr in rstrCollection)
            {
                if (rstr == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(String.Format(CultureInfo.InvariantCulture, "rstrCollection[{0}]", index));
                }

                ++index;
            }
            RstrCollection = rstrCollection;
            _standardsManager = standardsManager ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(standardsManager)));
        }

        public IEnumerable<RequestSecurityTokenResponse> RstrCollection { get; }

        public void WriteTo(XmlWriter writer)
        {
            _standardsManager.TrustDriver.WriteRequestSecurityTokenResponseCollection(this, writer);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            WriteTo(writer);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class SecurityHeaderElementInferenceEngine
    {
        public abstract void ExecuteProcessingPasses(ReceiveSecurityHeader securityHeader, XmlDictionaryReader reader);

        public abstract void MarkElements(ReceiveSecurityHeaderElementManager elementManager, bool messageSecurityMode);

        public static SecurityHeaderElementInferenceEngine GetInferenceEngine(SecurityHeaderLayout layout)
        {
            SecurityHeaderLayoutHelper.Validate(layout);
            switch (layout)
            {
                case SecurityHeaderLayout.Strict:
                    return StrictModeSecurityHeaderElementInferenceEngine.Instance;
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(layout)));
            }
        }
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal sealed class StrictModeSecurityHeaderElementInferenceEngine : SecurityHeaderElementInferenceEngine
    {
        private static StrictModeSecurityHeaderElementInferenceEngine s_instance = new StrictModeSecurityHeaderElementInferenceEngine();

        private StrictModeSecurityHeaderElementInferenceEngine() { }

        internal static StrictModeSecurityHeaderElementInferenceEngine Instance
        {
            get { return s_instance; }
        }

        public override void ExecuteProcessingPasses(ReceiveSecurityHeader securityHeader, XmlDictionaryReader reader)
        {
            securityHeader.ExecuteFullPass(reader);
        }

        public override void MarkElements(ReceiveSecurityHeaderElementManager elementManager, bool messageSecurityMode)
        {
            bool primarySignatureFound = false;
            for (int position = 0; position < elementManager.Count; position++)
            {
                ReceiveSecurityHeaderEntry entry;
                elementManager.GetElementEntry(position, out entry);
                if (entry.elementCategory == ReceiveSecurityHeaderElementCategory.Signature)
                {
                    if (!messageSecurityMode || primarySignatureFound)
                    {
                        elementManager.SetBindingMode(position, ReceiveSecurityHeaderBindingModes.Endorsing);
                    }
                    else
                    {
                        elementManager.SetBindingMode(position, ReceiveSecurityHeaderBindingModes.Primary);
                        primarySignatureFound = true;
                    }
                }
            }
        }
    }
}

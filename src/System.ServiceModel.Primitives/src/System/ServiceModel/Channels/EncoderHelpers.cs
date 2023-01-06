// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal static class EncoderHelpers
    {
        internal static XmlDictionaryReaderQuotas GetBufferedReadQuotas(XmlDictionaryReaderQuotas encoderQuotas)
        {
            XmlDictionaryReaderQuotas bufferedReadQuotas = new XmlDictionaryReaderQuotas();
            encoderQuotas.CopyTo(bufferedReadQuotas);

            // now we have the quotas from the encoder, we need to update the values with the new quotas from the default read quotas. 
            if (IsDefaultQuota(bufferedReadQuotas, XmlDictionaryReaderQuotaTypes.MaxStringContentLength))
            {
                bufferedReadQuotas.MaxStringContentLength = EncoderDefaults.BufferedReadDefaultMaxStringContentLength;
            }

            if (IsDefaultQuota(bufferedReadQuotas, XmlDictionaryReaderQuotaTypes.MaxArrayLength))
            {
                bufferedReadQuotas.MaxArrayLength = EncoderDefaults.BufferedReadDefaultMaxArrayLength;
            }

            if (IsDefaultQuota(bufferedReadQuotas, XmlDictionaryReaderQuotaTypes.MaxBytesPerRead))
            {
                bufferedReadQuotas.MaxBytesPerRead = EncoderDefaults.BufferedReadDefaultMaxBytesPerRead;
            }

            if (IsDefaultQuota(bufferedReadQuotas, XmlDictionaryReaderQuotaTypes.MaxNameTableCharCount))
            {
                bufferedReadQuotas.MaxNameTableCharCount = EncoderDefaults.BufferedReadDefaultMaxNameTableCharCount;
            }

            if (IsDefaultQuota(bufferedReadQuotas, XmlDictionaryReaderQuotaTypes.MaxDepth))
            {
                bufferedReadQuotas.MaxDepth = EncoderDefaults.BufferedReadDefaultMaxDepth;
            }

            return bufferedReadQuotas;
        }

        private static bool IsDefaultQuota(XmlDictionaryReaderQuotas quotas, XmlDictionaryReaderQuotaTypes quotaType)
        {
            switch (quotaType)
            {
                case XmlDictionaryReaderQuotaTypes.MaxDepth:
                case XmlDictionaryReaderQuotaTypes.MaxStringContentLength:
                case XmlDictionaryReaderQuotaTypes.MaxArrayLength:
                case XmlDictionaryReaderQuotaTypes.MaxBytesPerRead:
                case XmlDictionaryReaderQuotaTypes.MaxNameTableCharCount:
                    return (quotas.ModifiedQuotas & quotaType) == 0x00;
            }

            Fx.Assert("invalid quota type.");
            return false;
        }
    }
}

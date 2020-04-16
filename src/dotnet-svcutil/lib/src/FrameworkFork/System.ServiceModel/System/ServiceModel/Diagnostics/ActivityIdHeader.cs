// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime;
using System.ServiceModel.Channels;
using Microsoft.Xml;

namespace System.ServiceModel.Diagnostics
{
    internal class ActivityIdHeader : DictionaryHeader
    {
        private Guid _guid;
        private Guid _headerId;

        internal ActivityIdHeader(Guid activityId)
            : base()
        {
            _guid = activityId;
            _headerId = Guid.NewGuid();
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.ActivityIdFlowDictionary.ActivityId; }
        }

        public override XmlDictionaryString DictionaryNamespace
        {
            get { return XD.ActivityIdFlowDictionary.ActivityIdNamespace; }
        }

        internal static Guid ExtractActivityId(Message message)
        {
            Guid guid = Guid.Empty;
            try
            {
                if (message != null && message.State != MessageState.Closed && message.Headers != null)
                {
                    int index = message.Headers.FindHeader(DiagnosticStrings.ActivityId, DiagnosticStrings.DiagnosticsNamespace);

                    // Check the state again, in case the message was closed after we found the header
                    if (index >= 0)
                    {
                        using (XmlDictionaryReader reader = message.Headers.GetReaderAtHeader(index))
                        {
                            guid = reader.ReadElementContentAsGuid();
                        }
                    }
                }
            }
#pragma warning disable 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
            }

            return guid;
        }

        internal static bool ExtractActivityAndCorrelationId(Message message, out Guid activityId, out Guid correlationId)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            activityId = Guid.Empty;
            correlationId = Guid.Empty;

            try
            {
                if (message.State != MessageState.Closed && message.Headers != null)
                {
                    int index = message.Headers.FindHeader(DiagnosticStrings.ActivityId, DiagnosticStrings.DiagnosticsNamespace);

                    // Check the state again, in case the message was closed after we found the header
                    if (index >= 0)
                    {
                        using (XmlDictionaryReader reader = message.Headers.GetReaderAtHeader(index))
                        {
                            correlationId = Fx.CreateGuid(reader.GetAttribute("CorrelationId", null));
                            activityId = reader.ReadElementContentAsGuid();
                            return activityId != Guid.Empty;
                        }
                    }
                }
            }
#pragma warning disable 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
            }
            return false;
        }

        internal void AddTo(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            if (message.State != MessageState.Closed && message.Headers.MessageVersion.Envelope != EnvelopeVersion.None)
            {
                int index = message.Headers.FindHeader(DiagnosticStrings.ActivityId, DiagnosticStrings.DiagnosticsNamespace);
                if (index < 0)
                {
                    message.Headers.Add(this);
                }
            }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            writer.WriteAttributeString("CorrelationId", _headerId.ToString());
            writer.WriteValue(_guid);
        }
    }
}

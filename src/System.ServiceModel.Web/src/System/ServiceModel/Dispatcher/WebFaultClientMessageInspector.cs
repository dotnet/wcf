// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Dispatcher
{
    using System.Net;
    using System.ServiceModel.Channels;

    // Mirrors the .NET Framework System.ServiceModel.Web WebFaultClientMessageInspector:
    // if the server replied with HTTP 500, surface it as a CommunicationException on the
    // client so the call site doesn't silently see an empty payload.
    internal class WebFaultClientMessageInspector : IClientMessageInspector
    {
        public virtual void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply != null)
            {
                HttpResponseMessageProperty prop = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
                if (prop != null && prop.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(prop.StatusDescription));
                }
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }
    }
}

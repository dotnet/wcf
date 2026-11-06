// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Web
{
    public class WebOperationContext : IExtension<OperationContext>
    {
        private readonly OperationContext _operationContext;

        public WebOperationContext(OperationContext operationContext)
        {
            _operationContext = operationContext ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(operationContext));

            if (operationContext.Extensions.Find<WebOperationContext>() == null)
            {
                operationContext.Extensions.Add(this);
            }
        }

        public static WebOperationContext Current
        {
            get
            {
                if (OperationContext.Current == null)
                {
                    return null;
                }

                WebOperationContext existing = OperationContext.Current.Extensions.Find<WebOperationContext>();
                if (existing != null)
                {
                    return existing;
                }

                return new WebOperationContext(OperationContext.Current);
            }
        }

        // On the client side an operation sends an outgoing request and receives an
        // incoming response. (The mirror-image IncomingRequest/OutgoingResponse
        // accessors are server-side concepts and are intentionally not exposed here.)
        public OutgoingWebRequestContext OutgoingRequest => new OutgoingWebRequestContext(_operationContext);

        public IncomingWebResponseContext IncomingResponse => new IncomingWebResponseContext(_operationContext);

        public void Attach(OperationContext owner)
        {
        }

        public void Detach(OperationContext owner)
        {
        }
    }
}

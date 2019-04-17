// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal abstract class HeaderFilter : MessageFilter
    {
        protected HeaderFilter()
            : base()
        {
        }

        public override bool Match(MessageBuffer buffer)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(buffer));
            }

            Message message = buffer.CreateMessage();
            try
            {
                return Match(message);
            }
            finally
            {
                message.Close();
            }
        }
    }
}

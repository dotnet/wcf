// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Channels;
using System.Text;

namespace System.ServiceModel.Dispatcher
{
    internal class DemultiplexingClientMessageFormatter : IClientMessageFormatter
    {
        private readonly IClientMessageFormatter _defaultFormatter;
        private readonly Dictionary<WebContentFormat, IClientMessageFormatter> _formatters;
        private string _supportedFormats;

        public DemultiplexingClientMessageFormatter(
            IDictionary<WebContentFormat, IClientMessageFormatter> formatters,
            IClientMessageFormatter defaultFormatter)
        {
            if (formatters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(formatters));
            }

            _formatters = new Dictionary<WebContentFormat, IClientMessageFormatter>(formatters);
            _defaultFormatter = defaultFormatter;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            if (message == null)
            {
                return null;
            }

            IClientMessageFormatter selectedFormatter;
            if (TryGetEncodingFormat(message, out WebContentFormat format))
            {
                _formatters.TryGetValue(format, out selectedFormatter);
                if (selectedFormatter == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(
                        new InvalidOperationException(SR.Format(SR.UnrecognizedHttpMessageFormat, format, GetSupportedFormats())));
                }
            }
            else
            {
                selectedFormatter = _defaultFormatter;
                if (selectedFormatter == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(
                        new InvalidOperationException(SR.MessageFormatPropertyNotFound3));
                }
            }

            return selectedFormatter.DeserializeReply(message, parameters);
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                new NotSupportedException(SR.Format(SR.SerializingRequestNotSupportedByFormatter, this)));
        }

        private string GetSupportedFormats()
        {
            if (_supportedFormats == null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (WebContentFormat format in _formatters.Keys)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                        builder.Append(' ');
                    }

                    builder.Append('\'');
                    builder.Append(format);
                    builder.Append('\'');
                }

                _supportedFormats = builder.ToString();
            }

            return _supportedFormats;
        }

        private static bool TryGetEncodingFormat(Message message, out WebContentFormat format)
        {
            if (message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out object property) &&
                property is WebBodyFormatMessageProperty formatProperty)
            {
                format = formatProperty.Format;
                return true;
            }

            format = WebContentFormat.Default;
            return false;
        }
    }
}

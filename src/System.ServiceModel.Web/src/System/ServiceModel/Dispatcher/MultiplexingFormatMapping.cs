// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Mime;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace System.ServiceModel.Dispatcher
{
    internal abstract class MultiplexingFormatMapping
    {
        protected Encoding _writeEncoding;
        protected string _writeCharset;
        protected WebContentTypeMapper _contentTypeMapper;
        private ContentType _defaultContentType;

        public abstract WebMessageFormat MessageFormat { get; }

        public abstract WebContentFormat ContentFormat { get; }

        public abstract string DefaultMediaType { get; }

        protected abstract MessageEncoder Encoder { get; }

        public ContentType DefaultContentType
        {
            get
            {
                if (_defaultContentType == null)
                {
                    _defaultContentType = new ContentType(DefaultMediaType) { CharSet = _writeCharset };
                }

                return _defaultContentType;
            }
        }

        public MultiplexingFormatMapping(Encoding writeEncoding, WebContentTypeMapper contentTypeMapper)
        {
            if (writeEncoding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writeEncoding));
            }

            _writeEncoding = writeEncoding;
            _writeCharset = TextEncoderDefaults.EncodingToCharSet(writeEncoding);
            _contentTypeMapper = contentTypeMapper;
        }

        public bool CanFormatResponse(ContentType acceptHeaderElement, bool matchCharset, out ContentType contentType)
        {
            if (acceptHeaderElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(acceptHeaderElement));
            }

            // Scrub the content type so that it is only mediaType and the charset
            string charset = acceptHeaderElement.CharSet;
            contentType = new ContentType(acceptHeaderElement.MediaType);
            contentType.CharSet = DefaultContentType.CharSet;
            string contentTypeStr = contentType.ToString();

            if (matchCharset &&
                !string.IsNullOrEmpty(charset) &&
                !string.Equals(charset, DefaultContentType.CharSet, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (_contentTypeMapper != null &&
                _contentTypeMapper.GetMessageFormatForContentType(contentType.MediaType) == ContentFormat)
            {
                return true;
            }

            if (Encoder.IsContentTypeSupported(contentTypeStr) &&
                (charset == null || contentType.CharSet == DefaultContentType.CharSet))
            {
                return true;
            }

            contentType = null;
            return false;
        }
    }
}

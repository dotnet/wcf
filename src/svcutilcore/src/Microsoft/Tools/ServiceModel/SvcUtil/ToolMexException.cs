// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class ToolMexException : ToolInputException
    {
        private ToolInputException _httpGetException, _wsMexException;
        private Uri _serviceUri;

        internal ToolInputException WSMexException { get { return _wsMexException; } }
        internal ToolInputException HttpGetException { get { return _httpGetException; } }
        internal Uri ServiceUri { get { return _serviceUri; } }

        internal ToolMexException(ToolInputException wsMexException, ToolInputException httpGetException, Uri serviceUri)
            : base(SR.Format(SR.ErrUnableToRetrieveMetadataFromUri, serviceUri.AbsoluteUri, SR.Format(SR.EnableMetadataHelpMessage)))
        {
            _wsMexException = wsMexException;
            _httpGetException = httpGetException;
            _serviceUri = serviceUri;
        }
    }
}


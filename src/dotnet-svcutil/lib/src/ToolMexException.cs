// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using System;
    using System.Runtime.Serialization;

    internal class ToolMexException : ToolInputException
    {
        private ToolInputException _wsMexException;
        private Uri _serviceUri;

        internal ToolInputException WSMexException { get { return _wsMexException; } }
        internal Uri ServiceUri { get { return _serviceUri; } }

        internal ToolMexException(ToolInputException wsMexException, Uri serviceUri)
            : base(SR.GetString(SR.ErrUnableToRetrieveMetadataFromUriFormat, serviceUri.AbsoluteUri, SR.EnableMetadataHelpMessage))
        {
            _wsMexException = wsMexException;
            _serviceUri = serviceUri;
        }
    }
}


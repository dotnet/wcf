// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            : base(string.Format(SR.ErrUnableToRetrieveMetadataFromUriFormat, serviceUri.AbsoluteUri, SR.EnableMetadataHelpMessage))
        {
            _wsMexException = wsMexException;
            _serviceUri = serviceUri;
        }
    }
}


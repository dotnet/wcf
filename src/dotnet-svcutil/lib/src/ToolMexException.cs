// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using System;
    using System.Runtime.Serialization;

    class ToolMexException : ToolInputException
    {
        ToolInputException wsMexException;
        Uri serviceUri;

        internal ToolInputException WSMexException { get { return wsMexException; } }
        internal Uri ServiceUri { get { return serviceUri; } }

        internal ToolMexException(ToolInputException wsMexException, Uri serviceUri)
            : base(SR.GetString(SR.ErrUnableToRetrieveMetadataFromUriFormat, serviceUri.AbsoluteUri, SR.EnableMetadataHelpMessage))
        {

            this.wsMexException = wsMexException;
            this.serviceUri = serviceUri;
        }
    }

}


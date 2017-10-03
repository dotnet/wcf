//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.Runtime.Serialization;
    
    [Serializable] 
    class ToolMexException : ToolInputException
    {
        ToolInputException httpGetException, wsMexException;
        Uri serviceUri;

        internal ToolInputException WSMexException { get { return wsMexException; } }
        internal ToolInputException HttpGetException { get { return httpGetException; } }
        internal Uri ServiceUri { get { return serviceUri; } }

        internal ToolMexException(ToolInputException wsMexException, ToolInputException httpGetException, Uri serviceUri)
            : base(SR.Format(SR.ErrUnableToRetrieveMetadataFromUri, serviceUri.AbsoluteUri, SR.Format(SR.EnableMetadataHelpMessage)))
        {

            this.wsMexException = wsMexException;
            this.httpGetException = httpGetException;
            this.serviceUri = serviceUri;
        }
    }

}


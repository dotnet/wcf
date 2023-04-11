// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security;
using System.ServiceModel.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Tools.ServiceModel.Svcutil;

namespace System.IO
{
    using System.Runtime.Serialization;

    [Serializable]
    public class PipeException : IOException
    {
        public PipeException()
            : base()
        {
        }

        public PipeException(string message)
            : base(message)
        {
        }

        public PipeException(string message, int errorCode)
            : base(message, errorCode)
        {
        }

        public PipeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        //protected PipeException(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}

        public virtual int ErrorCode
        {
            get { return this.HResult; }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
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

        public virtual int ErrorCode
        {
            get { return this.HResult; }
        }
    }
}

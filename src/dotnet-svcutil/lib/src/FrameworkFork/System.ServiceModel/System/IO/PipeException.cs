﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

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
            get { return HResult; }
        }
    }
}

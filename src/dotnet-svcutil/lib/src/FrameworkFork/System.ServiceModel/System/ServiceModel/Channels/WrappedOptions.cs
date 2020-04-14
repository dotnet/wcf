// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    public class WrappedOptions
    {
        private bool _wrappedFlag = false;
        public bool WrappedFlag { get { return _wrappedFlag; } set { _wrappedFlag = value; } }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Runtime
{
    internal struct TracePayload
    {
        private string _hostReference;

        public TracePayload(string serializedException,
            string eventSource,
            string appDomainFriendlyName,
            string extendedData,
            string hostReference)
        {
            SerializedException = serializedException;
            EventSource = eventSource;
            AppDomainFriendlyName = appDomainFriendlyName;
            ExtendedData = extendedData;
            _hostReference = hostReference;
        }

        public string SerializedException { get; }

        public string EventSource { get; }

        public string AppDomainFriendlyName { get; }

        public string ExtendedData { get; }

        public string HostReference
        {
            get
            {
                return _hostReference;
            }
        }
    }
}

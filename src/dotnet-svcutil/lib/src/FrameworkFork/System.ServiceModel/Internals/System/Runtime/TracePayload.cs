// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime
{
    internal struct TracePayload
    {
        private string _serializedException;
        private string _eventSource;
        private string _appDomainFriendlyName;
        private string _extendedData;
        private string _hostReference;

        public TracePayload(string serializedException,
            string eventSource,
            string appDomainFriendlyName,
            string extendedData,
            string hostReference)
        {
            _serializedException = serializedException;
            _eventSource = eventSource;
            _appDomainFriendlyName = appDomainFriendlyName;
            _extendedData = extendedData;
            _hostReference = hostReference;
        }

        public string SerializedException
        {
            get
            {
                return _serializedException;
            }
        }

        public string EventSource
        {
            get
            {
                return _eventSource;
            }
        }

        public string AppDomainFriendlyName
        {
            get
            {
                return _appDomainFriendlyName;
            }
        }

        public string ExtendedData
        {
            get
            {
                return _extendedData;
            }
        }

        public string HostReference
        {
            get
            {
                return _hostReference;
            }
        }
    }
}

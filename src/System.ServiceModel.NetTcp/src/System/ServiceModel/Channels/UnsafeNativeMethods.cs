// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel.Channels
{
    internal static class UnsafeNativeMethods
    {
        //public const int ERROR_SUCCESS = 0;
        //public const int ERROR_FILE_NOT_FOUND = 2;
        //public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_INVALID_HANDLE = 6;
        public const int ERROR_NOT_ENOUGH_MEMORY = 8;
        public const int ERROR_OUTOFMEMORY = 14;
        //public const int ERROR_SHARING_VIOLATION = 32;
        //public const int ERROR_NETNAME_DELETED = 64;
        //public const int ERROR_INVALID_PARAMETER = 87;
        //public const int ERROR_BROKEN_PIPE = 109;
        //public const int ERROR_ALREADY_EXISTS = 183;
        //public const int ERROR_PIPE_BUSY = 231;
        //public const int ERROR_NO_DATA = 232;
        //public const int ERROR_MORE_DATA = 234;
        //public const int WAIT_TIMEOUT = 258;
        //public const int ERROR_PIPE_CONNECTED = 535;
        //public const int ERROR_OPERATION_ABORTED = 995;
        //public const int ERROR_IO_PENDING = 997;
        //public const int ERROR_SERVICE_ALREADY_RUNNING = 1056;
        //public const int ERROR_SERVICE_DISABLED = 1058;
        //public const int ERROR_NO_TRACKING_SERVICE = 1172;
        //public const int ERROR_ALLOTTED_SPACE_EXCEEDED = 1344;
        public const int ERROR_NO_SYSTEM_RESOURCES = 1450;

        // socket errors
        //public const int WSAACCESS = 10013;
        //public const int WSAEMFILE = 10024;
        //public const int WSAEMSGSIZE = 10040;
        //public const int WSAEADDRINUSE = 10048;
        public const int WSAEADDRNOTAVAIL = 10049;
        public const int WSAENETDOWN = 10050;
        public const int WSAENETUNREACH = 10051;
        public const int WSAENETRESET = 10052;
        public const int WSAECONNABORTED = 10053;
        public const int WSAECONNRESET = 10054;
        public const int WSAENOBUFS = 10055;
        //public const int WSAESHUTDOWN = 10058;
        public const int WSAETIMEDOUT = 10060;
        public const int WSAECONNREFUSED = 10061;
        public const int WSAEHOSTDOWN = 10064;
        public const int WSAEHOSTUNREACH = 10065;
    }
}

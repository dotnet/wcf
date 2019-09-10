// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public static class ServiceDefaults
    {
        public static TimeSpan CloseTimeout { get { return TimeSpan.FromMinutes(1); } }
        public static TimeSpan OpenTimeout { get { return TimeSpan.FromMinutes(1); } }
        public static TimeSpan ReceiveTimeout { get { return TimeSpan.FromMinutes(10); } }
        public static TimeSpan SendTimeout { get { return TimeSpan.FromMinutes(1); } }
    }
}

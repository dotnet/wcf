// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

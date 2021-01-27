// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Diagnostics;

namespace System.ServiceModel
{
    internal static partial class DiagnosticUtility
    {
        private const string TraceSourceName = "TraceSourceNameToReplace";
        internal const string EventSourceName = TraceSourceName + " [COR_BUILD_MAJOR].[COR_BUILD_MINOR].[CLR_OFFICIAL_ASSEMBLY_NUMBER].0";

        private static ExceptionUtility s_exceptionUtility = null;
        private static object s_lockObject = new object();

        public static ExceptionUtility ExceptionUtility
        {
            get
            {
                return s_exceptionUtility ?? GetExceptionUtility();
            }
        }

        private static ExceptionUtility GetExceptionUtility()
        {
            lock (s_lockObject)
            {
                if (s_exceptionUtility == null)
                {
                    s_exceptionUtility = new ExceptionUtility();
                }
            }

            return s_exceptionUtility;
        }
    }
}

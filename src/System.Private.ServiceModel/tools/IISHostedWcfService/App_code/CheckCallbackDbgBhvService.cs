// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace WcfService
{
    public class CheckCallbackDbgBhvService : ICheckCallbackDbgBhvService
    {
        public bool GetResult(bool includeExceptionDetailInFaults)
        {
            string envVar = "callbackexception" + includeExceptionDetailInFaults.ToString().ToLower();
            string result = Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(envVar, null, EnvironmentVariableTarget.Machine);
            if (includeExceptionDetailInFaults)
            {
                return result.Equals("included");
            }
            else
            {
                return result.Equals("unincluded");
            }
        }
    }
}

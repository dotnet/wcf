// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class MarkupTelemetryHelper
    {
        public static void SendBindingData(IEnumerable<Binding> bindings)
        {
            if (ToolConsole.IsMarkupEnabled)
            {
                try
                {
                    int index = 0;
                    foreach (Binding binding in bindings)
                    {
                        TelemetryAddProperty($"BindingScheme {index}", binding.Scheme);
                        TelemetryAddProperty($"BindingName {index}", GetDetails(binding));

                        index++;
                    }

                    TelemetryAddProperty("BindingCount", index.ToString());
                }
                catch
                {
                    // Make sure we won't affect product logic by collecting telemetry data
                }
            }
        }

        private static string GetDetails(Binding binding)
        {
            //
            // The type order is important, please always put derived class in front of the base class
            //
            switch (binding)
            {
                // From HttpBindingBase
                case BasicHttpBinding basicHttpBinding:
                    return GetBindingName<BasicHttpBinding>(binding);
                case NetHttpBinding netHttpBinding:
                    return GetBindingName<NetHttpBinding>(binding);
                case NetHttpsBinding netHttpsBinding:
                    return GetBindingName<NetHttpsBinding>(binding);
                case BasicHttpsBinding basicHttpsBinding:
                    return GetBindingName<BasicHttpsBinding>(binding);
                case HttpBindingBase httpBindingBase:
                    return GetBindingName<HttpBindingBase>(binding);
                // From NetTcpBinding
                case NetTcpContextBinding netTcpContextBinding:
                    return GetBindingName<NetTcpContextBinding>(binding);
                case NetTcpBinding netTcpBinding:
                    return GetBindingName<NetTcpBinding>(binding);
                // From WSFederationHttpBinding
                case WS2007FederationHttpBinding ws2007FederationHttpBinding:
                    return GetBindingName<WS2007FederationHttpBinding>(binding);
                case WSFederationHttpBinding wsFederationHttpBinding:
                    return GetBindingName<WSFederationHttpBinding>(binding);
                // From WSHttpBinding
                case WSHttpContextBinding wsHttpContextBinding:
                    return GetBindingName<WSHttpContextBinding>(binding);
                case WS2007HttpBinding ws2007HttpBinding:
                    return GetBindingName<WS2007HttpBinding>(binding);
                case WSHttpBinding wsHttpBinding:
                    return GetBindingName<WSHttpBinding>(binding);
                // Others
                case CustomBinding customBinding:
                    return GetBindingName<CustomBinding>(binding);
                default:
                    return "UserBinding";
            }
        }

        // To avoid GDPR issue, this function will check if the binding type name is a known type name 
        // For user type, we would append "*" at the end of known type name instead of using user type name directly
        private static string GetBindingName<T>(Binding binding) where T : Binding
        {
            string name = typeof(T).Name;
            return binding.GetType() == typeof(T) ? name : $"{name}*";
        }

        #region LoggingMarkup
        // While enabling logging markup option, the tool will append specific prefix string to each console output
        // This feature is designed for console redirection to get better UI experience also telemetry data collection

        private static void TelemetryAddProperty(string propName, string propValue)
        {
            if (!String.IsNullOrWhiteSpace(propName))
            {
                if (propValue == null)
                {
                    propValue = "<null>";
                }
                else if (propValue.Trim() == string.Empty)
                {
                    propValue = "<empty>";
                }

                ToolConsole.WriteLine(propName + LogTag.TraceProperty + propValue, LogTag.TraceProperty, ToolConsole.Space);
            }
        }

        public static void TelemetryPostOperation(bool succeess, string str)
        {
            if (ToolConsole.IsMarkupEnabled && !String.IsNullOrWhiteSpace(str))
            {
                ToolConsole.WriteLine(str, (succeess ? LogTag.TraceSuccess : LogTag.TraceFailure), ToolConsole.Space);
            }
        }

        public static void TelemetryPostFault(Exception ex)
        {
            if (ToolConsole.IsMarkupEnabled)
            {
                var str = Utils.GetExceptionMessage(ex, includeStackTrace: true);
                ToolConsole.WriteLine(str, LogTag.TraceException, ToolConsole.Space);
            }
        }
        #endregion // LoggingMarkup
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// Helper class for logging.
    /// </summary>
    internal class LogHelper
    {
        /// <summary>
        /// Formats the string using InvariantCulture
        /// </summary>
        /// <param name="format">Format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <returns>Formatted string.</returns>
        internal static string FormatInvariant(string format, params object[] args)
        {
            if (format == null)
                return string.Empty;

            if (args == null)
                return format;

            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class Utils
    {
        internal static bool IsUnexpected(Exception ex)
        {
            return ex != null && (ex is NullReferenceException || ex is ArgumentNullException);
        }

        internal static bool IsFatal(Exception ex)
        {
            return ex != null && (
#if NETCORE10
                 ex is OutOfMemoryException);
#else
                ex is OutOfMemoryException ||
                ex is StackOverflowException ||
                ex is AccessViolationException);
#endif
        }

        internal static bool IsFatalOrUnexpected(Exception ex)
        {
            return IsFatal(ex) || IsUnexpected(ex);
        }

        internal static string GetExceptionMessage(Exception e, bool includeStackTrace = false)
        {
            StringBuilder message = new StringBuilder();

            while (e != null)
            {
                var exMsg = e.Message?.Trim();
                if (!(e is AggregateException) && !string.IsNullOrWhiteSpace(exMsg) && !message.ToString().Contains(exMsg))
                {
                    message.Append(string.Concat(Environment.NewLine, exMsg, includeStackTrace ? e.StackTrace : string.Empty));
                }
                e = e.InnerException;
            }

            // don't trim this string, it will remove intended new lines at the start or end of the string.
            return message.ToString();
        }
    }
}

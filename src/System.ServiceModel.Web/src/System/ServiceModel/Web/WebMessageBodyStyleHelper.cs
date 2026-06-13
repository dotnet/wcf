// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System.ServiceModel.Web
{
    internal static class WebMessageBodyStyleHelper
    {
        internal static bool IsDefined(WebMessageBodyStyle style)
        {
            return (style == WebMessageBodyStyle.Bare
                || style == WebMessageBodyStyle.Wrapped
                || style == WebMessageBodyStyle.WrappedRequest
                || style == WebMessageBodyStyle.WrappedResponse);
        }
    }
}

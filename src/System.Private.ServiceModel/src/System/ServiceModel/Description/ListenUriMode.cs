// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Description
{
    public enum ListenUriMode
    {
        Explicit,
        Unique,
    }

    internal static class ListenUriModeHelper
    {
        static public bool IsDefined(ListenUriMode mode)
        {
            return mode == ListenUriMode.Explicit
                || mode == ListenUriMode.Unique;
        }
    }
}

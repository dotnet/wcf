// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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

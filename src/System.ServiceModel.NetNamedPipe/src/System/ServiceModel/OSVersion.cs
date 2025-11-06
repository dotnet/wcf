// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel
{
    enum OSVersion
    {
        /// <summary>
        /// Used when unable to determine the OS Version
        /// </summary>
        Unknown,

        /// <summary>
        /// OS Versions before WinXP
        /// </summary>
        PreWinXP,

        /// <summary>
        /// OS Version - Windows XP
        /// </summary>
        WinXP,

        /// <summary>
        /// OS Version - Windows Server 2003 
        /// </summary>
        Win2003,

        /// <summary>
        /// OS Version - Windows Vista
        /// </summary>
        WinVista,

        /// <summary>
        /// OS Version - Windows 7
        /// </summary>
        Win7,

        /// <summary>
        /// OS Version - Windows 8
        /// </summary>
        Win8,

        /// <summary>
        /// OS Versions after Windows 8
        /// </summary>
        PostWin8,
    }
}

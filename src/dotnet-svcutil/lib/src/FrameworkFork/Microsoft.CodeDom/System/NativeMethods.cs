// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Win32
{
    using System;
    using System.Runtime.InteropServices;


    internal static class NativeMethods
    {
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [StructLayout(LayoutKind.Sequential)]
        internal class TEXTMETRIC
        {
            public int tmHeight = 0;
            public int tmAscent = 0;
            public int tmDescent = 0;
            public int tmInternalLeading = 0;
            public int tmExternalLeading = 0;
            public int tmAveCharWidth = 0;
            public int tmMaxCharWidth = 0;
            public int tmWeight = 0;
            public int tmOverhang = 0;
            public int tmDigitizedAspectX = 0;
            public int tmDigitizedAspectY = 0;
            public char tmFirstChar = '\0';
            public char tmLastChar = '\0';
            public char tmDefaultChar = '\0';
            public char tmBreakChar = '\0';
            public byte tmItalic = 0;
            public byte tmUnderlined = 0;
            public byte tmStruckOut = 0;
            public byte tmPitchAndFamily = 0;
            public byte tmCharSet = 0;
        }

        public const int DEFAULT_GUI_FONT = 17;
        public const int SM_CYSCREEN = 1;

        public const int ERROR_FILE_EXISTS = 0x50;
    }
}

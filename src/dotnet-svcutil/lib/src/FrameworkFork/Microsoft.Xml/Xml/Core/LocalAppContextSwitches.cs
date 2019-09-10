#if disabled
// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Runtime.CompilerServices;

namespace System
{
    internal static class LocalAppContextSwitches
    {
        private static int _dontThrowOnInvalidSurrogatePairs;
        public static bool DontThrowOnInvalidSurrogatePairs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(@"Switch.Microsoft.Xml.DontThrowOnInvalidSurrogatePairs", ref _dontThrowOnInvalidSurrogatePairs);
            }
        }

        private static int _ignoreEmptyKeySequences;
        public static bool IgnoreEmptyKeySequences
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(@"Switch.Microsoft.Xml.IgnoreEmptyKeySequences", ref _ignoreEmptyKeySequences);
            }
        }
    }
}
#endif 
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System
{
    using System;
    using System.Collections;
    using System.Globalization;

    // [Serializable],
    internal class InvariantComparer : IComparer
    {
        private CompareInfo _compareInfo;
        internal static readonly InvariantComparer Default = new InvariantComparer();

        internal InvariantComparer()
        {
            _compareInfo = CultureInfo.InvariantCulture.CompareInfo;
        }

        public int Compare(Object a, Object b)
        {
            String sa = a as String;
            String sb = b as String;
            if (sa != null && sb != null)
                return _compareInfo.Compare(sa, sb);
            else
                return Comparer.Default.Compare(a, b);
        }
    }
}


// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.ServiceModel
{
    internal static class EmptyArray<T>
    {
        internal static T[] Allocate(int n)
        {
            if (n == 0)
                return Array.Empty<T>();
            else
                return new T[n];
        }

        internal static T[] ToArray(IList<T> collection)
        {
            if (collection.Count == 0)
            {
                return Array.Empty<T>();
            }
            else
            {
                T[] array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }
        }

        internal static T[] ToArray(SynchronizedCollection<T> collection)
        {
            lock (collection.SyncRoot)
            {
                return EmptyArray<T>.ToArray((IList<T>)collection);
            }
        }
    }
}

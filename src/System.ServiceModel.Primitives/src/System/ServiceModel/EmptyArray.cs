// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;

namespace System.ServiceModel
{
    internal static class EmptyArray<T>
    {
        internal static T[] Allocate(int n)
        {
            if (n == 0)
            {
                return Array.Empty<T>();
            }
            else
            {
                return new T[n];
            }
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

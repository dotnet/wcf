// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class CollectionHelpers
    {
        public static T Find<T>(IList list, Filter<T> filter)
            where T : class
        {
            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i] as T;
                if (t != null)
                {
                    if (filter(t))
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        public static void MapList<T>(IList list, Filter<T> filter, OnFiltered<T> onFiltered)
            where T : class
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                T t = list[i] as T;
                if (t != null)
                {
                    if (!filter(t))
                    {
                        list.RemoveAt(i);
                        if (onFiltered != null)
                            onFiltered(t, i);
                    }
                }
            }
        }

        public delegate bool Filter<T>(T t);
        public delegate void OnFiltered<T>(T t, int i);
    }
}

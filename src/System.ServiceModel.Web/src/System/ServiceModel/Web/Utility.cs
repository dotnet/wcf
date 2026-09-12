// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime;

namespace System.ServiceModel.Web
{
    internal static class Utility
    {
        public static void AddRange<T>(ICollection<T> list, IEnumerable<T> itemsToAdd)
        {
            Fx.Assert(list != null, "The 'list' argument should never be null.");
            Fx.Assert(itemsToAdd != null, "The 'itemsToAdd' argument should never be null.");

            foreach (T item in itemsToAdd)
            {
                list.Add(item);
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.ObjectPool;

namespace System.ServiceModel
{
    internal class NullCreatingPooledObjectPolicy<T> : PooledObjectPolicy<T> where T : class
    {
        public static ObjectPool<T> CreatePool(int maximumRetained)
        {
            return new DefaultObjectPool<T>(new NullCreatingPooledObjectPolicy<T>(), maximumRetained);
        }

        public override T Create()
        {
            return null;
        }

        public override bool Return(T obj)
        {
            return obj is not null;
        }
    }
}

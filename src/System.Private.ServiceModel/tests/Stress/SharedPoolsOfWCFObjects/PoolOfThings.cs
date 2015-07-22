// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WCFClientStressTests
{
    // Pooling of WCF channel factories and channels is a commonly reccomended practice due to its perf benefits.
    // However with pooling come more race condition-like scenarios such as using an already faulted channel or factory.
    // 
    // The number of such scenarios is rather significant:
    // - create and use a pool of channels using a pool of channel factories
    // - replace and close channels under stress
    // - replace and close channel factories under stress
    // - manipulate a pooled channel or a pooled factory state while others use them
    // 
    // PoolOfThings and PoolOfAsyncThings classes help to organize generic pools of objects
    // FactoryAndPoolOfItsObjects and FactoryAndPoolOfItsAsyncObjects classes help to organize pools of pools of objects
    // 

    public class PoolOfThings<T> : IDisposable where T : class
    {
        private T[] _thePool;
        private int _maxSize;

        private Func<T> _createInstance;
        private Action<T> _destroyInstance;

        /// <summary>
        /// ctor taking sync versions of delegates
        /// </summary>
        /// <param name="maxSize"> number of pooled instances </param>
        /// <param name="createInstance"> delegate to create a new instance; can return null in which case the pool will store null until DestoryAllPooledInstances is called </param>
        /// <param name="destroyInstance"></param>
        public PoolOfThings(int maxSize, Func<T> createInstance, Action<T> destroyInstance)
        {
            _thePool = new T[maxSize];
            _maxSize = maxSize;
            _createInstance = createInstance;
            _destroyInstance = destroyInstance;
        }

        public void Dispose()
        {
            DestoryAllPooledInstances();
        }

        // Replaces the pool of channels with a new one and destory all instances in the previous pool
        public void DestoryAllPooledInstances()
        {
            // Get the new pool ready
            T[] newPool = new T[_maxSize];

            // Replace the pool
            var oldPool = _thePool;
            if (Interlocked.CompareExchange(ref _thePool, newPool, oldPool) != oldPool)
            {
                // somebody beat us - they will be responsible for closing the old _channelsPool they replaced
            }
            else
            {
                // we are the one who replaced the old _thePool - we're responsible for cleaning it up
                DestroyAllPooledInstanciesImpl(oldPool, _destroyInstance);
            }
        }

        public IEnumerable<T> GetAllPooledInstances()
        {
            return new AllPooledInstancesCollection(this);
        }

        #region helpers

        private class AllPooledInstancesCollection : IEnumerable<T>
        {
            private PoolOfThings<T> _thePool;
            public AllPooledInstancesCollection(PoolOfThings<T> thePool)
            {
                _thePool = thePool;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _thePool._thePool.Length; i++)
                {
                    var instance = _thePool.GetPooledInstance(i);
                    // skip nulls 
                    if (instance != default(T))
                    {
                        yield return instance;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private T GetPooledInstance(int i)
        {
            var instance = _thePool[i];
            while (instance == null)
            {
                //try
                {
                    instance = _createInstance();
                    if (instance == null)
                    {
                        break;
                    }
                    if (Interlocked.CompareExchange<T>(ref _thePool[i], instance, null) != null)
                    {
                        _destroyInstance(instance);
                        instance = _thePool[i];
                        // Somebody stored the new instance before us - dispose ours and re-read theirs
                        // Since the whole pool can also be replaced we can still get a null from _thePool[index]
                        // so we retry while (instance == null)
                        // Note that _destroyInstance(instance) should not throw since nobody uses it yet

                        //Interlocked.Increment(ref _numPoolSetRetries);
                    }
                }
                //catch (Exception)
                {
                    //    break;
                }
            }
            return instance;
        }

        private void DestroyAllPooledInstanciesImpl(T[] instancesPool, Action<T> destroyInstance)
        {
            foreach (var instance in instancesPool)
            {
                if (instance != null)
                {
                    try
                    {
                        //Interlocked.Increment(ref _stats.FactoriesClosed);
                        destroyInstance(instance);
                    }
                    catch (Exception e)
                    {
                        //Interlocked.Increment(ref _stats.ExceptionsClosingFactories);
                    }
                }
            }
        }
        #endregion
    }
    public class PoolOfAsyncThings<T> : IDisposable where T : class
    {
        private T[] _thePool;
        private int _maxSize;

        private Func<T> _createInstance;
        private Func<T, Task> _destroyInstanceAsync;

        /// <summary>
        /// ctor taking sync versions of delegates
        /// </summary>
        /// <param name="maxSize"> number of pooled instances </param>
        /// <param name="createInstance"> delegate to create a new instance; can return null in which case the pool will store null until DestoryAllPooledInstancesAsync is called </param>
        /// <param name="destroyInstance"></param>
        public PoolOfAsyncThings(int maxSize, Func<T> createInstance, Func<T, Task> destroyInstanceAsync)
        {
            _thePool = new T[maxSize];
            _maxSize = maxSize;
            _createInstance = createInstance;
            _destroyInstanceAsync = destroyInstanceAsync;
        }

        public void Dispose()
        {
            DestoryAllPooledInstancesAsync().Wait();
        }

        // Replaces the pool of channels with a new one and destory all instances in the previous pool
        public async Task DestoryAllPooledInstancesAsync()
        {
            // Get the new pool ready
            T[] newPool = new T[_maxSize];

            // Replace the pool
            var oldPool = _thePool;
            if (Interlocked.CompareExchange(ref _thePool, newPool, oldPool) != oldPool)
            {
                // somebody beat us - they will be responsible for closing the old _channelsPool they replaced
            }
            else
            {
                // we are the one who replaced the old _thePool - we're responsible for cleaning it up
                await DestroyAllPooledInstanciesImplAsync(oldPool, _destroyInstanceAsync);
            }
        }

        public IEnumerable<T> GetAllPooledInstances()
        {
            return new AllPooledInstancesCollection(this);
        }

        #region helpers

        private class AllPooledInstancesCollection : IEnumerable<T>
        {
            private PoolOfAsyncThings<T> _thePool;
            public AllPooledInstancesCollection(PoolOfAsyncThings<T> thePool)
            {
                _thePool = thePool;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _thePool._thePool.Length; i++)
                {
                    var instance = _thePool.GetPooledInstance(i);
                    // skip nulls 
                    if (instance != default(T))
                    {
                        yield return instance;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private T GetPooledInstance(int i)
        {
            var instance = _thePool[i];
            while (instance == null)
            {
                instance = _createInstance();
                if (instance == null)
                {
                    break;
                }
                if (Interlocked.CompareExchange<T>(ref _thePool[i], instance, null) != null)
                {
                    // We're deliberately not awaiting it
                    _destroyInstanceAsync(instance);
                    instance = _thePool[i];
                    // Somebody stored the new instance before us - dispose ours and re-read theirs
                    // Since the whole pool can also be replaced we can still get a null from _thePool[index]
                    // so we retry while (instance == null)
                    // Note that _destroyInstance(instance) should not throw since nobody uses it yet

                    //Interlocked.Increment(ref _numPoolSetRetries);
                }
            }
            return instance;
        }

        private async Task DestroyAllPooledInstanciesImplAsync(T[] instancesPool, Func<T, Task> destroyInstanceAsync)
        {
            foreach (var instance in instancesPool)
            {
                if (instance != null)
                {
                    try
                    {
                        //Interlocked.Increment(ref _stats.FactoriesClosed);
                        await destroyInstanceAsync(instance);
                    }
                    catch (Exception e)
                    {
                        //Interlocked.Increment(ref _stats.ExceptionsClosingFactories);
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// This helper classes below group a factory F with a pool of objects O created with the help of the factory
    /// Keeping them together makes it easier to create and cleanup pools of pools of objects
    /// Unlike the traditional factory patters, rather than demanding the factory F to be able to create/destroy 
    /// instances of O we take delegates to create/destroy instances of O as constructor parameters.
    /// This allows us to decouple F and O and makes it easy to introduce additional wrappers around the instances F creates
    /// </summary>
    public class FactoryAndPoolOfItsObjects<F, O>
        where F : class
        where O : class
    {
        private Action<F> _destroyFactoryInstance;
        public F Factory { get; set; }
        public PoolOfThings<O> ObjectsPool { get; set; }

        /// <summary>
        /// The constructor simply creates an empty pool of objects and stores delegates to create/destroy instances
        /// </summary>
        /// <param name="createFactoryInstance"> A func to create an instance of F. </param>
        /// <param name="destroyFactoryInstance"> An action to destroy an instance of F</param>
        /// <param name="maxPooledObjects"> Max size of the pool of objects O </param>
        /// <param name="createObject"> A func to create all instances of O </param>
        /// <param name="destroyObject"> An action to destroy all instances of O</param>
        public FactoryAndPoolOfItsObjects(Func<F> createFactoryInstance, Action<F> destroyFactoryInstance, int maxPooledObjects, Func<F, O> createObject, Action<O> destroyObject)
        {
            _destroyFactoryInstance = destroyFactoryInstance;
            Factory = createFactoryInstance();
            ObjectsPool = new PoolOfThings<O>(
                maxSize: maxPooledObjects,
                createInstance: () =>
                {
                    // PoolOfThings lets us use the instances while destroying them
                    // Destroy may null Factory out so we save a local copy here
                    var f = Factory;
                    return f != null ? createObject(f) : null;
                },
                destroyInstance: destroyObject);
        }

        public void Destroy()
        {
            // should never be recycled more than once
            if (Factory == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            _destroyFactoryInstance(Factory);
            Factory = null;
        }
    }

    public class FactoryAndPoolOfItsAsyncObjects<F, O>
        where F : class
        where O : class
    {
        private Func<F, Task> _destroyFactoryInstanceAsync;
        public F Factory { get; set; }
        public PoolOfAsyncThings<O> ObjectsPool { get; set; }
        public FactoryAndPoolOfItsAsyncObjects(
            Func<F> createFactoryInstance,
            Func<F, Task> destroyFactoryInstanceAsync,
            int maxPooledObjects,
            Func<F, O> createObject,
            Func<O, Task> destroyObjectAsync)
        {
            _destroyFactoryInstanceAsync = destroyFactoryInstanceAsync;
            Factory = createFactoryInstance();
            ObjectsPool = new PoolOfAsyncThings<O>(
                maxSize: maxPooledObjects,
                createInstance: () =>
                {
                    // PoolOfThings lets us use the instances while destroying them
                    // DestroyAsync may null Factory out so we save a local copy
                    var f = Factory;
                    return f != null ? createObject(f) : null;
                },
                destroyInstanceAsync: async (o) => await destroyObjectAsync(o));
        }

        public async Task DestroyAsync()
        {
            // should never be recycled more than once
            if (Factory == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            await _destroyFactoryInstanceAsync(Factory);
            Factory = null;
        }
    }
}

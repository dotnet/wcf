// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bridge
{
    public class IdleTimeoutHandler : DelegatingHandler
    {
        IdleTimeoutManager _timeoutManager;

        private IdleTimeoutHandler(TimeSpan idleTimeout)
        {
            _timeoutManager = new IdleTimeoutManager(idleTimeout); ;
            _timeoutManager.OnTimeOut += (s, e) =>
            {
                Console.WriteLine("Timed out as there were no messages to the bridge for {0} seconds", (int)e.TotalSeconds);
                Environment.Exit(-1);
            };
        }

        internal static void Register(HttpConfiguration config)
        {
            // Read configuration value from enviroment and register for larger values. 
            var waitTimeout = TimeSpan.FromMinutes(10);            
            config.MessageHandlers.Add(new IdleTimeoutHandler(waitTimeout));
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("DEBUG: Processing : " + request.RequestUri);            
            using (_timeoutManager.Start())
            {
                // Call the inner handler.
                var response = await base.SendAsync(request, cancellationToken);
                Debug.WriteLine("DEBUG: Completed  : " + request.RequestUri);
                return response;
            }
        }

        class IdleTimeoutManager
        {
            bool _disposed = false;
            Timer _timer;
            TimeSpan _waitTimeout;
            public event EventHandler<TimeSpan> OnTimeOut;

            public IdleTimeoutManager(TimeSpan idleTimeout)
            {
                _waitTimeout = idleTimeout;
                _timer = new Timer(this.TimeOutCallback);
                this.Restart();
            }

            public IDisposable Start()
            {
                if (Update(Timeout.Infinite))
                {
                    return new OperationScope(this);
                }

                return null;
            }

            public void Restart()
            {
                Update((int)_waitTimeout.TotalMilliseconds);
            }

            private bool Update(int interval)
            {
                lock (_timer)
                {
                    if (!_disposed)
                    {
                        _timer.Change(interval, Timeout.Infinite);
                        return true;
                    }
                }

                return false;
            }

            private void TimeOutCallback(object state)
            {
                lock (_timer)
                {
                    if (_disposed)
                    {
                        return;
                    }
                }

                Dispose();

                if (OnTimeOut != null)
                {
                    OnTimeOut(this, _waitTimeout);
                }
            }

            public void Dispose()
            {
                lock (_timer)
                {
                    if (!_disposed)
                    {
                        _timer.Dispose();
                        _disposed = true;
                    }
                }
            }

            public class OperationScope : IDisposable
            {
                private IdleTimeoutManager _manager;

                public OperationScope(IdleTimeoutManager manager)
                {
                    _manager = manager;
                }

                void IDisposable.Dispose()
                {
                    _manager.Restart();
                }
            }
        }
    }
}

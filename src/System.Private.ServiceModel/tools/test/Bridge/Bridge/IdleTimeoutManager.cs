// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WcfTestBridgeCommon;

namespace Bridge
{
    public class IdleTimeoutHandler : DelegatingHandler
    {
        public static readonly TimeSpan Default_MaxIdleTimeSpan = TimeSpan.FromHours(24);
        private static IdleTimeoutManager _timeoutManager = new IdleTimeoutManager(Default_MaxIdleTimeSpan);

        private IdleTimeoutHandler(TimeSpan idleTimeout)
        {
            _timeoutManager.IdleTimeout = idleTimeout;
            _timeoutManager.Restart();

            _timeoutManager.OnTimeOut += (s, e) =>
            {
                Trace.WriteLine(String.Format("{0:T} - Timed out as there were no messages to the bridge for {1} seconds", DateTime.Now, (int)e.TotalSeconds),
                                this.GetType().Name);
                BridgeController.StopBridgeProcess(0);
            };
        }

        internal static void Register(HttpConfiguration config)
        {
            var waitTimeout = ConfigController.BridgeConfiguration.BridgeMaxIdleTimeSpan;            
            config.MessageHandlers.Add(new IdleTimeoutHandler(waitTimeout));
        }

        public static void RestartTimer()
        {
            _timeoutManager.Restart();
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Trace.WriteLine(String.Format("{0:T} - Bridge received {1} {2}", DateTime.Now, request.Method, request.RequestUri), 
                            this.GetType().Name);            
            using (_timeoutManager.Start())
            {
                // Call the inner handler.
                var response = await base.SendAsync(request, cancellationToken);
                Trace.WriteLine(String.Format("{0:T} - Bridge completed {1} {2}", DateTime.Now, request.Method, request.RequestUri),
                                this.GetType().Name);
                return response;
            }
        }

        class IdleTimeoutManager
        {
            bool _disposed = false;
            Timer _timer;
            public event EventHandler<TimeSpan> OnTimeOut;

            public IdleTimeoutManager(TimeSpan idleTimeout)
            {
                IdleTimeout = idleTimeout;
                _timer = new Timer(this.TimeOutCallback);
                this.Restart();

                // Register to be notified when the Bridge idle timeout changes
                ConfigController.IdleTimeoutChanged += (object s, ChangedEventArgs<TimeSpan> args) =>
                {
                    TimeSpan newTimeout = args.NewValue;
                    Trace.WriteLine(String.Format("{0:T} - Bridge idle timeout changed to {1}.", 
                                                  DateTime.Now, newTimeout),
                                    this.GetType().Name);
                    IdleTimeout = newTimeout;
                    Restart();
                };
            }

            public TimeSpan IdleTimeout { get; set; }

            public IDisposable Start()
            {
                if (Update((int)IdleTimeout.TotalMilliseconds))
                {
                    return new OperationScope(this);
                }

                return null;
            }

            public void Restart()
            {
                Update((int)IdleTimeout.TotalMilliseconds);
            }

            private bool Update(int interval)
            {
                Trace.WriteLine(String.Format("{0:T} Restarting timer for {1} ms", DateTime.Now, interval), this.GetType().Name);

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
                    OnTimeOut(this, IdleTimeout);
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

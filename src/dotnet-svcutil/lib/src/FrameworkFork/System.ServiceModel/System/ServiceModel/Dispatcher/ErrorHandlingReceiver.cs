// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal class ErrorHandlingReceiver
    {
        private ChannelDispatcher _dispatcher;
        private IChannelBinder _binder;

        internal ErrorHandlingReceiver(IChannelBinder binder, ChannelDispatcher dispatcher)
        {
            _binder = binder;
            _dispatcher = dispatcher;
        }

        internal void Close()
        {
            try
            {
                _binder.Channel.Close();
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                this.HandleError(e);
            }
        }

        private void HandleError(Exception e)
        {
            if (_dispatcher != null)
            {
                _dispatcher.HandleError(e);
            }
        }

        private void HandleErrorOrAbort(Exception e)
        {
            if ((_dispatcher == null) || !_dispatcher.HandleError(e))
            {
                if (_binder.HasSession)
                {
                    _binder.Abort();
                }
            }
        }

        internal bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            try
            {
                return _binder.TryReceive(timeout, out requestContext);
            }
            catch (CommunicationObjectAbortedException)
            {
                requestContext = null;
                return true;
            }
            catch (CommunicationObjectFaultedException)
            {
                requestContext = null;
                return true;
            }
            catch (CommunicationException e)
            {
                this.HandleError(e);
                requestContext = null;
                return false;
            }
            catch (TimeoutException e)
            {
                this.HandleError(e);
                requestContext = null;
                return false;
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                this.HandleErrorOrAbort(e);
                requestContext = null;
                return false;
            }
        }

        internal IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            try
            {
                return _binder.BeginTryReceive(timeout, callback, state);
            }
            catch (CommunicationObjectAbortedException)
            {
                return new ErrorHandlingCompletedAsyncResult(true, callback, state);
            }
            catch (CommunicationObjectFaultedException)
            {
                return new ErrorHandlingCompletedAsyncResult(true, callback, state);
            }
            catch (CommunicationException e)
            {
                this.HandleError(e);
                return new ErrorHandlingCompletedAsyncResult(false, callback, state);
            }
            catch (TimeoutException e)
            {
                this.HandleError(e);
                return new ErrorHandlingCompletedAsyncResult(false, callback, state);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                this.HandleErrorOrAbort(e);
                return new ErrorHandlingCompletedAsyncResult(false, callback, state);
            }
        }

        internal bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            ErrorHandlingCompletedAsyncResult handlerResult = result as ErrorHandlingCompletedAsyncResult;
            if (handlerResult != null)
            {
                requestContext = null;
                return ErrorHandlingCompletedAsyncResult.End(handlerResult);
            }
            else
            {
                try
                {
                    return _binder.EndTryReceive(result, out requestContext);
                }
                catch (CommunicationObjectAbortedException)
                {
                    requestContext = null;
                    return true;
                }
                catch (CommunicationObjectFaultedException)
                {
                    requestContext = null;
                    return true;
                }
                catch (CommunicationException e)
                {
                    this.HandleError(e);
                    requestContext = null;
                    return false;
                }
                catch (TimeoutException e)
                {
                    this.HandleError(e);
                    requestContext = null;
                    return false;
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    this.HandleErrorOrAbort(e);
                    requestContext = null;
                    return false;
                }
            }
        }

        private class ErrorHandlingCompletedAsyncResult : CompletedAsyncResult<bool>
        {
            internal ErrorHandlingCompletedAsyncResult(bool data, AsyncCallback callback, object state)
                : base(data, callback, state)
            {
            }
        }
    }
}

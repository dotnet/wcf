// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Helper class used to manage the lifetime of a connection relative to its pool.
    internal abstract class ConnectionPoolHelper
    {
        private IConnectionInitiator _connectionInitiator;
        private ConnectionPool _connectionPool;
        private Uri _via;
        private bool _closed;

        // key for rawConnection in the connection pool
        private string _connectionKey;

        // did rawConnection originally come from connectionPool?
        private bool _isConnectionFromPool;

        // the "raw" connection that should be stored in the pool
        private IConnection _rawConnection;

        // the "upgraded" connection built on top of the "raw" connection to be used for I/O
        private IConnection _upgradedConnection;

        public ConnectionPoolHelper(ConnectionPool connectionPool, IConnectionInitiator connectionInitiator, Uri via)
        {
            _connectionInitiator = connectionInitiator;
            _connectionPool = connectionPool;
            _via = via;
        }

        private object ThisLock
        {
            get { return this; }
        }

        protected abstract IConnection AcceptPooledConnection(IConnection connection, ref TimeoutHelper timeoutHelper);
        protected abstract Task<IConnection> AcceptPooledConnectionAsync(IConnection connection, ref TimeoutHelper timeoutHelper);
        protected abstract TimeoutException CreateNewConnectionTimeoutException(TimeSpan timeout, TimeoutException innerException);

        private IConnection TakeConnection(TimeSpan timeout)
        {
            return _connectionPool.TakeConnection(null, _via, timeout, out _connectionKey);
        }

        public async Task<IConnection> EstablishConnectionAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            IConnection localRawConnection = null;
            IConnection localUpgradedConnection = null;
            bool localIsConnectionFromPool = true;

            // first try and use a connection from our pool (and use it if we successfully receive an ACK)
            while (localIsConnectionFromPool)
            {
                localRawConnection = this.TakeConnection(timeoutHelper.RemainingTime());
                if (localRawConnection == null)
                {
                    localIsConnectionFromPool = false;
                }
                else
                {
                    bool preambleSuccess = false;
                    try
                    {
                        localUpgradedConnection = await AcceptPooledConnectionAsync(localRawConnection, ref timeoutHelper);
                        preambleSuccess = true;
                        break;
                    }
                    catch (CommunicationException)
                    {
                        // CommunicationException is ok since it was a cached connection of unknown state
                    }
                    catch (TimeoutException)
                    {
                        // ditto for TimeoutException
                    }
                    finally
                    {
                        if (!preambleSuccess)
                        {
                            // This cannot throw TimeoutException since isConnectionStillGood is false (doesn't attempt a Close).
                            _connectionPool.ReturnConnection(_connectionKey, localRawConnection, false, TimeSpan.Zero);
                        }
                    }
                }
            }

            // if there isn't anything in the pool, we need to use a new connection
            if (!localIsConnectionFromPool)
            {
                bool success = false;
                TimeSpan connectTimeout = timeoutHelper.RemainingTime();
                try
                {
                    try
                    {
                        localRawConnection = await _connectionInitiator.ConnectAsync(_via, connectTimeout);
                    }
                    catch (TimeoutException e)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateNewConnectionTimeoutException(
                            connectTimeout, e));
                    }

                    _connectionInitiator = null;
                    localUpgradedConnection = await AcceptPooledConnectionAsync(localRawConnection, ref timeoutHelper);
                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        _connectionKey = null;
                        if (localRawConnection != null)
                        {
                            localRawConnection.Abort();
                        }
                    }
                }
            }

            SnapshotConnection(localUpgradedConnection, localRawConnection, localIsConnectionFromPool);

            return localUpgradedConnection;
        }

        public IConnection EstablishConnection(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            IConnection localRawConnection = null;
            IConnection localUpgradedConnection = null;
            bool localIsConnectionFromPool = true;

            // first try and use a connection from our pool (and use it if we successfully receive an ACK)
            while (localIsConnectionFromPool)
            {
                localRawConnection = this.TakeConnection(timeoutHelper.RemainingTime());
                if (localRawConnection == null)
                {
                    localIsConnectionFromPool = false;
                }
                else
                {
                    bool preambleSuccess = false;
                    try
                    {
                        localUpgradedConnection = AcceptPooledConnection(localRawConnection, ref timeoutHelper);
                        preambleSuccess = true;
                        break;
                    }
                    catch (CommunicationException /*e*/)
                    {
                        // CommunicationException is ok since it was a cached connection of unknown state
                    }
                    catch (TimeoutException /*e*/)
                    {
                        // ditto for TimeoutException
                    }
                    finally
                    {
                        if (!preambleSuccess)
                        {
                            // This cannot throw TimeoutException since isConnectionStillGood is false (doesn't attempt a Close).
                            _connectionPool.ReturnConnection(_connectionKey, localRawConnection, false, TimeSpan.Zero);
                        }
                    }
                }
            }

            // if there isn't anything in the pool, we need to use a new connection
            if (!localIsConnectionFromPool)
            {
                bool success = false;
                TimeSpan connectTimeout = timeoutHelper.RemainingTime();
                try
                {
                    try
                    {
                        localRawConnection = _connectionInitiator.Connect(_via, connectTimeout);
                    }
                    catch (TimeoutException e)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateNewConnectionTimeoutException(
                            connectTimeout, e));
                    }

                    _connectionInitiator = null;
                    localUpgradedConnection = AcceptPooledConnection(localRawConnection, ref timeoutHelper);
                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        _connectionKey = null;
                        if (localRawConnection != null)
                        {
                            localRawConnection.Abort();
                        }
                    }
                }
            }

            SnapshotConnection(localUpgradedConnection, localRawConnection, localIsConnectionFromPool);

            return localUpgradedConnection;
        }

        private void SnapshotConnection(IConnection upgradedConnection, IConnection rawConnection, bool isConnectionFromPool)
        {
            lock (ThisLock)
            {
                if (_closed)
                {
                    upgradedConnection.Abort();

                    // cleanup our pool if necessary
                    if (isConnectionFromPool)
                    {
                        _connectionPool.ReturnConnection(_connectionKey, rawConnection, false, TimeSpan.Zero);
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new CommunicationObjectAbortedException(
                        string.Format(SRServiceModel.OperationAbortedDuringConnectionEstablishment, _via)));
                }
                else
                {
                    _upgradedConnection = upgradedConnection;
                    _rawConnection = rawConnection;
                    _isConnectionFromPool = isConnectionFromPool;
                }
            }
        }

        public void Abort()
        {
            ReleaseConnection(true, TimeSpan.Zero);
        }

        public void Close(TimeSpan timeout)
        {
            ReleaseConnection(false, timeout);
        }

        private void ReleaseConnection(bool abort, TimeSpan timeout)
        {
            string localConnectionKey;
            IConnection localUpgradedConnection;
            IConnection localRawConnection;

            lock (ThisLock)
            {
                _closed = true;
                localConnectionKey = _connectionKey;
                localUpgradedConnection = _upgradedConnection;
                localRawConnection = _rawConnection;

                _upgradedConnection = null;
                _rawConnection = null;
            }

            if (localUpgradedConnection == null)
            {
                return;
            }

            try
            {
                if (_isConnectionFromPool)
                {
                    _connectionPool.ReturnConnection(localConnectionKey, localRawConnection, !abort, timeout);
                }
                else
                {
                    if (abort)
                    {
                        localUpgradedConnection.Abort();
                    }
                    else
                    {
                        _connectionPool.AddConnection(localConnectionKey, localRawConnection, timeout);
                    }
                }
            }
            catch (CommunicationException /*e*/)
            {
                localUpgradedConnection.Abort();
            }
        }
    }
}

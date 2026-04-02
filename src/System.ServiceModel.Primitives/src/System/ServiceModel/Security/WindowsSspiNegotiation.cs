// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Security;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;

namespace System.ServiceModel.Security
{
    internal sealed class WindowsSspiNegotiation : ISspiNegotiation
    {
        private NegotiateAuthentication _outgoingNegotiateAuth;
        private NegotiateAuthentication _incomingNegotiateAuth;
        private bool _disposed;
        private bool _isCompleted;
        private bool _isServer;
        private string _servicePrincipalName;
        private TokenImpersonationLevel _impersonationLevel;
        private bool _doMutualAuth;
        private string _package;
        private NetworkCredential _credential;
        private object _syncObject = new object();

        /// <summary>
        /// Client side constructor
        /// </summary>
        internal WindowsSspiNegotiation(
            string package,
            NetworkCredential credential,
            TokenImpersonationLevel impersonationLevel,
            string servicePrincipalName,
            bool doMutualAuth)
        {
            _isServer = false;
            _servicePrincipalName = servicePrincipalName;
            _impersonationLevel = impersonationLevel;
            _doMutualAuth = doMutualAuth;
            _package = package;
            _credential = credential ?? CredentialCache.DefaultNetworkCredentials;
        }

        private void EnsureInitialized(ChannelBinding channelBinding)
        {
            if (_outgoingNegotiateAuth == null)
            {
                var clientOptions = new NegotiateAuthenticationClientOptions
                {
                    Package = _package,
                    Credential = _credential,
                    TargetName = _servicePrincipalName,
                    RequiredProtectionLevel = ProtectionLevel.EncryptAndSign,
                    RequireMutualAuthentication = _doMutualAuth,
                    AllowedImpersonationLevel = _impersonationLevel,
                    Binding = channelBinding,
                };

                _outgoingNegotiateAuth = new NegotiateAuthentication(clientOptions);
            }
        }

        public DateTime ExpirationTimeUtc
        {
            get
            {
                ThrowIfDisposed();
                // NegotiateAuthentication does not expose lifespan directly.
                // Return max value as in the .NET FX fallback.
                return SecurityUtils.MaxUtcDateTime;
            }
        }

        public bool IsCompleted
        {
            get
            {
                ThrowIfDisposed();
                return _isCompleted;
            }
        }

        public bool IsMutualAuthFlag
        {
            get
            {
                ThrowIfDisposed();
                return _outgoingNegotiateAuth.IsMutuallyAuthenticated;
            }
        }

        public bool IsValidContext
        {
            get
            {
                return _isCompleted && _outgoingNegotiateAuth != null && _outgoingNegotiateAuth.IsAuthenticated;
            }
        }

        public string KeyEncryptionAlgorithm
        {
            get
            {
                return SecurityAlgorithms.WindowsSspiKeyWrap;
            }
        }

        public string ProtocolName
        {
            get
            {
                ThrowIfDisposed();
                return _outgoingNegotiateAuth.Package;
            }
        }

        public string ServicePrincipalName
        {
            get
            {
                ThrowIfDisposed();
                return _servicePrincipalName;
            }
        }

        public string GetRemoteIdentityName()
        {
            if (!_isServer)
            {
                return _servicePrincipalName;
            }

            if (IsValidContext)
            {
                var remoteIdentity = _outgoingNegotiateAuth.RemoteIdentity;
                if (remoteIdentity != null)
                {
                    return remoteIdentity.Name;
                }
            }

            return string.Empty;
        }

        public byte[] Decrypt(byte[] encryptedContent)
        {
            if (encryptedContent == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(encryptedContent));
            }

            ThrowIfDisposed();

            var auth = _incomingNegotiateAuth ?? _outgoingNegotiateAuth;
            var outputWriter = new ArrayBufferWriter<byte>();
            NegotiateAuthenticationStatusCode statusCode = auth.Unwrap(encryptedContent, outputWriter, out _);

            if (statusCode != NegotiateAuthenticationStatusCode.Completed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new Win32Exception(SRP.Format(SRP.InvalidSspiNegotiation)));
            }

            return outputWriter.WrittenSpan.ToArray();
        }

        public byte[] Encrypt(byte[] input)
        {
            if (input == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(input));
            }

            ThrowIfDisposed();

            var auth = _outgoingNegotiateAuth;
            var outputWriter = new ArrayBufferWriter<byte>();
            NegotiateAuthenticationStatusCode statusCode = auth.Wrap(input, outputWriter, true, out _);

            if (statusCode != NegotiateAuthenticationStatusCode.Completed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new Win32Exception(SRP.Format(SRP.InvalidSspiNegotiation)));
            }

            return outputWriter.WrittenSpan.ToArray();
        }

        public byte[] GetOutgoingBlob(byte[] incomingBlob, ChannelBinding channelBinding, ExtendedProtectionPolicy protectionPolicy)
        {
            ThrowIfDisposed();
            EnsureInitialized(channelBinding);

            NegotiateAuthenticationStatusCode statusCode;
            ReadOnlySpan<byte> incomingSpan = incomingBlob != null ? incomingBlob : ReadOnlySpan<byte>.Empty;
            byte[] outgoingBlob = _outgoingNegotiateAuth.GetOutgoingBlob(incomingSpan, out statusCode);

            if (statusCode == NegotiateAuthenticationStatusCode.Completed)
            {
                _isCompleted = true;
            }
            else if (statusCode == NegotiateAuthenticationStatusCode.ContinueNeeded)
            {
                // continue the negotiation
            }
            else
            {
                _isCompleted = true;
                if (statusCode == NegotiateAuthenticationStatusCode.TargetUnknown ||
                    statusCode == NegotiateAuthenticationStatusCode.UnknownCredentials)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new Win32Exception(SRP.Format(SRP.IncorrectSpnOrUpnSpecified, _servicePrincipalName)));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new Win32Exception(SRP.Format(SRP.InvalidSspiNegotiation)));
                }
            }

            return outgoingBlob;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            lock (_syncObject)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        _outgoingNegotiateAuth?.Dispose();
                        _incomingNegotiateAuth?.Dispose();
                    }

                    _outgoingNegotiateAuth = null;
                    _incomingNegotiateAuth = null;
                    _servicePrincipalName = null;
                    _disposed = true;
                }
            }
        }

        private void ThrowIfDisposed()
        {
            lock (_syncObject)
            {
                if (_disposed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(null));
                }
            }
        }
    }
}

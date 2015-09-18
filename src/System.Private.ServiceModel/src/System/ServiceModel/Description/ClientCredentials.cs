// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace System.ServiceModel.Description
{
    public class ClientCredentials : SecurityCredentialsManager, IEndpointBehavior
    {
        private UserNamePasswordClientCredential _userName;
        private X509CertificateInitiatorClientCredential _clientCertificate;
        private X509CertificateRecipientClientCredential _serviceCertificate;
        private WindowsClientCredential _windows;
        private HttpDigestClientCredential _httpDigest;
        private bool _isReadOnly;

        public ClientCredentials()
        {
        }

        protected ClientCredentials(ClientCredentials other)
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("other");
            if (other._userName != null)
                _userName = new UserNamePasswordClientCredential(other._userName);
            if (other._httpDigest != null)
                _httpDigest = new HttpDigestClientCredential(other._httpDigest);
            _isReadOnly = other._isReadOnly;
        }

        public X509CertificateInitiatorClientCredential ClientCertificate
        {
            get
            {
                if (_clientCertificate == null)
                {
                    _clientCertificate = new X509CertificateInitiatorClientCredential();
                    if (_isReadOnly)
                        _clientCertificate.MakeReadOnly();
                }
                return _clientCertificate;
            }
        }

        public X509CertificateRecipientClientCredential ServiceCertificate
        {
            get
            {
                if (_serviceCertificate == null)
                {
                    _serviceCertificate = new X509CertificateRecipientClientCredential();
                    if (_isReadOnly)
                        _serviceCertificate.MakeReadOnly();
                }
                return _serviceCertificate;
            }
        }

        public UserNamePasswordClientCredential UserName
        {
            get
            {
                if (_userName == null)
                {
                    _userName = new UserNamePasswordClientCredential();
                    if (_isReadOnly)
                        _userName.MakeReadOnly();
                }
                return _userName;
            }
        }


        public WindowsClientCredential Windows
        {
            get
            {
                if (_windows == null)
                {
                    _windows = new WindowsClientCredential();
                    if (_isReadOnly)
                        _windows.MakeReadOnly();
                }
                return _windows;
            }
        }

        public HttpDigestClientCredential HttpDigest
        {
            get
            {
                if (_httpDigest == null)
                {
                    _httpDigest = new HttpDigestClientCredential();
                    if (_isReadOnly)
                        _httpDigest.MakeReadOnly();
                }
                return _httpDigest;
            }
        }


        internal static ClientCredentials CreateDefaultCredentials()
        {
            return new ClientCredentials();
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new ClientCredentialsSecurityTokenManager(this.Clone());
        }

        protected virtual ClientCredentials CloneCore()
        {
            return new ClientCredentials(this);
        }

        public ClientCredentials Clone()
        {
            ClientCredentials result = CloneCore();
            if (result == null || result.GetType() != this.GetType())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesignWithMessage(SR.Format(SR.CloneNotImplementedCorrectly, this.GetType(), (result != null) ? result.ToString() : "null")));
            }
            return result;
        }

        void IEndpointBehavior.Validate(ServiceEndpoint serviceEndpoint)
        {
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
        {
            if (bindingParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingParameters");
            }
            // throw if bindingParameters already has a SecurityCredentialsManager
            SecurityCredentialsManager otherCredentialsManager = bindingParameters.Find<SecurityCredentialsManager>();
            if (otherCredentialsManager != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.MultipleSecurityCredentialsManagersInChannelBindingParameters, otherCredentialsManager)));
            }
            bindingParameters.Add(this);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                SR.Format(SR.SFXEndpointBehaviorUsedOnWrongSide, typeof(ClientCredentials).Name)));
        }


        public virtual void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
            if (serviceEndpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serviceEndpoint");
            }

            if (serviceEndpoint.Binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serviceEndpoint.Binding");
            }


            if (serviceEndpoint.Binding.CreateBindingElements().Find<SecurityBindingElement>() == null)
            {
                return;
            }
        }

        // RC0 workaround to freeze credentials when the channel factory is opened
        internal void MakeReadOnly()
        {
            if (_windows != null)
                _windows.MakeReadOnly();
        }
    }
}

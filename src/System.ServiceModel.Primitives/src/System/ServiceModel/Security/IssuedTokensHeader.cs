// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal sealed class IssuedTokensHeader : MessageHeader
    {
        private SecurityStandardsManager _standardsManager;
        private string _actor;
        private bool _mustUnderstand;
        private bool _relay;
        private bool _isRefParam;

        public IssuedTokensHeader(RequestSecurityTokenResponse tokenIssuance, MessageSecurityVersion version, SecurityTokenSerializer tokenSerializer)
            : this(tokenIssuance, new SecurityStandardsManager(version, tokenSerializer))
        {
        }


        public IssuedTokensHeader(RequestSecurityTokenResponse tokenIssuance, SecurityStandardsManager standardsManager) : base()
        {
            if (tokenIssuance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenIssuance));
            }

            Collection<RequestSecurityTokenResponse> coll = new Collection<RequestSecurityTokenResponse>();
            coll.Add(tokenIssuance);
            Initialize(coll, standardsManager);
        }

        public IssuedTokensHeader(IEnumerable<RequestSecurityTokenResponse> tokenIssuances, SecurityStandardsManager standardsManager) : base()
        {
            if (tokenIssuances == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenIssuances));
            }

            int index = 0;
            Collection<RequestSecurityTokenResponse> coll = new Collection<RequestSecurityTokenResponse>();
            foreach (RequestSecurityTokenResponse rstr in tokenIssuances)
            {
                if (rstr == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(String.Format(CultureInfo.InvariantCulture, "tokenIssuances[{0}]", index));
                }

                coll.Add(rstr);
                ++index;
            }

            Initialize(coll, standardsManager);
        }

        private void Initialize(Collection<RequestSecurityTokenResponse> coll, SecurityStandardsManager standardsManager)
        {
            if (standardsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(standardsManager)));
            }

            _standardsManager = standardsManager;
            TokenIssuances = new ReadOnlyCollection<RequestSecurityTokenResponse>(coll);
            _actor = base.Actor;
            _mustUnderstand = base.MustUnderstand;
            _relay = base.Relay;
        }


        public IssuedTokensHeader(XmlReader xmlReader, MessageVersion version, SecurityStandardsManager standardsManager) : base()
        {
            if (xmlReader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(xmlReader));
            }

            if (standardsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(standardsManager)));
            }

            _standardsManager = standardsManager;
            XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
            MessageHeader.GetHeaderAttributes(reader, version, out _actor, out _mustUnderstand, out _relay, out _isRefParam);
            reader.ReadStartElement(Name, Namespace);
            Collection<RequestSecurityTokenResponse> coll = new Collection<RequestSecurityTokenResponse>();
            if (_standardsManager.TrustDriver.IsAtRequestSecurityTokenResponseCollection(reader))
            {
                RequestSecurityTokenResponseCollection rstrColl = _standardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection(reader);
                foreach (RequestSecurityTokenResponse rstr in rstrColl.RstrCollection)
                {
                    coll.Add(rstr);
                }
            }
            else
            {
                RequestSecurityTokenResponse rstr = _standardsManager.TrustDriver.CreateRequestSecurityTokenResponse(reader);
                coll.Add(rstr);
            }

            TokenIssuances = new ReadOnlyCollection<RequestSecurityTokenResponse>(coll);
            reader.ReadEndElement();
        }


        public ReadOnlyCollection<RequestSecurityTokenResponse> TokenIssuances { get; private set; }

        public override string Actor => _actor;

        public override bool IsReferenceParameter => _isRefParam;

        public override bool MustUnderstand => _mustUnderstand;

        public override bool Relay => _relay;

        public override string Name => _standardsManager.TrustDriver.IssuedTokensHeaderName;

        public override string Namespace => _standardsManager.TrustDriver.IssuedTokensHeaderNamespace;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            if (TokenIssuances.Count == 1)
            {
                _standardsManager.TrustDriver.WriteRequestSecurityTokenResponse(TokenIssuances[0], writer);
            }
            else
            {
                RequestSecurityTokenResponseCollection rstrCollection = new RequestSecurityTokenResponseCollection(TokenIssuances, _standardsManager);
                rstrCollection.WriteTo(writer);
            }
        }

        internal static Collection<RequestSecurityTokenResponse> ExtractIssuances(Message message, MessageSecurityVersion version, WSSecurityTokenSerializer tokenSerializer, string[] actors, XmlQualifiedName expectedAppliesToQName)
        {
            return ExtractIssuances(message, new SecurityStandardsManager(version, tokenSerializer), actors, expectedAppliesToQName);
        }

        // if expectedAppliesToQName is null all issuances matching the actors are returned.
        internal static Collection<RequestSecurityTokenResponse> ExtractIssuances(Message message, SecurityStandardsManager standardsManager, string[] actors, XmlQualifiedName expectedAppliesToQName)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            if (standardsManager == null)
            {
                standardsManager = SecurityStandardsManager.DefaultInstance;
            }

            if (actors == null)
            {
                throw TraceUtility.ThrowHelperArgumentNull(nameof(actors), message);
            }

            Collection<RequestSecurityTokenResponse> issuances = new Collection<RequestSecurityTokenResponse>();
            for (int i = 0; i < message.Headers.Count; ++i)
            {
                if (message.Headers[i].Name == standardsManager.TrustDriver.IssuedTokensHeaderName && message.Headers[i].Namespace == standardsManager.TrustDriver.IssuedTokensHeaderNamespace)
                {
                    bool isValidActor = false;
                    for (int j = 0; j < actors.Length; ++j)
                    {
                        if (actors[j] == message.Headers[i].Actor)
                        {
                            isValidActor = true;
                            break;
                        }
                    }
                    if (!isValidActor)
                    {
                        continue;
                    }
                    IssuedTokensHeader issuedTokensHeader = new IssuedTokensHeader(message.Headers.GetReaderAtHeader(i), message.Version, standardsManager);
                    for (int k = 0; k < issuedTokensHeader.TokenIssuances.Count; ++k)
                    {
                        bool isMatch;
                        if (expectedAppliesToQName != null)
                        {
                            string issuanceAppliesToName;
                            string issuanceAppliesToNs;
                            issuedTokensHeader.TokenIssuances[k].GetAppliesToQName(out issuanceAppliesToName, out issuanceAppliesToNs);
                            if (issuanceAppliesToName == expectedAppliesToQName.Name && issuanceAppliesToNs == expectedAppliesToQName.Namespace)
                            {
                                isMatch = true;
                            }
                            else
                            {
                                isMatch = false;
                            }
                        }
                        else
                        {
                            isMatch = true;
                        }
                        if (isMatch)
                        {
                            issuances.Add(issuedTokensHeader.TokenIssuances[k]);
                        }
                    }
                }
            }

            return issuances;
        }
    }
}

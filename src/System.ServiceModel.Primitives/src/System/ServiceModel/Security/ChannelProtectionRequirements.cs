// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net.Security;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace System.ServiceModel.Security
{
    public class ChannelProtectionRequirements
    {
        private ScopedMessagePartSpecification _outgoingEncryptionParts;

        public ChannelProtectionRequirements()
        {
            IncomingSignatureParts = new ScopedMessagePartSpecification();
            IncomingEncryptionParts = new ScopedMessagePartSpecification();
            OutgoingSignatureParts = new ScopedMessagePartSpecification();
            _outgoingEncryptionParts = new ScopedMessagePartSpecification();
        }

        public bool IsReadOnly { get; private set; }

        public ChannelProtectionRequirements(ChannelProtectionRequirements other)
        {
            if (other == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(other)));
            }

            IncomingSignatureParts = new ScopedMessagePartSpecification(other.IncomingSignatureParts);
            IncomingEncryptionParts = new ScopedMessagePartSpecification(other.IncomingEncryptionParts);
            OutgoingSignatureParts = new ScopedMessagePartSpecification(other.OutgoingSignatureParts);
            _outgoingEncryptionParts = new ScopedMessagePartSpecification(other._outgoingEncryptionParts);
        }

        internal ChannelProtectionRequirements(ChannelProtectionRequirements other, ProtectionLevel newBodyProtectionLevel)
        {
            if (other == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(other)));
            }

            IncomingSignatureParts = new ScopedMessagePartSpecification(other.IncomingSignatureParts, newBodyProtectionLevel != ProtectionLevel.None);
            IncomingEncryptionParts = new ScopedMessagePartSpecification(other.IncomingEncryptionParts, newBodyProtectionLevel == ProtectionLevel.EncryptAndSign);
            OutgoingSignatureParts = new ScopedMessagePartSpecification(other.OutgoingSignatureParts, newBodyProtectionLevel != ProtectionLevel.None);
            _outgoingEncryptionParts = new ScopedMessagePartSpecification(other._outgoingEncryptionParts, newBodyProtectionLevel == ProtectionLevel.EncryptAndSign);
        }

        public ScopedMessagePartSpecification IncomingSignatureParts { get; private set; }

        public ScopedMessagePartSpecification IncomingEncryptionParts { get; private set; }

        public ScopedMessagePartSpecification OutgoingSignatureParts { get; private set; }

        public ScopedMessagePartSpecification OutgoingEncryptionParts
        {
            get
            {
                return _outgoingEncryptionParts;
            }
        }

        public void Add(ChannelProtectionRequirements protectionRequirements)
        {
            Add(protectionRequirements, false);
        }

        public void Add(ChannelProtectionRequirements protectionRequirements, bool channelScopeOnly)
        {
            if (protectionRequirements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(protectionRequirements)));
            }

            if (protectionRequirements.IncomingSignatureParts != null)
            {
                IncomingSignatureParts.AddParts(protectionRequirements.IncomingSignatureParts.ChannelParts);
            }

            if (protectionRequirements.IncomingEncryptionParts != null)
            {
                IncomingEncryptionParts.AddParts(protectionRequirements.IncomingEncryptionParts.ChannelParts);
            }

            if (protectionRequirements.OutgoingSignatureParts != null)
            {
                OutgoingSignatureParts.AddParts(protectionRequirements.OutgoingSignatureParts.ChannelParts);
            }

            if (protectionRequirements._outgoingEncryptionParts != null)
            {
                _outgoingEncryptionParts.AddParts(protectionRequirements._outgoingEncryptionParts.ChannelParts);
            }

            if (!channelScopeOnly)
            {
                AddActionParts(IncomingSignatureParts, protectionRequirements.IncomingSignatureParts);
                AddActionParts(IncomingEncryptionParts, protectionRequirements.IncomingEncryptionParts);
                AddActionParts(OutgoingSignatureParts, protectionRequirements.OutgoingSignatureParts);
                AddActionParts(_outgoingEncryptionParts, protectionRequirements._outgoingEncryptionParts);
            }
        }

        private static void AddActionParts(ScopedMessagePartSpecification to, ScopedMessagePartSpecification from)
        {
            foreach (string action in from.Actions)
            {
                MessagePartSpecification p;
                if (from.TryGetParts(action, true, out p))
                {
                    to.AddParts(p, action);
                }
            }
        }

        public void MakeReadOnly()
        {
            if (!IsReadOnly)
            {
                IncomingSignatureParts.MakeReadOnly();
                IncomingEncryptionParts.MakeReadOnly();
                OutgoingSignatureParts.MakeReadOnly();
                _outgoingEncryptionParts.MakeReadOnly();
                IsReadOnly = true;
            }
        }

        public ChannelProtectionRequirements CreateInverse()
        {
            ChannelProtectionRequirements result = new ChannelProtectionRequirements();

            result.Add(this, true);
            result.IncomingSignatureParts = new ScopedMessagePartSpecification(OutgoingSignatureParts);
            result.OutgoingSignatureParts = new ScopedMessagePartSpecification(IncomingSignatureParts);
            result.IncomingEncryptionParts = new ScopedMessagePartSpecification(OutgoingEncryptionParts);
            result._outgoingEncryptionParts = new ScopedMessagePartSpecification(IncomingEncryptionParts);

            return result;
        }

        internal static ChannelProtectionRequirements CreateFromContract(ContractDescription contract, ISecurityCapabilities bindingElement, bool isForClient)
        {
            return CreateFromContract(contract, bindingElement.SupportedRequestProtectionLevel, bindingElement.SupportedResponseProtectionLevel, isForClient);
        }

        private static MessagePartSpecification UnionMessagePartSpecifications(ScopedMessagePartSpecification actionParts)
        {
            MessagePartSpecification result = new MessagePartSpecification(false);
            foreach (string action in actionParts.Actions)
            {
                MessagePartSpecification parts;
                if (actionParts.TryGetParts(action, out parts))
                {
                    if (parts.IsBodyIncluded)
                    {
                        result.IsBodyIncluded = true;
                    }
                    foreach (XmlQualifiedName headerType in parts.HeaderTypes)
                    {
                        if (!result.IsHeaderIncluded(headerType.Name, headerType.Namespace))
                        {
                            result.HeaderTypes.Add(headerType);
                        }
                    }
                }
            }
            return result;
        }

        internal static ChannelProtectionRequirements CreateFromContractAndUnionResponseProtectionRequirements(ContractDescription contract, ISecurityCapabilities bindingElement, bool isForClient)
        {
            ChannelProtectionRequirements contractRequirements = CreateFromContract(contract, bindingElement.SupportedRequestProtectionLevel, bindingElement.SupportedResponseProtectionLevel, isForClient);
            // union all the protection requirements for the response actions
            ChannelProtectionRequirements result = new ChannelProtectionRequirements();

            if (isForClient)
            {
                result.IncomingEncryptionParts.AddParts(UnionMessagePartSpecifications(contractRequirements.IncomingEncryptionParts), MessageHeaders.WildcardAction);
                result.IncomingSignatureParts.AddParts(UnionMessagePartSpecifications(contractRequirements.IncomingSignatureParts), MessageHeaders.WildcardAction);
                contractRequirements.OutgoingEncryptionParts.CopyTo(result.OutgoingEncryptionParts);
                contractRequirements.OutgoingSignatureParts.CopyTo(result.OutgoingSignatureParts);
            }
            else
            {
                result.OutgoingEncryptionParts.AddParts(UnionMessagePartSpecifications(contractRequirements.OutgoingEncryptionParts), MessageHeaders.WildcardAction);
                result.OutgoingSignatureParts.AddParts(UnionMessagePartSpecifications(contractRequirements.OutgoingSignatureParts), MessageHeaders.WildcardAction);
                contractRequirements.IncomingEncryptionParts.CopyTo(result.IncomingEncryptionParts);
                contractRequirements.IncomingSignatureParts.CopyTo(result.IncomingSignatureParts);
            }
            return result;
        }

        internal static ChannelProtectionRequirements CreateFromContract(ContractDescription contract, ProtectionLevel defaultRequestProtectionLevel, ProtectionLevel defaultResponseProtectionLevel, bool isForClient)
        {
            if (contract == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(contract)));
            }

            ChannelProtectionRequirements requirements = new ChannelProtectionRequirements();

            ProtectionLevel contractScopeDefaultRequestProtectionLevel;
            ProtectionLevel contractScopeDefaultResponseProtectionLevel;
            if (contract.HasProtectionLevel)
            {
                contractScopeDefaultRequestProtectionLevel = contract.ProtectionLevel;
                contractScopeDefaultResponseProtectionLevel = contract.ProtectionLevel;
            }
            else
            {
                contractScopeDefaultRequestProtectionLevel = defaultRequestProtectionLevel;
                contractScopeDefaultResponseProtectionLevel = defaultResponseProtectionLevel;
            }

            foreach (OperationDescription operation in contract.Operations)
            {
                ProtectionLevel operationScopeDefaultRequestProtectionLevel;
                ProtectionLevel operationScopeDefaultResponseProtectionLevel;

                operationScopeDefaultRequestProtectionLevel = contractScopeDefaultRequestProtectionLevel;
                operationScopeDefaultResponseProtectionLevel = contractScopeDefaultResponseProtectionLevel;

                foreach (MessageDescription message in operation.Messages)
                {
                    ProtectionLevel messageScopeDefaultProtectionLevel;
                    if (message.HasProtectionLevel)
                    {
                        messageScopeDefaultProtectionLevel = message.ProtectionLevel;
                    }
                    else if (message.Direction == MessageDirection.Input)
                    {
                        messageScopeDefaultProtectionLevel = operationScopeDefaultRequestProtectionLevel;
                    }
                    else
                    {
                        messageScopeDefaultProtectionLevel = operationScopeDefaultResponseProtectionLevel;
                    }

                    MessagePartSpecification signedParts = new MessagePartSpecification();
                    MessagePartSpecification encryptedParts = new MessagePartSpecification();

                    // determine header protection requirements for message
                    foreach (MessageHeaderDescription header in message.Headers)
                    {
                        AddHeaderProtectionRequirements(header, signedParts, encryptedParts, messageScopeDefaultProtectionLevel);
                    }

                    // determine body protection requirements for message
                    ProtectionLevel bodyProtectionLevel;
                    if (message.Body.Parts.Count > 0)
                    {
                        // initialize the body protection level to none. all the body parts will be
                        // unioned to get the effective body protection level
                        bodyProtectionLevel = ProtectionLevel.None;
                    }
                    else if (message.Body.ReturnValue != null)
                    {
                        if (!(message.Body.ReturnValue.GetType().Equals(typeof(MessagePartDescription))))
                        {
                            Fx.Assert("Only body return values are supported currently");
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.OnlyBodyReturnValuesSupported));
                        }
                        MessagePartDescription desc = message.Body.ReturnValue;
                        bodyProtectionLevel = desc.HasProtectionLevel ? desc.ProtectionLevel : messageScopeDefaultProtectionLevel;
                    }
                    else
                    {
                        bodyProtectionLevel = messageScopeDefaultProtectionLevel;
                    }

                    // determine body protection requirements for message
                    if (message.Body.Parts.Count > 0)
                    {
                        foreach (MessagePartDescription body in message.Body.Parts)
                        {
                            ProtectionLevel partProtectionLevel = body.HasProtectionLevel ? body.ProtectionLevel : messageScopeDefaultProtectionLevel;
                            bodyProtectionLevel = ProtectionLevelHelper.Max(bodyProtectionLevel, partProtectionLevel);
                            if (bodyProtectionLevel == ProtectionLevel.EncryptAndSign)
                            {
                                break;
                            }
                        }
                    }
                    if (bodyProtectionLevel != ProtectionLevel.None)
                    {
                        signedParts.IsBodyIncluded = true;
                        if (bodyProtectionLevel == ProtectionLevel.EncryptAndSign)
                        {
                            encryptedParts.IsBodyIncluded = true;
                        }
                    }

                    // add requirements for message 
                    if (message.Direction == MessageDirection.Input)
                    {
                        requirements.IncomingSignatureParts.AddParts(signedParts, message.Action);
                        requirements.IncomingEncryptionParts.AddParts(encryptedParts, message.Action);
                    }
                    else
                    {
                        requirements.OutgoingSignatureParts.AddParts(signedParts, message.Action);
                        requirements.OutgoingEncryptionParts.AddParts(encryptedParts, message.Action);
                    }
                }
                if (operation.Faults != null)
                {
                    if (operation.IsServerInitiated())
                    {
                        AddFaultProtectionRequirements(operation.Faults, requirements, operationScopeDefaultRequestProtectionLevel, true);
                    }
                    else
                    {
                        AddFaultProtectionRequirements(operation.Faults, requirements, operationScopeDefaultResponseProtectionLevel, false);
                    }
                }
            }

            return requirements;
        }

        private static void AddHeaderProtectionRequirements(MessageHeaderDescription header, MessagePartSpecification signedParts,
            MessagePartSpecification encryptedParts, ProtectionLevel defaultProtectionLevel)
        {
            ProtectionLevel p = header.HasProtectionLevel ? header.ProtectionLevel : defaultProtectionLevel;
            if (p != ProtectionLevel.None)
            {
                XmlQualifiedName headerName = new XmlQualifiedName(header.Name, header.Namespace);
                signedParts.HeaderTypes.Add(headerName);
                if (p == ProtectionLevel.EncryptAndSign)
                {
                    encryptedParts.HeaderTypes.Add(headerName);
                }
            }
        }

        private static void AddFaultProtectionRequirements(FaultDescriptionCollection faults, ChannelProtectionRequirements requirements, ProtectionLevel defaultProtectionLevel, bool addToIncoming)
        {
            if (faults == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(faults)));
            }

            if (requirements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(requirements)));
            }

            foreach (FaultDescription fault in faults)
            {
                MessagePartSpecification signedParts = new MessagePartSpecification();
                MessagePartSpecification encryptedParts = new MessagePartSpecification();
                ProtectionLevel p = fault.HasProtectionLevel ? fault.ProtectionLevel : defaultProtectionLevel;
                if (p != ProtectionLevel.None)
                {
                    signedParts.IsBodyIncluded = true;
                    if (p == ProtectionLevel.EncryptAndSign)
                    {
                        encryptedParts.IsBodyIncluded = true;
                    }
                }
                if (addToIncoming)
                {
                    requirements.IncomingSignatureParts.AddParts(signedParts, fault.Action);
                    requirements.IncomingEncryptionParts.AddParts(encryptedParts, fault.Action);
                }
                else
                {
                    requirements.OutgoingSignatureParts.AddParts(signedParts, fault.Action);
                    requirements.OutgoingEncryptionParts.AddParts(encryptedParts, fault.Action);
                }
            }
        }
    }
}

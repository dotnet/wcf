// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    internal sealed class TransactionChannelFactory<TChannel> : LayeredChannelFactory<TChannel>, ITransactionChannelManager
    {
        private SecurityStandardsManager _standardsManager;
        private Dictionary<DirectionalAction, TransactionFlowOption> _dictionary;
        private TransactionProtocol _transactionProtocol;
        private bool _allowWildcardAction;

        public TransactionChannelFactory(
            TransactionProtocol transactionProtocol,
            BindingContext context,
            Dictionary<DirectionalAction, TransactionFlowOption> dictionary,
            bool allowWildcardAction)
            : base(context.Binding, context.BuildInnerChannelFactory<TChannel>())
        {
            _dictionary = dictionary;
            TransactionProtocol = transactionProtocol;
            _allowWildcardAction = allowWildcardAction;
            _standardsManager = SecurityStandardsHelper.CreateStandardsManager(TransactionProtocol);
        }

        public TransactionProtocol TransactionProtocol
        {
            get { return _transactionProtocol; }
            set
            {
                if (!TransactionProtocol.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ArgumentException(SRP.SFxBadTransactionProtocols));
                _transactionProtocol = value;
            }
        }

        public TransactionFlowOption FlowIssuedTokens { get; set; }

        public SecurityStandardsManager StandardsManager
        {
            get { return _standardsManager; }
            set
            {
                _standardsManager = value ?? SecurityStandardsHelper.CreateStandardsManager(_transactionProtocol);
            }
        }

        public IDictionary<DirectionalAction, TransactionFlowOption> Dictionary => _dictionary;

        public TransactionFlowOption GetTransaction(MessageDirection direction, string action)
        {
            TransactionFlowOption txOption;
            if (!_dictionary.TryGetValue(new DirectionalAction(direction, action), out txOption))
            {
                if (_allowWildcardAction && _dictionary.TryGetValue(new DirectionalAction(direction, MessageHeaders.WildcardAction), out txOption))
                {
                    return txOption;
                }
                else
                {
                    return TransactionFlowOption.NotAllowed;
                }
            }
            else
            {
                return txOption;
            }
        }

        protected override TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            TChannel innerChannel = ((IChannelFactory<TChannel>)InnerChannelFactory).CreateChannel(remoteAddress, via);
            return CreateTransactionChannel(innerChannel);
        }

        private TChannel CreateTransactionChannel(TChannel innerChannel)
        {
            if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new TransactionDuplexSessionChannel(this, (IDuplexSessionChannel)(object)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (TChannel)(object)new TransactionRequestSessionChannel(this, (IRequestSessionChannel)(object)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (TChannel)(object)new TransactionOutputSessionChannel(this, (IOutputSessionChannel)(object)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IOutputChannel))
            {
                return (TChannel)(object)new TransactionOutputChannel(this, (IOutputChannel)(object)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new TransactionRequestChannel(this, (IRequestChannel)(object)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new TransactionDuplexChannel(this, (IDuplexChannel)(object)innerChannel);
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    CreateChannelTypeNotSupportedException(typeof(TChannel)));
            }
        }

        private sealed class TransactionOutputChannel : TransactionOutputChannelGeneric<IOutputChannel>
        {
            public TransactionOutputChannel(ChannelManagerBase channelManager, IOutputChannel innerChannel)
                : base(channelManager, innerChannel)
            {
            }
        }

        private sealed class TransactionRequestChannel : TransactionRequestChannelGeneric<IRequestChannel>
        {
            public TransactionRequestChannel(ChannelManagerBase channelManager, IRequestChannel innerChannel)
                : base(channelManager, innerChannel)
            {
            }
        }

        private sealed class TransactionDuplexChannel : TransactionOutputDuplexChannelGeneric<IDuplexChannel>
        {
            public TransactionDuplexChannel(ChannelManagerBase channelManager, IDuplexChannel innerChannel)
                : base(channelManager, innerChannel)
            {
            }
        }

        private sealed class TransactionOutputSessionChannel : TransactionOutputChannelGeneric<IOutputSessionChannel>, IOutputSessionChannel
        {
            public TransactionOutputSessionChannel(ChannelManagerBase channelManager, IOutputSessionChannel innerChannel)
                : base(channelManager, innerChannel)
            {
            }

            public IOutputSession Session => InnerChannel.Session;
        }

        private sealed class TransactionRequestSessionChannel : TransactionRequestChannelGeneric<IRequestSessionChannel>, IRequestSessionChannel
        {
            public TransactionRequestSessionChannel(ChannelManagerBase channelManager, IRequestSessionChannel innerChannel)
                : base(channelManager, innerChannel)
            {
            }

            public IOutputSession Session => InnerChannel.Session;
        }

        private sealed class TransactionDuplexSessionChannel : TransactionOutputDuplexChannelGeneric<IDuplexSessionChannel>, IDuplexSessionChannel
        {
            public TransactionDuplexSessionChannel(ChannelManagerBase channelManager, IDuplexSessionChannel innerChannel)
                : base(channelManager, innerChannel)
            {
            }

            public IDuplexSession Session => InnerChannel.Session;
        }
    }

    internal static class SecurityStandardsHelper
    {
        private static readonly SecurityStandardsManager s_securityStandardsManager2007 =
            CreateStandardsManager(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12);

        private static SecurityStandardsManager CreateStandardsManager(MessageSecurityVersion securityVersion)
        {
            return new SecurityStandardsManager(
                securityVersion,
                new WSSecurityTokenSerializer(securityVersion.SecurityVersion, securityVersion.TrustVersion, securityVersion.SecureConversationVersion, false, null, null, null));
        }

        public static SecurityStandardsManager CreateStandardsManager(TransactionProtocol transactionProtocol)
        {
            if (transactionProtocol == TransactionProtocol.WSAtomicTransactionOctober2004 ||
                transactionProtocol == TransactionProtocol.OleTransactions)
            {
                return SecurityStandardsManager.DefaultInstance;
            }
            else
            {
                return s_securityStandardsManager2007;
            }
        }
    }
}

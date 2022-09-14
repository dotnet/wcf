// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;

namespace System.ServiceModel.Channels
{
    public class ChannelParameterCollection : Collection<object>
    {
        private IChannel _channel;

        public ChannelParameterCollection()
        {
        }

        public ChannelParameterCollection(IChannel channel)
        {
            _channel = channel;
        }

        protected virtual IChannel Channel
        {
            get { return _channel; }
        }

        public void PropagateChannelParameters(IChannel innerChannel)
        {
            if (innerChannel == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(innerChannel));
            }

            ThrowIfMutable();

            ChannelParameterCollection innerCollection = innerChannel.GetProperty<ChannelParameterCollection>();
            if (innerCollection != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    innerCollection.Add(this[i]);
                }
            }
        }

        protected override void ClearItems()
        {
            ThrowIfDisposedOrImmutable();
            base.ClearItems();
        }

        protected override void InsertItem(int index, object item)
        {
            ThrowIfDisposedOrImmutable();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            ThrowIfDisposedOrImmutable();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, object item)
        {
            ThrowIfDisposedOrImmutable();
            base.SetItem(index, item);
        }

        private void ThrowIfDisposedOrImmutable()
        {
            IChannel channel = Channel;
            if (channel != null)
            {
                CommunicationState state = channel.State;
                string text = null;

                switch (state)
                {
                    case CommunicationState.Created:
                        break;

                    case CommunicationState.Opening:
                    case CommunicationState.Opened:
                    case CommunicationState.Closing:
                    case CommunicationState.Closed:
                    case CommunicationState.Faulted:
                        text = SRP.Format(SRP.ChannelParametersCannotBeModified,
                                            channel.GetType().ToString(), state.ToString());
                        break;

                    default:
                        text = SRP.Format(SRP.CommunicationObjectInInvalidState,
                                            channel.GetType().ToString(), state.ToString());
                        break;
                }

                if (text != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(text));
                }
            }
        }

        private void ThrowIfMutable()
        {
            IChannel channel = Channel;
            if (channel != null)
            {
                CommunicationState state = channel.State;
                string text = null;

                switch (state)
                {
                    case CommunicationState.Created:
                        text = SRP.Format(SRP.ChannelParametersCannotBePropagated,
                                            channel.GetType().ToString(), state.ToString());
                        break;

                    case CommunicationState.Opening:
                    case CommunicationState.Opened:
                    case CommunicationState.Closing:
                    case CommunicationState.Closed:
                    case CommunicationState.Faulted:
                        break;

                    default:
                        text = SRP.Format(SRP.CommunicationObjectInInvalidState,
                                            channel.GetType().ToString(), state.ToString());
                        break;
                }

                if (text != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(text));
                }
            }
        }
    }
}

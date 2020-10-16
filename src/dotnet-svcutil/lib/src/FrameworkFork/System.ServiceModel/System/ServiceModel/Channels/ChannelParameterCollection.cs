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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerChannel");
            }

            this.ThrowIfMutable();

            ChannelParameterCollection innerCollection = innerChannel.GetProperty<ChannelParameterCollection>();
            if (innerCollection != null)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    innerCollection.Add(this[i]);
                }
            }
        }

        protected override void ClearItems()
        {
            this.ThrowIfDisposedOrImmutable();
            base.ClearItems();
        }

        protected override void InsertItem(int index, object item)
        {
            this.ThrowIfDisposedOrImmutable();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            this.ThrowIfDisposedOrImmutable();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, object item)
        {
            this.ThrowIfDisposedOrImmutable();
            base.SetItem(index, item);
        }

        private void ThrowIfDisposedOrImmutable()
        {
            IChannel channel = this.Channel;
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
                        text = string.Format(SRServiceModel.ChannelParametersCannotBeModified,
                                            channel.GetType().ToString(), state.ToString());
                        break;

                    default:
                        text = string.Format(SRServiceModel.CommunicationObjectInInvalidState,
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
            IChannel channel = this.Channel;
            if (channel != null)
            {
                CommunicationState state = channel.State;
                string text = null;

                switch (state)
                {
                    case CommunicationState.Created:
                        text = string.Format(SRServiceModel.ChannelParametersCannotBePropagated,
                                            channel.GetType().ToString(), state.ToString());
                        break;

                    case CommunicationState.Opening:
                    case CommunicationState.Opened:
                    case CommunicationState.Closing:
                    case CommunicationState.Closed:
                    case CommunicationState.Faulted:
                        break;

                    default:
                        text = string.Format(SRServiceModel.CommunicationObjectInInvalidState,
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

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Channels
{
    internal sealed class ChannelBindingMessageProperty : IDisposable, IMessageProperty
    {
        private const string propertyName = "ChannelBindingMessageProperty";

        private ChannelBinding _channelBinding;
        private object _thisLock;
        private bool _ownsCleanup;
        private int _refCount;

        public ChannelBindingMessageProperty(ChannelBinding channelBinding, bool ownsCleanup)
        {
            _refCount = 1;
            _thisLock = new object();
            _channelBinding = channelBinding ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelBinding));
            _ownsCleanup = ownsCleanup;
        }

        public static string Name { get { return propertyName; } }

        private bool IsDisposed
        {
            get
            {
                return _refCount <= 0;
            }
        }

        public ChannelBinding ChannelBinding
        {
            get
            {
                ThrowIfDisposed();
                return _channelBinding;
            }
        }

        public static bool TryGet(Message message, out ChannelBindingMessageProperty property)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            return TryGet(message.Properties, out property);
        }

        public static bool TryGet(MessageProperties properties, out ChannelBindingMessageProperty property)
        {
            if (properties == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(properties));
            }

            property = null;
            object value;

            if (properties.TryGetValue(ChannelBindingMessageProperty.Name, out value))
            {
                property = value as ChannelBindingMessageProperty;
                return property != null;
            }

            return false;
        }

        public void AddTo(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            AddTo(message.Properties);
        }

        public void AddTo(MessageProperties properties)
        {
            ThrowIfDisposed();
            if (properties == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(properties));
            }

            properties.Add(ChannelBindingMessageProperty.Name, this);
        }

        public IMessageProperty CreateCopy()
        {
            lock (_thisLock)
            {
                ThrowIfDisposed();
                _refCount++;
                return this;
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                lock (_thisLock)
                {
                    if (!IsDisposed && --_refCount == 0)
                    {
                        if (_ownsCleanup)
                        {
                            // Accessing via IDisposable to avoid Security check (functionally the same)
                            ((IDisposable)_channelBinding).Dispose();
                        }
                    }
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().FullName));
            }
        }
    }
}

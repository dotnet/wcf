// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;

namespace System.ServiceModel.MsmqIntegration
{
    // Wrapper around a single MSMQ message body of type T plus the MSMQ
    // metadata exposed via MsmqIntegrationMessageProperty. Mirrors the
    // netfx public type — applications porting from .NET Framework can
    // keep their MsmqMessage<T> usage unchanged.
    [SupportedOSPlatform("windows")]
    public sealed class MsmqMessage<T>
    {
        private readonly MsmqIntegrationMessageProperty _property;

        public MsmqMessage(T body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }
            _property = new MsmqIntegrationMessageProperty { Body = body };
        }

        internal MsmqMessage()
        {
            _property = new MsmqIntegrationMessageProperty();
        }

        internal MsmqIntegrationMessageProperty Property => _property;

        public T Body
        {
            get { return (T)_property.Body; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _property.Body = value;
            }
        }

        public AcknowledgeTypes? AcknowledgeType
        {
            get => _property.AcknowledgeType;
            set => _property.AcknowledgeType = value;
        }

        public Acknowledgment? Acknowledgment => _property.Acknowledgment;

        public Uri AdministrationQueue
        {
            get => _property.AdministrationQueue;
            set => _property.AdministrationQueue = value;
        }

        public int? AppSpecific
        {
            get => _property.AppSpecific;
            set => _property.AppSpecific = value;
        }

        public DateTime? ArrivedTime => _property.ArrivedTime;
        public bool? Authenticated => _property.Authenticated;

        public int? BodyType
        {
            get => _property.BodyType;
            set => _property.BodyType = value;
        }

        public string CorrelationId
        {
            get => _property.CorrelationId;
            set => _property.CorrelationId = value;
        }

        public Uri DestinationQueue => _property.DestinationQueue;

        public byte[] Extension
        {
            get => _property.Extension;
            set => _property.Extension = value;
        }

        public string Id => _property.Id;

        public string Label
        {
            get => _property.Label;
            set => _property.Label = value;
        }

        public MessageType? MessageType => _property.MessageType;

        public MessagePriority? Priority
        {
            get => _property.Priority;
            set => _property.Priority = value;
        }

        public Uri ResponseQueue
        {
            get => _property.ResponseQueue;
            set => _property.ResponseQueue = value;
        }

        public byte[] SenderId => _property.SenderId;
        public DateTime? SentTime => _property.SentTime;

        public TimeSpan? TimeToReachQueue
        {
            get => _property.TimeToReachQueue;
            set => _property.TimeToReachQueue = value;
        }
    }
}

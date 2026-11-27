// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public abstract class MsmqBindingBase : Binding, IBindingRuntimePreferences
    {
        internal MsmqBindingElementBase _transport;

        protected MsmqBindingBase()
        {
        }

        [DefaultValue(null)]
        public Uri CustomDeadLetterQueue
        {
            get
            {
                return _transport.CustomDeadLetterQueue;
            }
            set
            {
                _transport.CustomDeadLetterQueue = value;
            }
        }

        [DefaultValue(MsmqDefaults.DeadLetterQueue)]
        public DeadLetterQueue DeadLetterQueue
        {
            get { return _transport.DeadLetterQueue; }
            set { _transport.DeadLetterQueue = value; }
        }

        [DefaultValue(MsmqDefaults.Durable)]
        public bool Durable
        {
            get { return _transport.Durable; }
            set { _transport.Durable = value; }
        }

        [DefaultValue(MsmqDefaults.ExactlyOnce)]
        public bool ExactlyOnce
        {
            get { return _transport.ExactlyOnce; }
            set { _transport.ExactlyOnce = value; }
        }

        // TransportDefaults.MaxReceivedMessageSize is internal to
        // System.ServiceModel.Primitives, so the literal is repeated here to
        // keep the attribute in step with .NET Framework's surface.
        [DefaultValue((long)65536)]
        public long MaxReceivedMessageSize
        {
            get { return _transport.MaxReceivedMessageSize; }
            set { _transport.MaxReceivedMessageSize = value; }
        }

        [DefaultValue(MsmqDefaults.ReceiveRetryCount)]
        public int ReceiveRetryCount
        {
            get { return _transport.ReceiveRetryCount; }
            set { _transport.ReceiveRetryCount = value; }
        }

        [DefaultValue(MsmqDefaults.MaxRetryCycles)]
        public int MaxRetryCycles
        {
            get { return _transport.MaxRetryCycles; }
            set { _transport.MaxRetryCycles = value; }
        }

        [DefaultValue(MsmqDefaults.ReceiveContextEnabled)]
        public bool ReceiveContextEnabled
        {
            get { return _transport.ReceiveContextEnabled; }
            set { _transport.ReceiveContextEnabled = value; }
        }

        [DefaultValue(MsmqDefaults.ReceiveErrorHandling)]
        public ReceiveErrorHandling ReceiveErrorHandling
        {
            get { return _transport.ReceiveErrorHandling; }
            set { _transport.ReceiveErrorHandling = value; }
        }

        [DefaultValue(typeof(TimeSpan), MsmqDefaults.RetryCycleDelayString)]
        public TimeSpan RetryCycleDelay
        {
            get { return _transport.RetryCycleDelay; }
            set { _transport.RetryCycleDelay = value; }
        }

        public override string Scheme => _transport.Scheme;

        [DefaultValue(typeof(TimeSpan), MsmqDefaults.TimeToLiveString)]
        public TimeSpan TimeToLive
        {
            get { return _transport.TimeToLive; }
            set { _transport.TimeToLive = value; }
        }

        [DefaultValue(MsmqDefaults.UseSourceJournal)]
        public bool UseSourceJournal
        {
            get { return _transport.UseSourceJournal; }
            set { _transport.UseSourceJournal = value; }
        }

        [DefaultValue(MsmqDefaults.UseMsmqTracing)]
        public bool UseMsmqTracing
        {
            get { return _transport.UseMsmqTracing; }
            set { _transport.UseMsmqTracing = value; }
        }

        [DefaultValue(typeof(TimeSpan), MsmqDefaults.ValidityDurationString)]
        public TimeSpan ValidityDuration
        {
            get { return _transport.ValidityDuration; }
            set { _transport.ValidityDuration = value; }
        }

        bool IBindingRuntimePreferences.ReceiveSynchronously => ExactlyOnce;
    }
}

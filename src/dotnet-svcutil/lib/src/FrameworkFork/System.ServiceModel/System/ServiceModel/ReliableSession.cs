// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System.ServiceModel.Channels;

    // The only purpose in life for these classes is so that, on standard bindings, you can say
    //     binding.ReliableSession.Ordered
    //     binding.ReliableSession.InactivityTimeout
    //     binding.ReliableSession.Enabled
    // where these properties are "bucketized" all under .ReliableSession, which makes them easier to 
    // discover/Intellisense
    public class ReliableSession
    {
        private ReliableSessionBindingElement _element;

        public ReliableSession()
        {
            _element = new ReliableSessionBindingElement();
        }

        public ReliableSession(ReliableSessionBindingElement reliableSessionBindingElement)
        {
            if (reliableSessionBindingElement == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reliableSessionBindingElement");
            _element = reliableSessionBindingElement;
        }

        public bool Ordered
        {
            get { return _element.Ordered; }
            set { _element.Ordered = value; }
        }

        public TimeSpan InactivityTimeout
        {
            get { return _element.InactivityTimeout; }
            set
            {
                if (value <= TimeSpan.Zero)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                                                    SRServiceModel.ValueMustBePositive));

                _element.InactivityTimeout = value;
            }
        }

        internal void CopySettings(ReliableSession copyFrom)
        {
            Ordered = copyFrom.Ordered;
            InactivityTimeout = copyFrom.InactivityTimeout;
        }
    }

    public class OptionalReliableSession : ReliableSession
    {
        public OptionalReliableSession() : base() { }

        public OptionalReliableSession(ReliableSessionBindingElement reliableSessionBindingElement) : base(reliableSessionBindingElement) { }

        // We don't include DefaultValue here because this defaults to false, so omitting it would make the XAML somewhat misleading
        public bool Enabled
        {
            get;
            set;
        }

        internal void CopySettings(OptionalReliableSession copyFrom)
        {
            Enabled = copyFrom.Enabled;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.MessageMember, AllowMultiple = false, Inherited = false)]
    public class MessageHeaderAttribute : MessageContractMemberAttribute
    {
        private bool _mustUnderstand;
        private bool _relay;
        private string _actor;

        public bool MustUnderstand
        {
            get { return _mustUnderstand; }
            set { _mustUnderstand = value; }
        }

        public bool Relay
        {
            get { return _relay; }
            set { _relay = value; }
        }

        public string Actor
        {
            get { return _actor; }
            set { _actor = value; }
        }
    }
}

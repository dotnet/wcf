// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Principal;

namespace System.ServiceModel.Security.Tokens
{
#if SUPPORTS_WINDOWSIDENTITY // NegotiateStream
    internal class WindowsSidIdentity : IIdentity
    {
        SecurityIdentifier _sid;
        string _name;
        string _authenticationType;

        public WindowsSidIdentity(SecurityIdentifier sid)
        {
            if (sid == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("sid");

            _sid = sid;
            _authenticationType = String.Empty;
        }

        public WindowsSidIdentity(SecurityIdentifier sid, string name, string authenticationType)
        {
            if (sid == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("sid");
            if (name == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
            if (authenticationType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("authenticationType");

            _sid = sid;
            _name = name;
            _authenticationType = authenticationType;
        }

        public SecurityIdentifier SecurityIdentifier
        {
            get { return _sid; }
        }

        public string AuthenticationType 
        {
            get { return _authenticationType; }
        }
        
        public bool IsAuthenticated
        { 
            get { return true; } 
        }

        public string Name 
        {
            get
            {
                if (_name == null)
                    _name = ((NTAccount)_sid.Translate(typeof(NTAccount))).Value;
                return _name;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            WindowsSidIdentity sidIdentity = obj as WindowsSidIdentity;
            if (sidIdentity == null)
                return false;

            return _sid == sidIdentity.SecurityIdentifier;
        }

        public override int GetHashCode()
        {
            return _sid.GetHashCode();
        }
    }
#endif // SUPPORTS_WINDOWSIDENTITY
}

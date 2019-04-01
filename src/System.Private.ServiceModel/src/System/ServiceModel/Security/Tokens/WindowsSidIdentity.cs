// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Security.Principal;

namespace System.ServiceModel.Security.Tokens
{
    internal class WindowsSidIdentity : IIdentity
    {
        private string _name;

        public WindowsSidIdentity(SecurityIdentifier sid)
        {
            SecurityIdentifier = sid ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(sid));
            AuthenticationType = String.Empty;
        }

        public WindowsSidIdentity(SecurityIdentifier sid, string name, string authenticationType)
        {
            SecurityIdentifier = sid ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(sid));
            _name = name ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(name));
            AuthenticationType = authenticationType ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(authenticationType));
        }

        public SecurityIdentifier SecurityIdentifier { get; }

        public string AuthenticationType { get; }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = ((NTAccount)SecurityIdentifier.Translate(typeof(NTAccount))).Value;
                }

                return _name;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            WindowsSidIdentity sidIdentity = obj as WindowsSidIdentity;
            if (sidIdentity == null)
            {
                return false;
            }

            return SecurityIdentifier == sidIdentity.SecurityIdentifier;
        }

        public override int GetHashCode()
        {
            return SecurityIdentifier.GetHashCode();
        }
    }
}

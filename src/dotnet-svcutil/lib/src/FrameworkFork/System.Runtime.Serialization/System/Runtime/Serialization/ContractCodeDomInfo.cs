// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    using System;
    using Microsoft.CodeDom;
    using System.Collections.Generic;

    internal class ContractCodeDomInfo
    {
        internal bool IsProcessed;
        internal CodeTypeDeclaration TypeDeclaration;
        internal CodeTypeReference TypeReference;
        internal CodeNamespace CodeNamespace;
        internal bool ReferencedTypeExists;
        internal bool UsesWildcardNamespace;
        private string _clrNamespace;
        private Dictionary<string, object> _memberNames;

        internal string ClrNamespace
        {
            get { return (ReferencedTypeExists ? null : _clrNamespace); }
            set
            {
                if (ReferencedTypeExists)
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRSerialization.Format(SRSerialization.CannotSetNamespaceForReferencedType, TypeReference.BaseType)));
                else
                    _clrNamespace = value;
            }
        }

        internal Dictionary<string, object> GetMemberNames()
        {
            if (ReferencedTypeExists)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRSerialization.Format(SRSerialization.CannotSetMembersForReferencedType, TypeReference.BaseType)));
            else
            {
                if (_memberNames == null)
                {
                    _memberNames = new Dictionary<string, object>(StringComparer.Ordinal);
                }
                return _memberNames;
            }
        }
    }
}

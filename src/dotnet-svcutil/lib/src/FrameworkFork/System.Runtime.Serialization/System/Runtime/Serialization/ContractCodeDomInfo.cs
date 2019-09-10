//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
 
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
        string clrNamespace;
        Dictionary<string, object> memberNames;

        internal string ClrNamespace
        {
            get { return (ReferencedTypeExists ? null : clrNamespace); }
            set
            {
                if (ReferencedTypeExists)
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRSerialization.Format(SRSerialization.CannotSetNamespaceForReferencedType, TypeReference.BaseType)));
                else
                    clrNamespace = value;
            }
        }

        internal Dictionary<string, object> GetMemberNames()
        {
            if (ReferencedTypeExists)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRSerialization.Format(SRSerialization.CannotSetMembersForReferencedType, TypeReference.BaseType)));
            else
            {
                if (memberNames == null)
                {
                    memberNames = new Dictionary<string, object>(StringComparer.Ordinal);
                }
                return memberNames;
            }
        }
    }
}

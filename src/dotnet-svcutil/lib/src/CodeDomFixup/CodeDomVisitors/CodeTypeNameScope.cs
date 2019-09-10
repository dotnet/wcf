//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class CodeTypeNameScope : NameScope
    {
        Dictionary<string, CodeTypeMember> nameTable = new Dictionary<string, CodeTypeMember>();
        public CodeTypeNameScope(CodeTypeDeclaration type)
        {
            foreach (CodeTypeMember member in type.Members)
            {
                nameTable[member.Name] = member;
            }
        }

        public override bool Contains(string key)
        {
            if (this.nameTable == null)
                throw new ObjectDisposedException(GetType().Name);

            return this.nameTable.ContainsKey(key);
        }

        protected override void OnDispose()
        {
            this.nameTable = null;
        }
    }
}
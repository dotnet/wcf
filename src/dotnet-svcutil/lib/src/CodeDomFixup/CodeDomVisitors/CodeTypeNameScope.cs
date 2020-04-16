// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class CodeTypeNameScope : NameScope
    {
        private Dictionary<string, CodeTypeMember> _nameTable = new Dictionary<string, CodeTypeMember>();
        public CodeTypeNameScope(CodeTypeDeclaration type)
        {
            foreach (CodeTypeMember member in type.Members)
            {
                _nameTable[member.Name] = member;
            }
        }

        public override bool Contains(string key)
        {
            if (_nameTable == null)
                throw new ObjectDisposedException(GetType().Name);

            return _nameTable.ContainsKey(key);
        }

        protected override void OnDispose()
        {
            _nameTable = null;
        }
    }
}

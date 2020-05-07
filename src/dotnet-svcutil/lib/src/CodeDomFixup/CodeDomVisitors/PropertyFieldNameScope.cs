// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class PropertyFieldNameScope : NameScope
    {
        private Dictionary<string, MemberInfo> _nameTable = new Dictionary<string, MemberInfo>();
        public PropertyFieldNameScope(Type type)
        {
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                _nameTable[property.Name] = property;
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                _nameTable[field.Name] = field;
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

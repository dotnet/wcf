//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{

    internal class PropertyFieldNameScope : NameScope
    {
        Dictionary<string, MemberInfo> nameTable = new Dictionary<string, MemberInfo>();
        public PropertyFieldNameScope(Type type)
        {
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                nameTable[property.Name] = property;
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                nameTable[field.Name] = field;
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
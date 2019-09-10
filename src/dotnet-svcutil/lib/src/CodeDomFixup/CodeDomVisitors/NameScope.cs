//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class NameScope : IDisposable
    {
        public abstract bool Contains(string key);
        protected abstract void OnDispose();
        void IDisposable.Dispose()
        {
            OnDispose();
        }
        public string UniqueMemberName(string memberName)
        {
            string uniqueName = memberName;
            int i = 0;
            while (this.Contains(uniqueName))
            {
                uniqueName = memberName + (++i).ToString(CultureInfo.InvariantCulture);
            }
            return uniqueName;
        }
    }
}
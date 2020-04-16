// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Threading;

namespace System.IdentityModel
{
    internal class SecurityUniqueId
    {
        private static long s_nextId = 0;
        private static string s_commonPrefix = "uuid-" + Guid.NewGuid().ToString() + "-";

        private long _id;
        private string _prefix;
        private string _val;

        private SecurityUniqueId(string prefix, long id)
        {
            _id = id;
            _prefix = prefix;
            _val = null;
        }

        public static SecurityUniqueId Create()
        {
            return SecurityUniqueId.Create(s_commonPrefix);
        }

        public static SecurityUniqueId Create(string prefix)
        {
            return new SecurityUniqueId(prefix, Interlocked.Increment(ref s_nextId));
        }

        public string Value
        {
            get
            {
                if (_val == null)
                    _val = _prefix + _id.ToString(CultureInfo.InvariantCulture);

                return _val;
            }
        }
    }
}

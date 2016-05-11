// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;

namespace Bridge
{
    public class resource
    {
        public string name { get; set; }

        public override string ToString()
        {
            return string.Format(@"{{ name : ""{0}"" }}", name);
        }
    }

    public class resourceResponse
    {
        public Guid id { get; set; }
        public string details { get; set; }

        public override string ToString()
        {
            return string.Format(@"{{
    id : ""{0}""
    details : ""{1}""
}}",
                id,
                details);
        }
    }
}

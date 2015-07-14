// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

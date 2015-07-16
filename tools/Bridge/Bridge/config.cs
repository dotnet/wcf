// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Bridge
{
    public class config
    {
        public string resourcesDirectory { get; set; }

        public override string ToString()
        {
            return string.Format(@"{{ resourcesDirectory : ""{0}"" }}", resourcesDirectory);
        }
    }

    public class configResponse
    {
        public IEnumerable<string> types { get; set; }

        public override string ToString()
        {
            return string.Format(@"{{
    types : [
        ""{0}""
    ]
}}",
                string.Join("\",\n        \"", types));
        }
    }
}

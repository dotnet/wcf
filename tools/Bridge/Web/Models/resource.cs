// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json.Linq;

namespace Web.Models.Data
{
    public class resource
    {
        public string name { get; set; }

        public JObject parameters { get; set; }
    }

    public class resourceResponse
    {
        public string id { get; set; }
    }
}

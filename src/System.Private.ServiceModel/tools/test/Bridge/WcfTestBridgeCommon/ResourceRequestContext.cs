// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfTestBridgeCommon
{
    [Serializable]
    public class ResourceRequestContext
    {
        public BridgeConfiguration BridgeConfiguration { get; set; }
        public string ResourceName { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}

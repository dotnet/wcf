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
    }
}

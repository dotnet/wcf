using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfTestBridgeCommon
{
    [Serializable]
    public class ResourceResponse
    {
        public ResourceResponse()
        {
            Properties = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Properties { get; set; }

        // RawResponse takes precedence over Properties if it exists
        public byte[] RawResponse { get; set; }
    }
}

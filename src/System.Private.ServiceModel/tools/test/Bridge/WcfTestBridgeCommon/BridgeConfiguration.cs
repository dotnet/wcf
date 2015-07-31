using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WcfTestBridgeCommon
{
    [Serializable]
    public class BridgeConfiguration
    {
        // These property names must match the names used in TestProperties because
        // that is the set of name/value pairs from which this type is created.
        private const string BridgeResourceFolder_PropertyName = "BridgeResourceFolder";
        private const string BridgeUrl_PropertyName = "BridgeUrl";
        private const string BridgeMaxIdleTimeSpan_PropertyName = "BridgeMaxIdleTimeSpan";
        private const string UseFiddlerUrl_PropertyName = "UseFiddlerUrl";

        public string BridgeResourceFolder { get; set; }
        public string BridgeUrl { get; set; }
        public TimeSpan BridgeMaxIdleTimeSpan { get; set; }
        public bool UseFiddlerUrl { get; set; }

        public BridgeConfiguration()
        {
        }

        // This ctor accepts an existing BridgeConfiguration and a set of name/value pairs.
        // It will create a new BridgeConfiguration instance that is a clone of the existing
        // one and will overwrite any properties with corresponding entries found in the name/value pairs.
        public BridgeConfiguration(BridgeConfiguration configuration, Dictionary<string, string> properties)
        {
            BridgeResourceFolder = configuration.BridgeResourceFolder;
            BridgeUrl = configuration.BridgeUrl;
            BridgeMaxIdleTimeSpan = configuration.BridgeMaxIdleTimeSpan;
            UseFiddlerUrl = configuration.UseFiddlerUrl;

            string propertyValue = null;
            if (properties.TryGetValue(BridgeResourceFolder_PropertyName, out propertyValue))
            {
                if (string.IsNullOrEmpty(propertyValue) || !Directory.Exists(propertyValue))
                {
                    throw new ArgumentException(
                        String.Format("The BridgeResourceFolder '{0}' does not exist.", propertyValue),
                        BridgeResourceFolder_PropertyName);
                }

                BridgeResourceFolder = Path.GetFullPath(propertyValue);
            }

            if (properties.TryGetValue(BridgeUrl_PropertyName, out propertyValue))
            {
                BridgeUrl = propertyValue;
            }

            if (properties.TryGetValue(BridgeMaxIdleTimeSpan_PropertyName, out propertyValue))
            {
                TimeSpan span;
                if (!TimeSpan.TryParse(propertyValue, out span))
                {
                    throw new ArgumentException(
                        String.Format("The BridgeMaxIdleTimeSpan value '{0}' is not a valid TimeSpan.", propertyValue),
                        BridgeMaxIdleTimeSpan_PropertyName);
                }

                BridgeMaxIdleTimeSpan = span;
            }

            if (properties.TryGetValue(UseFiddlerUrl_PropertyName, out propertyValue))
            {
                bool boolValue = false;
                if (!bool.TryParse(propertyValue, out boolValue))
                {
                    throw new ArgumentException(
                        String.Format("The UseFiddlerUrl value '{0}' is not a valid boolean.", propertyValue),
                        UseFiddlerUrl_PropertyName);
                }

                UseFiddlerUrl = boolValue;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0} : '{1}'", BridgeResourceFolder_PropertyName, BridgeResourceFolder));
            sb.AppendLine(String.Format("{0} : '{1}'", BridgeUrl_PropertyName, BridgeUrl));
            sb.AppendLine(String.Format("{0} : '{1}'", BridgeMaxIdleTimeSpan_PropertyName, BridgeMaxIdleTimeSpan));
            sb.AppendLine(String.Format("{0} : '{1}'", UseFiddlerUrl_PropertyName, UseFiddlerUrl));
            return sb.ToString();
        }
    }
}

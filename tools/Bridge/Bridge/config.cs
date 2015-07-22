// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bridge
{
    public class config
    {
        // These are the property names that must match TestProperties.
        // We cannot take a dependency on TestProperties from this full .NET Framework project.
        private const string BridgeResourceFolder_PropertyName = "BridgeResourceFolder";
        private const string BridgeUrl_PropertyName = "BridgeUrl";
        private const string UseFiddlerUrl_PropertyName = "UseFiddlerUrl";

        private IDictionary<string, string> _properties = new Dictionary<string, string>();

        public config()
        {
        }

        public config(IDictionary<string, string> properties)
        {
            foreach (var pair in properties)
            {
                Properties[pair.Key] = pair.Value == null ? String.Empty : pair.Value;
            }
        }
        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }
        public string BridgeResourceFolder
        {
            get
            {
                string value = String.Empty;
                Properties.TryGetValue(BridgeResourceFolder_PropertyName, out value);
                return value;
            }
        }
        public string BridgeUrl
        {
            get
            {
                string value = String.Empty;
                Properties.TryGetValue(BridgeUrl_PropertyName, out value);
                return value;
            }
        }

        public bool UseFiddlerUrl
        {
            get
            {
                string value = String.Empty;
                Properties.TryGetValue(UseFiddlerUrl_PropertyName, out value);
                return value == null ? false : String.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        public override string ToString()
        {
            IEnumerable<string> pairs = Properties.Select<KeyValuePair<string, string>, string>(p => string.Format("    {0} : {1}", p.Key, p.Value));
            return string.Join(Environment.NewLine, pairs);
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

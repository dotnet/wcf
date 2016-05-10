// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge
{
    internal class JsonSerializer
    {
        internal static readonly string JsonMediaType = "application/json";


        internal static string SerializeDictionary(IDictionary<String, string> dictionary)
        {
            return Serialize(dictionary);
        }

        internal static string Serialize(IEnumerable<KeyValuePair<string, string>> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (var kv in items)
            {
                sb.AppendFormat("   {0} : \"{1}\",\n", kv.Key, kv.Value == null ? String.Empty : kv.Value.ToString());
            }

            sb.Remove(sb.Length - 2, 2);
            sb.AppendLine("\n}");
            return sb.ToString();
        }

        internal static Dictionary<string, string> DeserializeDictionary(string data)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            data = data.Replace("{", String.Empty)
                    .Replace("}", String.Empty)
                    .Trim();

            string[] pairs = data.Split(',');
            foreach (string pair in pairs)
            {
                int colonPos = pair.IndexOf(':');
                if (colonPos > 0)
                {
                    string key = pair.Substring(0, colonPos - 1).Replace("\"", String.Empty).Trim();
                    string value = pair.Substring(colonPos + 1).Replace("\"", String.Empty).Trim();
                    dictionary[key] = value;
                }
            }

            return dictionary;
        }
    }
}

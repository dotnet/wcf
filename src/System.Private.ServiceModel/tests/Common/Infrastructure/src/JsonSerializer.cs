// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common
{
    internal class JsonSerializer
    {
        internal static readonly string JsonMediaType = "application/json";

        internal static string SerializeDictionary(IDictionary dictionary)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (var key in dictionary.Keys)
            {
                sb.AppendFormat("   {0} : \"{1}\",\n", key, dictionary[key] == null ? String.Empty : dictionary[key].ToString());
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

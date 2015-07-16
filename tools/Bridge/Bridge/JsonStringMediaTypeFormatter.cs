// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Bridge
{
    class JsonStringMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        private static readonly Regex regexResource = new Regex(@"name\s*:\s*""([a-z_][a-z0-9._]*)""", RegexOptions.IgnoreCase);
        private static readonly Regex regexConfig = new Regex(@"resourcesDirectory\s*:\s*""([^""]+)""", RegexOptions.IgnoreCase);

        public JsonStringMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string) || type == typeof(resource) || type == typeof(config);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string) || type == typeof(resource) || type == typeof(resourceResponse) || type == typeof(config) || type == typeof(configResponse) || type.IsAssignableFrom(typeof(IDictionary));
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content, CancellationToken cancellationToken)
        {
            using (var writer = new StreamWriter(writeStream))
            {

                if (type.IsAssignableFrom(typeof(IDictionary)))
                {
                    IDictionary dict = value as IDictionary;
                    if (dict != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("{");
                        foreach (var key in dict.Keys)
                        {
                            sb.AppendFormat("   {0} : \"{1}\",\n", key, dict[key]);
                        }

                        sb.Remove(sb.Length - 2, 2);
                        sb.AppendLine("\n}");
                        writer.Write(sb.ToString());
                    }
                }
                else
                {
                    writer.WriteLine(value.ToString());

                }
            }
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken cancellationToken)
        {
            string data;
            using (var reader = new StreamReader(readStream))
            {
                data = reader.ReadToEnd();
            }

            if (type == typeof(resource))
            {
                var match = regexResource.Match(data);
                if (match.Success && match.Groups.Count == 2)
                    return new resource() { name = match.Groups[1].Value };
            }
            else if (type == typeof(config))
            {
                var match = regexConfig.Match(data);
                if (match.Success && match.Groups.Count == 2)
                    return new config() { resourcesDirectory = match.Groups[1].Value };
            }

            return data;
        }
    }
}

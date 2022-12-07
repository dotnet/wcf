// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class FrameworkInfo
    {
        // https://github.com/dotnet/corefx/blob/master/Documentation/architecture/net-platform-standard.md
        public const string Netstandard = "netstandard";
        public const string Netcoreapp = "netcoreapp";
        public const string Netfx = "net";
        public const string Netversion = "version";

        private FrameworkInfo()
        {
        }

        public string FullName { get; private set; }
        public string Name { get; private set; }
        public Version Version { get; private set; }
        public bool IsDnx { get; private set; }
        public bool IsKnownDnx { get; private set; }

        public static FrameworkInfo Parse(string fullFrameworkName)
        {
            string name = null;
            Version version = null;

            if (fullFrameworkName == null)
            {
                throw new ArgumentNullException(nameof(fullFrameworkName));
            }

            // framework spec form: 'netcore1.5' or 'net452'
            // framework spec form: 'net5.0'
            // framework spec form: '.NETCoreApp,Version=v6.0'
            for (int i = 0; i < fullFrameworkName.Length; i++)
            {
                char c = fullFrameworkName[i];
                if (char.IsNumber(c))
                {
                    name = fullFrameworkName.Substring(0, i);

                    // Version ctr requires at least Major and Minor parts
                    string versionString = fullFrameworkName.Substring(i);
                    if ((name == Netfx) && !versionString.Contains("."))
                    {
                        // net452
                        StringBuilder sb = new StringBuilder(versionString);
                        for (int j = 1; j < sb.Length; j += 2)
                        {
                            sb.Insert(j, '.');
                        }
                        versionString = sb.ToString();
                    }

                    version = new Version(versionString);
                    break;
                }
            }

            if (name.ToLower().Contains(Netversion))
            {
                if (version.Major >= 5)
                {
                    name = Netfx;
                }
                else
                {
                    name = Netcoreapp;
                }

                fullFrameworkName = string.Concat(name, version.ToString());
            }

            if (version == null || name == null)
            {
                throw new FormatException(fullFrameworkName);
            }

            var fxInfo = new FrameworkInfo();

            fxInfo.FullName = fullFrameworkName;
            fxInfo.Name = name;
            fxInfo.Version = version;
            fxInfo.IsDnx = name == Netstandard || name == Netcoreapp || version.Major >= 5;
            fxInfo.IsKnownDnx = fxInfo.IsDnx &&
                        (TargetFrameworkHelper.NetStandardToNetCoreVersionMap.Keys.Any((netstdVersion) => netstdVersion == version) ||
                         TargetFrameworkHelper.NetStandardToNetCoreVersionMap.Values.Any((netcoreVersion) => netcoreVersion == version));

            return fxInfo;
        }

        internal static bool TryParse(string fullFrameworkName, out FrameworkInfo frameworkInfo)
        {
            frameworkInfo = null;
            try
            {
                frameworkInfo = Parse(fullFrameworkName);
            }
            catch
            {
            }
            return frameworkInfo != null;
        }

        public override string ToString()
        {
            return this.FullName;
        }
    }
}


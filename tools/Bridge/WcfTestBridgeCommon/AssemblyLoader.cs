// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WcfTestBridgeCommon
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private static Dictionary<string, Type> s_types = new Dictionary<string, Type>();

        public List<string> GetTypes()
        {
            return s_types.Keys.ToList();
        }

        public void LoadAssemblies()
        {
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                try
                {
                    if (string.Equals(Path.GetFileName(file), "WcfTestBridgeCommon.dll", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var assembly = Assembly.LoadFile(file);
                    foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && t.FindInterfaces(new TypeFilter(IResourceFilter), null).Length > 0))
                    {
                        s_types.Add(type.FullName, type);
                    }
                }
                catch { }
            }
        }

        public object IResourceCall(string typeName, string verb)
        {
            Type type = null;
            if (!s_types.TryGetValue(typeName, out type))
            {
                throw new ArgumentException("Type " + typeName + " does not exist");
            }

            var resource = Activator.CreateInstance(type);
            return resource.GetType().GetMethod(verb).Invoke(resource, null);
        }

        private bool IResourceFilter(Type t, object o)
        {
            return t.FullName == "WcfTestBridgeCommon.IResource";
        }
    }
}

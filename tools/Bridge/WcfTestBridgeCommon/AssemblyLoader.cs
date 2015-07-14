using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WcfTestBridgeCommon
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private const string TypeList = "TypeList";

        public List<string> GetTypes()
        {
            return (AppDomain.CurrentDomain.GetData(TypeList) as Dictionary<string, Type>).Keys.ToList();
        }

        public void LoadAssemblies()
        {
            var types = new Dictionary<string, Type>();
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
                        types.Add(type.FullName, type);
                    }
                }
                catch { }
            }

            AppDomain.CurrentDomain.SetData(TypeList, types);
        }

        public object IResourceCall(string typeName, string verb)
        {
            var types = AppDomain.CurrentDomain.GetData(TypeList) as Dictionary<string, Type>;
            Type type = null;
            if (!types.TryGetValue(typeName, out type))
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

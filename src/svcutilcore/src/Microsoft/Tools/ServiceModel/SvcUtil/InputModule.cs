namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Net;
    using System.Threading;
    using System.Runtime.InteropServices;

    partial class InputModule
    {
        static public Type[] LoadTypes(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                //ToolConsole.WriteWarning(SR.GetString(SR.WrnCouldNotLoadTypesFromReferenceAssemblyAt, assembly.Location));
                foreach (Exception e in rtle.LoaderExceptions)
                {
                    //ToolConsole.WriteLine("  " + e.Message, 2);
                }

                types = Array.FindAll<Type>(rtle.Types, delegate (Type t) { return t != null; });
                if (types.Length == 0)
                {
                    //throw new ToolInputException(SR.GetString(SR.ErrCouldNotLoadTypesFromAssemblyAt, assembly.Location));
                }

            }
            return types;
        }
    }
}
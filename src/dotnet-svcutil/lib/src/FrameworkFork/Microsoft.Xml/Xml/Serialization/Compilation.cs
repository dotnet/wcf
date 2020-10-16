// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Xml.Serialization
{
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Collections;
    using System.IO;
    using System;
    using System.Text;
    using Microsoft.Xml;
    using System.Threading;
    using System.Security;

    using System.Diagnostics;
    using Microsoft.CodeDom.Compiler;
    using System.Globalization;
    using System.Runtime.Versioning;

    internal class TempAssembly
    {
        internal const string GeneratedAssemblyNamespace = "System.Xml.Serialization.GeneratedAssembly";
        private Assembly _assembly;
        private bool _pregeneratedAssmbly = false;
        private XmlSerializerImplementation _contract = null;
        private Hashtable _writerMethods;
        private Hashtable _readerMethods;
        private TempMethodDictionary _methods;
        private static object[] s_emptyObjectArray = new object[0];
        private Hashtable _assemblies = new Hashtable();

        internal class TempMethod
        {
            internal MethodInfo writeMethod;
            internal MethodInfo readMethod;
            internal string name;
            internal string ns;
            internal bool isSoap;
            internal string methodKey;
        }

        private TempAssembly()
        {
        }

        internal TempAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, string location)
        {
            throw new NotImplementedException();
        }

        internal TempAssembly(XmlMapping[] xmlMappings, Assembly assembly, XmlSerializerImplementation contract)
        {
            _assembly = assembly;
            InitAssemblyMethods(xmlMappings);
            _contract = contract;
            _pregeneratedAssmbly = true;
        }
        internal TempAssembly(XmlSerializerImplementation contract)
        {
            _contract = contract;
            _pregeneratedAssmbly = true;
        }

        internal XmlSerializerImplementation Contract
        {
            get
            {
                if (_contract == null)
                {
                    _contract = (XmlSerializerImplementation)Activator.CreateInstance(GetTypeFromAssembly(_assembly, "XmlSerializerContract"));
                }
                return _contract;
            }
        }

        internal void InitAssemblyMethods(XmlMapping[] xmlMappings)
        {
            _methods = new TempMethodDictionary();
            for (int i = 0; i < xmlMappings.Length; i++)
            {
                TempMethod method = new TempMethod();
                method.isSoap = xmlMappings[i].IsSoap;
                method.methodKey = xmlMappings[i].Key;
                XmlTypeMapping xmlTypeMapping = xmlMappings[i] as XmlTypeMapping;
                if (xmlTypeMapping != null)
                {
                    method.name = xmlTypeMapping.ElementName;
                    method.ns = xmlTypeMapping.Namespace;
                }
                _methods.Add(xmlMappings[i].Key, method);
            }
        }

        internal static Assembly LoadGeneratedAssembly(Type type, string defaultNamespace, out XmlSerializerImplementation contract)
        {
            throw new NotImplementedException();
        }

        // SxS: This method does not take any resource name and does not expose any resources to the caller.
        // It's OK to suppress the SxS warning.
        // // [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
        // [ResourceExposure(ResourceScope.None)]
        private static MethodInfo GetMethodFromType(Type type, string methodName, Assembly assembly)
        {
            MethodInfo method = type.GetMethod(methodName);
            if (method != null)
                return method;

            MissingMethodException missingMethod = new MissingMethodException(string.Format("{0}:{1}", type.FullName, methodName));
            if (assembly != null)
            {
                throw new InvalidOperationException(string.Format(ResXml.XmlSerializerExpired, assembly.FullName, /*assembly.CodeBase*/ null), missingMethod);
            }
            throw missingMethod;
        }

        internal static Type GetTypeFromAssembly(Assembly assembly, string typeName)
        {
            typeName = GeneratedAssemblyNamespace + "." + typeName;
            Type type = assembly.GetType(typeName);
            if (type == null) throw new InvalidOperationException(string.Format(ResXml.XmlMissingType, typeName, assembly.FullName));
            return type;
        }

        internal bool CanRead(XmlMapping mapping, XmlReader xmlReader)
        {
            if (mapping == null)
                return false;

            if (mapping.Accessor.Any)
            {
                return true;
            }
            TempMethod method = _methods[mapping.Key];
            return xmlReader.IsStartElement(method.name, method.ns);
        }

        private string ValidateEncodingStyle(string encodingStyle, string methodKey)
        {
            if (encodingStyle != null && encodingStyle.Length > 0)
            {
                if (_methods[methodKey].isSoap)
                {
                    if (encodingStyle != Soap.Encoding && encodingStyle != Soap12.Encoding)
                    {
                        throw new InvalidOperationException(string.Format(ResXml.XmlInvalidEncoding3, encodingStyle, Soap.Encoding, Soap12.Encoding));
                    }
                }
                else
                {
                    throw new InvalidOperationException(string.Format(ResXml.XmlInvalidEncodingNotEncoded1, encodingStyle));
                }
            }
            else
            {
                if (_methods[methodKey].isSoap)
                {
                    encodingStyle = Soap.Encoding;
                }
            }
            return encodingStyle;
        }

        internal object InvokeReader(XmlMapping mapping, XmlReader xmlReader, XmlDeserializationEvents events, string encodingStyle)
        {
            XmlSerializationReader reader = null;
            try
            {
                encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
                reader = Contract.Reader;
                reader.Init(xmlReader, events, encodingStyle, this);
                if (_methods[mapping.Key].readMethod == null)
                {
                    if (_readerMethods == null)
                    {
                        _readerMethods = Contract.ReadMethods;
                    }
                    string methodName = (string)_readerMethods[mapping.Key];
                    if (methodName == null)
                    {
                        throw new InvalidOperationException(string.Format(ResXml.XmlNotSerializable, mapping.Accessor.Name));
                    }
                    _methods[mapping.Key].readMethod = GetMethodFromType(reader.GetType(), methodName, _pregeneratedAssmbly ? _assembly : null);
                }
                return _methods[mapping.Key].readMethod.Invoke(reader, s_emptyObjectArray);
            }
            catch (SecurityException e)
            {
                throw new InvalidOperationException(ResXml.XmlNoPartialTrust, e);
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }

        internal void InvokeWriter(XmlMapping mapping, XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
        {
            XmlSerializationWriter writer = null;
            try
            {
                encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
                writer = Contract.Writer;
                writer.Init(xmlWriter, namespaces, encodingStyle, id, this);
                if (_methods[mapping.Key].writeMethod == null)
                {
                    if (_writerMethods == null)
                    {
                        _writerMethods = Contract.WriteMethods;
                    }
                    string methodName = (string)_writerMethods[mapping.Key];
                    if (methodName == null)
                    {
                        throw new InvalidOperationException(string.Format(ResXml.XmlNotSerializable, mapping.Accessor.Name));
                    }
                    _methods[mapping.Key].writeMethod = GetMethodFromType(writer.GetType(), methodName, _pregeneratedAssmbly ? _assembly : null);
                }
                _methods[mapping.Key].writeMethod.Invoke(writer, new object[] { o });
            }
            catch (SecurityException e)
            {
                throw new InvalidOperationException(ResXml.XmlNoPartialTrust, e);
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        internal Assembly GetReferencedAssembly(string name)
        {
            return _assemblies != null && name != null ? (Assembly)_assemblies[name] : null;
        }

        internal bool NeedAssembyResolve
        {
            get { return _assemblies != null && _assemblies.Count > 0; }
        }

        internal sealed class TempMethodDictionary : DictionaryBase
        {
            internal TempMethod this[string key]
            {
                get
                {
                    return (TempMethod)Dictionary[key];
                }
            }
            internal void Add(string key, TempMethod value)
            {
                Dictionary.Add(key, value);
            }
        }
    }

    internal sealed class XmlSerializerCompilerParameters
    {
    }


    internal class TempAssemblyCacheKey
    {
        private string _ns;
        private object _type;

        internal TempAssemblyCacheKey(string ns, object type)
        {
            _type = type;
            _ns = ns;
        }

        public override bool Equals(object o)
        {
            TempAssemblyCacheKey key = o as TempAssemblyCacheKey;
            if (key == null) return false;
            return (key._type == _type && key._ns == _ns);
        }

        public override int GetHashCode()
        {
            return ((_ns != null ? _ns.GetHashCode() : 0) ^ (_type != null ? _type.GetHashCode() : 0));
        }
    }

    internal class TempAssemblyCache
    {
        private Hashtable _cache = new Hashtable();

        internal TempAssembly this[string ns, object o]
        {
            get { return (TempAssembly)_cache[new TempAssemblyCacheKey(ns, o)]; }
        }

        internal void Add(string ns, object o, TempAssembly assembly)
        {
            TempAssemblyCacheKey key = new TempAssemblyCacheKey(ns, o);
            lock (this)
            {
                if (_cache[key] == assembly) return;
                Hashtable clone = new Hashtable();
                foreach (object k in _cache.Keys)
                {
                    clone.Add(k, _cache[k]);
                }
                _cache = clone;
                _cache[key] = assembly;
            }
        }
    }
}

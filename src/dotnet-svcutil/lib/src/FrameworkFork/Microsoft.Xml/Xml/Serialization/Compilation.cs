// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization {

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

    internal class TempAssembly {
        internal const string GeneratedAssemblyNamespace = "System.Xml.Serialization.GeneratedAssembly";
        Assembly assembly;
        bool pregeneratedAssmbly = false;
        XmlSerializerImplementation contract = null;
        Hashtable writerMethods;
        Hashtable readerMethods;
        TempMethodDictionary methods;
        static object[] emptyObjectArray = new object[0];
        Hashtable assemblies = new Hashtable();

        internal class TempMethod {
            internal MethodInfo writeMethod;
            internal MethodInfo readMethod;
            internal string name;
            internal string ns;
            internal bool isSoap;
            internal string methodKey;
        }

        private TempAssembly() {
        }

        internal TempAssembly(XmlMapping[] xmlMappings, Type[] types, string defaultNamespace, string location) {
            throw new NotImplementedException();
        }

        internal TempAssembly(XmlMapping[] xmlMappings, Assembly assembly, XmlSerializerImplementation contract) {
            this.assembly = assembly;
            InitAssemblyMethods(xmlMappings);
            this.contract = contract;
            pregeneratedAssmbly = true;
        }
        internal TempAssembly(XmlSerializerImplementation contract) {
            this.contract = contract;
            pregeneratedAssmbly = true;
        }

        internal XmlSerializerImplementation Contract {
            get {
                if (contract == null) {
                    contract = (XmlSerializerImplementation)Activator.CreateInstance(GetTypeFromAssembly(this.assembly, "XmlSerializerContract"));
                }
                return contract;
            }
        }

        internal void InitAssemblyMethods(XmlMapping[] xmlMappings) {
            methods = new TempMethodDictionary();
            for (int i = 0; i < xmlMappings.Length; i++) {
                TempMethod method = new TempMethod();
                method.isSoap = xmlMappings[i].IsSoap;
                method.methodKey = xmlMappings[i].Key;
                XmlTypeMapping xmlTypeMapping = xmlMappings[i] as XmlTypeMapping;
                if (xmlTypeMapping != null) {
                    method.name = xmlTypeMapping.ElementName;
                    method.ns = xmlTypeMapping.Namespace;
                }
                methods.Add(xmlMappings[i].Key, method);
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
        static MethodInfo GetMethodFromType(Type type, string methodName, Assembly assembly) {
            MethodInfo method = type.GetMethod(methodName);
            if (method != null)
                return method;

            MissingMethodException missingMethod = new MissingMethodException(string.Format("{0}:{1}", type.FullName, methodName));
            if (assembly != null) {
                throw new InvalidOperationException(ResXml.GetString(ResXml.XmlSerializerExpired, assembly.FullName, /*assembly.CodeBase*/ null), missingMethod);
            }
            throw missingMethod;
        }

        internal static Type GetTypeFromAssembly(Assembly assembly, string typeName) {
            typeName = GeneratedAssemblyNamespace + "." + typeName;
            Type type = assembly.GetType(typeName);
            if (type == null) throw new InvalidOperationException(ResXml.GetString(ResXml.XmlMissingType, typeName, assembly.FullName));
            return type;
        }

        internal bool CanRead(XmlMapping mapping, XmlReader xmlReader) {
            if (mapping == null)
                return false;

            if (mapping.Accessor.Any) {
                return true;
            }
            TempMethod method = methods[mapping.Key];
            return xmlReader.IsStartElement(method.name, method.ns);
        }

        string ValidateEncodingStyle(string encodingStyle, string methodKey) {
            if (encodingStyle != null && encodingStyle.Length > 0) {
                if (methods[methodKey].isSoap) {
                    if (encodingStyle != Soap.Encoding && encodingStyle != Soap12.Encoding) {
                        throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidEncoding3, encodingStyle, Soap.Encoding, Soap12.Encoding));
                    }
                }
                else {
                    throw new InvalidOperationException(ResXml.GetString(ResXml.XmlInvalidEncodingNotEncoded1, encodingStyle));
                }
            }
            else {
                if (methods[methodKey].isSoap) {
                    encodingStyle = Soap.Encoding;
                }
            }
            return encodingStyle;
        }

        internal object InvokeReader(XmlMapping mapping, XmlReader xmlReader, XmlDeserializationEvents events, string encodingStyle) {
            XmlSerializationReader reader = null;
            try {
                encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
                reader = Contract.Reader;
                reader.Init(xmlReader, events, encodingStyle, this);
                if (methods[mapping.Key].readMethod == null) {
                    if (readerMethods == null) {
                        readerMethods = Contract.ReadMethods;
                    }
                    string methodName = (string)readerMethods[mapping.Key];
                    if (methodName == null) {
                        throw new InvalidOperationException(ResXml.GetString(ResXml.XmlNotSerializable, mapping.Accessor.Name));
                    }
                    methods[mapping.Key].readMethod = GetMethodFromType(reader.GetType(), methodName, pregeneratedAssmbly ? this.assembly : null);
                }
                return methods[mapping.Key].readMethod.Invoke(reader, emptyObjectArray);
            }
            catch (SecurityException e) {
                throw new InvalidOperationException(ResXml.GetString(ResXml.XmlNoPartialTrust), e);
            }
            finally {
                if (reader != null)
                    reader.Dispose();
            }
        }

        internal void InvokeWriter(XmlMapping mapping, XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id) {
            XmlSerializationWriter writer = null;
            try {
                encodingStyle = ValidateEncodingStyle(encodingStyle, mapping.Key);
                writer = Contract.Writer;
                writer.Init(xmlWriter, namespaces, encodingStyle, id, this);
                if (methods[mapping.Key].writeMethod == null) {
                    if (writerMethods == null) {
                        writerMethods = Contract.WriteMethods;
                    }
                    string methodName = (string)writerMethods[mapping.Key];
                    if (methodName == null) {
                        throw new InvalidOperationException(ResXml.GetString(ResXml.XmlNotSerializable, mapping.Accessor.Name));
                    }
                    methods[mapping.Key].writeMethod = GetMethodFromType(writer.GetType(), methodName, pregeneratedAssmbly ? assembly : null);
                }
                methods[mapping.Key].writeMethod.Invoke(writer, new object[] { o });
            }
            catch (SecurityException e) {
                throw new InvalidOperationException(ResXml.GetString(ResXml.XmlNoPartialTrust), e);
            }
            finally {
                if (writer != null)
                    writer.Dispose();
            }
        }

        internal Assembly GetReferencedAssembly(string name) {
            return assemblies != null && name != null ? (Assembly)assemblies[name] : null;
        }

        internal bool NeedAssembyResolve {
            get { return assemblies != null && assemblies.Count > 0; }
        }

        internal sealed class TempMethodDictionary : DictionaryBase {
            internal TempMethod this[string key] {
                get {
                    return (TempMethod)Dictionary[key];
                }
            }
            internal void Add(string key, TempMethod value) {
                Dictionary.Add(key, value);
            }
        }
    }

    sealed class XmlSerializerCompilerParameters {

    }


    class TempAssemblyCacheKey {
        string ns;
        object type;

        internal TempAssemblyCacheKey(string ns, object type) {
            this.type = type;
            this.ns = ns;
        }

        public override bool Equals(object o) {
            TempAssemblyCacheKey key = o as TempAssemblyCacheKey;
            if (key == null) return false;
            return (key.type == this.type && key.ns == this.ns);
        }

        public override int GetHashCode() {
            return ((ns != null ? ns.GetHashCode() : 0) ^ (type != null ? type.GetHashCode() : 0));
        }
    }

    internal class TempAssemblyCache {
        Hashtable cache = new Hashtable();

        internal TempAssembly this[string ns, object o] {
            get { return (TempAssembly)cache[new TempAssemblyCacheKey(ns, o)]; }
        }

        internal void Add(string ns, object o, TempAssembly assembly) {
            TempAssemblyCacheKey key = new TempAssemblyCacheKey(ns, o);
            lock (this) {
                if (cache[key] == assembly) return;
                Hashtable clone = new Hashtable();
                foreach (object k in cache.Keys) {
                    clone.Add(k, cache[k]);
                }
                cache = clone;
                cache[key] = assembly;
            }
        }
    }
}

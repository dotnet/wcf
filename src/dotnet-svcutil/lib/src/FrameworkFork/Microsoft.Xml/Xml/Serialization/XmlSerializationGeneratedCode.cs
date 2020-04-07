// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Serialization {
    using System;
    using System.IO;
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;
    using System.Reflection;
    using System.Security;
    using System.Globalization;
   
    /// <include file='doc\XmlSerializationGeneratedCode.uex' path='docs/doc[@for="XmlSerializationGeneratedCode"]/*' />
    ///<internalonly/>
    public abstract class XmlSerializationGeneratedCode {
        TempAssembly tempAssembly;
        int threadCode;
        //ResolveEventHandler assemblyResolver;

        internal void Init(TempAssembly tempAssembly) {
            this.tempAssembly = tempAssembly;
        }

        // this method must be called at the end of serialization
        internal void Dispose() {

        }
    }

    internal class XmlSerializationCodeGen {
        IndentedWriter writer;
        int nextMethodNumber = 0;
        Hashtable methodNames = new Hashtable();
        ReflectionAwareCodeGen raCodeGen;
        TypeScope[] scopes;
        TypeDesc stringTypeDesc = null;
        TypeDesc qnameTypeDesc = null;
        string access;
        string className;
        TypeMapping[] referencedMethods;
        int references = 0;
        Hashtable generatedMethods = new Hashtable();

        internal XmlSerializationCodeGen(IndentedWriter writer, TypeScope[] scopes, string access, string className) {
            this.writer = writer;
            this.scopes = scopes;
            if (scopes.Length > 0) {
                stringTypeDesc = scopes[0].GetTypeDesc(typeof(string));
                qnameTypeDesc = scopes[0].GetTypeDesc(typeof(XmlQualifiedName));
            }
            this.raCodeGen = new ReflectionAwareCodeGen(writer);
            this.className = className;
            this.access = access;
        }

        internal IndentedWriter Writer { get { return writer; } }
        internal int NextMethodNumber { get { return nextMethodNumber; } set { nextMethodNumber = value; } }
        internal ReflectionAwareCodeGen RaCodeGen { get { return raCodeGen; } }
        internal TypeDesc StringTypeDesc { get { return stringTypeDesc; } }
        internal TypeDesc QnameTypeDesc { get { return qnameTypeDesc; } }
        internal string ClassName { get { return className; } }
        internal string Access { get { return access; } }
        internal TypeScope[] Scopes { get { return scopes; } }
        internal Hashtable MethodNames { get { return methodNames; } }
        internal Hashtable GeneratedMethods { get { return generatedMethods; } }

        internal virtual void GenerateMethod(TypeMapping mapping){}

        internal void GenerateReferencedMethods() {
            while(references > 0) {
                TypeMapping mapping = referencedMethods[--references];
                GenerateMethod(mapping);
            }
        }

        internal string ReferenceMapping(TypeMapping mapping) {
            if (!mapping.IsSoap) {
                if (generatedMethods[mapping] == null) {
                    referencedMethods = EnsureArrayIndex(referencedMethods, references);
                    referencedMethods[references++] = mapping;
                }
            }
            return (string)methodNames[mapping];
        }

        TypeMapping[] EnsureArrayIndex(TypeMapping[] a, int index) {
            if (a == null) return new TypeMapping[32];
            if (index < a.Length) return a;
            TypeMapping[] b = new TypeMapping[a.Length + 32];
            Array.Copy(a, b, index);
            return b;
        }

        internal void WriteQuotedCSharpString(string value) {
            raCodeGen.WriteQuotedCSharpString(value);
        }

        internal void GenerateHashtableGetBegin(string privateName, string publicName) {
            writer.Write(typeof(Hashtable).FullName);
            writer.Write(" ");
            writer.Write(privateName);
            writer.WriteLine(" = null;");
            writer.Write("public override ");
            writer.Write(typeof(Hashtable).FullName);

            writer.Write(" ");
            writer.Write(publicName);
            writer.WriteLine(" {");
            writer.Indent++;

            writer.WriteLine("get {");
            writer.Indent++;

            writer.Write("if (");
            writer.Write(privateName);
            writer.WriteLine(" == null) {");
            writer.Indent++;

            writer.Write(typeof(Hashtable).FullName);
            writer.Write(" _tmp = new ");
            writer.Write(typeof(Hashtable).FullName);
            writer.WriteLine("();");

        }

        internal void GenerateHashtableGetEnd(string privateName) {
            writer.Write("if (");
            writer.Write(privateName);
            writer.Write(" == null) ");
            writer.Write(privateName);
            writer.WriteLine(" = _tmp;");
            writer.Indent--;
            writer.WriteLine("}");

            writer.Write("return ");
            writer.Write(privateName);
            writer.WriteLine(";");
            writer.Indent--;
            writer.WriteLine("}");

            writer.Indent--;
            writer.WriteLine("}");
        }
        internal void GeneratePublicMethods(string privateName, string publicName, string[] methods, XmlMapping[] xmlMappings) {
            GenerateHashtableGetBegin(privateName, publicName);
            if (methods != null && methods.Length != 0 && xmlMappings != null && xmlMappings.Length == methods.Length) {
                for (int i = 0; i < methods.Length; i++) {
                    if (methods[i] == null)
                        continue;
                    writer.Write("_tmp[");
                    WriteQuotedCSharpString(xmlMappings[i].Key);
                    writer.Write("] = ");
                    WriteQuotedCSharpString(methods[i]);
                    writer.WriteLine(";");
                }
            }
            GenerateHashtableGetEnd(privateName);
        }

        internal void GenerateSupportedTypes(Type[] types) {
            writer.Write("public override ");
            writer.Write(typeof(bool).FullName);
            writer.Write(" CanSerialize(");
            writer.Write(typeof(Type).FullName);
            writer.WriteLine(" type) {");
            writer.Indent++;
            Hashtable uniqueTypes = new Hashtable();
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                if (type == null)
                    continue;
                TypeInfo info = type.GetTypeInfo();
                if (!info.IsPublic && !info.IsNestedPublic)
                    continue;
                if (uniqueTypes[type] != null)
                    continue;
                if (DynamicAssemblies.IsTypeDynamic(type))
                    continue;
                if (info.IsGenericType || info.ContainsGenericParameters && DynamicAssemblies.IsTypeDynamic(type.GetGenericArguments()))
                    continue;
                uniqueTypes[type] = type;
                writer.Write("if (type == typeof(");
                writer.Write(CodeIdentifier.GetCSharpName(type));
                writer.WriteLine(")) return true;");
            }
            writer.WriteLine("return false;");
            writer.Indent--;
            writer.WriteLine("}");
        }

        internal static bool IsWildcard(SpecialMapping mapping) {
            if (mapping is SerializableMapping)
                return ((SerializableMapping)mapping).IsAny;
            return mapping.TypeDesc.CanBeElementValue;
        }
    }
}

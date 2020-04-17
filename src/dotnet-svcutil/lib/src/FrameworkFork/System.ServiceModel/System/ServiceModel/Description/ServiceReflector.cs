// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Microsoft.Xml;
using System.Threading.Tasks;
using System.Threading;

namespace System.ServiceModel.Description
{
    internal static class NamingHelper
    {
        internal const string DefaultNamespace = "http://tempuri.org/";
        internal const string DefaultServiceName = "service";
        internal const string MSNamespace = "http://schemas.microsoft.com/2005/07/ServiceModel";

        // simplified rules for appending paths to base URIs. note that this differs from new Uri(baseUri, string)
        // 1) CombineUriStrings("http://foo/bar/z", "baz") ==> "http://foo/bar/z/baz"
        // 2) CombineUriStrings("http://foo/bar/z/", "baz") ==> "http://foo/bar/z/baz"
        // 3) CombineUriStrings("http://foo/bar/z", "/baz") ==> "http://foo/bar/z/baz"
        // 4) CombineUriStrings("http://foo/bar/z", "http://baz/q") ==> "http://baz/q"
        // 5) CombineUriStrings("http://foo/bar/z", "") ==> ""

        internal static string CombineUriStrings(string baseUri, string path)
        {
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute) || path == String.Empty)
            {
                return path;
            }
            else
            {
                // combine
                if (baseUri.EndsWith("/", StringComparison.Ordinal))
                {
                    return baseUri + (path.StartsWith("/", StringComparison.Ordinal) ? path.Substring(1) : path);
                }
                else
                {
                    return baseUri + (path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path);
                }
            }
        }

        internal static string TypeName(Type t)
        {
            Type[] args = null;
            if (t.GetTypeInfo().IsGenericType)
            {
                args = t.GetTypeInfo().GenericTypeArguments;
            }
            else if (t.GetTypeInfo().ContainsGenericParameters)
            {
                args = t.GetTypeInfo().GenericTypeParameters;
            }

            if (args != null)
            {
                int nameEnd = t.Name.IndexOf('`');
                string result = nameEnd > 0 ? t.Name.Substring(0, nameEnd) : t.Name;
                result += "Of";
                for (int i = 0; i < args.Length; ++i)
                {
                    result = result + "_" + TypeName(args[i]);
                }
                return result;
            }
            else if (t.IsArray)
            {
                return "ArrayOf" + TypeName(t.GetElementType());
            }
            else
            {
                return t.Name;
            }
        }

        // name, ns could have any combination of nulls
        internal static XmlQualifiedName GetContractName(Type contractType, string name, string ns)
        {
            XmlName xmlName = new XmlName(name ?? TypeName(contractType));
            // ns can be empty
            if (ns == null)
            {
                ns = DefaultNamespace;
            }
            return new XmlQualifiedName(xmlName.EncodedName, ns);
        }

        // name could be null
        // logicalMethodName is MethodInfo.Name with Begin removed for async pattern
        // return encoded version to be used in OperationDescription
        internal static XmlName GetOperationName(string logicalMethodName, string name)
        {
            return new XmlName(String.IsNullOrEmpty(name) ? logicalMethodName : name);
        }

        internal static string GetMessageAction(OperationDescription operation, bool isResponse)
        {
            ContractDescription contract = operation.DeclaringContract;
            XmlQualifiedName contractQname = new XmlQualifiedName(contract.Name, contract.Namespace);
            return GetMessageAction(contractQname, operation.CodeName, null, isResponse);
        }

        // name could be null
        // logicalMethodName is MethodInfo.Name with Begin removed for async pattern
        internal static string GetMessageAction(XmlQualifiedName contractName, string opname, string action, bool isResponse)
        {
            if (action != null)
            {
                return action;
            }

            System.Text.StringBuilder actionBuilder = new System.Text.StringBuilder(64);
            if (String.IsNullOrEmpty(contractName.Namespace))
            {
                actionBuilder.Append("urn:");
            }
            else
            {
                actionBuilder.Append(contractName.Namespace);
                if (!contractName.Namespace.EndsWith("/", StringComparison.Ordinal))
                {
                    actionBuilder.Append('/');
                }
            }
            actionBuilder.Append(contractName.Name);
            actionBuilder.Append('/');
            action = isResponse ? opname + "Response" : opname;

            return CombineUriStrings(actionBuilder.ToString(), action);
        }

        internal delegate bool DoesNameExist(string name, object nameCollection);
        internal static string GetUniqueName(string baseName, DoesNameExist doesNameExist, object nameCollection)
        {
            for (int i = 0; i < Int32.MaxValue; i++)
            {
                string name = i > 0 ? baseName + i : baseName;
                if (!doesNameExist(name, nameCollection))
                {
                    return name;
                }
            }
            Fx.Assert("Too Many Names");
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Cannot generate unique name for name {0}", baseName)));
        }

        internal static void CheckUriProperty(string ns, string propName)
        {
            Uri uri;
            if (!Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out uri))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(string.Format(SRServiceModel.SFXUnvalidNamespaceValue, ns, propName));
        }

        internal static void CheckUriParameter(string ns, string paramName)
        {
            Uri uri;
            if (!Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out uri))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(paramName, string.Format(SRServiceModel.SFXUnvalidNamespaceParam, ns));
        }

        // Converts names that contain characters that are not permitted in XML names to valid names.
        internal static string XmlName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            if (IsAsciiLocalName(name))
                return name;
            if (IsValidNCName(name))
                return name;
            return XmlConvert.EncodeLocalName(name);
        }

        // Transforms an XML name into an object name.
        internal static string CodeName(string name)
        {
            return XmlConvert.DecodeName(name);
        }

        private static bool IsAlpha(char ch)
        {
            return (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z');
        }

        private static bool IsDigit(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }

        private static bool IsAsciiLocalName(string localName)
        {
            Fx.Assert(null != localName, "");
            if (!IsAlpha(localName[0]))
                return false;
            for (int i = 1; i < localName.Length; i++)
            {
                char ch = localName[i];
                if (!IsAlpha(ch) && !IsDigit(ch))
                    return false;
            }
            return true;
        }

        internal static bool IsValidNCName(string name)
        {
            try
            {
                XmlConvert.VerifyNCName(name);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }
    }

    internal class XmlName
    {
        private string _decoded;
        private string _encoded;

        internal XmlName(string name)
            : this(name, false)
        {
        }

        internal XmlName(string name, bool isEncoded)
        {
            if (isEncoded)
            {
                ValidateEncodedName(name, true /*allowNull*/);
                _encoded = name;
            }
            else
            {
                _decoded = name;
            }
        }

        internal string EncodedName
        {
            get
            {
                if (_encoded == null)
                    _encoded = NamingHelper.XmlName(_decoded);
                return _encoded;
            }
        }

        internal string DecodedName
        {
            get
            {
                if (_decoded == null)
                    _decoded = NamingHelper.CodeName(_encoded);
                return _decoded;
            }
        }

        private static void ValidateEncodedName(string name, bool allowNull)
        {
            if (allowNull && name == null)
                return;
            try
            {
                XmlConvert.VerifyNCName(name);
            }
            catch (XmlException e)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(e.Message, "name"));
            }
        }

        private bool IsEmpty { get { return string.IsNullOrEmpty(_encoded) && string.IsNullOrEmpty(_decoded); } }
        internal static bool IsNullOrEmpty(XmlName xmlName)
        {
            return xmlName == null || xmlName.IsEmpty;
        }

        private bool Matches(XmlName xmlName)
        {
            return string.Equals(this.EncodedName, xmlName.EncodedName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }

            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }

            XmlName xmlName = obj as XmlName;
            if (xmlName == null)
            {
                return false;
            }

            return Matches(xmlName);
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(EncodedName))
                return 0;
            return EncodedName.GetHashCode();
        }

        public override string ToString()
        {
            if (_encoded == null && _decoded == null)
                return null;
            if (_encoded != null)
                return _encoded;
            return _decoded;
        }

        public static bool operator ==(XmlName a, XmlName b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return (a.Equals(b));
        }

        public static bool operator !=(XmlName a, XmlName b)
        {
            return !(a == b);
        }
    }

    static internal class ServiceReflector
    {
        internal const string BeginMethodNamePrefix = "Begin";
        internal const string EndMethodNamePrefix = "End";
        internal static readonly Type VoidType = typeof(void);
        internal const string AsyncMethodNameSuffix = "Async";
        internal static readonly Type taskType = typeof(Task);
        internal static readonly Type taskTResultType = typeof(Task<>);
        internal static readonly Type CancellationTokenType = typeof(CancellationToken);
        internal static readonly Type IProgressType = typeof(IProgress<>);
        private static readonly Type s_asyncCallbackType = typeof(AsyncCallback);
        private static readonly Type s_asyncResultType = typeof(IAsyncResult);
        private static readonly Type s_objectType = typeof(object);
        private static readonly Type s_OperationContractAttributeType = typeof(OperationContractAttribute);

        static internal Type GetOperationContractProviderType(MethodInfo method)
        {
            if (GetSingleAttribute<OperationContractAttribute>(method) != null)
            {
                return s_OperationContractAttributeType;
            }
            IOperationContractAttributeProvider provider = GetFirstAttribute<IOperationContractAttributeProvider>(method);
            if (provider != null)
            {
                return provider.GetType();
            }
            return null;
        }

        // returns the set of root interfaces for the service class (meaning doesn't include callback ifaces)
        static internal List<Type> GetInterfaces(Type service)
        {
            List<Type> types = new List<Type>();
            bool implicitContract = false;
            if (service.IsDefined(typeof(ServiceContractAttribute), false))
            {
                implicitContract = true;
                types.Add(service);
            }
            if (!implicitContract)
            {
                Type t = GetAncestorImplicitContractClass(service);
                if (t != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxContractInheritanceRequiresInterfaces2, service, t)));
                }
                foreach (MethodInfo method in GetMethodsInternal(service))
                {
                    Type operationContractProviderType = GetOperationContractProviderType(method);
                    if (operationContractProviderType == s_OperationContractAttributeType)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ServicesWithoutAServiceContractAttributeCan2, operationContractProviderType.Name, method.Name, service.FullName)));
                    }
                }
            }
            foreach (Type t in service.GetInterfaces())
            {
                if (t.IsDefined(typeof(ServiceContractAttribute), false))
                {
                    if (implicitContract)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxContractInheritanceRequiresInterfaces, service, t)));
                    }
                    types.Add(t);
                }
            }

            return types;
        }

        private static Type GetAncestorImplicitContractClass(Type service)
        {
            for (service = service.BaseType(); service != null; service = service.BaseType())
            {
                if (ServiceReflector.GetSingleAttribute<ServiceContractAttribute>(service) != null)
                {
                    return service;
                }
            }
            return null;
        }

        static internal List<Type> GetInheritedContractTypes(Type service)
        {
            List<Type> types = new List<Type>();
            foreach (Type t in service.GetInterfaces())
            {
                if (ServiceReflector.GetSingleAttribute<ServiceContractAttribute>(t) != null)
                {
                    types.Add(t);
                }
            }
            for (service = service.BaseType(); service != null; service = service.BaseType())
            {
                if (ServiceReflector.GetSingleAttribute<ServiceContractAttribute>(service) != null)
                {
                    types.Add(service);
                }
            }
            return types;
        }

        static internal object[] GetCustomAttributes(CustomAttributeProvider attrProvider, Type attrType)
        {
            return GetCustomAttributes(attrProvider, attrType, false);
        }

        static internal object[] GetCustomAttributes(CustomAttributeProvider attrProvider, Type attrType, bool inherit)
        {
            try
            {
                if (typeof(Attribute).IsAssignableFrom(attrType))
                {
                    return attrProvider.GetCustomAttributes(attrType, inherit);
                }
                else
                {
                    List<object> attrTypeAttributes = new List<object>();
                    object[] allCustomAttributes = attrProvider.GetCustomAttributes(inherit);
                    foreach (object customAttribute in allCustomAttributes)
                    {
                        if (attrType.IsAssignableFrom(customAttribute.GetType()))
                        {
                            attrTypeAttributes.Add(customAttribute);
                        }
                    }

                    return attrTypeAttributes.ToArray();
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                Type type = attrProvider.Type;
                MethodInfo method = attrProvider.MethodInfo;
                ParameterInfo param = attrProvider.ParameterInfo;
                // there is no good way to know if this is a return type attribute
                if (type != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxErrorReflectingOnType2, attrType.Name, type.Name), e));
                }
                else if (method != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxErrorReflectingOnMethod3,
                                     attrType.Name, method.Name, method.DeclaringType.Name), e));
                }
                else if (param != null)
                {
                    method = param.Member as MethodInfo;
                    if (method != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            string.Format(SRServiceModel.SFxErrorReflectingOnParameter4,
                                         attrType.Name, param.Name, method.Name, method.DeclaringType.Name), e));
                    }
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.SFxErrorReflectionOnUnknown1, attrType.Name), e));
            }
        }

        static internal T GetFirstAttribute<T>(CustomAttributeProvider attrProvider)
            where T : class
        {
            Type attrType = typeof(T);
            object[] attrs = GetCustomAttributes(attrProvider, attrType);
            if (attrs.Length == 0)
            {
                return null;
            }
            else
            {
                return attrs[0] as T;
            }
        }

        static internal T GetSingleAttribute<T>(CustomAttributeProvider attrProvider)
            where T : class
        {
            Type attrType = typeof(T);
            object[] attrs = GetCustomAttributes(attrProvider, attrType);
            if (attrs == null || attrs.Length == 0)
            {
                return null;
            }
            else if (attrs.Length > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.tooManyAttributesOfTypeOn2, attrType, attrProvider.ToString())));
            }
            else
            {
                return attrs[0] as T;
            }
        }
        static internal T GetRequiredSingleAttribute<T>(CustomAttributeProvider attrProvider)
            where T : class
        {
            T result = GetSingleAttribute<T>(attrProvider);
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.couldnTFindRequiredAttributeOfTypeOn2, typeof(T), attrProvider.ToString())));
            }
            return result;
        }
        static internal T GetSingleAttribute<T>(CustomAttributeProvider attrProvider, Type[] attrTypeGroup)
            where T : class
        {
            T result = GetSingleAttribute<T>(attrProvider);
            if (result != null)
            {
                Type attrType = typeof(T);
                foreach (Type otherType in attrTypeGroup)
                {
                    if (otherType == attrType)
                    {
                        continue;
                    }
                    object[] attrs = GetCustomAttributes(attrProvider, otherType);
                    if (attrs != null && attrs.Length > 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxDisallowedAttributeCombination, attrProvider, attrType.FullName, otherType.FullName)));
                    }
                }
            }
            return result;
        }
        static internal T GetRequiredSingleAttribute<T>(CustomAttributeProvider attrProvider, Type[] attrTypeGroup)
            where T : class
        {
            T result = GetSingleAttribute<T>(attrProvider, attrTypeGroup);
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.couldnTFindRequiredAttributeOfTypeOn2, typeof(T), attrProvider.ToString())));
            }
            return result;
        }
        static internal Type GetContractType(Type interfaceType)
        {
            ServiceContractAttribute contractAttribute;
            return GetContractTypeAndAttribute(interfaceType, out contractAttribute);
        }

        static internal Type GetContractTypeAndAttribute(Type interfaceType, out ServiceContractAttribute contractAttribute)
        {
            contractAttribute = GetSingleAttribute<ServiceContractAttribute>(interfaceType);
            if (contractAttribute != null)
            {
                return interfaceType;
            }

            List<Type> types = new List<Type>(GetInheritedContractTypes(interfaceType));
            if (types.Count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.AttemptedToGetContractTypeForButThatTypeIs1, interfaceType.Name)));
            }


            foreach (Type potentialContractRoot in types)
            {
                bool mayBeTheRoot = true;
                foreach (Type t in types)
                {
                    if (!t.IsAssignableFrom(potentialContractRoot))
                    {
                        mayBeTheRoot = false;
                    }
                }
                if (mayBeTheRoot)
                {
                    contractAttribute = GetSingleAttribute<ServiceContractAttribute>(potentialContractRoot);
                    return potentialContractRoot;
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                string.Format(SRServiceModel.SFxNoMostDerivedContract, interfaceType.Name)));
        }

        private static List<MethodInfo> GetMethodsInternal(Type interfaceType)
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (MethodInfo mi in interfaceType.GetRuntimeMethods().Where(m => !m.IsStatic))
            {
                if (GetSingleAttribute<OperationContractAttribute>(mi) != null)
                {
                    methods.Add(mi);
                }
                else if (GetFirstAttribute<IOperationContractAttributeProvider>(mi) != null)
                {
                    methods.Add(mi);
                }
            }
            return methods;
        }

        // The metadata for "in" versus "out" seems to be inconsistent, depending upon what compiler generates it.
        // The following code assumes this is the truth table that all compilers will obey:
        // 
        // True Parameter Type     .IsIn      .IsOut    .ParameterType.IsByRef
        //
        // in                        F          F         F         ...OR...
        // in                        T          F         F
        //
        // in/out                    T          T         T         ...OR...
        // in/out                    F          F         T
        //
        // out                       F          T         T
        static internal void ValidateParameterMetadata(MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                if (!parameter.ParameterType.IsByRef)
                {
                    if (parameter.IsOut)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SFxBadByValueParameterMetadata,
                            methodInfo.Name, methodInfo.DeclaringType.Name)));
                    }
                }
                else
                {
                    if (parameter.IsIn && !parameter.IsOut)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SFxBadByReferenceParameterMetadata,
                            methodInfo.Name, methodInfo.DeclaringType.Name)));
                    }
                }
            }
        }

        static internal bool FlowsIn(ParameterInfo paramInfo)    // conceptually both "in" and "in/out" params return true
        {
            return !paramInfo.IsOut || paramInfo.IsIn;
        }
        static internal bool FlowsOut(ParameterInfo paramInfo)   // conceptually both "out" and "in/out" params return true
        {
            return paramInfo.ParameterType.IsByRef;
        }

        // for async method is the begin method
        static internal ParameterInfo[] GetInputParameters(MethodInfo method, bool asyncPattern)
        {
            int count = 0;
            ParameterInfo[] parameters = method.GetParameters();

            // length of parameters we care about (-2 for async)
            int len = parameters.Length;
            if (asyncPattern)
            {
                len -= 2;
            }

            // count the ins
            for (int i = 0; i < len; i++)
            {
                if (FlowsIn(parameters[i]))
                {
                    count++;
                }
            }

            // grab the ins
            ParameterInfo[] result = new ParameterInfo[count];
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                ParameterInfo param = parameters[i];
                if (FlowsIn(param))
                {
                    result[pos++] = param;
                }
            }
            return result;
        }

        // for async method is the end method
        static internal ParameterInfo[] GetOutputParameters(MethodInfo method, bool asyncPattern)
        {
            int count = 0;
            ParameterInfo[] parameters = method.GetParameters();

            // length of parameters we care about (-1 for async)
            int len = parameters.Length;
            if (asyncPattern)
            {
                len -= 1;
            }

            // count the outs
            for (int i = 0; i < len; i++)
            {
                if (FlowsOut(parameters[i]))
                {
                    count++;
                }
            }

            // grab the outs
            ParameterInfo[] result = new ParameterInfo[count];
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                ParameterInfo param = parameters[i];
                if (FlowsOut(param))
                {
                    result[pos++] = param;
                }
            }
            return result;
        }

        static internal bool HasOutputParameters(MethodInfo method, bool asyncPattern)
        {
            ParameterInfo[] parameters = method.GetParameters();

            // length of parameters we care about (-1 for async)
            int len = parameters.Length;
            if (asyncPattern)
            {
                len -= 1;
            }

            // count the outs
            for (int i = 0; i < len; i++)
            {
                if (FlowsOut(parameters[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static MethodInfo GetEndMethodInternal(MethodInfo beginMethod)
        {
            string logicalName = GetLogicalName(beginMethod);
            string endMethodName = EndMethodNamePrefix + logicalName;
            MethodInfo[] endMethods = beginMethod.DeclaringType.GetTypeInfo().GetDeclaredMethods(endMethodName).ToArray();
            if (endMethods.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.NoEndMethodFoundForAsyncBeginMethod3, beginMethod.Name, beginMethod.DeclaringType.FullName, endMethodName)));
            }
            if (endMethods.Length > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.MoreThanOneEndMethodFoundForAsyncBeginMethod3, beginMethod.Name, beginMethod.DeclaringType.FullName, endMethodName)));
            }
            return (MethodInfo)endMethods[0];
        }

        static internal MethodInfo GetEndMethod(MethodInfo beginMethod)
        {
            MethodInfo endMethod = GetEndMethodInternal(beginMethod);

            if (!HasEndMethodShape(endMethod))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.InvalidAsyncEndMethodSignatureForMethod2, endMethod.Name, endMethod.DeclaringType.FullName)));
            }

            return endMethod;
        }

        static internal XmlName GetOperationName(MethodInfo method)
        {
            OperationContractAttribute operationAttribute = GetOperationContractAttribute(method);
            return NamingHelper.GetOperationName(GetLogicalName(method), operationAttribute.Name);
        }


        static internal bool HasBeginMethodShape(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (!method.Name.StartsWith(BeginMethodNamePrefix, StringComparison.Ordinal) ||
                parameters.Length < 2 ||
                parameters[parameters.Length - 2].ParameterType != s_asyncCallbackType ||
                parameters[parameters.Length - 1].ParameterType != s_objectType ||
                method.ReturnType != s_asyncResultType)
            {
                return false;
            }
            return true;
        }

        static internal bool IsBegin(OperationContractAttribute opSettings, MethodInfo method)
        {
            if (opSettings.AsyncPattern)
            {
                if (!HasBeginMethodShape(method))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.InvalidAsyncBeginMethodSignatureForMethod2, method.Name, method.DeclaringType.FullName)));
                }

                return true;
            }
            return false;
        }

        static internal bool IsTask(MethodInfo method)
        {
            if (method.ReturnType == taskType)
            {
                return true;
            }
            if (method.ReturnType.IsGenericType() && method.ReturnType.GetGenericTypeDefinition() == taskTResultType)
            {
                return true;
            }
            return false;
        }

        static internal bool IsTask(MethodInfo method, out Type taskTResult)
        {
            taskTResult = null;
            Type methodReturnType = method.ReturnType;
            if (methodReturnType == taskType)
            {
                taskTResult = VoidType;
                return true;
            }

            if (methodReturnType.IsGenericType() && methodReturnType.GetGenericTypeDefinition() == taskTResultType)
            {
                taskTResult = methodReturnType.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        static internal bool HasEndMethodShape(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (!method.Name.StartsWith(EndMethodNamePrefix, StringComparison.Ordinal) ||
                parameters.Length < 1 ||
                parameters[parameters.Length - 1].ParameterType != s_asyncResultType)
            {
                return false;
            }
            return true;
        }

        internal static OperationContractAttribute GetOperationContractAttribute(MethodInfo method)
        {
            OperationContractAttribute operationContractAttribute = GetSingleAttribute<OperationContractAttribute>(method);
            if (operationContractAttribute != null)
            {
                return operationContractAttribute;
            }
            IOperationContractAttributeProvider operationContractProvider = GetFirstAttribute<IOperationContractAttributeProvider>(method);
            if (operationContractProvider != null)
            {
                return operationContractProvider.GetOperationContractAttribute();
            }
            return null;
        }

        static internal bool IsBegin(MethodInfo method)
        {
            OperationContractAttribute opSettings = GetOperationContractAttribute(method);
            if (opSettings == null)
                return false;
            return IsBegin(opSettings, method);
        }

        static internal string GetLogicalName(MethodInfo method)
        {
            bool isAsync = IsBegin(method);
            bool isTask = isAsync ? false : IsTask(method);
            return GetLogicalName(method, isAsync, isTask);
        }

        static internal string GetLogicalName(MethodInfo method, bool isAsync, bool isTask)
        {
            if (isAsync)
            {
                return method.Name.Substring(BeginMethodNamePrefix.Length);
            }
            else if (isTask && method.Name.EndsWith(AsyncMethodNameSuffix, StringComparison.Ordinal))
            {
                return method.Name.Substring(0, method.Name.Length - AsyncMethodNameSuffix.Length);
            }
            else
            {
                return method.Name;
            }
        }

        static internal bool HasNoDisposableParameters(MethodInfo methodInfo)
        {
            foreach (ParameterInfo inputInfo in methodInfo.GetParameters())
            {
                if (IsParameterDisposable(inputInfo.ParameterType))
                {
                    return false;
                }
            }

            if (methodInfo.ReturnParameter != null)
            {
                return (!IsParameterDisposable(methodInfo.ReturnParameter.ParameterType));
            }

            return true;
        }

        static internal bool IsParameterDisposable(Type type)
        {
            return ((!type.IsSealed()) || typeof(IDisposable).IsAssignableFrom(type));
        }
    }
}

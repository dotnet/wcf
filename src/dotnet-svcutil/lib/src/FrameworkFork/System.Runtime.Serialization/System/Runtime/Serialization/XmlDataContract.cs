// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Microsoft.Xml;
    using DataContractDictionary = System.Collections.Generic.Dictionary<Microsoft.Xml.XmlQualifiedName, DataContract>;
    using Microsoft.Xml.Serialization;
    using Microsoft.Xml.Schema;
    using System.Security;
    using System.Linq;

#if NET_NATIVE
    public delegate IXmlSerializable CreateXmlSerializableDelegate();
    public sealed class XmlDataContract : DataContract
#else
    internal delegate IXmlSerializable CreateXmlSerializableDelegate();
    internal sealed class XmlDataContract : DataContract
#endif
    {
        [SecurityCritical]
        /// <SecurityNote>
        /// Critical - holds instance of CriticalHelper which keeps state that is cached statically for serialization. 
        ///            Static fields are marked SecurityCritical or readonly to prevent
        ///            data from being modified or leaked to other components in appdomain.
        /// </SecurityNote>
        private XmlDataContractCriticalHelper _helper;

        /// <SecurityNote>
        /// Critical - initializes SecurityCritical field 'helper'
        /// Safe - doesn't leak anything
        /// </SecurityNote>
        [SecuritySafeCritical]
        public XmlDataContract() : base(new XmlDataContractCriticalHelper())
        {
            _helper = base.Helper as XmlDataContractCriticalHelper;
        }

        /// <SecurityNote>
        /// Critical - initializes SecurityCritical field 'helper'
        /// Safe - doesn't leak anything
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal XmlDataContract(Type type) : base(new XmlDataContractCriticalHelper(type))
        {
            _helper = base.Helper as XmlDataContractCriticalHelper;
        }

        public override DataContractDictionary KnownDataContracts
        {
            /// <SecurityNote>
            /// Critical - fetches the critical KnownDataContracts property 
            /// Safe - KnownDataContracts only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.KnownDataContracts; }
            /// <SecurityNote>
            /// Critical - sets the critical KnownDataContracts property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.KnownDataContracts = value; }
        }

        internal bool IsAnonymous
        {
            /// <SecurityNote>
            /// Critical - fetches the critical IsAnonymous property
            /// Safe - IsAnonymous only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.IsAnonymous; }
        }

        public override bool HasRoot
        {
            /// <SecurityNote>
            /// Critical - fetches the critical HasRoot property
            /// Safe - HasRoot only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.HasRoot; }
            /// <SecurityNote>
            /// Critical - sets the critical HasRoot property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.HasRoot = value; }
        }

        public override XmlDictionaryString TopLevelElementName
        {
            /// <SecurityNote>
            /// Critical - fetches the critical TopLevelElementName property
            /// Safe - TopLevelElementName only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.TopLevelElementName; }
            /// <SecurityNote>
            /// Critical - sets the critical TopLevelElementName property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.TopLevelElementName = value; }
        }

        internal XmlSchemaType XsdType
        {
            // TODO: [Fx.Tag.SecurityNote(Critical = "Fetches the critical XsdType property.",
            //    Safe = "XsdType only needs to be protected for write.")]
            [SecuritySafeCritical]
            get { return _helper.XsdType; }

            // TODO: [Fx.Tag.SecurityNote(Critical = "Sets the critical XsdType property.")]
            [SecurityCritical]
            set { _helper.XsdType = value; }
        }

        public override XmlDictionaryString TopLevelElementNamespace
        {
            /// <SecurityNote>
            /// Critical - fetches the critical TopLevelElementNamespace property
            /// Safe - TopLevelElementNamespace only needs to be protected for write
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            { return _helper.TopLevelElementNamespace; }
            /// <SecurityNote>
            /// Critical - sets the critical TopLevelElementNamespace property
            /// </SecurityNote>
            [SecurityCritical]
            set
            { _helper.TopLevelElementNamespace = value; }
        }

        internal bool IsTopLevelElementNullable
        {
            // TODO: [Fx.Tag.SecurityNote(Critical = "Fetches the critical IsTopLevelElementNullable property.",
            //    Safe = "IsTopLevelElementNullable only needs to be protected for write.")]
            [SecuritySafeCritical]
            get { return _helper.IsTopLevelElementNullable; }

            // TODO: [Fx.Tag.SecurityNote(Critical = "Sets the critical IsTopLevelElementNullable property.")]
            [SecurityCritical]
            set { _helper.IsTopLevelElementNullable = value; }
        }

        internal bool IsTypeDefinedOnImport
        {
            // TODO: [Fx.Tag.SecurityNote(Critical = "Fetches the critical IsTypeDefinedOnImport property.",
            //    Safe = "IsTypeDefinedOnImport only needs to be protected for write.")]
            [SecuritySafeCritical]
            get { return _helper.IsTypeDefinedOnImport; }

            // TODO: [Fx.Tag.SecurityNote(Critical = "Sets the critical IsTypeDefinedOnImport property.")]
            [SecurityCritical]
            set { _helper.IsTypeDefinedOnImport = value; }
        }

#if !NET_NATIVE
        internal CreateXmlSerializableDelegate CreateXmlSerializableDelegate
        {
            /// <SecurityNote>
            /// Critical - fetches the critical CreateXmlSerializableDelegate property
            /// Safe - CreateXmlSerializableDelegate only needs to be protected for write; initialized in getter if null
            /// </SecurityNote>
            [SecuritySafeCritical]
            get
            {
                if (_helper.CreateXmlSerializableDelegate == null)
                {
                    lock (this)
                    {
                        if (_helper.CreateXmlSerializableDelegate == null)
                        {
                            CreateXmlSerializableDelegate tempCreateXmlSerializable = GenerateCreateXmlSerializableDelegate();
                            Interlocked.MemoryBarrier();
                            _helper.CreateXmlSerializableDelegate = tempCreateXmlSerializable;
                        }
                    }
                }
                return _helper.CreateXmlSerializableDelegate;
            }
        }
#else
        public CreateXmlSerializableDelegate CreateXmlSerializableDelegate { get; set; }
#endif

        internal override bool CanContainReferences
        {
            get { return false; }
        }

        public override bool IsBuiltInDataContract
        {
            get
            {
                return UnderlyingType == Globals.TypeOfXmlElement || UnderlyingType == Globals.TypeOfXmlNodeArray;
            }
        }

        /// <SecurityNote>
        /// Critical - holds all state used for for (de)serializing XML types.
        ///            since the data is cached statically, we lock down access to it.
        /// </SecurityNote>
        [SecurityCritical]
        private class XmlDataContractCriticalHelper : DataContract.DataContractCriticalHelper
        {
            private DataContractDictionary _knownDataContracts;
            private bool _isKnownTypeAttributeChecked;
            private XmlDictionaryString _topLevelElementName;
            private XmlDictionaryString _topLevelElementNamespace;
            private bool _isTopLevelElementNullable;
            private bool _isTypeDefinedOnImport;
            private XmlSchemaType _xsdType;
            private bool _hasRoot;
            private CreateXmlSerializableDelegate _createXmlSerializable;

            internal XmlDataContractCriticalHelper()
            {
            }

            internal XmlDataContractCriticalHelper(Type type) : base(type)
            {
                if (type.GetTypeInfo().IsAttributeDefined(Globals.TypeOfDataContractAttribute))
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.IXmlSerializableCannotHaveDataContract, DataContract.GetClrTypeFullName(type))));
                if (type.GetTypeInfo().IsAttributeDefined(Globals.TypeOfCollectionDataContractAttribute))
                    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.IXmlSerializableCannotHaveCollectionDataContract, DataContract.GetClrTypeFullName(type))));
                XmlSchemaType xsdType;
                bool hasRoot;
                XmlQualifiedName stableName;
                SchemaExporter.GetXmlTypeInfo(type, out stableName, out xsdType, out hasRoot);
                this.StableName = stableName;
                this.XsdType = xsdType;
                this.HasRoot = hasRoot;
                XmlDictionary dictionary = new XmlDictionary();
                this.Name = dictionary.Add(StableName.Name);
                this.Namespace = dictionary.Add(StableName.Namespace);
                object[] xmlRootAttributes = (UnderlyingType == null) ? null : UnderlyingType.GetTypeInfo().GetCustomAttributes(Globals.TypeOfXmlRootAttribute, false).ToArray();
                if (xmlRootAttributes == null || xmlRootAttributes.Length == 0)
                {
                    if (hasRoot)
                    {
                        _topLevelElementName = Name;
                        _topLevelElementNamespace = (this.StableName.Namespace == Globals.SchemaNamespace) ? DictionaryGlobals.EmptyString : Namespace;
                    }
                }
                else
                {
                    if (hasRoot)
                    {
                        XmlRootAttribute xmlRootAttribute = (XmlRootAttribute)xmlRootAttributes[0];
                        _isTopLevelElementNullable = xmlRootAttribute.IsNullable;
                        string elementName = xmlRootAttribute.ElementName;
                        _topLevelElementName = (elementName == null || elementName.Length == 0) ? Name : dictionary.Add(DataContract.EncodeLocalName(elementName));
                        string elementNs = xmlRootAttribute.Namespace;
                        _topLevelElementNamespace = (elementNs == null || elementNs.Length == 0) ? DictionaryGlobals.EmptyString : dictionary.Add(elementNs);
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.IsAnyCannotHaveXmlRoot, DataContract.GetClrTypeFullName(UnderlyingType))));
                    }
                }
            }

            internal override DataContractDictionary KnownDataContracts
            {
                [SecurityCritical]
                get
                {
                    if (!_isKnownTypeAttributeChecked && UnderlyingType != null)
                    {
                        lock (this)
                        {
                            if (!_isKnownTypeAttributeChecked)
                            {
                                _knownDataContracts = DataContract.ImportKnownTypeAttributes(this.UnderlyingType);
                                Interlocked.MemoryBarrier();
                                _isKnownTypeAttributeChecked = true;
                            }
                        }
                    }
                    return _knownDataContracts;
                }
                [SecurityCritical]
                set
                { _knownDataContracts = value; }
            }

            internal XmlSchemaType XsdType
            {
                get { return _xsdType; }
                set { _xsdType = value; }
            }

            internal bool IsAnonymous
            {
                get { return false; }
            }

            internal override bool HasRoot
            {
                get { return _hasRoot; }
                set { _hasRoot = value; }
            }

            internal override XmlDictionaryString TopLevelElementName
            {
                [SecurityCritical]
                get
                { return _topLevelElementName; }
                [SecurityCritical]
                set
                { _topLevelElementName = value; }
            }

            internal override XmlDictionaryString TopLevelElementNamespace
            {
                [SecurityCritical]
                get
                { return _topLevelElementNamespace; }
                [SecurityCritical]
                set
                { _topLevelElementNamespace = value; }
            }

            internal bool IsTopLevelElementNullable
            {
                get { return _isTopLevelElementNullable; }
                set { _isTopLevelElementNullable = value; }
            }

            internal bool IsTypeDefinedOnImport
            {
                get { return _isTypeDefinedOnImport; }
                set { _isTypeDefinedOnImport = value; }
            }

            internal CreateXmlSerializableDelegate CreateXmlSerializableDelegate
            {
                get { return _createXmlSerializable; }
                set { _createXmlSerializable = value; }
            }
        }

        private ConstructorInfo GetConstructor()
        {
            Type type = UnderlyingType;

            if (type.GetTypeInfo().IsValueType)
                return null;

            ConstructorInfo ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, Array.Empty<Type>());
            if (ctor == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SRSerialization.Format(SRSerialization.IXmlSerializableMustHaveDefaultConstructor, DataContract.GetClrTypeFullName(type))));

            return ctor;
        }

        //TODO: [Fx.Tag.SecurityNote(Critical = "Sets critical properties on XmlDataContract.")]
        [SecurityCritical]
        internal void SetTopLevelElementName(XmlQualifiedName elementName)
        {
            if (elementName != null)
            {
                XmlDictionary dictionary = new XmlDictionary();
                this.TopLevelElementName = dictionary.Add(elementName.Name);
                this.TopLevelElementNamespace = dictionary.Add(elementName.Namespace);
            }
        }


#if !NET_NATIVE
        /// <SecurityNote>
        /// Critical - calls CodeGenerator.BeginMethod which is SecurityCritical
        /// Safe - self-contained: returns the delegate to the generated IL but otherwise all IL generation is self-contained here
        /// </SecurityNote>
        [SecuritySafeCritical]
        internal CreateXmlSerializableDelegate GenerateCreateXmlSerializableDelegate()
        {
            Type type = this.UnderlyingType;
            CodeGenerator ilg = new CodeGenerator();
            bool memberAccessFlag = RequiresMemberAccessForCreate(null) && !(type.FullName == "System.Xml.Linq.XElement");
            try
            {
                ilg.BeginMethod("Create" + DataContract.GetClrTypeFullName(type), typeof(CreateXmlSerializableDelegate), memberAccessFlag);
            }
            catch (SecurityException securityException)
            {
                if (memberAccessFlag)
                {
                    RequiresMemberAccessForCreate(securityException);
                }
                else
                {
                    throw;
                }
            }
            if (type.GetTypeInfo().IsValueType)
            {
                System.Reflection.Emit.LocalBuilder local = ilg.DeclareLocal(type, type.Name + "Value");
                ilg.Ldloca(local);
                ilg.InitObj(type);
                ilg.Ldloc(local);
            }
            else
            {
                // Special case XElement
                // codegen the same as 'internal XElement : this("default") { }'
                ConstructorInfo ctor = GetConstructor();
                if (!ctor.IsPublic && type.FullName == "System.Xml.Linq.XElement")
                {
                    Type xName = type.GetTypeInfo().Assembly.GetType("System.Xml.Linq.XName");
                    if (xName != null)
                    {
                        MethodInfo XName_op_Implicit = xName.GetMethod(
                            "op_Implicit",
                            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                            new Type[] { typeof(String) }
                            );
                        ConstructorInfo XElement_ctor = type.GetConstructor(
                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                            new Type[] { xName }
                            );
                        if (XName_op_Implicit != null && XElement_ctor != null)
                        {
                            ilg.Ldstr("default");
                            ilg.Call(XName_op_Implicit);
                            ctor = XElement_ctor;
                        }
                    }
                }
                ilg.New(ctor);
            }
            ilg.ConvertValue(this.UnderlyingType, Globals.TypeOfIXmlSerializable);
            ilg.Ret();
            return (CreateXmlSerializableDelegate)ilg.EndMethod();
        }

        /// <SecurityNote>
        /// Review - calculates whether this Xml type requires MemberAccessPermission for deserialization.
        ///          since this information is used to determine whether to give the generated code access
        ///          permissions to private members, any changes to the logic should be reviewed.
        /// </SecurityNote>
        private bool RequiresMemberAccessForCreate(SecurityException securityException)
        {
            if (!IsTypeVisible(UnderlyingType))
            {
                if (securityException != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new SecurityException(SRSerialization.Format(SRSerialization.PartialTrustIXmlSerializableTypeNotPublic, DataContract.GetClrTypeFullName(UnderlyingType)),
                        securityException));
                }
                return true;
            }

            if (ConstructorRequiresMemberAccess(GetConstructor()))
            {
                if (securityException != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new SecurityException(SRSerialization.Format(SRSerialization.PartialTrustIXmlSerialzableNoPublicConstructor, DataContract.GetClrTypeFullName(UnderlyingType)),
                        securityException));
                }
                return true;
            }

            return false;
        }
#endif

        public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
        {
            if (context == null)
                XmlObjectSerializerWriteContext.WriteRootIXmlSerializable(xmlWriter, obj);
            else
                context.WriteIXmlSerializable(xmlWriter, obj);
        }

        public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
        {
            object o;
            if (context == null)
            {
                o = XmlObjectSerializerReadContext.ReadRootIXmlSerializable(xmlReader, this, true /*isMemberType*/);
            }
            else
            {
                o = context.ReadIXmlSerializable(xmlReader, this, true /*isMemberType*/);
                context.AddNewObject(o);
            }
            xmlReader.ReadEndElement();
            return o;
        }
    }
}


// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.Xml;
using System.Collections.Generic;
using System.Globalization;

namespace System.Runtime.Serialization
{
    public sealed class ExtensionDataObject
    {
        private IList<ExtensionDataMember> _members;

#if USE_REFEMIT
        public ExtensionDataObject()
#else
        internal ExtensionDataObject()
#endif
        {
        }

#if USE_REFEMIT
        public IList<ExtensionDataMember> Members
#else
        internal IList<ExtensionDataMember> Members
#endif
        {
            get { return _members; }
            set { _members = value; }
        }
    }

#if USE_REFEMIT
    public class ExtensionDataMember
#else
    internal class ExtensionDataMember
#endif
    {
        private string _name;
        private string _ns;
        private IDataNode _value;
        private int _memberIndex;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Namespace
        {
            get { return _ns; }
            set { _ns = value; }
        }

        public IDataNode Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public int MemberIndex
        {
            get { return _memberIndex; }
            set { _memberIndex = value; }
        }
    }

#if USE_REFEMIT
    public interface IDataNode
#else
    internal interface IDataNode
#endif
    {
        Type DataType { get; }
        object Value { get; set; }  // boxes for primitives
        string DataContractName { get; set; }
        string DataContractNamespace { get; set; }
        string ClrTypeName { get; set; }
        string ClrAssemblyName { get; set; }
        string Id { get; set; }
        bool PreservesReferences { get; }

        // NOTE: consider moving below APIs to DataNode<T> if IDataNode API is made public
        void GetData(ElementData element);
        bool IsFinalValue { get; set; }
        void Clear();
    }

    internal class DataNode<T> : IDataNode
    {
        protected Type dataType;
        private T _value;
        private string _dataContractName;
        private string _dataContractNamespace;
        private string _clrTypeName;
        private string _clrAssemblyName;
        private string _id = Globals.NewObjectId;
        private bool _isFinalValue;

        internal DataNode()
        {
            this.dataType = typeof(T);
            _isFinalValue = true;
        }

        internal DataNode(T value)
            : this()
        {
            _value = value;
        }

        public Type DataType
        {
            get { return dataType; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = (T)value; }
        }

        bool IDataNode.IsFinalValue
        {
            get { return _isFinalValue; }
            set { _isFinalValue = value; }
        }

        public T GetValue()
        {
            return _value;
        }

#if NotUsed
        public void SetValue(T value)
        {
            this.value = value;
        }
#endif

        public string DataContractName
        {
            get { return _dataContractName; }
            set { _dataContractName = value; }
        }

        public string DataContractNamespace
        {
            get { return _dataContractNamespace; }
            set { _dataContractNamespace = value; }
        }

        public string ClrTypeName
        {
            get { return _clrTypeName; }
            set { _clrTypeName = value; }
        }

        public string ClrAssemblyName
        {
            get { return _clrAssemblyName; }
            set { _clrAssemblyName = value; }
        }

        public bool PreservesReferences
        {
            get { return (Id != Globals.NewObjectId); }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual void GetData(ElementData element)
        {
            element.dataNode = this;
            element.attributeCount = 0;
            element.childElementIndex = 0;

            if (DataContractName != null)
                AddQualifiedNameAttribute(element, Globals.XsiPrefix, Globals.XsiTypeLocalName, Globals.SchemaInstanceNamespace, DataContractName, DataContractNamespace);
            if (ClrTypeName != null)
                element.AddAttribute(Globals.SerPrefix, Globals.SerializationNamespace, Globals.ClrTypeLocalName, ClrTypeName);
            if (ClrAssemblyName != null)
                element.AddAttribute(Globals.SerPrefix, Globals.SerializationNamespace, Globals.ClrAssemblyLocalName, ClrAssemblyName);
        }

        public virtual void Clear()
        {
            // dataContractName not cleared because it is used when re-serializing from unknown data
            _clrTypeName = _clrAssemblyName = null;
        }

        internal void AddQualifiedNameAttribute(ElementData element, string elementPrefix, string elementName, string elementNs, string valueName, string valueNs)
        {
            string prefix = ExtensionDataReader.GetPrefix(valueNs);
            element.AddAttribute(elementPrefix, elementNs, elementName, String.Format(CultureInfo.InvariantCulture, "{0}:{1}", prefix, valueName));

            bool prefixDeclaredOnElement = false;
            if (element.attributes != null)
            {
                for (int i = 0; i < element.attributes.Length; i++)
                {
                    AttributeData attribute = element.attributes[i];
                    if (attribute != null && attribute.prefix == Globals.XmlnsPrefix && attribute.localName == prefix)
                    {
                        prefixDeclaredOnElement = true;
                        break;
                    }
                }
            }
            if (!prefixDeclaredOnElement)
                element.AddAttribute(Globals.XmlnsPrefix, Globals.XmlnsNamespace, prefix, valueNs);
        }
    }

    internal class ISerializableDataMember
    {
        private string _name;
        private IDataNode _value;

        internal string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        internal IDataNode Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}

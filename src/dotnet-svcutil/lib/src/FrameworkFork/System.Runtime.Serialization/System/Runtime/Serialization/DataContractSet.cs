// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using Microsoft.Xml;
    using DataContractDictionary = System.Collections.Generic.Dictionary<Microsoft.Xml.XmlQualifiedName, DataContract>;

    internal class DataContractSet
    {
        private Dictionary<XmlQualifiedName, DataContract> _contracts;
        private Dictionary<DataContract, object> _processedContracts;
        private IDataContractSurrogate _dataContractSurrogate;
        private Hashtable _surrogateDataTable;
        private DataContractDictionary _knownTypesForObject;
        private ICollection<Type> _referencedTypes;
        private ICollection<Type> _referencedCollectionTypes;
        private Dictionary<XmlQualifiedName, object> _referencedTypesDictionary;
        private Dictionary<XmlQualifiedName, object> _referencedCollectionTypesDictionary;

        internal DataContractSet(IDataContractSurrogate dataContractSurrogate, ICollection<Type> referencedTypes, ICollection<Type> referencedCollectionTypes)
        {
            _dataContractSurrogate = dataContractSurrogate;
            _referencedTypes = referencedTypes;
            _referencedCollectionTypes = referencedCollectionTypes;
        }

        internal DataContractSet(DataContractSet dataContractSet)
        {
            if (dataContractSet == null)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("dataContractSet"));

            _dataContractSurrogate = dataContractSet._dataContractSurrogate;
            _referencedTypes = dataContractSet._referencedTypes;
            _referencedCollectionTypes = dataContractSet._referencedCollectionTypes;

            foreach (KeyValuePair<XmlQualifiedName, DataContract> pair in dataContractSet)
            {
                Add(pair.Key, pair.Value);
            }

            if (dataContractSet._processedContracts != null)
            {
                foreach (KeyValuePair<DataContract, object> pair in dataContractSet._processedContracts)
                {
                    ProcessedContracts.Add(pair.Key, pair.Value);
                }
            }
        }

        private Dictionary<XmlQualifiedName, DataContract> Contracts
        {
            get
            {
                if (_contracts == null)
                {
                    _contracts = new Dictionary<XmlQualifiedName, DataContract>();
                }
                return _contracts;
            }
        }

        private Dictionary<DataContract, object> ProcessedContracts
        {
            get
            {
                if (_processedContracts == null)
                {
                    _processedContracts = new Dictionary<DataContract, object>();
                }
                return _processedContracts;
            }
        }

        private Hashtable SurrogateDataTable
        {
            get
            {
                if (_surrogateDataTable == null)
                    _surrogateDataTable = new Hashtable();
                return _surrogateDataTable;
            }
        }

        internal DataContractDictionary KnownTypesForObject
        {
            get { return _knownTypesForObject; }
            set { _knownTypesForObject = value; }
        }

        internal void Add(Type type)
        {
            DataContract dataContract = GetDataContract(type);
            EnsureTypeNotGeneric(dataContract.UnderlyingType);
            Add(dataContract);
        }

        internal static void EnsureTypeNotGeneric(Type type)
        {
            if (type.GetTypeInfo().ContainsGenericParameters)
                throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(string.Format(SRSerialization.GenericTypeNotExportable, type)));
        }

        private void Add(DataContract dataContract)
        {
            Add(dataContract.StableName, dataContract);
        }

        public void Add(XmlQualifiedName name, DataContract dataContract)
        {
            if (dataContract.IsBuiltInDataContract)
                return;
            InternalAdd(name, dataContract);
        }

        internal void InternalAdd(XmlQualifiedName name, DataContract dataContract)
        {
            DataContract dataContractInSet = null;
            if (Contracts.TryGetValue(name, out dataContractInSet))
            {
                if (!dataContractInSet.Equals(dataContract))
                {
                    if (dataContract.UnderlyingType == null || dataContractInSet.UnderlyingType == null)
                        throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRSerialization.DupContractInDataContractSet, dataContract.StableName.Name, dataContract.StableName.Namespace)));
                    else
                    {
                        bool typeNamesEqual = (DataContract.GetClrTypeFullName(dataContract.UnderlyingType) == DataContract.GetClrTypeFullName(dataContractInSet.UnderlyingType));
                        throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRSerialization.DupTypeContractInDataContractSet, (typeNamesEqual ? dataContract.UnderlyingType.AssemblyQualifiedName : DataContract.GetClrTypeFullName(dataContract.UnderlyingType)), (typeNamesEqual ? dataContractInSet.UnderlyingType.AssemblyQualifiedName : DataContract.GetClrTypeFullName(dataContractInSet.UnderlyingType)), dataContract.StableName.Name, dataContract.StableName.Namespace)));
                    }
                }
            }
            else
            {
                Contracts.Add(name, dataContract);

                if (dataContract is ClassDataContract)
                {
                    AddClassDataContract((ClassDataContract)dataContract);
                }
                else if (dataContract is CollectionDataContract)
                {
                    AddCollectionDataContract((CollectionDataContract)dataContract);
                }
                else if (dataContract is XmlDataContract)
                {
                    AddXmlDataContract((XmlDataContract)dataContract);
                }
            }
        }

        private void AddClassDataContract(ClassDataContract classDataContract)
        {
            if (classDataContract.BaseContract != null)
            {
                Add(classDataContract.BaseContract.StableName, classDataContract.BaseContract);
            }
            if (!classDataContract.IsISerializable)
            {
                if (classDataContract.Members != null)
                {
                    for (int i = 0; i < classDataContract.Members.Count; i++)
                    {
                        DataMember dataMember = classDataContract.Members[i];
                        DataContract memberDataContract = GetMemberTypeDataContract(dataMember);
                        if (_dataContractSurrogate != null && dataMember.MemberInfo != null)
                        {
                            object customData = DataContractSurrogateCaller.GetCustomDataToExport(
                                                   _dataContractSurrogate,
                                                   dataMember.MemberInfo,
                                                   memberDataContract.UnderlyingType);
                            if (customData != null)
                                SurrogateDataTable.Add(dataMember, customData);
                        }
                        Add(memberDataContract.StableName, memberDataContract);
                    }
                }
            }
            AddKnownDataContracts(classDataContract.KnownDataContracts);
        }

        private void AddCollectionDataContract(CollectionDataContract collectionDataContract)
        {
            if (collectionDataContract.IsDictionary)
            {
                ClassDataContract keyValueContract = collectionDataContract.ItemContract as ClassDataContract;
                AddClassDataContract(keyValueContract);
            }
            else
            {
                DataContract itemContract = GetItemTypeDataContract(collectionDataContract);
                if (itemContract != null)
                    Add(itemContract.StableName, itemContract);
            }
            AddKnownDataContracts(collectionDataContract.KnownDataContracts);
        }

        private void AddXmlDataContract(XmlDataContract xmlDataContract)
        {
            AddKnownDataContracts(xmlDataContract.KnownDataContracts);
        }

        private void AddKnownDataContracts(DataContractDictionary knownDataContracts)
        {
            if (knownDataContracts != null)
            {
                foreach (DataContract knownDataContract in knownDataContracts.Values)
                {
                    Add(knownDataContract);
                }
            }
        }

        internal XmlQualifiedName GetStableName(Type clrType)
        {
            if (_dataContractSurrogate != null)
            {
                Type dcType = DataContractSurrogateCaller.GetDataContractType(_dataContractSurrogate, clrType);

                //if (clrType.IsValueType != dcType.IsValueType)
                //    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.ValueTypeMismatchInSurrogatedType, dcType, clrType)));
                return DataContract.GetStableName(dcType);
            }
            return DataContract.GetStableName(clrType);
        }

        internal DataContract GetDataContract(Type clrType)
        {
            if (_dataContractSurrogate == null)
                return DataContract.GetDataContract(clrType);
            DataContract dataContract = DataContract.GetBuiltInDataContract(clrType);
            if (dataContract != null)
                return dataContract;
            Type dcType = DataContractSurrogateCaller.GetDataContractType(_dataContractSurrogate, clrType);
            //if (clrType.IsValueType != dcType.IsValueType)
            //    throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.ValueTypeMismatchInSurrogatedType, dcType, clrType)));
            dataContract = DataContract.GetDataContract(dcType);
            if (!SurrogateDataTable.Contains(dataContract))
            {
                object customData = DataContractSurrogateCaller.GetCustomDataToExport(
                                      _dataContractSurrogate, clrType, dcType);
                if (customData != null)
                    SurrogateDataTable.Add(dataContract, customData);
            }
            return dataContract;
        }

        internal DataContract GetMemberTypeDataContract(DataMember dataMember)
        {
            if (dataMember.MemberInfo != null)
            {
                Type dataMemberType = dataMember.MemberType;
                if (dataMember.IsGetOnlyCollection)
                {
                    if (_dataContractSurrogate != null)
                    {
                        Type dcType = DataContractSurrogateCaller.GetDataContractType(_dataContractSurrogate, dataMemberType);
                        if (dcType != dataMemberType)
                        {
                            throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(string.Format(SRSerialization.SurrogatesWithGetOnlyCollectionsNotSupported,
                                DataContract.GetClrTypeFullName(dataMemberType), DataContract.GetClrTypeFullName(dataMember.MemberInfo.DeclaringType), dataMember.MemberInfo.Name)));
                        }
                    }
                    return DataContract.GetGetOnlyCollectionDataContract(DataContract.GetId(dataMemberType.TypeHandle), dataMemberType.TypeHandle, dataMemberType, SerializationMode.SharedContract);
                }
                else
                {
                    return GetDataContract(dataMemberType);
                }
            }
            return dataMember.MemberTypeContract;
        }

        internal DataContract GetItemTypeDataContract(CollectionDataContract collectionContract)
        {
            if (collectionContract.ItemType != null)
                return GetDataContract(collectionContract.ItemType);
            return collectionContract.ItemContract;
        }

        internal object GetSurrogateData(object key)
        {
            return SurrogateDataTable[key];
        }

        internal void SetSurrogateData(object key, object surrogateData)
        {
            SurrogateDataTable[key] = surrogateData;
        }

        public DataContract this[XmlQualifiedName key]
        {
            get
            {
                DataContract dataContract = DataContract.GetBuiltInDataContract(key.Name, key.Namespace);
                if (dataContract == null)
                {
                    Contracts.TryGetValue(key, out dataContract);
                }
                return dataContract;
            }
        }

        public IDataContractSurrogate DataContractSurrogate
        {
            get { return _dataContractSurrogate; }
        }

        public bool Remove(XmlQualifiedName key)
        {
            if (DataContract.GetBuiltInDataContract(key.Name, key.Namespace) != null)
                return false;
            return Contracts.Remove(key);
        }

        public IEnumerator<KeyValuePair<XmlQualifiedName, DataContract>> GetEnumerator()
        {
            return Contracts.GetEnumerator();
        }

        internal bool IsContractProcessed(DataContract dataContract)
        {
            return ProcessedContracts.ContainsKey(dataContract);
        }

        internal void SetContractProcessed(DataContract dataContract)
        {
            ProcessedContracts.Add(dataContract, dataContract);
        }

        internal ContractCodeDomInfo GetContractCodeDomInfo(DataContract dataContract)
        {
            object info;
            if (ProcessedContracts.TryGetValue(dataContract, out info))
                return (ContractCodeDomInfo)info;
            return null;
        }

        internal void SetContractCodeDomInfo(DataContract dataContract, ContractCodeDomInfo info)
        {
            ProcessedContracts.Add(dataContract, info);
        }
        private Dictionary<XmlQualifiedName, object> GetReferencedTypes()
        {
            if (_referencedTypesDictionary == null)
            {
                _referencedTypesDictionary = new Dictionary<XmlQualifiedName, object>();
                //Always include Nullable as referenced type
                //Do not allow surrogating Nullable<T>
                _referencedTypesDictionary.Add(DataContract.GetStableName(Globals.TypeOfNullable), Globals.TypeOfNullable);
                if (_referencedTypes != null)
                {
                    foreach (Type type in _referencedTypes)
                    {
                        if (type == null)
                            throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRSerialization.ReferencedTypesCannotContainNull)));

                        AddReferencedType(_referencedTypesDictionary, type);
                    }
                }
            }
            return _referencedTypesDictionary;
        }

        private Dictionary<XmlQualifiedName, object> GetReferencedCollectionTypes()
        {
            if (_referencedCollectionTypesDictionary == null)
            {
                _referencedCollectionTypesDictionary = new Dictionary<XmlQualifiedName, object>();
                if (_referencedCollectionTypes != null)
                {
                    foreach (Type type in _referencedCollectionTypes)
                    {
                        if (type == null)
                            throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRSerialization.ReferencedCollectionTypesCannotContainNull)));
                        AddReferencedType(_referencedCollectionTypesDictionary, type);
                    }
                }
                XmlQualifiedName genericDictionaryName = DataContract.GetStableName(Globals.TypeOfDictionaryGeneric);
                if (!_referencedCollectionTypesDictionary.ContainsKey(genericDictionaryName) && GetReferencedTypes().ContainsKey(genericDictionaryName))
                    AddReferencedType(_referencedCollectionTypesDictionary, Globals.TypeOfDictionaryGeneric);
            }
            return _referencedCollectionTypesDictionary;
        }

        private void AddReferencedType(Dictionary<XmlQualifiedName, object> referencedTypes, Type type)
        {
            if (IsTypeReferenceable(type))
            {
                XmlQualifiedName stableName;
                try
                {
                    stableName = this.GetStableName(type);
                }
                catch (InvalidDataContractException)
                {
                    // Type not referenceable if we can't get a stable name.
                    return;
                }
                catch (InvalidOperationException)
                {
                    // Type not referenceable if we can't get a stable name.
                    return;
                }

                object value;
                if (referencedTypes.TryGetValue(stableName, out value))
                {
                    Type referencedType = value as Type;
                    if (referencedType != null)
                    {
                        if (referencedType != type)
                        {
                            referencedTypes.Remove(stableName);
                            List<Type> types = new List<Type>();
                            types.Add(referencedType);
                            types.Add(type);
                            referencedTypes.Add(stableName, types);
                        }
                    }
                    else
                    {
                        List<Type> types = (List<Type>)value;
                        if (!types.Contains(type))
                            types.Add(type);
                    }
                }
                else
                    referencedTypes.Add(stableName, type);
            }
        }

        internal bool TryGetReferencedType(XmlQualifiedName stableName, DataContract dataContract, out Type type)
        {
            return TryGetReferencedType(stableName, dataContract, false/*useReferencedCollectionTypes*/, out type);
        }

        internal bool TryGetReferencedCollectionType(XmlQualifiedName stableName, DataContract dataContract, out Type type)
        {
            return TryGetReferencedType(stableName, dataContract, true/*useReferencedCollectionTypes*/, out type);
        }

        private bool TryGetReferencedType(XmlQualifiedName stableName, DataContract dataContract, bool useReferencedCollectionTypes, out Type type)
        {
            object value;
            Dictionary<XmlQualifiedName, object> referencedTypes = useReferencedCollectionTypes ? GetReferencedCollectionTypes() : GetReferencedTypes();
            if (referencedTypes.TryGetValue(stableName, out value))
            {
                type = value as Type;
                if (type != null)
                    return true;
                else
                {
                    // Throw ambiguous type match exception
                    List<Type> types = (List<Type>)value;
                    StringBuilder errorMessage = new StringBuilder();
                    bool containsGenericType = false;
                    for (int i = 0; i < types.Count; i++)
                    {
                        Type conflictingType = types[i];
                        if (!containsGenericType)
                            containsGenericType = conflictingType.GetTypeInfo().IsGenericTypeDefinition;
                        errorMessage.AppendFormat("{0}\"{1}\" ", Environment.NewLine, conflictingType.AssemblyQualifiedName);
                        if (dataContract != null)
                        {
                            DataContract other = this.GetDataContract(conflictingType);
                            errorMessage.Append(string.Empty); // TODO: SR.Format(((other != null && other.Equals(dataContract)) ? SR.ReferencedTypeMatchingMessage : SR.ReferencedTypeNotMatchingMessage)));
                        }
                    }
                    if (containsGenericType)
                    {
                        throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                            (useReferencedCollectionTypes ? SRSerialization.AmbiguousReferencedCollectionTypes1 : SRSerialization.AmbiguousReferencedTypes1),
                            errorMessage.ToString())));
                    }
                    else
                    {
                        throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                            (useReferencedCollectionTypes ? SRSerialization.AmbiguousReferencedCollectionTypes3 : SRSerialization.AmbiguousReferencedTypes3),
                            XmlConvert.DecodeName(stableName.Name),
                            stableName.Namespace,
                            errorMessage.ToString())));
                    }
                }
            }
            type = null;
            return false;
        }

        private static bool IsTypeReferenceable(Type type)
        {
            Type itemType;

            try
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                return (typeInfo.IsSerializable ||
                        typeInfo.IsAttributeDefined(Globals.TypeOfDataContractAttribute) ||
                        (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type) && !typeInfo.IsGenericTypeDefinition) ||
                        CollectionDataContract.IsCollection(type, out itemType) ||
                        ClassDataContract.IsNonAttributedTypeValidForSerialization(type));
            }
            catch (Exception ex)
            {
                // An exception can be thrown in the designer when a project has a runtime binding redirection for a referenced assembly or a reference dependent assembly.
                // Type.IsDefined is known to throw System.IO.FileLoadException.
                // ClassDataContract.IsNonAttributedTypeValidForSerialization is known to throw System.IO.FileNotFoundException.
                // We guard against all non-critical exceptions.
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
            }

            return false;
        }
    }
}

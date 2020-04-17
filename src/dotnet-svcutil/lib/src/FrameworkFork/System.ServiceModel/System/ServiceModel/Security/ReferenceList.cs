// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System.Collections.Generic;
    using System.IdentityModel;
    using System.Runtime.CompilerServices;
    using Microsoft.Xml;
    using DictionaryManager = System.IdentityModel.DictionaryManager;
    using ISecurityElement = System.IdentityModel.ISecurityElement;

    [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    internal sealed class ReferenceList : ISecurityElement
    {
        internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.ReferenceList;
        private const string NamespacePrefix = XmlEncryptionStrings.Prefix;
        internal static readonly XmlDictionaryString NamespaceUri = EncryptedType.NamespaceUri;
        internal static readonly XmlDictionaryString UriAttribute = XD.XmlEncryptionDictionary.URI;
        private List<string> _referredIds = new List<string>();

        public ReferenceList()
        {
        }

        public int DataReferenceCount
        {
            get { return _referredIds.Count; }
        }

        public bool HasId
        {
            get { return false; }
        }

        public string Id
        {
            get
            {
                // PreSharp Bug: Property get methods should not throw exceptions.
#pragma warning disable 56503
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
        }

        public void AddReferredId(string id)
        {
            if (id == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("id"));
            }
            _referredIds.Add(id);
        }

        public bool ContainsReferredId(string id)
        {
            if (id == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("id"));
            }
            return _referredIds.Contains(id);
        }

        public string GetReferredId(int index)
        {
            return _referredIds[index];
        }

        public void ReadFrom(XmlDictionaryReader reader)
        {
            reader.ReadStartElement(ElementName, NamespaceUri);
            while (reader.IsStartElement())
            {
                string id = DataReference.ReadFrom(reader);
                if (_referredIds.Contains(id))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new SecurityMessageSerializationException(string.Format(SR_IdentityModel.InvalidDataReferenceInReferenceList, "#" + id)));
                }
                _referredIds.Add(id);
            }
            reader.ReadEndElement(); // ReferenceList
            if (this.DataReferenceCount == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityMessageSerializationException(SR_IdentityModel.ReferenceListCannotBeEmpty));
            }
        }

        public bool TryRemoveReferredId(string id)
        {
            if (id == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("id"));
            }
            return _referredIds.Remove(id);
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (this.DataReferenceCount == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR_IdentityModel.ReferenceListCannotBeEmpty));
            }
            writer.WriteStartElement(NamespacePrefix, ElementName, NamespaceUri);
            for (int i = 0; i < this.DataReferenceCount; i++)
            {
                DataReference.WriteTo(writer, _referredIds[i]);
            }
            writer.WriteEndElement(); // ReferenceList
        }

        private static class DataReference
        {
            internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.DataReference;
            internal static readonly XmlDictionaryString NamespaceUri = EncryptedType.NamespaceUri;

            public static string ReadFrom(XmlDictionaryReader reader)
            {
                string prefix;
                string uri = XmlHelper.ReadEmptyElementAndRequiredAttribute(reader, ElementName, NamespaceUri, ReferenceList.UriAttribute, out prefix);
                if (uri.Length < 2 || uri[0] != '#')
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new SecurityMessageSerializationException(string.Format(SR_IdentityModel.InvalidDataReferenceInReferenceList, uri)));
                }
                return uri.Substring(1);
            }

            public static void WriteTo(XmlDictionaryWriter writer, string referredId)
            {
                writer.WriteStartElement(XD.XmlEncryptionDictionary.Prefix.Value, ElementName, NamespaceUri);
                writer.WriteStartAttribute(ReferenceList.UriAttribute, null);
                writer.WriteString("#");
                writer.WriteString(referredId);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
        }
    }
}

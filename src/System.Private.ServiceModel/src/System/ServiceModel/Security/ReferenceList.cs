// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel;
using System.Runtime.CompilerServices;
using System.Xml;
using DictionaryManager = System.IdentityModel.DictionaryManager;
using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    sealed class ReferenceList : ISecurityElement
    {
        internal static readonly XmlDictionaryString s_ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.ReferenceList;
        const string NamespacePrefix = XmlEncryptionStrings.Prefix;
        internal static readonly XmlDictionaryString s_NamespaceUri = EncryptedType.s_NamespaceUri;
        internal static readonly XmlDictionaryString s_UriAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.URI;
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
                #pragma warning suppress 56503
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
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //reader.ReadStartElement(ElementName, NamespaceUri);
            //while (reader.IsStartElement())
            //{
            //    string id = DataReference.ReadFrom(reader);
            //    if (this.referredIds.Contains(id))
            //    {
            //        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
            //            new SecurityMessageSerializationException(SR.Format(SR.InvalidDataReferenceInReferenceList, "#" + id)));
            //    }
            //    this.referredIds.Add(id);
            //}
            //reader.ReadEndElement(); // ReferenceList
            //if (this.DataReferenceCount == 0)
            //{
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityMessageSerializationException(SR.Format(SR.ReferenceListCannotBeEmpty)));
            //}
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
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //if (this.DataReferenceCount == 0)
            //{
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ReferenceListCannotBeEmpty)));
            //}
            //writer.WriteStartElement(NamespacePrefix, ElementName, NamespaceUri);
            //for (int i = 0; i < this.DataReferenceCount; i++)
            //{
            //    DataReference.WriteTo(writer, this.referredIds[i]);
            //}
            //writer.WriteEndElement(); // ReferenceList
        }

        static class DataReference
        {
            internal static readonly XmlDictionaryString s_ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.DataReference;
            internal static readonly XmlDictionaryString s_NamespaceUri = EncryptedType.s_NamespaceUri;

            public static string ReadFrom(XmlDictionaryReader reader)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

                //string prefix;
                //string uri = XmlHelper.ReadEmptyElementAndRequiredAttribute(reader, ElementName, NamespaceUri, ReferenceList.UriAttribute, out prefix);
                //if (uri.Length < 2 || uri[0] != '#')
                //{
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                //        new SecurityMessageSerializationException(SR.Format(SR.InvalidDataReferenceInReferenceList, uri)));
                //}
                //return uri.Substring(1);
            }

            public static void WriteTo(XmlDictionaryWriter writer, string referredId)
            {
                writer.WriteStartElement(System.IdentityModel.XD.XmlEncryptionDictionary.Prefix.Value, s_ElementName, s_NamespaceUri);
                writer.WriteStartAttribute(ReferenceList.s_UriAttribute, null);
                writer.WriteString("#");
                writer.WriteString(referredId);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
        }
    }
}


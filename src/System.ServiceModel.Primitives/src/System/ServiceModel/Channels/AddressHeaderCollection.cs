// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public sealed class AddressHeaderCollection : ReadOnlyCollection<AddressHeader>
    {
        public AddressHeaderCollection()
            : base(new List<AddressHeader>())
        {
        }

        public AddressHeaderCollection(IEnumerable<AddressHeader> addressHeaders)
            : base(new List<AddressHeader>(addressHeaders))
        {
            // avoid allocating an enumerator when possible
            IList<AddressHeader> collection = addressHeaders as IList<AddressHeader>;
            if (collection != null)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i] == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.MessageHeaderIsNull0));
                    }
                }
            }
            else
            {
                foreach (AddressHeader addressHeader in addressHeaders)
                {
                    if (addressHeaders == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.MessageHeaderIsNull0));
                    }
                }
            }
        }

        internal static AddressHeaderCollection EmptyHeaderCollection { get; } = new AddressHeaderCollection();

        private int InternalCount
        {
            get
            {
                if (this == (object)EmptyHeaderCollection)
                {
                    return 0;
                }

                return Count;
            }
        }

        public void AddHeadersTo(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            for (int i = 0; i < InternalCount; i++)
            {
                message.Headers.Add(this[i].ToMessageHeader());
            }
        }

        public AddressHeader[] FindAll(string name, string ns)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(name)));
            }

            if (ns == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(ns)));
            }

            List<AddressHeader> results = new List<AddressHeader>();
            for (int i = 0; i < Count; i++)
            {
                AddressHeader header = this[i];
                if (header.Name == name && header.Namespace == ns)
                {
                    results.Add(header);
                }
            }

            return results.ToArray();
        }

        public AddressHeader FindHeader(string name, string ns)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(name)));
            }

            if (ns == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(ns)));
            }

            AddressHeader matchingHeader = null;

            for (int i = 0; i < Count; i++)
            {
                AddressHeader header = this[i];
                if (header.Name == name && header.Namespace == ns)
                {
                    if (matchingHeader != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.Format(SRP.MultipleMessageHeaders, name, ns)));
                    }

                    matchingHeader = header;
                }
            }

            return matchingHeader;
        }

        internal bool IsEquivalent(AddressHeaderCollection col)
        {
            if (InternalCount != col.InternalCount)
            {
                return false;
            }

            StringBuilder builder = new StringBuilder();
            Dictionary<string, int> myHeaders = new Dictionary<string, int>();
            PopulateHeaderDictionary(builder, myHeaders);

            Dictionary<string, int> otherHeaders = new Dictionary<string, int>();
            col.PopulateHeaderDictionary(builder, otherHeaders);

            if (myHeaders.Count != otherHeaders.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, int> pair in myHeaders)
            {
                int count;
                if (otherHeaders.TryGetValue(pair.Key, out count))
                {
                    if (count != pair.Value)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        internal void PopulateHeaderDictionary(StringBuilder builder, Dictionary<string, int> headers)
        {
            string key;
            for (int i = 0; i < InternalCount; ++i)
            {
                builder.Remove(0, builder.Length);
                key = this[i].GetComparableForm(builder);
                if (headers.ContainsKey(key))
                {
                    headers[key] = headers[key] + 1;
                }
                else
                {
                    headers.Add(key, 1);
                }
            }
        }

        internal static AddressHeaderCollection ReadServiceParameters(XmlDictionaryReader reader)
        {
            return ReadServiceParameters(reader, false);
        }

        internal static AddressHeaderCollection ReadServiceParameters(XmlDictionaryReader reader, bool isReferenceProperty)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return null;
            }
            else
            {
                reader.ReadStartElement();
                List<AddressHeader> headerList = new List<AddressHeader>();
                while (reader.IsStartElement())
                {
                    headerList.Add(new BufferedAddressHeader(reader, isReferenceProperty));
                }
                reader.ReadEndElement();
                return new AddressHeaderCollection(headerList);
            }
        }

        internal bool HasReferenceProperties
        {
            get
            {
                for (int i = 0; i < InternalCount; i++)
                {
                    if (this[i].IsReferenceProperty)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        internal bool HasNonReferenceProperties
        {
            get
            {
                for (int i = 0; i < InternalCount; i++)
                {
                    if (!this[i].IsReferenceProperty)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        internal void WriteReferencePropertyContentsTo(XmlDictionaryWriter writer)
        {
            for (int i = 0; i < InternalCount; i++)
            {
                if (this[i].IsReferenceProperty)
                {
                    this[i].WriteAddressHeader(writer);
                }
            }
        }

        internal void WriteNonReferencePropertyContentsTo(XmlDictionaryWriter writer)
        {
            for (int i = 0; i < InternalCount; i++)
            {
                if (!this[i].IsReferenceProperty)
                {
                    this[i].WriteAddressHeader(writer);
                }
            }
        }

        internal void WriteContentsTo(XmlDictionaryWriter writer)
        {
            for (int i = 0; i < InternalCount; i++)
            {
                this[i].WriteAddressHeader(writer);
            }
        }
    }
}

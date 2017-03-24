// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.ServiceModel
{
    public abstract class EndpointIdentity
    {
        protected EndpointIdentity()
        {
        }

        internal EndpointIdentity(object identityIdentifier)
        {
            IdentityIdentifier = identityIdentifier;
        }


        internal object IdentityIdentifier { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == (object)this)
                return true;

            // as handles null do we need the double null check?
            if (obj == null)
                return false;

            EndpointIdentity otherIdentity = obj as EndpointIdentity;
            if (otherIdentity == null)
                return false;

            throw ExceptionHelper.PlatformNotSupported();
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode() ^ IdentityIdentifier.GetHashCode();
        }

        internal static EndpointIdentity ReadIdentity(XmlDictionaryReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

            EndpointIdentity readIdentity = null;

            reader.MoveToContent();
            if (reader.IsEmptyElement)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.UnexpectedEmptyElementExpectingClaim, XD.AddressingDictionary.Identity.Value, XD.AddressingDictionary.IdentityExtensionNamespace.Value)));

            reader.ReadStartElement(XD.AddressingDictionary.Identity, XD.AddressingDictionary.IdentityExtensionNamespace);

            if (reader.IsStartElement(XD.AddressingDictionary.Spn, XD.AddressingDictionary.IdentityExtensionNamespace))
            {
                readIdentity = new SpnEndpointIdentity(reader.ReadElementString());
            }
            else if (reader.IsStartElement(XD.AddressingDictionary.Upn, XD.AddressingDictionary.IdentityExtensionNamespace))
            {
                readIdentity = new UpnEndpointIdentity(reader.ReadElementString());
            }
            else if (reader.IsStartElement(XD.AddressingDictionary.Dns, XD.AddressingDictionary.IdentityExtensionNamespace))
            {
                readIdentity = new DnsEndpointIdentity(reader.ReadElementString());
            }
            else if (reader.IsStartElement(XD.XmlSignatureDictionary.KeyInfo, XD.XmlSignatureDictionary.Namespace))
            {
                reader.ReadStartElement();
                if (reader.IsStartElement(XD.XmlSignatureDictionary.X509Data, XD.XmlSignatureDictionary.Namespace))
                {
                    readIdentity = new X509CertificateEndpointIdentity(reader);
                }
                else if (reader.IsStartElement(XD.XmlSignatureDictionary.RsaKeyValue, XD.XmlSignatureDictionary.Namespace))
                {
                    throw ExceptionHelper.PlatformNotSupported("EndpointIdentity.ReadIdentity RsaEndpointIdentity is not supported.");
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.UnrecognizedIdentityType, reader.Name, reader.NamespaceURI)));
                }
                reader.ReadEndElement();
            }
            else if (reader.NodeType == XmlNodeType.Element)
            {
                //
                // Something unknown
                // 
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.UnrecognizedIdentityType, reader.Name, reader.NamespaceURI)));
            }
            else
            {
                //
                // EndpointIdentity element is empty or some other invalid xml
                //
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.InvalidIdentityElement));
            }

            reader.ReadEndElement();

            return readIdentity;
        }

        internal void WriteTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writer));

            writer.WriteStartElement(XD.AddressingDictionary.Identity, XD.AddressingDictionary.IdentityExtensionNamespace);

            WriteContentsTo(writer);

            writer.WriteEndElement();
        }

        internal virtual void WriteContentsTo(XmlDictionaryWriter writer)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.UnrecognizedIdentityPropertyType, GetType().ToString())));
        }

        public override string ToString()
        {
            return IdentityIdentifier.ToString();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Globalization;

namespace Microsoft.Xml
{
    using System;

    internal static class XmlExceptionHelper
    {
        private static void ThrowXmlException(XmlDictionaryReader reader, string res)
        {
            ThrowXmlException(reader, res, null);
        }

        static public void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1)
        {
            ThrowXmlException(reader, res, arg1, null);
        }

        private static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1, string arg2)
        {
            ThrowXmlException(reader, res, arg1, arg2, null);
        }

        private static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1, string arg2, string arg3)
        {
            string s = string.Format(res, arg1, arg2, arg3);
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                s += " " + string.Format(SRSerialization.XmlLineInfo, lineInfo.LineNumber, lineInfo.LinePosition);
            }

            throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(s));
        }

        static public void ThrowXmlException(XmlDictionaryReader reader, XmlException exception)
        {
            string s = exception.Message;
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                s += " " + string.Format(SRSerialization.XmlLineInfo, lineInfo.LineNumber, lineInfo.LinePosition);
            }
            throw /*System.Runtime.Serialization.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(s));
        }

        private static string GetName(string prefix, string localName)
        {
            if (prefix.Length == 0)
                return localName;
            else
                return string.Concat(prefix, ":", localName);
        }

        private static string GetWhatWasFound(XmlDictionaryReader reader)
        {
            if (reader.EOF)
                return string.Format(SRSerialization.XmlFoundEndOfFile);
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    return string.Format(SRSerialization.XmlFoundElement, GetName(reader.Prefix, reader.LocalName), reader.NamespaceURI);
                case XmlNodeType.EndElement:
                    return string.Format(SRSerialization.XmlFoundEndElement, GetName(reader.Prefix, reader.LocalName), reader.NamespaceURI);
                case XmlNodeType.Text:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    return string.Format(SRSerialization.XmlFoundText, reader.Value);
                case XmlNodeType.Comment:
                    return string.Format(SRSerialization.XmlFoundComment, reader.Value);
                case XmlNodeType.CDATA:
                    return string.Format(SRSerialization.XmlFoundCData, reader.Value);
            }
            return string.Format(SRSerialization.XmlFoundNodeType, reader.NodeType);
        }

        static public void ThrowStartElementExpected(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlStartElementExpected, GetWhatWasFound(reader));
        }

        static public void ThrowStartElementExpected(XmlDictionaryReader reader, string name)
        {
            ThrowXmlException(reader, SRSerialization.XmlStartElementNameExpected, name, GetWhatWasFound(reader));
        }

        static public void ThrowStartElementExpected(XmlDictionaryReader reader, string localName, string ns)
        {
            ThrowXmlException(reader, SRSerialization.XmlStartElementLocalNameNsExpected, localName, ns, GetWhatWasFound(reader));
        }

        static public void ThrowStartElementExpected(XmlDictionaryReader reader, XmlDictionaryString localName, XmlDictionaryString ns)
        {
            ThrowStartElementExpected(reader, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(ns));
        }

        static public void ThrowFullStartElementExpected(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlFullStartElementExpected, GetWhatWasFound(reader));
        }

        static public void ThrowFullStartElementExpected(XmlDictionaryReader reader, string name)
        {
            ThrowXmlException(reader, SRSerialization.XmlFullStartElementNameExpected, name, GetWhatWasFound(reader));
        }

        static public void ThrowFullStartElementExpected(XmlDictionaryReader reader, string localName, string ns)
        {
            ThrowXmlException(reader, SRSerialization.XmlFullStartElementLocalNameNsExpected, localName, ns, GetWhatWasFound(reader));
        }

        static public void ThrowFullStartElementExpected(XmlDictionaryReader reader, XmlDictionaryString localName, XmlDictionaryString ns)
        {
            ThrowFullStartElementExpected(reader, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(ns));
        }

        static public void ThrowEndElementExpected(XmlDictionaryReader reader, string localName, string ns)
        {
            ThrowXmlException(reader, SRSerialization.XmlEndElementExpected, localName, ns, GetWhatWasFound(reader));
        }

        static public void ThrowMaxArrayLengthExceeded(XmlDictionaryReader reader, int maxArrayLength)
        {
            ThrowXmlException(reader, SRSerialization.XmlMaxArrayLengthExceeded, maxArrayLength.ToString(NumberFormatInfo.CurrentInfo));
        }

        static public void ThrowMaxBytesPerReadExceeded(XmlDictionaryReader reader, int maxBytesPerRead)
        {
            ThrowXmlException(reader, SRSerialization.XmlMaxBytesPerReadExceeded, maxBytesPerRead.ToString(NumberFormatInfo.CurrentInfo));
        }

        static public void ThrowMaxNameTableCharCountExceeded(XmlDictionaryReader reader, int maxNameTableCharCount)
        {
            ThrowXmlException(reader, SRSerialization.XmlMaxNameTableCharCountExceeded, maxNameTableCharCount.ToString(NumberFormatInfo.CurrentInfo));
        }

        static public void ThrowMaxDepthExceeded(XmlDictionaryReader reader, int maxDepth)
        {
            ThrowXmlException(reader, SRSerialization.XmlMaxDepthExceeded, maxDepth.ToString());
        }

        static public void ThrowMaxStringContentLengthExceeded(XmlDictionaryReader reader, int maxStringContentLength)
        {
            ThrowXmlException(reader, SRSerialization.XmlMaxStringContentLengthExceeded, maxStringContentLength.ToString(NumberFormatInfo.CurrentInfo));
        }

        static public void ThrowBase64DataExpected(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlBase64DataExpected, GetWhatWasFound(reader));
        }

        static public void ThrowUndefinedPrefix(XmlDictionaryReader reader, string prefix)
        {
            ThrowXmlException(reader, SRSerialization.XmlUndefinedPrefix, prefix);
        }

        static public void ThrowProcessingInstructionNotSupported(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlProcessingInstructionNotSupported);
        }

        static public void ThrowInvalidXml(XmlDictionaryReader reader, byte b)
        {
            ThrowXmlException(reader, SRSerialization.XmlInvalidXmlByte, b.ToString("X2", CultureInfo.InvariantCulture));
        }

        static public void ThrowUnexpectedEndOfFile(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlUnexpectedEndOfFile, ((XmlBaseReader)reader).GetOpenElements());
        }

        static public void ThrowUnexpectedEndElement(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlUnexpectedEndElement);
        }

        static public void ThrowTokenExpected(XmlDictionaryReader reader, string expected, char found)
        {
            ThrowXmlException(reader, SRSerialization.XmlTokenExpected, expected, found.ToString());
        }

        static public void ThrowTokenExpected(XmlDictionaryReader reader, string expected, string found)
        {
            ThrowXmlException(reader, SRSerialization.XmlTokenExpected, expected, found);
        }

        static public void ThrowInvalidCharRef(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlInvalidCharRef);
        }

        static public void ThrowTagMismatch(XmlDictionaryReader reader, string expectedPrefix, string expectedLocalName, string foundPrefix, string foundLocalName)
        {
            ThrowXmlException(reader, SRSerialization.XmlTagMismatch, GetName(expectedPrefix, expectedLocalName), GetName(foundPrefix, foundLocalName));
        }

        static public void ThrowDuplicateXmlnsAttribute(XmlDictionaryReader reader, string localName, string ns)
        {
            string name;
            if (localName.Length == 0)
                name = "xmlns";
            else
                name = "xmlns:" + localName;
            ThrowXmlException(reader, SRSerialization.XmlDuplicateAttribute, name, name, ns);
        }

        static public void ThrowDuplicateAttribute(XmlDictionaryReader reader, string prefix1, string prefix2, string localName, string ns)
        {
            ThrowXmlException(reader, SRSerialization.XmlDuplicateAttribute, GetName(prefix1, localName), GetName(prefix2, localName), ns);
        }

        static public void ThrowInvalidBinaryFormat(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlInvalidFormat);
        }

        static public void ThrowInvalidRootData(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlInvalidRootData);
        }

        static public void ThrowMultipleRootElements(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlMultipleRootElements);
        }

        static public void ThrowDeclarationNotFirst(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlDeclNotFirst);
        }

        static public void ThrowConversionOverflow(XmlDictionaryReader reader, string value, string type)
        {
            ThrowXmlException(reader, SRSerialization.XmlConversionOverflow, value, type);
        }

        static public void ThrowXmlDictionaryStringIDOutOfRange(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlDictionaryStringIDRange, XmlDictionaryString.MinKey.ToString(NumberFormatInfo.CurrentInfo), XmlDictionaryString.MaxKey.ToString(NumberFormatInfo.CurrentInfo));
        }

        static public void ThrowXmlDictionaryStringIDUndefinedStatic(XmlDictionaryReader reader, int key)
        {
            ThrowXmlException(reader, SRSerialization.XmlDictionaryStringIDUndefinedStatic, key.ToString(NumberFormatInfo.CurrentInfo));
        }

        static public void ThrowXmlDictionaryStringIDUndefinedSession(XmlDictionaryReader reader, int key)
        {
            ThrowXmlException(reader, SRSerialization.XmlDictionaryStringIDUndefinedSession, key.ToString(NumberFormatInfo.CurrentInfo));
        }

        static public void ThrowEmptyNamespace(XmlDictionaryReader reader)
        {
            ThrowXmlException(reader, SRSerialization.XmlEmptyNamespaceRequiresNullPrefix);
        }

        static public XmlException CreateConversionException(string type, Exception exception)
        {
            return new XmlException(string.Format(SRSerialization.XmlInvalidConversionWithoutValue, type), exception);
        }

        static public XmlException CreateConversionException(string value, string type, Exception exception)
        {
            return new XmlException(string.Format(SRSerialization.XmlInvalidConversion, value, type), exception);
        }

        static public XmlException CreateEncodingException(byte[] buffer, int offset, int count, Exception exception)
        {
            return CreateEncodingException(new System.Text.UTF8Encoding(false, false).GetString(buffer, offset, count), exception);
        }

        static public XmlException CreateEncodingException(string value, Exception exception)
        {
            return new XmlException(string.Format(SRSerialization.XmlInvalidUTF8Bytes, value), exception);
        }
    }
}

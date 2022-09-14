// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.ServiceModel;

namespace System.Xml
{
    internal static class XmlExceptionHelper
    {
        static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1)
        {
            ThrowXmlException(reader, res, arg1, null);
        }

        static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1, string arg2)
        {
            ThrowXmlException(reader, res, arg1, arg2, null);
        }

        static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1, string arg2, string arg3)
        {
            string s = SRP.Format(res, arg1, arg2, arg3);
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                s += " " + SRP.Format(SRP.XmlLineInfo, lineInfo.LineNumber, lineInfo.LinePosition);
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(s));
        }

        static public void ThrowMaxStringContentLengthExceeded(XmlDictionaryReader reader, int maxStringContentLength)
        {
            ThrowXmlException(reader, SRP.XmlMaxStringContentLengthExceeded, maxStringContentLength.ToString(NumberFormatInfo.CurrentInfo));
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ServiceModel
{
    public class FaultReasonText
    {
        private string _xmlLang;
        private string _text;

        public FaultReasonText(string text)
        {
            if (text == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("text"));
            _text = text;
            _xmlLang = CultureInfo.CurrentCulture.Name;
        }

        public FaultReasonText(string text, string xmlLang)
        {
            if (text == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("text"));
            if (xmlLang == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("xmlLang"));
            _text = text;
            _xmlLang = xmlLang;
        }

        public FaultReasonText(string text, CultureInfo cultureInfo)
        {
            if (text == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("text"));
            if (cultureInfo == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("cultureInfo"));
            _text = text;
            _xmlLang = cultureInfo.Name;
        }

        public bool Matches(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("cultureInfo"));

            return _xmlLang == cultureInfo.Name;
        }

        public string XmlLang
        {
            get { return _xmlLang; }
        }

        public string Text
        {
            get { return _text; }
        }
    }
}

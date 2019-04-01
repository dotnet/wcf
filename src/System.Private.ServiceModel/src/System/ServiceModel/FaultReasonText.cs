// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;

namespace System.ServiceModel
{
    public class FaultReasonText
    {
        private string _text;

        public FaultReasonText(string text)
        {
            _text = text ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(text)));
            XmlLang = CultureInfo.CurrentCulture.Name;
        }

        public FaultReasonText(string text, string xmlLang)
        {
            _text = text ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(text)));
            XmlLang = xmlLang ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(xmlLang)));
        }

        public FaultReasonText(string text, CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(cultureInfo)));
            }

            _text = text ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(text)));
            XmlLang = cultureInfo.Name;
        }

        public bool Matches(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(cultureInfo)));
            }

            return XmlLang == cultureInfo.Name;
        }

        public string XmlLang { get; }

        public string Text
        {
            get { return _text; }
        }
    }
}

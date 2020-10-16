// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace System.ServiceModel
{
    public class FaultReason
    {
        private SynchronizedReadOnlyCollection<FaultReasonText> _translations;

        public FaultReason(FaultReasonText translation)
        {
            if (translation == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("translation");

            Init(translation);
        }

        public FaultReason(string text)
        {
            // Let FaultReasonText constructor throw
            Init(new FaultReasonText(text));
        }

        internal FaultReason(string text, string xmlLang)
        {
            // Let FaultReasonText constructor throw
            Init(new FaultReasonText(text, xmlLang));
        }

        internal FaultReason(string text, CultureInfo cultureInfo)
        {
            // Let FaultReasonText constructor throw
            Init(new FaultReasonText(text, cultureInfo));
        }

        public FaultReason(IEnumerable<FaultReasonText> translations)
        {
            if (translations == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("translations"));
            int count = 0;
            foreach (FaultReasonText faultReasonText in translations)
                count++;
            if (count == 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRServiceModel.AtLeastOneFaultReasonMustBeSpecified, "translations"));
            FaultReasonText[] array = new FaultReasonText[count];
            int index = 0;
            foreach (FaultReasonText faultReasonText in translations)
            {
                if (faultReasonText == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("translations", SRServiceModel.NoNullTranslations);

                array[index++] = faultReasonText;
            }
            Init(array);
        }

        private void Init(FaultReasonText translation)
        {
            Init(new FaultReasonText[] { translation });
        }

        private void Init(FaultReasonText[] translations)
        {
            _translations = new SynchronizedReadOnlyCollection<FaultReasonText>(new object(), new ReadOnlyCollection<FaultReasonText>(translations));
        }

        public FaultReasonText GetMatchingTranslation()
        {
            return GetMatchingTranslation(CultureInfo.CurrentCulture);
        }

        public FaultReasonText GetMatchingTranslation(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("cultureInfo"));

            // If there's only one translation, use it
            if (_translations.Count == 1)
                return _translations[0];

            // Search for an exact match
            for (int i = 0; i < _translations.Count; i++)
                if (_translations[i].Matches(cultureInfo))
                    return _translations[i];

            // If no exact match is found, proceed by looking for the a translation with a language that is a parent of the current culture

            if (_translations.Count == 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRServiceModel.NoMatchingTranslationFoundForFaultText));

            // Search for a more general language
#pragma warning disable 56506
            string localLang = cultureInfo.Name;
            while (true)
            {
                int idx = localLang.LastIndexOf('-');

                // We don't want to accept xml:lang=""
                if (idx == -1)
                    break;

                // Clip off the last subtag and look for a match
                localLang = localLang.Substring(0, idx);

                for (int i = 0; i < _translations.Count; i++)
                    if (_translations[i].XmlLang == localLang)
                        return _translations[i];
            }

            // Return the first translation if no match is found
            return _translations[0];
        }

        public SynchronizedReadOnlyCollection<FaultReasonText> Translations
        {
            get { return _translations; }
        }

        public override string ToString()
        {
            if (_translations.Count == 0)
                return string.Empty;

            return GetMatchingTranslation().Text;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IdentityModel;
    using System.IdentityModel.Selectors;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.Xml;

    public class SamlSerializer
    {
        private DictionaryManager _dictionaryManager;

        public SamlSerializer()
        {
        }

        // Interface to plug in external Dictionaries. The external
        // dictionary should already be populated with all strings 
        // required by this assembly.
        public void PopulateDictionary(IXmlDictionary dictionary)
        {
            if (dictionary == null)
                throw /*System.ServiceModel.*/DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dictionary");

            _dictionaryManager = new DictionaryManager(dictionary);
        }

        internal DictionaryManager DictionaryManager
        {
            get
            {
                if (_dictionaryManager == null)
                    _dictionaryManager = new DictionaryManager();

                return _dictionaryManager;
            }
        }
    }
}

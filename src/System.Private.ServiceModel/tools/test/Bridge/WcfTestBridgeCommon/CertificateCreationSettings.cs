// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfTestBridgeCommon
{
    [Serializable]
    public class CertificateCreationSettings
    {
        public CertificateCreationSettings()
        {
            Subjects = new string[] { string.Empty };
            ValidityType = CertificateValidityType.Valid;
        }
        public string FriendlyName { get; set; }
        public string [] Subjects { get; set; }
        public DateTime ValidityNotBefore { get; set; }
        public DateTime ValidityNotAfter { get; set; }
        public CertificateValidityType ValidityType { get; set; }
    }
    
    [Serializable]
    public enum CertificateValidityType
    {
        Valid = 0, 
        Expired = 1, 
        Revoked = 2
    }
}

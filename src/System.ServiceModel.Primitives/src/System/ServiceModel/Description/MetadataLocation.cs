// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml.Serialization;

namespace System.ServiceModel.Description
{
    [XmlRoot(ElementName = MetadataStrings.MetadataExchangeStrings.Location, Namespace = MetadataStrings.MetadataExchangeStrings.Namespace)]
    public class MetadataLocation
    {
        private string _location;

        public MetadataLocation()
        {
        }

        public MetadataLocation(string location)
        {
            Location = location;
        }

        [XmlText]
        public string Location
        {
            get { return _location; }
            set
            {
                if (value != null)
                {
                    Uri uri;
                    if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.SFxMetadataReferenceInvalidLocation, value));
                    }
                }

                _location = value;
            }
        }
    }
}

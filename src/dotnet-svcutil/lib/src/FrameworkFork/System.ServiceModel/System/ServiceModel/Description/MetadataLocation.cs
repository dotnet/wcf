// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xml.Serialization;

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
            this.Location = location;
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
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.Format(SRServiceModel.SFxMetadataReferenceInvalidLocation, value));
                }

                _location = value;
            }
        }
    }
}

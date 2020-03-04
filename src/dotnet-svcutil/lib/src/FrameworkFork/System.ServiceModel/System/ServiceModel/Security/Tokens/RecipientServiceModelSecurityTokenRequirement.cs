// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Security.Tokens
{
    public sealed class RecipientServiceModelSecurityTokenRequirement : ServiceModelSecurityTokenRequirement
    {
        public RecipientServiceModelSecurityTokenRequirement()
            : base()
        {
            Properties.Add(IsInitiatorProperty, (object)false);
        }

        public Uri ListenUri
        {
            get
            {
                return GetPropertyOrDefault<Uri>(ListenUriProperty, null);
            }
            set
            {
                this.Properties[ListenUriProperty] = value;
            }
        }

        public override string ToString()
        {
            return InternalToString();
        }
    }
}

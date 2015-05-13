// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

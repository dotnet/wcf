// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ComponentModel;

namespace System.ServiceModel.Security
{
    public enum SecurityTokenAttachmentMode
    {
        Signed,
        Endorsing,
        SignedEndorsing,
        SignedEncrypted
    }

    internal static class SecurityTokenAttachmentModeHelper
    {
        internal static bool IsDefined(SecurityTokenAttachmentMode value)
        {
            return value == SecurityTokenAttachmentMode.Endorsing
                || value == SecurityTokenAttachmentMode.Signed
                || value == SecurityTokenAttachmentMode.SignedEncrypted
                || value == SecurityTokenAttachmentMode.SignedEndorsing;
        }

        internal static void Validate(SecurityTokenAttachmentMode value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SecurityTokenAttachmentMode)));
            }
        }

        internal static void Categorize(SecurityTokenAttachmentMode value,
            out bool isBasic, out bool isSignedButNotBasic, out ReceiveSecurityHeaderBindingModes mode)
        {
            SecurityTokenAttachmentModeHelper.Validate(value);

            switch (value)
            {
                case SecurityTokenAttachmentMode.Endorsing:
                    isBasic = false;
                    isSignedButNotBasic = false;
                    mode = ReceiveSecurityHeaderBindingModes.Endorsing;
                    break;
                case SecurityTokenAttachmentMode.Signed:
                    isBasic = false;
                    isSignedButNotBasic = true;
                    mode = ReceiveSecurityHeaderBindingModes.Signed;
                    break;
                case SecurityTokenAttachmentMode.SignedEncrypted:
                    isBasic = true;
                    isSignedButNotBasic = false;
                    mode = ReceiveSecurityHeaderBindingModes.Basic;
                    break;
                case SecurityTokenAttachmentMode.SignedEndorsing:
                    isBasic = false;
                    isSignedButNotBasic = true;
                    mode = ReceiveSecurityHeaderBindingModes.SignedEndorsing;
                    break;
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
            }
        }
    }
}

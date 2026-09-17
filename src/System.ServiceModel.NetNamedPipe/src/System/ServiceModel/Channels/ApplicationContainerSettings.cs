// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    public sealed class ApplicationContainerSettings
    {
        public const int CurrentSession = ApplicationContainerSettingsDefaults.CurrentSession;
        public const int ServiceSession = ApplicationContainerSettingsDefaults.ServiceSession;
        private const string GroupNameSuffixFormat = ";SessionId={0};PackageFullName={1}";

        private int _sessionId;

        internal ApplicationContainerSettings()
        {
            PackageFullName = ApplicationContainerSettingsDefaults.PackageFullNameDefaultString;
            _sessionId = ApplicationContainerSettingsDefaults.CurrentSession;
        }

        internal ApplicationContainerSettings(ApplicationContainerSettings source)
        {
            PackageFullName = source.PackageFullName;
            _sessionId = source._sessionId;
        }

        public string PackageFullName
        {
            get;
            set;
        }

        public int SessionId
        {
            get => _sessionId;

            set
            {
                // CurrentSession default is -1 and expect the user to set 
                // non-negative windows session Id.
                if (value < ApplicationContainerSettingsDefaults.CurrentSession)
                {
                    throw FxTrace.Exception.Argument(nameof(value), SR.Format(SR.SessionValueInvalid, value));
                }

                _sessionId = value;
            }
        }

        internal bool TargetingAppContainer => !string.IsNullOrEmpty(PackageFullName);

        internal ApplicationContainerSettings Clone() => new ApplicationContainerSettings(this);

        internal string GetConnectionGroupSuffix()
        {
            string suffix = string.Empty;
            if (TargetingAppContainer)
            {
                suffix = string.Format(CultureInfo.InvariantCulture, GroupNameSuffixFormat, SessionId, PackageFullName);
            }

            return suffix;
        }

        internal bool IsMatch(ApplicationContainerSettings applicationContainerSettings)
        {
            if (applicationContainerSettings == null)
            {
                return false;
            }

            if (PackageFullName != applicationContainerSettings.PackageFullName)
            {
                return false;
            }

            if (_sessionId != applicationContainerSettings._sessionId)
            {
                return false;
            }

            return true;
        }
    }
}

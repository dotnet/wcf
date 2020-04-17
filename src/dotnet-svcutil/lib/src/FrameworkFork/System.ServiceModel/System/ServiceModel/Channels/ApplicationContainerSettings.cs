// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    public sealed class ApplicationContainerSettings
    {
        public const int CurrentSession = ApplicationContainerSettingsDefaults.CurrentSession;
        public const int ServiceSession = ApplicationContainerSettingsDefaults.ServiceSession;
        private const string GroupNameSuffixFormat = ";SessionId={0};PackageFullName={1}";

        private int _sessionId;

        internal ApplicationContainerSettings()
        {
            this.PackageFullName = ApplicationContainerSettingsDefaults.PackageFullNameDefaultString;
            _sessionId = ApplicationContainerSettingsDefaults.CurrentSession;
        }

        private ApplicationContainerSettings(ApplicationContainerSettings source)
        {
            this.PackageFullName = source.PackageFullName;
            _sessionId = source._sessionId;
        }

        public string PackageFullName
        {
            get;
            set;
        }

        public int SessionId
        {
            get
            {
                return _sessionId;
            }

            set
            {
                // CurrentSession default is -1 and expect the user to set 
                // non-negative windows session Id.
                if (value < ApplicationContainerSettingsDefaults.CurrentSession)
                {
                    throw FxTrace.Exception.Argument("value", string.Format(SRServiceModel.SessionValueInvalid, value));
                }

                _sessionId = value;
            }
        }

        internal bool TargetingAppContainer
        {
            get
            {
                return !string.IsNullOrEmpty(this.PackageFullName);
            }
        }

        internal ApplicationContainerSettings Clone()
        {
            return new ApplicationContainerSettings(this);
        }

        internal bool IsMatch(ApplicationContainerSettings applicationContainerSettings)
        {
            if (applicationContainerSettings == null)
            {
                return false;
            }

            if (this.PackageFullName != applicationContainerSettings.PackageFullName)
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

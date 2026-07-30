// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    [SupportedOSPlatform("windows")]
    public sealed class NamedPipeSettings
    {
        internal NamedPipeSettings() => ApplicationContainerSettings = new ApplicationContainerSettings();

        internal NamedPipeSettings(NamedPipeSettings elementToBeCloned)
        {
            if (elementToBeCloned.ApplicationContainerSettings != null)
            {
                ApplicationContainerSettings = elementToBeCloned.ApplicationContainerSettings.Clone();
            }
        }

        public ApplicationContainerSettings ApplicationContainerSettings
        {
            get;
            private set;
        }

        internal NamedPipeSettings Clone() => new NamedPipeSettings(this);

        internal bool IsMatch(NamedPipeSettings pipeSettings)
        {
            if (pipeSettings == null)
            {
                return false;
            }

            if (!ApplicationContainerSettings.IsMatch(pipeSettings.ApplicationContainerSettings))
            {
                return false;
            }

            return true;
        }
    }
}

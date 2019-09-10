// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    public sealed class NamedPipeSettings
    {
        internal NamedPipeSettings()
        {
            this.ApplicationContainerSettings = new ApplicationContainerSettings();
        }

        private NamedPipeSettings(NamedPipeSettings elementToBeCloned)
        {
            if (elementToBeCloned.ApplicationContainerSettings != null)
            {
                this.ApplicationContainerSettings = elementToBeCloned.ApplicationContainerSettings.Clone();
            }
        }

        public ApplicationContainerSettings ApplicationContainerSettings
        {
            get;
            private set;
        }

        internal NamedPipeSettings Clone()
        {
            return new NamedPipeSettings(this);
        }

        internal bool IsMatch(NamedPipeSettings pipeSettings)
        {
            if (pipeSettings == null)
            {
                return false;
            }

            if (!this.ApplicationContainerSettings.IsMatch(pipeSettings.ApplicationContainerSettings))
            {
                return false;
            }

            return true;
        }
    }
}

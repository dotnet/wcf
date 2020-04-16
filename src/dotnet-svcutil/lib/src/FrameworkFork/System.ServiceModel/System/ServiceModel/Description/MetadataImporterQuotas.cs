// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    public sealed class MetadataImporterQuotas
    {
        private const int DefaultMaxPolicyConversionContexts = 32;
        private const int DefaultMaxPolicyNodes = 4096;
        private const int DefaultMaxPolicyAssertions = 1024;
        private const int DefaultMaxYields = 1024;

        private int _maxPolicyConversionContexts;
        private int _maxPolicyNodes;
        private int _maxPolicyAssertions;
        private int _maxYields;

        public MetadataImporterQuotas()
        {
            _maxYields = DefaultMaxYields;
        }

        public static MetadataImporterQuotas Defaults
        {
            get
            {
                return CreateDefaultSettings();
            }
        }
        public static MetadataImporterQuotas Max
        {
            get
            {
                return CreateMaxSettings();
            }
        }

        internal int MaxPolicyConversionContexts
        {
            get { return _maxPolicyConversionContexts; }
            set { _maxPolicyConversionContexts = value; }
        }
        internal int MaxPolicyNodes
        {
            get { return _maxPolicyNodes; }
            set { _maxPolicyNodes = value; }
        }
        internal int MaxPolicyAssertions
        {
            get { return _maxPolicyAssertions; }
            set { _maxPolicyAssertions = value; }
        }

        internal int MaxYields
        {
            get { return _maxYields; }
            set { _maxYields = value; }
        }

        private static MetadataImporterQuotas CreateDefaultSettings()
        {
            MetadataImporterQuotas settings = new MetadataImporterQuotas();
            settings._maxPolicyConversionContexts = DefaultMaxPolicyConversionContexts;
            settings._maxPolicyNodes = DefaultMaxPolicyNodes;
            settings._maxPolicyAssertions = DefaultMaxPolicyAssertions;

            return settings;
        }
        private static MetadataImporterQuotas CreateMaxSettings()
        {
            MetadataImporterQuotas settings = new MetadataImporterQuotas();
            settings._maxPolicyConversionContexts = DefaultMaxPolicyConversionContexts;
            settings._maxPolicyNodes = int.MaxValue;
            settings._maxPolicyAssertions = int.MaxValue;

            return settings;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class SvcutilOptions : UpdateOptions
    {
        #region option keys
        public const string BootstrapPathKey = "bootstrapPath";
        public const string CultureInfoKey = "cultureName";
        public const string EnableLoggingMarkupKey = "enableLoggingMarkup";
        public const string HelpKey = "help";
        public const string NoBootstrappingKey = "noBootstrapping";
        public const string NoLogoKey = "noLogo";
        public const string NoProjectUpdatesKey = "noProjectUpdates";
        public const string NoTelemetryKey = "noTelemetry";
        public const string OutputDirKey = "outputDir";
        public const string ProjectFileKey = "projectFile";
        public const string ToolContextKey = "toolContext";
        public const string VerbosityKey = "verbosity";
        public const string AccecptCertificateKey = "acceptCertificate";
        public const string ServiceContractKey = "serviceContract";
        #endregion

        #region Properties
        public DirectoryInfo BootstrapPath { get { return GetValue<DirectoryInfo>(BootstrapPathKey); } set { SetValue(BootstrapPathKey, value); } }
        public CultureInfo CultureInfo { get { return GetValue<CultureInfo>(CultureInfoKey); } set { SetValue(CultureInfoKey, value); } }
        public bool? EnableLoggingMarkup { get { return GetValue<bool?>(EnableLoggingMarkupKey); } set { SetValue(EnableLoggingMarkupKey, value); } }
        public bool? Help { get { return GetValue<bool>(HelpKey); } set { SetValue(HelpKey, value); } }
        public bool? NoBootstrapping { get { return GetValue<bool?>(NoBootstrappingKey); } set { SetValue(NoBootstrappingKey, value); } }
        public bool? NoLogo { get { return GetValue<bool?>(NoLogoKey); } set { SetValue(NoLogoKey, value); } }
        public bool? NoProjectUpdates { get { return GetValue<bool?>(NoProjectUpdatesKey); } set { SetValue(NoProjectUpdatesKey, value); } }
        public bool? NoTelemetry { get { return GetValue<bool?>(NoTelemetryKey); } set { SetValue(NoTelemetryKey, value); } }
        public DirectoryInfo OutputDir { get { return GetValue<DirectoryInfo>(OutputDirKey); } set { SetValue(OutputDirKey, value); } }
        public MSBuildProj Project { get { return GetValue<MSBuildProj>(ProjectFileKey); } set { SetValue(ProjectFileKey, value); } }
        public OperationalContext? ToolContext { get { return GetValue<OperationalContext?>(ToolContextKey); } set { SetValue(ToolContextKey, value); } }
        public Verbosity? Verbosity { get { return GetValue<Verbosity?>(VerbosityKey); } set { SetValue(VerbosityKey, value); } }
        public bool? AcceptCert { get { return GetValue<bool?>(AccecptCertificateKey); } set { SetValue(AccecptCertificateKey, value); } }
        public bool? ServiceContract { get { return GetValue<bool?>(ServiceContractKey); } set { SetValue(ServiceContractKey, value); } }
        #endregion

        public SvcutilOptions()
        {
            RegisterOptions(
                new SingleValueOption<DirectoryInfo>(BootstrapPathKey),
                new SingleValueOption<CultureInfo>(CultureInfoKey),
                new SingleValueOption<bool>(EnableLoggingMarkupKey),
                new SingleValueOption<bool>(HelpKey),
                new SingleValueOption<bool>(NoBootstrappingKey),
                new SingleValueOption<bool>(NoLogoKey),
                new SingleValueOption<bool>(NoProjectUpdatesKey),
                new SingleValueOption<bool>(NoTelemetryKey),
                new SingleValueOption<DirectoryInfo>(OutputDirKey, OutputDirKey),
                new SingleValueOption<MSBuildProj>(ProjectFileKey, ProjectFileKey),
                new SingleValueOption<OperationalContext>(ToolContextKey),
                new SingleValueOption<Verbosity>(VerbosityKey) { DefaultValue = Svcutil.Verbosity.Normal },
                new SingleValueOption<bool>(AccecptCertificateKey),
                new SingleValueOption<bool>(ServiceContractKey));
        }

        public static new SvcutilOptions FromFile(string filePath, bool throwOnError = true)
        {
            return FromFile<SvcutilOptions>(filePath, throwOnError);
        }

        public static new SvcutilOptions FromJson(string jsonText, bool throwOnError = true)
        {
            return FromJson<SvcutilOptions>(jsonText, throwOnError);
        }
    }
}

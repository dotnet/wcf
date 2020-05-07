// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal partial class UpdateOptions : ApplicationOptions
    {
        #region option keys
        public const string CollectionTypesKey = "collectionType";
        public const string EnableDataBindingKey = "enableDataBinding";
        public const string ExcludeTypesKey = "excludeType";
        public const string InternalTypeAccessKey = "internal";
        public const string MessageContractKey = "messageContract";
        public const string NamespaceMappingsKey = "namespace";
        public const string NoStandardLibraryKey = "noStdLib";
        public const string OutputFileKey = "outputFile";
        public const string ReferencesKey = "reference";
        public const string RuntimeIdentifierKey = "runtimeIdentifier";
        public const string SerializerModeKey = "serializer";
        public const string SyncKey = "sync";
        public const string TargetFrameworkKey = "targetFramework";
        public const string TypeReuseModeKey = "typeReuseMode";
        public const string WrappedKey = "wrapped";
        #endregion

        #region properties
        public ListValue<string> CollectionTypes { get { return GetValue<ListValue<string>>(CollectionTypesKey); } }
        public bool? EnableDataBinding { get { return GetValue<bool?>(EnableDataBindingKey); } set { SetValue(EnableDataBindingKey, value); } }
        public ListValue<string> ExcludeTypes { get { return GetValue<ListValue<string>>(ExcludeTypesKey); } }
        public bool? InternalTypeAccess { get { return GetValue<bool?>(InternalTypeAccessKey); } set { SetValue(InternalTypeAccessKey, value); } }
        public bool? MessageContract { get { return GetValue<bool?>(MessageContractKey); } set { SetValue(MessageContractKey, value); } }
        public ListValue<KeyValuePair<string, string>> NamespaceMappings { get { return GetValue<ListValue<KeyValuePair<string, string>>>(NamespaceMappingsKey); } }
        public bool? NoStandardLibrary { get { return GetValue<bool?>(NoStandardLibraryKey); } set { SetValue(NoStandardLibraryKey, value); } }
        public FileInfo OutputFile { get { return GetValue<FileInfo>(OutputFileKey); } set { SetValue(OutputFileKey, value); } }
        public ListValue<ProjectDependency> References { get { return GetValue<ListValue<ProjectDependency>>(ReferencesKey); } }
        public string RuntimeIdentifier { get { return GetValue<string>(RuntimeIdentifierKey); } set { SetValue(RuntimeIdentifierKey, value); } }
        public SerializerMode? SerializerMode { get { return GetValue<SerializerMode?>(SerializerModeKey); } set { SetValue(SerializerModeKey, value); } }
        public bool? Sync { get { return GetValue<bool?>(SyncKey); } set { SetValue(SyncKey, value); } }
        public FrameworkInfo TargetFramework { get { return GetValue<FrameworkInfo>(TargetFrameworkKey); } set { SetValue(TargetFrameworkKey, value); } }
        public TypeReuseMode? TypeReuseMode { get { return GetValue<TypeReuseMode?>(TypeReuseModeKey); } set { SetValue(TypeReuseModeKey, value); } }
        public bool? Wrapped { get { return GetValue<bool?>(WrappedKey); } set { SetValue(WrappedKey, value); } }
        #endregion

        public UpdateOptions()
        {
            RegisterOptions(
                new ListValueOption<string>(CollectionTypesKey) { SerializationName = "collectionTypes" },
                new SingleValueOption<bool>(EnableDataBindingKey),
                new ListValueOption<string>(ExcludeTypesKey) { SerializationName = "excludeTypes" },
                new SingleValueOption<bool>(InternalTypeAccessKey) { SerializationName = "internalTypeAccess" },
                new SingleValueOption<bool>(MessageContractKey),
                new ListValueOption<KeyValuePair<string, string>>(NamespaceMappingsKey) { SerializationName = "namespaceMappings" },
                new SingleValueOption<bool>(NoStandardLibraryKey) { SerializationName = "noStandardLibrary" },
                new SingleValueOption<FileInfo>(OutputFileKey),
                new ListValueOption<ProjectDependency>(ReferencesKey) { SerializationName = "references" },
                new SingleValueOption<string>(RuntimeIdentifierKey),
                new SingleValueOption<SerializerMode>(SerializerModeKey) { SerializationName = "serializerMode", DefaultValue = Svcutil.SerializerMode.Default },
                new SingleValueOption<bool>(SyncKey) { SerializationName = "sync" },
                new SingleValueOption<FrameworkInfo>(TargetFrameworkKey),
                new SingleValueOption<TypeReuseMode>(TypeReuseModeKey),
                new SingleValueOption<bool>(WrappedKey));
        }

        public static UpdateOptions FromFile(string filePath, bool throwOnError = true)
        {
            return FromFile<UpdateOptions>(filePath, throwOnError);
        }

        public static UpdateOptions FromJson(string jsonText, bool throwOnError = true)
        {
            return FromJson<UpdateOptions>(jsonText, throwOnError);
        }

        /// <summary>
        /// The web service reference update options are serialized into a permanent file which can be committed into a source repository,
        /// Make paths relatives to the specified directory representing where the options file will be located.
        /// </summary>
        public void MakePathsRelativeTo(DirectoryInfo optionsFileDirectory)
        {
            if (this.OutputFile != null && PathHelper.GetRelativePath(this.OutputFile.OriginalPath(), optionsFileDirectory, out var relPath))
            {
                this.OutputFile = new FileInfo(relPath);
            }

            // Update inputs
            for (int idx = 0; idx < this.Inputs.Count; idx++)
            {
                var input = this.Inputs[idx];
                if (input.IsAbsoluteUri && input.IsFile && PathHelper.GetRelativePath(input.LocalPath, optionsFileDirectory, out relPath))
                {
                    this.Inputs[idx] = new Uri(relPath, UriKind.Relative);
                }
            }
        }

        public void ResolveFullPathsFrom(DirectoryInfo optionsFileDirectory)
        {
            if (this.OutputFile != null && !Path.IsPathRooted(this.OutputFile.OriginalPath()))
            {
                this.OutputFile = new FileInfo(Path.Combine(optionsFileDirectory.FullName, this.OutputFile.OriginalPath()));
            }

            // Update inputs
            for (int idx = 0; idx < this.Inputs.Count; idx++)
            {
                var input = this.Inputs[idx];

                if (!input.IsAbsoluteUri && PathHelper.IsFile(input, optionsFileDirectory.FullName, out var fileUri))
                {
                    this.Inputs[idx] = fileUri;
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Represents the options as formatted by the WCF Connected Service.
    /// This class maps the WCF CS options into update options.
    /// </summary>
    internal class WCFCSUpdateOptions : UpdateOptions
    {
        #region option keys
        public const string CollectionTypeReferenceKey = "CollectionTypeReference";
        public const string DictionaryCollectionTypeReferenceKey = "DictionaryCollectionTypeReference";
        public const string ReuseTypesinReferencedAssembliesKey = "ReuseTypesinReferencedAssemblies";
        public const string ReuseTypesinAllReferencedAssembliesKey = "ReuseTypesinAllReferencedAssemblies";
        #endregion

        #region properties (private deserialization-only properties that resolve to base's properties).
        private ListValue<string> CollectionTypeReference { get { return GetValue<ListValue<string>>(CollectionTypeReferenceKey); } }
        private ListValue<string> DictionaryCollectionTypeReference { get { return GetValue<ListValue<string>>(DictionaryCollectionTypeReferenceKey); } }
        private bool? ReuseTypesinReferencedAssemblies { get { return GetValue<bool?>(ReuseTypesinReferencedAssembliesKey); } }
        private bool? ReuseTypesinAllReferencedAssemblies { get { return GetValue<bool?>(ReuseTypesinAllReferencedAssembliesKey); } }
        #endregion

        private List<string> _deserializedCollectionAssemblies = new List<string>();

        public WCFCSUpdateOptions()
        {
            RegisterOptions(
                new ListValueOption<string>(CollectionTypeReferenceKey) { CanSerialize = false },
                new ListValueOption<string>(DictionaryCollectionTypeReferenceKey) { CanSerialize = false },
                new SingleValueOption<bool>(ReuseTypesinReferencedAssembliesKey) { CanSerialize = false },
                new SingleValueOption<bool>(ReuseTypesinAllReferencedAssembliesKey) { CanSerialize = false });

            GetOption(OptionsKey).SerializationName = "ExtendedData";
            GetOption(InputsKey).Aliases.Add("Uri");
            GetOption(MessageContractKey).Aliases.Add("GenerateMessageContract");

            var internalOption = GetOption(InternalTypeAccessKey);
            internalOption.Aliases.Add("SelectedAccessLevelForGeneratedClass");
            internalOption.Deserializing += this.OnSelectedAccessLevelForGeneratedClassDeserializing;

            var namespaceOption = GetOption(NamespaceMappingsKey);
            namespaceOption.Deserializing += this.OnNamespaceOptionDeserializing;

            var collectionTypeRefOption = GetOption(CollectionTypeReferenceKey);
            collectionTypeRefOption.Deserializing += this.OnCollectionTypeDeserializing;

            var dictionaryCollectionTypeRefOption = GetOption(DictionaryCollectionTypeReferenceKey);
            dictionaryCollectionTypeRefOption.Deserializing += this.OnCollectionTypeDeserializing;
        }

        public static new WCFCSUpdateOptions FromFile(string filePath, bool throwOnError = true)
        {
            return FromFile<WCFCSUpdateOptions>(filePath, throwOnError);
        }

        public static new WCFCSUpdateOptions FromJson(string jsonText, bool throwOnError = true)
        {
            return FromJson<WCFCSUpdateOptions>(jsonText, throwOnError);
        }

        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            // Synchronize base options (VS2017 format).

            if (this.ReuseTypesinReferencedAssemblies == false)
            {
                this.TypeReuseMode = Svcutil.TypeReuseMode.None;
            }
            else if (this.ReuseTypesinAllReferencedAssemblies.HasValue)
            {
                this.TypeReuseMode = this.ReuseTypesinAllReferencedAssemblies == false ?
                    Svcutil.TypeReuseMode.Specified : Svcutil.TypeReuseMode.All;
            }

            if (this.CollectionTypeReference.Count > 0)
            {
                this.CollectionTypes.AddRange(this.CollectionTypeReference);
            }

            if (this.DictionaryCollectionTypeReference.Count > 0)
            {
                this.CollectionTypes.AddRange(this.DictionaryCollectionTypeReference);
            }

            // The collections supported by the WCF CS are all part of .NET Core.
            foreach (var packageName in _deserializedCollectionAssemblies.Select(an => ProjectDependency.FromPackage(an, ProjectDependency.NetCoreAppPackageID, "*")))
            {
                if (!this.References.Contains(packageName))
                {
                    this.References.Add(packageName);
                }
            }
        }

        private void OnCollectionTypeDeserializing(object sender, OptionDeserializingEventArgs e)
        {
            // Alias for CollectionTypes option, translate to collection of string ...

            var jToken = e.JToken;
            if (jToken.Type == JTokenType.Object)
            {
                // This must be an object with two properties like below: we need separate the collection type and the type assembly.
                // "CollectionTypeReference": {"Item1": "System.Collections.ArrayList", "Item2": "System.Collections.NonGeneric.dll" }
                // "CollectionType": "System.Collections.ArrayList"
                // "Reference": "System.Collections.NonGeneric, {Microsoft.NETCore.App, *}"

                var properties = jToken.Value<JObject>().Properties();
                var item1 = properties.FirstOrDefault(p => p.Name == "Item1")?.Value.Value<string>();
                if (item1 != null)
                {
                    e.Value = new List<object> { item1.Trim() };

                    var item2 = properties.FirstOrDefault(p => p.Name == "Item2")?.Value.Value<string>();
                    if (item2 != null)
                    {
                        _deserializedCollectionAssemblies.Add(System.IO.Path.GetFileNameWithoutExtension(item2.Trim()));
                    }
                }
            }
        }

        private void OnSelectedAccessLevelForGeneratedClassDeserializing(object sender, OptionDeserializingEventArgs e)
        {
            // Alias for InternalTypeAccess option, translate to bool ...

            var jToken = e.JToken;
            if (jToken.Type == JTokenType.String)
            {
                var value = jToken.Value<string>();
                switch (value.ToLowerInvariant())
                {
                    case "internal":
                        e.Value = true;
                        break;
                    case "public":
                        e.Value = false;
                        break;
                }
            }
        }

        private void OnNamespaceOptionDeserializing(object sender, OptionDeserializingEventArgs e)
        {
            // Alias for NamespaceMappings option, translate to list of strings that can be parsed into a key/value pairs ...

            var jToken = e.JToken;
            if (jToken.Type == JTokenType.String)
            {
                e.Value = new List<object> { ParseNamespace(jToken.Value<string>(), sender as OptionBase) };
            }
        }

        private object ParseNamespace(string stringValue, OptionBase option)
        {
            // format namespace as a mapping key/value pair:
            // "Namespace": "MyServiceReference1"
            var parts = stringValue.Split(',');
            OptionValueParser.ThrowInvalidValueIf(parts.Length > 2, stringValue, option);

            var valueKey = parts.Length == 1 ? "*" : parts[0];
            var valueVal = parts.Length == 1 ? parts[0] : parts[1];
            var value = new KeyValuePair<string, string>(valueKey.Trim(), valueVal.Trim());
            return value;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// This is the options container OM base class and it defines the basic json schema (see options serializer class).
    /// Provides base serialization and option handling functionality.
    /// </summary>
    internal partial class ApplicationOptions : IOptionsSerializationHandler
    {
        // dictionary of options name/object, kept sorted for serialization purposes and to ease of troubleshooting.
        private SortedDictionary<string, OptionBase> PropertyBag { get; set; }

        // a list of value-parsing warngins (used mainly for deserialization)
        private List<string> _warnings { get; set; } = new List<string>();
        public IEnumerable<string> Warnings { get { return this._warnings; } }

        // a list of value-parsing errors (used mainly for deserialization)
        private List<Exception> _errors { get; set; } = new List<Exception>();
        public IEnumerable<Exception> Errors { get { return this._errors; } }

        public virtual string Json { get { return Serialize<ApplicationOptions, OptionsSerializer<ApplicationOptions>>(); } }

        #region option keys
        // basic json schema (see options serializer).
        public const string InputsKey = "inputs";
        public const string OptionsKey = "options";
        public const string ProviderIdKey = "providerId";
        public const string VersionKey = "version";
        #endregion

        #region properties
        public ListValue<Uri> Inputs { get { return GetValue<ListValue<Uri>>(InputsKey); } }
        private string Options { get { return GetValue<string>(OptionsKey); } set { SetValue(OptionsKey, value); } }
        public string ProviderId { get { return GetValue<string>(ProviderIdKey); } set { SetValue(ProviderIdKey, value); } }
        public string Version { get { return GetValue<string>(VersionKey); } set { SetValue(VersionKey, value); } }
        #endregion

        #region construction methods.
        public ApplicationOptions()
        {
            this.PropertyBag = new SortedDictionary<string, OptionBase>();

            // base options implementing the JSON schema.
            RegisterOptions(
                new ListValueOption<Uri>(InputsKey),
                new SingleValueOption<string>(OptionsKey) { CanSerialize = false },
                new SingleValueOption<string>(ProviderIdKey),
                new SingleValueOption<string>(VersionKey));
        }

        /// <summary>
        /// Options registration method.  
        /// This is the way a derived options class declare what options it cares about.
        /// This mechanism works well with the property bag and the serialization and cloning of options so that the container works only with the registered options 
        /// </summary>
        /// <param name="options"></param>
        protected void RegisterOptions(params OptionBase[] options)
        {
            foreach (var option in options)
            {
                var existingOption = GetOption(option.Name, throwOnMissing: false);
                if (existingOption != null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorOptionAlreadyRegisteredFormat, option.Name));
                }
                this.PropertyBag[option.Name] = option;
            }
        }

        /// <summary>
        /// Copy option values between different options container types.
        /// Observe that only the options that have been set in the source container are considered (see GetOptions), and only the registered options in the destination container are copied (see GetOption).
        /// </summary>
        /// <typeparam name="TOptionsBase"></typeparam>
        /// <param name="other"></param>
        public void CopyTo<TOptionsBase>(TOptionsBase other) where TOptionsBase : ApplicationOptions
        {
            foreach (var thisOption in this.GetOptions())
            {
                var otherOption = other.GetOption(thisOption.Name, throwOnMissing: false);
                if (otherOption != null)
                {
                    thisOption.CopyTo(otherOption);
                }
            }
        }

        public TOptionsBase CloneAs<TOptionsBase>() where TOptionsBase : ApplicationOptions, new()
        {
            var options = new TOptionsBase();
            this.CopyTo(options);
            return options;
        }
        #endregion

        #region serialization methods and events.
        protected string Serialize<TOptionsBase, TSerializer>() where TOptionsBase : ApplicationOptions, new()
                                                                where TSerializer : OptionsSerializer<TOptionsBase>, new()
        {
            var serializer = new TSerializer();
            string jsonText = JsonConvert.SerializeObject(this, Formatting.Indented, serializer);
            return jsonText;
        }

        protected static TOptionsBase Deserialize<TOptionsBase, TSerializer>(FileInfo fileInfo, bool throwOnError = true)
            where TOptionsBase : ApplicationOptions, new()
            where TSerializer : OptionsSerializer<TOptionsBase>, new()
        {
            return Deserialize<TOptionsBase, TSerializer>(null, fileInfo, throwOnError);
        }

        protected static TOptionsBase Deserialize<TOptionsBase, TSerializer>(string jsonText, bool throwOnError = true)
            where TOptionsBase : ApplicationOptions, new()
            where TSerializer : OptionsSerializer<TOptionsBase>, new()
        {
            return Deserialize<TOptionsBase, TSerializer>(jsonText, null, throwOnError);
        }

        private static TOptionsBase Deserialize<TOptionsBase, TSerializer>(string jsonText, FileInfo fileInfo, bool throwOnError = true)
            where TOptionsBase : ApplicationOptions, new()
            where TSerializer : OptionsSerializer<TOptionsBase>, new()
        {
            TOptionsBase options = new TOptionsBase();

            try
            {
                if (string.IsNullOrEmpty(jsonText))
                {
                    jsonText = File.ReadAllText(fileInfo.FullName);
                    fileInfo = null;
                }
                var serializer = new TSerializer();
                options = JsonConvert.DeserializeObject<TOptionsBase>(jsonText, serializer);
            }
            catch (Exception ex)
            {
                if (Utils.IsFatalOrUnexpected(ex)) throw;
                options._errors.Add(ex);
            }

            if (options._errors.Count > 0 && throwOnError)
            {
                var ex = new AggregateException(options.Errors);
                throw ex;
            }

            return options;
        }

        protected static TOptionsBase FromFile<TOptionsBase>(string filePath, bool throwOnError = true) where TOptionsBase : ApplicationOptions, new()
        {
            return Deserialize<TOptionsBase, OptionsSerializer<TOptionsBase>>(null, new FileInfo(filePath), throwOnError);
        }

        protected static TOptionsBase FromJson<TOptionsBase>(string jsonText, bool throwOnError = true) where TOptionsBase : ApplicationOptions, new()
        {
            return Deserialize<TOptionsBase, OptionsSerializer<TOptionsBase>>(jsonText, null, throwOnError);
        }

        public void Save(string filePath)
        {
            File.WriteAllText(filePath, this.Json);
        }

        protected virtual void OnBeforeSerialize() { }
        protected virtual void OnAfterSerialize() { }
        protected virtual void OnBeforeDeserialize() { ResetOptions(); }
        protected virtual void OnAfterDeserialize() { }

        void IOptionsSerializationHandler.RaiseBeforeSerializeEvent() { OnBeforeSerialize(); }
        void IOptionsSerializationHandler.RaiseAfterSerializeEvent() { OnAfterSerialize(); }
        void IOptionsSerializationHandler.RaiseBeforeDeserializeEvent() { OnBeforeDeserialize(); }
        void IOptionsSerializationHandler.RaiseAfterDeserializeEvent() { OnAfterDeserialize(); }
        #endregion

        #region options/values get/set
        public OptionBase GetOption(string optionId, bool throwOnMissing = true)
        {
            return GetOption<OptionBase>(optionId, throwOnMissing);
        }

        /// <summary>
        /// Returns the option requested by the option's ID, which can be the option's name or any of the option's aliases.
        /// The option type is casted to the specified generic type.
        /// </summary>
        public TOptionBase GetOption<TOptionBase>(string optionId, bool throwOnMissing = true) where TOptionBase : OptionBase
        {
            var option = this.PropertyBag.ContainsKey(optionId) ?
                this.PropertyBag[optionId] :
                this.PropertyBag.Values.FirstOrDefault(v => v.HasSameId(optionId));

            if (option == null && throwOnMissing)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.WarningUnrecognizedOptionFormat, optionId));
            }

            return (TOptionBase)option;
        }

        public bool TryGetOption(string optionId, out OptionBase option)
        {
            option = GetOption(optionId, throwOnMissing: false);
            return option != null;
        }

        /// <summary>
        /// Return the options that have been set.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OptionBase> GetOptions(bool allOptions = false)
        {
            var requestedOptions = this.PropertyBag.Where(p =>
            {
                var option = p.Value;
                // exclude the 'options' option and return only the options that have been set.
                return option.Name != OptionsKey && (allOptions || option.HasValue);
            }).Select(o => o.Value);

            return requestedOptions;
        }

        public IEnumerable<OptionBase> GetAllOptions()
        {
            return GetOptions(allOptions: true);
        }

        /// <summary>
        /// Get the value of the specified option casted to the specified generic type.
        /// </summary>
        public TValue GetValue<TValue>(string optionId, bool throwOnMissing = true)
        {
            var option = GetOption(optionId, throwOnMissing);
            if (option != null && option.Value != null)
            {
                return (TValue)option.Value;
            }
            return default(TValue);
        }

        internal void SetValue(string optionId, object value)
        {
            var option = GetOption(optionId);
            if (value != option.Value)
            {
                option.Value = value;
            }
        }

        /// <summary>
        /// This is used when deserializing into an options object.  
        /// Option values set in the constructor must be reset so they can be initialized from the deserialized object,
        /// </summary>
        private void ResetOptions()
        {
            GetOption(OptionsKey).IsInitialized = false;
            foreach (var option in GetAllOptions())
            {
                option.IsInitialized = false;
            }
        }
        #endregion

        public void AddWarning(string warning, int idx = -1)
        {
            if (idx == -1)
            {
                this._warnings.Add(warning);
            }
            else
            {
                this._warnings.Insert(idx, warning);
            }
        }

        public void AddError(string error)
        {
            var ex = new ArgumentException(error);
            AddError(ex);
        }

        public void AddError(Exception ex)
        {
            this._errors.Add(ex);
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool prettyFormat)
        {
            var value = OptionsSerializer<ApplicationOptions>.SerializeToString(this, prettyFormat);
            return value;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class OptionBase
    {
        public OptionBase(string name, params string[] aliases) : this(name, null, aliases)
        {
        }

        /// <summary>
        /// .ctr
        /// </summary>
        /// <param name="name">the option main name.</param>
        /// <param name="originalValue">Mainly used for initializaing options which value is a collection.</param>
        /// <param name="aliases">Used for identifying the option with different names, this allows for supporting different property names in the JSON schema.</param>
        public OptionBase(string name, object originalValue, params string[] aliases)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
            _serializationName = name;
            _value = originalValue;
            this.Aliases = new List<string>();

            if (aliases != null && aliases.Length > 0)
            {
                this.Aliases.AddRange(aliases);
            }

            this.IsInitialized = true;
        }

        #region Properties and events

        // This property ensures an option is deserialized only once (in case it is defined multiple times in the JSON).
        internal bool IsInitialized { get; set; }

        // Aliases allows for deserializing a property that is referred to with a different name in the json file.
        public List<string> Aliases { get; private set; }

        public string Name { get; }

        private object _value;
        public object Value
        {
            get { return _value; }
            set { SetValue(value); }
        }

        // this is used in serialization to avoid serializing the option when unset. 
        public virtual bool HasValue { get { return this.Value != null; } }

        // this is used in serialization to avoid serializing the option when it set to its default value.
        // obseve that this can be different from the CLR type's default value (Enum with a default different from the first value). 
        public object DefaultValue { get; set; }

        // This allows for supporting a JSON schema with different property names (WCFCS vs SVCUTIL naming conventions).
        private string _serializationName;
        public string SerializationName
        {
            get { return _serializationName; }
            set { if (value != null && !Aliases.Contains(value)) Aliases.Add(value); _serializationName = value; }
        }

        // Determines whether the option can be serialized. 
        // This can be a static value for options that should not be automatically serialized like the 'options' options.
        private bool _canSerialize = true;
        public virtual bool CanSerialize
        {
            get { return _canSerialize && this.Value != null && !this.Value.Equals(this.DefaultValue); }
            set { _canSerialize = value; }
        }

        // events
        public event EventHandler<OptionEventArgs> ValueChanging;
        public event EventHandler<OptionEventArgs> ValueChanged;

        public event EventHandler<OptionEventArgs> Serializing;
        public event EventHandler<OptionEventArgs> Serialized;

        public event EventHandler<OptionDeserializingEventArgs> Deserializing;
        public event EventHandler<EventArgs> Deserialized;
        #endregion

        public bool HasSameId(string optionId)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(this.Name, optionId) == 0 ||
                this.Aliases.Any(a => StringComparer.OrdinalIgnoreCase.Compare(a, optionId) == 0);
        }

        public virtual void CopyTo(OptionBase other)
        {
            // serialization name should not be copied as options may define different serialization names for their options.
            // other._serializationName = this._serializationName;

            // copy the canserialize field, not the property.  The field contains a hardcoded value while the property may be computed dynamically!
            other._canSerialize = _canSerialize;

            other.Value = this.Value;
            other.DefaultValue = this.DefaultValue;

            other.ValueChanging = this.ValueChanging;
            other.ValueChanged = this.ValueChanged;
            other.Serializing = this.Serializing;
            other.Serialized = this.Serialized;
            other.Deserializing = this.Deserializing;
            other.Deserialized = this.Deserialized;
        }

        public OptionBase Clone()
        {
            var other = new OptionBase((string)this.Name, this.Aliases.ToArray());
            CopyTo(other);
            return other;
        }

        protected virtual void SetValue(object value)
        {
            if (this.Value == null || !this.Value.Equals(value))
            {
                var oldValue = this.Value;

                // notify event handlers (if any) to pre-process the value.
                value = OnValueChanging(value);

                _value = value;

                if (oldValue != _value)
                {
                    OnValueChanged(oldValue);
                }
            }
        }

        internal void Deserialize(JToken jToken)
        {
            // Raise event for handlers to preprocess the JSON value.
            var value = OnDeserializing(jToken);

            // validate value; observe that null values are not allowed during deserialization.
            var stringValue = value as string;
            OptionValueParser.ThrowInvalidValueIf(value == null || (stringValue != null && string.IsNullOrWhiteSpace(stringValue)), jToken, this);

            _value = value;
            OnDeserialized();
        }

        internal void Serialize(JsonWriter writer, JsonSerializer serializer)
        {
            if (this.CanSerialize)
            {
                // ensure the value is converted to a primitive type or a collection of primitive types
                // that can be safely serialized.
                var value = OptionValueParser.GetSerializationValue(this.Value);

                // notify event handlers (if any) to pre-process the JSON value.
                value = OnSerializing(value);

                serializer.Serialize(writer, value, value?.GetType());
                OnSerialized(value);
            }
        }

        protected virtual object OnDeserializing(JToken jToken)
        {
            if (this.IsInitialized)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorOptionAlreadyDeserializedFormat, this.SerializationName));
            }

            object value = null;
            if (Deserializing != null)
            {
                var e = new OptionDeserializingEventArgs(jToken);
                Deserializing(this, e);
                value = e.Value;
            }
            return value;
        }

        protected virtual void OnDeserialized()
        {
            this.IsInitialized = true;
            this.Deserialized?.Invoke(this, new EventArgs());
        }

        protected virtual object OnSerializing(object value)
        {
            if (Serializing != null)
            {
                var e = new OptionEventArgs(value);
                Serializing(this, e);
                value = e.Value;
            }
            return value;
        }

        protected virtual void OnSerialized(object value)
        {
            // provide the actual serialized value.
            this.Serialized?.Invoke(this, new OptionEventArgs(value));
        }

        protected virtual object OnValueChanging(object value)
        {
            if (this.IsInitialized && this.ValueChanging != null)
            {
                var eventArgs = new OptionEventArgs(value);
                this.ValueChanging.Invoke(this, eventArgs);
                value = eventArgs.Value;
            }
            return value;
        }

        protected virtual void OnValueChanged(object oldValue)
        {
            if (this.IsInitialized)
            {
                this.ValueChanged?.Invoke(this, new OptionEventArgs(oldValue));
            }
        }

        public override string ToString()
        {
            var value = string.Format(CultureInfo.CurrentCulture, "{0}: {1}", this.Name, this.Value?.ToString());
            return value;
        }
    }

    public class OptionEventArgs : EventArgs
    {
        public object Value { get; set; }
        public OptionEventArgs(object value)
        {
            Value = value;
        }
    }

    public class OptionDeserializingEventArgs : OptionEventArgs
    {
        public JToken JToken { get; set; }
        public OptionDeserializingEventArgs(JToken jToken) : base(null)
        {
            JToken = jToken;
        }
    }
}

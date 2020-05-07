// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Represents an option that can be specified only once in the command line.
    /// </summary>
    internal class SingleValueOption<TValue> : OptionBase
    {
        public SingleValueOption(string name, params string[] aliases) : base(name, aliases)
        {
            if (typeof(TValue) == typeof(bool))
            {
                this.DefaultValue = false;
            }
        }

        protected override object OnValueChanging(object value)
        {
            // allow any event handler to pre-process the value.
            value = base.OnValueChanging(value);
            if (value != null)
            {
                // ensure the value is of the right type.
                value = OptionValueParser.ParseValue<TValue>(value, this);
            }
            return value;
        }

        protected override object OnDeserializing(JToken jToken)
        {
            // allow base class to notify event handlers (if any) to pre-process the JSON value.
            var value = base.OnDeserializing(jToken);
            if (value == null)
            {
                value = jToken.Value<string>();
            }

            // ensure the value is of the right type.
            value = OptionValueParser.ParseValue<TValue>(value, this);
            return value;
        }

        public override string ToString()
        {
            return typeof(TValue) == typeof(bool) ? this.Name : base.ToString();
        }
    }
}

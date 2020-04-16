// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
//--------------------------------------------------------------------------------------------
// C`opyright(c) 2015 Microsoft Corporation
//--------------------------------------------------------------------------------------------

using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Represents an option that can be specified multiple times in the command line.
    /// Setting a value in the command line for an option that can be set multiple times is like adding an element to the collection.
    /// </summary>
    internal class ListValueOption<TValue> : OptionBase
    {
        private ListValue<TValue> InnerList { get; }

        public ListValueOption(string name) : base(name, new ListValue<TValue>())
        {
            this.InnerList = this.Value as ListValue<TValue>;
            this.InnerList.Owner = this;

            // because setting the value of a collection option means Add value, we need to hook up events 
            // to be able to notify the base class about the value changing/changed events, let's hook up handlers.
            this.InnerList.Inserting += this.InnerList_Inserting;
            this.InnerList.Inserted += this.InnerList_Inserted;
            this.InnerList.Removing += this.InnerList_Removing;
            this.InnerList.Removed += this.InnerList_Removed;
        }

        public override bool HasValue { get { return this.InnerList.Count > 0; } }

        public override bool CanSerialize
        {
            get { return base.CanSerialize && this.HasValue; }
            set { base.CanSerialize = value; }
        }

        private void InnerList_Inserting(object sender, ListOptionEventArgs e)
        {
            e.Value = OnValueChanging(e.Value);
        }

        private void InnerList_Inserted(object sender, ListOptionEventArgs e)
        {
            OnValueChanged(e.Value);
        }

        private void InnerList_Removing(object sender, ListOptionEventArgs e)
        {
            OnValueChanging(e.Value);
        }

        private void InnerList_Removed(object sender, ListOptionEventArgs e)
        {
            OnValueChanged(e.Value);
        }

        /// <summary>
        /// Core method for setting the option and/or adding values to the collection.
        /// Setting the value has the same semantics as when the option is specified in the command line, when specified multiple times 
        /// it is equivalent to having a collection of values not a collection of options and the operation is Add instead of SetValue.
        /// </summary>
        protected override void SetValue(object value)
        {
            if (value == null)
            {
                this.InnerList.Clear();
            }
            else
            {
                if (value is IList<TValue> list)
                {
                    this.InnerList.AddRange(list);
                }
                else
                {
                    this.InnerList.Add(OptionValueParser.ParseValue<TValue>(value, this));
                }
            }
        }

        protected override object OnValueChanging(object value)
        {
            // allow base class to notify event handlers (if any) to pre-process the value.
            value = base.OnValueChanging(value);

            // a null value means, the value is being removed.
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
            var collection = value as ICollection;

            if (value == null)
            {
                if (jToken.Type == JTokenType.Array)
                {
                    OptionValueParser.ThrowInvalidValueIf(jToken.Any(i => i.Type != JTokenType.String), jToken, this);
                    collection = jToken.Select(i => i.Value<string>()).ToList();
                }
                else if (jToken.Type == JTokenType.String)
                {
                    // allow for a single-value list to be represented with a string.
                    collection = new List<string>() { jToken.Value<string>() };
                }
            }

            OptionValueParser.ThrowInvalidValueIf(collection == null, jToken, this);

            foreach (var item in collection)
            {
                this.InnerList.Add(OptionValueParser.ParseValue<TValue>(item, this));
            }

            // ensure the underlying value is the inner list.
            return this.InnerList;
        }
    }
}

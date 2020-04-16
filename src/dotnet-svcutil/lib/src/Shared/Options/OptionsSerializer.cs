// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// This class drives serializing/deserializing the application options into/from a JSON (file or text).
    /// This is to support updating the web service reference added to a project. A JSON file with the options used to generate the web service reference
    /// is added along with the generated source file, and is used later to update the reference by passing it as the param to the tool.
    /// </summary>
    internal class OptionsSerializer<TAppOptions> : JsonConverter where TAppOptions : ApplicationOptions, new()
    {
        /* JSON BASIC SCHEMA
            {
              "providerId": "",
              "version": "",
              "options": {
                "op" : value
              }
            }
        */

        public override bool CanConvert(Type objectType)
        {
            return typeof(TAppOptions).GetTypeInfo().IsAssignableFrom(objectType);
        }

        #region Read (deserialization) operations
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader, new JsonLoadSettings() { CommentHandling = CommentHandling.Ignore, LineInfoHandling = LineInfoHandling.Ignore });
            return ReadJson(jObject);
        }

        public static TAppOptions ReadJson(JObject jObject)
        {
            var options = new TAppOptions();

            ((IOptionsSerializationHandler)options).RaiseBeforeDeserializeEvent();

            foreach (var jPropInfo in jObject)
            {
                try
                {
                    bool optionFound = options.TryGetOption(jPropInfo.Key, out var option);
                    if (optionFound)
                    {
                        switch (option.Name)
                        {
                            case ApplicationOptions.ProviderIdKey:
                            case ApplicationOptions.VersionKey:
                                {
                                    ReadOption(option, jPropInfo.Value);
                                }
                                break;
                            case ApplicationOptions.OptionsKey:
                                // special-case the 'options' option to preserve the order. Observe that it must be set to ensure it is initialized only once in case there are multiple definitions in the JSON.
                                option.Deserialize(new JValue(jPropInfo.Key));

                                var jOptionsObject = jPropInfo.Value as JObject ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidOptionValueFormat, jPropInfo.Value?.Type, option.SerializationName));
                                ReadOptions(options, jOptionsObject);
                                break;
                            default:
                                // it happened to be a property with the same name but on a different location in the JSON schema.
                                optionFound = false;
                                break;
                        }
                    }

                    if (!optionFound)
                    {
                        options.AddWarning(string.Format(CultureInfo.CurrentCulture, Shared.Resources.WarningUnrecognizedOptionFormat, jPropInfo.Key));
                    }
                }
                catch (Exception ex)
                {
                    if (Utils.IsFatalOrUnexpected(ex)) throw;
                    options.AddError(ex);
                }
            }

            ((IOptionsSerializationHandler)options).RaiseAfterDeserializeEvent();
            return options;
        }

        private static void ReadOption(OptionBase option, JToken jToken)
        {
            // let the option deserialize the value.
            option.Deserialize(jToken);
        }

        private static void ReadOptions(TAppOptions options, JObject jOptionsObject)
        {
            foreach (var jProperty in jOptionsObject)
            {
                try
                {
                    if (options.TryGetOption(jProperty.Key, out var option))
                    {
                        ReadOption(option, jProperty.Value);
                    }
                    else
                    {
                        options.AddWarning(string.Format(CultureInfo.CurrentCulture, Shared.Resources.WarningUnrecognizedOptionFormat, jProperty.Key));
                    }
                }
                catch (Exception ex)
                {
                    if (Utils.IsFatalOrUnexpected(ex)) throw;
                    options.AddError(ex);
                }
            }
        }
        #endregion

        #region write (serialization) operations
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            WriteJson(writer, (TAppOptions)value, serializer);
        }

        public static void WriteJson(JsonWriter writer, TAppOptions options, JsonSerializer serializer)
        {
            serializer.TypeNameHandling = TypeNameHandling.Auto;

            ((IOptionsSerializationHandler)options).RaiseBeforeSerializeEvent();

            writer.WriteStartObject();
            WriteHeaderProperties(options, writer, serializer);
            WriteOptions(options, writer, serializer);
            writer.WriteEndObject();

            ((IOptionsSerializationHandler)options).RaiseAfterSerializeEvent();
        }

        private static void WriteHeaderProperties(TAppOptions options, JsonWriter writer, JsonSerializer serializer)
        {
            var providerOption = options.GetOption(ApplicationOptions.ProviderIdKey);
            SerializeOption(providerOption, writer, serializer);

            var versionOption = options.GetOption(ApplicationOptions.VersionKey);
            SerializeOption(versionOption, writer, serializer);
        }

        private static void WriteOptions(TAppOptions options, JsonWriter writer, JsonSerializer serializer)
        {
            var optionsOption = options.GetOption(ApplicationOptions.OptionsKey);
            writer.WritePropertyName(optionsOption.SerializationName);
            writer.WriteStartObject();

            // keep inputs as the first options entry for convenience (the property bag is sorted).
            var inputsOption = options.GetOption(ApplicationOptions.InputsKey);
            SerializeOption(inputsOption, writer, serializer);

            // get unprocessed options.
            var otherOptions = options.GetOptions().Where(o =>
                                                             o.Name != ApplicationOptions.InputsKey &&
                                                             o.Name != ApplicationOptions.OptionsKey &&
                                                             o.Name != ApplicationOptions.ProviderIdKey &&
                                                             o.Name != ApplicationOptions.VersionKey).OrderBy(o => o.SerializationName);

            foreach (var option in otherOptions)
            {
                SerializeOption(option, writer, serializer);
            }

            writer.WriteEndObject();
        }

        private static void SerializeOption(OptionBase option, JsonWriter writer, JsonSerializer serializer)
        {
            if (option.CanSerialize)
            {
                writer.WritePropertyName(option.SerializationName);

                // let the option serialize the value.
                option.Serialize(writer, serializer);
            }
        }
        #endregion

        /// <summary>
        /// This is a convenient method to serialize the options into a command-line-formatted string.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="prettyFormat"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
        public static string SerializeToString(TAppOptions options, bool prettyFormat)
        {
            // Format the string similarly to the JSON format.
            var valueBuilder = new System.Text.StringBuilder();
            var separator = prettyFormat ? Environment.NewLine : " ";

            foreach (var input in options.Inputs)
            {
                valueBuilder.Append($"\"{OptionValueParser.GetSerializationValue(input)}\"{separator}");
            }

            var printOptions = options.GetOptions().Where(o =>
                                                             o.Name != ApplicationOptions.InputsKey &&
                                                             o.Name != ApplicationOptions.OptionsKey &&
                                                             o.Name != ApplicationOptions.ProviderIdKey &&
                                                             o.Name != ApplicationOptions.VersionKey).OrderBy(o => o.SerializationName);

            foreach (var option in printOptions)
            {
                if (option.CanSerialize)
                {
                    var value = OptionValueParser.GetSerializationValue(option.Value);
                    if (value is ICollection collection)
                    {
                        foreach (var listValue in collection)
                        {
                            valueBuilder.Append($"--{option.Name} \"{listValue}\"{separator}");
                        }
                    }
                    else
                    {
                        value = value is bool ? string.Empty : $" \"{value}\"";
                        valueBuilder.Append($"--{option.Name}{value}{separator}");
                    }
                }
            }

            return valueBuilder.ToString();
        }
    }

    /// <summary>
    /// This interface allows the options serializer to notify the options container about serialization events.
    /// This is useful for options container that need to synchronize property values after serialization for instance.
    /// </summary>
    public interface IOptionsSerializationHandler
    {
        void RaiseBeforeSerializeEvent();
        void RaiseAfterSerializeEvent();
        void RaiseBeforeDeserializeEvent();
        void RaiseAfterDeserializeEvent();
    }
}

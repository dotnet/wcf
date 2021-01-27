// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// This class knows how to parse objects into the types supported by the Options OM, 
    /// and how to represent these types as strings that can be serialized into JSON.
    /// </summary>
    internal class OptionValueParser
    {
        public static TValue ParseValue<TValue>(object value, OptionBase option)
        {
            ThrowInvalidValueIf(value == null, value, option);

            var valueType = typeof(TValue);

            if (value.GetType() != typeof(TValue))
            {
                // parsing is needed, the passed in value must be a string.

                var stringValue = value as string;
                ThrowInvalidValueIf(stringValue == null, value, option);

                if (valueType == typeof(bool))
                {
                    // Special-case boolean values as it is common to specify them as strings in the json file.
                    // { "myFlag" : "True" } will be resolved to {  "MyFlag" : true }
                    ThrowInvalidValueIf(!bool.TryParse(stringValue, out var boolValue), stringValue, option);
                    value = boolValue;
                }
                else if (valueType.GetTypeInfo().IsEnum)
                {
                    value = ParseEnum<TValue>(stringValue, option);
                }
                else if (valueType == typeof(CultureInfo))
                {
                    value = CreateValue<CultureInfo>(() => new CultureInfo(stringValue), option, stringValue);
                }
                else if (valueType == typeof(Uri))
                {
                    value = CreateValue<Uri>(() => new Uri(stringValue, UriKind.RelativeOrAbsolute), option, stringValue);
                }
                else if (valueType == typeof(DirectoryInfo))
                {
                    value = CreateValue<DirectoryInfo>(() => new DirectoryInfo(stringValue), option, stringValue);
                }
                else if (valueType == typeof(FileInfo))
                {
                    value = CreateValue<FileInfo>(() => new FileInfo(stringValue), option, stringValue);
                }
                else if (valueType == typeof(MSBuildProj))
                {
                    value = CreateValue<MSBuildProj>(() => MSBuildProj.FromPathAsync(stringValue, null, System.Threading.CancellationToken.None).Result, option, stringValue);
                }
                else if (valueType == typeof(FrameworkInfo))
                {
                    value = CreateValue<FrameworkInfo>(() => TargetFrameworkHelper.GetValidFrameworkInfo(stringValue), option, stringValue);
                }
                else if (valueType == typeof(ProjectDependency))
                {
                    value = CreateValue<ProjectDependency>(() => ProjectDependency.Parse(stringValue), option, stringValue);
                }
                else if (valueType == typeof(KeyValuePair<string, string>))
                {
                    value = ParseKeyValuePair(stringValue, option);
                }
                else
                {
                    ThrowInvalidValueIf(true, stringValue, option);
                }
            }

            return (TValue)value;
        }

        public static object GetSerializationValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueType = value.GetType();

            if (value is string || value is bool)
            {
                // no formatting needed, optimize if-else block for these two types.
            }
            else if (valueType.GetTypeInfo().IsEnum)
            {
                value = value.ToString();
            }
            else if (value is CultureInfo ci)
            {
                value = ci.Name;
            }
            else if (value is Uri uri)
            {
                value = (uri.IsAbsoluteUri && uri.IsFile ? uri.LocalPath : uri.OriginalString).Replace("\\", "/");
            }
            else if (value is DirectoryInfo di)
            {
                value = di.OriginalPath().Replace("\\", "/");
            }
            else if (value is FileInfo fi)
            {
                value = fi.OriginalPath().Replace("\\", "/");
            }
            else if (value is MSBuildProj proj)
            {
                value = proj.FullPath.Replace("\\", "/");
            }
            else if (value is FrameworkInfo fx)
            {
                value = fx.FullName;
            }
            else if (value is ProjectDependency pd)
            {
                value = pd.ReferenceIdentity;
            }
            else if (valueType == typeof(KeyValuePair<string, string>))
            {
                var pair = (KeyValuePair<string, string>)value;
                value = $"{pair.Key}, {pair.Value}";
            }
            else if (value is ICollection collection)
            {
                var list = new List<object>();
                foreach (var item in collection)
                {
                    var serializationValue = GetSerializationValue(item);
                    list.Add(serializationValue);
                }
                list.Sort();
                value = list;
            }

            return value;
        }

        private static readonly string[] s_nonTelemetrySensitiveOptionIds = new string[]
        {
            ApplicationOptions.ProviderIdKey, ApplicationOptions.VersionKey,
            UpdateOptions.CollectionTypesKey, UpdateOptions.ExcludeTypesKey, UpdateOptions.ReferencesKey, UpdateOptions.RuntimeIdentifierKey
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public static object GetTelemetryValue(OptionBase option)
        {
            // Avoid logging arbitrary strings input by the user!
            var value = option.Value;

            if (value == null)
            {
                value = "<null>";
            }
            else if (s_nonTelemetrySensitiveOptionIds.Any(id => option.HasSameId(id)))
            {
                var newValue = GetSerializationValue(value);
                if (newValue is List<object> list)
                {
                    value = list.Select(item => $"'{item}'").Aggregate((num, s) => num + ", " + s).ToString();
                }
            }
            else
            {
                var valueType = value.GetType();

                if (value is bool)
                {
                    value = value.ToString();
                }
                else if (valueType.GetTypeInfo().IsEnum)
                {
                    value = value.ToString();
                }
                else if (value is CultureInfo ci)
                {
                    value = ci.Name;
                }
                else if (value is FrameworkInfo fx)
                {
                    value = fx.FullName;
                }
                else if (value is ICollection collection)
                {
                    value = $"Count:{collection.Count}";
                }
                else
                {
                    value = $"<{valueType}>";
                }
            }

            return value;
        }


        private static KeyValuePair<string, string> ParseKeyValuePair(string stringValue, OptionBase option)
        {
            // format namespace as a mapping key/value pair:
            // "Namespace": "MyServiceReference1"
            var parts = stringValue.Split(',');
            ThrowInvalidValueIf(parts.Length != 2, stringValue, option);

            var value = new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
            return value;
        }

        public static object ParseEnum<TValue>(string value, OptionBase option)
        {
            // Enum.TryParse is not available in all supported platforms, need to implement own parsing of enums.

            Type thisType = typeof(TValue);
            object enumValue = null;
            foreach (var entry in thisType.GetTypeInfo().GetEnumValues())
            {
                if (StringComparer.OrdinalIgnoreCase.Compare(entry.ToString(), value) == 0)
                {
                    enumValue = entry;
                    break;
                }
            }

            if (enumValue == null || enumValue.GetType() != thisType)
            {
                var invalidValueError = string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidOptionValueFormat, value, option.Name);
                var supportedValues = string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorOnInvalidEnumSupportedValuesFormat, string.Join(", ", Enum.GetNames(typeof(TValue))));
                throw new ArgumentException(string.Concat(invalidValueError, " ", supportedValues));
            }

            return enumValue;
        }

        public static object CreateValue<TValue>(Func<object> GetValueFunc, OptionBase option, object originalValue)
        {
            object value = null;
            try
            {
                value = GetValueFunc();
            }
            catch (Exception ex)
            {
                if (Utils.IsFatalOrUnexpected(ex)) throw;
                ThrowInvalidValue(originalValue, option, ex);
            }

            ThrowInvalidValueIf(value.GetType() != typeof(TValue), value, option);
            return value;
        }

        public static void ThrowInvalidValueIf(bool condition, object value, OptionBase option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            if (condition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidOptionValueFormat, value, option.Name));
            }
        }

        public static void ThrowInvalidValue(object value, OptionBase option, Exception innerException)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidOptionValueFormat, value, option.Name), innerException);
        }
    }
}

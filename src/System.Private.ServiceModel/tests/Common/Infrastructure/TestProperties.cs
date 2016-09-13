// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;

namespace Infrastructure.Common
{
    // This type exists to provide name/value pairs for properties
    // defined at build time and accessible at runtime.
    // These name/value pairs are auto-generated when this current
    // project is built.
    public static partial class TestProperties
    {
        // Lazily create the dictionary, and initialize it via a
        // partial method in generated code.
        private static Lazy<Dictionary<String, String>> s_properties = new Lazy<Dictionary<String, String>>(() =>
        {
            Dictionary<String, String> properties = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            Initialize(properties);
            return properties;
        });

        // This partial method will be implemented by code generated at build time.
        static partial void Initialize(Dictionary<string, string> properties);

        /// <summary>
        /// Gets the list of available property names.
        /// </summary>
        public static IEnumerable<string> PropertyNames
        {
            get
            {
                return s_properties.Value.Keys;
            }
        }

        /// <summary>
        /// Returns the value associated with a given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The value associated with that property.</returns>
        /// <exception cref="ArgumentNullException">propertyName is <c>null</c>.</exception> 
        /// <exception cref="KeyNotFoundException">The request property is not available.</exception>
        /// <remarks>Precedence will be given to Environment variables.
        /// If there is no Environment variable of the request name, the fallback
        /// is the value in a dictionary created at build time.</remarks>
        public static string GetProperty(string propertyName)
        {
            // Environment variables take precedence, but limit access
            // to only the environment variables corresponding to known
            // property names.
            string result = s_properties.Value.ContainsKey(propertyName)
                                ? Environment.GetEnvironmentVariable(propertyName)
                                : null;
            if (String.IsNullOrEmpty(result))
            {
                // Throw KeyNotFoundException if caller asks for nonexistent property.
                result = s_properties.Value[propertyName];
            }

            if (result != null)
            {
                result = result.Trim();
            }

            return result;
        }
    }
}

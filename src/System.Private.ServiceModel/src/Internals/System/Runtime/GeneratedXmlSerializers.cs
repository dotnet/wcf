// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if FEATURE_NETNATIVE
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
    public class GeneratedXmlSerializers
    {
        // For NetNative, ToolChain sets s_generatedSerializersInitializer at App startup.
        // For UWP, s_generatedSerializersInitializer is null.
        private static Func<Dictionary<string, Type>> s_generatedSerializersInitializer;
        private static Lazy<Dictionary<string, Type>> s_generatedSerializers = new Lazy<Dictionary<string, Type>>(InitGeneratedSerializers);

        public static Func<Dictionary<string, Type>> GeneratedSerializersInitializer
        {
            get
            {
                return s_generatedSerializersInitializer;
            }
            set
            {
                Contract.Assert(s_generatedSerializersInitializer == null, "s_generatedSerializersInitializer is already initialized.");
                s_generatedSerializersInitializer = value;
            }
        }

        private static Dictionary<string, Type> InitGeneratedSerializers()
        {
            if (GeneratedSerializersInitializer != null)
            {
                return GeneratedSerializersInitializer();
            }
            else
            {
                return new Dictionary<string, Type>();
            }
        }

        internal static Dictionary<string, Type> GetGeneratedSerializers()
        {
            return s_generatedSerializers.Value;
        }

        // This property is used to determine if the code is running in NetNative or in UWP.
        // true  - NetNative
        // false - UWP
        internal static bool IsInitialized
        {
            get
            {
                return GetGeneratedSerializers().Count != 0;
            }
        }
    }
}
#endif
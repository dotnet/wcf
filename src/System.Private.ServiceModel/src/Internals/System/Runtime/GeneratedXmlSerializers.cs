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
        private static Func<Dictionary<string, Type>> s_GeneratedSerializersInitializer;
        private static Lazy<Dictionary<string, Type>> generatedSerializers = new Lazy<Dictionary<string, Type>>(InitGeneratedSerializers);

        public static Func<Dictionary<string, Type>> GeneratedSerializersInitializer
        {
            get
            {
                return s_GeneratedSerializersInitializer;
            }
            set
            {
                Contract.Assert(s_GeneratedSerializersInitializer == null, "s_GeneratedSerializersInitializer is already initialized.");
                s_GeneratedSerializersInitializer = value;
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
            return generatedSerializers.Value;
        }

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
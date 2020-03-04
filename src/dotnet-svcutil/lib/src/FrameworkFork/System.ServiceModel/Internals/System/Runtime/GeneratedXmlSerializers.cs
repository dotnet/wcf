// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

#if FEATURE_NETNATIVE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
    public class GeneratedXmlSerializers
    {
        private static Dictionary<string, Type> s_generatedSerializers = new Dictionary<string, Type>();

        public static Dictionary<string, Type> GetGeneratedSerializers()
        {
            return s_generatedSerializers;
        }

        public static bool IsInitialized
        {
            get
            {
                return s_generatedSerializers.Count != 0;
            }
        }
    }
}
#endif

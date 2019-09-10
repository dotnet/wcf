// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal static class AddressingVersionExtensions
    {
        public static Uri AnonymousUri(this AddressingVersion thisPtr)
        {
            if (thisPtr is null) return null;

            return thisPtr.ToString() switch
            {
                "AddressingNone (http://schemas.microsoft.com/ws/2005/05/addressing/none)" => null,
                "Addressing200408 (http://schemas.xmlsoap.org/ws/2004/08/addressing)" => new Uri("http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous"),
                "Addressing10 (http://www.w3.org/2005/08/addressing)" => new Uri("http://www.w3.org/2005/08/addressing/anonymous"),
                _ => throw new ArgumentException($"Invalid AddressingVersion:\"{thisPtr}\"", nameof(AddressingVersion))
            };
        }

        public static Uri NoneUri(this AddressingVersion thisPtr)
        {
            if (thisPtr is null) return null;

            return thisPtr.ToString() switch
            {
                "AddressingNone (http://schemas.microsoft.com/ws/2005/05/addressing/none)" => null,
                "Addressing200408 (http://schemas.xmlsoap.org/ws/2004/08/addressing)" => null,
                "Addressing10 (http://www.w3.org/2005/08/addressing)" => new Uri("http://www.w3.org/2005/08/addressing/none"),
                _ => throw new ArgumentException($"Invalid AddressingVersion:\"{thisPtr}\"", nameof(AddressingVersion))
            };
        }
    }
}

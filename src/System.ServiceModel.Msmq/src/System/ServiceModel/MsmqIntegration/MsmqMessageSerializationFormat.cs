// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.MsmqIntegration
{
    public enum MsmqMessageSerializationFormat
    {
        Xml,
        Binary,
        ActiveX,
        ByteArray,
        Stream
    }

    internal static class MsmqMessageSerializationFormatHelper
    {
        internal static bool IsDefined(MsmqMessageSerializationFormat value)
        {
            return
                value == MsmqMessageSerializationFormat.ActiveX ||
                value == MsmqMessageSerializationFormat.Binary ||
                value == MsmqMessageSerializationFormat.ByteArray ||
                value == MsmqMessageSerializationFormat.Stream ||
                value == MsmqMessageSerializationFormat.Xml;
        }
    }
}

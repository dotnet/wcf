// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
namespace System.ServiceModel.Channels
{
    using Microsoft.Xml;
    using System.ServiceModel.Description;

    public interface ITransportTokenAssertionProvider
    {
        XmlElement GetTransportTokenAssertion();
    }
}

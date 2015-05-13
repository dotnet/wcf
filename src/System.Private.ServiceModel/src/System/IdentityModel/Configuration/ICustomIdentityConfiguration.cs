// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml;

namespace System.IdentityModel.Configuration
{
    /// <summary>
    /// Types that implement ICustomIdentityConfiguration can load custom configuration
    /// </summary>
    public interface ICustomIdentityConfiguration
    {
        /// <summary>
        /// Override LoadCustomConfiguration to provide custom handling of configuration elements
        /// </summary>
        /// <param name="nodeList">Xml Nodes which contain custom configuration</param>
        void LoadCustomConfiguration(XmlNodeList nodeList);
    }
}

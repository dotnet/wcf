// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Claims;
using System.ServiceModel;
using System.IdentityModel.Policy;
using System.ServiceModel.Security.Tokens;
using System.Xml;

using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    class SendSecurityHeaderElement
    {
        private string _id;
        private ISecurityElement _item;
        private bool _markedForEncryption;

        public SendSecurityHeaderElement(string id, ISecurityElement item)
        {
            _id = id;
            _item = item;
            _markedForEncryption = false;
        }

        public string Id
        {
            get { return _id; }
        }

        public ISecurityElement Item
        {
            get { return _item; }
        }

        public bool MarkedForEncryption
        {
            get { return _markedForEncryption; }
            set { _markedForEncryption = value; }
        }

        public bool IsSameItem(ISecurityElement item)
        {
            return _item == item || _item.Equals(item);
        }

        public void Replace(string id, ISecurityElement item)
        {
            _item = item;
            _id = id;
        }
    }
}


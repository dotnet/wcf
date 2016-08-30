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
            this._id = id;
            this._item = item;
            _markedForEncryption = false;
        }

        public string Id
        {
            get { return this._id; }
        }

        public ISecurityElement Item
        {
            get { return this._item; }
        }

        public bool MarkedForEncryption
        {
            get { return this._markedForEncryption; }
            set { this._markedForEncryption = value; }
        }

        public bool IsSameItem(ISecurityElement item)
        {
            return this._item == item || this._item.Equals(item);
        }

        public void Replace(string id, ISecurityElement item)
        {
            this._item = item;
            this._id = id;
        }
    }
}


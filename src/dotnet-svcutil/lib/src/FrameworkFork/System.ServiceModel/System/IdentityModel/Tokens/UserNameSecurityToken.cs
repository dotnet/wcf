// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Runtime;

namespace System.IdentityModel.Tokens
{
    public class UserNameSecurityToken : SecurityToken
    {
        private string _id;
        private string _password;
        private string _userName;
        private DateTime _effectiveTime;

        public UserNameSecurityToken(string userName, string password)
            : this(userName, password, SecurityUniqueId.Create().Value)
        {
        }

        public UserNameSecurityToken(string userName, string password, string id)
        {
            if (userName == null)
                throw Fx.Exception.ArgumentNull("userName");
            if (userName == string.Empty)
            {
                throw Fx.Exception.Argument("userName", SRServiceModel.UserNameCannotBeEmpty);
            }

            _userName = userName;
            _password = password;
            _id = id;
            _effectiveTime = DateTime.UtcNow;
        }

        public override string Id
        {
            get { return _id; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return EmptyReadOnlyCollection<SecurityKey>.Instance; }
        }

        public override DateTime ValidFrom
        {
            get { return _effectiveTime; }
        }

        public override DateTime ValidTo
        {
            // Never expire
            get { return SecurityUtils.MaxUtcDateTime; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public string Password
        {
            get { return _password; }
        }
    }
}

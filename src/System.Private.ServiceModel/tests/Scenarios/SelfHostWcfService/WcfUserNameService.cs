// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace WcfService
{
    public class CustomUserNameValidator : UserNamePasswordValidator
    {
        // This method validates users. It allows in two users, test1 and test2 
        // with passwords 1tset and 2tset respectively.
        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == "test1" && password == "test1pwd") && !(userName == "test2" && password == "test2pwd"))
            {
                throw new SecurityTokenException("Unknown Username or Incorrect Password");
            }
        }
    }

    public class WcfUserNameService : IWcfCustomUserNameService
    {
        public String Echo(String message)
        {
            return message;
        }
    }
}

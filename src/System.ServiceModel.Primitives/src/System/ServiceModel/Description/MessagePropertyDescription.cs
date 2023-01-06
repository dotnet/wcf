// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Description
{
    public class MessagePropertyDescription : MessagePartDescription
    {
        public MessagePropertyDescription(string name)
            : base(name, "")
        {
        }

        internal MessagePropertyDescription(MessagePropertyDescription other)
            : base(other)
        {
        }

        internal override MessagePartDescription Clone()
        {
            return new MessagePropertyDescription(this);
        }
    }
}

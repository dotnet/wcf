// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

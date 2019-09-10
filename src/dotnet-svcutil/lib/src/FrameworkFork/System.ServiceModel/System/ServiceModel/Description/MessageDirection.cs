// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Description
{
    public enum MessageDirection
    {
        Input,
        Output
    }

    internal static class MessageDirectionHelper
    {
        internal static bool IsDefined(MessageDirection value)
        {
            return (value == MessageDirection.Input || value == MessageDirection.Output);
        }

        internal static MessageDirection Opposite(MessageDirection d)
        {
            return d == MessageDirection.Input ? MessageDirection.Output : MessageDirection.Input;
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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

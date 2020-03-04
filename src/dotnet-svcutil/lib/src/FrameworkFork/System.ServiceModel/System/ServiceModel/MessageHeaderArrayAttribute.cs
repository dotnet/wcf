// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
namespace System.ServiceModel
{
    [AttributeUsage(ServiceModelAttributeTargets.MessageMember, AllowMultiple = false, Inherited = false)]
    public sealed class MessageHeaderArrayAttribute : MessageHeaderAttribute
    {
    }
}

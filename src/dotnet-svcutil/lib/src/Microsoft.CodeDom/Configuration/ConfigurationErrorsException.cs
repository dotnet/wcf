// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
namespace Microsoft.CodeDom.Compiler
{
    using System;

    // Not needed in dotnet-svcutil scenario. 
    internal class ConfigurationErrorsException : Exception
    {
        public ConfigurationErrorsException()
        {
        }

        public ConfigurationErrorsException(string message) : base(message)
        {
        }

        public ConfigurationErrorsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

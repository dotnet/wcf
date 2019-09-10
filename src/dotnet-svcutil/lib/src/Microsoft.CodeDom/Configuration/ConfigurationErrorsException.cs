namespace Microsoft.CodeDom.Compiler
{
    using System;

    // TODO (Miguell): remove
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
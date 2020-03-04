// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    public class MetadataConversionError
    {
        private string _message;
        private bool _isWarning;

        public MetadataConversionError(string message) : this(message, false) { }
        public MetadataConversionError(string message, bool isWarning)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            _message = message;
            _isWarning = isWarning;
        }

        public string Message { get { return _message; } }
        public bool IsWarning { get { return _isWarning; } }
        public override bool Equals(object obj)
        {
            MetadataConversionError otherError = obj as MetadataConversionError;
            if (otherError == null)
                return false;
            return otherError.IsWarning == this.IsWarning && otherError.Message == this.Message;
        }

        public override int GetHashCode()
        {
            return _message.GetHashCode();
        }
    }
}

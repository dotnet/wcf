// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Description
{
    public class MetadataConversionError
    {
        private bool _isWarning;

        public MetadataConversionError(string message) : this(message, false) { }
        public MetadataConversionError(string message, bool isWarning)
        {
            Message = message ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            _isWarning = isWarning;
        }

        public string Message { get; }
        public bool IsWarning { get { return _isWarning; } }
        public override bool Equals(object obj)
        {
            MetadataConversionError otherError = obj as MetadataConversionError;
            if (otherError == null)
            {
                return false;
            }

            return otherError.IsWarning == IsWarning && otherError.Message == Message;
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }
    }
}

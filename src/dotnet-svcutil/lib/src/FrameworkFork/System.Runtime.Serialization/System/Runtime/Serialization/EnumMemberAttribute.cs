// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumMemberAttribute : Attribute
    {
        private string _value;
        private bool _isValueSetExplicitly;

        public EnumMemberAttribute()
        {
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; _isValueSetExplicitly = true; }
        }

        public bool IsValueSetExplicitly
        {
            get { return _isValueSetExplicitly; }
        }
    }
}

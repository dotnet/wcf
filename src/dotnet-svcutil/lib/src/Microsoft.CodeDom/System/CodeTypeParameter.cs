// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom
{
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeTypeParameter : CodeObject
    {
        private string _name;
        private CodeAttributeDeclarationCollection _customAttributes;
        private CodeTypeReferenceCollection _constraints;
        private bool _hasConstructorConstraint;

        public CodeTypeParameter()
        {
        }

        public CodeTypeParameter(string name)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return (_name == null) ? string.Empty : _name;
            }
            set
            {
                _name = value;
            }
        }

        public CodeTypeReferenceCollection Constraints
        {
            get
            {
                if (_constraints == null)
                {
                    _constraints = new CodeTypeReferenceCollection();
                }
                return _constraints;
            }
        }

        public CodeAttributeDeclarationCollection CustomAttributes
        {
            get
            {
                if (_customAttributes == null)
                {
                    _customAttributes = new CodeAttributeDeclarationCollection();
                }
                return _customAttributes;
            }
        }

        public bool HasConstructorConstraint
        {
            get
            {
                return _hasConstructorConstraint;
            }
            set
            {
                _hasConstructorConstraint = value;
            }
        }
    }
}



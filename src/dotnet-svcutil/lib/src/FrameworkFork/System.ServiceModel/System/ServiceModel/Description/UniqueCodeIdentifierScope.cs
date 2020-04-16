// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using Microsoft.CodeDom;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime;
    using System.Text;

    internal class UniqueCodeIdentifierScope
    {
        private const int MaxIdentifierLength = 511;
        private SortedList<string, string> _names;

        // assumes identifier is valid
        protected virtual void AddIdentifier(string identifier)
        {
            if (_names == null)
                _names = new SortedList<string, string>(StringComparer.Ordinal);

            _names.Add(identifier, identifier);
        }

        // assumes identifier is valid
        public void AddReserved(string identifier)
        {
            Fx.Assert(IsUnique(identifier), "");

            AddIdentifier(identifier);
        }

        // validates name before trying to add
        public string AddUnique(string name, string defaultName)
        {
            string validIdentifier = MakeValid(name, defaultName);

            string uniqueIdentifier = validIdentifier;
            int i = 1;

            while (!IsUnique(uniqueIdentifier))
            {
                uniqueIdentifier = validIdentifier + (i++).ToString(CultureInfo.InvariantCulture);
            }

            AddIdentifier(uniqueIdentifier);

            return uniqueIdentifier;
        }

        // assumes identifier is valid
        public virtual bool IsUnique(string identifier)
        {
            return _names == null || !_names.ContainsKey(identifier);
        }

        private static bool IsValidStart(char c)
        {
            return (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.DecimalDigitNumber);
        }

        private static bool IsValid(char c)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);

            // each char must be Lu, Ll, Lt, Lm, Lo, Nd, Mn, Mc, Pc

            switch (uc)
            {
                case UnicodeCategory.UppercaseLetter:        // Lu
                case UnicodeCategory.LowercaseLetter:        // Ll
                case UnicodeCategory.TitlecaseLetter:        // Lt
                case UnicodeCategory.ModifierLetter:         // Lm
                case UnicodeCategory.OtherLetter:            // Lo
                case UnicodeCategory.DecimalDigitNumber:     // Nd
                case UnicodeCategory.NonSpacingMark:         // Mn
                case UnicodeCategory.SpacingCombiningMark:   // Mc
                case UnicodeCategory.ConnectorPunctuation:   // Pc
                    return true;
                default:
                    return false;
            }
        }

        public static string MakeValid(string identifier, string defaultIdentifier)
        {
            if (String.IsNullOrEmpty(identifier))
                return defaultIdentifier;

            if (identifier.Length <= MaxIdentifierLength && Microsoft.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(identifier))
                return identifier;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < identifier.Length && builder.Length < MaxIdentifierLength; i++)
            {
                char c = identifier[i];
                if (IsValid(c))
                {
                    if (builder.Length == 0)
                    {
                        // check for valid start char
                        if (!IsValidStart(c))
                            builder.Append('_');
                    }
                    builder.Append(c);
                }
            }
            if (builder.Length == 0)
                return defaultIdentifier;

            return builder.ToString();
        }
    }

    internal class UniqueCodeNamespaceScope : UniqueCodeIdentifierScope
    {
        private CodeNamespace _codeNamespace;

        // possible direction: add an option to cache for multi-use cases
        public UniqueCodeNamespaceScope(CodeNamespace codeNamespace)
        {
            _codeNamespace = codeNamespace;
        }

        public CodeNamespace CodeNamespace
        {
            get { return _codeNamespace; }
        }

        protected override void AddIdentifier(string identifier)
        {
        }

        public CodeTypeReference AddUnique(CodeTypeDeclaration codeType, string name, string defaultName)
        {
            codeType.Name = base.AddUnique(name, defaultName);
            _codeNamespace.Types.Add(codeType);
            return ServiceContractGenerator.NamespaceHelper.GetCodeTypeReference(_codeNamespace, codeType);
        }

        public override bool IsUnique(string identifier)
        {
            return !NamespaceContainsType(identifier);
        }

        private bool NamespaceContainsType(string typeName)
        {
            foreach (CodeTypeDeclaration codeType in _codeNamespace.Types)
            {
                if (String.Compare(codeType.Name, typeName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

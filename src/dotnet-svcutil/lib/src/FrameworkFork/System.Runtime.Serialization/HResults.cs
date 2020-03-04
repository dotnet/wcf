// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// mscorlib defines the HRESULT constants it uses in the internal class System.__HResults.
    /// Since we cannot use that internal class in this assembly, we define the constants we need
    /// in this class.
    /// </summary>
    internal static class __HResults
    {
        internal const Int32 COR_E_SYSTEM = unchecked((int)0x80131501);
    }  
}  

// HResults.cs

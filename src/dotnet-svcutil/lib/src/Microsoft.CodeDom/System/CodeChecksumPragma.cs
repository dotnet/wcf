// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Runtime.InteropServices;

    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeChecksumPragma : CodeDirective
    {
        private string _fileName;
        private byte[] _checksumData;
        private Guid _checksumAlgorithmId;

        public CodeChecksumPragma()
        {
        }

        public CodeChecksumPragma(string fileName, Guid checksumAlgorithmId, byte[] checksumData)
        {
            _fileName = fileName;
            _checksumAlgorithmId = checksumAlgorithmId;
            _checksumData = checksumData;
        }

        public string FileName
        {
            get
            {
                return (_fileName == null) ? string.Empty : _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public Guid ChecksumAlgorithmId
        {
            get
            {
                return _checksumAlgorithmId;
            }
            set
            {
                _checksumAlgorithmId = value;
            }
        }

        public byte[] ChecksumData
        {
            get
            {
                return _checksumData;
            }
            set
            {
                _checksumData = value;
            }
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom {

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
    public class CodeChecksumPragma: CodeDirective {
        private string fileName;
        private byte[] checksumData;
        private Guid checksumAlgorithmId;

        public CodeChecksumPragma() {
        }
        
        public CodeChecksumPragma(string fileName, Guid checksumAlgorithmId, byte[] checksumData) {
            this.fileName = fileName;
            this.checksumAlgorithmId = checksumAlgorithmId;
            this.checksumData = checksumData;
        }

        public string FileName {
            get {
                return (fileName == null) ? string.Empty : fileName;
            }
            set {
                fileName = value;
            }
        }
        
        public Guid ChecksumAlgorithmId {
            get {
                return checksumAlgorithmId;
            }
            set {
                checksumAlgorithmId = value;
            }
        }
        
        public byte[] ChecksumData {
            get {
                return checksumData;
            }
            set {
                checksumData = value;
            }
        }
    }
}

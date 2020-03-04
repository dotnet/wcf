// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Microsoft.CodeDom.Compiler;

namespace Microsoft.Xml.Xsl {
				using System;
				using Microsoft.Xml;

    public sealed class XsltSettings {
        private bool enableDocumentFunction;
        private bool enableScript;
        private bool checkOnly;
        private bool includeDebugInformation;
        private int  warningLevel = -1;     // -1 means not set
        private bool treatWarningsAsErrors;
        private TempFileCollection tempFiles;

        public XsltSettings() { }

        public XsltSettings(bool enableDocumentFunction, bool enableScript) {
            this.enableDocumentFunction = enableDocumentFunction;
            this.enableScript           = enableScript;
        }

        public static XsltSettings Default {
            get { return new XsltSettings(false, false); }
        }

        public static XsltSettings TrustedXslt {
            get { return new XsltSettings(true, true); }
        }

        public bool EnableDocumentFunction {
            get { return enableDocumentFunction;  }
            set { enableDocumentFunction = value; }
        }

        public bool EnableScript {
            get { return enableScript;  }
            set { enableScript = value; }
        }

        internal bool CheckOnly {
            get { return checkOnly;  }
            set { checkOnly = value; }
        }

        internal bool IncludeDebugInformation {
            get { return includeDebugInformation;  }
            set { includeDebugInformation = value; }
        }

        internal int WarningLevel {
            get { return warningLevel;  }
            set { warningLevel = value; }
        }

        internal bool TreatWarningsAsErrors {
            get { return treatWarningsAsErrors;  }
            set { treatWarningsAsErrors = value; }
        }

        internal TempFileCollection TempFiles {
            get { return tempFiles;  }
            set { tempFiles = value; }
        }
    }
}

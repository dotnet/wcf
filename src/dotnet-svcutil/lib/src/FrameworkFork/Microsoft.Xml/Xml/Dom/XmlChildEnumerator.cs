// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml {
				using System;
				
    using System.Collections;

    internal sealed class XmlChildEnumerator: IEnumerator {
        internal XmlNode container;
        internal XmlNode child;
        internal bool isFirst;

        internal XmlChildEnumerator( XmlNode container ) {
            this.container = container;
            this.child = container.FirstChild;
            this.isFirst = true;
        }

        bool IEnumerator.MoveNext() {
            return this.MoveNext();
        }

        internal bool MoveNext() {
            if (isFirst) {
                child = container.FirstChild;
                isFirst = false;
            }
            else if (child != null) {
                child = child.NextSibling;
            }

            return child != null;
        }

        void IEnumerator.Reset() {
            isFirst = true;
            child = container.FirstChild;
        }

        object IEnumerator.Current {
            get {
                return this.Current;
            }
        }

        internal XmlNode Current {
            get {
                if (isFirst || child == null)
                    throw new InvalidOperationException(ResXml.GetString(ResXml.Xml_InvalidOperation));

                return child;
            }
        }
    }
}

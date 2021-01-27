// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;


    /// <devdoc>
    ///     <para>
    ///       A collection that stores <see cref='Microsoft.CodeDom.CodeNamespace'/> objects.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeNamespaceCollection : CollectionBase
    {
        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/>.
        ///    </para>
        /// </devdoc>
        public CodeNamespaceCollection()
        {
        }

        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> based on another <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/>.
        ///    </para>
        /// </devdoc>
        public CodeNamespaceCollection(CodeNamespaceCollection value)
        {
            this.AddRange(value);
        }

        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> containing any array of <see cref='Microsoft.CodeDom.CodeNamespace'/> objects.
        ///    </para>
        /// </devdoc>
        public CodeNamespaceCollection(CodeNamespace[] value)
        {
            this.AddRange(value);
        }

        /// <devdoc>
        /// <para>Represents the entry at the specified index of the <see cref='Microsoft.CodeDom.CodeNamespace'/>.</para>
        /// </devdoc>
        public CodeNamespace this[int index]
        {
            get
            {
                return ((CodeNamespace)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        /// <devdoc>
        ///    <para>Adds a <see cref='Microsoft.CodeDom.CodeNamespace'/> with the specified value to the 
        ///    <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> .</para>
        /// </devdoc>
        public int Add(CodeNamespace value)
        {
            return List.Add(value);
        }

        /// <devdoc>
        /// <para>Copies the elements of an array to the end of the <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/>.</para>
        /// </devdoc>
        public void AddRange(CodeNamespace[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            for (int i = 0; ((i) < (value.Length)); i = ((i) + (1)))
            {
                this.Add(value[i]);
            }
        }

        /// <devdoc>
        ///     <para>
        ///       Adds the contents of another <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> to the end of the collection.
        ///    </para>
        /// </devdoc>
        public void AddRange(CodeNamespaceCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            int currentCount = value.Count;
            for (int i = 0; i < currentCount; i = ((i) + (1)))
            {
                this.Add(value[i]);
            }
        }

        /// <devdoc>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> contains the specified <see cref='Microsoft.CodeDom.CodeNamespace'/>.</para>
        /// </devdoc>
        public bool Contains(CodeNamespace value)
        {
            return List.Contains(value);
        }

        /// <devdoc>
        /// <para>Copies the <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </devdoc>
        public void CopyTo(CodeNamespace[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <devdoc>
        ///    <para>Returns the index of a <see cref='Microsoft.CodeDom.CodeNamespace'/> in 
        ///       the <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> .</para>
        /// </devdoc>
        public int IndexOf(CodeNamespace value)
        {
            return List.IndexOf(value);
        }

        /// <devdoc>
        /// <para>Inserts a <see cref='Microsoft.CodeDom.CodeNamespace'/> into the <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> at the specified index.</para>
        /// </devdoc>
        public void Insert(int index, CodeNamespace value)
        {
            List.Insert(index, value);
        }

        /// <devdoc>
        ///    <para> Removes a specific <see cref='Microsoft.CodeDom.CodeNamespace'/> from the 
        ///    <see cref='Microsoft.CodeDom.CodeNamespaceCollection'/> .</para>
        /// </devdoc>
        public void Remove(CodeNamespace value)
        {
            List.Remove(value);
        }
    }
}

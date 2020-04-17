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
    ///       A collection that stores <see cref='Microsoft.CodeDom.CodeTypeMember'/> objects.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeTypeMemberCollection : CollectionBase
    {
        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/>.
        ///    </para>
        /// </devdoc>
        public CodeTypeMemberCollection()
        {
        }

        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> based on another <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/>.
        ///    </para>
        /// </devdoc>
        public CodeTypeMemberCollection(CodeTypeMemberCollection value)
        {
            this.AddRange(value);
        }

        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> containing any array of <see cref='Microsoft.CodeDom.CodeTypeMember'/> objects.
        ///    </para>
        /// </devdoc>
        public CodeTypeMemberCollection(CodeTypeMember[] value)
        {
            this.AddRange(value);
        }

        /// <devdoc>
        /// <para>Represents the entry at the specified index of the <see cref='Microsoft.CodeDom.CodeTypeMember'/>.</para>
        /// </devdoc>
        public CodeTypeMember this[int index]
        {
            get
            {
                return ((CodeTypeMember)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        /// <devdoc>
        ///    <para>Adds a <see cref='Microsoft.CodeDom.CodeTypeMember'/> with the specified value to the 
        ///    <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> .</para>
        /// </devdoc>
        public int Add(CodeTypeMember value)
        {
            return List.Add(value);
        }

        /// <devdoc>
        /// <para>Copies the elements of an array to the end of the <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/>.</para>
        /// </devdoc>
        public void AddRange(CodeTypeMember[] value)
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
        ///       Adds the contents of another <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> to the end of the collection.
        ///    </para>
        /// </devdoc>
        public void AddRange(CodeTypeMemberCollection value)
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
        ///    <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> contains the specified <see cref='Microsoft.CodeDom.CodeTypeMember'/>.</para>
        /// </devdoc>
        public bool Contains(CodeTypeMember value)
        {
            return List.Contains(value);
        }

        /// <devdoc>
        /// <para>Copies the <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </devdoc>
        public void CopyTo(CodeTypeMember[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <devdoc>
        ///    <para>Returns the index of a <see cref='Microsoft.CodeDom.CodeTypeMember'/> in 
        ///       the <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> .</para>
        /// </devdoc>
        public int IndexOf(CodeTypeMember value)
        {
            return List.IndexOf(value);
        }

        /// <devdoc>
        /// <para>Inserts a <see cref='Microsoft.CodeDom.CodeTypeMember'/> into the <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> at the specified index.</para>
        /// </devdoc>
        public void Insert(int index, CodeTypeMember value)
        {
            List.Insert(index, value);
        }

        /// <devdoc>
        ///    <para> Removes a specific <see cref='Microsoft.CodeDom.CodeTypeMember'/> from the 
        ///    <see cref='Microsoft.CodeDom.CodeTypeMemberCollection'/> .</para>
        /// </devdoc>
        public void Remove(CodeTypeMember value)
        {
            List.Remove(value);
        }
    }
}

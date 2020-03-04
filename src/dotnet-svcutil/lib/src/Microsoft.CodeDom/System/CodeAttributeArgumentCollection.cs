// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
namespace Microsoft.CodeDom {
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    
    
    /// <devdoc>
    ///     <para>
    ///       A collection that stores <see cref='Microsoft.CodeDom.CodeAttributeArgument'/> objects.
    ///    </para>
    /// </devdoc>
    [
       //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
        // Serializable,
    ]
    public class CodeAttributeArgumentCollection : CollectionBase {
        
        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/>.
        ///    </para>
        /// </devdoc>
        public CodeAttributeArgumentCollection() {
        }
        
        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> based on another <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/>.
        ///    </para>
        /// </devdoc>
        public CodeAttributeArgumentCollection(CodeAttributeArgumentCollection value) {
            this.AddRange(value);
        }
        
        /// <devdoc>
        ///     <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> containing any array of <see cref='Microsoft.CodeDom.CodeAttributeArgument'/> objects.
        ///    </para>
        /// </devdoc>
        public CodeAttributeArgumentCollection(CodeAttributeArgument[] value) {
            this.AddRange(value);
        }
        
        /// <devdoc>
        /// <para>Represents the entry at the specified index of the <see cref='Microsoft.CodeDom.CodeAttributeArgument'/>.</para>
        /// </devdoc>
        public CodeAttributeArgument this[int index] {
            get {
                return ((CodeAttributeArgument)(List[index]));
            }
            set {
                List[index] = value;
            }
        }
        
        /// <devdoc>
        ///    <para>Adds a <see cref='Microsoft.CodeDom.CodeAttributeArgument'/> with the specified value to the 
        ///    <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> .</para>
        /// </devdoc>
        public int Add(CodeAttributeArgument value) {
            return List.Add(value);
        }
        
        /// <devdoc>
        /// <para>Copies the elements of an array to the end of the <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/>.</para>
        /// </devdoc>
        public void AddRange(CodeAttributeArgument[] value) {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            for (int i = 0; ((i) < (value.Length)); i = ((i) + (1))) {
                this.Add(value[i]);
            }
        }
        
        /// <devdoc>
        ///     <para>
        ///       Adds the contents of another <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> to the end of the collection.
        ///    </para>
        /// </devdoc>
        public void AddRange(CodeAttributeArgumentCollection value) {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            int currentCount = value.Count;
            for (int i = 0; i < currentCount; i = ((i) + (1))) {
                this.Add(value[i]);
            }
        }
        
        /// <devdoc>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> contains the specified <see cref='Microsoft.CodeDom.CodeAttributeArgument'/>.</para>
        /// </devdoc>
        public bool Contains(CodeAttributeArgument value) {
            return List.Contains(value);
        }
        
        /// <devdoc>
        /// <para>Copies the <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </devdoc>
        public void CopyTo(CodeAttributeArgument[] array, int index) {
            List.CopyTo(array, index);
        }
        
        /// <devdoc>
        ///    <para>Returns the index of a <see cref='Microsoft.CodeDom.CodeAttributeArgument'/> in 
        ///       the <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> .</para>
        /// </devdoc>
        public int IndexOf(CodeAttributeArgument value) {
            return List.IndexOf(value);
        }
        
        /// <devdoc>
        /// <para>Inserts a <see cref='Microsoft.CodeDom.CodeAttributeArgument'/> into the <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> at the specified index.</para>
        /// </devdoc>
        public void Insert(int index, CodeAttributeArgument value) {
            List.Insert(index, value);
        }
        
        /// <devdoc>
        ///    <para> Removes a specific <see cref='Microsoft.CodeDom.CodeAttributeArgument'/> from the 
        ///    <see cref='Microsoft.CodeDom.CodeAttributeArgumentCollection'/> .</para>
        /// </devdoc>
        public void Remove(CodeAttributeArgument value) {
            List.Remove(value);
        }
    }
}

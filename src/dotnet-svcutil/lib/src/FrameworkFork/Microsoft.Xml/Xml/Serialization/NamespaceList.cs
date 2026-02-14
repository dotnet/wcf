namespace Microsoft.Tools.ServiceModel.Svcutil.XmlSerializer
{
    using System;
    using System.Collections;
    
    internal class NamespaceList
    {
        internal enum ListType
        {
            Any,
            Other,
            Set
        }

        private ListType _type;
        private Hashtable _set; 

        public NamespaceList(string namespaces, string targetNamespace)
        {
             bool targetNamespaceFound = targetNamespace != null && targetNamespace.Length > 0;
             
             if (namespaces == "##any" || namespaces == null || namespaces.Length == 0) {
                 _type = ListType.Any;
             }
             else if (namespaces == "##other") {
                 _type = ListType.Other;
             }
             else {
                 _type = ListType.Set;
                 _set = new Hashtable();
                 string[] parts = namespaces.Split((char[])null, StringSplitOptions.RemoveEmptyEntries); // split on whitespace
                 foreach(string s in parts) {
                     if (s == "##local") {
                         _set[""] = "";
                     }
                     else if (s == "##targetNamespace") {
                         if (targetNamespaceFound) _set[targetNamespace] = targetNamespace;
                     }
                     else {
                         _set[s] = s;
                     }
                 }
             }
        }

        public ListType Type 
        {
            get { return _type; }
        }

        public ICollection Enumerate
        {
            get { return _set.Keys; }
        }
    }
}

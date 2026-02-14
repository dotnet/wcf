namespace Microsoft.Tools.ServiceModel.Svcutil.XmlSerializer
{
    using System.Runtime.CompilerServices;
    using System.Xml.Schema;

    internal static class SchemaHelper
    {
        private static ConditionalWeakTable<XmlSchema, object> _preprocessed = new ConditionalWeakTable<XmlSchema, object>();
        
        public static bool IsPreprocessed(XmlSchema schema)
        {
             return _preprocessed.TryGetValue(schema, out _);
        }
        
        public static void SetPreprocessed(XmlSchema schema, bool value)
        {
             if (value) 
             {
                 _preprocessed.GetValue(schema, k => null);
             }
             else 
             {
                 _preprocessed.Remove(schema);
             }
        }
    }
}

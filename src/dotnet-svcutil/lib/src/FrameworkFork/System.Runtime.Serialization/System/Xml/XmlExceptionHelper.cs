namespace System.Xml
{
    using System;

    internal static class XmlExceptionHelper
    {
         public static Exception CreateConversionException(string value, string type, Exception exception)
         {
             return new XmlException($"The value '{value}' cannot be parsed as the type '{type}'.", exception);
         }

         public static ArgumentException CreateConversionException(string value, string type)
         {
             return new ArgumentException($"The value '{value}' cannot be parsed as the type '{type}'.");
         }

         public static Exception CreateEncodingException(byte[] buffer, int offset, int count, Exception exception)
         {
             return new XmlException("Encoding exception", exception);
         }

         public static Exception CreateEncodingException(string value, Exception exception)
         {
             return new XmlException("Encoding exception: " + value, exception);
         }
    }
}

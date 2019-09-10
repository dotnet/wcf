namespace System.Web.Services.Protocols
{
    internal class Fx
    {
        internal static bool IsFatal(Exception e)
        {
            return e is OutOfMemoryException;
        }
    }
}
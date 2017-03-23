using System.ServiceModel.Channels;

namespace System.ServiceModel.Diagnostics
{
    public static class TraceUtility
    {
        internal static Exception ThrowHelperError(Exception exception, Message message)
        {
            return exception;
        }
    }
}
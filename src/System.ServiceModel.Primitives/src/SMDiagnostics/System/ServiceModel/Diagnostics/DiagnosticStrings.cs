// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Diagnostics
{
    internal static class DiagnosticStrings
    {
        internal const string DiagnosticsNamespace = "http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics";

        internal const string ActivityIdName = "E2ETrace.ActivityID";
        internal const string ActivityId = "ActivityId";
        internal const string AppDomain = "AppDomain";
        internal const string ChannelTag = "Channel";
        internal const string DataTag = "Data";
        internal const string DataItemsTag = "DataItems";
        internal const string DeflateCookieAfterDeflatingTag = "AfterDeflating";
        internal const string DeflateCookieOriginalSizeTag = "OriginalSize";
        internal const string Description = "Description";
        internal const string DescriptionTag = "Description";
        internal const string EventLogTag = "EventLog";
        internal const string ExceptionTag = "Exception";
        internal const string ExceptionTypeTag = "ExceptionType";
        internal const string ExceptionStringTag = "ExceptionString";
        internal const string ExtendedDataTag = "ExtendedData";
        internal const string HeaderTag = "Header";
        internal const string InnerExceptionTag = "InnerException";
        internal const string KeyTag = "Key";
        internal const string MessageTag = "Message";
        internal const string NameTag = "Name";
        internal const string NamespaceTag = "xmlns";
        internal const string NativeErrorCodeTag = "NativeErrorCode";
        internal const string ProcessId = "ProcessId";
        internal const string ProcessName = "ProcessName";
        internal const string RoleTag = "Role";
        internal const string Separator = ":";
        internal const string SeverityTag = "Severity";
        internal const string SourceTag = "Source";
        internal const string StackTraceTag = "StackTrace";
        internal const string Task = "Task";
        internal const string TraceCodeTag = "TraceIdentifier";
        internal const string TraceRecordTag = "TraceRecord";
        internal const string ValueTag = "Value";

        internal static string[][] HeadersPaths { get; } = {
                new string[] { TraceRecordTag, ExtendedDataTag, "MessageHeaders", "Security" },
                new string[] { TraceRecordTag, ExtendedDataTag, "MessageHeaders", "IssuedTokens" } };
        internal static string[] PiiList { get; } = new string[] { "BinarySecret", "Entropy", "Password", "Nonce", "Username", "BinarySecurityToken", "NameIdentifier", "SubjectLocality", "AttributeValue" };
    }
}

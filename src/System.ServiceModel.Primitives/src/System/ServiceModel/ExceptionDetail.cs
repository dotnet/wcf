// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.Text;
using System.Runtime.Serialization;

namespace System.ServiceModel
{
    [DataContract]
    public class ExceptionDetail
    {
        private string _type;

        public ExceptionDetail(Exception exception)
        {
            if (exception == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(exception));
            }

            HelpLink = exception.HelpLink;
            Message = exception.Message;
            StackTrace = exception.StackTrace;
            _type = exception.GetType().ToString();

            if (exception.InnerException != null)
            {
                InnerException = new ExceptionDetail(exception.InnerException);
            }
        }

        [DataMember]
        public string HelpLink { get; set; }

        [DataMember]
        public ExceptionDetail InnerException { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}\n{1}", SRP.SFxExceptionDetailFormat, ToStringHelper(false));
        }

        private string ToStringHelper(bool isInner)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}: {1}", Type, Message);
            if (InnerException != null)
            {
                sb.AppendFormat(" ----> {0}", InnerException.ToStringHelper(true));
            }
            else
            {
                sb.Append("\n");
            }
            sb.Append(StackTrace);
            if (isInner)
            {
                sb.AppendFormat("\n   {0}\n", SRP.SFxExceptionDetailEndOfInner);
            }
            return sb.ToString();
        }
    }
}


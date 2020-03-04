// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Globalization;
using System.Text;
using System.Runtime.Serialization;

namespace System.ServiceModel
{
    [DataContract]
    public class ExceptionDetail
    {
        private string _helpLink;
        private ExceptionDetail _innerException;
        private string _message;
        private string _stackTrace;
        private string _type;

        public ExceptionDetail(Exception exception)
        {
            if (exception == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("exception");
            }

            _helpLink = exception.HelpLink;
            _message = exception.Message;
            _stackTrace = exception.StackTrace;
            _type = exception.GetType().ToString();

            if (exception.InnerException != null)
            {
                _innerException = new ExceptionDetail(exception.InnerException);
            }
        }

        [DataMember]
        public string HelpLink
        {
            get { return _helpLink; }
            set { _helpLink = value; }
        }

        [DataMember]
        public ExceptionDetail InnerException
        {
            get { return _innerException; }
            set { _innerException = value; }
        }

        [DataMember]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        [DataMember]
        public string StackTrace
        {
            get { return _stackTrace; }
            set { _stackTrace = value; }
        }

        [DataMember]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}\n{1}", SRServiceModel.SFxExceptionDetailFormat, this.ToStringHelper(false));
        }

        private string ToStringHelper(bool isInner)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}: {1}", this.Type, this.Message);
            if (this.InnerException != null)
            {
                sb.AppendFormat(" ----> {0}", this.InnerException.ToStringHelper(true));
            }
            else
            {
                sb.Append("\n");
            }
            sb.Append(this.StackTrace);
            if (isInner)
            {
                sb.AppendFormat("\n   {0}\n", SRServiceModel.SFxExceptionDetailEndOfInner);
            }
            return sb.ToString();
        }
    }
}


using System;
using System.Runtime.Serialization;

namespace FastReport.Web
{
    [Serializable]
    internal abstract class WebReportException : Exception
    {
        public WebReportException()
        {
        }

        public WebReportException(string message) : base(message)
        {
        }

        public WebReportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebReportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    internal sealed class UnsupportedExportException : WebReportException
    {

    }
}
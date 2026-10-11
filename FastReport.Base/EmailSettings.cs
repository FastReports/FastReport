using System;
using System.Collections.Generic;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// Contains the email settings such as recipient(s) address, name, subject, message body.
    /// </summary>
    [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
    public class EmailSettings
    {
        #region Fields
        private string[] recipients;
        private string subject;
        private string message;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the recipient(s) email addresses.
        /// </summary>
        /// <remarks>
        /// This property contains one or several email addresses in the following form: "john@url.com".
        /// </remarks>
        public string[] Recipients
        {
            get { return recipients; }
            set { recipients = value; }
        }

        /// <summary>
        /// Gets or sets the message subject.
        /// </summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        internal string RecipientsText
        {
            get
            {
                if (Recipients == null)
                    return "";

                string result = "";
                foreach (string recipient in Recipients)
                {
                    result += recipient + "\r\n";
                }
                return result.Substring(0, result.Length - 2);
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    recipients = null;
                    return;
                }

                Recipients = value.Replace("\r\n", "\n").Split('\n');
            }
        }

        internal string FirstRecipient
        {
            get { return Recipients == null ? "" : Recipients[0]; }
        }

        internal string[] CCRecipients
        {
            get
            {
                if (Recipients == null || Recipients.Length == 1)
                    return null;

                List<string> result = new List<string>();
                for (int i = 1; i < Recipients.Length; i++)
                {
                    result.Add(Recipients[i]);
                }
                return result.ToArray();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Copies email settings from another source.
        /// </summary>
        /// <param name="source">Source to copy settings from.</param>
        public void Assign(EmailSettings source)
        {
            Recipients = source.Recipients;
            Subject = source.Subject;
            Message = source.Message;
        }

        /// <summary>
        /// Resets all settings to its default values.
        /// </summary>
        public void Clear()
        {
            recipients = null;
            subject = "";
            message = "";
        }

        internal void Serialize(FRWriter writer, EmailSettings c)
        {
            if (Recipients != c.Recipients)
                writer.WriteValue("EmailSettings.Recipients", Recipients);
            if (Subject != c.Subject)
                writer.WriteStr("EmailSettings.Subject", Subject);
            if (Message != c.Message)
                writer.WriteStr("EmailSettings.Message", Message);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSettings"/> class with default settings.
        /// </summary>
        public EmailSettings()
        {
            Clear();
        }
    }
}

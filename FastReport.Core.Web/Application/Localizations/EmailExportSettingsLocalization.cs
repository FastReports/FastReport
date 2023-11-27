namespace FastReport.Web.Application.Localizations
{
    internal class EmailExportSettingsLocalization
    {
        internal readonly string Title;

        internal readonly string Email;
        internal readonly string Address;
        internal readonly string Subject;
        internal readonly string Message;

        internal readonly string Account;
        internal readonly string Host;
        internal readonly string Password;
        internal readonly string EnableSSL;
        internal readonly string NameAttachmentFile;
        internal readonly string Name;
        internal readonly string Attachment;
        internal readonly string Template;
        internal readonly string Username;
        internal readonly string Port;
        internal readonly string FailureMessage;
        internal readonly string SuccessMessage;
        internal readonly string Settings;

        public EmailExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Email");
            Title = res.Get("");
            Email = res.Get("Email");
            Settings = res.Get("Settings");
            Address = res.Get("Address");
            Subject = res.Get("Subject");
            Message = res.Get("Message");
            Account = res.Get("Account");
            Host = res.Get("Host");
            Password = res.Get("Password");
            EnableSSL = res.Get("EnableSSL");
            NameAttachmentFile = res.Get("NameAttachmentFile");
            Attachment = res.Get("Attachment");
            Name = res.Get("Name");
            Template = res.Get("Template");
            Username = res.Get("UserName");
            Port = res.Get("Port");
            FailureMessage = res.Get("ErrorEmailSending");
            SuccessMessage = res.Get("SuccessfulEmailSending");
        }
    }
}
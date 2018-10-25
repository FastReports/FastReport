using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FastReport.Barcode.QRCode
{
    abstract class QRData
    {
        public string data;

        public QRData() { }

        public QRData(string data)
        {
            Unpack(data);
        }

        public virtual string Pack()
        {
            return data;
        }

        public virtual void Unpack(string data)
        {
            this.data = data;
        }

        public static QRData Parse(string data)
        {
            if (data.StartsWith("BEGIN:VCARD"))
                return new QRvCard(data);
            else if (data.StartsWith("MATMSG:"))
                return new QREmailMessage(data);
            else if (data.StartsWith("geo:"))
                return new QRGeo(data);
            else if (data.StartsWith("SMSTO:"))
                return new QRSMS(data);
            else if (data.StartsWith("tel:"))
                return new QRCall(data);
            else if (data.StartsWith("BEGIN:VEVENT"))
                return new QREvent(data);
            else if (data.StartsWith("WIFI:"))
                return new QRWifi(data);
            else if (Uri.IsWellFormedUriString(data, UriKind.Absolute))
                return new QRURI(data);
            else if (Regex.IsMatch(data, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                return new QREmailAddress(data);
            else
                return new QRText(data);
        }
    }

    class QRText : QRData
    {
        public QRText() : base() { }
        public QRText(string data) : base(data) { }
    }

    class QRvCard : QRData
    {
        public string firstName;
        public string lastName;
        public string street;
        public string zipCode;
        public string city;
        public string country;

        public string fn_card;
        public string title;
        public string org;
        public string url;
        public string tel_cell;
        public string tel_work_voice;
        public string tel_home_voice;
        public string email_home_internet;
        public string email_work_internet;

        public QRvCard() : base() { }

        public QRvCard(string data) : base(data) { }

        public override string Pack()
        {
            StringBuilder data = new StringBuilder("BEGIN:VCARD\nVERSION:2.1\n");

            if ((firstName != null && firstName != "") ||
                (lastName  != null && lastName  != ""))
            {
                data.Append("FN:" + firstName + " " + lastName + "\n");
                data.Append("N:" + lastName + ";" + firstName + "\n");
            }

            data.Append(Append("TITLE:", title));
            data.Append(Append("ORG:", org));
            data.Append(Append("URL:", url));
            data.Append(Append("TEL;CELL:", tel_cell));
            data.Append(Append("TEL;WORK;VOICE:", tel_work_voice));
            data.Append(Append("TEL;HOME;VOICE:", tel_home_voice));
            data.Append(Append("EMAIL;HOME;INTERNET:", email_home_internet));
            data.Append(Append("EMAIL;WORK;INTERNET:", email_work_internet));

            if ((street  != null && street  != "") ||
                (zipCode != null && zipCode != "") ||
                (city    != null && city    != "") ||
                (country != null && country != ""))
            {
                data.Append("ADR:;;" + street + ";" + city + ";;" + zipCode + ";" + country + "\n");
            }

            data.Append("END:VCARD");

            return data.ToString();
        }

        private string Append(string name, string data)
        {
            if (data != null && data != "")
                return name + data + "\n";
            
            return "";
        }

        public override void Unpack(string data)
        {
            string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] s = line.Split(new string[] { ":" }, 2, StringSplitOptions.None);

                switch (s[0])
                {
                    case "FN":
                        fn_card = s[1];
                        break;
                    case "N":
                        string[] n = s[1].Split(new string[] { ";" }, StringSplitOptions.None);
                        lastName  = n[0];
                        firstName = n[1];
                        break;
                    case "TITLE":
                        title = s[1];
                        break;
                    case "ORG":
                        org = s[1];
                        break;
                    case "URL":
                        url = s[1];
                        break;
                    case "TEL;CELL":
                        tel_cell = s[1];
                        break;
                    case "TEL;WORK;VOICE":
                        tel_work_voice = s[1];
                        break;
                    case "TEL;HOME;VOICE":
                        tel_home_voice = s[1];
                        break;
                    case "EMAIL;HOME;INTERNET":
                        email_home_internet = s[1];
                        break;
                    case "EMAIL;WORK;INTERNET":
                        email_work_internet = s[1];
                        break;
                    case "ADR":
                        string[] adr = s[1].Split(new string[] { ";" }, StringSplitOptions.None);
                        street = adr[2];
                        city = adr[3];
                        zipCode = adr[5];
                        country = adr[6];
                        break;
                }
            }
        }
    }

    class QRURI : QRData
    {
        public QRURI() : base() { }
        public QRURI(string data) : base(data) { }
    }

    class QREmailAddress : QRData
    {
        public QREmailAddress() : base() { }
        public QREmailAddress(string data) : base(data) { }
    }

    class QREmailMessage : QRData
    {
        public string msg_to;
        public string msg_sub;
        public string msg_body;

        public QREmailMessage() : base() { }

        public QREmailMessage(string data) : base(data) { }

        public override string Pack()
        {
            return "MATMSG:TO:" + msg_to + ";SUB:" + msg_sub + ";BODY:" + msg_body + ";;";
        }

        public override void Unpack(string data)
        {
            string[] s = data.Split(new string[] { "MATMSG:TO:", ";SUB:", ";BODY:" }, 4, StringSplitOptions.None);

            msg_to = s[1];
            msg_sub = s[2];
            msg_body = s[3].Remove(s[3].Length - 2, 2);
        }
    }

    class QRGeo : QRData
    {
        public string latitude;
        public string longitude;
        public string meters;

        public QRGeo() : base() { }

        public QRGeo(string data) : base(data) { }

        public override string Pack()
        {
            return "geo:" + latitude + "," + longitude + "," + meters;
        }

        public override void Unpack(string data)
        {
            string[] s = data.Split(new string[] { "geo:", "," }, 4, StringSplitOptions.None);

            latitude = s[1];
            longitude = s[2];
            meters = s[3];
        }
    }

    class QRSMS : QRData
    {
        public string sms_to;
        public string sms_text;

        public QRSMS() : base() { }

        public QRSMS(string data) : base(data) { }

        public override string Pack()
        {
            return "SMSTO:" + sms_to + ":" + sms_text;
        }

        public override void Unpack(string data)
        {
            string[] s = data.Split(new string[] { "SMSTO:", ":" }, StringSplitOptions.None);

            sms_to = s[1];
            sms_text = s[2];
        }
    }

    class QRCall : QRData
    {
        public string tel;

        public QRCall() : base() { }

        public QRCall(string data) : base(data) { }

        public override string Pack()
        {
            return "tel:" + tel;
        }

        public override void Unpack(string data)
        {
            tel = data.Remove(0, 4);
        }
    }

    class QREvent : QRData
    {
        public string summary;
        public DateTime dtStart;
        public DateTime dtEnd;

        public QREvent() : base() { }

        public QREvent(string data) : base(data) { }

        public override string Pack()
        {
            return "BEGIN:VEVENT\nSUMMARY:" + summary +
                   "\nDTSTART:" + dtStart.Year.ToString("D4") +
                                  dtStart.Month.ToString("D2") +
                                  dtStart.Day.ToString("D2") + "T" +
                                  dtStart.Hour.ToString("D2") +
                                  dtStart.Minute.ToString("D2") +
                                  dtStart.Second.ToString("D2") + "Z" +
                   "\nDTEND:" + dtEnd.Year.ToString("D4") +
                                dtEnd.Month.ToString("D2") +
                                dtEnd.Day.ToString("D2") + "T" +
                                dtEnd.Hour.ToString("D2") +
                                dtEnd.Minute.ToString("D2") +
                                dtEnd.Second.ToString("D2") + "Z" +
                   "\nEND:VEVENT";
        }

        public override void Unpack(string data)
        {
            string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string From = "";
            string To = "";

            foreach (string line in lines)
            {
                string[] attr = line.Split(new string[] { ":" }, 2, StringSplitOptions.None);

                switch (attr[0])
                {
                    case "SUMMARY":
                        summary = attr[1];
                        break;
                    case "DTSTART":
                        From = attr[1];
                        break;
                    case "DTEND":
                        To = attr[1];
                        break;
                }
            }

            dtStart = new DateTime(int.Parse(From.Substring(0, 4)),
                                   int.Parse(From.Substring(4, 2)),
                                   int.Parse(From.Substring(6, 2)),
                                   int.Parse(From.Substring(9, 2)),
                                   int.Parse(From.Substring(11, 2)),
                                   int.Parse(From.Substring(13, 2)));

            dtEnd = new DateTime(int.Parse(To.Substring(0, 4)),
                                 int.Parse(To.Substring(4, 2)),
                                 int.Parse(To.Substring(6, 2)),
                                 int.Parse(To.Substring(9, 2)),
                                 int.Parse(To.Substring(11, 2)),
                                 int.Parse(To.Substring(13, 2)));
        }
    }

    class QRWifi : QRData
    {
        public string encryption;
        public string networkName;
        public string password;
        public bool hidden;

        public QRWifi() : base() { }

        public QRWifi(string data) : base(data) { }

        public override string Pack()
        {
            return "WIFI:T:" + (encryption == "unencrypted" ? "nopass" : encryption) +
                   ";S:" + networkName + ";P:" + (encryption == "unencrypted" ? "" : password) +
                   (hidden ? ";H:true;" : ";;");
        }

        public override void Unpack(string data)
        {
            string[] s = data.Split(new string[] { "WIFI:T:", ";S:", ";P:", ";H:", ";;" }, StringSplitOptions.None);

            encryption = s[1] == "nopass" ? "unencrypted" : s[1];
            networkName = s[2];
            password = s[3];
            hidden = s[4] == "true;" ? true : false;
        }
    }
}

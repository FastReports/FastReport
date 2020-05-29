using FastReport.Utils;
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
            else if (data.StartsWith("SPC"))
                return new QRSwiss(data);
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
                (lastName != null && lastName != ""))
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

            if ((street != null && street != "") ||
                (zipCode != null && zipCode != "") ||
                (city != null && city != "") ||
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
                        lastName = n[0];
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


    class QRSwiss : QRData
    {
        private string br = "\r\n";
        private string alternativeProcedure1, alternativeProcedure2;
        private Iban iban;
        private decimal? amount;
        private Contact creditor, ultimateCreditor, debitor;
        private Currency currency;
        private Reference reference;
        private AdditionalInformation additionalInformation;
        private MyRes res;

        public Iban _Iban { get { return iban; } set { iban = value; } }
        public Contact Creditor { get { return creditor; } set { creditor = value; } }
        public Contact Debitor { get { return debitor; } set { debitor = value; } }
        public Decimal? Amount { get { return amount; } }
        public Currency _Currency { get { return currency; } set { currency = value; } }
        public Reference _Reference { get { return reference; } set { reference = value; } }
        public AdditionalInformation _AdditionalInformation { get { return additionalInformation; } set { additionalInformation = value; } }
        public string AlternativeProcedure1 { get { return alternativeProcedure1; } set { alternativeProcedure1 = value; } }
        public string AlternativeProcedure2 { get { return alternativeProcedure2; } set { alternativeProcedure2 = value; } }

        //public QRSwiss() : base() { }
        public QRSwiss(string data) : base(data) 
        {
            res = new MyRes("Messages,Swiss");
        }

        public QRSwiss(QRSwitchParametres parametres)
        {
            res = new MyRes("Messages,Swiss");
            if (parametres.Iban == null)
                throw new SwissQrCodeException(res.Get("SwissNullIban"));
            if (parametres.Creditor == null)
                throw new SwissQrCodeException(res.Get("SwissNullCreditor"));
            if (parametres.Reference == null)
                throw new SwissQrCodeException(res.Get("SwissNullReference"));
            if (parametres.Currency == null)
                throw new SwissQrCodeException(res.Get("SwissNullCurrency"));

            this.iban = parametres.Iban;
            this.creditor = parametres.Creditor;
            this.additionalInformation = parametres.AdditionalInformation != null ? parametres.AdditionalInformation : new AdditionalInformation(null, null);

            if (parametres.Amount != null && parametres.Amount.ToString().Length > 12)
                throw new SwissQrCodeException(res.Get("SwissAmountLength"));
            this.amount = parametres.Amount;

            this.currency = parametres.Currency.Value;
            this.debitor = parametres.Debitor;

            if (iban.IsQrIban && parametres.Reference.RefType != Reference.ReferenceType.QRR)
                throw new SwissQrCodeException(res.Get("SwissQRIban"));
            if (!iban.IsQrIban && parametres.Reference.RefType == Reference.ReferenceType.QRR)
                throw new SwissQrCodeException(res.Get("SwissNonQRIban"));
            this.reference = parametres.Reference;

            if (parametres.AlternativeProcedure1 != null && parametres.AlternativeProcedure1.Length > 100)
                throw new SwissQrCodeException(res.Get("SwissAltProcedureLength"));
            this.alternativeProcedure1 = parametres.AlternativeProcedure1;
            if (parametres.AlternativeProcedure2 != null && parametres.AlternativeProcedure2.Length > 100)
                throw new SwissQrCodeException(res.Get("SwissAltProcedureLength"));
            this.alternativeProcedure2 = parametres.AlternativeProcedure2;
        }

        public enum Currency
        {
            CHF = 756,
            EUR = 978
        }

        public override void Unpack(string data)
        {
            string[] datasWithR = data.Split('\n');
            string datass = "";
            foreach (string s in datasWithR)
            {
                datass += s;
            }
            string[] datas = datass.Split('\r');

            List<string> vs = new List<string>();

            int counter = 3;

            string infoString = "";

            this.iban = new Iban(datas[counter]);
            counter++;
            infoString = "";

            for (int i = counter; i < counter + 7; i++)
            {
                infoString += datas[i] + '\r';
            }
            counter += 7;
            this.creditor = new Contact(infoString);


            if (datas[counter] != String.Empty)
            {
                infoString = String.Empty;
                for (int i = counter; i < counter + 7; i++)
                {
                    infoString += datas[i] + '\r';
                }
                this.ultimateCreditor = new Contact(infoString);
            }
            counter += 7;
            this.amount = datas[counter] == String.Empty ? amount = null : Decimal.Parse(datas[counter].Replace('.', ','));
            counter++;

            switch (datas[counter])
            {
                case "EUR":
                    this.currency = Currency.EUR;
                    break;
                case "CHF":
                    this.currency = Currency.CHF;
                    break;
            }
            counter++;

            if (datas[counter] != String.Empty)
            {
                infoString = String.Empty;
                for (int i = counter; i < counter + 7; i++)
                {
                    infoString += datas[i] + '\r';
                }
                this.debitor = new Contact(infoString);
            }
            counter += 7;

            infoString = String.Empty;
            for (int i = counter; i < counter + 2; i++)
            {
                infoString += datas[i] + '\r';
            }
            this.reference = new Reference(infoString);
            if (reference.RefType == Reference.ReferenceType.QRR)
                iban.TypeIban = Iban.IbanType.QrIban;
            else
                iban.TypeIban = Iban.IbanType.Iban;
            if(!String.IsNullOrEmpty(reference.ReferenceText))
            {
                if (reference.ChecksumMod10(reference.ReferenceText))
                    reference._ReferenceTextType = Reference.ReferenceTextType.QrReference;
                else if (Regex.IsMatch(reference.ReferenceText, @"^[0-9]+$"))
                    reference._ReferenceTextType = Reference.ReferenceTextType.CreditorReferenceIso11649;
            }
            


            counter += 2;

            infoString = String.Empty;
            for (int i = counter; i < counter + 3; i++)
            {
                infoString += datas[i] + '\r';
            }
            this.additionalInformation = new AdditionalInformation(infoString);
            counter += 3;

            if (datas.Length - 1 >= counter)
            {
                alternativeProcedure1 = datas[counter];
            }
            counter++;

            if (datas.Length - 1 >= counter)
            {
                alternativeProcedure2 = datas[counter];
            }
        }

        public override string Pack()
        {
            string SwissQrCodePayload = "SPC" + br; //QRType
            SwissQrCodePayload += "0200" + br; //Version
            SwissQrCodePayload += "1" + br; //Coding

            //CdtrInf "logical" element
            SwissQrCodePayload += iban.ToString() + br; //IBAN


            //Cdtr "logical" element
            SwissQrCodePayload += creditor.ToString();

            //UltmtCdtr "logical" element
            //Since version 2.0 ultimate creditor was marked as "for future use" and has to be delivered empty in any case!
            //SwissQrCodePayload += string.Concat(Enumerable.Repeat(br, 7).ToArray());
            for (int i = 0; i < 7; i++)
            {
                SwissQrCodePayload += br;
            }


            //CcyAmtDate "logical" element
            //Amoutn has to use . as decimal seperator in any case. See https://www.paymentstandards.ch/dam/downloads/ig-qr-bill-en.pdf page 27.
            //SwissQrCodePayload += (amount != null ? amount.ToString().Replace(",", ".") : string.Empty) + br; //Amt
            if (amount != null)
            {
                string strAmount = amount.ToString();
                if (!strAmount.Contains("."))
                    strAmount = amount.ToString().Replace(",", ".");
                else
                    strAmount += ".00";
                SwissQrCodePayload += strAmount;
            }
            else
                SwissQrCodePayload += string.Empty;
            SwissQrCodePayload += br;

            SwissQrCodePayload += currency + br; //Ccy

            //UltmtDbtr "logical" element
            if (debitor != null)
                SwissQrCodePayload += debitor.ToString();
            else
                for (int i = 0; i < 7; i++)
                {
                    SwissQrCodePayload += br;
                }


            //RmtInf "logical" element
            SwissQrCodePayload += reference.RefType.ToString() + br; //Tp
            SwissQrCodePayload += (!string.IsNullOrEmpty(reference.ReferenceText) ? reference.ReferenceText : string.Empty) + br; //Ref


            //AddInf "logical" element
            SwissQrCodePayload += (!string.IsNullOrEmpty(additionalInformation.UnstructureMessage) ? additionalInformation.UnstructureMessage : string.Empty) + br; //Ustrd
            SwissQrCodePayload += additionalInformation.Trailer + br; //Trailer
            SwissQrCodePayload += (!string.IsNullOrEmpty(additionalInformation.BillInformation) ? additionalInformation.BillInformation : string.Empty) + br; //StrdBkgInf

            //AltPmtInf "logical" element
            if (!string.IsNullOrEmpty(alternativeProcedure1))
                SwissQrCodePayload += alternativeProcedure1.Replace("\n", "") + br; //AltPmt
            if (!string.IsNullOrEmpty(alternativeProcedure2))
                SwissQrCodePayload += alternativeProcedure2.Replace("\n", "") + br; //AltPmt

            //S-QR specification 2.0, chapter 4.2.3
            if (SwissQrCodePayload.EndsWith(br))
                SwissQrCodePayload = SwissQrCodePayload.Remove(SwissQrCodePayload.Length - br.Length);

            return SwissQrCodePayload;
        }

        public class QRSwitchParametres
        {
            #region private fields
            private Iban iban;
            private Currency? currency;
            private Contact creditor;
            private Reference reference;
            private AdditionalInformation additionalInformation;
            private Contact debitor;
            private decimal? amount;
            private string alternativeProcedure1;
            private string alternativeProcedure2;
            #endregion

            #region public properties
            /// <summary>
            /// IBAN object
            /// </summary>
            public Iban Iban { get { return iban; } set { iban = value; } }

            /// <summary>
            /// (either EUR or CHF)
            /// </summary>
            public Currency? Currency { get { return currency; } set { currency = value; } }

            /// <summary>
            /// Creditor (payee) information
            /// </summary>
            public Contact Creditor { get { return creditor; } set { creditor = value; } }

            /// <summary>
            /// Reference information
            /// </summary>
            public Reference Reference { get { return reference; } set { reference = value; } }

            /// <summary>
            /// Can be null
            /// </summary>
            public AdditionalInformation AdditionalInformation { get { return additionalInformation; } set { additionalInformation = value; } }

            /// <summary>
            /// Debitor (payer) information
            /// </summary>
            public Contact Debitor { get { return debitor; } set { debitor = value; } }

            /// <summary>
            /// Amount
            /// </summary>
            public decimal? Amount { get { return amount; } set { amount = value; } }

            /// <summary>
            /// Optional command for alternative processing mode - line 1
            /// </summary>
            public string AlternativeProcedure1 { get { return alternativeProcedure1; } set { alternativeProcedure1 = value; } }

            /// <summary>
            /// Optional command for alternative processing mode - line 2
            /// </summary>
            public string AlternativeProcedure2 { get { return alternativeProcedure2; } set { alternativeProcedure2 = value; } }
            #endregion
        }

        public class AdditionalInformation
        {
            private string unstructuredMessage, billInformation, trailer;

            /// <summary>
            /// Creates an additional information object. Both parameters are optional and must be shorter than 141 chars in combination.
            /// </summary>
            /// <param name="unstructuredMessage">Unstructured text message</param>
            /// <param name="billInformation">Bill information</param>
            public AdditionalInformation(string unstructuredMessage, string billInformation)
            {
                MyRes res = new MyRes("Messages,Swiss");
                if (((unstructuredMessage != null ? unstructuredMessage.Length : 0) + (billInformation != null ? billInformation.Length : 0)) > 140)
                    throw new SwissQrCodeException(res.Get("SwissUnstructBillLength"));
                this.unstructuredMessage = unstructuredMessage;
                this.billInformation = billInformation;
                this.trailer = "EPD";
            }

            public AdditionalInformation(string addInfo)
            {
                string[] data = addInfo.Split('\r');
                this.trailer = data[1].Trim();
                this.unstructuredMessage = data[0].Trim();
                this.billInformation = data[2].Trim();

            }

            public string UnstructureMessage
            {
                get { return !string.IsNullOrEmpty(unstructuredMessage) ? unstructuredMessage.Replace("\n", "") : null; }
                set { this.unstructuredMessage = value; }
            }

            public string BillInformation
            {
                get { return !string.IsNullOrEmpty(billInformation) ? billInformation.Replace("\n", "") : null; }
                set { this.billInformation = value; }
            }

            public string Trailer
            {
                get { return trailer; }
            }
        }        

        public class Reference
        {
            private ReferenceType referenceType;
            private string reference;
            private ReferenceTextType? referenceTextType;


            public ReferenceType RefType
            {
                get { return referenceType; }
                set { referenceType = value; }
            }

            public string ReferenceText
            {
                get { return !string.IsNullOrEmpty(reference) ? reference.Replace("\n", "") : null; }
                set { reference = value; }
            }

            public ReferenceTextType? _ReferenceTextType
            {
                get { return referenceTextType; }
                set { referenceTextType = value; }
            }

            /// <summary>
            /// Creates a reference object which must be passed to the SwissQrCode instance
            /// </summary>
            /// <param name="referenceType">Type of the reference (QRR, SCOR or NON)</param>
            /// <param name="reference">Reference text</param>
            /// <param name="referenceTextType">Type of the reference text (QR-reference or Creditor Reference)</param>                
            public Reference(ReferenceType referenceType, string reference, ReferenceTextType? referenceTextType)
            {
                MyRes res = new MyRes("Messages,Swiss");

                this.referenceType = referenceType;
                this.referenceTextType = referenceTextType;

                if (referenceType == ReferenceType.NON && reference != null)
                    throw new SwissQrCodeException(res.Get("SwissRefTypeNon"));
                if (referenceType != ReferenceType.NON && reference != null && referenceTextType == null)
                    throw new SwissQrCodeException(res.Get("SwissRefTextTypeNon"));
                if (referenceTextType == ReferenceTextType.QrReference && reference != null && (reference.Length > 27))
                    throw new SwissQrCodeException(res.Get("SwissRefQRLength"));
                if (referenceTextType == ReferenceTextType.QrReference && reference != null && !Regex.IsMatch(reference, @"^[0-9]+$"))
                    throw new SwissQrCodeException(res.Get("SwissRefQRNotOnlyDigits"));
                if (referenceTextType == ReferenceTextType.QrReference && reference != null && !ChecksumMod10(reference))
                    throw new SwissQrCodeException(res.Get("SwissRefQRCheckSum"));
                if (referenceTextType == ReferenceTextType.CreditorReferenceIso11649 && reference != null && (reference.Length > 25))
                    throw new SwissQrCodeException(res.Get("SwissRefISOLength"));

                this.reference = reference;
            }


            public Reference(string reference)
            {
                string[] data = reference.Split('\r');

                switch (data[0].Trim())
                {
                    case "QRR":
                        this.referenceType = ReferenceType.QRR;
                        break;
                    case "SCOR":
                        this.referenceType = ReferenceType.SCOR;
                        break;
                    case "NON":
                        this.referenceType = ReferenceType.NON;
                        break;
                }

                this.reference = data[1].Trim();
            }

            /// <summary>
            /// Reference type. When using a QR-IBAN you have to use either "QRR" or "SCOR"
            /// </summary>
            public enum ReferenceType
            {
                QRR,
                SCOR,
                NON
            }
            public enum ReferenceTextType
            {
                QrReference,
                CreditorReferenceIso11649
            }

            public bool ChecksumMod10(string digits)
            {
                if (string.IsNullOrEmpty(digits) || digits.Length < 2)
                    return false;
                int[] mods = new int[] { 0, 9, 4, 6, 8, 2, 7, 1, 3, 5 };

                int remainder = 0;
                for (int i = 0; i < digits.Length - 1; i++)
                {
                    int num = Convert.ToInt32(digits[i]) - 48;
                    remainder = mods[(num + remainder) % 10];
                }
                int checksum = (10 - remainder) % 10;
                return checksum == Convert.ToInt32(digits[digits.Length - 1]) - 48;
            }
        }

        public class Contact
        {
            private List<string> twoLetterCodes;
            private string br = "\r\n";
            private string name, streetOrAddressline1, houseNumberOrAddressline2, zipCode, city, country;
            private AddressType adrType;

            public string Name { get { return name; } set { name = value; } }
            public string StreetOrAddressline { get { return streetOrAddressline1; } set { streetOrAddressline1 = value; } }
            public string HouseNumberOrAddressline { get { return houseNumberOrAddressline2; } set { houseNumberOrAddressline2 = value; } }
            public string ZipCode { get { return zipCode; } set { zipCode = value; } }
            public string City { get { return city; } set { city = value; } }
            public string Country { get { return country; } set { country = value; } }

            /// <summary>
            /// Contact type. Can be used for payee, ultimate payee, etc. with address in structured mode (S).
            /// </summary>
            /// <param name="name">Last name or company (optional first name)</param>
            /// <param name="zipCode">Zip-/Postcode</param>
            /// <param name="city">City name</param>
            /// <param name="country">Two-letter country code as defined in ISO 3166-1</param>
            /// <param name="street">Streetname without house number</param>
            /// <param name="houseNumber">House number</param>
            public Contact(string name, string zipCode, string city, string country, string street, string houseNumber) : this(name, zipCode, city, country, street, houseNumber, AddressType.StructuredAddress)
            {
            }

            /// <summary>
            /// Contact type. Can be used for payee, ultimate payee, etc. with address in combined mode (K).
            /// </summary>
            /// <param name="name">Last name or company (optional first name)</param>
            /// <param name="country">Two-letter country code as defined in ISO 3166-1</param>
            /// <param name="addressLine1">Adress line 1</param>
            /// <param name="addressLine2">Adress line 2</param>
            public Contact(string name, string country, string addressLine1, string addressLine2) : this(name, null, null, country, addressLine1, addressLine2, AddressType.CombinedAddress)
            {
            }

            private Contact(string name, string zipCode, string city, string country, string streetOrAddressline1, string houseNumberOrAddressline2, AddressType addressType)
            {
                twoLetterCodes = ValidTwoLetterCodes();
                MyRes res = new MyRes("Messages,Swiss");
                MyRes resForms = new MyRes("Forms,BarcodeEditor,Swiss");
                //Pattern extracted from https://qr-validation.iso-payments.ch as explained in https://github.com/codebude/QRCoder/issues/97
                string charsetPattern = @"^([a-zA-Z0-9\.,;:'\ \+\-/\(\)?\*\[\]\{\}\\`´~ ]|[!""#%&<>÷=@_$£]|[àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ])*$";

                this.adrType = addressType;

                if (string.IsNullOrEmpty(name))
                    throw new SwissQrCodeContactException(String.Format(res.Get("SwissEmptyProperty"), resForms.Get("Name")));
                if (name.Length > 70)
                    throw new SwissQrCodeContactException(String.Format(res.Get("SwissLengthMore"), resForms.Get("Name"), 71));
                if (!Regex.IsMatch(name, charsetPattern))
                    throw new SwissQrCodeContactException(String.Format(res.Get("SwissPatternError"), resForms.Get("Name")) + charsetPattern);   
                this.name = name;

                if (AddressType.StructuredAddress == this.adrType)
                {
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && (streetOrAddressline1.Length > 70))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissLengthMore"), resForms.Get("Street"), 71));
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && !Regex.IsMatch(streetOrAddressline1, charsetPattern))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissPatternError"), resForms.Get("Street")) + charsetPattern);
                    this.streetOrAddressline1 = streetOrAddressline1;

                    if (!string.IsNullOrEmpty(houseNumberOrAddressline2) && houseNumberOrAddressline2.Length > 16)
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissLengthMore"), resForms.Get("HouseNumber"), 71));
                    this.houseNumberOrAddressline2 = houseNumberOrAddressline2;
                }
                else
                {
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && (streetOrAddressline1.Length > 70))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissLengthMore"), "Address line 1", 71));
                    if (!string.IsNullOrEmpty(streetOrAddressline1) && !Regex.IsMatch(streetOrAddressline1, charsetPattern))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissPatternError"), "Address line 1") + charsetPattern);
                    this.streetOrAddressline1 = streetOrAddressline1;

                    if (string.IsNullOrEmpty(houseNumberOrAddressline2))
                        throw new SwissQrCodeContactException(res.Get("SwissAddressLine2Error"));
                    if (!string.IsNullOrEmpty(houseNumberOrAddressline2) && (houseNumberOrAddressline2.Length > 70))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissLengthMore"), "Address line 2", 71));
                    if (!string.IsNullOrEmpty(houseNumberOrAddressline2) && !Regex.IsMatch(houseNumberOrAddressline2, charsetPattern))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissPatternError"), "Address line 2") + charsetPattern);
                    this.houseNumberOrAddressline2 = houseNumberOrAddressline2;
                }

                if (AddressType.StructuredAddress == this.adrType)
                {
                    if (string.IsNullOrEmpty(zipCode))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissEmptyProperty"), resForms.Get("ZipCode")));
                    if (zipCode.Length > 16)
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissLengthMore"), resForms.Get("ZipCode"), 17));
                    if (!Regex.IsMatch(zipCode, charsetPattern))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissPatternError"), resForms.Get("ZipCode")) + charsetPattern);
                    this.zipCode = zipCode;

                    if (string.IsNullOrEmpty(city))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissEmptyProperty"), resForms.Get("City")));
                    if (city.Length > 35)
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissLengthMore"), resForms.Get("City"), 36));
                    if (!Regex.IsMatch(city, charsetPattern))
                        throw new SwissQrCodeContactException(String.Format(res.Get("SwissPatternError"), resForms.Get("City")) + charsetPattern);
                    this.city = city;
                }
                else
                {
                    this.zipCode = this.city = string.Empty;
                }

                if (!IsValidTwoLetterCode(country))
                    throw new SwissQrCodeContactException(res.Get("SwissCountryTwoLetters"));

                this.country = country;
            }

            private bool IsValidTwoLetterCode(string code)
            {
                return twoLetterCodes.Contains(code);
            }

            private List<string> ValidTwoLetterCodes()
            {
                string[] codes = new string[] { "AF", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BQ", "BA", "BW", "BV", "BR", "IO", "BN", "BG", "BF", "BI", "CV", "KH", "CM", "CA", "KY", "CF", "TD", "CL", "CN", "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "CI", "HR", "CU", "CW", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "SZ", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HM", "VA", "HN", "HK", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IM", "IL", "IT", "JM", "JP", "JE", "JO", "KZ", "KE", "KI", "KP", "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MO", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MQ", "MR", "MU", "YT", "MX", "FM", "MD", "MC", "MN", "ME", "MS", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "NC", "NZ", "NI", "NE", "NG", "NU", "NF", "MP", "MK", "NO", "OM", "PK", "PW", "PS", "PA", "PG", "PY", "PE", "PH", "PN", "PL", "PT", "PR", "QA", "RE", "RO", "RU", "RW", "BL", "SH", "KN", "LC", "MF", "PM", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SX", "SK", "SI", "SB", "SO", "ZA", "GS", "SS", "ES", "LK", "SD", "SR", "SJ", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TK", "TO", "TT", "TN", "TR", "TM", "TC", "TV", "UG", "UA", "AE", "GB", "US", "UM", "UY", "UZ", "VU", "VE", "VN", "VG", "VI", "WF", "EH", "YE", "ZM", "ZW", "AX" };
                List<string> codesList = new List<string>();

                foreach(string str in codes)
                {
                    codesList.Add(str);
                }
                return codesList;
            }

            public Contact(string contact)
            {
                string[] data = contact.Split('\r');
                if (data[0].Trim() == "S")
                    this.adrType = AddressType.StructuredAddress;
                else
                    this.adrType = AddressType.CombinedAddress;
                name = data[1].Trim();
                streetOrAddressline1 = data[2].Trim();
                houseNumberOrAddressline2 = data[3].Trim();
                zipCode = data[4].Trim();
                city = data[5].Trim();
                country = data[6].Trim();
            }

            public override string ToString()
            {
                string contactData = ""; //AdrTp
                if (AddressType.StructuredAddress == adrType)
                    contactData += "S";
                else
                    contactData += "K";
                contactData += br;
                contactData += name.Replace("\n", "") + br; //Name
                contactData += (!string.IsNullOrEmpty(streetOrAddressline1) ? streetOrAddressline1.Replace("\n", "") : string.Empty) + br; //StrtNmOrAdrLine1
                contactData += (!string.IsNullOrEmpty(houseNumberOrAddressline2) ? houseNumberOrAddressline2.Replace("\n", "") : string.Empty) + br; //BldgNbOrAdrLine2
                contactData += zipCode.Replace("\n", "") + br; //PstCd
                contactData += city.Replace("\n", "") + br; //TwnNm
                contactData += country + br; //Ctry
                return contactData;
            }

            public enum AddressType
            {
                StructuredAddress,
                CombinedAddress
            }

            public class SwissQrCodeContactException : SwissQrCodeException
            {
                public SwissQrCodeContactException()
                {
                }

                public SwissQrCodeContactException(string message)
                    : base(message)
                {
                }

                public SwissQrCodeContactException(string message, SwissQrCodeException inner)
                    : base(message, inner)
                {
                }
            }
        }

        public class Iban
        {
            private string iban;
            private IbanType? ibanType;

            public IbanType? TypeIban { get { return ibanType; } set { ibanType = value; } }
            public string _Iban { get { return iban; } set { iban = value; } }

            /// <summary>
            /// IBAN object with type information
            /// </summary>
            /// <param name="iban">IBAN</param>
            /// <param name="ibanType">Type of IBAN (normal or QR-IBAN)</param>
            public Iban(string iban, IbanType ibanType)
            {
                MyRes res = new MyRes("Messages,Swiss");
                if (ibanType == IbanType.Iban && !IsValidIban(iban))
                    throw new SwissQrCodeException(res.Get("SwissIbanNotValid"));
                if (ibanType == IbanType.QrIban && !IsValidQRIban(iban))
                    throw new SwissQrCodeException(res.Get("SwissQRIbanNotValid"));
                if (!iban.StartsWith("CH") && !iban.StartsWith("LI"))
                    throw new SwissQrCodeException("SwissQRStartNotValid");
                this.iban = iban;
                this.ibanType = ibanType;
            }

            public bool IsQrIban
            {
                get { return ibanType == IbanType.QrIban; }
            }

            public Iban(string iban)
            {
                this.iban = iban;
            }

            public override string ToString()
            {
                return iban.Replace("-", "").Replace("\n", "").Replace(" ", "");
            }

            public enum IbanType
            {
                Iban,
                QrIban
            }

            private bool IsValidIban(string iban)
            {
                //Clean IBAN
                string ibanCleared = iban.ToUpper().Replace(" ", "").Replace("-", "");

                //Check for general structure
                bool structurallyValid = Regex.IsMatch(ibanCleared, @"^[a-zA-Z]{2}[0-9]{2}([a-zA-Z0-9]?){16,30}$");

                //Check IBAN checksum
                char[] charSum = (ibanCleared.Substring(4) + ibanCleared.Substring(0, 4)).ToCharArray();
                string sum = "";

                foreach (char c in charSum)
                {
                    sum += (char.IsLetter(c) ? (c - 55).ToString() : c.ToString());
                }
                decimal sumDec;
                if (!decimal.TryParse(sum, out sumDec))
                    return false;
                bool checksumValid = (sumDec % 97) == 1;

                return structurallyValid && checksumValid;
            }

            private bool IsValidQRIban(string iban)
            {
                bool foundQrIid = false;
                try
                {
                    string ibanCleared = iban.ToUpper().Replace(" ", "").Replace("-", "");
                    int possibleQrIid = Convert.ToInt32(ibanCleared.Substring(4, 5));
                    foundQrIid = possibleQrIid >= 30000 && possibleQrIid <= 31999;
                }
                catch { }
                return IsValidIban(iban) && foundQrIid;
            }
        }
    }
}

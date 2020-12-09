using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Security.Policy;
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
            else if (data.StartsWith("ST"))
                return new QRSberBank(data);
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

    class QRSberBank : QRData
    {
        char Separator = '|';

        private string FormatIdentifier="ST";
        public string VersionStandart = "0001";
        public string Encoding="2";

        #region necessary payment details 
        public string Name;
        public string PersonalAcc;
        public string BankName;
        public string BIC;
        public string CorrespAcc;
        #endregion

        #region Additional payment info
        public string Sum;
        public string Purpose;
        public string PayeeINNB;
        public string PayerINN;
        public string DrawerStatus;
        public string KPP;
        public string CBC;
        public string OKTMO;
        public string PaytReason;
        public string TaxPeriod;
        public string DocNo;
        public DateTime DocDate;
        public string TaxPaytKind;
        #endregion

        #region Another additional payment info
        public string LastName;
        public string FirstName;
        public string MiddleName;
        public string PayerAddress;
        public string PersonalAccount;
        public string DocIdx;
        public string PensAcc;
        public string Contract;
        public string PersAccp;
        public string Flat;
        public string Phone;
        public string PayerIdType;
        public string PayerIdNum;
        public string ChildFio;
        public DateTime BirthDate;
        public DateTime PaymTerm;
        public string PaymPeriod;
        public string Category;
        public string ServiceName;
        public string CounterId;
        public string CounterVal;
        public string QuittId;
        public DateTime QuittDate;
        public string InstNum;
        public string ClassNum;
        public string SpecFio;
        public string AddAmount;
        public string RuleId;
        public string ExecId;
        public string RegType;
        public string UIN;
        public string TechCode; //specify numbers somewhere
        #endregion

        public QRSberBank() : base() { }
        public QRSberBank(string data) : base(data) { }
        public override string Pack()
        {
            string result = FormatIdentifier;
            result += VersionStandart;
            result += Encoding;
            result += Separator + "Name=" + Name;
            result += Separator + "PersonalAcc=" + PersonalAcc;
            result += Separator + "BankName=" + BankName;
            result += Separator + "BIC=" + BIC;
            result += Separator + "CorrespAcc=" + CorrespAcc;
            if (!String.IsNullOrWhiteSpace(Sum))  result += Separator + "Sum=" + Sum;
            if (!String.IsNullOrWhiteSpace(Purpose)) result += Separator + "Purpose=" + Purpose;
            if (!String.IsNullOrWhiteSpace(PayeeINNB)) result += Separator + "PayeeINN=" + PayeeINNB;
            if (!String.IsNullOrWhiteSpace(PayerINN)) result += Separator + "PayerINN=" + PayerINN;
            if (!String.IsNullOrWhiteSpace(DrawerStatus)) result += Separator + "DrawerStatus=" + DrawerStatus;
            if (!String.IsNullOrWhiteSpace(KPP)) result += Separator+ "KPP=" + KPP;
            if(!String.IsNullOrWhiteSpace(CBC)) result += Separator+"CBC="+CBC;
            if (!String.IsNullOrWhiteSpace(OKTMO)) result += Separator + "OKTMO=" + OKTMO;
            if (!String.IsNullOrWhiteSpace(PaytReason)) result += Separator + "PaytReason="+PaytReason;
            if (!String.IsNullOrWhiteSpace(TaxPeriod)) result += Separator +"TaxPeriod="+TaxPeriod;
            if (!String.IsNullOrWhiteSpace(DocNo)) result += Separator +"DocNo="+DocNo;
            if (DocDate != DateTime.MinValue)
            {
                result += Separator + "DocDate=" + DocDate.ToString("MM.dd.yyyy");
            }
            if (!String.IsNullOrWhiteSpace(TaxPaytKind)) result += Separator + "TaxPaytKind=" + TaxPaytKind;
            if (!String.IsNullOrWhiteSpace(LastName)) result += Separator + "LastName=" + LastName;
            if (!String.IsNullOrWhiteSpace(FirstName)) result += Separator +"FirstName="+FirstName;
            if (!String.IsNullOrWhiteSpace(MiddleName)) result += Separator +"MiddleName="+MiddleName;
            if (!String.IsNullOrWhiteSpace(PayerAddress)) result += Separator +"PayerAddress="+PayerAddress;
            if (!String.IsNullOrWhiteSpace(PersonalAccount)) result += Separator +"PersonalAccount="+PersonalAccount;
            if (!String.IsNullOrWhiteSpace(DocIdx)) result += Separator + "DocIdx="+DocIdx;
            if (!String.IsNullOrWhiteSpace(PensAcc)) result += Separator +"PensAcc="+PensAcc;
            if (!String.IsNullOrWhiteSpace(Contract)) result += Separator +"Contract="+Contract;
            if (!String.IsNullOrWhiteSpace(PersAccp)) result += Separator +"PersAcc="+PersAccp;
            if (!String.IsNullOrWhiteSpace(Flat)) result += Separator +"Flat="+Flat;
            if (!String.IsNullOrWhiteSpace(Phone)) result += Separator + "Phone=" + Phone; 
            if (!String.IsNullOrWhiteSpace(PayerIdType)) result += Separator +"PayerIdType="+PayerIdType;
            if (!String.IsNullOrWhiteSpace(PayerIdNum)) result += Separator +"PayerIdNum="+PayerIdNum;
            if (!String.IsNullOrWhiteSpace(ChildFio)) result += Separator +"ChildFio="+ChildFio;
            if (BirthDate != DateTime.MinValue)
            {
                result+=Separator+"BirthDate="+BirthDate.ToString("MM.dd.yyyy");
            }
            if (PaymTerm!=DateTime.MinValue)
            {
                result+=Separator+"PaymTerm="+PaymTerm.ToString("MM.dd.yyyy");
            }
            if (!String.IsNullOrWhiteSpace(PaymPeriod)) result += Separator +"PaymPeriod="+PaymPeriod;
            if (!String.IsNullOrWhiteSpace(Category)) result += Separator + "Category=" + Category;
            if (!String.IsNullOrWhiteSpace(ServiceName)) result += Separator + "ServiceName=" + ServiceName;
            if (!String.IsNullOrWhiteSpace(CounterId)) result += Separator + "CounterId=" + CounterId;
            if (!String.IsNullOrWhiteSpace(CounterVal)) result += Separator + "CounterVal=" + CounterVal;
            if (!String.IsNullOrWhiteSpace(QuittId)) result += Separator + "QuittId=" + QuittId;
            if (QuittDate != DateTime.MinValue)
            {
                result+=Separator+"QuittDate="+QuittDate.ToString("MM.dd.yyyy");
            }
            if (!String.IsNullOrWhiteSpace(InstNum)) result += Separator + "InstNum=" + InstNum;
            if (!String.IsNullOrWhiteSpace(ClassNum)) result += Separator + "ClassNum=" + ClassNum;
            if (!String.IsNullOrWhiteSpace(SpecFio)) result += Separator + "SpecFio=" + SpecFio;
            if (!String.IsNullOrWhiteSpace(AddAmount)) result += Separator + "AddAmount=" + AddAmount;
            if (!String.IsNullOrWhiteSpace(RuleId)) result += Separator + "RuleId=" + RuleId;
            if (!String.IsNullOrWhiteSpace(ExecId)) result += Separator + "ExecId=" + ExecId;
            if (!String.IsNullOrWhiteSpace(RegType)) result += Separator + "RegType=" + RegType;
            if (!String.IsNullOrWhiteSpace(UIN)) result += Separator + "UIN=" + UIN;
            if (!String.IsNullOrWhiteSpace(TechCode)) result += Separator + "TechCode=" + TechCode;
                return result;
        }
        public override void Unpack(string data)
        {
            data = RetrieveServiceData(data);
            string[] splitedString = data.Split(Separator);
            foreach(string str in splitedString)
            {
                string[] splitedStr = str.Split(new string[] { "=" }, 2,StringSplitOptions.RemoveEmptyEntries);
                switch (splitedStr[0])
                {
                    case "Name":
                        Name = splitedStr[1];
                        break;
                    case "PersonalAcc":
                        PersonalAcc = splitedStr[1];
                        break;
                    case "BankName":
                        BankName = splitedStr[1];
                        break;
                    case "BIC":
                        BIC = splitedStr[1];
                        break;
                    case "CorrespAcc":
                        CorrespAcc = splitedStr[1];
                        break;
                    case "Sum":
                        Sum = splitedStr[1];
                        break;
                    case "Purpose":
                        Purpose = splitedStr[1];
                        break;
                    case "PayeeINN":
                        PayeeINNB = splitedStr[1];
                        break;
                    case "PayerINN":
                        PayerINN = splitedStr[1];
                        break;
                    case "DrawerStatus":
                        DrawerStatus = splitedStr[1];
                        break;
                    case "KPP":
                        KPP = splitedStr[1];
                        break;
                    case "CBC":
                        CBC = splitedStr[1];
                        break;
                    case "OKTMO":
                        OKTMO = splitedStr[1];
                        break;
                    case "PaytReason":
                        PaytReason = splitedStr[1];
                        break;
                    case "TaxPeriod":
                        TaxPeriod = splitedStr[1];
                        break;
                    case "DocNo":
                        DocNo = splitedStr[1];
                        break;
                    case "DocDate":
                        DocDate = DateTime.Parse(splitedStr[1]);
                        break;
                    case "TaxPaytKind":
                        TaxPaytKind = splitedStr[1];
                        break;
                    case "LastName":
                        LastName = splitedStr[1];
                        break;
                    case "FirstName":
                        FirstName = splitedStr[1];
                        break;
                    case "MiddleName":
                        MiddleName = splitedStr[1];
                        break;
                    case "PayerAddress":
                        PayerAddress = splitedStr[1];
                        break;
                    case "PersonalAccount":
                        PersonalAccount = splitedStr[1];
                        break;
                    case "DocIdx":
                        DocIdx = splitedStr[1];
                        break;
                    case "PensAcc":
                        PensAcc = splitedStr[1];
                        break;
                    case "PersAcc":
                        PersAccp = splitedStr[1];
                        break;
                    case "Contract":
                        Contract = splitedStr[1];
                        break;
                    case "Flat":
                        Flat = splitedStr[1];
                        break;
                    case "Phone":
                        Phone = splitedStr[1];
                        break;
                    case "PayerIdType":
                        PayerIdType = splitedStr[1];
                        break;
                    case "PayerIdNum":
                        PayerIdNum = splitedStr[1];
                        break;
                    case "ChildFio":
                        ChildFio = splitedStr[1];
                        break;
                    case "BirthDate":
                        BirthDate = DateTime.Parse(splitedStr[1]);
                        break;
                    case "PaymTerm":
                        PaymTerm = DateTime.Parse(splitedStr[1]);
                        break;
                    case "PaymPeriod":
                        PaymPeriod = splitedStr[1];
                        break;
                    case "Category":
                        Category = splitedStr[1];
                        break;
                    case "ServiceName":
                        ServiceName = splitedStr[1];
                        break;
                    case "CounterId":
                        CounterId = splitedStr[1];
                        break;
                    case "CounterVal":
                        CounterVal = splitedStr[1];
                        break;
                    case "QuittId":
                        QuittId = splitedStr[1];
                        break;
                    case "QuittDate":
                        QuittDate = DateTime.Parse(splitedStr[1]);
                        break;
                    case "InstNum":
                        InstNum = splitedStr[1];
                        break;
                    case "ClassNum":
                        ClassNum = splitedStr[1];
                        break;
                    case "SpecFio":
                        SpecFio = splitedStr[1];
                        break;
                    case "AddAmount":
                        AddAmount = splitedStr[1];
                        break;
                    case "RuleId":
                        RuleId = splitedStr[1];
                        break;
                    case "ExecId":
                        ExecId = splitedStr[1];
                        break;
                    case "RegType":
                        RegType = splitedStr[1];
                        break;
                    case "UIN":
                        UIN = splitedStr[1];
                        break;
                    case "TechCode":
                        TechCode = splitedStr[1];
                        break;
                }
            }

        }
        private string RetrieveServiceData(string data)
        {
            int firstSeparatorIndex = data.IndexOf(Separator);

            string retrievedData = data.Substring(0, firstSeparatorIndex);
            data = data.Substring(firstSeparatorIndex + 1);
            FormatIdentifier = retrievedData.Substring(0, 2);
            VersionStandart = retrievedData.Substring(2, 4);
            Encoding = retrievedData.Substring(6, 1);

            return data;
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

        public QRSwiss(QRSwissParameters parameters)
        {
            res = new MyRes("Messages,Swiss");
            if (parameters.Iban == null)
                throw new SwissQrCodeException(res.Get("SwissNullIban"));
            if (parameters.Creditor == null)
                throw new SwissQrCodeException(res.Get("SwissNullCreditor"));
            if (parameters.Reference == null)
                throw new SwissQrCodeException(res.Get("SwissNullReference"));
            if (parameters.Currency == null)
                throw new SwissQrCodeException(res.Get("SwissNullCurrency"));

            this.iban = parameters.Iban;
            this.creditor = parameters.Creditor;
            this.additionalInformation = parameters.AdditionalInformation != null ? parameters.AdditionalInformation : new AdditionalInformation(null, null);

            if (parameters.Amount != null && parameters.Amount.ToString().Length > 12)
                throw new SwissQrCodeException(res.Get("SwissAmountLength"));
            this.amount = parameters.Amount;

            this.currency = parameters.Currency.Value;
            this.debitor = parameters.Debitor;

            if (iban.IsQrIban && parameters.Reference.RefType != Reference.ReferenceType.QRR)
                throw new SwissQrCodeException(res.Get("SwissQRIban"));
            if (!iban.IsQrIban && parameters.Reference.RefType == Reference.ReferenceType.QRR)
                throw new SwissQrCodeException(res.Get("SwissNonQRIban"));
            this.reference = parameters.Reference;

            if (parameters.AlternativeProcedure1 != null && parameters.AlternativeProcedure1.Length > 100)
                throw new SwissQrCodeException(res.Get("SwissAltProcedureLength"));
            this.alternativeProcedure1 = parameters.AlternativeProcedure1;
            if (parameters.AlternativeProcedure2 != null && parameters.AlternativeProcedure2.Length > 100)
                throw new SwissQrCodeException(res.Get("SwissAltProcedureLength"));
            this.alternativeProcedure2 = parameters.AlternativeProcedure2;
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
            if (!String.IsNullOrEmpty(reference.ReferenceText))
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

    }
}

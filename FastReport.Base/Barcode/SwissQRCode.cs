using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FastReport.Barcode
{
    /// <summary>
    /// Represents a class that contains all parameters of Swiss QR Code.
    /// </summary>
    public class QRSwissParameters
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

            foreach (string str in codes)
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

    public enum Currency
    {
        CHF = 756,
        EUR = 978
    }

}


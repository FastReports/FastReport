using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FastReport
{
    /// <summary>
    /// Represents a RFID label.
    /// </summary>
    public partial class RFIDLabel : ContainerObject
    {
        /// <summary>
        /// Specifies lock type on write\rewrite bank of data.
        /// </summary>
        public enum LockType
        {
            /// <summary>
            /// Unlock.
            /// </summary>
            Unlock = 'U',

            /// <summary>
            /// Lock.
            /// </summary>
            Lock = 'L',

            /// <summary>
            /// Permanently unlock.
            /// </summary>
            Open = 'O',

            /// <summary>
            /// Permanently lock.
            /// </summary>
            Protect = 'P'
        }

        /// <summary>
        /// Specifies action on error printing of RFID label.
        /// </summary>
        public enum EErrorHandle
        {
            /// <summary>
            /// Skip label.
            /// </summary>
            None = 'N',

            /// <summary>
            /// Place printer in Pause mode.
            /// </summary>
            PauseMode = 'P',

            /// <summary>
            /// Place printer in Error mode.
            /// </summary>
            ErrorMode = 'E',
        }

        private RFIDBank tidBank;
        private RFIDBank userBank;
        private RFIDBank epcBank;
        private string epcFormat;
        private string accessPassword;
        private string accessPasswordDataColumn;
        private string killPassword;
        private string killPasswordDataColumn;
        private LockType lockKillPassword;
        private LockType lockAccessPassword;
        private LockType lockEPCBank;
        private LockType lockUserBank;
        private int startPermaLock;
        private int countPermaLock;
        private bool adaptiveAntenna;
        private short readPower;
        private short writePower;
        private bool useAdjustForEPC;
        private bool rewriteEPCbank;
        private EErrorHandle errorHandle;


        /// <inheritdoc/>
        [Browsable(false)]
        public override FillBase Fill { get => base.Fill; set => base.Fill = value; }

        /// <inheritdoc/>
        [Browsable(false)]
        public override AnchorStyles Anchor { get => base.Anchor; set => base.Anchor = value; }

        /// <inheritdoc/>
        [Browsable(false)]
        public override Border Border { get => base.Border; set => base.Border = value; }

        /// <inheritdoc/>
        [Browsable(false)]
        public override DockStyle Dock { get => base.Dock; set => base.Dock = value; }

        /// <inheritdoc/>
        [Browsable(false)]
        public override bool Visible { get => base.Visible; set => base.Visible = value; }

        /// <summary>
        /// Gets or sets Tag ID memory bank.
        /// </summary>
        public RFIDBank TIDBank
        {
            get
            {
                return tidBank;
            }
            set
            {
                tidBank = value;
            }
        }

        /// <summary>
        /// Gets or sets User memory bank.
        /// </summary>
        public RFIDBank UserBank
        {
            get
            {
                return userBank;
            }
            set
            {
                userBank = value;
            }
        }

        /// <summary>
        /// Gets or sets EPC memory bank.
        /// </summary>
        public RFIDBank EpcBank
        {
            get
            {
                return epcBank;
            }
            set
            {
                epcBank = value;
            }
        }


        /// <summary>
        /// Gets or sets EPC format.
        /// </summary>
        [DefaultValue("96,8,3,3,20,24,38")]
        public string EpcFormat
        {
            get
            {
                return epcFormat;
            }
            set
            {
                epcFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets access password.
        /// </summary>
        public string AccessPassword
        {
            get
            {
                return accessPassword;
            }
            set
            {
                accessPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets kill password.
        /// </summary>
        public string KillPassword
        {
            get
            {
                return killPassword;
            }
            set
            {
                killPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the data column used to set the access password.
        /// </summary>
        public string AccessPasswordDataColumn
        {
            get
            {
                return accessPasswordDataColumn;
            }
            set
            {
                accessPasswordDataColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the data column used to set the kill password.
        /// </summary>
        public string KillPasswordDataColumn
        {
            get
            {
                return killPasswordDataColumn;
            }
            set
            {
                killPasswordDataColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets the lock type for the kill password.
        /// </summary>
        public LockType LockKillPassword
        {
            get
            {
                return lockKillPassword;
            }
            set
            {
                lockKillPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets the lock type for the access password.
        /// </summary>
        public LockType LockAccessPassword
        {
            get
            {
                return lockAccessPassword;
            }
            set
            {
                lockAccessPassword = value;
            }
        }

        /// <summary>
        /// Gets or sets the lock type for the EPC bank.
        /// </summary>
        public LockType LockEPCBank
        {
            get
            {
                return lockEPCBank;
            }
            set
            {
                lockEPCBank = value;
            }
        }

        /// <summary>
        /// Gets or sets the lock type for the user bank.
        /// </summary>
        public LockType LockUserBank
        {
            get
            {
                return lockUserBank;
            }
            set
            {
                lockUserBank = value;
            }
        }

        /// <summary>
        /// Gets or sets the start section for permanent lock of user bank.
        /// </summary>
        public int StartPermaLock
        {
            get
            {
                return startPermaLock;
            }
            set
            {
                startPermaLock = value;
            }
        }

        /// <summary>
        /// Gets or sets the count of section for permanent lock of user bank.
        /// </summary>
        public int CountPermaLock
        {
            get
            {
                return countPermaLock;
            }
            set
            {
                countPermaLock = value;
            }
        }

        /// <summary>
        /// Gets or sets the read power level for the label.
        /// </summary>
        public short ReadPower
        {
            get
            {
                return readPower;
            }
            set
            {
                readPower = value;
            }
        }

        /// <summary>
        /// Gets or sets the write power level for the label.
        /// </summary>
        public short WritePower
        {
            get
            {
                return writePower;
            }
            set
            {
                writePower = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the using auto adjust of bits when write EPC bank.
        /// </summary>
        public bool UseAdjustForEPC
        {
            get
            {
                return useAdjustForEPC;
            }
            set
            {
                useAdjustForEPC = value;
                if (value)
                    rewriteEPCbank = false;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the entire EPC bank will be overwritten.
        /// </summary>
        public bool RewriteEPCbank
        {
            get
            {
                return rewriteEPCbank;
            }
            set
            {
                rewriteEPCbank = value;
                if (value)
                    useAdjustForEPC = false;
            }
        }

        /// <summary>
        /// Gets or sets error handle mode.
        /// </summary>
        public EErrorHandle ErrorHandle
        {
            get
            {
                return errorHandle;
            }
            set
            {
                errorHandle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to use the adaptive antenna property.
        /// </summary>
        public bool AdaptiveAntenna
        {
            get { return adaptiveAntenna; }
            set { adaptiveAntenna = value; }
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();
            GetDataShared();
        }

        private void GetDataShared()
        {
            if (TIDBank.DataColumn.Contains("[") && TIDBank.DataColumn.Contains("]"))
                TIDBank.Data = Report.Calc(TIDBank.DataColumn).ToString();

            if (UserBank.DataColumn.Contains("[") && UserBank.DataColumn.Contains("]"))
                UserBank.Data = Report.Calc(UserBank.DataColumn).ToString();

            if (EpcBank.DataColumn.Contains("[") && EpcBank.DataColumn.Contains("]"))
                EpcBank.Data = Report.Calc(EpcBank.DataColumn).ToString();

            if (AccessPasswordDataColumn.Contains("[") && AccessPasswordDataColumn.Contains("]"))
                AccessPassword = Report.Calc(AccessPasswordDataColumn).ToString();

            if (KillPasswordDataColumn.Contains("[") && KillPasswordDataColumn.Contains("]"))
                KillPassword = Report.Calc(KillPasswordDataColumn).ToString();
        }

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);
            RFIDLabel rFIDLabel = source as RFIDLabel;
            TIDBank.Assign(rFIDLabel.TIDBank);
            EpcBank.Assign(rFIDLabel.EpcBank);
            UserBank.Assign(rFIDLabel.UserBank);
            EpcFormat = rFIDLabel.EpcFormat;
            AccessPassword = rFIDLabel.AccessPassword;
            KillPassword = rFIDLabel.KillPassword;
            AccessPasswordDataColumn = rFIDLabel.AccessPasswordDataColumn;
            KillPasswordDataColumn = rFIDLabel.KillPasswordDataColumn;
            LockAccessPassword = rFIDLabel.LockAccessPassword;
            LockKillPassword = rFIDLabel.LockKillPassword;
            LockEPCBank = rFIDLabel.LockEPCBank;
            LockUserBank = rFIDLabel.LockUserBank;
            StartPermaLock = rFIDLabel.StartPermaLock;
            CountPermaLock = rFIDLabel.CountPermaLock;
            AdaptiveAntenna = rFIDLabel.AdaptiveAntenna;
            ReadPower = rFIDLabel.ReadPower;
            WritePower = rFIDLabel.WritePower;
            UseAdjustForEPC = rFIDLabel.UseAdjustForEPC;
            RewriteEPCbank = rFIDLabel.RewriteEPCbank;
            ErrorHandle = rFIDLabel.ErrorHandle;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            RFIDLabel c = writer.DiffObject as RFIDLabel;
            base.Serialize(writer);

            EpcBank.Serialize(writer, "EpcBank", c.EpcBank);
            TIDBank.Serialize(writer, "TidBank", c.TIDBank);
            UserBank.Serialize(writer, "UserBank", c.UserBank);

            if (EpcFormat != c.EpcFormat)
                writer.WriteStr("EpcFormat", EpcFormat);
            if (AccessPassword != c.AccessPassword)
                writer.WriteStr("AccessPassword", AccessPassword);
            if (KillPassword != c.KillPassword)
                writer.WriteStr("KillPassword", KillPassword);
            if (AccessPasswordDataColumn != c.AccessPasswordDataColumn)
                writer.WriteStr("AccessPasswordDataColumn", AccessPasswordDataColumn);
            if (KillPasswordDataColumn != c.KillPasswordDataColumn)
                writer.WriteStr("KillPasswordDataColumn", KillPasswordDataColumn);
            if (LockAccessPassword != c.LockAccessPassword)
                writer.WriteValue("LockAccessPassword", LockAccessPassword);
            if (LockKillPassword != c.LockKillPassword)
                writer.WriteValue("LockKillPassword", LockKillPassword);
            if (LockEPCBank != c.LockEPCBank)
                writer.WriteValue("LockEPCBlock", LockEPCBank);
            if (LockUserBank != c.LockUserBank)
                writer.WriteValue("LockUserBlock", LockUserBank);
            if (StartPermaLock != c.StartPermaLock)
                writer.WriteInt("StartPermaLock", StartPermaLock);
            if (CountPermaLock != c.CountPermaLock)
                writer.WriteInt("CountPermaLock", CountPermaLock);
            if (AdaptiveAntenna != c.AdaptiveAntenna)
                writer.WriteBool("AdaptiveAntenna", AdaptiveAntenna);
            if (ReadPower != c.ReadPower)
                writer.WriteInt("PowerRead", ReadPower);
            if (WritePower != c.WritePower)
                writer.WriteInt("PowerWrite", WritePower);
            if (UseAdjustForEPC != c.UseAdjustForEPC)
                writer.WriteBool("UseAdjustForEPC", UseAdjustForEPC);
            if (RewriteEPCbank != c.RewriteEPCbank)
                writer.WriteBool("RewriteEPCbank", RewriteEPCbank);
            if (ErrorHandle != c.ErrorHandle)
                writer.WriteValue("ErrorHandle", ErrorHandle);
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            DrawBackground(e);
            DrawMarkers(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            base.Draw(e);
        }

        public RFIDLabel()
        {
            tidBank = new RFIDBank();
            epcBank = new RFIDBank();
            userBank = new RFIDBank();
            epcFormat = "96,8,3,3,20,24,38";
            accessPassword = "00000000";
            killPassword = "00000000";
            accessPasswordDataColumn = "";
            killPasswordDataColumn = "";
            lockKillPassword = LockType.Open;
            lockAccessPassword = LockType.Open;
            lockEPCBank = LockType.Open;
            lockUserBank = LockType.Open;
            startPermaLock = 0;
            countPermaLock = 0;
            adaptiveAntenna = false;
            readPower = 16;
            writePower = 16;
            useAdjustForEPC = false;
            rewriteEPCbank = false;
            ErrorHandle = EErrorHandle.None;
            //Dock = System.Windows.Forms.DockStyle.Fill;
        }

#if !FRCORE
        [TypeConverter(typeof(TypeConverters.FRExpandableObjectConverter))]
#endif
        public class RFIDBank
        {
            private int offset;
            private string data;
            private string dataColumn;
            private Format dataFormat;

            /// <summary>
            /// Specifies the data format of a RFID label bank.
            /// </summary>
            public enum Format
            {
                Hex = 'H',
                ASCII = 'A'
            }

            /// <summary>
            /// Gets or sets a data of bank.
            /// </summary>
            public string Data
            {
                get { return data; }
                set { data = value; }
            }

            /// <summary>
            /// Gets or sets a data column name to this bank.
            /// </summary>
            public string DataColumn
            {
                get { return dataColumn; }
                set { dataColumn = value; }
            }

            /// <summary>
            /// Gets or sets a data offset of bank. Offset measured in 16-bit blocks.
            /// </summary>
            public int Offset
            {
                get { return offset; }
                set { offset = value; }
            }

            /// <summary>
            /// Gets or sets a data format of bank.
            /// </summary>
            public Format DataFormat
            {
                get { return dataFormat; }
                set { dataFormat = value; }
            }

            /// <summary>
            /// Gets count byte of data.
            /// </summary>
            public int CountByte
            {
                get
                {
                    if (dataFormat == Format.ASCII)
                        return Encoding.ASCII.GetByteCount(Data);
                    else
                        return (int)Math.Ceiling((double)Data.Length / 2);
                }
            }

            public RFIDBank()
            {
                dataColumn = "";
                data = "";
                dataFormat = Format.Hex;
                offset = 0;
            }

            /// <summary>
            /// Serializes the object.
            /// </summary>
            public void Serialize(FRWriter writer, string prefix, RFIDBank c)
            {
                if (Data != c.Data)
                    writer.WriteStr(prefix + ".Data", Data);
                if (DataColumn != c.DataColumn)
                    writer.WriteStr(prefix + ".DataColumn", DataColumn);
                if (DataFormat != c.DataFormat)
                    writer.WriteValue(prefix + ".DataFormat", DataFormat);
                if (Offset != c.Offset)
                    writer.WriteInt(prefix + ".Offset", Offset);
            }

            /// <summary>
            /// Copies the contents of another, similar object.
            /// </summary>
            public void Assign(RFIDBank rfidBank)
            {
                DataColumn = rfidBank.DataColumn;
                Data = rfidBank.Data;
                Offset = rfidBank.Offset;
                DataFormat = rfidBank.DataFormat;
            }
        }
    }
}

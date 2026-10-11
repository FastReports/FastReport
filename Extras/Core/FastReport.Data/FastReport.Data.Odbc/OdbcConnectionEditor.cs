using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Forms;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Data.OleDb;

namespace FastReport.Data.ConnectionEditors
{
    internal partial class OdbcConnectionEditor : ConnectionEditorBase
    {
        [DllImport("odbc32.dll")]
        private static extern short SQLAllocEnv(out IntPtr environmentHandle);

        [DllImport("odbc32.dll")]
        private static extern short SQLAllocConnect(IntPtr environmentHandle, out IntPtr connectionHandle);

        [DllImport("odbc32.dll", CharSet = CharSet.Ansi)]
        private static extern short SQLDriverConnect(IntPtr hdbc, IntPtr hwnd, string szConnStrIn, short cbConnStrIn, StringBuilder szConnStrOut, short cbConnStrOutMax, out short pcbConnStrOut, ushort fDriverCompletion);

        [DllImport("odbc32.dll")]
        private static extern short SQLDisconnect(IntPtr connectionHandle);

        [DllImport("odbc32.dll")]
        private static extern short SQLFreeConnect(IntPtr connectionHandle);

        [DllImport("odbc32.dll")]
        private static extern short SQLFreeEnv(IntPtr environmentHandle);

        private static bool SQL_SUCCEEDED(short rc)
        {
            return (rc & -2) == 0;
        }

        private string BuildOdbcConnectionString(string connectionString)
        {
#if AVALONIA
            if (!OperatingSystem.IsWindows())
                return string.Empty;
#endif

            IntPtr hwnd = FindForm().Handle;
            IntPtr environmentHandle = IntPtr.Zero;
            IntPtr connectionHandle = IntPtr.Zero;
            StringBuilder connStrOut = new StringBuilder(0x400);
            short connStrOutLen = 0;
            short rc = 0;

            try
            {
                if (!SQL_SUCCEEDED(SQLAllocEnv(out environmentHandle)))
                    throw new Exception("ODBC SQLAllocEnv Failed");

                if (!SQL_SUCCEEDED(SQLAllocConnect(environmentHandle, out connectionHandle)))
                    throw new Exception("ODBC SQLAllocConnect Failed");

                rc = SQLDriverConnect(connectionHandle, hwnd, connectionString, (short)connectionString.Length, connStrOut, 0x400, out connStrOutLen, 2);
                if (!SQL_SUCCEEDED(rc) && (rc != 100))
                    rc = SQLDriverConnect(connectionHandle, hwnd, null, 0, connStrOut, 0x400, out connStrOutLen, 2);

                if (!SQL_SUCCEEDED(rc) && (rc != 100))
                    throw new Exception("ODBC Connection Failed");

                SQLDisconnect(connectionHandle);
                return connStrOut.ToString();
            }
            finally
            {
                if (connectionHandle != IntPtr.Zero)
                    SQLFreeConnect(connectionHandle);
                if (environmentHandle != IntPtr.Zero)
                    SQLFreeEnv(environmentHandle);
            }
        }

        private void FillDataSources()
        {
            DataTable table = new DataTable();
            table.Locale = CultureInfo.InvariantCulture;
            try
            {
                OleDbDataReader enumerator = OleDbEnumerator.GetEnumerator(
                  Type.GetTypeFromCLSID(new Guid("C8B522CD-5CF3-11ce-ADE5-00AA0044773D")));
                using (enumerator)
                {
                    table.Load(enumerator);
                }
            }
            catch
            {
            }

            cbxDsName.Items.Clear();
            foreach (DataRow row in table.Rows)
            {
                cbxDsName.Items.Add(row["SOURCES_NAME"].ToString());
            }
            cbxDsName.Sorted = true;
        }

        private void rbDsName_CheckedChanged(object sender, EventArgs e)
        {
            cbxDsName.Enabled = rbDsName.Checked;
            tbConnectionString.Enabled = rbConnectionString.Checked;
        }

        private void cbxDsName_SelectedIndexChanged(object sender, EventArgs e)
        {
            OdbcConnectionStringBuilder builder = new OdbcConnectionStringBuilder(tbConnectionString.Text);
            builder["Dsn"] = cbxDsName.SelectedItem as string;
            tbConnectionString.Text = builder.ToString();
        }

        private void tbConnectionString_ButtonClick(object sender, EventArgs e)
        {
            string connString = BuildOdbcConnectionString(ConnectionString);
            if (!String.IsNullOrEmpty(connString))
                ConnectionString = connString;
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
            {
                OdbcConnectionStringBuilder builder = new OdbcConnectionStringBuilder(ConnectionString);
                builder.BrowsableConnectionString = false;
                form.AdvancedProperties = builder;

                if (form.ShowDialog() == DialogResult.OK)
                    ConnectionString = form.AdvancedProperties.ToString();
            }
        }

        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,Odbc");
            gbDatasource.Text = res.Get("DataSource");
            rbDsName.Text = res.Get("DsName");
            rbConnectionString.Text = res.Get("ConnString");
            gbLogin.Text = res.Get("Login");

            res = new MyRes("ConnectionEditors,Common");
            lblUserName.Text = res.Get("UserName");
            lblPassword.Text = res.Get("Password");
            btnAdvanced.Text = Res.Get("Buttons,Advanced");
        }

        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
            tbConnectionString.Image = this.GetImage(68);
        }

        protected override string GetConnectionString()
        {
            OdbcConnectionStringBuilder builder = new OdbcConnectionStringBuilder(tbConnectionString.Text);

            if (!String.IsNullOrEmpty(tbUserName.Text))
                builder.Add("uid", tbUserName.Text);
            else
                builder.Remove("uid");

            if (!String.IsNullOrEmpty(tbPassword.Text))
                builder.Add("pwd", tbPassword.Text);
            else
                builder.Remove("pwd");

            return builder.ToString();
        }

        protected override void SetConnectionString(string value)
        {
            OdbcConnectionStringBuilder builder = new OdbcConnectionStringBuilder(value);

            FillDataSources();
            if (String.IsNullOrEmpty(value))
            {
                rbDsName.Checked = true;
            }
            else if (builder.ContainsKey("Dsn"))
            {
                string dsn = builder["Dsn"] as string;
                if (cbxDsName.Items.Contains(dsn) && builder.Count == 1)
                {
                    cbxDsName.SelectedIndex = cbxDsName.Items.IndexOf(dsn);
                    rbDsName.Checked = true;
                }
                else
                {
                    rbConnectionString.Checked = true;
                }
            }

            if (builder.ContainsKey("uid"))
            {
                tbUserName.Text = builder["uid"] as string;
                builder.Remove("uid");
            }
            if (builder.ContainsKey("pwd"))
            {
                tbPassword.Text = builder["pwd"] as string;
                builder.Remove("pwd");
            }

            // display the string w/o uid, pwd
            tbConnectionString.Text = builder.ToString();

            // update the enabled state
            rbDsName_CheckedChanged(this, EventArgs.Empty);
        }

        public OdbcConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }

    }
}
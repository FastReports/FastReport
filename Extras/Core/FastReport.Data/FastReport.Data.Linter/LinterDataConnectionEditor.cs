using System;
using FastReport.Forms;
using System.Data.Common;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport.Data.ConnectionEditors
{
    public partial class LinterDataConnectionEditor : ConnectionEditorBase
    {
        private DbProviderFactory factory;
        private string connectionString;

        public LinterDataConnectionEditor(DbProviderFactory factory)
        {
            InitializeComponent();
            Localize();
            this.factory = factory;
        }

        private void Localize()
        {
            lbDataSource.Text = Res.Get("ConnectionEditors,Common,DataSource") + ":";
            lbPassword.Text = Res.Get("ConnectionEditors,Common,Password");
            lbUserID.Text = Res.Get("ConnectionEditors,Common,UserName");
            gbDatabase.Text = Res.Get("ConnectionEditors,Common,Database");
            btnAdvanced.Text = Res.Get("Buttons,Advanced");
        }

        protected override string GetConnectionString()
        {
            if (factory == null)
                return "";

            DbConnectionStringBuilder stringBuilder = factory.CreateConnectionStringBuilder();
            stringBuilder.ConnectionString = connectionString;
            stringBuilder["UserID"] = tbUserID.Text;
            stringBuilder["Password"] = tbPassword.Text;
            stringBuilder["DataSource"] = tbDataSource.Text;
            return stringBuilder.ConnectionString;
        }

        protected override void SetConnectionString(string value)
        {
            if (factory == null)
                return;

            DbConnectionStringBuilder stringBuilder = factory.CreateConnectionStringBuilder();
            stringBuilder.ConnectionString = value;
            connectionString = value;
            tbUserID.Text = (string)stringBuilder["UserID"];
            tbPassword.Text = (string)stringBuilder["Password"];
            tbDataSource.Text = (string)stringBuilder["DataSource"];
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            if (factory == null)
                return;

            using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
            {
                var stringBuilder = factory.CreateConnectionStringBuilder();
                stringBuilder.BrowsableConnectionString = false;
                form.AdvancedProperties = stringBuilder;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    ConnectionString = form.AdvancedProperties.ToString();
                }
            }
        }
    }
}

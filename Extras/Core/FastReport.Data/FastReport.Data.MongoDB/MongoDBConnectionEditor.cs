using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using MongoDB.Driver;
using FastReport.Utils;
using FastReport.Data;
using MongoDB.Driver.Core.Configuration;

namespace FastReport.Data
{
    public partial class MongoDBConnectionEditor : ConnectionEditorBase
    {
        private string FConnectionString;

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
            {
                MongoUrlBuilder builder = new MongoUrlBuilder(ConnectionString);
                //  form.AdvancedProperties = builder;
                if (form.ShowDialog() == DialogResult.OK)
                    ConnectionString = form.AdvancedProperties.ToString();
            }
        }

        private void comboBoxScheme_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((ConnectionStringScheme)comboBoxScheme.SelectedItem == ConnectionStringScheme.MongoDBPlusSrv)
                tbPort.Enabled = false;
            else tbPort.Enabled = true;
        }

        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,Common");

            gbServer.Text = res.Get("ServerLogon");
            lblHost.Text = Res.Get("ConnectionEditors,MongoDB,Host");
            lblPort.Text = Res.Get("ConnectionEditors,MongoDB,Port");
            lblUserName.Text = res.Get("UserName");
            lblPassword.Text = res.Get("Password");
            lblScheme.Text = Res.Get("ConnectionEditors,MongoDB,Scheme");

            gbDatabase.Text = res.Get("Database");
            lblDatabase.Text = res.Get("DatabaseName");
            btnAdvanced.Text = Res.Get("Buttons,Advanced");
            cbUseSsl.Text = Res.Get("ConnectionEditors,MongoDB,UseSsl");
        }

        protected override string GetConnectionString()
        {
            MongoUrlBuilder builder = new MongoUrlBuilder();
            builder.Scheme = (ConnectionStringScheme)comboBoxScheme.SelectedItem;
            if (!string.IsNullOrEmpty(FConnectionString))
                builder = new MongoUrlBuilder(FConnectionString);
            builder.Server = new MongoServerAddress(tbHost.Text, int.Parse(tbPort.Text));
            if (!string.IsNullOrEmpty(tbUserName.Text) && !string.IsNullOrEmpty(tbPassword.Text))
            {
                builder.Username = tbUserName.Text;
                builder.Password = tbPassword.Text;
                builder.UseSsl = cbUseSsl.Checked;
            }
            MongoDBDataConnection.dbName = builder.DatabaseName = tbDatabase.Text;
#if NET45
            string url = builder.ToString();
            if(builder.Scheme == ConnectionStringScheme.MongoDBPlusSrv && builder.Server.Port != 27017)
            {
                string portString = builder.Server.Port.ToString();
                url = url.Remove(url.IndexOf(portString) - 1, 1).Replace(portString, "");
            }
            return url;
#else
            return builder.ToMongoUrl().Url;
#endif
        }

        protected override void SetConnectionString(string value)
        {
            FConnectionString = value;
            MongoUrlBuilder builder = new MongoUrlBuilder();
            if (!string.IsNullOrEmpty(value))
                builder = new MongoUrlBuilder(value);

            tbHost.Text = builder.Server.Host;
            tbPort.Text = builder.Server.Port.ToString();
            tbUserName.Text = builder.Username;
            tbPassword.Text = builder.Password;
            tbDatabase.Text = builder.DatabaseName;
        }

        public MongoDBConnectionEditor()
        {
            InitializeComponent();
            comboBoxScheme.DataSource = Enum.GetValues(typeof(ConnectionStringScheme));
            Localize();
        }
    }
}

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

        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,Common");

            gbServer.Text = res.Get("ServerLogon");
            //lblHost.Text = res.Get("Server");
            lblUserName.Text = res.Get("UserName");
            lblPassword.Text = res.Get("Password");

            gbDatabase.Text = res.Get("Database");
            lblDatabase.Text = res.Get("DatabaseName");
            btnAdvanced.Text = Res.Get("Buttons,Advanced");
        }

        protected override string GetConnectionString()
        {
            MongoUrlBuilder builder = new MongoUrlBuilder();
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
            return builder.ToMongoUrl().Url;
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
            Localize();
        }
    }
}

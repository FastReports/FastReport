using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FastReport.Utils;
using ClickHouse.Client.ADO;

namespace FastReport.Data
{
    public partial class ClickHouseConnectionEditor : ConnectionEditorBase
    {
        private string FConnectionString;

        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,Common");
            lblUserName.Text = res.Get("UserName");
            lblPassword.Text = res.Get("Password");
            lblServer.Text = res.Get("Server");
            lblDatabase.Text = res.Get("DatabaseName");

            res = new MyRes("ConnectionEditors,ClickHouse");
            gbServer.Text = res.Get("Server");
            port.Text = res.Get("ServerPort");
            gbDatabase.Text = res.Get("Database");
        }

        protected override string GetConnectionString()
        {
            ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder(FConnectionString);

            builder.Username = tbUserName.Text;
            builder.Host = tbServer.Text;
            builder.Port = ushort.Parse(tbPort.Text);
            builder.Password = tbPassword.Text;
            builder.Database = tbDatabase.Text;

            return builder.ToString();
        }

        protected override void SetConnectionString(string value)
        {
            FConnectionString = value;
            ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder(FConnectionString);
            tbServer.Text = builder.Host;
            tbUserName.Text = builder.Username;
            tbPassword.Text = builder.Password;
            tbDatabase.Text = builder.Database;
            tbPort.Text = builder.Port.ToString();
        }

        public ClickHouseConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }

      
    }
}

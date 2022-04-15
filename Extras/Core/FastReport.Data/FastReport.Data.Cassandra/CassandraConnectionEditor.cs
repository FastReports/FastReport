using Cassandra;
using FastReport.Data.ConnectionEditors;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Data.Cassandra
{
    public partial class CassandraConnectionEditor : ConnectionEditorBase
    {
        private void Localize()
        {
            lblHostName.Text = Res.Get("ConnectionEditors,Cassandra,ContactPoints");
            lblPassword.Text = Res.Get("ConnectionEditors,Common,UserName");
            lblUsername.Text = Res.Get("ConnectionEditors,Common,Password");
            lblPort.Text = Res.Get("ConnectionEditors,Cassandra,Port");
            lblKeyspace.Text = Res.Get("ConnectionEditors,Cassandra,Keyspace");
        }

        protected override string GetConnectionString()
        {
            CassandraConnectionStringBuilder stringBuilder = new CassandraConnectionStringBuilder();
            stringBuilder.ContactPoints = tbHostName.Text.Split(';');
            stringBuilder.Password = tbPassword.Text;
            stringBuilder.Username = tbUsername.Text;
            stringBuilder.Port = (int)tbPort.Value;
            stringBuilder.DefaultKeyspace = tbKeyspace.Text;
            return stringBuilder.ToString();
        }


        protected override void SetConnectionString(string value)
        {
            CassandraConnectionStringBuilder stringBuilder = new CassandraConnectionStringBuilder(value);

            try
            {
                for (int i = 0; i < stringBuilder.ContactPoints.Length; i++)
                {
                    tbHostName.Text += stringBuilder.ContactPoints[i];
                    if (i < stringBuilder.ContactPoints.Length - 1)
                        tbHostName.Text += ";";
                }
            }
            catch { }

            tbPassword.Text = stringBuilder.Password;
            tbUsername.Text = stringBuilder.Username;
            tbPort.Value = stringBuilder.Port;
            tbKeyspace.Text = stringBuilder.DefaultKeyspace;
        }

        public CassandraConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }
    }
}

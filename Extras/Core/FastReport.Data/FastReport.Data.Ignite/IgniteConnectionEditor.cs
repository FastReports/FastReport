using Apache.Ignite.Core.Client;
using FastReport.Data.ConnectionEditors;
using FastReport.Utils;
using System;
using System.Linq;

namespace FastReport.Data
{
    public partial class IgniteConnectionEditor : ConnectionEditorBase
    {
        private void Localize()
        {
            gbServer.Text = Res.Get("ConnectionEditors,Common,ServerLogon");
            lblEndpoints.Text = Res.Get("ConnectionEditors,Ignite,Endpoints");
            lblUsername.Text = Res.Get("ConnectionEditors,Common,UserName");
            lblPassword.Text = Res.Get("ConnectionEditors,Common,Password");
        }

        /// <inheritdoc/>
        protected override string GetConnectionString()
        {
            // Create the Apache Ignite client configuration
            var clientCfg = new IgniteClientConfiguration()
            {
                // Split hosts by ";", trim whitespace, and remove empty entries
                Endpoints = tbEndpoints.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(p => p.Trim())
                                            .Where(p => !string.IsNullOrWhiteSpace(p))
                                            .ToArray()
            };

            if (!string.IsNullOrEmpty(tbUsername.Text) && !string.IsNullOrEmpty(tbPassword.Text))
            {
                clientCfg.UserName = tbUsername.Text;
                clientCfg.Password = tbPassword.Text;
            }

            // Construct the connection string by joining Endpoints with ";"
            var endpoints = string.Join(";", clientCfg.Endpoints); // Join Endpoints with ";"
            var connectionString = $"Endpoints={endpoints}";

            // Append Username and Password to the connection string if they are specified
            if (!string.IsNullOrEmpty(clientCfg.UserName) && !string.IsNullOrEmpty(clientCfg.Password))
            {
                connectionString += $";Username={clientCfg.UserName};Password={clientCfg.Password}";
            }
            return connectionString;
        }

        /// <inheritdoc/>
        protected override void SetConnectionString(string value)
        {
            // Split the connection string into parts using the ";" delimiter
            var parts = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            string endpoints = null;
            string username = null;
            string password = null;

            // Process each part of the connection string
            foreach (var part in parts)
            {
                // Split the part into key and value using the "=" delimiter
                var keyValue = part.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var val = keyValue[1].Trim();

                    switch (key.ToLowerInvariant())
                    {
                        case "endpoints":
                            endpoints = val;
                            break;
                        case "username":
                            username = val;
                            break;
                        case "password":
                            password = val;
                            break;
                    }
                }
            }

            // Populate values in UI components
            tbEndpoints.Text = endpoints ?? "";
            tbUsername.Text = username ?? "";
            tbPassword.Text = password ?? "";
        }

        public IgniteConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }
    }
}
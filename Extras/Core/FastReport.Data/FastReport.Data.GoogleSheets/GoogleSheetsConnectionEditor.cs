using FastReport.Data.ConnectionEditors;
using FastReport.Utils;
using System;
using System.Windows.Forms;

namespace FastReport.Data
{
    /// <summary>
    /// A user control that provides a UI for editing Google Sheets connection properties.
    /// </summary>
    internal partial class GoogleSheetsConnectionEditor : ConnectionEditorBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GoogleSheetsConnectionEditor.
        /// </summary>
        public GoogleSheetsConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }

        #endregion

        #region Events Handlers

        private void btnConfigureAuth_Click(object sender, EventArgs e)
        {
            using (GoogleAuthConfigurationDialog authDialog = new GoogleAuthConfigurationDialog())
            {
                var result = authDialog.ShowDialog();

                if (result != DialogResult.OK)
                {
                    return;
                }
            }
        }

        #endregion

        #region Protected Methods

        ///<inheritdoc/>
        protected override string GetConnectionString()
        {
            GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder
            {
                SpreadsheetId = tbSpreadsheetId.Text,
                FieldNamesInFirstRow = cbxFieldNames.Checked,
                IncludeHiddenSheets = cbxIncludeHidden.Checked
            };
            return builder.ToString();
        }

        ///<inheritdoc/>
        protected override void SetConnectionString(string value)
        {
            GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(value);
            tbSpreadsheetId.Text = builder.SpreadsheetId;
            cbxFieldNames.Checked = builder.FieldNamesInFirstRow;
            cbxIncludeHidden.Checked = builder.IncludeHiddenSheets;
        }

        #endregion

        #region Private Methods

        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,GoogleSheets");
            gbConnectionProperties.Text = res.Get("ConfigureDatabase");
            lblSpreadsheetId.Text = res.Get("SelectSpreadsheetId");
            cbxFieldNames.Text = res.Get("FieldNames");
            cbxIncludeHidden.Text = res.Get("IncludeHidden");
            btnConfigureAuth.Text = res.Get("SignIn");
        }

        #endregion

        #region Public Methods

        ///<inheritdoc/>
        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
        }

        #endregion
    }
}
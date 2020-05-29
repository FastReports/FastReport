namespace FastReport.Utils
{
    static partial class Config
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating that the ASP.NET hosting permission level is set to full trust.
        /// </summary>
        public static bool FullTrust
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value that determines whether to disable some functionality to run in web mode.
        /// Set method does nothing.
        /// </summary>
        /// <remarks>
        /// Use this property if you use FastReport in ASP.Net. Set this property to <b>true</b> <b>before</b>
        /// you access any FastReport.Net objects.
        /// </remarks>
        public static bool WebMode
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Does nothing
        /// </summary>
        private static void RestoreUIStyle()
        {
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        private static void SaveUIStyle()
        {
        }

        private static void RestorePreviewSettings()
        {
        }

        private static void SavePreviewSettings()
        {
        }

        private static void SaveExportOptions()
        {
        }

        private static void RestoreExportOptions()
        {
            ExportsOptions options = ExportsOptions.GetInstance();
            options.RestoreExportOptions();
        }

        #endregion Private Methods

        internal static void DoEvent()
        {
            // do nothing
        }
    }
}
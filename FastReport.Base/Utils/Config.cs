using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace FastReport.Utils
{
    /// <summary>
    /// Contains some configuration properties and settings that will be applied to the FastReport.Net
    /// environment, including Report, Designer and Preview components.
    /// </summary>
    public static partial class Config
    {
#if COMMUNITY
        const string CONFIG_NAME = "FastReport.Community.config";
#else
        const string CONFIG_NAME = "FastReport.config";
#endif
        #region Private Fields

        private static CultureInfo engCultureInfo = new CultureInfo("en-US");
        private static XmlDocument FDoc = new XmlDocument();

        private static string FFolder = null;
        private static string FLogs = "";
        private static ReportSettings FReportSettings = new ReportSettings();
        private static bool FRightToLeft = false;
        private static string FTempFolder = null;
        private static bool FStringOptimization = false;
        private static bool preparedCompressed = true;
        private static bool disableHotkeys = false;

#endregion Private Fields

#region Public Properties

        /// <summary>
        /// Gets or sets the optimization of strings. Is experimental feature.
        /// </summary>
        public static bool IsStringOptimization
        {
            get { return FStringOptimization; }
            set { FStringOptimization = value; }
        }

        /// <summary>
        /// Enable or disable the compression in files with prepared reports (fpx).
        /// </summary>
        public static bool PreparedCompressed
        {
            get { return preparedCompressed; }
            set { preparedCompressed = value; }
        }

        /// <summary>
        /// Gets the application folder.
        /// </summary>
        public static string ApplicationFolder
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        /// Gets an english culture information for localization purposes
        /// </summary>
        public static CultureInfo EngCultureInfo
        {
            get { return engCultureInfo; }
        }

        /// <summary>
        /// Gets or sets the path used to load/save the configuration file.
        /// </summary>
        /// <remarks>
        /// By default, the configuration file is saved to the application local data folder
        /// (C:\Documents and Settings\User_Name\Local Settings\Application Data\FastReport\).
        /// Set this property to "" if you want to store the configuration file in the application folder.
        /// </remarks>
        public static string Folder
        {
            get { return FFolder; }
            set { FFolder = value; }
        }

        /// <summary>
        /// Gets or sets the settings for the Report component.
        /// </summary>
        public static ReportSettings ReportSettings
        {
            get { return FReportSettings; }
            set { FReportSettings = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether RTL layout should be used.
        /// </summary>
        public static bool RightToLeft
        {
            get { return FRightToLeft; }
            set { FRightToLeft = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether hotkeys should be disabled.
        /// </summary>
        public static bool DisableHotkeys
        {
            get { return disableHotkeys; }
            set { disableHotkeys =  value; }
        }

        /// <summary>
        /// Gets the root item of config xml.
        /// </summary>
        public static XmlItem Root
        {
            get { return FDoc.Root; }
        }

        /// <summary>
        /// Gets or sets the path to the temporary folder used to store temporary files.
        /// </summary>
        /// <remarks>
        /// The default value is <b>null</b>, so the system temp folder will be used.
        /// </remarks>
        public static string TempFolder
        {
            get { return FTempFolder; }
            set { FTempFolder = value; }
        }

        /// <summary>
        /// Gets FastReport version.
        /// </summary>
        public static string Version
        {
            get { return typeof(Report).Assembly.GetName().Version.ToString(3); }
        }

#endregion Public Properties

        #region Internal Methods

        internal static string CreateTempFile(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                return Path.GetTempFileName();
            if (dir[dir.Length - 1] != '\\')
                dir += '\\';
            return dir + Path.GetRandomFileName();
        }

        internal static string GetTempFolder()
        {
            return TempFolder == null ? Path.GetTempPath() : TempFolder;
        }

        internal static void Init()
        {
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            WebMode = String.Compare(processName, "iisexpress") == 0 ||
                      String.Compare(processName, "w3wp") == 0;

            if (!WebMode)
                LoadConfig();

            LoadPlugins();

            // init TextRenderingHint.SystemDefault
            // bug in .Net: if you use any other hint before SystemDefault, the SystemDefault will
            // look like SingleBitPerPixel
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                g.DrawString(" ", SystemFonts.DefaultFont, Brushes.Black, 0, 0);
            }
        }

        internal static void WriteLogString(string s)
        {
            WriteLogString(s, false);
        }

        internal static void WriteLogString(string s, bool distinct)
        {
            if (distinct)
            {
                if (FLogs.IndexOf(s + "\r\n") != -1)
                    return;
            }
            FLogs += s + "\r\n";
        }

#endregion Internal Methods

#region Private Methods

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            FDoc.Root.Name = "Config";
            FDoc.AutoIndent = true;
            SaveUIStyle();
            SaveUIOptions();
            if (!WebMode)
            {
                try
                {
                    // added by Samuray
                    if (!Directory.Exists(Folder))
                        Directory.CreateDirectory(Folder);
                    FDoc.Save(Path.Combine(Folder, CONFIG_NAME));
                    if (FLogs != "")
                        File.WriteAllText(Path.Combine(Folder, "FastReport.logs"), FLogs);
                }
                catch
                {
                }
            }
        }

        private static void LoadConfig()
        {
            bool configLoaded = false;
            if (!Config.WebMode)
            {
                try
                {
                    if (Folder == null)
                    {
                        string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        Folder = Path.Combine(baseFolder, "FastReport");
                        // commented by Samuray
                        //if (!Directory.Exists(Folder))
                        //    Directory.CreateDirectory(Folder);
                    }
                    else if (Folder == "")
                        Folder = ApplicationFolder;
                }
                catch
                {
                }

                string fileName = Path.Combine(Folder, CONFIG_NAME);

                if (File.Exists(fileName))
                {
                    try
                    {
                        FDoc.Load(fileName);
                        configLoaded = true;
                    }
                    catch
                    {
                    }
                }

                RestoreUIStyle();
                RestoreDefaultLanguage();
                RestoreUIOptions();
                Res.LoadDefaultLocale();
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            }
            if (!configLoaded)
            {
                // load default config
                using (Stream stream = ResourceLoader.GetStream("FastReport.config"))
                {
                    FDoc.Load(stream);
                }
            }
        }

        private static void LoadPlugins()
        {
            // main assembly initializer
            ProcessAssembly(typeof(Config).Assembly);

            XmlItem pluginsItem = Root.FindItem("Plugins");
            for (int i = 0; i < pluginsItem.Count; i++)
            {
                XmlItem item = pluginsItem[i];
                string pluginName = item.GetProp("Name");

                try
                {
                    ProcessAssembly(Assembly.LoadFrom(pluginName));
                }
                catch
                {
                }
            }
        }

        private static void ProcessAssemblies()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                ProcessAssembly(a);
            }
        }

        private static void ProcessAssembly(Assembly a)
        {
            foreach (Type t in a.GetTypes())
            {
                if (t != typeof(AssemblyInitializerBase) && t.IsSubclassOf(typeof(AssemblyInitializerBase)))
                    Activator.CreateInstance(t);
            }
        }

        private static void RestoreDefaultLanguage()
        {
            XmlItem xi = Root.FindItem("Designer").FindItem("Code");
            string defaultLanguage = xi.GetProp("DefaultScriptLanguage");
            ReportSettings.DefaultLanguage = defaultLanguage == Language.Vb.ToString() ? Language.Vb : Language.CSharp;
        }

        private static void RestoreRightToLeft()
        {
            XmlItem xi = Root.FindItem("UIOptions");
            string rtl = xi.GetProp("RightToLeft");

            if (!String.IsNullOrEmpty(rtl))
            {
                switch (rtl)
                {
                    case "Auto":
                        RightToLeft = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft;
                        break;

                    case "No":
                        RightToLeft = false;
                        break;

                    case "Yes":
                        RightToLeft = true;
                        break;

                    default:
                        RightToLeft = false;
                        break;
                }
            }
        }

        private static void SaveUIOptions()
        {
            XmlItem xi = Root.FindItem("UIOptions");
            xi.SetProp("DisableHotkeys", Converter.ToString(DisableHotkeys));
        }

        private static void RestoreUIOptions()
        {
            RestoreRightToLeft();

            XmlItem xi = Root.FindItem("UIOptions");
            string disableHotkeysStringValue = xi.GetProp("DisableHotkeys");
            if (!String.IsNullOrEmpty(disableHotkeysStringValue))
            {
                disableHotkeys = disableHotkeysStringValue.ToLower() != "false";
            }
        }

#endregion Private Methods
    }
}
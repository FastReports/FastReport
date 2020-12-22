﻿using System;
using System.Drawing;
using System.Drawing.Text;
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
#if MONO
        const string CONFIG_NAME = "FastReport.Mono.config";
#else
        const string CONFIG_NAME = "FastReport.config";
#endif
#endif
        #region Private Fields

        private static CultureInfo engCultureInfo = new CultureInfo("en-US");
        private static XmlDocument FDoc = new XmlDocument();

        private static string FFolder = null;
        private static string FFontListFolder = null;
        private static string FLogs = "";
        private static bool FIsRunningOnMono;
        private static ReportSettings FReportSettings = new ReportSettings();
        private static bool FRightToLeft = false;
        private static string FTempFolder = null;
        private static string systemTempFolder = null;
        private static bool FStringOptimization = false;
        private static bool preparedCompressed = true;
        private static bool disableHotkeys = false;
        private static bool disableBacklight = false;
        private static bool enableScriptSecurity = false;
        private static ScriptSecurityProperties scriptSecurityProps = null;

        #endregion Private Fields

        #region Public Properties
        /// <summary>
        /// Gets a value indicating that the Mono runtime is used.
        /// </summary>
        public static bool IsRunningOnMono
        {
            get { return FIsRunningOnMono; }
        }

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
        /// Gets or sets the path used to font.list file.
        /// </summary>
        /// <remarks>
        /// By default, the font.list file is saved to the FastReport.config folder
        /// If WebMode enabled (or config file path is null), then file is saved in the application folder.
        /// </remarks>
        public static string FontListFolder
        {
            get { return FFontListFolder; }
            set { FFontListFolder = value; }
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
        /// Gets the path to the system temporary folder used to store temporary files.
        /// </summary>
        public static string SystemTempFolder
        {
            get { return systemTempFolder == null ? GetTempPath() : systemTempFolder; }
        }

        /// <summary>
        /// Gets FastReport version.
        /// </summary>
        public static string Version
        {
            get { return typeof(Report).Assembly.GetName().Version.ToString(3); }
        }

        /// <summary>
        /// Called on script compile
        /// </summary>
        public static event EventHandler<ScriptSecurityEventArgs> ScriptCompile;

        /// <summary>
        /// Gets a PrivateFontCollection instance.
        /// </summary>
        public static PrivateFontCollection PrivateFontCollection
        {
            get { return FastReport.TypeConverters.FontConverter.PrivateFontCollection; }
        }

        /// <summary>
        /// Enable report script validation. For WebMode only
        /// </summary>
        public static bool EnableScriptSecurity
        {
            get
            {
                return enableScriptSecurity;
            }
            set
            {
                if (OnEnableScriptSecurityChanged != null)
                    OnEnableScriptSecurityChanged.Invoke(null, null);
                enableScriptSecurity = value;
            }
        }

        /// <summary>
        /// Throws when property EnableScriptSecurity has been changed
        /// </summary>
        public static event EventHandler OnEnableScriptSecurityChanged;

        /// <summary>
        /// Properties of report script validation
        /// </summary>
        public static ScriptSecurityProperties ScriptSecurityProps
        {
            get { return scriptSecurityProps; }
        }

        #endregion Public Properties

        #region Internal Methods

        internal static string CreateTempFile(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                return GetTempFileName();
            return Path.Combine(dir, Path.GetRandomFileName());
        }

        internal static string GetTempFolder()
        {
            return TempFolder == null ? GetTempPath() : TempFolder;
        }

        internal static void Init()
        {
            FIsRunningOnMono = Type.GetType("Mono.Runtime") != null;

#if !(NETSTANDARD2_0 || NETSTANDARD2_1)
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            WebMode = String.Compare(processName, "iisexpress") == 0 ||
                      String.Compare(processName, "w3wp") == 0;
            if (!WebMode)
                LoadConfig();
#else
            WebMode = true;
#endif
            if (WebMode)
            {
#if !COMMUNITY
                RestoreExportOptions();
                enableScriptSecurity = true;    // don't throw event
                scriptSecurityProps = new ScriptSecurityProperties();
#endif
            }
            //RestoreScriptSecurity();
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


        internal static void OnScriptCompile(ScriptSecurityEventArgs e)
        {
            if (ScriptCompile != null)
            {
                ScriptCompile.Invoke(null, e);
            }

            if (!e.IsValid)
            {
                throw new CompilerException(Res.Get("Messages,CompilerError"));
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private static string GetTempFileName()
        {
            return Path.Combine(GetTempFolder(), DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss-") + Path.GetRandomFileName());
        }

        private static string GetTempPath()
        {
            if (!string.IsNullOrEmpty(systemTempFolder))
                return systemTempFolder;

            systemTempFolder = Environment.GetEnvironmentVariable("TMP");
            if (string.IsNullOrEmpty(systemTempFolder))
                systemTempFolder = Environment.GetEnvironmentVariable("TEMP");
            if (string.IsNullOrEmpty(systemTempFolder))
                systemTempFolder = Environment.GetEnvironmentVariable("TMPDIR");
            if (string.IsNullOrEmpty(systemTempFolder))
                systemTempFolder = Path.GetTempPath();

            return systemTempFolder;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            FDoc.Root.Name = "Config";
            FDoc.AutoIndent = true;
            SaveUIStyle();
            SaveUIOptions();
            SavePreviewSettings();
#if !COMMUNITY
                SaveExportOptions();
#endif

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
                RestorePreviewSettings();
#if !COMMUNITY
                RestoreExportOptions();
#endif
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
            ProcessMainAssembly();

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

        private static void ProcessAssembly(Assembly a)
        {
            foreach (Type t in a.GetTypes())
            {
                if (t.IsSubclassOf(typeof(AssemblyInitializerBase)))
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

        /// <summary>
        /// Properties of ScriptSecurity
        /// </summary>
        public class ScriptSecurityProperties
        {
            private readonly string[] defaultBlackList = new[]
                {
                    "GetType",
                    "typeof", 
                    "TypeOf",   // VB
                    "DllImport",
                    "LoadLibrary",
                    "GetProcAddress",
                };

            private string[] blackList;

            /// <summary>
            /// Add stubs for the most dangerous classes (in System.IO, System.Reflection etc) 
            /// </summary>
            public bool AddStubClasses { get; set; } = true;

            /// <summary>
            /// List of keywords that shouldn't be declared in the report script
            /// </summary>
            public string[] BlackList 
            { 
                get { return (string[])blackList.Clone(); }
                set
                {
                    if(value != null)
                    {
                        OnBlackListChanged?.Invoke(this, null);
                        blackList = value;
                    }
                }
            }

            /// <summary>
            /// Throws when <see cref="BlackList"/> has changed
            /// </summary>
            public event EventHandler OnBlackListChanged;

            internal ScriptSecurityProperties()
            {
                SetDefaultBlackList();
            }

            internal ScriptSecurityProperties(string[] blackList)
            {
                this.blackList = blackList;
            }

            /// <summary>
            /// Sets default value for <see cref="BlackList"/>
            /// </summary>
            public void SetDefaultBlackList()
            {
                BlackList = defaultBlackList;
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
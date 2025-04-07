using System;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
#elif MONO
#if WPF
        const string CONFIG_NAME = "FastReport.WPF.config";
#elif AVALONIA
        const string CONFIG_NAME = "FastReport.Avalonia.config";
#else
        const string CONFIG_NAME = "FastReport.Mono.config";
#endif
#else
        const string CONFIG_NAME = "FastReport.config";
#endif
        #region Private Fields

        private static readonly XmlDocument FDoc = new XmlDocument();

        private static readonly string version = typeof(Report).Assembly.GetName().Version.ToString(3);
        private static string FFolder = null;
        private static string FFontListFolder = null;
        private static string FLogs = "";
        private static bool FIsRunningOnMono;
        private static ReportSettings FReportSettings = new ReportSettings();
        private static bool FRightToLeft = false;
        private static string FTempFolder = null;
        private static string systemTempFolder = null;
        private static bool FStringOptimization = true;
        private static bool FWebMode;
        private static bool preparedCompressed = true;
        private static bool enableScriptSecurity = false;
        private static ScriptSecurityProperties scriptSecurityProps = null;
        private static bool forbidLocalData = false;
        private static bool userSetsScriptSecurity = false;
        private static readonly FRPrivateFontCollection privateFontCollection = new FRPrivateFontCollection();
        internal static bool CleanupOnExit;
        private static CompilerSettings compilerSettings = new CompilerSettings();
        private static string applicationFolder;
        private static readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;


        #endregion Private Fields

        #region Public Properties
        /// <summary>
        /// Gets a value indicating that the Mono runtime is used.
        /// </summary>
        internal static bool IsRunningOnMono
        {
            get { return FIsRunningOnMono; }
        }

#if CROSSPLATFORM
        internal static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        internal static bool IsWasm { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));// (int)RuntimeInformation.ProcessArchitecture == 4;  // Architecture.Wasm = 4;
#endif


        /// <summary>
        /// Gets or sets a value indicating is it impossible to specify a local data path in Xml and Csv.
        /// </summary>
        public static bool ForbidLocalData
        {
            get { return forbidLocalData; }
            set { forbidLocalData = value; }
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
        /// Gets or sets the application folder.
        /// </summary>
        public static string ApplicationFolder
        {
            get
            {
                if (applicationFolder == null)
                    return baseDirectory;
                return applicationFolder;
            }
            set
            {
                applicationFolder = value;
            }
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
            get { return version; }
        }

        /// <summary>
        /// Called on script compile
        /// </summary>
        public static event EventHandler<ScriptSecurityEventArgs> ScriptCompile;

        /// <summary>
        /// Gets a PrivateFontCollection instance.
        /// </summary>
        [Obsolete("Use FastReport.FontManager instead")]
        public static FRPrivateFontCollection PrivateFontCollection
        {
            get { return privateFontCollection; }
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
                // 
                userSetsScriptSecurity = true;
                if (value)
                {
                    if (scriptSecurityProps == null)
                        scriptSecurityProps = new ScriptSecurityProperties();
                }
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

        /// <summary>
        /// Settings of report compiler.
        /// </summary>
        public static CompilerSettings CompilerSettings
        {
            get { return compilerSettings; }
            set { compilerSettings = value; }
        }

        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// Warms up the Roslyn compiler asynchronously.
        /// </summary>
        /// <remarks>
        /// Call this method at an application start to warm up the Roslyn compiler (used in NetCore).
        /// </remarks>
        public static async void CompilerWarmup()
        {
            Report.EnsureInit();
            await Task.Run(() =>
            {
                new Report() { ScriptText = "using System; namespace FastReport { public class ReportScript {\r\n} }" }.Compile();
            });
        }
        #endregion

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
            CheckWebMode();

#if !CROSSPLATFORM || AVALONIA
            if (!WebMode)
                LoadConfig();
#endif

            if (!userSetsScriptSecurity && WebMode)
            {
                enableScriptSecurity = true;    // don't throw event
                scriptSecurityProps = new ScriptSecurityProperties();
            }

            LoadPlugins();
#if !COMMUNITY
            RestoreExportOptions();
#endif
#if !SKIA
            InitTextRenderingHint();
#endif
        }

        private static void InitTextRenderingHint()
        {
            // init TextRenderingHint.SystemDefault
            // bug in .Net: if you use any other hint before SystemDefault, the SystemDefault will
            // look like SingleBitPerPixel
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TextRenderingHint = TextRenderingHint.SystemDefault;
                g.DrawString(" ", SystemFonts.DefaultFont, Brushes.Black, 0, 0);
            }
        }

        private static void CheckWebMode()
        {
            // If we/user sets 'WebMode = true' before this check - Config shouln't change it (because check may be incorrect)
            if (!WebMode)
            {
#if NETSTANDARD || NETCOREAPP
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var loadedAsmbly in loadedAssemblies)
                {
                    bool isAspNetCore = loadedAsmbly.GetName().Name.StartsWith("Microsoft.AspNetCore");
                    if (isAspNetCore)
                    {
                        WebMode = true;
                        break;
                    }
                }
#else
                string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                WebMode = String.Compare(processName, "iisexpress") == 0 ||
                              String.Compare(processName, "w3wp") == 0;
#endif
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


        /// <summary>
        /// Invokes ScriptCompile event.
        /// </summary>
        /// <param name="e">Event args.</param>
        /// <exception cref="CompilerException"></exception>
        public static void OnScriptCompile(ScriptSecurityEventArgs e)
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

#if NETSTANDARD || NETCOREAPP
        public static event EventHandler<Code.CodeDom.Compiler.CompilationEventArgs> BeforeEmitCompile;

        internal static void OnBeforeScriptCompilation(object sender, Code.CodeDom.Compiler.CompilationEventArgs e)
        {
            if (BeforeEmitCompile != null)
            {
                BeforeEmitCompile.Invoke(sender, e);
            }
        }
#endif

        #endregion Internal Methods

        #region Private Methods

        private static string GetTempFileName()
        {
            return Path.Combine(GetTempFolder(), SystemFake.DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss-") + Path.GetRandomFileName());
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

        static partial void SaveMaskConnectionStringPassword();

        static partial void SaveConnectionStringVisible();

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            FDoc.Root.Name = "Config";
            FDoc.AutoIndent = true;
            SaveUIStyle();
            SaveUIOptions();
            SaveCompilerSettings();
            SaveAuthServiceUser();
            SaveConnectionStringVisible();
            SaveMaskConnectionStringPassword();
#if !COMMUNITY
            SaveExportOptions();
#endif

            if (!WebMode)
            {
                try
                {
                    if (!Directory.Exists(Folder))
                        Directory.CreateDirectory(Folder);
                    string configFile = Path.Combine(Folder, CONFIG_NAME);
                    if (CleanupOnExit)
                    {
                        File.Delete(configFile);
                    }
                    else
                    {
                        FDoc.Save(configFile);
                    }
                    if (FLogs != "")
                        File.WriteAllText(Path.Combine(Folder, "FastReport.logs"), FLogs);
                }
                catch
                {
                }
            }
        }

        static partial void RestoreMaskConnectionStringPassword();

        static partial void RestoreConnectionStringVisible();

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
                RestoreCompilerSettings();
                Res.LoadDefaultLocale();
                RestoreAuthServiceUser();
                RestoreMaskConnectionStringPassword();
                RestoreConnectionStringVisible();
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
                    var assembly = Assembly.LoadFrom(pluginName);
                    ProcessAssembly(assembly);
                }
                catch
                {
                }
            }

            // For CoreWin
#if (COREWIN || AVALONIA)
            LoadPluginsInCurrentFolder();
#endif
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
            var storage = new StorageService("Designer,Code");
            ReportSettings.DefaultLanguage = storage.GetEnum("DefaultScriptLanguage", Language.CSharp);
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
            private static readonly Regex[] defaultRegexStopList = new[]
                {
                    new Regex("[\\s\\d\\W]GetType[\\s\\W]", RegexOptions.Compiled),
                    new Regex("[\\s\\W]typeof[\\s\\W]", RegexOptions.Compiled),
                    new Regex("[\\s\\W]TypeOf[\\s]", RegexOptions.Compiled),   // VB
                };

            private static readonly string[] defaultStopList = new[]
                {
                   "DllImport",
                   "LoadLibrary",
                   "GetProcAddress",
                };

            private string[] stopList;
            private Regex[] regexStopList;

            /// <summary>
            /// Add stubs for the most dangerous classes (in System.IO, System.Reflection etc) 
            /// </summary>
            public bool AddStubClasses { get; set; } = true;

            /// <summary>
            /// List of keywords that shouldn't be declared in the report script
            /// </summary>
            public string[] StopList
            {
                get { return (string[])stopList.Clone(); }
                set
                {
                    if (value != null)
                    {
                        OnStopListChanged?.Invoke(this, null);
                        stopList = value;
                    }
                }
            }

            /// <summary>
            /// List of keywords in regex format that shouldn't be declared in the report script
            /// </summary>
            public Regex[] RegexStopList
            {
                get { return (Regex[])regexStopList.Clone(); }
                set
                {
                    if (value != null)
                    {
                        OnStopListChanged?.Invoke(this, null);
                        regexStopList = value;
                    }
                }
            }

            /// <summary>
            /// Throws when <see cref="StopList"/> has changed
            /// </summary>
            public event EventHandler OnStopListChanged;

            internal ScriptSecurityProperties()
            {
                SetDefaultStopList();
            }

            internal ScriptSecurityProperties(string[] stopList)
            {
                regexStopList = defaultRegexStopList;
                this.stopList = stopList;
            }

            /// <summary>
            /// Sets default value for <see cref="StopList"/>
            /// </summary>
            public void SetDefaultStopList()
            {
                RegexStopList = defaultRegexStopList;
                StopList = defaultStopList;
            }

        }

        private static void SaveCompilerSettings()
        {
            var storage = new StorageService("CompilerSettings");
            storage.SetStr("Placeholder", CompilerSettings.Placeholder);
            storage.SetEnum("ExceptionBehaviour", CompilerSettings.ExceptionBehaviour);
        }

        private static void RestoreCompilerSettings()
        {
            var storage = new StorageService("CompilerSettings");
            CompilerSettings.Placeholder = storage.GetStr("Placeholder");
            CompilerSettings.ExceptionBehaviour = storage.GetEnum("ExceptionBehaviour", CompilerExceptionBehaviour.Default);
        }

        #endregion Private Methods
    }
}
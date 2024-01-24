using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using Microsoft.Win32;

namespace FastReport.Utils
{
    /// <summary>
    /// Used to get localized values from the language resource file.
    /// </summary>
    /// <remarks>
    /// The resource file used by default is english. To load another locale, call 
    /// the <see cref="Res.LoadLocale(string)"/> method. It should be done at application start
    /// before you use any FastReport classes.
    /// </remarks>
    public static partial class Res
    {
        private static Dictionary<CultureInfo, XmlDocument> LocalesCache { get; }
        internal static CultureInfo CurrentCulture { get; private set; }

        private static XmlDocument FLocale;
        private static XmlDocument FBuiltinLocale;
        private const string FBadResult = "NOT LOCALIZED!";

        /// <summary>
        /// Gets or set the folder that contains localization files (*.frl).
        /// </summary>
        public static string LocaleFolder
        {
            get
            {
                Report.EnsureInit();
                string folder = Config.Root.FindItem("Language").GetProp("Folder");
                // check the registry
#if !CROSSPLATFORM
                if (String.IsNullOrEmpty(folder) && !Config.WebMode)
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("FastReports");
                    if (key != null)
                    {
                        key = key.OpenSubKey("FastReport.Net");
                        if (key != null)
                            folder = (string)key.GetValue("LocalizationFolder", "");
                    }
                }
#endif
                // get application folder
                if (String.IsNullOrEmpty(folder))
                    folder = Config.ApplicationFolder;

                return folder;
            }
            set
            {
                Config.Root.FindItem("Language").SetProp("Folder", value);
            }
        }

        /// <summary>
        /// Returns the current UI locale name, for example "en".
        /// </summary>
        public static string LocaleName
        {
            get
            {
                return FLocale.Root.GetProp("Name");
            }
        }

        internal static string DefaultLocaleName
        {
            get { return Config.Root.FindItem("Language").GetProp("Name"); }
            set { Config.Root.FindItem("Language").SetProp("Name", value); }
        }

        private static void LoadBuiltinLocale()
        {
            FLocale = new XmlDocument();
            FBuiltinLocale = FLocale;
            using (Stream stream = ResourceLoader.GetStream("en.xml"))
            {
                FLocale.Load(stream);
                CultureInfo enCulture;
                try
                {
                    enCulture = CultureInfo.GetCultureInfo("en");
                }
                catch   // InvariantGlobalization mod fix (#939)
                {
                    enCulture = CultureInfo.InvariantCulture;
                }
                CurrentCulture = enCulture;
                if (!LocalesCache.ContainsKey(enCulture))
                    LocalesCache.Add(enCulture, FLocale);
            }
        }

        private static void SetCurrentCulture()
        {
            try
            {
                CurrentCulture = CultureInfo.GetCultureInfo(LocaleName);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Loads the locale from a file.
        /// </summary>
        /// <param name="fileName">The name of the file that contains localized strings.</param>
        public static void LoadLocale(string fileName)
        {
            Report.EnsureInit();

            if (File.Exists(fileName))
            {
                FLocale = new XmlDocument();
                FLocale.Load(fileName);
                SetCurrentCulture();
            }
            else
                LoadEnglishLocale();
        }

        /// <summary>
        /// Loads and caches the locale from <see cref="CultureInfo"/> information.
        /// Notes: *.frl the localization file is looked for in <see cref="LocaleFolder"/>
        /// To work correctly, it is recommended to install FastReport.Localization package
        /// </summary>
        /// <param name="culture"></param>
        public static void LoadLocale(CultureInfo culture)
        {
            if (culture == CultureInfo.InvariantCulture)
            {
                CurrentCulture = culture;
                FLocale = FBuiltinLocale;
                return;
            }

            if (LocalesCache.ContainsKey(culture))
            {
                CurrentCulture = culture;
                FLocale = LocalesCache[culture];
                return;
            }

            // if culture not found, we try find parent culture
            var parent = culture.Parent;
            if (parent != CultureInfo.InvariantCulture)
            {
                if (LocalesCache.ContainsKey(parent))
                {
                    CurrentCulture = parent;
                    FLocale = LocalesCache[parent];
                    return;
                }

                // in some cultures, parent have self parent
                if (parent.Parent != CultureInfo.InvariantCulture)
                    if (LocalesCache.ContainsKey(parent.Parent))
                    {
                        CurrentCulture = parent.Parent;
                        FLocale = LocalesCache[parent.Parent];
                        return;
                    }
            }

            string localeFolder = LocaleFolder;
            string localeFile = string.Empty;

            if (Directory.Exists(localeFolder))
            {
                localeFile = FindLocaleFile(ref culture, localeFolder);
            }

            // Find 'Localization' directory from FastReport.Localization package
            if (string.IsNullOrEmpty(localeFile))
            {
                localeFolder = Path.Combine(Config.ApplicationFolder, "Localization");
                if (Directory.Exists(localeFolder))
                {
                    localeFile = FindLocaleFile(ref culture, localeFolder);
                }
            }

            if (!string.IsNullOrEmpty(localeFile))
            {
                Report.EnsureInit();

                var newLocale = new XmlDocument();
                newLocale.Load(localeFile);
                CurrentCulture = culture;
                FLocale = newLocale;
                LocalesCache.Add(culture, newLocale);
            }
        }

        private static string FindLocaleFile(ref CultureInfo culture, string localeFolder)
        {
            var files = Directory.GetFiles(localeFolder, "*.frl");
            CultureInfo parent = culture.Parent;
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                if (filename == culture.EnglishName)
                {
                    return file;
                }
                else
                {
                    if (filename == parent.EnglishName)
                    {
                        culture = parent;
                        return file;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Loads the locale from a stream.
        /// </summary>
        /// <param name="stream">The stream that contains localized strings.</param>
        public static void LoadLocale(Stream stream)
        {
            Report.EnsureInit();

            FLocale = new XmlDocument();
            FLocale.Load(stream);

            SetCurrentCulture();
        }

        /// <summary>
        /// Loads the english locale.
        /// </summary>
        public static void LoadEnglishLocale()
        {
            CurrentCulture = CultureInfo.GetCultureInfo("en");
            FLocale = FBuiltinLocale;
        }

        internal static void LoadDefaultLocale()
        {
            if (!Directory.Exists(LocaleFolder))
                return;

            if (String.IsNullOrEmpty(DefaultLocaleName))
            {
                // locale is set to "Auto"
                CultureInfo currentCulture = CultureInfo.CurrentCulture;

                LoadLocale(currentCulture);
            }
            else
            {
                // locale is set to specific name
                LoadLocale(Path.Combine(LocaleFolder, DefaultLocaleName + ".frl"));
            }
        }

        /// <summary>
        /// Gets a string with specified ID.
        /// </summary>
        /// <param name="id">The resource ID.</param>
        /// <returns>The localized string.</returns>
        /// <remarks>
        /// Since the locale file is xml-based, it may contain several xml node levels. For example, 
        /// the file contains the following items:
        /// <code>
        /// &lt;Objects&gt;
        ///   &lt;Report Text="Report"/&gt;
        ///   &lt;Bands Text="Bands"&gt;
        ///     &lt;ReportTitle Text="Report Title"/&gt;
        ///   &lt;/Bands&gt;
        /// &lt;/Objects&gt;
        /// </code>
        /// To get the localized "ReportTitle" value, you should pass the following ID
        /// to this method: "Objects,Bands,ReportTitle".
        /// </remarks>
        public static string Get(string id)
        {
            string result = Get(id, FLocale);
            // if no item found, try built-in (english) locale
            if (string.IsNullOrEmpty(result))
            {
                if (FLocale != FBuiltinLocale)
                {
                    result = Get(id, FBuiltinLocale);
                    if (string.IsNullOrEmpty(result))
                        result = id + " " + FBadResult;
                }
                else
                    result = id + " " + FBadResult;
            }
            return result;
        }

        private static string Get(string id, XmlDocument locale)
        {
            string[] categories = id.Split(',');
            XmlItem xi = locale.Root;
            foreach (string category in categories)
            {
                int i = xi.Find(category);
                if (i == -1)
                    return null;
                xi = xi[i];
            }

            return xi.GetProp("Text");
        }

        /// <summary>
        /// Get builtin string.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetBuiltin(string id)
        {
            return Get(id, FBuiltinLocale);
        }

        /// <summary>
        /// Replaces the specified locale string with the new value.
        /// </summary>
        /// <param name="id">Comma-separated path to the existing locale string.</param>
        /// <param name="value">The new string.</param>
        /// <remarks>
        /// Use this method if you want to replace some existing locale value with the new one.
        /// </remarks>
        /// <example>
        /// <code>
        /// Res.Set("Messages,SaveChanges", "My text that will appear when you close the designer");
        /// </code>
        /// </example>
        public static void Set(string id, string value)
        {
            string[] categories = id.Split(',');
            XmlItem xi = FLocale.Root;
            foreach (string category in categories)
            {
                xi = xi.FindItem(category);
            }

            xi.SetProp("Text", value);
        }

        /// <summary>
        /// Tries to get a string with specified ID.
        /// </summary>
        /// <param name="id">The resource ID.</param>
        /// <returns>The localized value, if specified ID exists; otherwise, the ID itself.</returns>
        public static string TryGet(string id)
        {
            string result = Get(id);
            if (result.IndexOf(FBadResult) != -1)
                result = id;
            return result;
        }

        /// <summary>
        /// Tries to get builtin string with specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string TryGetBuiltin(string id)
        {
            string result = GetBuiltin(id);
            if (string.IsNullOrEmpty(result))
                result = id;
            return result;
        }

        /// <summary>
        /// Checks if specified ID exists.
        /// </summary>
        /// <param name="id">The resource ID.</param>
        /// <returns><b>true</b> if specified ID exists.</returns>
        public static bool StringExists(string id)
        {
            return Get(id).IndexOf(FBadResult) == -1;
        }

        static Res()
        {
            LocalesCache = new Dictionary<CultureInfo, XmlDocument>();
            LoadBuiltinLocale();
            ResDesignExt();
        }

        static partial void ResDesignExt();
    }

    /// <summary>
    /// Used to access to resource IDs inside the specified branch.
    /// </summary>
    /// <remarks>
    /// Using the <see cref="Res.Get(string)"/> method, you have to specify the full path to your resource.
    /// Using this class, you can shorten the path:
    /// <code>
    /// // using the Res.Get method
    /// miKeepTogether = new ToolStripMenuItem(Res.Get("ComponentMenu,HeaderBand,KeepTogether"));
    /// miResetPageNumber = new ToolStripMenuItem(Res.Get("ComponentMenu,HeaderBand,ResetPageNumber"));
    /// miRepeatOnEveryPage = new ToolStripMenuItem(Res.Get("ComponentMenu,HeaderBand,RepeatOnEveryPage"));
    /// 
    /// // using MyRes.Get method
    /// MyRes res = new MyRes("ComponentMenu,HeaderBand");
    /// miKeepTogether = new ToolStripMenuItem(res.Get("KeepTogether"));
    /// miResetPageNumber = new ToolStripMenuItem(res.Get("ResetPageNumber"));
    /// miRepeatOnEveryPage = new ToolStripMenuItem(res.Get("RepeatOnEveryPage"));
    /// 
    /// </code>
    /// </remarks>
    public class MyRes
    {
        private string category;

        /// <summary>
        /// Gets a string with specified ID inside the main branch.
        /// </summary>
        /// <param name="id">The resource ID.</param>
        /// <returns>The localized value.</returns>
        public string Get(string id)
        {
            if (id != "")
                return Res.Get(category + "," + id);
            else
                return Res.Get(category);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyRes"/> class with spevified branch.
        /// </summary>
        /// <param name="category">The main resource branch.</param>
        public MyRes(string category)
        {
            this.category = category;
        }
    }


    /// <summary>
    /// Localized CategoryAttribute class.
    /// </summary>
    public class SRCategory : CategoryAttribute
    {
        /// <inheritdoc/>
        protected override string GetLocalizedString(string value)
        {
            return Res.TryGet("Properties,Categories," + value);
        }

        /// <summary>
        /// Initializes a new instance of the SRCategory class.
        /// </summary>
        /// <param name="value">The category name.</param>
        public SRCategory(string value)
          : base(value)
        {
        }
    }
}

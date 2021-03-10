using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
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
    private static XmlDocument FLocale;
    private static XmlDocument FBuiltinLocale;
    private static bool FLocaleLoaded = false;
    private static string FBadResult = "NOT LOCALIZED!";

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
#if !(NETSTANDARD2_0 || NETSTANDARD2_1)
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

        if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
          folder += Path.DirectorySeparatorChar;
        return folder;  
      }
      set { Config.Root.FindItem("Language").SetProp("Folder", value); }
    }

    /// <summary>
    /// Returns the current UI locale name, for example "en".
    /// </summary>
    public static string LocaleName
    {
      get 
      {
        if (!FLocaleLoaded)
          LoadBuiltinLocale();
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
      }
      FLocaleLoaded = true;
    }

        /// <summary>
        /// Loads the locale from a file.
        /// </summary>
        /// <param name="fileName">The name of the file that contains localized strings.</param>
        public static void LoadLocale(string fileName)
    {
      Report.EnsureInit();
      if (!FLocaleLoaded)
        LoadBuiltinLocale();

      if (File.Exists(fileName))
      {
        FLocale = new XmlDocument();
        FLocale.Load(fileName);
      }
      else
        FLocale = FBuiltinLocale;
    }

    /// <summary>
    /// Loads the locale from a stream.
    /// </summary>
    /// <param name="stream">The stream that contains localized strings.</param>
    public static void LoadLocale(Stream stream)
    {
      Report.EnsureInit();
      if (!FLocaleLoaded)
        LoadBuiltinLocale();

      FLocale = new XmlDocument();
      FLocale.Load(stream);
    }

    /// <summary>
    /// Loads the english locale.
    /// </summary>
    public static void LoadEnglishLocale()
    {
      LoadBuiltinLocale();
    }
    
    internal static void LoadDefaultLocale()
    {
      if (!Directory.Exists(LocaleFolder))
        return;
        
      if (String.IsNullOrEmpty(DefaultLocaleName))
      {
        // locale is set to "Auto"
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        string[] files = Directory.GetFiles(LocaleFolder, "*.frl");
        
        foreach (string file in files)
        {
          // find the CultureInfo for given file
          string localeName = Path.GetFileNameWithoutExtension(file);
          CultureInfo localeCulture = null;
          foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
          {
            if (String.Compare(info.EnglishName, localeName, true) == 0)
            {
              localeCulture = info;
              break;
            }
          }

          // if current culture equals to or is subculture of localeCulture, load the locale
          if (currentCulture.Equals(localeCulture) || 
            (currentCulture.Parent != null && currentCulture.Parent.Equals(localeCulture)))
          {
            LoadLocale(file);
            break;
          }
        }
      }
      else
      {
        // locale is set to specific name
        LoadLocale(LocaleFolder + DefaultLocaleName + ".frl");
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
      if (!FLocaleLoaded)
        LoadBuiltinLocale();

      string result = Get(id, FLocale);
      // if no item found, try built-in (english) locale
      if (result.IndexOf(FBadResult) != -1 && FLocale != FBuiltinLocale)
        result = Get(id, FBuiltinLocale);
      return result;
    }

    private static string Get(string id, XmlDocument locale)
    {
      string[] categories = id.Split(new char[] { ',' });
      XmlItem xi = locale.Root;
      foreach (string category in categories)
      {
        int i = xi.Find(category);
        if (i == -1)
          return id + " " + FBadResult;
        xi = xi[i];
      }

      string result = xi.GetProp("Text");
      if (result == "")
        result = id + " " + FBadResult;
      return result;
    }

    /// <summary>
    /// Get builtin string.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetBuiltin(string id)
    {
      if (!FLocaleLoaded)
        LoadBuiltinLocale();

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
      if (!FLocaleLoaded)
        LoadBuiltinLocale();

      string[] categories = id.Split(new char[] { ',' });
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
      if (result.IndexOf(FBadResult) != -1)
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

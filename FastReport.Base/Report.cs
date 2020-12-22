using FastReport.Code;
using FastReport.CrossView;
using FastReport.Data;
using FastReport.Dialog;
using FastReport.Engine;
using FastReport.Export;
using FastReport.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Security;
using System.Text;

namespace FastReport
{
    /// <summary>
    /// Specifies the language of the report's script.
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// The C# language.
        /// </summary>
        CSharp,

        /// <summary>
        /// The VisualBasic.Net language.
        /// </summary>
        Vb
    }

    /// <summary>
    /// Specifies the quality of text rendering.
    /// </summary>
    public enum TextQuality
    {
        /// <summary>
        /// The default text quality, depends on system settings.
        /// </summary>
        Default,

        /// <summary>
        /// The regular quality.
        /// </summary>
        Regular,

        /// <summary>
        /// The "ClearType" quality.
        /// </summary>
        ClearType,

        /// <summary>
        /// The AntiAlias quality. This mode may be used to produce the WYSIWYG text.
        /// </summary>
        AntiAlias,

        /// <summary>
        /// The "SingleBitPerPixel" quality.
        /// </summary>
        SingleBPP,


        /// <summary>
        /// The "SingleBitPerPixelGridFit" quality.
        /// </summary>
        SingleBPPGridFit
    }

    /// <summary>
    /// Specifies the report operation.
    /// </summary>
    public enum ReportOperation
    {
        /// <summary>
        /// Specifies no operation.
        /// </summary>
        None,

        /// <summary>
        /// The report is running.
        /// </summary>
        Running,

        /// <summary>
        /// The report is printing.
        /// </summary>
        Printing,

        /// <summary>
        /// The report is exporting.
        /// </summary>
        Exporting
    }

    /// <summary>
    /// Specifies the page range to print/export.
    /// </summary>
    public enum PageRange
    {
        /// <summary>
        /// Print all pages.
        /// </summary>
        All,

        /// <summary>
        /// Print current page.
        /// </summary>
        Current,

        /// <summary>
        /// Print pages specified in the <b>PageNumbers</b> property of the <b>PrintSettings</b>.
        /// </summary>
        PageNumbers
    }

    /// <summary>
    /// Represents a report object.
    /// </summary>
    /// <remarks>
    /// <para>The instance of this class contains a report. Here are some common
    /// actions that can be performed with this object:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>To load a report, use the <see cref="Load(string)"/>
    ///     method or call static <see cref="FromFile"/> method. </description>
    ///   </item>
    ///   <item>
    ///     <description>To save a report, call the <see cref="Save(string)"/> method.</description>
    ///   </item>
    ///   <item>
    ///     <description>To register application dataset for use it in a report, call one of the
    ///     <b>RegisterData</b> methods.</description>
    ///   </item>
    ///   <item>
    ///     <description>To pass some parameter to a report, use the
    ///     <see cref="SetParameterValue"/> method.</description>
    ///   </item>
    ///   <item>
    ///     <description>To design a report, call the <see cref="Design()"/> method.</description>
    ///   </item>
    ///   <item>
    ///     <description>To run a report and preview it, call the <see cref="Show()"/> method.
    ///     Another way is to call the <see cref="Prepare()"/> method, then call the
    ///     <see cref="ShowPrepared()"/> method.</description>
    ///   </item>
    ///   <item>
    ///     <description>To run a report and print it, call the <see cref="Print"/> method.
    ///     Another way is to call the <see cref="Prepare()"/> method, then call the
    ///     <see cref="PrintPrepared()"/> method.</description>
    ///   </item>
    ///   <item>
    ///     <description>To load/save prepared report, use one of the <b>LoadPrepared</b> and
    ///     <b>SavePrepared</b> methods.</description>
    ///   </item>
    ///   <item>
    ///     <description>To set up some global properties, use the <see cref="Config"/> static class
    ///     or <see cref="EnvironmentSettings"/> component that you can use in the Visual Studio IDE.
    ///     </description>
    ///   </item>
    /// </list>
    /// <para/>The report consists of one or several report pages (pages of the
    /// <see cref="ReportPage"/> type) and/or dialog forms (pages of the <see cref="DialogPage"/> type).
    /// They are stored in the <see cref="Pages"/> collection. In turn, each page may contain report
    /// objects. See the example below how to create a simple report in code.
    /// </remarks>
    /// <example>This example shows how to create a report instance, load it from a file,
    /// register the application data, run and preview.
    /// <code>
    /// Report report = new Report();
    /// report.Load("reportfile.frx");
    /// report.RegisterData(application_dataset);
    /// report.Show();
    /// </code>
    /// <para/>This example shows how to create simple report in code.
    /// <code>
    /// Report report = new Report();
    /// // create the report page
    /// ReportPage page = new ReportPage();
    /// page.Name = "ReportPage1";
    /// // set paper width and height. Note: these properties are measured in millimeters.
    /// page.PaperWidth = 210;
    /// page.PaperHeight = 297;
    /// // add a page to the report
    /// report.Pages.Add(page);
    /// // create report title
    /// page.ReportTitle = new ReportTitleBand();
    /// page.ReportTitle.Name = "ReportTitle1";
    /// page.ReportTitle.Height = Units.Millimeters * 10;
    /// // create Text object and put it to the title
    /// TextObject text = new TextObject();
    /// text.Name = "Text1";
    /// text.Bounds = new RectangleF(0, 0, Units.Millimeters * 100, Units.Millimeters * 5);
    /// page.ReportTitle.Objects.Add(text);
    /// // create data band
    /// DataBand data = new DataBand();
    /// data.Name = "Data1";
    /// data.Height = Units.Millimeters * 10;
    /// // add data band to a page
    /// page.Bands.Add(data);
    /// </code>
    /// </example>

    public partial class Report : Base, IParent, ISupportInitialize
    {
        #region Fields

        private PageCollection pages;
        private Dictionary dictionary;
        private ReportInfo reportInfo;
        private string baseReport;
        private Report baseReportObject;
        private string baseReportAbsolutePath;
        private string fileName;
        private string scriptText;
        private Language scriptLanguage;
        private bool compressed;
        private bool useFileCache;
        private TextQuality textQuality;
        private bool smoothGraphics;
        private string password;
        private bool convertNulls;
        private bool doublePass;
        private bool autoFillDataSet;
        private int initialPageNumber;
        private int maxPages;
        private string startReportEvent;
        private string finishReportEvent;
        private StyleCollection styles;
        private CodeHelperBase codeHelper;
        private GraphicCache graphicCache;
        private string[] referencedAssemblies;
        private Hashtable cachedDataItems;
        private AssemblyCollection assemblies;
        private FastReport.Preview.PreparedPages preparedPages;
        private ReportEngine engine;
        private bool aborted;
        private bool modified;
        private Bitmap measureBitmap;
        private Graphics measureGraphics;
        private bool storeInResources;
        private PermissionSet scriptRestrictions;
        private ReportOperation operation;
        private int tickCount;
        private bool needCompile;
        private bool needRefresh;
        private bool initializing;
        private object initializeData;
        private string initializeDataName;
        private object tag;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Occurs when calc execution is started.
        /// </summary>
        public event CustomCalcEventHandler CustomCalc;

        /// <summary>
        /// Occurs when report is inherited and trying to load a base report.
        /// </summary>
        /// <remarks>
        /// Typical use of this event is to load the base report from a database instead of a file.
        /// </remarks>
        public event CustomLoadEventHandler LoadBaseReport;

        /// <summary>
        /// Occurs when report execution is started.
        /// </summary>
        public event EventHandler StartReport;

        /// <summary>
        /// Occurs when report execution is finished.
        /// </summary>
        public event EventHandler FinishReport;

        /// <summary>
        /// Occurs before export to set custom export parameters.
        /// </summary>
        public event EventHandler<ExportParametersEventArgs> ExportParameters;

        /// <summary>
        /// Gets the pages contained in this report.
        /// </summary>
        /// <remarks>
        /// This property contains pages of all types (report and dialog). Use the <b>is/as</b> operators
        /// if you want to work with pages of <b>ReportPage</b> type.
        /// </remarks>
        /// <example>The following code demonstrates how to access the first report page:
        /// <code>
        /// ReportPage page1 = report1.Pages[0] as ReportPage;
        /// </code>
        /// </example>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PageCollection Pages
        {
            get { return pages; }
        }

        /// <summary>
        /// Gets the report's data.
        /// </summary>
        /// <remarks>
        /// The dictionary contains all data items such as connections, data sources, parameters,
        /// system variables.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary Dictionary
        {
            get { return dictionary; }
            set
            {
                SetProp(dictionary, value);
                dictionary = value;
            }
        }

        /// <summary>
        /// Gets the collection of report parameters.
        /// </summary>
        /// <remarks>
        /// <para>Parameters are displayed in the "Data" window under the "Parameters" node.</para>
        /// <para>Typical use of parameters is to pass some static data from the application to the report.
        /// You can print such data, use it in the data row filter, script etc. </para>
        /// <para>Another way to use parameters is to define some reusable piece of code, for example,
        /// to define an expression that will return the concatenation of first and second employee name.
        /// In this case, you set the parameter's <b>Expression</b> property to something like this:
        /// [Employees.FirstName] + " " + [Employees.LastName]. Now this parameter may be used in the report
        /// to print full employee name. Each time you access such parameter, it will calculate the expression
        /// and return its value. </para>
        /// <para>You can create nested parameters. To do this, add the new <b>Parameter</b> to the
        /// <b>Parameters</b> collection of the root parameter. To access the nested parameter, you may use the
        /// <see cref="GetParameter"/> method.</para>
        /// <para>To get or set the parameter's value, use the <see cref="GetParameterValue"/> and
        /// <see cref="SetParameterValue"/> methods. To set the parameter's expression, use the
        /// <see cref="GetParameter"/> method that returns a <b>Parameter</b> object and set its
        /// <b>Expression</b> property.</para>
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ParameterCollection Parameters
        {
            get { return dictionary.Parameters; }
        }

        /// <summary>
        /// Gets or sets the report information such as report name, author, description etc.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Design")]
        public ReportInfo ReportInfo
        {
            get { return reportInfo; }
            set { reportInfo = value; }
        }

        /// <summary>
        /// Gets or sets the base report file name.
        /// </summary>
        /// <remarks>
        /// This property contains the name of a report file this report is inherited from.
        /// <b>Note:</b> setting this property to non-empty value will clear the report and
        /// load the base file into it.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string BaseReport
        {
            get { return baseReport; }
            set { SetBaseReport(value); }
        }

        /// <summary>
        /// Gets or sets the absolute path to the parent report.
        /// </summary>
        /// <remarks>
        /// This property contains the absolute path to the parent report.
        /// </remarks>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string BaseReportAbsolutePath
        {
            get { return baseReportAbsolutePath; }
            set { baseReportAbsolutePath = value; }
        }

        /// <summary>
        /// Gets or sets the name of a file the report was loaded from.
        /// </summary>
        /// <remarks>
        /// This property is used to support the FastReport.Net infrastructure;
        /// typically you don't need to use it.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Gets or sets the report script.
        /// </summary>
        /// <remarks>
        /// <para>The script contains the <b>ReportScript</b> class that contains all report objects'
        /// event handlers and own items such as private fields, properties, methods etc. The script
        /// contains only items written by you. Unlike other report generators, the script does not
        /// contain report objects declarations, initialization code. It is added automatically when
        /// you run the report.</para>
        /// <para>By default this property contains an empty script text. You may see it in the designer
        /// when you switch to the Code window.</para>
        /// <para>If you set this property programmatically, you have to declare the <b>FastReport</b>
        /// namespace and the <b>ReportScript</b> class in it. Do not declare report items (such as bands,
        /// objects, etc) in the <b>ReportScript</b> class: the report engine does this automatically when
        /// you run the report.</para>
        /// <para><b>Security note:</b> since the report script is compiled into .NET assembly, it allows
        /// you to do ANYTHING. For example, you may create a script that will read/write files from/to a disk.
        /// To restrict such operations, use the <see cref="ScriptRestrictions"/> property.</para>
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ScriptText
        {
            get { return scriptText; }
            set { scriptText = value; }
        }

        /// <summary>
        /// Gets or sets the script language of this report.
        /// </summary>
        /// <remarks>
        /// Note: changing this property will reset the report script to default empty script.
        /// </remarks>
        [DefaultValue(Language.CSharp)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Script")]
        public Language ScriptLanguage
        {
            get { return scriptLanguage; }
            set
            {
                bool needClear = scriptLanguage != value;
                scriptLanguage = value;
                if (scriptLanguage == Language.CSharp)
                    codeHelper = new CsCodeHelper(this);
                else
                    codeHelper = new VbCodeHelper(this);
                if (needClear)
                    scriptText = codeHelper.EmptyScript();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the null DB value must be converted to zero, false or
        /// empty string depending on the data column type.
        /// </summary>
        /// <remarks>
        /// This property is <b>true</b> by default. If you set it to <b>false</b>, you should check
        /// the DB value before you do something with it (for example, typecast it to any type, use it
        /// in a expression etc.)
        /// </remarks>
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Engine")]
        public bool ConvertNulls
        {
            get { return convertNulls; }
            set { convertNulls = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether the report engine should perform the second pass.
        /// </summary>
        /// <remarks>
        /// <para>Typically the second pass is necessary to print the number of total pages. It also
        /// may be used to perform some calculations on the first pass and print its results on the
        /// second pass.</para>
        /// <para>Use the <b>Engine.FirstPass</b>, <b>Engine.FinalPass</b> properties to determine which
        /// pass the engine is performing now.</para>
        /// </remarks>
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Engine")]
        public bool DoublePass
        {
            get { return doublePass; }
            set { doublePass = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to compress the report file.
        /// </summary>
        /// <remarks>
        /// The report file is compressed using the Gzip algorithm. So you can open the
        /// compressed report in any zip-compatible archiver.
        /// </remarks>
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Misc")]
        public bool Compressed
        {
            get { return compressed; }
            set { compressed = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to use the file cache rather than memory
        /// to store the prepared report pages.
        /// </summary>
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Engine")]
        public bool UseFileCache
        {
            get { return useFileCache; }
            set { useFileCache = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies the quality of text rendering.
        /// </summary>
        /// <remarks>
        /// <b>Note:</b> the default property value is <b>TextQuality.Default</b>. That means the report
        /// may look different depending on OS settings. This property does not affect the printout.
        /// </remarks>
        [DefaultValue(TextQuality.Default)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Misc")]
        public TextQuality TextQuality
        {
            get { return textQuality; }
            set { textQuality = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies if the graphic objects such as bitmaps
        /// and shapes should be displayed smoothly.
        /// </summary>
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Misc")]
        public bool SmoothGraphics
        {
            get { return smoothGraphics; }
            set { smoothGraphics = value; }
        }

        /// <summary>
        /// Gets or sets the report password.
        /// </summary>
        /// <remarks>
        /// <para>When you try to load the password-protected report, you will be asked
        /// for a password. You also may specify the password in this property before loading
        /// the report. In this case the report will load silently.</para>
        /// <para>Password-protected report file is crypted using Rijndael algorithm.
        /// Do not forget your password! It will be hard or even impossible to open
        /// the protected file in this case.</para>
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to automatically fill
        /// DataSet registered with <b>RegisterData</b> call.
        /// </summary>
        /// <remarks>
        /// If this property is <b>true</b> (by default), FastReport will automatically fill
        /// the DataSet with data when you trying to run a report. Set it to <b>false</b> if
        /// you want to fill the DataSet by yourself.
        /// </remarks>
        [DefaultValue(true)]
        [SRCategory("Misc")]
        public bool AutoFillDataSet
        {
            get { return autoFillDataSet; }
            set { autoFillDataSet = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of generated pages in a prepared report.
        /// </summary>
        /// <remarks>
        /// Use this property to limit the number of pages in a prepared report.
        /// </remarks>
        [DefaultValue(0)]
        [SRCategory("Misc")]
        public int MaxPages
        {
            get { return maxPages; }
            set { maxPages = value; }
        }

        /// <summary>
        /// Gets or sets the collection of styles used in this report.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Misc")]
        public StyleCollection Styles
        {
            get { return styles; }
            set { styles = value; }
        }

        /// <summary>
        /// Gets or sets an array of assembly names that will be used to compile the report script.
        /// </summary>
        /// <remarks>
        /// By default this property contains the following assemblies: "System.dll", "System.Drawing.dll",
        /// "System.Windows.Forms.dll", "System.Data.dll", "System.Xml.dll". If your script uses some types
        /// from another assemblies, you have to add them to this property.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Script")]
        public string[] ReferencedAssemblies
        {
            get { return referencedAssemblies; }
            set {
                // fix for old reports with "System.Windows.Forms.DataVisualization" in referenced assemblies 
                for (int i = 0; i < value.Length;i++)
                {
                    value[i] = value[i].Replace("System.Windows.Forms.DataVisualization", "FastReport.DataVisualization");
                }
                referencedAssemblies = value; 
            }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the report starts.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Build")]
        public string StartReportEvent
        {
            get { return startReportEvent; }
            set { startReportEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the report is finished.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Build")]
        public string FinishReportEvent
        {
            get { return finishReportEvent; }
            set { finishReportEvent = value; }
        }

        /// <summary>
        /// Gets a value indicating that report execution was aborted.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Aborted
        {
            get
            {
                Config.DoEvent();
                return aborted;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to store the report in the application resources.
        /// Use this property in the MS Visual Studio IDE only.
        /// </summary>
        /// <remarks>
        /// By default this property is <b>true</b>. When set to <b>false</b>, you should store your report
        /// in a file.
        /// </remarks>
        [DefaultValue(true)]
        [SRCategory("Design")]
        public bool StoreInResources
        {
            get { return storeInResources; }
            set { storeInResources = value; }
        }

        /// <summary>
        /// Gets or sets the resource string that contains the report.
        /// </summary>
        /// <remarks>
        /// This property is used by the MS Visual Studio to store the report. Do not use it directly.
        /// </remarks>
        [Browsable(false)]
        [Localizable(true)]
        public string ReportResourceString
        {
            get
            {
                if (!StoreInResources)
                    return "";
                return SaveToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    Clear();
                    return;
                }
                LoadFromString(value);
            }
        }

        /// <summary>
        /// Gets a value indicating that this report contains dialog forms.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDialogs
        {
            get
            {
                foreach (PageBase page in Pages)
                {
                    if (page is DialogPage)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a set of permissions that will be restricted for the script code.
        /// </summary>
        /// <remarks>
        /// Since the report script is compiled into .NET assembly, it allows you to do ANYTHING.
        /// For example, you may create a script that will read/write files from/to a disk. This property
        /// is used to restrict such operations.
        /// <example>This example shows how to restrict the file IO operations in a script:
        /// <code>
        /// using System.Security;
        /// using System.Security.Permissions;
        /// ...
        /// PermissionSet ps = new PermissionSet(PermissionState.None);
        /// ps.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
        /// report1.ScriptRestrictions = ps;
        /// report1.Prepare();
        /// </code>
        /// </example>
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PermissionSet ScriptRestrictions
        {
            get { return scriptRestrictions; }
            set { scriptRestrictions = value; }
        }

        /// <summary>
        /// Gets a reference to the graphics cache for this report.
        /// </summary>
        /// <remarks>
        /// This property is used to support the FastReport.Net infrastructure. Do not use it directly.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GraphicCache GraphicCache
        {
            get { return graphicCache; }
        }

        /// <summary>
        /// Gets a pages of the prepared report.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Preview.PreparedPages PreparedPages
        {
            get { return preparedPages; }
        }

        /// <summary>
        /// Gets a reference to the report engine.
        /// </summary>
        /// <remarks>
        /// This property can be used when report is running. In other cases it returns <b>null</b>.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReportEngine Engine
        {
            get { return engine; }
        }

        /// <summary>
        /// Gets or sets the initial page number for PageN/PageNofM system variables.
        /// </summary>
        [DefaultValue(1)]
        [SRCategory("Engine")]
        public int InitialPageNumber
        {
            get { return initialPageNumber; }
            set { initialPageNumber = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Name
        {
            get { return base.Name; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Restrictions Restrictions
        {
            get { return base.Restrictions; }
            set { base.Restrictions = value; }
        }

        /// <summary>
        /// Gets the report operation that is currently performed.
        /// </summary>
        [Browsable(false)]
        public ReportOperation Operation
        {
            get { return operation; }
        }

        /// <summary>
        /// Gets or sets the Tag object of the report.
        /// </summary>
        [Browsable(false)]
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private string[] DefaultAssemblies
        {
            get
            {
                return new string[] {
                    "System.dll",

                    "System.Drawing.dll",

                    "System.Data.dll",

                    "System.Xml.dll",

                    "FastReport.Compat.dll",
#if !NETSTANDARD
                    "System.Windows.Forms.dll",
#endif

#if NETSTANDARD || NETCOREAPP
                    "System.Drawing.Primitives",
#endif

#if MSCHART
                    "FastReport.DataVisualization.dll"
#endif
                };
            }
        }

        internal CodeHelperBase CodeHelper
        {
            get { return codeHelper; }
        }

        internal Graphics MeasureGraphics
        {
            get
            {
                if (measureGraphics == null)
                {
#if NETSTANDARD2_0 || NETSTANDARD2_1 || MONO
                    measureBitmap = new Bitmap(1, 1);
                    measureGraphics = Graphics.FromImage(measureBitmap);
#else
                    measureGraphics = Graphics.FromHwnd(IntPtr.Zero);
#endif
                }
                return measureGraphics;
            }
        }

        internal string GetReportName
        {
            get
            {
                string result = ReportInfo.Name;
                if (String.IsNullOrEmpty(result))
                    result = Path.GetFileNameWithoutExtension(FileName);
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the flag for refresh.
        /// </summary>
        public bool NeedRefresh
        {
            get { return needRefresh; }
            set { needRefresh = value; }
        }

        internal ObjectCollection AllNamedObjects
        {
            get
            {
                ObjectCollection allObjects = AllObjects;
                // data objects are not included into AllObjects list. Include named items separately.
                foreach (Base c in Dictionary.AllObjects)
                {
                    if (c is DataConnectionBase || c is DataSourceBase || c is Relation || c is CubeSourceBase)
                        allObjects.Add(c);
                }

                return allObjects;
            }
        }

#endregion Properties

#region Private Methods

        private bool ShouldSerializeReferencedAssemblies()
        {
            return Converter.ToString(ReferencedAssemblies) != Converter.ToString(DefaultAssemblies);
        }

        // convert absolute path to the base report to relative path (based on the main report path).
        private string GetRelativePathToBaseReport()
        {
            string path = "";
            if (!String.IsNullOrEmpty(FileName))
            {
                try
                {
                    path = Path.GetDirectoryName(FileName);
                }
                catch
                {
                }
            }

            if (!String.IsNullOrEmpty(path))
            {
                try
                {
                    return FileUtils.GetRelativePath(BaseReport, path);
                }
                catch
                {
                }
            }
            return BaseReport;
        }

        private void SetBaseReport(string value)
        {
            baseReport = value;
            if (baseReportObject != null)
            {
                baseReportObject.Dispose();
                baseReportObject = null;
            }

            // detach the base report
            if (String.IsNullOrEmpty(value))
            {
                foreach (Base c in AllObjects)
                {
                    c.SetAncestor(false);
                }
                SetAncestor(false);
                return;
            }

            string saveFileName = fileName;
            if (LoadBaseReport != null)
            {
                LoadBaseReport(this, new CustomLoadEventArgs(value, this));
            }
            else
            {
                // convert the relative path to absolute path (based on the main report path).
                if (!Path.IsPathRooted(value))
                {
                    value = Path.GetFullPath(Path.GetDirectoryName(FileName) + Path.DirectorySeparatorChar + value);
                }
                if (!File.Exists(value) && File.Exists(BaseReportAbsolutePath))
                {
                    value = BaseReportAbsolutePath;
                }
                Load(value);
            }

            fileName = saveFileName;
            baseReport = "";
            Password = "";
            baseReportObject = Activator.CreateInstance(GetType()) as Report;
            baseReportObject.AssignAll(this, true);

            // set Ancestor & CanChangeParent flags
            foreach (Base c in AllObjects)
            {
                c.SetAncestor(true);
            }
            SetAncestor(true);
            baseReport = value;
        }

        private void GetDiff(object sender, DiffEventArgs e)
        {
            if (baseReportObject != null)
            {
                if (e.Object is Report)
                    e.DiffObject = baseReportObject;
                else if (e.Object is Base)
                    e.DiffObject = baseReportObject.FindObject((e.Object as Base).Name);
            }
        }

        private void StartPerformanceCounter()
        {
            tickCount = Environment.TickCount;
        }

        private void StopPerformanceCounter()
        {
            tickCount = Environment.TickCount - tickCount;
        }

        private void ClearReportProperties()
        {
            ReportInfo.Clear();
            Dictionary.Clear();
            if (IsDesigning)
            {
                ScriptLanguage = Config.ReportSettings.DefaultLanguage;
            }
            else
            {
                ScriptLanguage = Language.CSharp;
            }
            ScriptText = codeHelper.EmptyScript();
            BaseReport = "";
            BaseReportAbsolutePath = "";
            DoublePass = false;
            ConvertNulls = true;
            Compressed = false;
            TextQuality = TextQuality.Default;
            SmoothGraphics = false;
            Password = "";
            InitialPageNumber = 1;
            MaxPages = 0;
            ClearDesign();
            Styles.Clear();
            Styles.Name = "";
            ReferencedAssemblies = DefaultAssemblies;
            StartReportEvent = "";
            FinishReportEvent = "";
            needCompile = true;
        }

#endregion Private Methods

#region Protected Methods

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (graphicCache != null)
                    graphicCache.Dispose();
                graphicCache = null;
                if (measureGraphics != null)
                    measureGraphics.Dispose();
                measureGraphics = null;
                if (measureBitmap != null)
                    measureBitmap.Dispose();
                measureBitmap = null;
                DisposeDesign();
                if (PreparedPages != null)
                    PreparedPages.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override void DeserializeSubItems(FRReader reader)
        {
            if (String.Compare(reader.ItemName, "ScriptText", true) == 0)
                ScriptText = reader.ReadPropertyValue();
            else if (String.Compare(reader.ItemName, "Dictionary", true) == 0)
                reader.Read(Dictionary);
            else if (String.Compare(reader.ItemName, "Styles", true) == 0)
                reader.Read(Styles);
            else
                base.DeserializeSubItems(reader);
        }

#endregion Protected Methods

#region IParent

        /// <inheritdoc/>
        public bool CanContain(Base child)
        {
            return child is PageBase || child is Dictionary;
        }

        /// <inheritdoc/>
        public void GetChildObjects(ObjectCollection list)
        {
            foreach (PageBase page in pages)
            {
                list.Add(page);
            }
        }

        /// <inheritdoc/>
        public void AddChild(Base obj)
        {
            if (obj is PageBase)
                pages.Add(obj as PageBase);
            else if (obj is Dictionary)
                Dictionary = obj as Dictionary;
        }

        /// <inheritdoc/>
        public void RemoveChild(Base obj)
        {
            if (obj is PageBase)
                pages.Remove(obj as PageBase);
            else if (obj is Dictionary && (obj as Dictionary) == dictionary)
                Dictionary = null;
        }

        /// <inheritdoc/>
        public virtual int GetChildOrder(Base child)
        {
            if (child is PageBase)
                return pages.IndexOf(child as PageBase);
            return 0;
        }

        /// <inheritdoc/>
        public virtual void SetChildOrder(Base child, int order)
        {
            if (child is PageBase)
            {
                if (order > pages.Count)
                    order = pages.Count;
                int oldOrder = child.ZOrder;
                if (oldOrder != -1 && order != -1 && oldOrder != order)
                {
                    if (oldOrder <= order)
                        order--;
                    pages.Remove(child as PageBase);
                    pages.Insert(order, child as PageBase);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateLayout(float dx, float dy)
        {
            // do nothing
        }

#endregion IParent

#region ISupportInitialize Members

        /// <inheritdoc/>
        public void BeginInit()
        {
            initializing = true;
        }

        /// <inheritdoc/>
        public void EndInit()
        {
            initializing = false;
            Dictionary.RegisterData(initializeData, initializeDataName, false);
        }

#endregion ISupportInitialize Members

#region Script related

        private void FillDataSourceCache()
        {
            cachedDataItems.Clear();
            ObjectCollection dictionaryObjects = Dictionary.AllObjects;
            foreach (Parameter c in Dictionary.SystemVariables)
            {
                dictionaryObjects.Add(c);
            }
            foreach (Base c in dictionaryObjects)
            {
                if (c is DataSourceBase)
                {
                    DataSourceBase data = c as DataSourceBase;
                    CachedDataItem cachedItem = new CachedDataItem();
                    cachedItem.dataSource = data;
                    cachedDataItems[data.FullName] = cachedItem;

                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        cachedItem = new CachedDataItem();
                        cachedItem.dataSource = data;
                        cachedItem.column = data.Columns[i];
                        cachedDataItems[data.FullName + "." + data.Columns[i].Alias] = cachedItem;
                    }
                }
                else if (c is Parameter)
                {
                    cachedDataItems[(c as Parameter).FullName] = c;
                }
                else if (c is Total)
                {
                    cachedDataItems[(c as Total).Name] = c;
                }
            }
        }

        internal void Compile()
        {
            FillDataSourceCache();

            if (needCompile)
            {
                AssemblyDescriptor descriptor = new AssemblyDescriptor(this, ScriptText);
                assemblies.Clear();
                assemblies.Add(descriptor);
                descriptor.AddObjects();
                descriptor.AddExpressions();
                descriptor.AddFunctions();
                descriptor.Compile();
            }
            else
            {
                InternalInit();
            }
        }

        /// <summary>
        /// Initializes the report's fields.
        /// </summary>
        /// <remarks>
        /// This method is for internal use only.
        /// </remarks>
        protected void InternalInit()
        {
            needCompile = false;

            AssemblyDescriptor descriptor = new AssemblyDescriptor(this, CodeHelper.EmptyScript());
            assemblies.Clear();
            assemblies.Add(descriptor);
            descriptor.InitInstance(this);
        }

        /// <summary>
        /// Generates the file (.cs or .vb) that contains the report source code.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <remarks>
        /// Use this method to generate the report source code. This code can be attached to your project.
        /// In this case, you will need to call the following code to run a report:
        /// <code>
        /// SimpleListReport report = new SimpleListReport();
        /// report.RegisterData(your_dataset);
        /// report.Show();
        /// </code>
        /// </remarks>
        public void GenerateReportAssembly(string fileName)
        {
            // create the class name
            string className = "";
            string punctuation = " ~`!@#$%^&*()-=+[]{},.<>/?;:'\"\\|";
            foreach (char c in Path.GetFileNameWithoutExtension(fileName))
            {
                if (!punctuation.Contains(c.ToString()))
                    className += c;
            }

            AssemblyDescriptor descriptor = new AssemblyDescriptor(this, ScriptText);
            descriptor.AddObjects();
            descriptor.AddExpressions();
            descriptor.AddFunctions();

            string reportClassText = descriptor.GenerateReportClass(className);
            File.WriteAllText(fileName, reportClassText, Encoding.UTF8);
        }

        /// <summary>
        /// Calculates an expression and returns the result.
        /// </summary>
        /// <param name="expression">The expression to calculate.</param>
        /// <returns>If report is running, returns the result of calculation.
        /// Otherwise returns <b>null</b>.</returns>
        /// <remarks>
        /// <para>The expression may be any valid expression such as "1 + 2". The expression
        /// is calculated in the report script's <b>ReportScript</b> class instance context,
        /// so you may refer to any objects available in this context: private fields,
        /// methods, report objects.</para>
        /// </remarks>
        public object Calc(string expression)
        {
            return Calc(expression, 0);
        }

        /// <summary>
        /// Calculates an expression and returns the result.
        /// </summary>
        /// <param name="expression">The expression to calculate.</param>
        /// <param name="value">The value of currently printing object.</param>
        /// <returns>If report is running, returns the result of calculation.
        /// Otherwise returns <b>null</b>.</returns>
        /// <remarks>
        /// Do not call this method directly. Use the <b>Calc(string expression)</b> method instead.
        /// </remarks>
        public object Calc(string expression, Variant value)
        {
            if (!IsRunning)
                return null;
            if (String.IsNullOrEmpty(expression) || String.IsNullOrEmpty(expression.Trim()))
                return null;

            string expr = expression;
            if (expr.StartsWith("[") && expr.EndsWith("]"))
                expr = expression.Substring(1, expression.Length - 2);

            // check cached items first
            object cachedObject = cachedDataItems[expr];

            if (cachedObject is CachedDataItem)
            {
                CachedDataItem cachedItem = cachedDataItems[expr] as CachedDataItem;
                DataSourceBase data = cachedItem.dataSource;
                Column column = cachedItem.column;

                object val = ConvertToColumnDataType(column.Value, column.DataType, false);

                

                if (CustomCalc != null)
                {
                    CustomCalcEventArgs e = new CustomCalcEventArgs(expr, val, this);
                    CustomCalc(this, e);
                    val = e.CalculatedObject;
                }

                return val;
            }
            else if (cachedObject is Parameter)
            {
                return (cachedObject as Parameter).Value;
            }
            else if (cachedObject is Total)
            {
                object val = (cachedObject as Total).Value;
                if (ConvertNulls && (val == null || val is DBNull))
                    val = 0;

                (cachedObject as Total).ExecuteTotal(val);

                return val;
            }

            // calculate the expression
            return CalcExpression(expression, value);
        }

        private object ConvertToColumnDataType( object val, Type dataType, bool convertNulls)
        {
            if (val == null || val is DBNull)
            {
                if (ConvertNulls || convertNulls)
                    val = Converter.ConvertNull(dataType);
            }
            else
            {
                if (val is IConvertible)
                {
                    Type t = Nullable.GetUnderlyingType(dataType);
                    try
                    {
                        val = Convert.ChangeType(val, t != null ? t : dataType);
                    }
                    catch (InvalidCastException)
                    {
                        // do nothing
                    }
                    catch (FormatException)
                    {
                        // do nothing
                    }
                }
            }
            return val;
        }

        /// <summary>
        /// Returns an expression value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value of currently printing object.</param>
        /// <returns>Returns the result of calculation.</returns>
        /// <remarks>
        /// This method is for internal use only, do not call it directly.
        /// </remarks>
        protected virtual object CalcExpression(string expression, Variant value)
        {
            // try to calculate the expression
            foreach (AssemblyDescriptor d in assemblies)
            {
                if (d.ContainsExpression(expression))
                    return d.CalcExpression(expression, value);
            }

            // expression not found. Probably it was added after the start of the report.
            // Compile new assembly containing this expression.
            AssemblyDescriptor descriptor = new AssemblyDescriptor(this, ScriptText);
            assemblies.Add(descriptor);
            descriptor.AddObjects();
            descriptor.AddSingleExpression(expression);
            descriptor.AddFunctions();
            descriptor.Compile();
            return descriptor.CalcExpression(expression, value);
        }

        /// <summary>
        /// Invokes the script event handler with given name.
        /// </summary>
        /// <param name="name">The name of the script method.</param>
        /// <param name="parms">The method parameters.</param>
        public void InvokeEvent(string name, object[] parms)
        {
            if (assemblies.Count > 0)
                assemblies[0].InvokeEvent(name, parms);
        }

        private Column GetColumn(string complexName)
        {
            if (String.IsNullOrEmpty(complexName))
                return null;

            CachedDataItem cachedItem = cachedDataItems[complexName] as CachedDataItem;
            if (cachedItem != null)
                return cachedItem.column;

            string[] names = complexName.Split(new char[] { '.' });
            cachedItem = cachedDataItems[names[0]] as CachedDataItem;
            DataSourceBase data = cachedItem != null ? cachedItem.dataSource : GetDataSource(names[0]);

            return DataHelper.GetColumn(Dictionary, data, names, true);
        }

        private object GetColumnValue(string complexName, bool convertNull)
        {
            Column column = GetColumn(complexName);
            if (column == null)
                return null;

            return ConvertToColumnDataType(column.Value, column.DataType, convertNull);
        }

        private Variant GetTotalValue(string name, bool convertNull)
        {
            object value = Dictionary.Totals.GetValue(name);
            if (convertNull && (value == null || value is DBNull))
                value = 0;

            return new Variant(value);
        }

        /// <summary>
        /// Gets the data column's value. Automatically converts null value to 0, false or ""
        /// depending on the column type.
        /// </summary>
        /// <param name="complexName">The name of the data column including the datasource name.</param>
        /// <returns>If report is running, returns the column value. Otherwise returns <b>null</b>.</returns>
        /// <remarks>
        /// The return value of this method does not depend on the <see cref="ConvertNulls"/> property.
        /// </remarks>
        /// <example>
        /// <code>
        /// string employeeName = (string)report.GetColumnValue("Employees.FirstName");
        /// </code>
        /// </example>
        public object GetColumnValue(string complexName)
        {
            return GetColumnValue(complexName, true);
        }

        /// <summary>
        /// Gets the data column's value. This method does not convert null values.
        /// </summary>
        /// <param name="complexName">The name of the data column including the datasource name.</param>
        /// <returns>If report is running, returns the column value.
        /// Otherwise returns <b>null</b>.</returns>
        public object GetColumnValueNullable(string complexName)
        {
            return GetColumnValue(complexName, false);
        }

        /// <summary>
        /// Gets the report parameter with given name.
        /// </summary>
        /// <param name="complexName">The name of the parameter.</param>
        /// <returns>The <see cref="Parameter"/> object if found, otherwise <b>null</b>.</returns>
        /// <remarks>
        /// To find nested parameter, use the "." separator: "MainParameter.NestedParameter"
        /// </remarks>
        public Parameter GetParameter(string complexName)
        {
            if (IsRunning)
                return cachedDataItems[complexName] as Parameter;
            return DataHelper.GetParameter(Dictionary, complexName);
        }

        /// <summary>
        /// Gets a value of the parameter with given name.
        /// </summary>
        /// <param name="complexName">The name of the parameter.</param>
        /// <returns>The parameter's value if found, otherwise <b>null</b>.</returns>
        /// <remarks>
        /// To find nested parameter, use the "." separator: "MainParameter.NestedParameter"
        /// </remarks>
        public object GetParameterValue(string complexName)
        {
            Parameter par = GetParameter(complexName);
            if (par != null)
            {
                // avoid InvalidCastException when casting object that is int to double
                if (par.DataType.Name == "Double" && par.Value.GetType() == typeof(int))
                {
                    return (double)(int)par.Value;
                }
                return par.Value;
            }
            return null;
        }

        /// <summary>
        /// Sets the parameter's value.
        /// </summary>
        /// <param name="complexName">The name of the parameter.</param>
        /// <param name="value">Value to set.</param>
        /// <remarks>
        /// Use this method to pass a value to the parameter that you've created in the "Data" window.
        /// Such parameter may be used everythere in a report; for example, you can print its value
        /// or use it in expressions.
        /// <para/>You should call this method <b>after</b> the report was loaded and <b>before</b> you run it.
        /// <para/>To access a nested parameter, use the "." separator: "MainParameter.NestedParameter"
        /// <note type="caution">
        /// This method will create the parameter if it does not exist.
        /// </note>
        /// </remarks>
        /// <example>This example shows how to pass a value to the parameter with "MyParam" name:
        /// <code>
        /// // load the report
        /// report1.Load("report.frx");
        /// // setup the parameter
        /// report1.SetParameterValue("MyParam", 10);
        /// // show the report
        /// report1.Show();
        /// </code>
        /// </example>
        public void SetParameterValue(string complexName, object value)
        {
            Parameter par = GetParameter(complexName);
            if (par == null)
                par = DataHelper.CreateParameter(Dictionary, complexName);
            if (par != null)
            {
                par.Value = value;
                par.Expression = "";
            }
        }

        /// <summary>
        /// Gets a value of the system variable with specified name.
        /// </summary>
        /// <param name="complexName">Name of a variable.</param>
        /// <returns>The variable's value if found, otherwise <b>null</b>.</returns>
        public object GetVariableValue(string complexName)
        {
            return GetParameterValue(complexName);
        }

        /// <summary>
        /// Gets a value of the total with specified name.
        /// </summary>
        /// <param name="name">Name of total.</param>
        /// <returns>The total's value if found, otherwise <b>0</b>.</returns>
        /// <remarks>This method converts null values to 0 if the <see cref="ConvertNulls"/> property is set to true.
        /// Use the <see cref="GetTotalValueNullable"/> method if you don't want the null conversion.
        /// </remarks>
        public Variant GetTotalValue(string name)
        {
            return GetTotalValue(name, ConvertNulls);
        }

        /// <summary>
        /// Gets a value of the total with specified name.
        /// </summary>
        /// <param name="name">Name of total.</param>
        /// <returns>The total's value if found, otherwise <b>null</b>.</returns>
        public Variant GetTotalValueNullable(string name)
        {
            return GetTotalValue(name, false);
        }

        /// <summary>
        /// Gets the datasource with specified name.
        /// </summary>
        /// <param name="alias">Alias name of a datasource.</param>
        /// <returns>The datasource object if found, otherwise <b>null</b>.</returns>
        public DataSourceBase GetDataSource(string alias)
        {
            return Dictionary.FindByAlias(alias) as DataSourceBase;
        }

#endregion Script related

#region Public Methods

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            BaseAssign(source);
        }

        /// <summary>
        /// Aborts the report execution.
        /// </summary>
        public void Abort()
        {
            SetAborted(true);
        }

        /// <inheritdoc/>
        public override Base FindObject(string name)
        {
            foreach (Base c in AllNamedObjects)
            {
                if (String.Compare(name, c.Name, true) == 0)
                    return c;
            }
            return null;
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            base.Clear();
            ClearReportProperties();
        }

        /// <summary>
        /// Updates the report component's styles.
        /// </summary>
        /// <remarks>
        /// Call this method if you change the <see cref="Styles"/> collection.
        /// </remarks>
        public void ApplyStyles()
        {
            foreach (Base c in AllObjects)
            {
                if (c is ReportComponentBase)
                    (c as ReportComponentBase).Style = (c as ReportComponentBase).Style;
            }
        }

        /// <summary>
        /// Sets prepared pages.
        /// </summary>
        /// <param name="pages"></param>
        public void SetPreparedPages(Preview.PreparedPages pages)
        {
            preparedPages = pages;
            if (pages != null)
                pages.SetReport(this);
        }

        internal void SetAborted(bool value)
        {
            aborted = value;
        }

        internal void SetOperation(ReportOperation operation)
        {
            this.operation = operation;
        }

        /// <summary>
        /// This method fires the <b>StartReport</b> event and the script code connected
        /// to the <b>StartReportEvent</b>.
        /// </summary>
        public void OnStartReport(EventArgs e)
        {
            SetRunning(true);
            if (StartReport != null)
                StartReport(this, e);
            InvokeEvent(StartReportEvent, new object[] { this, e });
        }

        /// <summary>
        /// This method fires the <b>FinishReport</b> event and the script code connected
        /// to the <b>FinishReportEvent</b>.
        /// </summary>
        public void OnFinishReport(EventArgs e)
        {
            SetRunning(false);
            if (FinishReport != null)
                FinishReport(this, e);
            InvokeEvent(FinishReportEvent, new object[] { this, e });
        }

        /// <summary>
        /// Runs the Export event.
        /// </summary>
        /// <param name="e">ExportReportEventArgs object.</param>
        public void OnExportParameters(ExportParametersEventArgs e)
        {
            if (ExportParameters != null)
            {
                ExportParameters(this, e);
            }
        }

        /// <summary>
        /// Add the name of the assembly (in addition to the default) that will be used to compile the report script
        /// </summary>
        /// <param name="assembly_name">Assembly name</param>
        /// <remarks>
        /// For example: <code>report.AddReferencedAssembly("Newtonsoft.Json.dll")</code>
        /// </remarks>
        public void AddReferencedAssembly(string assembly_name)
        {
            string[] assemblies = ReferencedAssemblies;
            Array.Resize(ref assemblies, assemblies.Length + 1);
            assemblies[assemblies.Length - 1] = assembly_name;
        }

        /// <summary>
        /// Add the names of the assembly (in addition to the default) that will be used to compile the report script
        /// </summary>
        /// <param name="assembly_names">Assembly's names</param>
        public void AddReferencedAssembly(IList<string> assembly_names)
        {
            string[] assemblies = ReferencedAssemblies;
            int oldLength = assemblies.Length;
            Array.Resize(ref assemblies, oldLength + assembly_names.Count);
            for (int i = 0; i < assembly_names.Count; i++)
            {
                assemblies[oldLength + i] = assembly_names[i];
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            Report c = writer.DiffObject as Report;
            writer.ItemName = IsAncestor ? "inherited" : ClassName;
            if (BaseReport != c.BaseReport)
            {
                // when save to the report file, convert absolute path to the base report to relative path
                // (based on the main report path). Do not convert when saving to the clipboard.
                string value = writer.SerializeTo != SerializeTo.Undo ? GetRelativePathToBaseReport() : BaseReport;
                writer.WriteStr("BaseReport", value);
                // Fix bug with moving child report to another folder without parent report.
                if (writer.SerializeTo == SerializeTo.Report)
                    writer.WriteStr("BaseReportAbsolutePath", BaseReport);
            }
            // always serialize ScriptLanguage because its default value depends on Config.ReportSettings.DefaultLanguage
            writer.WriteValue("ScriptLanguage", ScriptLanguage);
            if (ScriptText != c.ScriptText)
                writer.WritePropertyValue("ScriptText", ScriptText);
            if (!writer.AreEqual(ReferencedAssemblies, c.ReferencedAssemblies))
                writer.WriteValue("ReferencedAssemblies", ReferencedAssemblies);
            if (ConvertNulls != c.ConvertNulls)
                writer.WriteBool("ConvertNulls", ConvertNulls);
            if (DoublePass != c.DoublePass)
                writer.WriteBool("DoublePass", DoublePass);
            if (Compressed != c.Compressed)
                writer.WriteBool("Compressed", Compressed);
            if (UseFileCache != c.UseFileCache)
                writer.WriteBool("UseFileCache", UseFileCache);
            if (TextQuality != c.TextQuality)
                writer.WriteValue("TextQuality", TextQuality);
            if (SmoothGraphics != c.SmoothGraphics)
                writer.WriteBool("SmoothGraphics", SmoothGraphics);
            if (Password != c.Password)
                writer.WriteStr("Password", Password);
            if (InitialPageNumber != c.InitialPageNumber)
                writer.WriteInt("InitialPageNumber", InitialPageNumber);
            if (MaxPages != c.MaxPages)
                writer.WriteInt("MaxPages", MaxPages);
            if (StartReportEvent != c.StartReportEvent)
                writer.WriteStr("StartReportEvent", StartReportEvent);
            if (FinishReportEvent != c.FinishReportEvent)
                writer.WriteStr("FinishReportEvent", FinishReportEvent);
            ReportInfo.Serialize(writer, c.ReportInfo);
            SerializeDesign(writer, c);
            if (Styles.Count > 0)
                writer.Write(Styles);
            writer.Write(Dictionary);
            if (writer.SaveChildren)
            {
                foreach (Base child in ChildObjects)
                {
                    writer.Write(child);
                }
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
            if (reader.HasProperty("BaseReportAbsolutePath"))
            {
                BaseReportAbsolutePath = reader.ReadStr("BaseReportAbsolutePath");
            }

            base.Deserialize(reader);

            // call OnAfterLoad method of each report object
            foreach (Base c in AllObjects)
            {
                c.OnAfterLoad();
            }
        }

        /// <summary>
        /// Saves the report to a stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public void Save(Stream stream)
        {
            using (FRWriter writer = new FRWriter())
            {
                if (IsAncestor)
                    writer.GetDiff += new DiffEventHandler(GetDiff);
                writer.Write(this);

                List<Stream> disposeList = new List<Stream>();

                if (Compressed)
                {
                    stream = Compressor.Compress(stream);
                    disposeList.Add(stream);
                }
                if (!String.IsNullOrEmpty(Password))
                {
                    stream = Crypter.Encrypt(stream, Password);
                    disposeList.Insert(0, stream);
                }
                writer.Save(stream);

                foreach (Stream s in disposeList)
                {
                    s.Dispose();
                }
            }
        }

        /// <summary>
        /// Saves the report to a file.
        /// </summary>
        /// <param name="fileName">The name of the file to save to.</param>
        public void Save(string fileName)
        {
            FileName = fileName;
            using (FileStream f = new FileStream(fileName, FileMode.Create))
            {
                Save(f);
            }
        }

        /// <summary>
        /// Loads report from a stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        /// <remarks>
        /// When you try to load the password-protected report, you will be asked
        /// for a password. You also may specify the password in the <see cref="Password"/>
        /// property before loading the report. In this case the report will load silently.
        /// </remarks>
        public void Load(Stream stream)
        {
            string password = Password;
            Clear();

            using (FRReader reader = new FRReader(this))
            {
                List<Stream> disposeList = new List<Stream>();
                if (Compressor.IsStreamCompressed(stream))
                {
                    stream = Compressor.Decompress(stream, true);
                    disposeList.Add(stream);
                }
                bool crypted = Crypter.IsStreamEncrypted(stream);
                if (crypted)
                {
                    if (String.IsNullOrEmpty(password))
                    {
                        password = ShowPaswordForm(password);
                    }
                    stream = Crypter.Decrypt(stream, password);
                    disposeList.Add(stream);
                }

                try
                {
                    reader.Load(stream);
                }
                catch (Exception e)
                {
                    if (crypted)
                        throw new DecryptException();
                    throw e;
                }
                finally
                {
                    foreach (Stream s in disposeList)
                    {
                        try
                        {
                            s.Dispose();
                        }
                        catch
                        {
                        }
                    }
                }

                reader.Read(this);
            }
        }

        /// <summary>
        /// Loads the report from a file.
        /// </summary>
        /// <param name="fileName">The name of the file to load from.</param>
        /// <remarks>
        /// When you try to load the password-protected report, you will be asked
        /// for a password. You also may specify the password in the <see cref="Password"/>
        /// property before loading the report. In this case the report will load silently.
        /// </remarks>
        public void Load(string fileName)
        {
            this.fileName = "";
            using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this.fileName = fileName;
                Load(f);
            }
        }

        /// <summary>
        /// Loads the report from a string.
        /// </summary>
        /// <param name="s">The string that contains a stream in UTF8 or Base64 encoding.</param>
        public void LoadFromString(string s)
        {
            if (String.IsNullOrEmpty(s))
                return;

            int startIndex = s.IndexOf("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            if (startIndex != -1)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                using (MemoryStream stream = new MemoryStream(encoding.GetBytes(s.Substring(startIndex))))
                {
                    Load(stream);
                }
            }
            else
            {
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(s)))
                {
                    Load(stream);
                }
            }
        }

        /// <summary>
        /// Saves the report to a string.
        /// </summary>
        /// <returns>The string that contains a stream.</returns>
        public string SaveToString()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Save(stream);

                if (Compressed || !String.IsNullOrEmpty(Password))
                {
                    return Convert.ToBase64String(stream.ToArray());
                }
                else
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    return encoding.GetString(stream.ToArray());
                }
            }
        }

        /// <summary>
        /// Saves the report to a string using the Base64 encoding.
        /// </summary>
        /// <returns>The string that contains a stream.</returns>
        public string SaveToStringBase64()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Save(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        /// <summary>
        /// Creates the report instance and loads the report from a stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        /// <returns>The new report instance.</returns>
        public static Report FromStream(Stream stream)
        {
            Report result = new Report();
            result.Load(stream);
            return result;
        }

        /// <summary>
        /// Creates the report instance and loads the report from a file.
        /// </summary>
        /// <param name="fileName">The name of the file to load from.</param>
        /// <returns>The new report instance.</returns>
        public static Report FromFile(string fileName)
        {
            Report result = new Report();
            result.Load(fileName);
            return result;
        }

        /// <summary>
        /// Creates the report instance and loads the report from a string.
        /// </summary>
        /// <param name="utf8String">The string that contains a stream in UTF8 encoding.</param>
        /// <returns>The new report instance.</returns>
        public static Report FromString(string utf8String)
        {
            Report result = new Report();
            result.LoadFromString(utf8String);
            return result;
        }

        /// <summary>
        /// Registers the application dataset with all its tables and relations to use it in the report.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <remarks>
        /// If you register more than one dataset, use the <see cref="RegisterData(DataSet, string)"/> method.
        /// </remarks>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(dataSet1);
        /// </code>
        /// </example>
        public void RegisterData(DataSet data)
        {
            Dictionary.RegisterDataSet(data, "Data", false);
        }

        /// <summary>
        /// Registers the application dataset with all its tables and relations to use it in the report and enables all its tables.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <param name="enableAllTables">The boolean value indicating whether all tables should be enabled.</param>
        /// <remarks>
        /// If you register more than one dataset, use the <see cref="RegisterData(DataSet, string)"/> method.
        /// </remarks>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(dataSet1, true);
        /// </code>
        /// </example>
        public void RegisterData(DataSet data, bool enableAllTables)
        {
            Dictionary.RegisterDataSet(data, "Data", false);
            foreach (DataTable table in data.Tables)
            {
                DataSourceBase ds = Report.GetDataSource(table.TableName);
                if (ds != null)
                    ds.Enabled = true;
            }
        }

        /// <summary>
        /// Registers the application dataset with specified name.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        /// <remarks>
        /// Use this method if you register more than one dataset. You may specify any value
        /// for the <b>name</b> parameter: it is not displayed anywhere in the designer and used only
        /// to load/save a report. The name must be persistent and unique for each registered dataset.
        /// </remarks>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(dataSet1, "NorthWind");
        /// </code>
        /// </example>
        public void RegisterData(DataSet data, string name)
        {
            if (initializing)
            {
                initializeData = data;
                initializeDataName = name;
            }
            else
                Dictionary.RegisterDataSet(data, name, false);
        }

        /// <summary>
        /// Registers the application dataset with specified name and enables all its tables.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        /// <param name="enableAllTables">The boolean value indicating whether all tables should be enabled.</param>
        /// <remarks>
        /// Use this method if you register more than one dataset. You may specify any value
        /// for the <b>name</b> parameter: it is not displayed anywhere in the designer and used only
        /// to load/save a report. The name must be persistent and unique for each registered dataset.
        /// </remarks>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(dataSet1, "NorthWind", true);
        /// </code>
        /// </example>
        public void RegisterData(DataSet data, string name, bool enableAllTables)
        {
            if (initializing)
            {
                initializeData = data;
                initializeDataName = name;
            }
            else
            {
                Dictionary.RegisterDataSet(data, name, false);
                foreach (DataTable table in data.Tables)
                {
                    DataSourceBase ds = Report.GetDataSource(table.TableName);
                    if (ds != null)
                        ds.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Registers the application data table to use it in the report.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(dataSet1.Tables["Orders"], "Orders");
        /// </code>
        /// </example>
        public void RegisterData(DataTable data, string name)
        {
            Dictionary.RegisterDataTable(data, name, false);
        }

        /// <summary>
        /// Registers the application data view to use it in the report.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(myDataView, "OrdersView");
        /// </code>
        /// </example>
        public void RegisterData(DataView data, string name)
        {
            Dictionary.RegisterDataView(data, name, false);
        }

        /// <summary>
        /// Registers the application data relation to use it in the report.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        /// <remarks>
        /// You may specify any value for the <b>name</b> parameter: it is not displayed anywhere
        /// in the designer and used only to load/save a report. The name must be persistent
        /// and unique for each registered relation.
        /// </remarks>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(myDataRelation, "myRelation");
        /// </code>
        /// </example>
        public void RegisterData(DataRelation data, string name)
        {
            Dictionary.RegisterDataRelation(data, name, false);
        }

        /// <summary>
        /// <b>Obsolete</b>. Registers the application business object to use it in the report.
        /// </summary>
        /// <param name="data">Application data.</param>
        /// <param name="name">Name of the data.</param>
        /// <param name="flags">Not used.</param>
        /// <param name="maxNestingLevel">Maximum nesting level of business objects.</param>
        /// <remarks>
        /// This method is obsolete. Use the <see cref="RegisterData(IEnumerable, string)"/> method instead.
        /// </remarks>
        public void RegisterData(IEnumerable data, string name, BOConverterFlags flags, int maxNestingLevel)
        {
            RegisterData(data, name, maxNestingLevel);
        }

        /// <summary>
        /// Registers the application business object to use it in the report.
        /// </summary>
        /// <param name="data">Application data.</param>
        /// <param name="name">Name of the data.</param>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(myBusinessObject, "Customers");
        /// </code>
        /// </example>
        public void RegisterData(IEnumerable data, string name)
        {
            if (initializing)
            {
                initializeData = data;
                initializeDataName = name;
            }
            else
                Dictionary.RegisterBusinessObject(data, name, 1, false);
        }

        /// <summary>
        /// Registers the application business object to use it in the report.
        /// </summary>
        /// <param name="data">Application data.</param>
        /// <param name="name">Name of the data.</param>
        /// <param name="maxNestingLevel">Maximum nesting level of business objects.</param>
        /// <remarks>
        /// This method creates initial datasource with specified nesting level. It is useful if
        /// you create a report in code. In most cases, you don't need to specify the nesting level
        /// because it may be selected in the designer's "Choose Report Data" dialog.
        /// </remarks>
        public void RegisterData(IEnumerable data, string name, int maxNestingLevel)
        {
            Dictionary.RegisterBusinessObject(data, name, maxNestingLevel, false);
        }

        /// <summary>
        /// Registers the application cube link to use it in the report.
        /// </summary>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        /// <example>
        /// <code>
        /// report1.Load("report.frx");
        /// report1.RegisterData(myCubeLink, "Orders");
        /// </code>
        /// </example>
        public void RegisterData(IBaseCubeLink data, string name)
        {
            Dictionary.RegisterCubeLink(data, name, false);
        }

        /// <summary>
        /// Prepares the report.
        /// </summary>
        /// <returns><b>true</b> if report was prepared succesfully.</returns>
        public bool Prepare()
        {
            return Prepare(false);
        }

        /// <summary>
        /// Prepares the report.
        /// </summary>
        /// <param name="append">Specifies whether the new report should be added to a
        /// report that was prepared before.</param>
        /// <returns><b>true</b> if report was prepared succesfully.</returns>
        /// <remarks>
        /// Use this method to merge prepared reports.
        /// </remarks>
        /// <example>This example shows how to merge two reports and preview the result:
        /// <code>
        /// Report report = new Report();
        /// report.Load("report1.frx");
        /// report.Prepare();
        /// report.Load("report2.frx");
        /// report.Prepare(true);
        /// report.ShowPrepared();
        /// </code>
        /// </example>
        public bool Prepare(bool append)
        {
            SetRunning(true);
            try
            {
                if (PreparedPages == null || !append)
                {
                    ClearPreparedPages();

                    SetPreparedPages(new Preview.PreparedPages(this));
                }
                engine = new ReportEngine(this);

                if (!Config.WebMode)
                    StartPerformanceCounter();

                try
                {
                    Compile();
                    return Engine.Run(true, append, true);
                }
                finally
                {
                    if (!Config.WebMode)
                        StopPerformanceCounter();
                }
            }
            finally
            {
                SetRunning(false);
            }
        }

        /// <summary>
        /// Prepares the report with pages limit.
        /// </summary>
        /// <param name="pagesLimit">Pages limit. The number of pages equal or less will be prepared.</param>
        /// <returns><b>true</b> if report was prepared succesfully.</returns>
        public bool Prepare(int pagesLimit)
        {
            SetRunning(true);
            try
            {
                ClearPreparedPages();
                SetPreparedPages(new Preview.PreparedPages(this));
                engine = new ReportEngine(this);

                if (!Config.WebMode)
                    StartPerformanceCounter();

                try
                {
                    Compile();
                    return Engine.Run(true, false, true, pagesLimit);
                }
                finally
                {
                    if (!Config.WebMode)
                        StopPerformanceCounter();
                }
            }
            finally
            {
                SetRunning(false);
            }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreparePhase1()
        {
            SetRunning(true);
            if (preparedPages != null)
                preparedPages.Clear();
            SetPreparedPages(new Preview.PreparedPages(this));
            engine = new ReportEngine(this);
            Compile();
            Engine.RunPhase1();
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreparePhase2()
        {
            Engine.RunPhase2();
            SetRunning(false);
        }

        /// <summary>
        /// Refresh the current report.
        /// </summary>
        /// <remarks>
        /// Call this method in the Click or MouseUp event handler of a report object to refresh
        /// the currently previewed report. Report will be generated again, but without dialog forms.
        /// </remarks>
        public void Refresh()
        {
            needRefresh = true;
        }

        /// <summary>
        ///  Refresh prepared report after intercative actions.
        /// </summary>
        public void InteractiveRefresh()
        {
            PreparedPages.ClearPageCache();
            InternalRefresh();
        }

        internal void InternalRefresh()
        {
            SetRunning(true);
            try
            {
                Engine.Run(false, false, false);
            }
            finally
            {
                SetRunning(false);
            }
        }


        internal TextRenderingHint GetTextQuality()
        {
            switch (this.TextQuality)
            {
                case TextQuality.Regular:
                    return TextRenderingHint.AntiAliasGridFit;

                case TextQuality.ClearType:
                    return TextRenderingHint.ClearTypeGridFit;

                case TextQuality.AntiAlias:
                    return TextRenderingHint.AntiAlias;

                case TextQuality.SingleBPP:
                    return TextRenderingHint.SingleBitPerPixel;

                case TextQuality.SingleBPPGridFit:
                    return TextRenderingHint.SingleBitPerPixelGridFit;
            }

            return TextRenderingHint.SystemDefault;
        }


        /// <summary>
        /// Prepare page
        /// </summary>
        /// <param name="page"></param>
        public void PreparePage(ReportPage page)
        {
            SetRunning(true);
            try
            {
                Engine.Run(false, false, false, page);
            }
            finally
            {
                SetRunning(false);
            }
        }

        /// <summary>
        /// Exports a report. Report should be prepared using the <see cref="Prepare()"/> method.
        /// </summary>
        /// <param name="export">The export filter.</param>
        /// <param name="stream">Stream to save export result to.</param>
        public void Export(ExportBase export, Stream stream)
        {
            export.Export(this, stream);
        }

        /// <summary>
        /// Exports a report. Report should be prepared using the <see cref="Prepare()"/> method.
        /// </summary>
        /// <param name="export">The export filter.</param>
        /// <param name="fileName">File name to save export result to.</param>
        public void Export(ExportBase export, string fileName)
        {
            export.Export(this, fileName);
        }

        /// <summary>
        /// Saves the prepared report. Report should be prepared using the <see cref="Prepare()"/> method.
        /// </summary>
        /// <param name="fileName">File name to save to.</param>
        public void SavePrepared(string fileName)
        {
            if (PreparedPages != null)
                PreparedPages.Save(fileName);
        }

        /// <summary>
        /// Saves the prepared report. Report should be prepared using the <see cref="Prepare()"/> method.
        /// </summary>
        /// <param name="stream">Stream to save to.</param>
        public void SavePrepared(Stream stream)
        {
            if (PreparedPages != null)
                PreparedPages.Save(stream);
        }

        /// <summary>
        /// Loads the prepared report from a .fpx file.
        /// </summary>
        /// <param name="fileName">File name to load form.</param>
        public void LoadPrepared(string fileName)
        {
            if (PreparedPages == null)
                SetPreparedPages(new FastReport.Preview.PreparedPages(this));
            PreparedPages.Load(fileName);
        }

        /// <summary>
        /// Loads the prepared report from a .fpx file.
        /// </summary>
        /// <param name="stream">Stream to load from.</param>
        public void LoadPrepared(Stream stream)
        {
            if (PreparedPages == null)
                SetPreparedPages(new FastReport.Preview.PreparedPages(this));
            PreparedPages.Load(stream);
        }

#endregion Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Report"/> class with default settings.
        /// </summary>
        public Report()
        {
            pages = new PageCollection(this);
            reportInfo = new ReportInfo();
            InitDesign();
            styles = new StyleCollection();
            Dictionary = new Dictionary();
            graphicCache = new GraphicCache();
            assemblies = new AssemblyCollection();
            cachedDataItems = new Hashtable(StringComparer.InvariantCultureIgnoreCase); // needed for case insensitivity
            storeInResources = true;
            fileName = "";
            autoFillDataSet = true;
            tag = null;
            ClearReportProperties();
            SetFlags(Flags.CanMove | Flags.CanResize | Flags.CanDelete | Flags.CanEdit | Flags.CanChangeOrder |
              Flags.CanChangeParent | Flags.CanCopy, false);
            //FInlineImageCache = new InlineImageCache();
        }

        static Report()
        {
            Config.Init();
        }

        /// <summary>
        /// Ensure that static constructor is called.
        /// </summary>
        public static void EnsureInit()
        {
            // do nothing, just ensure that static constructor is called.
        }

        /// <summary>
        /// Create name for all unnamed elements with prefix and start with number
        /// </summary>
        /// <param name="prefix">Prefix for name</param>
        /// <param name="number">Number from which to start</param>
        public void PostNameProcess(string prefix, int number)
        {
            int i = number;

            foreach (Base obj in AllObjects)
            {
                if (String.IsNullOrEmpty(obj.Name))
                {
                    obj.SetName(prefix + i.ToString());
                    i++;
                }
            }
        }

        private class CachedDataItem
        {
            public DataSourceBase dataSource;
            public Column column;
        }
    }
}
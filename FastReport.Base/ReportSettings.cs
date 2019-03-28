using System.ComponentModel;
using FastReport.Data;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Specifies the default paper size used when creating a new report.
  /// </summary>
  public enum DefaultPaperSize
  {
    /// <summary>
    /// A4 paper (210 x 297 mm).
    /// </summary>
    A4,
    
    /// <summary>
    /// Letter paper (8.5 x 11 inches, 216 x 279 mm).
    /// </summary>
    Letter
  }
  
  /// <summary>
  /// This class contains settings that will be applied to the Report component.
  /// </summary>
  [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
  public partial class ReportSettings
  {
    private Language defaultLanguage;
    private DefaultPaperSize defaultPaperSize;
    private bool usePropValuesToDiscoverBO;
    private string imageLocationRoot;

    /// <include file='Resources/doc.xml' path='//CodeDoc/Topics/EnvironmentSettings/DatabaseLogin/*'/>
    public event DatabaseLoginEventHandler DatabaseLogin;

    /// <summary>
    /// Occurs after the database connection is established.
    /// </summary>
    public event AfterDatabaseLoginEventHandler AfterDatabaseLogin;

    /// <summary>
    /// Occurs when discovering the business object's structure.
    /// </summary>
    public event FilterPropertiesEventHandler FilterBusinessObjectProperties;

    /// <summary>
    /// Occurs when determining the kind of business object's property.
    /// </summary>
    public event GetPropertyKindEventHandler GetBusinessObjectPropertyKind;

    /// <summary>
    /// Occurs when discovering the structure of business object of ICustomTypeDescriptor type 
    /// with no instance specified.
    /// </summary>
    /// <remarks>
    /// The event handler must return an instance of that type.
    /// </remarks>
    public event GetTypeInstanceEventHandler GetBusinessObjectTypeInstance;

    /// <summary>
    /// Gets or sets the default script language.
    /// </summary>
    [DefaultValue(Language.CSharp)]
    public Language DefaultLanguage
    {
      get { return defaultLanguage; }
      set { defaultLanguage = value; }
    }

    /// <summary>
    /// Gets or sets the default paper size used when creating a new report.
    /// </summary>
    [DefaultValue(DefaultPaperSize.A4)]
    public DefaultPaperSize DefaultPaperSize
    {
      get { return defaultPaperSize; }
      set { defaultPaperSize = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that the business object engine will use property values 
    /// when possible to discover the BO structure.
    /// </summary>
    [DefaultValue(true)]
    public bool UsePropValuesToDiscoverBO
    {
      get { return usePropValuesToDiscoverBO; }
      set { usePropValuesToDiscoverBO = value; }
    }

    /// <summary>
    /// Gets or sets the default path for root of PictureObject.ImageLocation path.
    /// </summary>
    [DefaultValue("")]
    public string ImageLocationRoot
    {
      get { return imageLocationRoot; }
      set { imageLocationRoot = value; }
    }

    internal void OnAfterDatabaseLogin(DataConnectionBase sender, AfterDatabaseLoginEventArgs e)
    {
      if (AfterDatabaseLogin != null)
        AfterDatabaseLogin(sender, e);
    }

    internal void OnFilterBusinessObjectProperties(object sender, FilterPropertiesEventArgs e)
    {
      if (FilterBusinessObjectProperties != null)
        FilterBusinessObjectProperties(sender, e);
    }

    internal void OnGetBusinessObjectPropertyKind(object sender, GetPropertyKindEventArgs e)
    {
      if (GetBusinessObjectPropertyKind != null)
        GetBusinessObjectPropertyKind(sender, e);
    }

    internal void OnGetBusinessObjectTypeInstance(object sender, GetTypeInstanceEventArgs e)
    {
      if (GetBusinessObjectTypeInstance != null)
        GetBusinessObjectTypeInstance(sender, e);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportSettings"/> class.
    /// </summary>
    public ReportSettings()
    {
      usePropValuesToDiscoverBO = true;
    }
  }
}

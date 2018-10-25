using System;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Format;
using System.Windows.Forms;
using System.Drawing.Design;

namespace FastReport
{
  /// <summary>
  /// Specifies how to display the duplicate values.
  /// </summary>
  public enum Duplicates
  {
    /// <summary>
    /// The <b>TextObject</b> can show duplicate values.
    /// </summary>
    Show,

    /// <summary>
    /// The <b>TextObject</b> with duplicate value will be hidden.
    /// </summary>
    Hide,

    /// <summary>
    /// The <b>TextObject</b> with duplicate value will be shown but with no text.
    /// </summary>
    Clear,

    /// <summary>
    /// Several <b>TextObject</b> objects with the same value will be merged into one <b>TextObject</b> object.
    /// </summary>
    Merge
  }

  /// <summary>
  /// Specifies how the report engine processes the text objects.
  /// </summary>
  public enum ProcessAt
  {
    /// <summary>
    /// Specifies the default process mode. The text object is processed just-in-time.
    /// </summary>
    Default,

    /// <summary>
    /// Specifies that the text object must be processed when the entire report is finished. This mode
    /// can be used to print grand total value (which is normally calculated at the end of report) in the
    /// report title band.
    /// </summary>
    ReportFinished,

    /// <summary>
    /// Specifies that the text object must be processed when the entire report page is finished. This mode
    /// can be used if the report template consists of several report pages.
    /// </summary>
    ReportPageFinished,

    /// <summary>
    /// Specifies that the text object must be processed when any report page is finished. This mode
    /// can be used to print the page total (which is normally calculated at the page footer) in the
    /// page header band.
    /// </summary>
    PageFinished,

    /// <summary>
    /// Specifies that the text object must be processed when the column is finished. This mode
    /// can be used to print the column total (which is normally calculated at the column footer) in the
    /// column header band.
    /// </summary>
    ColumnFinished,

    /// <summary>
    /// Specifies that the text object must be processed when the data block is finished. This mode can be
    /// used to print a total value in the data header (which is normally available
    /// in the data footer only).
    /// </summary>
    DataFinished,

    /// <summary>
    /// Specifies that the text object must be processed when the group is finished. This mode can be
    /// used to print a total value in the group header (which is normally available
    /// in the group footer only).
    /// </summary>
    GroupFinished,

    /// <summary>
    /// Specifies that the text object is processed manually when you call the <b>Engine.ProcessObject</b> 
    /// method in the report script.
    /// </summary>
    Custom
  }
  
  /// <summary>
  /// Base class for text objects such as <see cref="TextObject"/> and <see cref="RichObject"/>.
  /// </summary>
  /// <remarks>
  /// This class implements common functionality of the text objects.
  /// </remarks>
  public partial class TextObjectBase : BreakableComponent
  {
    #region Fields
    private string text;
    private object value;
    private bool allowExpressions;
    private string brackets;
    private Padding padding;
    private bool hideZeros;
    private string hideValue;
    private string nullValue;
    private FormatCollection formats;
    private ProcessAt processAt;
    private Duplicates duplicates;
    private bool editable;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating that the object's text may contain expressions.
    /// </summary>
    [DefaultValue(true)]
    [Category("Data")]
    public bool AllowExpressions
    {
      get { return allowExpressions; }
      set { allowExpressions = value; }
    }

    /// <summary>
    /// Gets or sets the symbols that will be used to find expressions in the object's text.
    /// </summary>
    /// <remarks>
    /// The default property value is "[,]". As you can see, the open and close symbols are 
    /// separated by the comma. You may use another symbols, for example: "&lt;,&gt;" or "&lt;%,%&gt;".
    /// You should use different open and close symbols.
    /// </remarks>
    [Category("Data")]
    public string Brackets
    {
      get { return brackets; }
      set { brackets = value; }
    }

    /// <summary>
    /// Gets or sets the object's text.
    /// </summary>
    /// <remarks>
    /// Text may contain expressions and data items, for example: "Today is [Date]". 
    /// When report is running, all expressions are calculated and replaced with actual 
    /// values, so the text would be "Today is 01.01.2008".
    /// </remarks>
    [Category("Data")]
    public virtual string Text
    {
      get { return text; }
      set { text = value; }
    }

    /// <summary>
    /// Gets or sets padding within the text object.
    /// </summary>
    [Category("Layout")]
    public Padding Padding
    {
      get { return padding; }
      set { padding = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that zero values must be hidden.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool HideZeros
    {
      get { return hideZeros; }
      set { hideZeros = value; }
    }

    /// <summary>
    /// Gets or sets a value that will be hidden.
    /// </summary>
    /// <remarks>
    /// Use this property to specify a value that you would like to hide. For example, specify "0"
    /// if you want to hide zero values, or use <see cref="HideZeros"/> property to do this.
    /// <para/>You also may use this property to hide default DateTime values (such as 1/1/1900).
    /// In this case you need to specify a string containing both date and time, for example:
    /// "1/1/1900 0:00:00".
    /// <note type="caution">
    /// FastReport uses the <b>ToString</b> conversion to compare the expression value with this property.
    /// This conversion depends on regional settings selected in the Control Panel, so be aware of this
    /// if you going to distribute your report worldwide.
    /// </note>
    /// </remarks>
    [Category("Behavior")]
    public string HideValue
    {
      get { return hideValue; }
      set { hideValue = value; }
    }

    /// <summary>
    /// Gets or sets a string that will be displayed instead of a null value.
    /// </summary>
    [Category("Behavior")]
    public string NullValue
    {
      get { return nullValue; }
      set { nullValue = value; }
    }

    /// <summary>
    /// Gets or sets the formatter that will be used to format data in the Text object.
    /// </summary>
    /// <remarks>
    /// The default formatter does nothing, i.e. it shows expression values with no formatting.
    /// To set another formatting, create a new formatter and assign it to this property.
    /// <para/>If there are several expressions in the text, use the <see cref="Formats"/> property
    /// to format each expression value.
    /// </remarks>
    /// <example>This example shows how to set currency formatter.
    /// <code>
    /// TextObject text1;
    /// text1.Format = new CurrencyFormat();
    /// </code>
    /// </example>
    [Category("Data")]
    [Editor("FastReport.TypeEditors.FormatEditor, FastReport", typeof(UITypeEditor))]
    public FormatBase Format
    {
      get { return formats.Count == 0 ? new GeneralFormat() : formats[0]; }
      set
      {
        if (value == null)
          value = new GeneralFormat();
        if (formats.Count == 0)
          formats.Add(value);
        else
          formats[0] = value;
      }
    }

    /// <summary>
    /// Gets or sets a value that specifies how the report engine processes this text object.
    /// </summary>
    /// <remarks>
    /// Use this property to perform such task as "print a total value in the group header". Normally,
    /// all total values are calculated in the footers (for example, in a group footer). If you try to print
    /// a total value in the group header, you will get 0. If you set this property to 
    /// <b>ProcessAt.DataFinished</b>, FastReport will do the following:
    /// <list type="bullet">
    ///   <item>
    ///     <description>print the object (with wrong value);</description>
    ///   </item>
    ///   <item>
    ///     <description>print all related data rows;</description>
    ///   </item>
    ///   <item>
    ///     <description>calculate the correct object's value and replace old (wrong) value with the new one.</description>
    ///   </item>
    /// </list>
    /// <note type="caution">
    /// This option will not work if you set the <see cref="Report.UseFileCache"/> to <b>true</b>.
    /// </note>
    /// </remarks>
    [Category("Behavior")]
    [DefaultValue(ProcessAt.Default)]
    public ProcessAt ProcessAt
    {
      get { return processAt; }
      set { processAt = value; }
    }
    
    /// <summary>
    /// Gets the collection of formatters.
    /// </summary>
    /// <remarks>
    /// This property is used to set format for each expression contained in the text.
    /// For example, if the <b>TextObject</b> contains two expressions: 
    /// <para/><i>Today is [Date]; Page [PageN]</i>
    /// <para/>you can use the following code to format these expressions separately:
    /// <code>
    /// text1.Formats.Clear();
    /// text1.Formats.Add(new DateFormat());
    /// text1.Formats.Add(new NumberFormat());
    /// </code>
    /// </remarks>
    [Browsable(false)]
    public FormatCollection Formats
    {
      get { return formats; }
    }

    /// <summary>
    /// Gets or sets a value that determines how to display duplicate values.
    /// </summary>
    [DefaultValue(Duplicates.Show)]
    [Category("Behavior")]
    public Duplicates Duplicates
    {
      get { return duplicates; }
      set { duplicates = value; }
    }

    /// <summary>
    /// Gets a value of expression contained in the object's text.
    /// </summary>
    [Browsable(false)]
    public object Value
    {
      get { return value; }
    }
    
    /// <summary>
    /// Gets or sets editable for pdf export
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool Editable
    {
        get { return editable; }
        set { editable = value; }
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override void DeserializeSubItems(FRReader reader)
    {
      if (String.Compare(reader.ItemName, "Formats", true) == 0)
        reader.Read(Formats);
      else
        base.DeserializeSubItems(reader);
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);

      TextObjectBase src = source as TextObjectBase;
      Text = src.Text;
      Padding = src.Padding;
      AllowExpressions = src.AllowExpressions;
      Brackets = src.Brackets;
      HideZeros = src.HideZeros;
      HideValue = src.HideValue;
      NullValue = src.NullValue;
      ProcessAt = src.ProcessAt;
      Formats.Assign(src.Formats);
      Duplicates = src.Duplicates;
      Editable = src.Editable;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      TextObjectBase c = writer.DiffObject as TextObjectBase;
      base.Serialize(writer);

      if (Text != c.Text)
        writer.WriteStr("Text", Text);
      if (Padding != c.Padding)
        writer.WriteValue("Padding", Padding);
      if (writer.SerializeTo != SerializeTo.Preview)
      {
        if (AllowExpressions != c.AllowExpressions)
          writer.WriteBool("AllowExpressions", AllowExpressions);
        if (Brackets != c.Brackets)
          writer.WriteStr("Brackets", Brackets);
        if (HideZeros != c.HideZeros)
          writer.WriteBool("HideZeros", HideZeros);
        if (HideValue != c.HideValue)
          writer.WriteStr("HideValue", HideValue);
        if (NullValue != c.NullValue)
          writer.WriteStr("NullValue", NullValue);
        if (ProcessAt != c.ProcessAt)
          writer.WriteValue("ProcessAt", ProcessAt);
        if (Duplicates != c.Duplicates)
          writer.WriteValue("Duplicates", Duplicates);
      }
      if (Editable)
        writer.WriteBool("Editable", Editable);
      if (Formats.Count > 1)
        writer.Write(Formats);
      else
        Format.Serialize(writer, "Format.", c.Format);
    }

    /// <inheritdoc/>
    public override void ExtractMacros()
    {
      Text = ExtractDefaultMacros(Text);
    }

    internal void SetValue(object value)
    {
      this.value = value;
    }

    internal string GetTextWithBrackets(string text)
    {
      string[] brackets = Brackets.Split(new char[] { ',' });
      return brackets[0] + text + brackets[1];
    }

    internal string GetTextWithoutBrackets(string text)
    {
      string[] brackets = Brackets.Split(new char[] { ',' });
      if (text.StartsWith(brackets[0]))
        text = text.Remove(0, brackets[0].Length);
      if (text.EndsWith(brackets[1]))
        text = text.Remove(text.Length - brackets[1].Length);
      return text;
    }

    internal string FormatValue(object value)
    {
      return FormatValue(value, 0);
    }

    internal string FormatValue(object value, int formatIndex)
    {
      this.value = value;
      string formattedValue = "";
      if (value == null || value is DBNull)
      {
        formattedValue = NullValue;
      }
      else
      {
        if (formatIndex < Formats.Count)
          formattedValue = Formats[formatIndex].FormatValue(value);
        else
          formattedValue = Format.FormatValue(value);

        if (!String.IsNullOrEmpty(HideValue))
        {
          if (value.ToString() == HideValue)
            formattedValue = "";
        }
        if (HideZeros)
        {
          Variant v = new Variant(value);
          if ((v.IsNumeric && v == 0) || (v.IsDate && v.ToDateTime() == DateTime.MinValue))
            formattedValue = "";
        }
      }

      return formattedValue;
    }

    internal string CalcAndFormatExpression(string expression, int expressionIndex)
    {
      try
      {
        return FormatValue(Report.Calc(expression), expressionIndex);
      }
      catch (Exception e)
      {
        throw new Exception(Name + ": " + Res.Get("Messages,ErrorInExpression") + ": " + expression, e.InnerException);
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TextObjectBase"/> class with default settings.
    /// </summary>
    public TextObjectBase()
    {
      allowExpressions = true;
      brackets = "[,]";
      padding = new Padding(2, 0, 2, 0);
      hideValue = "";
      nullValue = "";
      text = "";
      formats = new FormatCollection();
      formats.Add(new GeneralFormat());
    }
  }
}

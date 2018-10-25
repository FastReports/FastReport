using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Format
{
  /// <summary>
  /// Defines how boolean values are formatted and displayed.
  /// </summary>
  public class BooleanFormat : FormatBase
  {
    #region Fields
    private string falseText;
    private string trueText;
    #endregion
    
    #region Properties
    /// <summary>
    /// Gets or sets a string that will be displayed if value is <b>false</b>.
    /// </summary>
    public string FalseText
    {
      get { return falseText; }
      set { falseText = value; }
    }

    /// <summary>
    /// Gets or sets a string that will be displayed if value is <b>true</b>.
    /// </summary>
    public string TrueText
    {
      get { return trueText; }
      set { trueText = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override FormatBase Clone()
    {
      BooleanFormat result = new BooleanFormat();
      result.FalseText = FalseText;
      result.TrueText = TrueText;
      return result;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      BooleanFormat f = obj as BooleanFormat;
      return f != null && FalseText == f.FalseText && TrueText == f.TrueText;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc/>
    public override string FormatValue(object value)
    {
      if (value is Variant)
        value = ((Variant)value).Value;
      if (value is bool && (bool)value == false)
        return FalseText;
      if (value is bool && (bool)value == true)
        return TrueText;
      return value.ToString();  
    }

    internal override string GetSampleValue()
    {
      return FormatValue(false);
    }

    internal override void Serialize(FRWriter writer, string prefix, FormatBase format)
    {
      base.Serialize(writer, prefix, format);
      BooleanFormat c = format as BooleanFormat;
      
      if (c == null || TrueText != c.TrueText)
        writer.WriteStr(prefix + "TrueText", TrueText);
      if (c == null || FalseText != c.FalseText)
        writer.WriteStr(prefix + "FalseText", FalseText);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>BooleanFormat</b> class with default settings. 
    /// </summary>
    public BooleanFormat()
    {
      FalseText = "False";
      TrueText = "True";
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Format
{
  /// <summary>
  /// Represents a format that uses the <b>Format</b> string to display values.
  /// </summary>
  public class CustomFormat : FormatBase
  {
    #region Fields
    private string format;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a format string.
    /// </summary>
    /// <remarks>
    /// Default format is "G". For example, if you want to format a date, use the following
    /// format string: "MM/dd/yyyy". See the <b>System.String.Format</b> method for list 
    /// of possible format strings.
    /// </remarks>
    public string Format
    {
      get { return format; }
      set { format = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override FormatBase Clone()
    {
      CustomFormat result = new CustomFormat();
      result.Format = Format;
      return result;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      CustomFormat f = obj as CustomFormat;
      return f != null && Format == f.Format;
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
      return String.Format("{0:" + Format + "}", value);
    }

    internal override string GetSampleValue()
    {
      return "";
    }

    internal override void Serialize(FRWriter writer, string prefix, FormatBase format)
    {
      base.Serialize(writer, prefix, format);
      CustomFormat c = format as CustomFormat;
      
      if (c == null || Format != c.Format)
        writer.WriteStr(prefix + "Format", Format);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>CustomFormat</b> class with default settings. 
    /// </summary>
    public CustomFormat()
    {
      Format = "G";
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Represents a style.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Style class holds border, fill, text fill and font settings. It can be applied to any report object of
  /// <see cref="ReportComponentBase"/> type.
  /// </para>
  /// <para>
  /// The <b>Report</b> object holds list of styles in its <see cref="Report.Styles"/> property. Each style has
  /// unique name. To apply a style to the report component, set its <see cref="ReportComponentBase.Style"/>
  /// property to the style name.
  /// </para>
  /// </remarks>
  public class Style : StyleBase
  {
    private string name;
    
    /// <summary>
    /// Gets or sets a name of the style.
    /// </summary>
    /// <remarks>
    /// The name must be unique.
    /// </remarks>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      writer.ItemName = "Style";
      writer.WriteStr("Name", Name);
      base.Serialize(writer);
    }

    /// <inheritdoc/>
    public override void Assign(StyleBase source)
    {
      base.Assign(source);
      Name = (source as Style).Name;
    }
    
    /// <summary>
    /// Creates exact copy of this <b>Style</b>.
    /// </summary>
    /// <returns>Copy of this style.</returns>
    public Style Clone()
    {
      Style result = new Style();
      result.Assign(this);
      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Style"/> class with default settings.
    /// </summary>
    public Style()
    {
      Name = "";
      ApplyBorder = true;
      ApplyFill = true;
      ApplyTextFill = true;
      ApplyFont = true;
    }
  }
}

using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using System.Drawing.Design;

namespace FastReport
{
  /// <summary>
  /// Represents a single highlight condition used by the <see cref="TextObject.Highlight"/> property
  /// of the <see cref="TextObject"/>.
  /// </summary>
  public class HighlightCondition : StyleBase
  {
    #region Fields
    private string expression;
    private bool visible;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a highlight expression.
    /// </summary>
    /// <remarks>
    /// This property can contain any valid boolean expression. If value of this expression is <b>true</b>,
    /// the fill and font settings will be applied to the <b>TextObject</b>.
    /// </remarks>
    [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
    public string Expression
    {
      get { return expression; }
      set { expression = value; }
    }

    /// <summary>
    /// Gets or sets the visibility flag.
    /// </summary>
    /// <remarks>
    /// If this property is set to <b>false</b>, the Text object will be hidden if the
    /// condition is met.
    /// </remarks>
    public bool Visible
    {
      get { return visible; }
      set { visible = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      HighlightCondition c = writer.DiffObject as HighlightCondition;
      writer.ItemName = "Condition";

      if (Expression != c.Expression)
        writer.WriteStr("Expression", Expression);
      if (Visible != c.Visible)
        writer.WriteBool("Visible", Visible);
      
      base.Serialize(writer);
    }

    /// <inheritdoc/>
    public override void Assign(StyleBase source)
    {
      base.Assign(source);
      Expression = (source as HighlightCondition).Expression;
      Visible = (source as HighlightCondition).Visible;
    }

    /// <summary>
    /// Creates exact copy of this condition.
    /// </summary>
    /// <returns>A copy of this condition.</returns>
    public HighlightCondition Clone()
    {
      HighlightCondition result = new HighlightCondition();
      result.Assign(this);
      return result;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      HighlightCondition c = obj as HighlightCondition;
      return c != null && Expression == c.Expression && Border.Equals(c.Border) && Fill.Equals(c.Fill) &&
        TextFill.Equals(c.TextFill) && Font.Equals(c.Font) && Visible == c.Visible &&
        ApplyBorder == c.ApplyBorder && ApplyFill == c.ApplyFill && ApplyTextFill == c.ApplyTextFill && 
        ApplyFont == c.ApplyFont;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightCondition"/> class with default settings.
    /// </summary>
    public HighlightCondition()
    {
      Expression = "";
      TextFill = new SolidFill(Color.Red); 
      Visible = true;
      ApplyBorder = false;
      ApplyFill = false;
      ApplyTextFill = true;
      ApplyFont = false;
    }
  }
}

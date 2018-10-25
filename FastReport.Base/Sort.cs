using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Represents a sort condition used in the <see cref="DataBand.Sort"/>.
  /// </summary>
  public class Sort : IFRSerializable
  {
    private string expression;
    private bool descending;
    
    /// <summary>
    /// Gets or sets an expression used to sort data band rows.
    /// </summary>
    /// <remarks>
    /// This property can contain any valid expression.
    /// </remarks>
    public string Expression
    {
      get { return expression; }
      set { expression = value; }
    }
    
    /// <summary>
    /// Gets or sets a value indicating that sort must be performed in descending order.
    /// </summary>
    public bool Descending
    {
      get { return descending; }
      set { descending = value; }
    }
    
    /// <summary>
    /// Serializes the class.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = "Sort";
      writer.WriteStr("Expression", Expression);
      if (Descending)
        writer.WriteBool("Descending", Descending);
    }

    /// <summary>
    /// Deserializes the class.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void Deserialize(FRReader reader)
    {
      reader.ReadProperties(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sort"/> class with default settings.
    /// </summary>
    public Sort() : this("")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sort"/> class with specified expression.
    /// </summary>
    public Sort(string expression) : this(expression, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sort"/> class with specified expression and sort order.
    /// </summary>
    public Sort(string expression, bool descending)
    {
            this.expression = expression;
            this.descending = descending;
    }
  }
}

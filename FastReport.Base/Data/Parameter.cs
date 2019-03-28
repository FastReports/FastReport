using System;
using System.ComponentModel;
using FastReport.Utils;
using System.Drawing.Design;

namespace FastReport.Data
{
  /// <summary>
  /// Represents a report parameter that is used to pass user data to a report.
  /// </summary>
  /// <remarks>
  /// See <see cref="Report.Parameters"/> for details about using parameters.
  /// </remarks>
  public class Parameter : Base, IParent
  {
    #region Fields
    private Type dataType;
    private object value;
    private string expression;
    private string description;
    private ParameterCollection parameters;
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name of parameter.
    /// </summary>
    [MergableProperty(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Category("Design")]
    [DisplayName("(Name)")]
    public new string Name
    {
        get { return (this as Base).Name; }
        set
        {
            if (value != "" && Report != null)
            {
                foreach (Parameter p in Report.Parameters)
                {
                    if (p != null && p != this && p.Name == value)
                    {
                        return;
                    }
                }
            }

            (this as Base).Name = value;
        }
    }

    /// <summary>
    /// Gets or sets the type of parameter.
    /// </summary>
    [TypeConverter(typeof(FastReport.TypeConverters.DataTypeConverter))]
    [Category("Data")]
    [Editor("FastReport.TypeEditors.DataTypeEditor, FastReport", typeof(UITypeEditor))]
    public Type DataType
    {
      get { return dataType; }
      set { dataType = value; }
    }

    /// <summary>
    /// Gets or sets the value of parameter.
    /// </summary>
    /// <remarks>
    /// You may specify the static value in this property. Note: if the <see cref="Expression"/>
    /// property is not empty, it will be calculated and its value will be returned.
    /// </remarks>
    [Browsable(false)]
    public virtual object Value
    {
      get 
      {
        if (!String.IsNullOrEmpty(Expression) && Report != null && Report.IsRunning)
          value = Report.Calc(Expression);
        return value; 
      }
      set 
      {
                this.value = value;
        if (value != null)
          dataType = value.GetType();
      }
    }

    /// <summary>
    /// Gets or sets value of the parameter as a string.
    /// </summary>
    [Browsable(false)]
    public string AsString
    {
      get 
      {
        object value = Value;
        return value == null ? "" : value.ToString();
      }
      set
      {
        Expression = "";
                this.value = Convert.ChangeType(value, DataType);
      }
    }

    /// <summary>
    /// Gets or sets an expression of the parameter.
    /// </summary>
    /// <remarks>
    /// This expression will be calculated each time you access a parameter's <b>Value</b>.
    /// </remarks>
    [Category("Data")]
    [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
    public string Expression
    {
      get { return expression; }
      set { expression = value; }
    }

    /// <summary>
    /// Gets or sets the description of a parameter.
    /// </summary>
    public string Description
    {
      get { return description; }
      set { description = value; }
    }

    /// <summary>
    /// Gets a collection of nested parameters.
    /// </summary>
    /// <remarks>
    /// Parameters can have child (nested) parameters. To get or set a nested
    /// parameter's value, use the <see cref="Report.GetParameter"/> method.
    /// </remarks>
    [Browsable(false)]
    public ParameterCollection Parameters
    {
      get { return parameters; }
    }
    
    /// <summary>
    /// Gets the full name of the parameter. This is useful to get the nested parameter's full name.
    /// </summary>
    [Browsable(false)]
    public string FullName
    {
      get
      {
        string result = Name;
        Parameter parent = Parent as Parameter;
        while (parent != null)
        {
          result = parent.Name + "." + result;
          parent = parent.Parent as Parameter;
        }
        return result;
      }
    }
    
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new Restrictions Restrictions
    {
      get { return base.Restrictions; }
      set { base.Restrictions = value; }
    }

    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      BaseAssign(source);
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      base.Serialize(writer);
      writer.WriteValue("DataType", DataType);
      if (!String.IsNullOrEmpty(Expression))
        writer.WriteStr("Expression", Expression);
      if (!String.IsNullOrEmpty(Description))
        writer.WriteStr("Description", Description);
    }

    /// <inheritdoc/>
    public override string[] GetExpressions()
    {
      return new string[] { Expression };
    }

    internal Parameter Add(string name)
    {
      if (Parameters.FindByName(name) != null)
        throw new Exception("Parameter " + name + " already exists.");
      Parameter variable = new Parameter(name);
      Parameters.Add(variable);
      return variable;
    }
    #endregion

    #region IParent Members
    /// <inheritdoc/>
    public virtual bool CanContain(Base child)
    {
      return child is Parameter;
    }

    /// <inheritdoc/>
    public void GetChildObjects(ObjectCollection list)
    {
      foreach (Parameter v in Parameters)
      {
        list.Add(v);
      }
    }

    /// <inheritdoc/>
    public void AddChild(Base child)
    {
      Parameters.Add(child as Parameter);
    }

    /// <inheritdoc/>
    public void RemoveChild(Base child)
    {
      Parameters.Remove(child as Parameter);
    }

    /// <inheritdoc/>
    public int GetChildOrder(Base child)
    {
      return Parameters.IndexOf(child as Parameter);
    }

    /// <inheritdoc/>
    public void SetChildOrder(Base child, int order)
    {
      int oldOrder = child.ZOrder;
      if (oldOrder != -1 && order != -1 && oldOrder != order)
      {
        if (order > Parameters.Count)
          order = Parameters.Count;
        if (oldOrder <= order)
          order--;
        Parameters.Remove(child as Parameter);
        Parameters.Insert(order, child as Parameter);
      }
    }

    /// <inheritdoc/>
    public void UpdateLayout(float dx, float dy)
    {
      // do nothing
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class with default settings.
    /// </summary>
    public Parameter() : this("")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class with specified name.
    /// </summary>
    public Parameter(string name)
    {
      Name = name;
      DataType = typeof(string);
      expression = "";
      description = "";
      parameters = new ParameterCollection(this);
      SetFlags(Flags.CanCopy | Flags.CanEdit, false);
    }
  }
}

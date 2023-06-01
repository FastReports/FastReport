using System.Data;

namespace FastReport.Data
{
    /// <summary>
    /// Datasource for stored procedure.
    /// </summary>
    public partial class ProcedureDataSource : TableDataSource
    {
        internal string DisplayNameWithParams
        {
            get
            {
                if (Parameters != null)
                {
                    string paramsStr = "";
                    foreach (CommandParameter parameter in Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Input || parameter.Direction == ParameterDirection.InputOutput)
                            paramsStr += parameter.Name + ", ";
                    }
                    if (paramsStr.EndsWith(", "))
                        paramsStr = paramsStr.Substring(0, paramsStr.Length - 2);
                    if (paramsStr != "")
                        return Name + " (" + paramsStr + ")";
                }
                return Name;
            }
        }

        /// <inheritdoc/>
        public override void InitSchema()
        {
            base.InitSchema();

            if (Parameters != null)
            {
                foreach (CommandParameter parameter in Parameters)
                {
                    if (parameter.Direction == ParameterDirection.Input)
                        continue;

                    var column = Columns.FindByName(parameter.Name);
                    if (column == null)
                    {
                        column = new Column
                        {
                            Name = parameter.Name,
                            DataType = typeof(Variant)
                        };
                        Columns.Add(column);
                    }
                    column.Tag = parameter;
                }
            }
        }

        /// <inheritdoc/>
        protected override object GetValue(Column column)
        {
            if (column.Tag is CommandParameter parameter)
            {
                return parameter.Value;
            }

            return base.GetValue(column);
        }

    }
}
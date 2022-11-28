using System.Data;

namespace FastReport.Data
{
    /// <summary>
    /// Datasource for stored procedure.
    /// </summary>
    public partial class ProcedureDataSource : TableDataSource
    {
        /// <inheritdoc/>
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (value)
                {
                    if (Parameters != null && Report != null)
                    {
                        if (Connection != null)
                            Connection.FillTable(this);
                        foreach (CommandParameter parameter in Parameters)
                        {
                            if (parameter.Direction == ParameterDirection.Input)
                                continue;
                            Report.SetParameterValue(Name + "_" + parameter.Name, parameter.Value);
                            ReportDesignerSetModified();
                        }
                    }
                }
                else
                {
                    if (Parameters != null && Report != null)
                        foreach (CommandParameter parameter in Parameters)
                        {
                            if (parameter.Direction == ParameterDirection.Input)
                                continue;
                            Report.Parameters.Remove(Report.GetParameter(Name + "_" + parameter.Name));
                            ReportDesignerSetModified();
                        }
                }
            }
        }

        /// <inheritdoc/>
        public override string Name
        {
            get => base.Name;
            set
            {
                if (Enabled && Parameters != null && Report != null)
                    foreach (CommandParameter parameter in Parameters)
                    {
                        Parameter param = Report.GetParameter(Name + "_" + parameter.Name);
                        if (param != null)
                            param.Name = value + "_" + parameter.Name;
                    }
                base.Name = value;
            }
        }
    }
}
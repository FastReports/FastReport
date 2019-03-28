using System;
using System.ComponentModel;
using System.IO;
using FastReport.Utils;
using FastReport.Data;
using System.Drawing.Design;

namespace FastReport
{
    /// <summary>
    /// Specifies the hyperlink type.
    /// </summary>
    public enum HyperlinkKind
    {
        /// <summary>
        /// Specifies the hyperlink to external URL such as "http://www.fast-report.com", "mailto:"
        /// or any other system command.
        /// </summary>
        URL,

        /// <summary>
        /// Specifies hyperlink to a given page number.
        /// </summary>
        PageNumber,

        /// <summary>
        /// Specifies hyperlink to a bookmark.
        /// </summary>
        Bookmark,

        /// <summary>
        /// Specifies hyperlink to external report. This report will be run when you follow the hyperlink.
        /// </summary>
        DetailReport,

        /// <summary>
        /// Specifies hyperlink to this report's page. The page will be run when you follow the hyperlink.
        /// </summary>
        DetailPage,

        /// <summary>
        /// Specifies a custom hyperlink. No actions performed when you click it, you should handle it
        /// in the object's Click event handler.
        /// </summary>
        Custom
    }

    /// <summary>
    /// This class contains a hyperlink settings.
    /// </summary>
    [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
    public class Hyperlink
    {
        #region Fields
        private ReportComponentBase parent;
        private HyperlinkKind kind;
        private string expression;
        private string value;
        private string detailReportName;
        private string detailPageName;
        private string reportParameter;
        private string valuesSeparator;
        private string saveValue;
        private bool openLinkInNewTab;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the kind of hyperlink.
        /// </summary>
        /// <remarks>
        /// <para>Use the <b>Kind</b> property to define hyperlink's behavior. 
        /// The hyperlink may be used to navigate to the external url, the page number, 
        /// the bookmark defined by other report object, the external report, the other page of this report, 
        /// and custom hyperlink.</para>
        /// </remarks>
        [DefaultValue(HyperlinkKind.URL)]
        public HyperlinkKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        /// <summary>
        /// Gets or sets the expression which value will be used for navigation.
        /// </summary>
        /// <remarks>
        /// <para>Normally you should set the <b>Expression</b> property to
        /// any valid expression that will be calculated when this object is about to print.
        /// The value of an expression will be used for navigation.</para>
        /// <para>If you want to navigate to some fixed data (URL or page number, for example),
        /// use the <see cref="Value"/> property instead.</para>
        /// </remarks>
        [Editor("FastReport.TypeEditors.HyperlinkExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        /// <summary>
        /// Gets or sets a value that will be used for navigation.
        /// </summary>
        /// <remarks>
        /// Use this property to specify the fixed data (such as URL, page number etc). If you want to
        /// navigate to some dynamically calculated value, use the <see cref="Expression"/> property instead.
        /// </remarks>
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicate should be links open in new tab or not.
        /// </summary>
        /// <remarks>
        /// It works for HTML-export only!
        /// </remarks>
        public bool OpenLinkInNewTab
        {
            get { return openLinkInNewTab; }
            set { openLinkInNewTab = value; }
        }

        /// <summary>
        /// Gets or sets an external report file name.
        /// </summary>
        /// <remarks>
        /// <para>Use this property if <see cref="Kind"/> is set to <b>DetailReport</b>. </para>
        /// <para>When you follow the hyperlink, this report will be loaded and run. 
        /// You also may specify the report's parameter in the <see cref="ReportParameter"/> property.</para>
        /// </remarks>
        [Editor("FastReport.TypeEditors.HyperlinkReportFileEditor, FastReport", typeof(UITypeEditor))]
        public string DetailReportName
        {
            get { return detailReportName; }
            set { detailReportName = value; }
        }

        /// <summary>
        /// Gets or sets the name of this report's page.
        /// </summary>
        /// <remarks>
        /// <para>Use this property if <see cref="Kind"/> is set to <b>DetailPage</b>. </para>
        /// <para>When you follow the hyperlink, the specified page will be executed. It may contain the
        /// detailed report. You also may specify the report's parameter in the 
        /// <see cref="ReportParameter"/> property.</para>
        /// </remarks>
        [Editor("FastReport.TypeEditors.HyperlinkReportPageEditor, FastReport", typeof(UITypeEditor))]
        public string DetailPageName
        {
            get { return detailPageName; }
            set { detailPageName = value; }
        }

        /// <summary>
        /// Gets or sets a parameter's name that will be set to hyperlink's value.
        /// </summary>
        /// <remarks>
        /// Use this property if <see cref="Kind"/> is set to <b>DetailReport</b> or <b>DetailPage</b>.
        /// <para>If you want to pass the hyperlink's value to the report's parameter, specify the
        /// parameter name in this property. This parameter will be set to the hyperlink's value 
        /// before running a report. It may be used to display detailed information about clicked item.</para>
        /// <para>It is also possible to pass multiple values to several parameters. If hyperlink's value
        /// contains separators (the separator string can be set in the <see cref="ValuesSeparator"/>
        /// property), it will be splitted to several values. That values will be passed to nested parameters
        /// of the <b>ReportParameter</b> (you should create nested parameters by youself). For example, you have
        /// the <b>ReportParameter</b> called "SelectedValue" which has two nested parameters: the first one is 
        /// "Employee" and the second is "Category". The hyperlink's value is "Andrew Fuller;Beverages". 
        /// It will be splitted to two values: "Andrew Fuller" and "Beverages". The first nested parameter 
        /// of the <b>ReportParameter</b> that is "Employee" in our case will be set to "Andrew Fuller";
        /// the second nested parameter ("Category") will be set to "Beverages".</para>
        /// <para>Note: when you create a parameter in the detailed report, don't forget to set 
        /// its <b>DataType</b> property. It is used to convert string values to actual data type.
        /// </para>
        /// </remarks>
        [Editor("FastReport.TypeEditors.HyperlinkReportParameterEditor, FastReport", typeof(UITypeEditor))]
        public string ReportParameter
        {
            get { return reportParameter; }
            set { reportParameter = value; }
        }

        /// <summary>
        /// Gets or sets a string that will be used as a separator to pass several values 
        /// to the external report parameters.
        /// </summary>
        public string ValuesSeparator
        {
            get { return valuesSeparator; }
            set { valuesSeparator = value; }
        }

        internal ReportComponentBase Parent
        {
            get { return parent; }
        }

        internal Report Report
        {
            get { return parent.Report; }
        }
        #endregion

        #region Private Methods
        private bool ShouldSerializeValuesSeparator()
        {
            return ValuesSeparator != ";";
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Assigns values from another source.
        /// </summary>
        /// <param name="source">Source to assign from.</param>
        public void Assign(Hyperlink source)
        {
            Kind = source.Kind;
            Expression = source.Expression;
            Value = source.Value;
            DetailReportName = source.DetailReportName;
            ReportParameter = source.ReportParameter;
            DetailPageName = source.DetailPageName;
            OpenLinkInNewTab = source.openLinkInNewTab;
        }

        internal bool Equals(Hyperlink h)
        {
            return h != null && Kind == h.Kind && Expression == h.Expression &&
              DetailReportName == h.DetailReportName && ReportParameter == h.ReportParameter &&
              DetailPageName == h.DetailPageName;
        }

        internal void SetParent(ReportComponentBase parent)
        {
            this.parent = parent;
        }

        internal void Calculate()
        {
            if (!String.IsNullOrEmpty(Expression))
            {
                object value = Report.Calc(Expression);
                Value = value == null ? "" : value.ToString();
            }
        }

        internal Report GetReport(bool updateParameter)
        {
            Report report = Report.FromFile(DetailReportName);
            Report.Dictionary.ReRegisterData(report.Dictionary);

            if (updateParameter)
                SetParameters(report);
            return report;
        }

        internal void SetParameters(Report report)
        {
            if (!String.IsNullOrEmpty(ReportParameter))
            {
                Parameter param = report.GetParameter(ReportParameter);
                if (param != null)
                {
                    if (Value.IndexOf(ValuesSeparator) != -1)
                    {
                        string[] values = Value.Split(new string[] { ValuesSeparator }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (i < param.Parameters.Count)
                                param.Parameters[i].AsString = values[i];
                        }
                    }
                    else
                    {
                        param.AsString = Value;
                    }
                }
            }
        }

        internal void Serialize(FRWriter writer, Hyperlink hyperlink)
        {
            if (Kind != hyperlink.Kind)
                writer.WriteValue("Hyperlink.Kind", Kind);
            if (Expression != hyperlink.Expression)
                writer.WriteStr("Hyperlink.Expression", Expression);
            if (Value != hyperlink.Value)
                writer.WriteStr("Hyperlink.Value", Value);
            if (DetailReportName != hyperlink.DetailReportName)
            {
                // when saving to the report file, convert absolute path to the external report to relative path
                // (based on the main report path).
                string value = DetailReportName;
                if (writer.SerializeTo == SerializeTo.Report && Report != null && !String.IsNullOrEmpty(Report.FileName))
                    value = FileUtils.GetRelativePath(DetailReportName, Path.GetDirectoryName(Report.FileName));
                writer.WriteStr("Hyperlink.DetailReportName", value);
            }
            if (DetailPageName != hyperlink.DetailPageName)
                writer.WriteStr("Hyperlink.DetailPageName", DetailPageName);
            if (ReportParameter != hyperlink.ReportParameter)
                writer.WriteStr("Hyperlink.ReportParameter", ReportParameter);
            if (ValuesSeparator != hyperlink.ValuesSeparator)
                writer.WriteStr("Hyperlink.ValuesSeparator", ValuesSeparator);
            if (OpenLinkInNewTab != hyperlink.OpenLinkInNewTab)
                writer.WriteBool("Hyperlink.OpenLinkInNewTab", OpenLinkInNewTab);
        }

        internal void OnAfterLoad()
        {
            // convert relative path to the external report to absolute path (based on the main report path).
            if (String.IsNullOrEmpty(DetailReportName) || String.IsNullOrEmpty(Report.FileName))
                return;

            if (!Path.IsPathRooted(DetailReportName))
                DetailReportName = Path.GetDirectoryName(Report.FileName) + Path.DirectorySeparatorChar + DetailReportName;
        }

        internal void SaveState()
        {
            saveValue = Value;
        }

        internal void RestoreState()
        {
            Value = saveValue;
        }
        #endregion

        internal Hyperlink(ReportComponentBase parent)
        {
            SetParent(parent);
            expression = "";
            value = "";
            detailReportName = "";
            detailPageName = "";
            reportParameter = "";
            valuesSeparator = ";";
        }
    }
}

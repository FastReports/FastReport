using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// Represents a subreport object.
    /// </summary>
    /// <remarks>
    /// To create a subreport in code, you should create the report page first and 
    /// connect it to the subreport using the <see cref="ReportPage"/> property.
    /// </remarks>
    /// <example>The following example shows how to create a subreport object in code.
    /// <code>
    /// // create the main report page
    /// ReportPage reportPage = new ReportPage();
    /// reportPage.Name = "Page1";
    /// report.Pages.Add(reportPage);
    /// // create report title band
    /// reportPage.ReportTitle = new ReportTitleBand();
    /// reportPage.ReportTitle.Name = "ReportTitle1";
    /// reportPage.ReportTitle.Height = Units.Millimeters * 10;
    /// // add subreport on it
    /// SubreportObject subreport = new SubreportObject();
    /// subreport.Name = "Subreport1";
    /// subreport.Bounds = new RectangleF(0, 0, Units.Millimeters * 25, Units.Millimeters * 5);
    /// reportPage.ReportTitle.Objects.Add(subreport);
    /// // create subreport page
    /// ReportPage subreportPage = new ReportPage();
    /// subreportPage.Name = "SubreportPage1";
    /// report.Pages.Add(subreportPage);
    /// // connect the subreport to the subreport page
    /// subreport.ReportPage = subreportPage;
    /// </code>
    /// </example>
    public partial class SubreportObject : ReportComponentBase
    {
        private ReportPage reportPage;
        private bool printOnParent;

        #region Properties
        /// <summary>
        /// Gets or sets a report page that contains the subreport bands and objects.
        /// </summary>
        //[Browsable(false)]
        [Editor("FastReport.TypeEditors.SubreportPageEditor, FastReport", typeof(UITypeEditor))]
        [TypeConverter("FastReport.TypeConverters.ComponentRefConverter, FastReport")]
        public ReportPage ReportPage
        {
            get { return reportPage; }
            set
            {
                if (reportPage != null)
                    reportPage.Subreport = null;
                if (value != null)
                {
                    value.Subreport = this;
                    value.PageName = Name;
                }
                reportPage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that subreport must print its objects on a parent band to which it belongs.
        /// </summary>
        /// <remarks>
        /// Default behavior of the subreport is to print subreport objects they own separate bands.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool PrintOnParent
        {
            get { return printOnParent; }
            set { printOnParent = value; }
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            SubreportObject src = source as SubreportObject;
            PrintOnParent = src.PrintOnParent;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            SubreportObject c = writer.DiffObject as SubreportObject;
            base.Serialize(writer);

            writer.WriteRef("ReportPage", ReportPage);
            if (PrintOnParent != c.PrintOnParent)
                writer.WriteBool("PrintOnParent", PrintOnParent);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SubreportObject"/> class with default settings.
        /// </summary>
        public SubreportObject()
        {
            Fill = new SolidFill(SystemColors.Control);
            FlagUseBorder = false;
            FlagUseFill = false;
            FlagPreviewVisible = false;
            SetFlags(Flags.CanCopy, false);
        }
    }
}
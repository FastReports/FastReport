using System;
using System.Collections.Generic;
using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Configures a report definition for a specific data source type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReportBuilder<T>
    {
        internal IEnumerable<T> _data;
        internal List<DataDefinition> _columns = new List<DataDefinition>();
        internal DataHeaderDefinition _dataHeader = new DataHeaderDefinition();
        internal ReportDefinition _report = new ReportDefinition();
        internal ReportTitleDefinition _reportTitle = new ReportTitleDefinition();
        internal TextBandDefinition _pageHeader = new TextBandDefinition();
        internal TextBandDefinition _pageFooter = new TextBandDefinition();
        internal TextBandDefinition _reportSummary = new TextBandDefinition();
        internal GroupHeaderDefinition _groupHeader = new GroupHeaderDefinition();
        internal GroupHeaderDefinition _groupFooter = new GroupHeaderDefinition();

        /// <summary>
        /// Initializes a new report builder for the provided data source.
        /// </summary>
        /// <param name="data">The records that will be registered with the FastReport data source.</param>
        public ReportBuilder(IEnumerable<T> data)
        {
            _data = data;
        }

        /// <summary>
        /// Add a report title band
        /// </summary>
        /// <param name="config">The configuration callback used to customize the report title band.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> ReportTitle(Action<ReportTitleBuilder<T>> config)
        {
            var builder = new ReportTitleBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a data header band
        /// </summary>
        /// <param name="config">The configuration callback used to customize the data header band.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> DataHeader(Action<DataHeaderBuilder<T>> config)
        {
            var builder = new DataHeaderBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a data band with columns
        /// </summary>
        /// <param name="config">The configuration callback used to define report columns.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> Data(Action<DataBuilder<T>> config)
        {
            var builder = new DataBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a group header band for grouping rows
        /// </summary>
        /// <param name="config">The configuration callback used to customize the group header band.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> GroupHeader(Action<GroupHeaderBuilder<T>> config)
        {
            var builder = new GroupHeaderBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a page header band
        /// </summary>
        /// <param name="config">The configuration callback used to customize the page header band.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> PageHeader(Action<PageHeaderBuilder<T>> config)
        {
            var builder = new PageHeaderBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a page footer band
        /// </summary>
        /// <param name="config">The configuration callback used to customize the page footer band.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> PageFooter(Action<PageFooterBuilder<T>> config)
        {
            var builder = new PageFooterBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a report summary band
        /// </summary>
        /// <param name="config">The configuration callback used to customize the report summary band.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> ReportSummary(Action<ReportSummaryBuilder<T>> config)
        {
            var builder = new ReportSummaryBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Set report font family name, size, style
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <param name="emSize">The font size in points.</param>
        /// <param name="style">The font style to apply.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> Font(string familyName, float emSize, FontStyle style)
        {
            _report.Font = new Font(familyName, emSize, style);
            return this;
        }

        /// <summary>
        /// Set report font family name, size
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <param name="emSize">The font size in points.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> Font(string familyName, float emSize)
        {
            return Font(familyName, emSize, FontStyle.Regular);
        }

        /// <summary>
        /// Set report font family name
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> Font(string familyName)
        {
            return Font(familyName, 10.0f, FontStyle.Regular);
        }

        /// <summary>
        /// Align report content vertical top, bottom, center
        /// </summary>
        /// <param name="vertAlign">The default vertical alignment for report text objects.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> VertAlign(VertAlign vertAlign)
        {
            _report.VertAlign = vertAlign;
            return this;
        }

        /// <summary>
        /// Align report content horizontal left, right, center, justify
        /// </summary>
        /// <param name="horzAlign">The default horizontal alignment for report text objects.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> HorzAlign(HorzAlign horzAlign)
        {
            _report.HorzAlign = horzAlign;
            return this;
        }

        /// <summary>
        /// Set page orientation
        /// </summary>
        /// <param name="landscape"><see langword="true"/> to render the page in landscape orientation; otherwise, <see langword="false"/>.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> Landscape(bool landscape = true)
        {
            _report.Landscape = landscape;
            return this;
        }

        /// <summary>
        /// Set page size in millimeters
        /// </summary>
        /// <param name="width">The paper width in millimeters.</param>
        /// <param name="height">The paper height in millimeters.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> PaperSize(float width, float height)
        {
            _report.PaperWidth = width;
            _report.PaperHeight = height;
            return this;
        }

        /// <summary>
        /// Set page margins in millimeters
        /// </summary>
        /// <param name="left">The left margin in millimeters.</param>
        /// <param name="top">The top margin in millimeters.</param>
        /// <param name="right">The right margin in millimeters.</param>
        /// <param name="bottom">The bottom margin in millimeters.</param>
        /// <returns>The current report builder.</returns>
        public ReportBuilder<T> Margins(float left, float top, float right, float bottom)
        {
            _report.LeftMargin = left;
            _report.TopMargin = top;
            _report.RightMargin = right;
            _report.BottomMargin = bottom;
            return this;
        }
    }
}

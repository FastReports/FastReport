using FastReport;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReportBuilder<T>
    {
        internal IEnumerable<T> _data;
        internal List<DataDefinition> _columns = new List<DataDefinition>();
        internal DataHeaderDefinition _dataHeader = new DataHeaderDefinition();
        internal ReportDefinition _report = new ReportDefinition();
        internal ReportTitleDefinition _reportTitle = new ReportTitleDefinition();
        internal GroupHeaderDefinition _groupHeader = new GroupHeaderDefinition();
        internal GroupHeaderDefinition _groupFooter = new GroupHeaderDefinition();

        public ReportBuilder(IEnumerable<T> data)
        {
            _data = data;
        }

        /// <summary>
        /// Add a report title band
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ReportBuilder<T> ReportTitle(Action<ReportTitleBuilder<T>> config)
        {
            var builder = new ReportTitleBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a data header band
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ReportBuilder<T> DataHeader(Action<DataHeaderBuilder<T>> config)
        {
            var builder = new DataHeaderBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a data band with columns
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ReportBuilder<T> Data(Action<DataBuilder<T>> config)
        {
            var builder = new DataBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Add a group header band for grouping rows
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ReportBuilder<T> GroupHeader(Action<GroupHeaderBuilder<T>> config)
        {
            var builder = new GroupHeaderBuilder<T>(this);
            config(builder);
            return this;
        }

        /// <summary>
        /// Set report font family name, size, style
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="emSize"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public ReportBuilder<T> Font(string familyName, float emSize, FontStyle style)
        {
            _report.Font = new Font(familyName, emSize, style);
            return this;
        }

        /// <summary>
        /// Set report font family name, size
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="emSize"></param>
        /// <returns></returns>
        public ReportBuilder<T> Font(string familyName, float emSize)
        {
            return Font(familyName, emSize, FontStyle.Regular);
        }

        /// <summary>
        /// Set report font family name
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public ReportBuilder<T> Font(string familyName)
        {
            return Font(familyName, 10.0f, FontStyle.Regular);
        }

        /// <summary>
        /// Align report content vertical top, bottom, center
        /// </summary>
        /// <param name="vertAlign"></param>
        /// <returns></returns>
        public ReportBuilder<T> VertAlign(VertAlign vertAlign)
        {
            _report.VertAlign = vertAlign;
            return this;
        }

        /// <summary>
        /// Align report content horizontal left, right, center, justify
        /// </summary>
        /// <param name="horzAlign"></param>
        /// <returns></returns>
        public ReportBuilder<T> HorzAlign(HorzAlign horzAlign)
        {
            _report.HorzAlign = horzAlign;
            return this;
        }
    }
}

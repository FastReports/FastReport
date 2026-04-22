using FastReport;
using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReportTitleBuilder<T>
    {
        private readonly ReportBuilder<T> _report;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        public ReportTitleBuilder(ReportBuilder<T> report)
        {
            _report = report;
        }

        /// <summary>
        /// Report title text (or add an expression)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> Text(string text)
        {
            _report._reportTitle.Text = text;
            _report._reportTitle.Visible = true;
            return this;
        }

        /// <summary>
        /// Set report title font family name, size, style
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="emSize"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> Font(string familyName, float emSize, FontStyle style)
        {
            _report._reportTitle.Font = new Font(familyName, emSize, style);
            return this;
        }

        /// <summary>
        /// Set report title font family name, size
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="emSize"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> Font(string familyName, float emSize)
        {
            return Font(familyName, emSize, FontStyle.Regular | FontStyle.Bold);
        }

        /// <summary>
        /// Set report title font family name
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> Font(string familyName)
        {
            return Font(familyName, 14, FontStyle.Regular | FontStyle.Bold);
        }

        /// <summary>
        /// Set report title visibility
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> Visible(bool visible)
        {
            _report._reportTitle.Visible = visible;
            return this;
        }

        /// <summary>
        /// Set report title text color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> TextColor(Color color)
        {
            _report._reportTitle.TextColor = color;
            return this;
        }

        /// <summary>
        /// Set report title background color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> FillColor(Color color)
        {
            _report._reportTitle.FillColor = color;
            return this;
        }

        /// <summary>
        /// Align report title content vertical top, bottom, center
        /// </summary>
        /// <param name="vertAlign"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> VertAlign(VertAlign vertAlign)
        {
            _report._reportTitle.VertAlign = vertAlign;
            return this;
        }

        /// <summary>
        /// Align report title content horizontal left, right, center, justify
        /// </summary>
        /// <param name="horzAlign"></param>
        /// <returns></returns>
        public ReportTitleBuilder<T> HorzAlign(HorzAlign horzAlign)
        {
            _report._reportTitle.HorzAlign = horzAlign;
            return this;
        }
    }
}

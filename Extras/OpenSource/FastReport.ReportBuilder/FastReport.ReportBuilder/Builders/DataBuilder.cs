using System;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataBuilder<T>
    {
        private readonly ReportBuilder<T> _report;
        private DataDefinition _column { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        public DataBuilder(ReportBuilder<T> report)
        {
            _report = report;
        }

        /// <summary>
        /// Add a column with expression
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public DataBuilder<T> Column<TProp>(Expression<Func<T, TProp>> property)
        {
            var member = property.Body as MemberExpression;
            _column = new DataDefinition
            {
                Name = GenericHelpers<T>.PropertyName(property),
                Title = member.Member.GetCustomAttribute<DisplayAttribute>()?.Name ?? GenericHelpers<T>.PropertyName(property),
                Format = member.Member.GetCustomAttribute<DisplayFormatAttribute>()?.DataFormatString ?? ""
            };
            _report._columns.Add(_column);
            return this;
        }

        /// <summary>
        /// Column title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public DataBuilder<T> Title(string title)
        {
            _column.Title = title;
            return this;
        }

        /// <summary>
        /// Percentage width
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public DataBuilder<T> Width(uint width)
        {
            _column.Width = width;
            return this;
        }

        /// <summary>
        /// Formats of datetime, currency, number etc.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public DataBuilder<T> Format(string format)
        {
            _column.Format = format;
            return this;
        }

        /// <summary>
        /// Rendering column with expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DataBuilder<T> Expression(string expression)
        {
            _column.Expression = expression;
            return this;
        }

        /// <summary>
        /// Align column content vertical top, bottom, center
        /// </summary>
        /// <param name="vertAlign"></param>
        /// <returns></returns>
        public DataBuilder<T> VertAlign(VertAlign vertAlign)
        {
            _column.VertAlign = vertAlign;
            return this;
        }

        /// <summary>
        /// Align column content horizontal left, right, center, justify
        /// </summary>
        /// <param name="horzAlign"></param>
        /// <returns></returns>
        public DataBuilder<T> HorzAlign(HorzAlign horzAlign)
        {
            _column.HorzAlign = horzAlign;
            return this;
        }
    }
}

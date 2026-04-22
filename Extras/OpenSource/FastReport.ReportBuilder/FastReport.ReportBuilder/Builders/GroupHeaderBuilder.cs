using System;
using System.Linq.Expressions;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Group header band
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupHeaderBuilder<T>
    {
        private readonly ReportBuilder<T> _report;

        /// <summary>
        /// Group header band
        /// </summary>
        /// <param name="report"></param>
        public GroupHeaderBuilder(ReportBuilder<T> report)
        {
            _report = report;
        }

        /// <summary>
        /// Set a property for data group condition
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public GroupHeaderBuilder<T> Condition<TProp>(Expression<Func<T, TProp>> property)
        {
            _report._groupHeader.Name = GenericHelpers<T>.PropertyName(property);
            _report._groupHeader.Visible = true;
            return this;
        }

        /// <summary>
        /// Sort order group None, Ascending, Descending
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public GroupHeaderBuilder<T> SortOrder(SortOrder order)
        {
            _report._groupHeader.SortOrder = order;
            return this;
        }

        /// <summary>
        /// Customize condition with expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public GroupHeaderBuilder<T> Expression(string expression)
        {
            _report._groupHeader.Expression = expression;
            return this;
        }

        /// <summary>
        /// Set group header text visibility
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        public GroupHeaderBuilder<T> TextVisible(bool visible)
        {
            _report._groupHeader.TextVisible = visible;
            return this;
        }
    }
}

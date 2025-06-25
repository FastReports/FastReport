using System;
using System.Collections;
using FastReport.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Data
{
    public abstract partial class DataSourceBase
    {
        #region Public Methods
        /// <summary>
        /// Initializes the datasource schema.
        /// </summary>
        /// <remarks>
        /// This method is used to support the FastReport.Net infrastructure. Do not call it directly.
        /// </remarks>
        public abstract Task InitSchemaAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the datasource with data.
        /// </summary>
        /// <remarks>
        /// This method is used to support the FastReport.Net infrastructure. Do not call it directly.
        /// </remarks>
        /// <param name="rows">Rows to fill with data.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        public abstract Task LoadDataAsync(ArrayList rows, CancellationToken cancellationToken = default);

        internal Task LoadDataAsync(CancellationToken cancellationToken)
        {
            return LoadDataAsync(InternalRows, cancellationToken);
        }

        internal async Task FindParentRowAsync(Relation relation, CancellationToken cancellationToken)
        {
            await InitSchemaAsync(cancellationToken);
            await LoadDataAsync(cancellationToken);

            int columnCount = relation.ChildColumns.Length;
            object[] childValues = new object[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                childValues[i] = relation.ChildDataSource[relation.ChildColumns[i]];
            }

            object result = null;
            if (childValues[0] == null)
            {
                SetCurrentRow(null);
                return;
            }

            // improve performance for single column index
            if (columnCount == 1)
            {
                if (rowIndices.Count == 0)
                {
                    foreach (object row in InternalRows)
                    {
                        SetCurrentRow(row);
                        rowIndices[this[relation.ParentColumns[0]]] = row;
                    }
                }

                result = rowIndices[childValues[0]];
                if (result != null)
                {
                    SetCurrentRow(result);
                    return;
                }
            }

            foreach (object row in InternalRows)
            {
                SetCurrentRow(row);
                bool found = true;

                for (int i = 0; i < columnCount; i++)
                {
                    if (!this[relation.ParentColumns[i]].Equals(childValues[i]))
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    result = row;
                    break;
                }
            }

            if (columnCount == 1)
                rowIndices[childValues[0]] = result;

            SetCurrentRow(result);
        }

        /// <summary>
        /// Initializes this datasource.
        /// </summary>
        /// <remarks>
        /// This method fills the table with data. You should always call it before using most of
        /// datasource properties.
        /// </remarks>
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            return InitAsync("", cancellationToken);
        }

        /// <summary>
        /// Initializes this datasource and applies the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        public Task InitAsync(string filter, CancellationToken cancellationToken = default)
        {
            return InitAsync(filter, null, cancellationToken);
        }

        /// <summary>
        /// Initializes this datasource, applies the specified filter and sorts the rows.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="sort">The collection of sort descriptors.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        public Task InitAsync(string filter, SortCollection sort, CancellationToken cancellationToken = default)
        {
            DataSourceBase parentData = null;
            return InitAsync(parentData, filter, sort, cancellationToken);
        }

        /// <summary>
        /// Initializes this datasource and filters data rows according to the master-detail relation between
        /// this datasource and <b>parentData</b>.
        /// </summary>
        /// <param name="parentData">Parent datasource.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <remarks>
        /// To use master-detail relation, you must define the <see cref="Relation"/> object that describes
        /// the relation, and add it to the <b>Report.Dictionary.Relations</b> collection.
        /// </remarks>
        public Task InitAsync(DataSourceBase parentData, CancellationToken cancellationToken = default)
        {
            return InitAsync(parentData, "", null, cancellationToken);
        }

        /// <summary>
        /// Initializes this datasource and filters data rows according to the master-detail relation between
        /// this datasource and <b>parentData</b>. Also applies the specified filter and sorts the rows.
        /// </summary>
        /// <param name="parentData">Parent datasource.</param>
        /// <param name="filter">The filter expression.</param>
        /// <param name="sort">The collection of sort descriptors.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <remarks>
        /// To use master-detail relation, you must define the <see cref="Relation"/> object that describes
        /// the relation, and add it to the <b>Report.Dictionary.Relations</b> collection.
        /// </remarks>
        public Task InitAsync(DataSourceBase parentData, string filter, SortCollection sort, CancellationToken cancellationToken = default)
        {
            return InitAsync(parentData, filter, sort, false, cancellationToken);
        }

        /// <summary>
        /// Initializes this datasource and filters data rows according to the master-detail relation.
        /// Also applies the specified filter and sorts the rows.
        /// </summary>
        /// <param name="relation">The master-detail relation.</param>
        /// <param name="filter">The filter expression.</param>
        /// <param name="sort">The collection of sort descriptors.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <remarks>
        /// To use master-detail relation, you must define the <see cref="Relation"/> object that describes
        /// the relation, and add it to the <b>Report.Dictionary.Relations</b> collection.
        /// </remarks>
        public Task InitAsync(Relation relation, string filter, SortCollection sort, CancellationToken cancellationToken = default)
        {
            return InitAsync(relation, filter, sort, false, cancellationToken);
        }

        internal Task InitAsync(DataSourceBase parentData, string filter, SortCollection sort, bool useAllParentRows, CancellationToken cancellationToken)
        {
            Relation relation = parentData != null ? DataHelper.FindRelation(Report.Dictionary, parentData, this) : null;
            return InitAsync(relation, filter, sort, useAllParentRows, cancellationToken);
        }

        internal async Task InitAsync(Relation relation, string filter, SortCollection sort, bool useAllParentRows, CancellationToken cancellationToken)
        {
            if (FShowAccessDataMessage)
                Config.ReportSettings.OnProgress(Report, Res.Get("Messages,AccessingData"));

            // InitSchema may fail sometimes (for example, when using OracleConnection with nested select).
            try
            {
                await InitSchemaAsync(cancellationToken);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
            await LoadDataAsync(cancellationToken);

            InitShared(relation, filter, sort, useAllParentRows);
        }

        /// <summary>
        /// Initializes the data source if it is not initialized yet.
        /// </summary>
        public async Task EnsureInitAsync(CancellationToken cancellationToken = default)
        {
            if (InternalRows.Count == 0)
                await InitAsync(cancellationToken);
        }

        #endregion
    }
}

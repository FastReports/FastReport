using System.Data;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Data
{
    public partial class TableDataSource
    {
        #region Public Methods
        /// <inheritdoc/>
        public override async Task InitSchemaAsync(CancellationToken cancellationToken = default)
        {
            if (Connection != null)
            {
                if (!StoreData)
                {
                    await Connection.CreateTableAsync(this, cancellationToken);
                    if (Table.Columns.Count == 0)
                        await Connection.FillTableSchemaAsync(Table, SelectCommand, Parameters, cancellationToken);
                }
            }
            else
                table = Reference as DataTable;

            InitSchemaShared();
        }

        /// <inheritdoc/>
        public override async Task LoadDataAsync(ArrayList rows, CancellationToken cancellationToken)
        {
            if (Connection != null)
            {
                if (!StoreData)
                    await Connection.FillTableAsync(this, cancellationToken);
            }
            else
            {
                TryToLoadData();
            }
            LoadDataShared(rows);
        }


        /// <summary>
        /// Refresh the table schema.
        /// </summary>
        public async Task RefreshTableAsync(CancellationToken cancellationToken)
        {
            DeleteTable();
            await InitSchemaAsync(cancellationToken);
            RefreshColumns(true);
        }
        #endregion

    }
}
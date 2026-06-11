using System;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Data
{

    public partial class BusinessObjectDataSource
    {
        /// <inheritdoc/>
        public override Task InitSchemaAsync(CancellationToken cancellationToken = default)
        {
            InitSchema();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task LoadDataAsync(ArrayList rows, CancellationToken cancellationToken = default)
        {
            LoadData(rows);
            return Task.CompletedTask;
        }
    }
}

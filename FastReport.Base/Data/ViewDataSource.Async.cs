using System;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Data
{

    public partial class ViewDataSource
    {
        public override Task InitSchemaAsync(CancellationToken cancellationToken = default)
        {
            InitSchema();
            return Task.CompletedTask;
        }

        public override Task LoadDataAsync(ArrayList rows, CancellationToken cancellationToken = default)
        {
            LoadData(rows);
            return Task.CompletedTask;
        }
    }
}

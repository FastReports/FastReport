using System;
using System.Collections.Generic;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Data;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport
{
    public partial class DataBand
    {
        #region Public Methods

        /// <summary>
        /// Initializes the data source connected to this band.
        /// </summary>
        public async Task InitDataSourceAsync(CancellationToken cancellationToken = default)
        {
            if (DataSource == null)
            {
                DataSource = new VirtualDataSource();
                DataSource.SetReport(Report);
            }

            if (DataSource is VirtualDataSource)
                (DataSource as VirtualDataSource).VirtualRowsCount = RowCount;

            DataSourceBase parentDataSource = ParentDataBand == null ? null : ParentDataBand.DataSource;
            bool collectChildRows = ParentDataBand == null ? false : ParentDataBand.CollectChildRows;
            if (Relation != null)
                await DataSource.InitAsync(Relation, Filter, Sort, collectChildRows, cancellationToken);
            else
                await DataSource.InitAsync(parentDataSource, Filter, Sort, collectChildRows, cancellationToken);
        }

        internal override async Task<bool> IsEmptyAsync(CancellationToken cancellationToken)
        {
            await InitDataSourceAsync(cancellationToken);
            if (IsDatasourceEmpty)
                return !PrintIfDatasourceEmpty;

            DataSource.First();
            while (DataSource.HasMoreRows)
            {
                if (!await IsDetailEmptyAsync(cancellationToken))
                    return false;
                DataSource.Next();
            }
            return true;
        }

        internal async Task<bool> IsDetailEmptyAsync(CancellationToken cancellationToken)
        {
            if (PrintIfDetailEmpty || Bands.Count == 0)
                return false;

            foreach (BandBase band in Bands)
            {
                if (!await band.IsEmptyAsync(cancellationToken))
                    return false;
            }
            return true;
        }

        #endregion
    }
}
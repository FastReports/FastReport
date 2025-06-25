using FastReport.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods

        private async Task ShowDataHeaderAsync(GroupHeaderBand groupBand, CancellationToken cancellationToken)
        {
            groupBand.RowNo = 0;

            DataHeaderBand header = groupBand.Header;
            if (header != null)
            {
                await ShowBandAsync(header, cancellationToken);
                if (header.RepeatOnEveryPage)
                    AddReprint(header);
            }

            DataFooterBand footer = groupBand.Footer;
            if (footer != null)
            {
                if (footer.RepeatOnEveryPage)
                    AddReprint(footer);
            }
        }

        private async Task ShowDataFooterAsync(GroupHeaderBand groupBand, CancellationToken cancellationToken)
        {
            DataFooterBand footer = groupBand.Footer;
            RemoveReprint(footer);
            await ShowBandAsync(footer, cancellationToken);
            RemoveReprint(groupBand.Header);
        }

        private async Task ShowGroupHeaderAsync(GroupHeaderBand header, CancellationToken cancellationToken)
        {
            header.AbsRowNo++;
            header.RowNo++;

            if (header.ResetPageNumber && (header.FirstRowStartsNewPage || header.RowNo > 1))
                ResetLogicalPageNumber();
            if (header.KeepTogether)
                StartKeep(header);
            if (header.KeepWithData)
                StartKeep(header.GroupDataBand);

            // start group event
            OnStateChanged(header, EngineState.GroupStarted);

            await ShowBandAsync(header, cancellationToken);
            if (header.RepeatOnEveryPage)
                AddReprint(header);

            GroupFooterBand footer = header.GroupFooter;
            if (footer != null)
            {
                if (footer.RepeatOnEveryPage)
                    AddReprint(footer);
            }
        }

        private async Task ShowGroupFooterAsync(GroupHeaderBand header, CancellationToken cancellationToken)
        {
            // finish group event
            OnStateChanged(header, EngineState.GroupFinished);

            // rollback to previous data row to print the header condition in the footer.
            DataBand dataBand = header.GroupDataBand;
            DataSourceBase dataSource = dataBand.DataSource;
            dataSource.Prior();

            GroupFooterBand footer = header.GroupFooter;
            if (footer != null)
            {
                footer.AbsRowNo++;
                footer.RowNo++;
            }
            RemoveReprint(footer);
            await ShowBandAsync(footer, cancellationToken);
            RemoveReprint(header);

            // restore current row
            dataSource.Next();

            OutlineUp(header);
            if (header.KeepTogether)
                EndKeep();
            if (footer != null && footer.KeepWithData)
                EndKeep();
        }


        private async Task ShowGroupTreeAsync(GroupTreeItem root, CancellationToken cancellationToken)
        {
            if (root.Band != null)
            {
                root.Band.GroupDataBand.DataSource.CurrentRowNo = root.RowNo;
                await ShowGroupHeaderAsync(root.Band, cancellationToken);
            }

            if (root.Items.Count == 0)
            {
                if (root.RowCount != 0)
                {
                    int rowCount = root.RowCount;
                    int maxRows = root.Band.GroupDataBand.MaxRows;
                    if (maxRows > 0 && rowCount > maxRows)
                        rowCount = maxRows;
                    bool keepFirstRow = NeedKeepFirstRow(root.Band);
                    bool keepLastRow = NeedKeepLastRow(root.Band.GroupDataBand);
                    await RunDataBandAsync(root.Band.GroupDataBand, rowCount, keepFirstRow, keepLastRow, cancellationToken);
                }
            }
            else
            {
                await ShowDataHeaderAsync(root.FirstItem.Band, cancellationToken);

                for (int i = 0; i < root.Items.Count; i++)
                {
                    GroupTreeItem item = root.Items[i];
                    item.Band.IsFirstRow = i == 0;
                    item.Band.IsLastRow = i == root.Items.Count - 1;

                    await ShowGroupTreeAsync(item, cancellationToken);
                    if (Report.Aborted)
                        break;
                }

                await ShowDataFooterAsync(root.FirstItem.Band, cancellationToken);
            }

            if (root.Band != null)
                await ShowGroupFooterAsync(root.Band, cancellationToken);
        }

        private async Task RunGroupAsync(GroupHeaderBand groupBand, CancellationToken cancellationToken)
        {
            DataSourceBase dataSource = groupBand.DataSource;
            if (dataSource != null)
            {
                // init the datasource - set group conditions to sort data rows
                await groupBand.InitDataSourceAsync(cancellationToken);

                // show the group tree
                await ShowGroupTreeAsync(MakeGroupTree(groupBand), cancellationToken);

                // finalize the datasource, remove the group condition
                // from the databand sort
                groupBand.FinalizeDataSource();

                // do not leave the datasource in EOF state to allow print something in the footer
                dataSource.Prior();
            }
        }

        #endregion Private Methods
    }
}

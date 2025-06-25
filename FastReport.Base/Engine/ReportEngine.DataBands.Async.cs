using FastReport.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods

        private async Task RunDataBandAsync(DataBand dataBand, CancellationToken cancellationToken)
        {
            if (page.Columns.Count > 1 && Report.Engine.UnlimitedHeight)
                dataBand.Columns.Count = page.Columns.Count;

            await dataBand.InitDataSourceAsync(cancellationToken);
            dataBand.DataSource.First();

            int rowCount = dataBand.DataSource.RowCount;
            if (dataBand.IsDatasourceEmpty && dataBand.PrintIfDatasourceEmpty)
                rowCount = 1;
            if (dataBand.CollectChildRows && rowCount > 1)
                rowCount = 1;
            if (dataBand.MaxRows > 0 && rowCount > dataBand.MaxRows)
                rowCount = dataBand.MaxRows;

            bool keepFirstRow = NeedKeepFirstRow(dataBand);
            bool keepLastRow = NeedKeepLastRow(dataBand);

            await RunDataBandAsync(dataBand, rowCount, keepFirstRow, keepLastRow, cancellationToken);

            // do not leave the datasource in EOF state to allow print something in the footer
            dataBand.DataSource.Prior();
        }

        private async Task RunDataBandAsync(DataBand dataBand, int rowCount, bool keepFirstRow, bool keepLastRow, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (dataBand.IsHierarchical)
            {
                await ShowHierarchyAsync(dataBand, rowCount, cancellationToken);
                return;
            }

            bool isFirstRow = true;
            bool someRowsPrinted = false;
            dataBand.RowNo = 0;
            dataBand.IsFirstRow = false;
            dataBand.IsLastRow = false;

            // check if we have only one data row that should be kept with both header and footer
            bool oneRow = rowCount == 1 && keepFirstRow && keepLastRow;

            // cycle through records
            for (int i = 0; i < rowCount; i++)
            {
                bool isLastRow = i == rowCount - 1;
                if (!await dataBand.IsDetailEmptyAsync(cancellationToken))
                {
                    dataBand.RowNo++;
                    dataBand.AbsRowNo++;
                    dataBand.IsFirstRow = isFirstRow;
                    dataBand.IsLastRow = isLastRow;

                    // keep header
                    if (isFirstRow && keepFirstRow)
                        StartKeep(dataBand);

                    // keep together
                    if (isFirstRow && dataBand.KeepTogether)
                        StartKeep(dataBand);

                    // keep detail
                    if (dataBand.KeepDetail)
                        StartKeep(dataBand);

                    // show header
                    if (isFirstRow)
                        await ShowDataHeaderAsync(dataBand, cancellationToken);

                    // keep footer
                    if (isLastRow && keepLastRow && dataBand.IsDeepmostDataBand)
                        StartKeep(dataBand);

                    // start block event
                    if (isFirstRow)
                        OnStateChanged(dataBand, EngineState.BlockStarted);

                    // show band
                    await ShowDataBandAsync(dataBand, rowCount, cancellationToken);

                    // end keep header
                    if (isFirstRow && keepFirstRow && !oneRow)
                        EndKeep();

                    // end keep footer
                    if (isLastRow && keepLastRow && dataBand.IsDeepmostDataBand)
                        CheckKeepFooter(dataBand);

                    // show sub-bands
                    await RunBandsAsync(dataBand.Bands, cancellationToken);

                    // up the outline
                    OutlineUp(dataBand);

                    // end keep detail
                    if (dataBand.KeepDetail)
                        EndKeep();

                    isFirstRow = false;
                    someRowsPrinted = true;

                    if (dataBand.Columns.Count > 1)
                        break;
                }

                dataBand.DataSource.Next();
                if (Report.Aborted)
                    break;
            }

            // complete upto N rows
            ChildBand child = dataBand.Child;
            if (child != null && child.CompleteToNRows > rowCount)
            {
                for (int i = 0; i < child.CompleteToNRows - rowCount; i++)
                {
                    child.RowNo = rowCount + i + 1;
                    child.AbsRowNo = rowCount + i + 1;
                    await ShowBandAsync(child, cancellationToken);
                }
            }
            // print child if databand is empty
            if (child != null && child.PrintIfDatabandEmpty && dataBand.IsDatasourceEmpty)
            {
                await ShowBandAsync(child, cancellationToken);
            }

            if (someRowsPrinted)
            {
                // finish block event
                OnStateChanged(dataBand, EngineState.BlockFinished);

                // show footer
                await ShowDataFooterAsync(dataBand, cancellationToken);

                // end KeepTogether
                if (dataBand.KeepTogether)
                    EndKeep();

                // end KeepLastRow
                if (keepLastRow)
                    EndKeep();
            }
        }

        private async Task ShowDataBandAsync(DataBand dataBand, int rowCount, CancellationToken cancellationToken)
        {
            if (dataBand.Columns.Count > 1)
            {
                dataBand.Width = dataBand.Columns.ActualWidth;
                await RenderMultiColumnBandAsync(dataBand, rowCount, cancellationToken);
            }
            else
            {
                if (dataBand.ResetPageNumber && (dataBand.FirstRowStartsNewPage || dataBand.RowNo > 1))
                    ResetLogicalPageNumber();
                if (dataBand.Footer != null && dataBand.CanBreak)
                    if (dataBand.Footer.KeepWithData && dataBand.Footer.Height + dataBand.Height > await GetFreeSpaceAsync(cancellationToken))
                    {
                        dataBand.AddLastToFooter(dataBand.Footer);
                    }
                await ShowBandAsync(dataBand, cancellationToken);
            }
        }

        private async Task RenderMultiColumnBandAsync(DataBand dataBand, int rowCount, CancellationToken cancellationToken)
        {
            if (dataBand.Columns.Layout == ColumnLayout.AcrossThenDown)
                RenderBandAcrossThenDown(dataBand, rowCount);
            else
            {
                DataSourceBase dataSource = dataBand.DataSource;
                int saveRow = dataSource.CurrentRowNo;

                // calc height of each data row. This list is shared across RenderBandDownThenAcross calls.
                Hashtable heights = new Hashtable();
                for (int i = 0; i < rowCount; i++)
                {
                    dataSource.CurrentRowNo = i + saveRow;
                    heights[i + saveRow] = await CalcHeightAsync(dataBand, cancellationToken);
                }

                dataSource.CurrentRowNo = saveRow;
                while (rowCount > 0)
                {
                    rowCount = RenderBandDownThenAcross(dataBand, rowCount, heights);
                }
            }
        }

        private async Task ShowHierarchyAsync(DataBand dataBand, HierarchyItem rootItem, int level, string fullRowNo, CancellationToken cancellationToken)
        {
            int saveLevel = hierarchyLevel;
            float saveIndent = hierarchyIndent;
            string saveRowNo = hierarchyRowNo;
            hierarchyLevel = level;
            hierarchyIndent = dataBand.Indent * (level - 1);

            try
            {
                if (rootItem.items.Count > 0)
                {
                    await ShowBandAsync(dataBand.Header, cancellationToken);

                    int rowNo = 0;
                    foreach (HierarchyItem item in rootItem.items)
                    {
                        rowNo++;
                        dataBand.RowNo = rowNo;
                        dataBand.AbsRowNo++;
                        dataBand.DataSource.CurrentRowNo = item.rowNo;
                        hierarchyRowNo = fullRowNo + rowNo.ToString();

                        // show the main hierarchy band
                        await ShowBandAsync(dataBand, cancellationToken);

                        // show sub-bands if any
                        await RunBandsAsync(dataBand.Bands, cancellationToken);

                        await ShowHierarchyAsync(dataBand, item, level + 1, hierarchyRowNo + ".", cancellationToken);

                        // up the outline
                        OutlineUp(dataBand);
                    }

                    await ShowBandAsync(dataBand.Footer, cancellationToken);
                }
            }
            finally
            {
                hierarchyLevel = saveLevel;
                hierarchyIndent = saveIndent;
                hierarchyRowNo = saveRowNo;
            }
        }

        private async Task ShowHierarchyAsync(DataBand dataBand, int rowCount, CancellationToken cancellationToken)
        {
            HierarchyItem rootItem = MakeHierarchy(dataBand, rowCount);
            if (rootItem == null)
                return;

            await ShowHierarchyAsync(dataBand, rootItem, 1, "", cancellationToken);
        }

        private async Task ShowDataHeaderAsync(DataBand dataBand, CancellationToken cancellationToken)
        {
            DataHeaderBand header = dataBand.Header;
            if (header != null)
            {
                await ShowBandAsync(header, cancellationToken);
                if (header.RepeatOnEveryPage)
                    AddReprint(header);
            }

            DataFooterBand footer = dataBand.Footer;
            if (footer != null)
            {
                if (footer.RepeatOnEveryPage)
                    AddReprint(footer);
            }
        }

        private async Task ShowDataFooterAsync(DataBand dataBand, CancellationToken cancellationToken)
        {
            dataBand.DataSource.Prior();

            DataFooterBand footer = dataBand.Footer;
            RemoveReprint(footer);
            await ShowBandAsync(footer, cancellationToken);
            RemoveReprint(dataBand.Header);

            dataBand.DataSource.Next();
        }

        #endregion Private Methods
    }
}

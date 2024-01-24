using FastReport.Matrix;
using FastReport.Table;
using System;
using System.Collections.Generic;
using System.Xml;

namespace FastReport.Import.RDL
{

    // Represents the RDL tables import
    public partial class RDLImport
    {
        private void LoadTableColumn(XmlNode tableColumnNode, TableColumn column)
        {
            XmlNodeList nodeList = tableColumnNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Width")
                {
                    column.Width = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "Visibility")
                {
                    LoadVisibility(node);
                }
            }
        }

        private void LoadTableColumns(XmlNode tableColumnsNode)
        {
            if (tableColumnsNode != null)
            {
                XmlNodeList nodeList = tableColumnsNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "TableColumn" || node.Name == "TablixColumn")
                    {
                        if (component is TableObject)
                        {
                            TableColumn column = new TableColumn();
                            (component as TableObject).Columns.Add(column);
                            LoadTableColumn(node, column);
                        }
                    }
                }
            }
        }

        private void LoadTableCell(XmlNode tableCellNode, ref int col)
        {
            int row = (component as TableObject).RowCount - 1;
            XmlNodeList nodeList = tableCellNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems" || node.Name == "CellContents")
                {
                    Base tempParent = parent;
                    ComponentBase tempComponent = component;
                    parent = (component as TableObject).GetCellData(col, row).Cell;
                    LoadReportItems(node);
                    component = tempComponent;
                    parent = tempParent;
                }
                else if (node.Name == "ColSpan")
                {
                    int colSpan = Convert.ToInt32(node.InnerText);
                    (component as TableObject).GetCellData(col, row).Cell.ColSpan = colSpan;
                    col += colSpan - 1;
                }
            }
        }

        private void LoadTableCells(XmlNode tableCellsNode)
        {
            int col = 0;
            XmlNodeList nodeList = tableCellsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "TableCell" || node.Name == "TablixCell")
                {
                    LoadTableCell(node, ref col);
                    col++;
                }
            }
        }

        private void LoadTableRow(XmlNode tableRowNode, TableRow row)
        {
            XmlNodeList nodeList = tableRowNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "TableCells" || node.Name == "TablixCells")
                {
                    LoadTableCells(node);
                }
                else if (node.Name == "Height")
                {
                    row.Height = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "Visibility")
                {
                    LoadVisibility(node);
                }
            }
        }

        private void LoadTableRows(XmlNode tableRowsNode)
        {
            XmlNodeList nodeList = tableRowsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "TableRow" || node.Name == "TablixRow")
                {
                    if (component is TableObject)
                    {
                        TableRow row = new TableRow();
                        (component as TableObject).Rows.Add(row);
                        LoadTableRow(node, row);
                    }
                }
            }
        }

        private void LoadHeader(XmlNode headerNode)
        {
            if (headerNode != null)
            {
                XmlNodeList nodeList = headerNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "TableRows" || node.Name == "TablixRows")
                    {
                        LoadTableRows(node);
                    }
                }
            }
        }

        private void LoadTableGroup(XmlNode tableGroupNode)
        {
            XmlNodeList nodeList = tableGroupNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Header")
                {
                    LoadHeader(node);
                }
                else if (node.Name == "Footer")
                {
                    LoadFooter(node);
                }
                else if (node.Name == "Visibility")
                {
                    LoadVisibility(node);
                }
            }
        }

        private void LoadTableGroups(XmlNode tableGroupsNode)
        {
            XmlNodeList nodeList = tableGroupsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "TableGroup")
                {
                    LoadTableGroup(node);
                }
            }
        }

        private void LoadDetails(XmlNode detailsNode)
        {
            if (detailsNode != null)
            {
                XmlNodeList nodeList = detailsNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "TableRows")
                    {
                        LoadTableRows(node);
                    }
                    else if (node.Name == "Visibility")
                    {
                        LoadVisibility(node);
                    }
                }
            }
        }

        private void LoadFooter(XmlNode footerNode)
        {
            if (footerNode != null)
            {
                XmlNodeList nodeList = footerNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "TableRows")
                    {
                        LoadTableRows(node);
                    }
                }
            }
        }

        private void LoadCorner(XmlNode cornerNode)
        {
            if (cornerNode != null)
            {
                XmlNodeList nodeList = cornerNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "ReportItems")
                    {
                        //LoadReportItems(node);
                    }
                }
            }
        }

        private void LoadDynamicColumns(XmlNode dynamicColumnsNode, List<XmlNode> dynamicColumns)
        {
            XmlNodeList nodeList = dynamicColumnsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Subtotal")
                {
                    XmlNodeList subtotalNodeList = node.ChildNodes;
                    foreach (XmlNode subtotalNode in subtotalNodeList)
                    {
                        if (subtotalNode.Name == "ReportItems")
                        {
                            dynamicColumns.Add(subtotalNode.Clone());
                        }
                    }
                }
                else if (node.Name == "ReportItems")
                {
                    dynamicColumns.Add(node.Clone());
                }
                else if (node.Name == "Visibility")
                {
                    LoadVisibility(node);
                }
            }
        }

        private XmlNode LoadStaticColumn(XmlNode staticColumnNode)
        {
            XmlNode staticColumn = null;
            XmlNodeList nodeList = staticColumnNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems")
                {
                    staticColumn = node.Clone();
                }
            }
            return staticColumn;
        }

        private void LoadStaticColumns(XmlNode staticColumnsNode, List<XmlNode> staticColumns)
        {
            XmlNodeList nodeList = staticColumnsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "StaticColumn")
                {
                    staticColumns.Add(LoadStaticColumn(node));
                }
            }
        }

        private float LoadColumnGrouping(XmlNode columnGroupingNode, List<XmlNode> dynamicColumns, List<XmlNode> staticColumns)
        {
            float cornerHeight = 0.8f * Utils.Units.Centimeters;
            XmlNodeList nodeList = columnGroupingNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Height")
                {
                    cornerHeight = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "DynamicColumns")
                {
                    LoadDynamicColumns(node, dynamicColumns);
                }
                else if (node.Name == "StaticColumns")
                {
                    LoadStaticColumns(node, staticColumns);
                }
            }
            return cornerHeight;
        }

        private float LoadColumnGroupings(XmlNode columnGroupingsNode, List<XmlNode> dynamicColumns, List<XmlNode> staticColumns)
        {
            float cornerHeight = 0.8f * Utils.Units.Centimeters;
            if (columnGroupingsNode != null)
            {
                XmlNodeList nodeList = columnGroupingsNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "ColumnGrouping")
                    {
                        if (component is MatrixObject)
                        {
                            cornerHeight = LoadColumnGrouping(node, dynamicColumns, staticColumns);
                        }
                    }
                }
            }
            return cornerHeight;
        }

        private void LoadDynamicRows(XmlNode dynamicRowsNode, List<XmlNode> dynamicRows)
        {
            XmlNodeList nodeList = dynamicRowsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Subtotal")
                {
                    XmlNodeList subtotalNodeList = node.ChildNodes;
                    foreach (XmlNode subtotalNode in subtotalNodeList)
                    {
                        if (subtotalNode.Name == "ReportItems")
                        {
                            dynamicRows.Add(subtotalNode.Clone());
                        }
                    }
                }
                else if (node.Name == "ReportItems")
                {
                    dynamicRows.Add(node.Clone());
                }
                else if (node.Name == "Visibility")
                {
                    LoadVisibility(node);
                }
            }
        }

        private XmlNode LoadStaticRow(XmlNode staticRowNode)
        {
            XmlNode staticRow = null;
            XmlNodeList nodeList = staticRowNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems")
                {
                    staticRow = node.Clone();
                }
            }
            return staticRow;
        }

        private void LoadStaticRows(XmlNode staticRowsNode, List<XmlNode> staticRows)
        {
            XmlNodeList nodeList = staticRowsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "StaticRow")
                {
                    staticRows.Add(LoadStaticRow(node));
                }
            }
        }

        private float LoadRowGrouping(XmlNode rowGroupingNode, List<XmlNode> dynamicRows, List<XmlNode> staticRows)
        {
            float cornerWidth = 2.5f * Utils.Units.Centimeters;
            XmlNodeList nodeList = rowGroupingNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Width")
                {
                    cornerWidth = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "DynamicRows")
                {
                    LoadDynamicRows(node, dynamicRows);
                }
                else if (node.Name == "StaticRows")
                {
                    LoadStaticRows(node, staticRows);
                }
            }
            return cornerWidth;
        }

        private float LoadRowGroupings(XmlNode rowGroupingsNode, List<XmlNode> dynamicRows, List<XmlNode> staticRows)
        {
            float cornerWidth = 2.5f * Utils.Units.Centimeters;
            if (rowGroupingsNode != null)
            {
                XmlNodeList nodeList = rowGroupingsNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "RowGrouping")
                    {
                        if (component is MatrixObject)
                        {
                            cornerWidth = LoadRowGrouping(node, dynamicRows, staticRows);
                        }
                    }
                }
            }
            return cornerWidth;
        }

        private void LoadMatrixCell(XmlNode matrixCellNode, MatrixCellDescriptor cell, int col)
        {
            int row = (component as MatrixObject).RowCount - 1;
            XmlNodeList nodeList = matrixCellNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems")
                {
                }
            }
        }

        private void LoadMatrixCells(XmlNode matrixCellsNode)
        {
            int col = 0;
            XmlNodeList nodeList = matrixCellsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "MatrixCell")
                {
                    if (component is MatrixObject)
                    {
                        MatrixCellDescriptor cell = new MatrixCellDescriptor();
                        (component as MatrixObject).Data.Cells.Add(cell);
                        LoadMatrixCell(node, cell, col);
                        col++;
                    }
                }
            }
        }

        private float LoadMatrixRow(XmlNode matrixRowNode, MatrixHeaderDescriptor row)
        {
            float rowHeight = 0.8f * Utils.Units.Centimeters;
            XmlNodeList nodeList = matrixRowNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Height")
                {
                    rowHeight = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "MatrixCells")
                {
                    LoadMatrixCells(node);
                }
            }
            return rowHeight;
        }

        private float LoadMatrixRows(XmlNode matrixRowsNode)
        {
            float rowHeight = 0.8f * Utils.Units.Centimeters;
            if (matrixRowsNode != null)
            {
                XmlNodeList nodeList = matrixRowsNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "MatrixRow")
                    {
                        if (component is MatrixObject)
                        {
                            MatrixHeaderDescriptor row = new MatrixHeaderDescriptor();
                            (component as MatrixObject).Data.Rows.Add(row);
                            rowHeight = LoadMatrixRow(node, row);
                        }
                    }
                }
            }
            return rowHeight;
        }

        private float LoadMatrixColumn(XmlNode matrixColumnNode, MatrixHeaderDescriptor column)
        {
            float columnWidth = 2.5f * Utils.Units.Centimeters;
            XmlNodeList nodeList = matrixColumnNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Width")
                {
                    columnWidth = UnitsConverter.SizeToPixels(node.InnerText);
                }
            }
            return columnWidth;
        }

        private float LoadMatrixColumns(XmlNode matrixColumnsNode)
        {
            float columnWidth = 2.5f * Utils.Units.Centimeters;
            if (matrixColumnsNode != null)
            {
                XmlNodeList nodeList = matrixColumnsNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "MatrixColumn")
                    {
                        if (component is MatrixObject)
                        {
                            MatrixHeaderDescriptor column = new MatrixHeaderDescriptor();
                            (component as MatrixObject).Data.Columns.Add(column);
                            columnWidth = LoadMatrixColumn(node, column);
                        }
                    }
                }
            }
            return columnWidth;
        }

        private void LoadTable(XmlNode tableNode)
        {
            component = ComponentsFactory.CreateTableObject(tableNode.Attributes["Name"].Value, parent);
            XmlNodeList nodeList = tableNode.ChildNodes;
            LoadReportItem(nodeList);
            XmlNode tableColumnsNode = null;
            XmlNode headerNode = null;
            XmlNode detailsNode = null;
            XmlNode footerNode = null;

            XmlNode tableRowsNode = null;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "TableColumns")
                {
                    tableColumnsNode = node.Clone();
                }
                else if (node.Name == "Header")
                {
                    headerNode = node.Clone();
                }
                else if (node.Name == "Details")
                {
                    detailsNode = node.Clone();
                }
                else if (node.Name == "Footer")
                {
                    footerNode = node.Clone();
                }
                else if (node.Name == "TablixBody")
                {
                    if (node.HasChildNodes)
                        foreach (XmlNode bodyChild in node.ChildNodes)
                            if (bodyChild.Name == "TablixColumns")
                            {
                                tableColumnsNode = bodyChild.Clone();
                            }
                            else if (bodyChild.Name == "TablixRows")
                            {
                                tableRowsNode = node.Clone();
                            }
                }
            }
            LoadTableColumns(tableColumnsNode);
            LoadHeader(headerNode != null ? headerNode : tableRowsNode);
            LoadDetails(detailsNode);
            LoadFooter(footerNode);
            (component as TableObject).CreateUniqueNames();
        }

        private bool IsTablixMatrix(XmlNode node)
        {
            if (node.HasChildNodes)
                foreach (XmlNode tablixItem in node.ChildNodes)
                {
                    if (tablixItem.Name == "TablixCorner")
                        return true;
                }
            return false;
        }




        private void LoadMatrix(XmlNode matrixNode)
        {
            component = ComponentsFactory.CreateMatrixObject(matrixNode.Attributes["Name"].Value, parent);
            MatrixObject matrix = component as MatrixObject;
            matrix.AutoSize = false;

            XmlNodeList nodeList = matrixNode.ChildNodes;
            LoadReportItem(nodeList);
            //XmlNode cornerNode = null;
            XmlNode columnGroupingsNode = null;
            XmlNode rowGroupingsNode = null;
            XmlNode matrixRowsNode = null;
            XmlNode matrixColumnsNode = null;
            foreach (XmlNode node in nodeList)
            {
                //if (node.Name == "Corner")
                //{
                //    cornerNode = node.Clone();
                //}
                /*else */
                if (node.Name == "ColumnGroupings")
                {
                    columnGroupingsNode = node.Clone();
                }
                else if (node.Name == "RowGroupings")
                {
                    rowGroupingsNode = node.Clone();
                }
                else if (node.Name == "MatrixColumns")
                {
                    matrixColumnsNode = node.Clone();
                }
                else if (node.Name == "MatrixRows")
                {
                    matrixRowsNode = node.Clone();
                }
            }

            //LoadCorner(cornerNode);

            List<XmlNode> dynamicColumns = new List<XmlNode>();
            List<XmlNode> staticColumns = new List<XmlNode>();
            LoadColumnGroupings(columnGroupingsNode, dynamicColumns, staticColumns);

            List<XmlNode> dynamicRows = new List<XmlNode>();
            List<XmlNode> staticRows = new List<XmlNode>();
            LoadRowGroupings(rowGroupingsNode, dynamicRows, staticRows);

            float columnWidth = LoadMatrixColumns(matrixColumnsNode);
            float rowHeight = LoadMatrixRows(matrixRowsNode);

            matrix.CreateUniqueNames();
            matrix.BuildTemplate();

            for (int i = 1; i < matrix.Columns.Count; i++)
            {
                matrix.Columns[i].Width = columnWidth;
            }
            for (int i = 1; i < matrix.Rows.Count; i++)
            {
                matrix.Rows[i].Height = rowHeight;
            }

            for (int i = 0; i < matrix.Columns.Count; i++)
            {
                for (int j = 0; j < matrix.Rows.Count; j++)
                {
                    matrix.GetCellData(i, j).Cell.Text = "";
                }
            }

            for (int i = 0; i < dynamicColumns.Count; i++)
            {
                Base tempParent = parent;
                ComponentBase tempComponent = component;
                parent = matrix.GetCellData(i + 1, 0).Cell;
                LoadReportItems(dynamicColumns[i]);
                component = tempComponent;
                parent = tempParent;
            }
            for (int i = 0; i < dynamicRows.Count; i++)
            {
                Base tempParent = parent;
                ComponentBase tempComponent = component;
                parent = matrix.GetCellData(0, i + 1).Cell;
                LoadReportItems(dynamicRows[i]);
                component = tempComponent;
                parent = tempParent;
            }
        }
    }
}

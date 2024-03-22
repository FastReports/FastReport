using FastReport.Utils;
using FastReport.Data;
using FastReport.Table;
using FastReport.Export.Image;
using FastReport.Barcode;
using FastReport.Matrix;
using FastReport.CrossView;
using FastReport.Format;
using FastReport.Functions;
using FastReport.Gauge.Linear;
using FastReport.Gauge.Simple;
using FastReport.Gauge.Radial;
using FastReport.Gauge.Simple.Progress;
using System.Runtime.CompilerServices;

namespace FastReport
{
    /// <summary>
    /// The FastReport.dll assembly initializer.
    /// </summary>
    public sealed class AssemblyInitializer : AssemblyInitializerBase
    {
        /// <summary>
        /// Registers all core objects, wizards, export filters.
        /// </summary>
        public AssemblyInitializer()
        {
            // report
            RegisteredObjects.AddReport(typeof(Report), 134);

            // pages
            RegisteredObjects.AddPage(typeof(ReportPage), "ReportPage", 135);

            // data items
            RegisteredObjects.InternalAdd(typeof(Column), "", 0);
            RegisteredObjects.InternalAdd(typeof(CommandParameter), "", 0);
            RegisteredObjects.InternalAdd(typeof(ProcedureParameter), "", 0);
            RegisteredObjects.InternalAdd(typeof(Relation), "", 0);
            RegisteredObjects.InternalAdd(typeof(Parameter), "", 0);
            RegisteredObjects.InternalAdd(typeof(Total), "", 0);
            RegisteredObjects.InternalAdd(typeof(TableDataSource), "", 0);
            RegisteredObjects.InternalAdd(typeof(ProcedureDataSource), "", 0);
            RegisteredObjects.InternalAdd(typeof(ViewDataSource), "", 0);
            RegisteredObjects.InternalAdd(typeof(BusinessObjectDataSource), "", 0);
            RegisteredObjects.InternalAdd(typeof(SliceCubeSource), "", 0);

            RegisteredObjects.InternalAddConnection(typeof(XmlDataConnection));
            RegisteredObjects.InternalAddConnection(typeof(CsvDataConnection));

            // json
            RegisteredObjects.InternalAdd(typeof(Data.JsonConnection.JsonTableDataSource), "", 0);
            //RegisteredObjects.Add(typeof(Data.JsonConnection.JsonObjectDataSource), "", 0);
            //RegisteredObjects.Add(typeof(Data.JsonConnection.JsonArrayDataSource), "", 0);
            RegisteredObjects.InternalAddConnection(typeof(Data.JsonConnection.JsonDataSourceConnection));
            // formats
            RegisteredObjects.InternalAdd(typeof(BooleanFormat), "", 0);
            RegisteredObjects.InternalAdd(typeof(CurrencyFormat), "", 0);
            RegisteredObjects.InternalAdd(typeof(CustomFormat), "", 0);
            RegisteredObjects.InternalAdd(typeof(DateFormat), "", 0);
            RegisteredObjects.InternalAdd(typeof(GeneralFormat), "", 0);
            RegisteredObjects.InternalAdd(typeof(NumberFormat), "", 0);
            RegisteredObjects.InternalAdd(typeof(PercentFormat), "", 0);
            RegisteredObjects.InternalAdd(typeof(TimeFormat), "", 0);


            // bands
            RegisteredObjects.Add(typeof(ReportTitleBand), "", 154, "Objects,Bands,ReportTitle");
            RegisteredObjects.Add(typeof(ReportSummaryBand), "", 155, "Objects,Bands,ReportSummary");
            RegisteredObjects.Add(typeof(PageHeaderBand), "", 156, "Objects,Bands,PageHeader");
            RegisteredObjects.Add(typeof(PageFooterBand), "", 157, "Objects,Bands,PageFooter");
            RegisteredObjects.Add(typeof(ColumnHeaderBand), "", 158, "Objects,Bands,ColumnHeader");
            RegisteredObjects.Add(typeof(ColumnFooterBand), "", 159, "Objects,Bands,ColumnFooter");
            RegisteredObjects.Add(typeof(DataHeaderBand), "", 160, "Objects,Bands,DataHeader");
            RegisteredObjects.Add(typeof(DataFooterBand), "", 161, "Objects,Bands,DataFooter");
            RegisteredObjects.Add(typeof(DataBand), "", 162, "Objects,Bands,Data");
            RegisteredObjects.Add(typeof(GroupHeaderBand), "", 163, "Objects,Bands,GroupHeader");
            RegisteredObjects.Add(typeof(GroupFooterBand), "", 164, "Objects,Bands,GroupFooter");
            RegisteredObjects.Add(typeof(ChildBand), "", 165, "Objects,Bands,Child");
            RegisteredObjects.Add(typeof(OverlayBand), "", 166, "Objects,Bands,Overlay");

            // report objects
            RegisteredObjects.InternalAdd(typeof(TextObject), "ReportPage", 102, 1);
            RegisteredObjects.InternalAdd(typeof(PictureObject), "ReportPage", 103, 2);

            RegisteredObjects.AddCategory("ReportPage,Shapes", 106, 4, "Objects,Shapes");
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 105, "Objects,Shapes,Line", 0, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 107, "Objects,Shapes,DiagonalLine", 1, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 150, "Objects,Shapes,DiagonalLine", 2, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 151, "Objects,Shapes,DiagonalLine", 3, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 152, "Objects,Shapes,DiagonalLine", 4, true);
            RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 108, "Objects,Shapes,Rectangle", 0);
            RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 109, "Objects,Shapes,RoundRectangle", 1);
            RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 110, "Objects,Shapes,Ellipse", 2);
            RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 111, "Objects,Shapes,Triangle", 3);
            RegisteredObjects.Add(typeof(ShapeObject), "ReportPage,Shapes", 131, "Objects,Shapes,Diamond", 4);
            RegisteredObjects.Add(typeof(PolyLineObject), "ReportPage,Shapes", 240, "Objects,Shapes,Polyline");
            RegisteredObjects.Add(typeof(PolygonObject), "ReportPage,Shapes", 241, "Objects,Shapes,Polygon");
            RegisteredObjects.Add(typeof(PolygonObject), "ReportPage,Shapes", 242, "Objects,Shapes,Pentagon", 0x50);
            RegisteredObjects.Add(typeof(PolygonObject), "ReportPage,Shapes", 243, "Objects,Shapes,Hexagon", 0x60);
            RegisteredObjects.Add(typeof(PolygonObject), "ReportPage,Shapes", 244, "Objects,Shapes,Heptagon", 0x70);
            RegisteredObjects.Add(typeof(PolygonObject), "ReportPage,Shapes", 245, "Objects,Shapes,Octagon", 0x80);
            RegisteredObjects.InternalAdd(typeof(SubreportObject), "ReportPage", 104, 5);
            RegisteredObjects.InternalAdd(typeof(ContainerObject), "ReportPage", 144, 5);

            RegisteredObjects.InternalAdd(typeof(TableObject), "ReportPage", 127, 6);
            RegisteredObjects.InternalAdd(typeof(TableColumn), "", 215);
            RegisteredObjects.InternalAdd(typeof(TableRow), "", 216);
            RegisteredObjects.InternalAdd(typeof(TableCell), "", 214);

#if !COMMUNITY
            RegisteredObjects.AddCategory("ReportPage,Matrix", 142, 7, "Objects,Matrix");
            RegisteredObjects.InternalAdd(typeof(MatrixObject), "ReportPage,Matrix", 142, 1);
#if !(WPF || AVALONIA)
            RegisteredObjects.InternalAdd(typeof(CrossViewObject), "ReportPage,Matrix", 247, 2);
#endif
#else
            RegisteredObjects.InternalAdd(typeof(MatrixObject), "ReportPage", 142, 7);
#endif


            RegisteredObjects.AddCategory("ReportPage,Barcodes", 123, 9, "Objects,BarcodeObject");
            RegisteredObjects.AddCategory("ReportPage,Barcodes,2D", 149, 9, "Objects,BarcodeObject,TwoD");
            RegisteredObjects.AddCategory("ReportPage,Barcodes,EANUPC", 123, 9, "Objects,BarcodeObject,EANUPC");
            RegisteredObjects.AddCategory("ReportPage,Barcodes,Post", 123, 9, "Objects,BarcodeObject,Post");
            RegisteredObjects.AddCategory("ReportPage,Barcodes,GS1", 123, 9, "Objects,BarcodeObject,GS1");
            RegisteredObjects.AddCategory("ReportPage,Barcodes,Others", 123, 9, "Objects,BarcodeObject,Others");
            for (int i = 0; i <= Barcodes.Items.Length - 1; i++)
            {
                var barcode = Barcodes.Items[i];
                RegisteredObjects.Add(typeof(BarcodeObject), "ReportPage,Barcodes," + barcode.category, barcode.category == "2D" ? 149 : 123, barcode.barcodeName, i);
            }

            RegisteredObjects.InternalAdd(typeof(CheckBoxObject), "ReportPage", 124, 10);
            RegisteredObjects.InternalAdd(typeof(ZipCodeObject), "ReportPage", 129, 14);
            RegisteredObjects.InternalAdd(typeof(CellularTextObject), "ReportPage", 121, 15);

            RegisteredObjects.AddCategory("ReportPage,Gauge", 140, 17, "Objects,Gauge");
            RegisteredObjects.Add(typeof(LinearGauge), "ReportPage,Gauge", 140, "Objects,Gauge,Linear", 0, false);
            RegisteredObjects.Add(typeof(SimpleGauge), "ReportPage,Gauge", 140, "Objects,Gauge,Simple", 0, false);
            RegisteredObjects.Add(typeof(RadialGauge), "ReportPage,Gauge", 140, "Objects,Gauge,Radial", 0, false);
            RegisteredObjects.Add(typeof(SimpleProgressGauge), "ReportPage,Gauge", 140, "Objects,Gauge,SimpleProgress", 0, false);

            RegisteredObjects.InternalAdd(typeof(HtmlObject), "ReportPage", 246, 18);
            RegisteredObjects.InternalAdd(typeof(RFIDLabel), "ReportPage", 265, 19);

            // exports
            RegisteredObjects.InternalAddExport(typeof(ImageExport), "Image", "Export,Image,File");

            // functions
            StdFunctions.Register();

        }
    }
}

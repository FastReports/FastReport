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
            RegisteredObjects.Add(typeof(Column), "", 0);
            RegisteredObjects.Add(typeof(CommandParameter), "", 0);
            RegisteredObjects.Add(typeof(Relation), "", 0);
            RegisteredObjects.Add(typeof(Parameter), "", 0);
            RegisteredObjects.Add(typeof(Total), "", 0);
            RegisteredObjects.Add(typeof(TableDataSource), "", 0);
            RegisteredObjects.Add(typeof(ViewDataSource), "", 0);
            RegisteredObjects.Add(typeof(BusinessObjectDataSource), "", 0);
            RegisteredObjects.Add(typeof(SliceCubeSource), "", 0);

            RegisteredObjects.AddConnection(typeof(XmlDataConnection));
            RegisteredObjects.AddConnection(typeof(CsvDataConnection));

            // json
            RegisteredObjects.Add(typeof(Data.JsonConnection.JsonTableDataSource), "", 0);
            //RegisteredObjects.Add(typeof(Data.JsonConnection.JsonObjectDataSource), "", 0);
            //RegisteredObjects.Add(typeof(Data.JsonConnection.JsonArrayDataSource), "", 0);
            RegisteredObjects.AddConnection(typeof(Data.JsonConnection.JsonDataSourceConnection));

            // formats
            RegisteredObjects.Add(typeof(BooleanFormat), "", 0);
            RegisteredObjects.Add(typeof(CurrencyFormat), "", 0);
            RegisteredObjects.Add(typeof(CustomFormat), "", 0);
            RegisteredObjects.Add(typeof(DateFormat), "", 0);
            RegisteredObjects.Add(typeof(GeneralFormat), "", 0);
            RegisteredObjects.Add(typeof(NumberFormat), "", 0);
            RegisteredObjects.Add(typeof(PercentFormat), "", 0);
            RegisteredObjects.Add(typeof(TimeFormat), "", 0);


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
            RegisteredObjects.Add(typeof(TextObject), "ReportPage", 102, 1);
            RegisteredObjects.Add(typeof(PictureObject), "ReportPage", 103, 2);

            RegisteredObjects.AddCategory("ReportPage,Shapes", 106, 4, "Objects,Shapes");
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 105, "Objects,Shapes,Line", 0, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 107, "Objects,Shapes,DiagonalLine", 1, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 150, "Objects,Shapes,DiagonalLine", 2, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 151, "Objects,Shapes,DiagonalLine", 3, true);
            RegisteredObjects.Add(typeof(LineObject), "ReportPage,Shapes", 152, "Objects,Shapes,DiagonalLine", 4, true);
            //RegisteredObjects.Add(typeof(BezierObject), "ReportPage,Shapes", 239, "Objects,Shapes,Bezier", 5, true);
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
            RegisteredObjects.Add(typeof(SubreportObject), "ReportPage", 104, 5);
            RegisteredObjects.Add(typeof(ContainerObject), "ReportPage", 144, 5);

            RegisteredObjects.Add(typeof(TableObject), "ReportPage", 127, 6);
            RegisteredObjects.Add(typeof(TableColumn), "", 215);
            RegisteredObjects.Add(typeof(TableRow), "", 216);
            RegisteredObjects.Add(typeof(TableCell), "", 214);
            RegisteredObjects.Add(typeof(MatrixObject), "ReportPage", 142, 7);

#if !COMMUNITY
            RegisteredObjects.Add(typeof(CrossViewObject), "ReportPage", 247, 8);
#endif

            RegisteredObjects.AddCategory("ReportPage,Barcodes", 123, 9, "Objects,BarcodeObject");
            for (int i = 0; i <= Barcodes.Items.Length - 1; i++)
                RegisteredObjects.Add(typeof(BarcodeObject), "ReportPage,Barcodes", -1, Barcodes.Items[i].barcodeName, i);

            RegisteredObjects.Add(typeof(CheckBoxObject), "ReportPage", 124, 10);

            RegisteredObjects.Add(typeof(ZipCodeObject), "ReportPage", 129, 14);
            RegisteredObjects.Add(typeof(CellularTextObject), "ReportPage", 121, 15);

            RegisteredObjects.AddCategory("ReportPage,Gauge", 140, 17, "Objects,Gauge");
            RegisteredObjects.Add(typeof(LinearGauge), "ReportPage,Gauge", -1, "Objects,Gauge,Linear", 0, false);
            RegisteredObjects.Add(typeof(SimpleGauge), "ReportPage,Gauge", -1, "Objects,Gauge,Simple", 0, false);
            RegisteredObjects.Add(typeof(RadialGauge), "ReportPage,Gauge", -1, "Objects,Gauge,Radial", 0, false);
            RegisteredObjects.Add(typeof(SimpleProgressGauge), "ReportPage,Gauge", -1, "Objects,Gauge,SimpleProgress", 0, false);

            //      RegisteredObjects.Add(typeof(CrossBandObject), "ReportPage", 11, "Cross-band line", 0);
            //      RegisteredObjects.Add(typeof(CrossBandObject), "ReportPage", 11, "Cross-band rectangle", 1);

            RegisteredObjects.Add(typeof(HtmlObject), "ReportPage", 246, 18);


            // exports

            RegisteredObjects.AddExport(typeof(ImageExport), "Image", "Export,Image,File");

            // functions
            RegisteredObjects.AddCategory("Functions", -1, "");
            StdFunctions.Register();


        }
    }
}

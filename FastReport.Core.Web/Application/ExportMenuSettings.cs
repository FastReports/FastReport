using System;

namespace FastReport.Web
{
    public class ExportMenuSettings
    {    
        private const Exports defaultExports =
            Exports.Prepared | Exports.Pdf | Exports.Excel2007 | Exports.Word2007 | Exports.Text | Exports.Rtf
            | Exports.Xps | Exports.Ods | Exports.Odt | Exports.XmlExcel | Exports.Csv;
     
        private const Exports allExports =
            Exports.Prepared | Exports.Pdf | Exports.Excel2007 | Exports.Word2007 | Exports.PowerPoint2007 | Exports.XmlExcel 
            | Exports.Text | Exports.Rtf | Exports.Xps | Exports.Ods | Exports.Odt | Exports.Dbf | Exports.Csv | Exports.Mht
            | Exports.HTML | Exports.Hpgl /*| Exports.Email*/ | Exports.Dxf | Exports.Json | Exports.LaTeX /*| Exports.Image*/
            | Exports.Ppml | Exports.PS | Exports.Xaml | Exports.Zpl | Exports.Excel97 | Exports.Svg;

        /// <summary>
        /// Show Exports menu
        /// </summary>
        public bool Show { get; set; } = true;
        
        /// <summary>
        /// Used to set exports in toolbar.
        /// </summary>
        public Exports ExportTypes { get; set; } = defaultExports;
        /// <summary>
        /// Get an instance of ExportMenuSettings with default exports 
        /// <para>Default : Prepared, Pdf, Excel2007, Word2007, Text, Rtf, Xps, Ods, Odt, XmlExcel, Csv</para>
        /// </summary>
        public static ExportMenuSettings Default => new ExportMenuSettings
        {
            ExportTypes = defaultExports
        };
        /// <summary>
        /// Get an instance of ExportMenuSettings with all exports 
        /// </summary>
        public static ExportMenuSettings All => new ExportMenuSettings
        {
            ExportTypes = allExports
        };

        /// <summary>
        /// Switch a visibility of prepared report export in toolbar
        /// </summary>
        public bool ShowPreparedReport
        {
            get => (ExportTypes & Exports.Prepared) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Prepared;
                else
                    ExportTypes &= ~Exports.Prepared;
            }
        }

#if !OPENSOURCE
        /// <summary>
        /// Switches a visibility of PDF (Adobe Acrobat) export in toolbar.
        /// </summary>
        public bool ShowPdfExport
        {
            get => (ExportTypes & Exports.Pdf) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Pdf;

                else
                    ExportTypes &= ~Exports.Pdf;
            }
        }

        /// <summary>
        /// Switches a visibility of Excel 2007 export in toolbar.
        /// </summary>
        public bool ShowExcel2007Export {
            get => (ExportTypes & Exports.Excel2007) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Excel2007;
                else
                    ExportTypes &= ~Exports.Excel2007;
            }
        }

        /// <summary>
        /// Switches a visibility of Word 2007 export in toolbar.
        /// </summary>
        public bool ShowWord2007Export {
            get => (ExportTypes & Exports.Word2007) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Word2007;
                else
                    ExportTypes &= ~Exports.Word2007;
            }
        }

        /// <summary>
        /// Switches a visibility of PowerPoint 2007 export in toolbar.
        /// </summary>
        public bool ShowPowerPoint2007Export {
            get => (ExportTypes & Exports.PowerPoint2007) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.PowerPoint2007;
                else
                    ExportTypes &= ~Exports.PowerPoint2007;
            }
        }

        /// <summary>
        /// Switch a visibility of text (plain text) export in toolbar
        /// </summary>
        public bool ShowTextExport {
            get => (ExportTypes & Exports.Text) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Text;
                else
                    ExportTypes &= ~Exports.Text;
            }
        }

        /// <summary>
        /// Switches a visibility of RTF export in toolbar.
        /// </summary>
        public bool ShowRtfExport {
            get => (ExportTypes & Exports.Rtf) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Rtf;
                else
                    ExportTypes &= ~Exports.Rtf;
            }
        }

        /// <summary>
        /// Switches a visibility of XPS export in toolbar.
        /// </summary>
        public bool ShowXpsExport {
            get => (ExportTypes & Exports.Xps) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Xps;
                else
                    ExportTypes &= ~Exports.Xps;
            }
        }

        /// <summary>
        /// Switches a visibility of Open Office Spreadsheet (ODS) export in toolbar.
        /// </summary>
        public bool ShowOdsExport {
            get => (ExportTypes & Exports.Ods) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Ods;
                else
                    ExportTypes &= ~Exports.Ods;
            }
        }

        /// <summary>
        /// Switches a visibility of Open Office Text (ODT) export in toolbar
        /// </summary>
        public bool ShowOdtExport {
            get => (ExportTypes & Exports.Odt) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Odt;
                else
                    ExportTypes &= ~Exports.Odt;
            }
        }

        /// <summary>
        /// Switches a visibility of XML (Excel) export in toolbar.
        /// </summary>
        public bool ShowXmlExcelExport {
            get => (ExportTypes & Exports.XmlExcel) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.XmlExcel;
                else
                    ExportTypes &= ~Exports.XmlExcel;
            }
        }

        /// <summary>
        /// Switches a visibility of DBF export in toolbar.
        /// </summary>
        public bool ShowDbfExport {
            get => (ExportTypes & Exports.Dbf) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Dbf;
                else
                    ExportTypes &= ~Exports.Dbf;
            }
        }

        /// <summary>
        /// Switches visibility the CSV (comma separated values) export in toolbar.
        /// </summary>
        public bool ShowCsvExport {
            get => (ExportTypes & Exports.Csv) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Csv;
                else
                    ExportTypes &= ~Exports.Csv;
            }
        }

        /// <summary>
        /// Switches visibility the MHT export in toolbar.
        /// </summary>
        public bool ShowMhtExport {
            get => (ExportTypes & Exports.Mht) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Mht;
                else
                    ExportTypes &= ~Exports.Mht;
            }
        }

        /// <summary>
        /// Switches visibility the HTML export in toolbar.
        /// </summary>
        public bool ShowHTMLExport
        {
            get => (ExportTypes & Exports.HTML) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.HTML;
                else
                    ExportTypes &= ~Exports.HTML;
            }
        }
        /// <summary>
        /// Switches visibility the HPGL export in toolbar.
        /// </summary>
        public bool ShowHpglExport
        {
            get => (ExportTypes & Exports.Hpgl) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Hpgl;
                else
                    ExportTypes &= ~Exports.Hpgl;
            }
        }
        //public bool ShowEmailExport
        //{
        //    get => (ExportTypes & Exports.Email) > 0;
        //    set
        //    {
        //        if (value)
        //            ExportTypes |= Exports.Email;
        //        else
        //            ExportTypes &= ~Exports.Email;
        //    }
        //}
        /// <summary>
        /// Switches visibility the DXF export in toolbar.
        /// </summary>
        public bool ShowDxfExport
        {
            get => (ExportTypes & Exports.Dxf) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Dxf;
                else
                    ExportTypes &= ~Exports.Dxf;
            }
        }
        /// <summary>
        /// Switches visibility the Json export in toolbar.
        /// </summary>
        public bool ShowJsonExport
        {
            get => (ExportTypes & Exports.Json) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Json;
                else
                    ExportTypes &= ~Exports.Json;
            }
        }
        /// <summary>
        /// Switches visibility the LaTeX export in toolbar.
        /// </summary>
        public bool ShowLaTeXExport
        {
            get => (ExportTypes & Exports.LaTeX) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.LaTeX;
                else
                    ExportTypes &= ~Exports.LaTeX;
            }
        }
        //public bool ShowImageExport
        //{
        //    get => (ExportTypes & Exports.Image) > 0;
        //    set
        //    {
        //        if (value)
        //            ExportTypes |= Exports.Image;
        //        else
        //            ExportTypes &= ~Exports.Image;
        //    }
        //}
        /// <summary>
        /// Switches visibility the PPML export in toolbar.
        /// </summary>
        public bool ShowPpmlExport
        {
            get => (ExportTypes & Exports.Ppml) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Ppml;
                else
                    ExportTypes &= ~Exports.Ppml;
            }
        }
        /// <summary>
        /// Switches visibility the PS export in toolbar.
        /// </summary>
        public bool ShowPSExport
        {
            get => (ExportTypes & Exports.PS) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.PS;
                else
                    ExportTypes &= ~Exports.PS;
            }
        }
        /// <summary>
        /// Switches visibility the XAML export in toolbar.
        /// </summary>
        public bool ShowXamlExport
        {
            get => (ExportTypes & Exports.Xaml) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Xaml;
                else
                    ExportTypes &= ~Exports.Xaml;
            }
        }
        /// <summary>
        /// Switches visibility the ZPL export in toolbar.
        /// </summary>
        public bool ShowZplExport
        {
            get => (ExportTypes & Exports.Zpl) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Zpl;
                else
                    ExportTypes &= ~Exports.Zpl;
            }
        }
        /// <summary>
        /// Switches visibility the Excel-97 export in toolbar.
        /// </summary>
        public bool ShowExcel97Export
        {
            get => (ExportTypes & Exports.Excel97) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Excel97;
                else
                    ExportTypes &= ~Exports.Excel97;
            }
        }
        /// <summary>
        /// Switches visibility the SVG export in toolbar.
        /// </summary>
        public bool ShowSvgExport
        {
            get => (ExportTypes & Exports.Svg) > 0;
            set
            {
                if (value)
                    ExportTypes |= Exports.Svg;
                else
                    ExportTypes &= ~Exports.Svg;
            }
        }
#endif
  
    }
    ///<summary>
    ///Used to select exports 
    /// </summary>
    [Flags]
    public enum Exports
    {
        Prepared = 1,
        Pdf = 2,
        Excel2007 = 4,
        Word2007 = 8,
        PowerPoint2007 = 16,
        XmlExcel = 32,
        Text = 64,
        Rtf = 128,
        Xps = 256,
        Ods = 512,
        Odt = 1024,
        Dbf = 2048,
        Csv = 4096,
        Mht = 8192,
        HTML = 16384,
        Hpgl = 32768,
        //Email = 65536,
        Dxf = 131_072,
        Json = 262_144,
        LaTeX = 524_288,
        //Image = 1_048_576,
        Ppml = 2_097_152,
        PS = 4_194_304,
        Xaml = 8_388_608,
        Zpl = 16_777_216,
        Excel97 = 33_554_432,
        Svg = 67_108_864,
    }
}
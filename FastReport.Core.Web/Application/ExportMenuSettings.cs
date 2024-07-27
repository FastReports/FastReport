using System;
using System.Drawing;

namespace FastReport.Web
{
    public class ExportMenuSettings
    {

        /// <summary>
        /// Show Exports menu
        /// </summary>
        public bool Show { get; set; } = true;

        /// <summary>
        /// Used to set exports in toolbar.
        /// </summary>
        public Exports ExportTypes { get; set; } = Exports.Default;
        /// <summary>
        /// Used to change font family, style in export settings.
        /// </summary>
        public Font FontSettings { get; set; } = null;
        /// <summary>
        /// Used to change font color in export settings.
        /// </summary>
        public Color FontColor { get; set; } = Color.White;
        /// <summary>
        /// Used to change window, buttons color in export settings.
        /// </summary>
        public Color Color { get; set; } = Color.Maroon;

        /// <summary>
        /// Used to on/off export settings.
        /// </summary>
        public bool EnableSettings { get; set; } = false;

        /// <summary>
        /// If enabled, the container with the settings will be fixed on the screen and will be in the foreground.
        /// </summary>
        /// <remarks>
        /// Default value: false
        /// </remarks>
        public bool PinnedSettingsPosition { get; set; } = false;
        internal string UserFontSettingsStyle
        {
            get
            {
                if (FontSettings != null)
                {
                    return  FontSettings.Style + " ";
                }
                else
                    return "";
            }

        }

        internal string FixedContainerPosition
        {
            get => PinnedSettingsPosition ? "position: fixed; top: 0; left: 0;" : "";
        }

        internal string FixedContainerTags
        {
            get => PinnedSettingsPosition ? "position : fixed; top: 40%; left: 40%; transform: translate(-50%, -50%);" : "";
        }

        internal string UserFontSettingsFamily
        {
            get
            {
                if (FontSettings != null)
                {
                    return " " + FontSettings.OriginalFontName;
                }
                else
                    return "Verdana,Arial";
              
            }
        }

        /// <summary>
        /// Get an instance of ExportMenuSettings with default exports 
        /// <para>Default : Prepared, Pdf, Excel2007, Word2007, Text, Rtf, Xps, Ods, Odt, XmlExcel, Csv</para>
        /// </summary>
        public static ExportMenuSettings Default => new ExportMenuSettings
        {
            ExportTypes = Exports.Default

        };

        /// <summary>
        /// Get an instance of ExportMenuSettings with all exports 
        /// </summary>
        public static ExportMenuSettings All => new ExportMenuSettings
        {
            ExportTypes = Exports.All

        };

      
        /// <summary>
        /// Switch a visibility of prepared report export in toolbar
        /// </summary>
        public bool ShowPreparedReport
        {
            get => GetExport(Exports.Prepared);
            set => SetExport(value, Exports.Prepared);
        }

#if !OPENSOURCE

        /// <summary>
        /// Switches a visibility of PDF (Adobe Acrobat) export in toolbar.
        /// </summary>
        public bool ShowPdfExport
        {
            get => GetExport(Exports.Pdf);
            set => SetExport(value, Exports.Pdf);
        }

        /// <summary>
        /// Switches a visibility of Excel 2007 export in toolbar.
        /// </summary>
        public bool ShowExcel2007Export {
            get => GetExport(Exports.Excel2007);
            set => SetExport(value, Exports.Excel2007);
        }

        /// <summary>
        /// Switches a visibility of Word 2007 export in toolbar.
        /// </summary>
        public bool ShowWord2007Export {
            get => GetExport(Exports.Word2007);
            set => SetExport(value, Exports.Word2007);
        }

        /// <summary>
        /// Switches a visibility of PowerPoint 2007 export in toolbar.
        /// </summary>
        public bool ShowPowerPoint2007Export {
            get => GetExport(Exports.PowerPoint2007);
            set => SetExport(value, Exports.PowerPoint2007);
        }

        /// <summary>
        /// Switch a visibility of text (plain text) export in toolbar
        /// </summary>
        public bool ShowTextExport {
            get => GetExport(Exports.Text);
            set => SetExport(value, Exports.Text);
        }

        /// <summary>
        /// Switches a visibility of RTF export in toolbar.
        /// </summary>
        public bool ShowRtfExport {
            get => GetExport(Exports.Rtf);
            set => SetExport(value, Exports.Rtf);
        }

        /// <summary>
        /// Switches a visibility of XPS export in toolbar.
        /// </summary>
        public bool ShowXpsExport {
            get => GetExport(Exports.Xps);
            set => SetExport(value, Exports.Xps);
        }

        /// <summary>
        /// Switches a visibility of Open Office Spreadsheet (ODS) export in toolbar.
        /// </summary>
        public bool ShowOdsExport {
            get => GetExport(Exports.Ods);
            set => SetExport(value, Exports.Ods);
        }

        /// <summary>
        /// Switches a visibility of Open Office Text (ODT) export in toolbar
        /// </summary>
        public bool ShowOdtExport {
            get => GetExport(Exports.Odt);
            set => SetExport(value, Exports.Odt);
        }

        /// <summary>
        /// Switches a visibility of XML (Excel) export in toolbar.
        /// </summary>
        public bool ShowXmlExcelExport {
            get => GetExport(Exports.XmlExcel);
            set => SetExport(value, Exports.XmlExcel);
        }

        /// <summary>
        /// Switches a visibility of DBF export in toolbar.
        /// </summary>
        public bool ShowDbfExport {
            get => GetExport(Exports.Dbf);
            set => SetExport(value, Exports.Dbf);
        }

        /// <summary>
        /// Switches visibility the CSV (comma separated values) export in toolbar.
        /// </summary>
        public bool ShowCsvExport {
            get => GetExport(Exports.Csv);
            set => SetExport(value, Exports.Csv);
        }

        /// <summary>
        /// Switches visibility the MHT export in toolbar.
        /// </summary>
        public bool ShowMhtExport {
            get => GetExport(Exports.Mht);
            set => SetExport(value, Exports.Mht);
        }

        /// <summary>
        /// Switches visibility the HTML export in toolbar.
        /// </summary>
        public bool ShowHTMLExport
        {
            get => GetExport(Exports.HTML);
            set => SetExport(value, Exports.HTML);
        }
        /// <summary>
        /// Switches visibility the HPGL export in toolbar.
        /// </summary>
        public bool ShowHpglExport
        {
            get => GetExport(Exports.Hpgl);
            set => SetExport(value, Exports.Hpgl);
        }

#if !WASM
        /// <summary>
        /// Switches visibility the Email export in toolbar.
        /// </summary>
        public bool ShowEmailExport
        {
            get => GetExport(Exports.Email);
            set
            {
                if (Infrastructure.FastReportGlobal.InternalEmailExportOptions is null)
                    throw new Exception("Please add your account information when registering for services. Use services.AddFastReport(options => options.EmailExportOptions = new EmailExportOptions() {...} )");

                SetExport(value, Exports.Email);
            }
        }
#endif

        /// <summary>
        /// Switches visibility the DXF export in toolbar.
        /// </summary>
        public bool ShowDxfExport
        {
            get => GetExport(Exports.Dxf);
            set => SetExport(value, Exports.Dxf);
        }
        /// <summary>
        /// Switches visibility the Json export in toolbar.
        /// </summary>
        public bool ShowJsonExport
        {
            get => GetExport(Exports.Json);
            set => SetExport(value, Exports.Json);
        }
        /// <summary>
        /// Switches visibility the LaTeX export in toolbar.
        /// </summary>
        public bool ShowLaTeXExport
        {
            get => GetExport(Exports.LaTeX);
            set => SetExport(value, Exports.LaTeX);
        }
        /// <summary>
        /// Switches a visibility of Image export in toolbar.
        /// </summary>
        public bool ShowImageExport
        {
            get => GetExport(Exports.Image);
            set => SetExport(value, Exports.Image);
        }
        /// <summary>
        /// Switches visibility the PPML export in toolbar.
        /// </summary>
        public bool ShowPpmlExport
        {
            get => GetExport(Exports.Ppml);
            set => SetExport(value, Exports.Ppml);
        }
        /// <summary>
        /// Switches visibility the PS export in toolbar.
        /// </summary>
        public bool ShowPSExport
        {
            get => GetExport(Exports.PS);
            set => SetExport(value, Exports.PS);
        }
        /// <summary>
        /// Switches visibility the XAML export in toolbar.
        /// </summary>
        public bool ShowXamlExport
        {
            get => GetExport(Exports.Xaml);
            set => SetExport(value, Exports.Xaml);
        }
        /// <summary>
        /// Switches visibility the ZPL export in toolbar.
        /// </summary>
        public bool ShowZplExport
        {
            get => GetExport(Exports.Zpl);
            set => SetExport(value, Exports.Zpl);
        }
        /// <summary>
        /// Switches visibility the Excel-97 export in toolbar.
        /// </summary>
        public bool ShowExcel97Export
        {
            get => GetExport(Exports.Excel97);
            set => SetExport(value, Exports.Excel97);
        }
        /// <summary>
        /// Switches visibility the SVG export in toolbar.
        /// </summary>
        public bool ShowSvgExport
        {
            get => GetExport(Exports.Svg);
            set => SetExport(value, Exports.Svg);
        }
#endif

        private bool GetExport(Exports export)
        {
            return (ExportTypes & export) > 0;
        }

        private void SetExport(bool value, Exports export)
        {
            if (value)
                ExportTypes |= export;
            else
                ExportTypes &= ~export;
        }

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
        Email = 65536,
        Dxf = 131_072,
        Json = 262_144,
        LaTeX = 524_288,
        Image = 1_048_576,
        Ppml = 2_097_152,
        PS = 4_194_304,
        Xaml = 8_388_608,
        Zpl = 16_777_216,
        Excel97 = 33_554_432,
        Svg = 67_108_864,

        Default = Prepared | Pdf | Excel2007 | Word2007 | Text | Rtf
            | Xps | Ods | Odt | XmlExcel | Csv,

        All = Default | PowerPoint2007 | Dbf | Mht
            | HTML | Hpgl /*| Email*/ | Dxf | Json | LaTeX | Image
            | Ppml | PS | Xaml | Zpl | Excel97 | Svg,
    }
}
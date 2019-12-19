using FastReport.Export.PdfSimple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace FastReport.Tests.OpenSource.Export.PdfSimple
{
    public class PdfSimpleExportTests
    {
        [Fact]
        public void ExportPdf()
        {
            Report r = new Report();
            r.LoadPrepared("TestReport.fpx");

            PDFSimpleExport export = new PDFSimpleExport();
            string pdf;

            using (MemoryStream ms = new MemoryStream())
            {
                r.Export(export, ms);
                pdf = Encoding.ASCII.GetString(ms.ToArray());
            }

#pragma warning disable xUnit2009 // Do not use boolean check to check for substrings
            Assert.True(pdf.StartsWith("%PDF-1.5"));
#pragma warning restore xUnit2009 // Do not use boolean check to check for substrings

            int i = 0;
            int index = 0;
            while( (index = pdf.IndexOf("/Page ", index + 1)) != -1)
            {
                i++;
            }
            Assert.Equal(4, i);

            i = 0;
            index = 0;

            while ((index = pdf.IndexOf("FEFF0046006100730074005200650070006F00720074002E004E00450054", index + 1)) != -1)
            {
                i++;
            }

            Assert.Equal(2, i);
        }

        [Fact]
        public void TestExportPdfInfo()
        {
            Report r = new Report();
            ReportPage page = new ReportPage();
            PageHeaderBand pageHeaderBand = new PageHeaderBand();
            pageHeaderBand.CreateUniqueName();
            pageHeaderBand.Height = 300;
            page.Bands.Add(pageHeaderBand);
            r.Pages.Add(page);
            r.Prepare();

            PDFSimpleExport export = new PDFSimpleExport();
            export.Title = "FastReport OpenSource Test Title dad5dd69-4c07-4789-ab4d-f03d0ba68c9c";
            export.Subject = "FastReport OpenSource Test Subject 7cf3d3d9-716f-4c51-a397-c6389c3100ca";
            export.Keywords = "FastReport OpenSource Test Keywors 2fbbf8b9-2daf-40b5-b216-a4c3130aac56";
            export.Author = "FastReport OpenSource Test Author a1e57c3e-1e0e-4b94-a472-07b5f05fa515";
            string pdf;

            using (MemoryStream ms = new MemoryStream())
            {
                r.Export(export, ms);
                pdf = Encoding.UTF8.GetString(ms.ToArray());
            }

#pragma warning disable xUnit2009 // Do not use boolean check to check for substrings
            Assert.True(pdf.Contains("/Title (" + StringToPdfUnicode(export.Title) + ")"));
            Assert.True(pdf.Contains("/Subject (" + StringToPdfUnicode(export.Subject) + ")"));
            Assert.True(pdf.Contains("/Keywords (" + StringToPdfUnicode(export.Keywords) + ")"));
            Assert.True(pdf.Contains("/Author (" + StringToPdfUnicode(export.Author) + ")"));
#pragma warning restore xUnit2009 // Do not use boolean check to check for substrings
        }

        [Fact]
        public void TestExportWatermark()
        {
            Report r = new Report();
            r.LoadPrepared("Watermark.fpx");

            PDFSimpleExport export = new PDFSimpleExport();
            r.Export(export, "Watermark.pdf");
        }

        [Fact]
        public void TestExportPdfImages()
        {
            

            PDFSimpleExport export = new PDFSimpleExport();

            export.ImageDpi = 300;
            export.JpegQuality = 90;
            Assert.Equal(300, export.ImageDpi);
            Assert.Equal(90, export.JpegQuality);


            export.ImageDpi = 1200;
            export.JpegQuality = 100;
            Assert.Equal(1200, export.ImageDpi);
            Assert.Equal(100, export.JpegQuality);

            export.ImageDpi = 96;
            export.JpegQuality = 10;
            Assert.Equal(96, export.ImageDpi);
            Assert.Equal(10, export.JpegQuality);

            export.ImageDpi = 300;
            export.JpegQuality = 90;
            Assert.Equal(300, export.ImageDpi);
            Assert.Equal(90, export.JpegQuality);

            export.ImageDpi = 3000;
            export.JpegQuality = 110;
            Assert.Equal(1200, export.ImageDpi);
            Assert.Equal(100, export.JpegQuality);

            export.ImageDpi = 0;
            export.JpegQuality = 0;
            Assert.Equal(96, export.ImageDpi);
            Assert.Equal(10, export.JpegQuality);
        }


        private string StringToPdfUnicode(string s)
        {
            StringBuilder sb = new StringBuilder();

            Append(sb, (char)254);
            Append(sb, (char)255);
            foreach (char c in s)
            {
                Append(sb, (char)(c >> 8));
                Append(sb, (char)(c & 0xFF));
            }
            return sb.ToString();
        }

        private void Append(StringBuilder sb, char c)
        {
            if (c < 127)
            {
                switch (c)
                {
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '(': sb.Append("\\("); break;
                    case ')': sb.Append("\\)"); break;
                    case '\\': sb.Append("\\\\"); break;
                    default: sb.Append(c); break;
                }
            }
            else
            {
                sb.Append("\\");
                sb.Append((int)c);
            }
        }
    }
}

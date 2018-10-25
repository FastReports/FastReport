using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace FastReport.Tests.Core.ReportObjectTests
{
    public class TextObjectTests
    {
        private Report report;
        ReportPage page;
        PageHeaderBand pageHeaderBand;
        DataBand dataBand;
        TextObject textObject;

        public TextObjectTests()
        {
            report = new Report();    
        }

        private void CreateTemplate()
        {
            // create report with bands
            report = new Report();
            page = new ReportPage();
            pageHeaderBand = new PageHeaderBand();
            pageHeaderBand.CreateUniqueName();
            pageHeaderBand.Width = 718;
            pageHeaderBand.Height = 300;
            dataBand = new DataBand();
            dataBand.CreateUniqueName();
            dataBand.Width = 718;
            dataBand.Height = 300;

            page.Bands.Add(pageHeaderBand);
            page.Bands.Add(dataBand);
            report.Pages.Add(page);
        }

        private void AddTextObject()
        {
            textObject = new TextObject();
            textObject.CreateUniqueName();
            textObject.Width = 100;
            textObject.Height = 100;
            textObject.Text = "Test";
            textObject.Left = 0;
            textObject.Top = 0;
            textObject.Border.Lines = BorderLines.All;

            dataBand.AddChild(textObject);
        }

        private DataBand GetDataBand() => 
            report.PreparedPages.GetPage(0).Bands.ToArray()
            .FirstOrDefault(b => b.GetType() == typeof(DataBand)) as DataBand;

        private TextObject GetTextObject(string name) =>
    report.PreparedPages.GetPage(0).FindObject(name) as TextObject;


        //Can't pass!!!
        //[Fact]
        //public void BandSquizingAnchorBotLeftTest()
        //{
        //    CreateTemplate();

        //    AddTextObject();
        //    textObject.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        //    //decrease dataBandSize
        //    dataBand.Height -= textObject.Height / 2;

        //    Assert.True(textObject.Top >= 0);
        //    //report.Save("BandSquizingAnchorBotLeftTest.frx");
        //}

        [Fact]
        public void AutoWidthTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "very very loooooooooooooooooooooooooooooooooooooooooooooooooooooong text!";
            textObject.Width = 1;
            textObject.Name = "AutoWidthTextObject";
            textObject.AutoWidth = true;

            report.Prepare();
            
            TextObject t = GetTextObject("AutoWidthTextObject");
            
            Assert.True(t.Width > 1);
        }

        [Fact]
        public void CanGrowTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "teeeeeeeeeeeeeeeeeeeeeeest" +
                "teeeeeeeeeeeeeeeeeeeeeeest" +
                "teeeeeeeeeeeeeeeeeeeeeeest" +
                "teeeeeeeeeeeeeeeeeeeeeeest" +
                "teeeeeeeeeeeeeeeeeeeeeeest" +
                "teeeeeeeeeeeeeeeeeeeeeeest" +
                "teeeeeeeeeeeeeeeeeeeeeeest";
            textObject.Width = 100;
            textObject.Height = 1;
            textObject.Name = "CanGrowTextObject";
            textObject.CanGrow = true;

            report.Prepare();
            TextObject t = GetTextObject("CanGrowTextObject");

            Assert.True(t.Height > 1);
        }

        [Fact]
        public void CanShrinkTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "teeeeeeeeeeeeeeeeeeeeeeest";
            textObject.Width = 100;
            textObject.Height = 1000;
            textObject.Name = "CanShrinkTextObject";
            textObject.CanShrink = true;

            report.Prepare();
            TextObject t = GetTextObject("CanShrinkTextObject");

            Assert.True(t.Height < 1000);
        }

        [Fact]
        public void GrowToBottomTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "teeeeeeeeeeeeeeeeeeeeeeest";
            textObject.Width = 100;
            textObject.Height = 1;
            textObject.Name = "GrowToBottomTextObject";
            textObject.GrowToBottom = true;

            report.Prepare();
            TextObject t = GetTextObject("GrowToBottomTextObject");

            DataBand d = GetDataBand();

            Assert.True(t.Top + t.Height == d.Top + d.Height);
        }

        [Fact]
        public void ShouldHideValueTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "[3-2]";
            textObject.Name = "HideValueTextObject";
            textObject.HideValue = "1";

            report.Prepare();
            TextObject t = GetTextObject("HideValueTextObject");

            Assert.True(t.Text == "");
        }

        [Fact]
        public void ShouldNotHideValueTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "[3-3]";
            textObject.Name = "HideValueTextObject";
            textObject.HideValue = "1";

            report.Prepare();
            TextObject t = GetTextObject("HideValueTextObject");

            Assert.True(t.Text != "");
        }

        [Fact]
        public void ShouldHideZerosTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "[3-3]";
            textObject.Name = "HideZerosTextObject";
            textObject.HideZeros = true;

            report.Prepare();
            TextObject t = GetTextObject("HideZerosTextObject");

            Assert.True(t.Text == "");
        }

        [Fact]
        public void ShouldNotHideZerosTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "[3-3]";
            textObject.Name = "HideZerosTextObject";
            textObject.HideZeros = false;

            report.Prepare();
            TextObject t = GetTextObject("HideZerosTextObject");

            Assert.True(t.Text != "");
        }

        [InlineData("[null]")]
        [InlineData("[5]")]
        [Theory]
        public void NullValueTest(string text)
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = text;
            textObject.Name = "NullValueTextObject";
            textObject.NullValue = "notnull";

            report.Prepare();
            TextObject t = GetTextObject("NullValueTextObject");

            if (text == "[null]")
                Assert.True(t.Text == "notnull");
            else
                Assert.True(t.Text != "notnull");
        }

        [Fact]
        public void AllowExpressionsTrueTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "[2+2*2]";
            textObject.Name = "AllowExpressionsTextObject";
            textObject.AllowExpressions = true;

            report.Prepare();
            TextObject t = GetTextObject("AllowExpressionsTextObject");

            Assert.True(t.Text == "6");
        }

        [Fact]
        public void AllowExpressionsFalseTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "[2+2*2]";
            textObject.Name = "AllowExpressionsTextObject";
            textObject.AllowExpressions = false;

            report.Prepare();
            TextObject t = GetTextObject("AllowExpressionsTextObject");

            Assert.True(t.Text == "[2+2*2]");
        }

        [Fact]
        public void CustomBracketsTest()
        {
            CreateTemplate();

            AddTextObject();
            textObject.Text = "{2+2*2}";
            textObject.Name = "BracketsTextObject";
            textObject.Brackets = "{,}";

            report.Prepare();
            TextObject t = GetTextObject("BracketsTextObject");

            Assert.True(t.Text == "6");
        }
    }
}

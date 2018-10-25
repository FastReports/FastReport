using System;
using Xunit;

namespace FastReport.Tests.Core
{
    /// <summary>
    /// Tests Base.cs methods
    /// </summary>
    public class BaseTests
    {
        private Report report;

        public BaseTests()
        {
            report = new Report();
        }

        private void SetFlags()
        {
            report.SetFlags(Flags.CanChangeOrder, true);
            report.SetFlags(Flags.CanChangeParent, true);
            report.SetFlags(Flags.CanCopy, true);
            report.SetFlags(Flags.CanDelete, false);
            report.SetFlags(Flags.CanDraw, false);
            report.SetFlags(Flags.CanEdit, false);
        }

        [Fact]
        public void FlagsTest()
        {
            report = new Report();

            SetFlags();
            bool hasCanChangeOrder = report.Flags.HasFlag(Flags.CanChangeOrder);
            bool hasCanChangeParent = report.Flags.HasFlag(Flags.CanChangeParent);
            bool hasCanCopy = report.Flags.HasFlag(Flags.CanCopy);
            bool hasCanDelete = report.Flags.HasFlag(Flags.CanDelete);
            bool hasCanDrawr = report.Flags.HasFlag(Flags.CanDraw);
            bool hasCanEdit = report.Flags.HasFlag(Flags.CanEdit);

            Assert.True(hasCanChangeOrder);
            Assert.True(hasCanChangeParent);
            Assert.True(hasCanCopy);
            Assert.True(!hasCanDelete);
            Assert.True(!hasCanDrawr);
            Assert.True(!hasCanEdit);
        }

        [Fact]
        public void SetReportTest()
        {
            report = new Report();

            Base myChild = new ReportPage();
            myChild.SetReport(report);

            Assert.Equal(myChild.Report, report);
        }

        [Theory]
        [InlineData("TestName")]
        [InlineData("1")]
        [InlineData(" _")]
        [InlineData(" ")]
        [InlineData(".")]
        public void SetNameTest(string name)
        {
            Base myChild = new ReportPage();
            myChild.SetName(name);

            Assert.Equal(myChild.Name, name);
        }

        [Fact]
        public void SetParentTest()
        {
            report = new Report();

            Base myChild = new ReportPage();
            myChild.SetParent(report);

            Assert.Equal(myChild.Parent, report);
        }

        [Fact]
        public void FindObjectTest()
        {
            report = new Report();

            Base page = new ReportPage();
            page.Name = "TestPage";
            report.AddChild(page);

            Base foundedPage = report.FindObject("TestPage");
            Assert.Equal(page, foundedPage);
        }

        [Fact]
        public void CreateUniqueNameTest()
        {
            report = new Report();

            Base page = new ReportPage();
            page.Name = "Page1";
            report.AddChild(page);

            page = new ReportPage();
            page.CreateUniqueName();
            report.AddChild(page);

            Assert.NotEqual(report.Pages[0].Name, report.Pages[1].Name);
        }

        [Fact]
        public void ClearTest()
        {
            report = new Report();

            Base page = new ReportPage();
            page.Name = "Page1";
            report.AddChild(page);

            page = new ReportPage();
            page.CreateUniqueName();
            report.AddChild(page);

            report.Clear();

            Base foundedPage = report.FindObject("Page1");

            Assert.True(foundedPage == null);
        }

        // Serializing
        // Deserializing

        [Fact]
        public void HasParentTest()
        {
            report = new Report();

            Base myChild = new ReportPage();
            myChild.SetParent(report);

            bool hasParentReport = myChild.HasParent(report);

            Assert.True(hasParentReport);
        }


        [Fact]
        public void HasFlagTest()
        {
            report = new Report();

            SetFlags();
            bool hasCanChangeOrder = report.Flags.HasFlag(Flags.CanChangeOrder);
            bool hasCanChangeParent = report.Flags.HasFlag(Flags.CanChangeParent);
            bool hasCanCopy = report.Flags.HasFlag(Flags.CanCopy);
            bool hasCanDelete = report.Flags.HasFlag(Flags.CanDelete);
            bool hasCanDraw = report.Flags.HasFlag(Flags.CanDraw);
            bool hasCanEdit = report.Flags.HasFlag(Flags.CanEdit);

            bool tHasCanChangeOrder = report.HasFlag(Flags.CanChangeOrder);
            bool tHasCanChangeParent = report.Flags.HasFlag(Flags.CanChangeParent);
            bool tHasCanCopy = report.HasFlag(Flags.CanCopy);
            bool tHasCanDelete = report.HasFlag(Flags.CanDelete);
            bool tHasCanDraw = report.HasFlag(Flags.CanDraw);
            bool tHasCanEdit = report.HasFlag(Flags.CanEdit);

            Assert.Equal(hasCanChangeOrder, tHasCanChangeOrder);
            Assert.Equal(hasCanChangeParent, tHasCanChangeParent);
            Assert.Equal(hasCanCopy, tHasCanCopy);
            Assert.Equal(hasCanDelete, tHasCanDelete);
            Assert.Equal(hasCanDraw, tHasCanDraw);
            Assert.Equal(hasCanEdit, tHasCanEdit);
        }

        [Fact]
        public void HasRestrictionTest()
        {
            report = new Report();

            report.Restrictions = Restrictions.DontDelete | Restrictions.DontResize;

            bool dontDelete = report.HasRestriction(Restrictions.DontDelete);
            bool dontResize = report.HasRestriction(Restrictions.DontResize);
            bool none = report.HasRestriction(Restrictions.None);

            Assert.True(dontDelete);
            Assert.True(dontResize);
            Assert.False(none);
        }

        [Theory]
        [InlineData("TestPage")]
        [InlineData("1")]
        [InlineData(" _")]
        [InlineData(" ")]
        [InlineData(".")]
        public void AddPageFindObjectTest(string pageName)
        {
            report = new Report();
            ReportPage page = new ReportPage();
            
            page.Name = pageName;
            report.AddChild(page);

            Base foundedPage = report.FindObject(pageName);
            if (foundedPage != null)
            {
                Assert.Equal(page, foundedPage);
            }
        }


    }
}

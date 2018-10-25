using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


namespace FastReport.Tests.Core
{
    public class TextObjectBaseTests
    {
        private TextObjectBase textObject;

        public TextObjectBaseTests()
        {
            textObject = new TextObjectBase();
        }


        [Theory]
        [InlineData("[,]")]
        [InlineData("(,)")]
        [InlineData("{,}")]
        [InlineData("(,]")]
        [InlineData("(,}")]
        public void GetTextWithBracketsTest(string brackets)
        {     
            textObject = new TextObjectBase();
            textObject.Brackets = brackets;
            string text = "testTest";

            string[] bracketsArr = textObject.Brackets.Split(new char[] { ',' });
            string expected = bracketsArr[0] + text + bracketsArr[1];

            string textWithBrackets = textObject.GetTextWithBrackets(text);

            Assert.Equal(expected, textWithBrackets);
        }

        [Fact]
        public void GetTextWithoutBracketsTest()
        {
            textObject = new TextObjectBase();
            textObject.Brackets = "[,]";
            string text = "[testTest]";

            string expected = "testTest";

            string textWithoutBrackets = textObject.GetTextWithoutBrackets(text);

            Assert.Equal(expected, textWithoutBrackets);
        }
    }
}

using FastReport.Data.JsonConnection;
using FastReport.Data.JsonConnection.JsonParser;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FastReport.Tests.OpenSource.Data
{

    public class JsonConnectionTests
    {

        [Fact]
        public void EmptyTextTest()
        {
            Assert.Throws<FormatException>(() => JsonBase.FromString(""));
        }

        [Theory]
        [InlineData("{}")]
        [InlineData(" { } ")]
        [InlineData(" {}")]
        [InlineData("{  \n} ")]
        public void EmptyObjectTest(string jsonText)
        {
            Assert.IsType<JsonObject>(JsonBase.FromString(jsonText));
        }

        [Theory]
        [InlineData("[]")]
        [InlineData(" [ ] ")]
        [InlineData(" []")]
        [InlineData("[  \n] ")]
        public void EmptyArrayTest(string jsonText)
        {
            Assert.IsType<JsonArray>(JsonBase.FromString(jsonText));
        }

        [Theory]
        [InlineData("[0.1]", 0.1)]
        [InlineData("[0.1e010]", 0.1e010)]
        [InlineData("[0.10]", 0.10)]
        [InlineData("[0.01e010]", 0.01e010)]
        [InlineData("[-0.1]", -0.1)]
        [InlineData("[0.1e-010]", 0.1e-010)]
        [InlineData("[51251.1]", 51251.1)]
        [InlineData("[0.1e+010]", 0.1e+010)]
        [InlineData("[-0010.1]", -0010.1)]
        [InlineData("[5e110]", 5e110)]
        [InlineData("[-1]", -1)]
        [InlineData("[-5]", -5)]
        [InlineData("[7]", 7)]
        [InlineData("[7312312]", 7312312)]
        [InlineData("[-12317]", -12317)]
        public void ReadNumberTest(string jsonText, double number)
        {
            JsonBase obj = JsonBase.FromString(jsonText);

            Assert.IsType<JsonArray>(obj);
            Assert.Single(obj as JsonArray);
            Assert.IsType<double>((obj as JsonArray)[0]);
            Assert.Equal(number, (double)(obj as JsonArray)[0], 1);
        }

        [Theory]
        [InlineData("{\"key\":0}", "key")]
        [InlineData("{\"\\uabcdkey\":0}", "\uabcdkey")]
        [InlineData("{\"\\uabCdkey\":0}", "\uabCdkey")]
        [InlineData("{\"\\u0001key\":0}", "\u0001key")]
        [InlineData("{\"\\u00Cdkey\":0}", "\u00Cdkey")]
        [InlineData("{\"\\uab00key\":0}", "\uab00key")]
        [InlineData("{\"\\r\":0}", "\r")]
        [InlineData("{\"\\t\":0}", "\t")]
        [InlineData("{\"\\n\\n\":0}", "\n\n")]
        [InlineData("{\"\\f\":0}", "\f")]
        [InlineData("{\"\\b\":0}", "\b")]
        [InlineData("{\"\\\\\":0}", "\\")]
        [InlineData("{\"\\/\":0}", "/")]
        [InlineData("{\"\\\"\":0}", "\"")]
        public void ReadKeyTest(string jsonText, string key)
        {
            JsonBase obj = JsonBase.FromString(jsonText);

            Assert.IsType<JsonObject>(obj);
            Assert.Single(obj as JsonObject);
            Assert.Contains(key, (obj as JsonObject).Keys);
        }

        [Theory]
        [InlineData("{  \n\t\"true\": true}", "true", true)]
        [InlineData("{\"true\":false }", "true", false)]
        public void ReadBool(string jsonText, string key, bool value)
        {
            JsonBase obj = JsonBase.FromString(jsonText);

            Assert.IsType<JsonObject>(obj);
            Assert.Single(obj as JsonObject);
            Assert.Contains(key, (obj as JsonObject).Keys);
            Assert.IsType<bool>((obj as JsonObject)[key]);
            Assert.Equal(value, (bool)(obj as JsonObject)[key]);
        }

        [Theory]
        [InlineData("{  \n\t\"true\": null}", "true")]
        [InlineData("{\"true\":null }", "true")]
        public void ReadNull(string jsonText, string key)
        {
            JsonBase obj = JsonBase.FromString(jsonText);

            Assert.IsType<JsonObject>(obj);
            Assert.Single(obj as JsonObject);
            Assert.Contains(key, (obj as JsonObject).Keys);
            Assert.Null((obj as JsonObject)[key]);
        }

        [Fact]
        public void TestComplex()
        {
            string jsonText = @"{
    ""glossary"": {
        ""title"": ""example glossary"",
		""GlossDiv"": {
            ""title"": ""S"",
			""GlossList"": {
                ""GlossEntry"": {
                    ""ID"": ""SGML"",
					""SortAs"": ""SGML"",
					""GlossTerm"": ""Standard Generalized Markup Language"",
					""Acronym"": ""SGML"",
					""Abbrev"": ""ISO 8879:1986"",
					""GlossDef"": {
                        ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
						""GlossSeeAlso"": [""GML"", ""XML""]
                    },
					""GlossSee"": ""markup""
                }
            }
        }
    }
}";

            JsonBase jsonBase = JsonBase.FromString(jsonText);

            Assert.IsType<JsonObject>(jsonBase);

            JsonObject obj = jsonBase as JsonObject;

            Assert.Single(obj);

            Assert.IsType<JsonObject>(obj["glossary"]);

            obj = obj["glossary"] as JsonObject;

            Assert.Equal(2, obj.Count);

            Assert.IsType<string>(obj["title"]);

            Assert.Equal("example glossary", obj["title"] as string);

            Assert.IsType<JsonObject>(obj["GlossDiv"]);
            obj = obj["GlossDiv"] as JsonObject;

            Assert.IsType<JsonObject>(obj["GlossList"]);
            obj = obj["GlossList"] as JsonObject;

            Assert.IsType<JsonObject>(obj["GlossEntry"]);
            obj = obj["GlossEntry"] as JsonObject;

            Assert.IsType<JsonObject>(obj["GlossDef"]);
            obj = obj["GlossDef"] as JsonObject;

            Assert.IsType<JsonArray>(obj["GlossSeeAlso"]);
        }
    }
}

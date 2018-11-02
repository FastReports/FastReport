// Copyright © 2010 Xamasoft
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE.txt for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.JsonClassGenerator
{
    public class FieldInfo
    {

        public FieldInfo(IJsonClassGeneratorConfig generator, string jsonMemberName, JsonType type, bool usePascalCase, IList<object> Examples)
        {
            this.generator = generator;
            this.JsonMemberName = jsonMemberName;
            this.MemberName = jsonMemberName;
            if (usePascalCase) MemberName = JsonClassGenerator.ToTitleCase(MemberName);
            this.Type = type;
            this.Examples = Examples;
        }
        private IJsonClassGeneratorConfig generator;
        public string MemberName { get; private set; }
        public string JsonMemberName { get; private set; }
        public JsonType Type { get; private set; }
        public IList<object> Examples { get; private set; }

        public string GetGenerationCode(string jobject)
        {
            var field = this;
            if (field.Type.Type == JsonTypeEnum.Array)
            {
                var innermost = field.Type.GetInnermostType();
                return string.Format("({1})JsonClassHelper.ReadArray<{5}>(JsonClassHelper.GetJToken<JArray>({0}, \"{2}\"), JsonClassHelper.{3}, typeof({6}))",
                    jobject,
                    field.Type.GetTypeName(),
                    field.JsonMemberName,
                    innermost.GetReaderName(),
                    -1,
                    innermost.GetTypeName(),
                    field.Type.GetTypeName()
                    );
            }
            else if (field.Type.Type == JsonTypeEnum.Dictionary)
            {

                return string.Format("({1})JsonClassHelper.ReadDictionary<{2}>(JsonClassHelper.GetJToken<JObject>({0}, \"{3}\"))",
                    jobject,
                    field.Type.GetTypeName(),
                    field.Type.InternalType.GetTypeName(),
                    field.JsonMemberName,
                    field.Type.GetTypeName()
                    );
            }
            else
            {
                return string.Format("JsonClassHelper.{1}(JsonClassHelper.GetJToken<{2}>({0}, \"{3}\"))",
                    jobject,
                    field.Type.GetReaderName(),
                    field.Type.GetJTokenType(),
                    field.JsonMemberName);
            }

        }



        public string GetExamplesText()
        {
            return string.Join(", ", Examples.Take(5).Select(x => JsonConvert.SerializeObject(x)).ToArray());
        }

    }
}

// Copyright © 2010 Xamasoft
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE.txt for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
#if !FRCORE
using System.Data.Entity.Design.PluralizationServices;
#endif
using System.Globalization;


namespace FastReport.JsonClassGenerator
{
    public class JsonClassGenerator : IJsonClassGeneratorConfig
    {


        public string Example { get; set; }
        public string TargetFolder { get; set; }
        public string Namespace { get; set; }
        public string SecondaryNamespace { get; set; }
        public bool UseProperties { get; set; }
        public bool InternalVisibility { get; set; }
        public bool ExplicitDeserialization { get; set; }
        public bool NoHelperClass { get; set; }
        public string MainClass { get; set; }
        public bool UsePascalCase { get; set; }
        public bool UseNestedClasses { get; set; }
        public bool ApplyObfuscationAttributes { get; set; }
        public bool SingleFile { get; set; }
        public ICodeWriter CodeWriter { get; set; }
        public TextWriter OutputStream { get; set; }
        public bool AlwaysUseNullableValues { get; set; }
        public bool ExamplesInDocumentation { get; set; }

#if !FRCORE
        private readonly PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-us"));
#endif

        private bool used = false;
        public bool UseNamespaces { get { return Namespace != null; } }

        public void GenerateClasses()
        {
            if (CodeWriter == null) CodeWriter = new CSharpCodeWriter();
            if (ExplicitDeserialization && !(CodeWriter is CSharpCodeWriter)) throw new ArgumentException("Explicit deserialization is obsolete and is only supported by the C# provider.");

            if (used) throw new InvalidOperationException("This instance of JsonClassGenerator has already been used. Please create a new instance.");
            used = true;


            var writeToDisk = TargetFolder != null;
            if (writeToDisk && !Directory.Exists(TargetFolder)) Directory.CreateDirectory(TargetFolder);


            JObject[] examples;
            var example = Example.StartsWith("HTTP/") ? Example.Substring(Example.IndexOf("\r\n\r\n")) : Example;
            using (var sr = new StringReader(example))
            using (var reader = new JsonTextReader(sr))
            {
                var json = JToken.ReadFrom(reader);
                if (json is JArray)
                {
                    examples = ((JArray)json).Cast<JObject>().ToArray();
                }
                else if (json is JObject)
                {
                    examples = new[] { (JObject)json };
                }
                else
                {
                    throw new Exception("Sample JSON must be either a JSON array, or a JSON object.");
                }
            }


            Types = new List<JsonType>();
            Names.Add(MainClass);
            var rootType = new JsonType(this, examples[0]);
            rootType.IsRoot = true;
            rootType.AssignName(MainClass);
            GenerateClass(examples, rootType);

            if (writeToDisk)
            {
                //var parentFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //if (writeToDisk && !NoHelperClass && ExplicitDeserialization)
                //    File.WriteAllBytes(Path.Combine(TargetFolder, "JsonClassHelper.cs"), Properties.Resources.JsonClassHelper);
                //if (SingleFile)
                //{
                //    WriteClassesToFile(Path.Combine(TargetFolder, MainClass + CodeWriter.FileExtension), Types);
                //}
                //else
                //{

                //    foreach (var type in Types)
                //    {
                //        var folder = TargetFolder;
                //        if (!UseNestedClasses && !type.IsRoot && SecondaryNamespace != null)
                //        {
                //            var s = SecondaryNamespace;
                //            if (s.StartsWith(Namespace + ".")) s = s.Substring(Namespace.Length + 1);
                //            folder = Path.Combine(folder, s);
                //            Directory.CreateDirectory(folder);
                //        }
                //        WriteClassesToFile(Path.Combine(folder, (UseNestedClasses && !type.IsRoot ? MainClass + "." : string.Empty) + type.AssignedName + CodeWriter.FileExtension), new[] { type });
                //    }
                //}
            }
            else if (OutputStream != null)
            {
                WriteClassesToFile(OutputStream, Types);
            }

        }

        private void WriteClassesToFile(string path, IEnumerable<JsonType> types)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                WriteClassesToFile(sw, types);
            }
        }

        private void WriteClassesToFile(TextWriter sw, IEnumerable<JsonType> types)
        {
            var inNamespace = false;
            var rootNamespace = false;

            CodeWriter.WriteFileStart(this, sw);
            foreach (var type in types)
            {
                if (UseNamespaces && inNamespace && rootNamespace != type.IsRoot && SecondaryNamespace != null) { CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace); inNamespace = false; }
                if (UseNamespaces && !inNamespace) { CodeWriter.WriteNamespaceStart(this, sw, type.IsRoot); inNamespace = true; rootNamespace = type.IsRoot; }
                CodeWriter.WriteClass(this, sw, type);
            }
            if (UseNamespaces && inNamespace) CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace);
            CodeWriter.WriteFileEnd(this, sw);
        }


        private void GenerateClass(JObject[] examples, JsonType type)
        {
            var jsonFields = new Dictionary<string, JsonType>();
            var fieldExamples = new Dictionary<string, IList<object>>();

            var first = true;

            foreach (var obj in examples)
            {
                foreach (var prop in obj.Properties())
                {
                    JsonType fieldType;
                    var currentType = new JsonType(this, prop.Value);
                    var propName = prop.Name;
                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {

                        var commonType = fieldType.GetCommonType(currentType);

                        jsonFields[propName] = commonType;
                    }
                    else
                    {
                        var commonType = currentType;
                        if (first) commonType = commonType.MaybeMakeNullable(this);
                        else commonType = commonType.GetCommonType(JsonType.GetNull(this));
                        jsonFields.Add(propName, commonType);
                        fieldExamples[propName] = new List<object>();
                    }
                    var fe = fieldExamples[propName];
                    var val = prop.Value;
                    if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
                    {
                        if (!fe.Contains(null))
                        {
                            fe.Insert(0, null);
                        }
                    }
                    else
                    {
                        var v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
                        if (!fe.Any(x => v.Equals(x)))
                            fe.Add(v);
                    }
                }
                first = false;
            }

            if (UseNestedClasses)
            {
                foreach (var field in jsonFields)
                {
                    Names.Add(field.Key.ToLower());
                }
            }

            foreach (var field in jsonFields)
            {
                var fieldType = field.Value;
                if (fieldType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                subexamples.Add((JObject)value);
                            }
                        }
                    }

                    fieldType.AssignName(CreateUniqueClassName(field.Key));
                    GenerateClass(subexamples.ToArray(), fieldType);
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                foreach (var item in (JArray)value)
                                {
                                    if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                    subexamples.Add((JObject)item);
                                }

                            }
                            else if (value.Type == JTokenType.Object)
                            {
                                foreach (var item in (JObject)value)
                                {
                                    if (!(item.Value is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");

                                    subexamples.Add((JObject)item.Value);
                                }
                            }
                        }
                    }

                    field.Value.InternalType.AssignName(CreateUniqueClassNameFromPlural(field.Key));
                    GenerateClass(subexamples.ToArray(), field.Value.InternalType);
                }
            }

            type.Fields = jsonFields.Select(x => new FieldInfo(this, x.Key, x.Value, UsePascalCase, fieldExamples[x.Key])).ToArray();

            Types.Add(type);

        }

        public IList<JsonType> Types { get; private set; }
        private HashSet<string> Names = new HashSet<string>();

        private string CreateUniqueClassName(string name)
        {
            name = ToTitleCase(name);

            var finalName = name;
            var i = 2;
            while (Names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = name + i.ToString();
                i++;
            }

            Names.Add(finalName);
            return finalName;
        }

        private string CreateUniqueClassNameFromPlural(string plural)
        {
            plural = ToTitleCase(plural);
            string name;
#if !FRCORE
            name = pluralizationService.Singularize(plural);
#else
            name = plural.Last() == 's' ? plural.Remove(plural.Length - 1) : plural;
#endif
            return CreateUniqueClassName(name);
        }



        internal static string ToTitleCase(string str)
        {
            var sb = new StringBuilder(str.Length);
            var flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(flag ? char.ToUpper(c) : c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }

        public bool HasSecondaryClasses
        {
            get { return Types.Count > 1; }
        }

        public static readonly string[] FileHeader = new[] { 
            "Generated by Xamasoft JSON Class Generator", 
            "http://www.xamasoft.com/json-class-generator"
        };
    }
}

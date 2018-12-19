using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace FastReport.Json
{
    /*
        HOW TO USE:
     
        var type = JsonCompiler.Compile(json);
        var properties = type.GetProperties();
        var obj = JsonConvert.DeserializeObject(json, type);

        Report report = new Report();
        report.Load(@"C:\report.frx");
     
        foreach (var prop in properties)
        {
            report.RegisterData((IList)prop.GetValue(obj, null), prop.Name);
        }
    */
    public class JsonCompiler
    {
        public static Type Compile(string json)
        {
            JsonClassGenerator.JsonClassGenerator gen = new JsonClassGenerator.JsonClassGenerator();
            gen.Example = json;
            gen.UseProperties = true;
            gen.Namespace = "__JSON__";
            gen.MainClass = "__JSON__";
            gen.UsePascalCase = true;

            string source = "";
            using (StringWriter sw = new StringWriter())
            {
                gen.OutputStream = sw;
                gen.GenerateClasses();
                sw.Flush();
                source = sw.ToString();
            }

            string nLocation;
            string nPath;

            try
            {
                nLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
                nPath = Path.Combine(nLocation, "Newtonsoft.Json.dll");
            }
            catch
            {
                nLocation = Environment.CurrentDirectory;
                nPath = Path.Combine(nLocation, "Newtonsoft.Json.dll");
            }

            Type type = Utils.CompileHelper.GenerateAssemblyInMemory(source, nPath).GetType("__JSON__.__JSON__");
            return type;
            
        }
    }
}

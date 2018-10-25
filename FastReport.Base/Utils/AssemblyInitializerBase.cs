using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Utils
{
  /// <summary>
  /// Base class for plugin's assembly initializer.
  /// </summary>
  /// <remarks>
  /// FastReport has an open architecture. That means you can extend it with own classes
  /// such as report objects, wizards, export filters. Usually such classes are
  /// placed in separate dlls (plugins). FastReport has mechanism to load plugin dlls. You can specify 
  /// which plugins to load at first start, in the FastReport configuration file (by default it is located in the
  /// C:\Documents and Settings\User_Name\Local Settings\Application Data\FastReport\FastReport.config file).
  /// To do this, add an xml item with your plugin name inside the &lt;Plugins&gt; item:
  /// <code>
  /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
  /// &lt;Config&gt;
  ///   &lt;Plugins&gt;
  ///     &lt;Plugin Name="c:\Program Files\MyProgram\MyPlugin.dll"/&gt;
  ///   &lt;/Plugins&gt;
  /// &lt;/Config&gt;
  /// </code>
  /// When you run your application and use the <b>Report</b> object first time, all plugins will be loaded. 
  /// To register objects contained in a plugin, FastReport searches for classes of type 
  /// <b>AssemblyInitializerBase</b> and instantiates them.
  /// <para>Use this class to register custom report objects, controls, wizards, exports that
  /// are contained in the assembly. To do this, make your own class of the <b>AssemblyInitializerBase</b>
  /// type and override its default constructor. In the constructor, call <b>RegisteredObjects.Add</b>
  /// methods to register all necessary items.</para>
  /// </remarks>
  public class AssemblyInitializerBase
  {
    /// <summary>
    /// Registers plugins contained in this assembly.
    /// </summary>
    /// <remarks>
    /// This constructor is called automatically when the assembly is loaded.
    /// </remarks>
    /// <example>This example show how to create own assembly initializer to register own items.
    /// <code>
    /// public class MyAssemblyInitializer : AssemblyInitializerBase
    /// {
    ///   public MyAssemblyInitializer()
    ///   {
    ///     // register own wizard
    ///     RegisteredObjects.AddWizard(typeof(MyWizard), myWizBmp, "My Wizard", true);
    ///     // register own export filter
    ///     RegisteredObjects.AddExport(typeof(MyExport), "My Export");
    ///     // register own report object
    ///     RegisteredObjects.Add(typeof(MyObject), "ReportPage", myObjBmp, "My Object");
    ///   }
    /// }
    /// </code>
    /// </example>
    public AssemblyInitializerBase()
    {
    }
  }
}

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FastReport.OpenSource.MVC
{
    public class Program
    {
        static Process xvfb;
        const string xvfb_pid = "pid.xvfb.fr";
        public static void Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                LinuxStart();
            BuildWebHost(args).Run();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                LinuxStop();
        }

        private static void LinuxStop()
        {
            xvfb.Kill();
            if (File.Exists(xvfb_pid))
                File.Delete(xvfb_pid);
        }

        public static void LinuxStart()
        {
            if (File.Exists(xvfb_pid))
            {
                string pid = File.ReadAllText(xvfb_pid);
                try
                {
                    xvfb = Process.GetProcessById(int.Parse(pid));
                    xvfb.Kill();
                    xvfb = null;
                }
                catch {  }
                File.Delete(xvfb_pid);
            }
            string display = Environment.GetEnvironmentVariable("DISPLAY");
            if (String.IsNullOrEmpty(display))
            {
                Environment.SetEnvironmentVariable("DISPLAY", ":99");
                display = ":99";
            }
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "/usr/bin/Xvfb";
            info.Arguments = display + " -ac -screen 0 1024x768x16 +extension RANDR -dpi 96";
            info.CreateNoWindow = true;
            xvfb = new Process();
            xvfb.StartInfo = info;
            xvfb.Start();
            File.WriteAllText(xvfb_pid, xvfb.Id.ToString());
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://[::]:5000")
                .Build();
    }
}

using ManualMapInjection.Injection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Loader
{
    class Program
    {
        public static String GetProcessorId()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String Id = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                Id = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return Id;
        }
        static void Main(string[] args)
        {
            File.WriteAllText("Hwid.inf", GetProcessorId());
            WebClient OHS = new WebClient();
            string compare = OHS.DownloadString("URL");
            if(compare.Contains(GetProcessorId()))
            {
                try
                {
                    string Dll = "Directory";
                    OHS.DownloadFile("URL", Dll);
                    var name = "ProcessName";
                    var target = Process.GetProcessesByName(name).FirstOrDefault();
                    var injector = new ManualMapInjector(target) { AsyncInjection = true };
                    var oi = ($"hmodule = 0x{injector.Inject(Dll).ToInt32():x8}");
                    File.Delete(Dll);
                }
               catch
               {
               }
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RetroStationUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Replace Files...");
            replaceOldFiles(Environment.CurrentDirectory  + "\\tmp");
            Console.WriteLine("Cleaning Up...");
            cleanUp(Environment.CurrentDirectory + "\\tmp");
            Console.WriteLine("Done.");
            System.Diagnostics.Process p = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "RetroStation.exe"
                }
            };
            p.Start();
        }

        private static void downloadUpdate()
        {
            string versionArchiveURL = @"https://raw.githubusercontent.com/KieranMcCool/RetroStation/master/Versions/RetroStationLatest.zip";
            using (var wc = new System.Net.WebClient())
                wc.DownloadFile(new Uri(versionArchiveURL), @"DL.zip");
        }


        private static void replaceOldFiles(string dir)
        {
            foreach (string d in Directory.GetDirectories(dir))
            {
                foreach (string f in Directory.GetFiles(d))
                    File.Copy(f, f.Replace(@"\tmp", ""), true);
                replaceOldFiles(d);
            }
        }
            
        private static void cleanUp(string dir)
        {
            Action<string> DeleteFiles = new Action<string>((s) => 
                { foreach(string f in Directory.GetFiles(s)) File.Delete(f); });
            string de = "";
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    de = d;
                    DeleteFiles(d);
                    Directory.Delete(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(de);
                Console.WriteLine(excpt.Message);
            }
        }
    }
}
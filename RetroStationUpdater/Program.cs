using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;


namespace RetroStationUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Downloading Update...");
            downloadUpdate();
            Console.WriteLine("Downloading Complete.");
            Console.WriteLine("Exctracting...");
            ExtractZipFile(Environment.CurrentDirectory + @"\DL.zip", "tmp");
            Console.WriteLine("Overwriting Old Version....");
            replaceOldFiles();
            Console.WriteLine("Cleaning Up...");
            cleanUp();
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


        private static void replaceOldFiles()
        {
            foreach (string s in Directory.GetFiles("tmp"))
            {
                FileInfo fi = new FileInfo(s);
                File.Copy(s, fi.Name, true);
            } 
        }

        private static void cleanUp()
        {
            File.Delete("DL.zip");
            Directory.Delete("\tmp");
        }

        private static void ExtractZipFile(string archiveFilenameIn, string outFolder)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;
                    }
                    String entryFileName = zipEntry.Name;

                    byte[] buffer = new byte[4096];
                    Stream zipStream = zf.GetInputStream(zipEntry);
                    
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

    }
}
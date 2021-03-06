//The MIT License (MIT)
//
//Copyright (c) 2015 Kieran McCool

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RetroStation
{
    static class Program
    { 
        public static System.Diagnostics.Process Play;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (DataManagement.loadAll())
                Application.Run(new frmMain());
        }

        public static bool updateToDate()
        {
            var wc = new System.Net.WebClient();
            string version;
            string onlineVersion;

            string versionInfoURL = @"https://raw.githubusercontent.com/KieranMcCool/RetroStation/master/Versions/RetroStationLatest/Resources/BuildDate.txt";
            using (var sr = new StreamReader(Environment.CurrentDirectory + @"\Resources\BuildDate.txt"))
            { version = sr.ReadToEnd().Trim();
                sr.Close(); sr.Dispose(); }
            try
            {
                onlineVersion = wc.DownloadString(new Uri(versionInfoURL)).Trim();
            }
            catch { onlineVersion = version; }
            return DateTime.Parse(onlineVersion) < DateTime.Parse(version);
        }
    }
}

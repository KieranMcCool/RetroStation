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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace RetroStation
{
    static class DataManagement
    {
        public static List<Platform> Platforms = new List<Platform>();
        public static List<Game> Games = new List<Game>();

        public static string platInfoDir = "Resources\\PlatformInfo.csv";

        public static bool loadAll()
        {
            if (platformsCheck())
            {
                loadSupported();
                loadGames();
                return true;
            }
            MessageBox.Show("Failed to create platforms, terminating...");
            return false;
        }

        public static void loadSupported()
        {
            if (File.Exists(platInfoDir))
            {
                Platforms.Clear();
                using (StreamReader sr = new StreamReader(platInfoDir))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        Platforms.Add(new Platform(line.Split(',').ToList<string>()));
                    }
                }
                Platforms = Platforms.OrderBy(x => x.getFriendlyName()).ToList();
            }
        }

        public static void loadGames()
        {
            if (Directory.Exists("ROMS"))
            {
                Games.Clear();
                foreach (string d in Directory.GetDirectories("ROMS"))
                    Games.Add(new Game(d));
            }
        }

        public static string[] friendlyPlatName(List<Platform> p)
        {
            var arr = new string[p.Count];
            int i = 0;
            foreach (var v in p)
            {
                arr[i] = v.getFriendlyName();
                i++;
            }
            return arr;
        }

        public static string getPlatform(string fileName)
        {
            var fi = new FileInfo(fileName);
            var platforms = DataManagement.Platforms.FindAll(x => x.getFileExtension().Contains(
                fi.Extension.Replace(".", "").ToLower()));
            frmPlatformChoose dialog;
            if (platforms.Count > 0)
                if (platforms.Count == 1)
                    return platforms[0].getFriendlyName();
                else
                {
                    dialog = new frmPlatformChoose(friendlyPlatName(platforms), fi.Name);
                    if (dialog.ShowDialog() == DialogResult.OK)
                        return platforms[dialog.result].getFriendlyName();
                }
            if (MessageBox.Show(string.Format("No Platform found for file {0}\nDo you want to select one?",
                fileName), "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dialog = new frmPlatformChoose(friendlyPlatName(Platforms), fi.Name);
                if (dialog.ShowDialog() == DialogResult.OK)
                    return Platforms[dialog.result].getFriendlyName();
            }
            return null;
        }

        public static void removePlatform(int p)
        {
            Platforms.RemoveAt(p);
            File.Delete(platInfoDir);
            foreach (var v in Platforms)
                addPlatform(v, false);
            loadSupported();
        }

        public static void addPlatform(Platform p, bool reload = true)
        {
            using (var sw = new StreamWriter(platInfoDir, true))
            {
                sw.Write(p.getCSVLine() + "\n");
            }
            if (reload)
                loadSupported();
        }

        private static bool platformsCheck()
        {
            if (!File.Exists(platInfoDir))
            {
                MessageBox.Show("No Platforms found.\nPlatform must be created to proceed.");
                frmPlatManager addP = new frmPlatManager();
                addP.ShowDialog();
                if (Platforms.Count == 0)
                    return false;
            }
            return true;
        }
    }
}

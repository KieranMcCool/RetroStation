﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmulationStation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public static List<Platform> Platforms = new List<Platform>();
        public static List<Game> Games = new List<Game>();
        public static System.Diagnostics.Process Play;

        [STAThread]
        static void Main()
        {
            loadSupported();
            loadGames();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        public static string platInfoDir = "Resources\\PlatformInfo.csv";

        public static void loadSupported(string path = "")
        {
            if (path == "")
                path = platInfoDir;
            if (File.Exists(path))
            {
                Platforms.Clear();
                using (StreamReader sr = new StreamReader(path))
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

        private static string[] friendlyPlatName(List<Platform> p)
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
            var platforms = Program.Platforms.FindAll(x => x.getFileExtension().Contains(
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
            if(MessageBox.Show(string.Format("No Platform found for file {0}\nDo you want to select one?",
                fileName), "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dialog = new frmPlatformChoose(friendlyPlatName(Platforms), fi.Name);
                if(dialog.ShowDialog() == DialogResult.OK)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EmulationStation
{
    public class Game
    {
        public Game(string _path)
        {
            if (_path != "**add:ing**")
            { 
                path = _path;
                getInfo(_path);
            }
            else
            {
                added();
            }
        }

        string friendlyName;
        Platform platform;
        long secPlayed;
        DateTime lastPlayed;
        string path;
        string romPath;

        public void added()
        {
            var dialog = new frmAddGameDia();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                addArray(dialog.results);
        }

        public void addArray(string[] array)
        {
            Directory.CreateDirectory("ROMS/" + array[1]);
            path = "ROMS/" + array[1] + "/";
            if (array[3] == "True")
                File.Move(array[0], path + new FileInfo(array[0]).Name);
            else
                File.Copy(array[0], path + new FileInfo(array[0]).Name);

            var s = new string[] { array[1], path + new FileInfo(array[0]).Name,
                    array[2], "0", DateTime.Now.Ticks.ToString()};

            outputFile(path, s);
        }

        private void getInfo(string _path)
        {
            if (File.Exists(path + "/info.dat"))
            {
                using (var sr = new StreamReader(_path + "/info.dat"))
                {
                    friendlyName = sr.ReadLine();
                    romPath = sr.ReadLine();
                    string _platform = sr.ReadLine();
                    platform = Program.Platforms.Find(x => x.getFriendlyName() == _platform);
                    secPlayed = long.Parse(sr.ReadLine());
                    lastPlayed = new DateTime(long.Parse(
                        sr.ReadLine()));
                }
            }
            else return;
        }

        private void outputFile(string _path, string[] writing = null)
        {
            using (var sr = new StreamWriter(_path + "/info.dat", false))
            {
                if (writing == null)
                {
                    sr.WriteLine(friendlyName);
                    sr.WriteLine(romPath);
                    sr.WriteLine(platform.getFriendlyName());
                    sr.WriteLine(secPlayed);
                    sr.WriteLine(lastPlayed.Ticks);
                }
                else
                {
                    foreach (var s in writing)
                        sr.WriteLine(s);
                }
            }
            getInfo(_path);
        }

        public void launch()
        {
                platform.launchGame(this);
        }

        public string getFriendlyName()
        {
            return friendlyName;
        }

        public Platform getPlatform()
        {
            return platform;
        }

        public DateTime getLastPlayed()
        {
            return lastPlayed;
        }

        public long getSecondsPlayed()
        {
            return secPlayed;
        }

        public string getRomPath()
        {
            return romPath;
        }

        public void registerTime(DateTime time,int s)
        {
            lastPlayed = time;
            secPlayed += s;
            outputFile(path);
        }
    }
}

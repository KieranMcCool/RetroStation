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

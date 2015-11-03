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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroStation
{
    public class Platform
    {
        public Platform(List<string> Info)
        {
            friendlyName = Info[0];
            emulatorPath =  Info[1].Replace(Environment.CurrentDirectory, "");
            commandTemplate = Info[2];
            fileExtension = Info.GetRange(3, Info.Count - 3).ToArray();
        }

        private string friendlyName;
        private string emulatorPath;
        private string commandTemplate;
        private string[] fileExtension;

        public string getFriendlyName()
        {
            return friendlyName;
        }

        public string getEmulatorPath()
        {
            return emulatorPath;
        }

        public string getCommandTemplate()
        {
            return commandTemplate;
        }

        public string[] getFileExtension()
        {
            return fileExtension;
        }

        public string getCSVLine()
        {
            return string.Format("{0},{1},{2},{3}",
                getFriendlyName(), getEmulatorPath(), getCommandTemplate(),
                string.Join(",", getFileExtension()));
        }

        public void launchGame(Game Game)
        {
            var game = new FileInfo(Game.getRomPath());
            string command = string.Format(this.getCommandTemplate(), game.FullName);

            Program.Play = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = command,
                }
            };

            string emuPath;

            if (File.Exists(Environment.CurrentDirectory + this.getEmulatorPath()))
                emuPath = Environment.CurrentDirectory + this.getEmulatorPath();
            else
                emuPath = this.getEmulatorPath();

            Program.Play.StartInfo.FileName = emuPath;
            Program.Play.StartInfo.WorkingDirectory = new FileInfo(emuPath).Directory.FullName;

            Program.Play.Start();
            var startTime = DateTime.Now;
            Program.Play.WaitForExit();
            Game.registerTime(DateTime.Now, (int)DateTime.Now.Subtract(startTime).TotalSeconds);
        }
    }
}
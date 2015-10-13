using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulationStation
{
    class Platform
    {
        public Platform(List<string> Info)
        {
            friendlyName = Info[0];
            emulatorPath = Environment.CurrentDirectory + Info[1];
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
                    FileName = this.getEmulatorPath(),
                    Arguments = command,
                    WorkingDirectory = new FileInfo(this.getEmulatorPath()).Directory.FullName
                }
            };
            Program.Play.Start();
            var startTime = DateTime.Now;
            Program.Play.WaitForExit();
            Game.registerTime(DateTime.Now, (int)DateTime.Now.Subtract(startTime).TotalSeconds);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EmulationStation
{
    public partial class frmMain : Form
    {
        ControllerInput ci = new ControllerInput();

        public frmMain()
        {
            InitializeComponent();
            reload();

            this.WindowState = FormWindowState.Maximized;

            ci.ButtonPressed += Ci_ButtonPressed;

            btnPlay.Click += (sender, e) =>
            {
                int sel = lbGames.SelectedIndex; var p = cbPlatform.SelectedItem; playing = true;
                Program.Games.Find(x => x.getFriendlyName() == (string)lbGames.SelectedItem).launch();
                reload(); lbGames.SelectedIndex = sel; cbPlatform.SelectedItem = p; playing = false;
                playing = false;
            };

            bulkFromDirectoryToolStripMenuItem.Click += BulkFromDirectoryToolStripMenuItem_Click;
            fromFileToolStripMenuItem.Click += (sender, e) => Program.Games.Add(new Game("**add:ing**"));
            managePlatformsToolStripMenuItem.Click += (sender, e) => {
                frmPlatManager fp = new frmPlatManager();
                fp.ShowDialog();  reload(); };
        }

        private void Ci_ButtonPressed(string[] actions, string[] buttonsPressed)
        {
            var cycleSelected = new Action<object, int>((s, i) => {
                try
                {
                    var contextS = ((ListBox)s);
                    contextS.BeginInvoke(new MethodInvoker(() =>
                    contextS.SelectedIndex = pyMod(contextS.SelectedIndex + i, contextS.Items.Count)));
                }
                catch (InvalidCastException e)
                {
                    var contextS = (ComboBox)s;
                    contextS.BeginInvoke(new MethodInvoker(() =>
                    contextS.SelectedIndex = pyMod(contextS.SelectedIndex + i, contextS.Items.Count)));
                    Console.WriteLine(e.Message);
                }
                catch { }
            });
            if (!playing)
            {
                foreach (var action in actions)
                    switch (action)
                    {
                        case "UP":
                            cycleSelected(lbGames, -1);
                            break;
                        case "DOWN":
                            cycleSelected(lbGames, 1);
                            break;
                        case "LEFT":
                            cycleSelected(cbPlatform, -1);
                            break;
                        case "RIGHT":
                            cycleSelected(cbPlatform, 1);
                            break;
                        case "FORWARD":
                            this.BeginInvoke(new MethodInvoker(() =>
                        { btnPlay.PerformClick(); }));
                            break;
                    }
            }
            else if (actions.Contains("QUIT"))
                Program.Play.Kill();
        }
    
        bool playing = false;

        private int pyMod(int a, int b)
        {
            return ((a % b) + b) % b;
        }

        private void reload()
        {
            Program.loadSupported();
            Program.loadGames();

            lbGames.Items.Clear();
            cbPlatform.Items.Clear();

            cbPlatform.Items.Add("All");

            foreach (var p in Program.Platforms)
            {
                cbPlatform.Items.Add(p.getFriendlyName());
            }

            cbPlatform.SelectedIndex = 0;
            if (lbGames.Items.Count != 0)
                lbGames.SelectedIndex = 0;
            filterBy(null);
        }

        private void BulkFromDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool complete = false;

            var thread = Task.Factory.StartNew(() =>
            {
                var wait = new frmCopying();
                wait.Show();
                while (!complete)
                    System.Threading.Thread.Sleep(100);
                wait.Close();
                wait.Dispose();
            });

            bool move = false;
            if (MessageBox.Show("Do you want to delete the original files?",
                "Import Options", MessageBoxButtons.YesNo) == DialogResult.Yes)
                move = true;
            var dia = new FolderBrowserDialog();
            string dir = "";
            if (dia.ShowDialog() == DialogResult.OK)
                dir = dia.SelectedPath;
            foreach (string s in Directory.GetFiles(dir, "*"))
            {
                var fi = new FileInfo(s);
                var g = new Game("**");
                string platform = Program.getPlatform(s);
                if (platform != null)
                {
                    g.addArray(new string[]
                    {
                    s, fi.Name.Replace(fi.Extension, "").Replace(".", ""),
                    platform, move.ToString()
                    });
                }
            }

            complete = true;
            thread.Wait();
            reload();
        }

        private void filterBy(Platform fType)
        {
            lbGames.Items.Clear();
            if (fType == null)
            {
                foreach (var v in Program.Games)
                    lbGames.Items.Add(v.getFriendlyName());
            }
            else
            {
                foreach (var v in Program.Games)
                    if (v.getPlatform() == fType)
                        lbGames.Items.Add(v.getFriendlyName());
            }
        }

        private void cbPlatform_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbPlatform.SelectedIndex)
            {
                case 0:
                    filterBy(null);
                    break;
                default:
                    var filterType = Program.Platforms.Find(x => x.getFriendlyName() ==
                        (string)cbPlatform.Items[cbPlatform.SelectedIndex]);
                    filterBy(filterType);
                    break;
            }
        }

        private void lbGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            var game = Program.Games.Find(x => x.getFriendlyName() == (string)lbGames.SelectedItem);

            lblGameName.Text = game.getFriendlyName();
            label2.Text = FormatTime(new TimeSpan(0, 0, (int)game.getSecondsPlayed()),
                game.getLastPlayed());
        }

        private string FormatTime(TimeSpan d, DateTime LastPlayed)
        {
            if (d.TotalHours > 1)
                return ((int)d.TotalHours).ToString() + " Hours | " + LastPlayed.ToShortDateString();
            else if (d.TotalMinutes > 1)
                return ((int)d.TotalMinutes).ToString() + " Minutes | " + LastPlayed.ToShortDateString();
            else if (d.TotalSeconds > 1)
                return ((int)d.TotalSeconds).ToString() + " Seconds | " + LastPlayed.ToShortDateString();
            return "Never Played";
        }
    }
}

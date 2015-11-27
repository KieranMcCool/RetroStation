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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RetroStation
{
    public partial class frmMain : Form
    {
        ControllerInput ci = new ControllerInput();

        public frmMain()
        {
            InitializeComponent();

            llUpdate.Visible = !Program.updateToDate();

            reload();

            this.WindowState = FormWindowState.Maximized;

            ci.ButtonPressed += Ci_ButtonPressed;

            btnPlay.Click += (sender, e) =>
            {
                int sel = lbGames.SelectedIndices[0]; var p = cbPlatform.SelectedItem; playing = true;
                DataManagement.Games.Find(x => x.getFriendlyName() == (string)lbGames.SelectedItem).launch();
                reload(); lbGames.SelectedIndex = sel; cbPlatform.SelectedItem = p; playing = false;
                playing = false;
            };

            bulkFromDirectoryToolStripMenuItem.Click += BulkFromDirectoryToolStripMenuItem_Click;
            fromFileToolStripMenuItem.Click += (sender, e) =>
            {
                var dia = new frmAddGameDia();
                if (dia.ShowDialog() == DialogResult.OK)
                    DataManagement.Games.Add(new Game(dia.results));
                reload();
            };

            managePlatformsToolStripMenuItem.Click += (sender, e) => {
                frmPlatManager fp = new frmPlatManager();
                fp.ShowDialog();  reload(); };
        }

        private void Ci_ButtonPressed(string[] actions, string[] buttonsPressed)
        {
            var cycleSelected = new Action<object, int>((s, i) => {
                if(s  is ListBox)
                {
                    var contextS = ((ListBox)s);
                    contextS.BeginInvoke(new MethodInvoker(() =>
                    contextS.SelectedIndex = pyMod(contextS.SelectedIndex + i, contextS.Items.Count)));
                } else {
                    var contextS = (ComboBox)s;
                    contextS.BeginInvoke(new MethodInvoker(() =>
                    contextS.SelectedIndex = pyMod(contextS.SelectedIndex + i, contextS.Items.Count)));
                }
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
            {
                try
                {
                    Program.Play.Kill();
                    playing = false;
                } catch { }
            }
        }
    
        bool playing = false;

        private int pyMod(int a, int b)
        {
            return ((a % b) + b) % b;
        }

        private void reload()
        {
            DataManagement.loadSupported();
            DataManagement.loadGames();

            lbGames.Items.Clear();
            cbPlatform.Items.Clear();

            cbPlatform.Items.Add("All");

            foreach (var p in DataManagement.Platforms)
            {
                cbPlatform.Items.Add(p.getFriendlyName());
            }

            cbPlatform.SelectedIndex = 0;
            if (lbGames.Items.Count != 0)
                lbGames.SelectedIndex = 0;
            tbSearch_TextChanged(this, null);
        }

        private void BulkFromDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                Game g;
                string platform = DataManagement.getPlatform(s);
                if (platform != null)
                {
                    g = new Game(new string[]
                    {
                    s, fi.Name.Replace(fi.Extension, "").Replace(".", ""),
                    platform, move.ToString()
                    });
                }
            }
            reload();
        }

        private void filterBy(Platform fType, List<Game> g = null)
        {
            if (g == null)
                g = DataManagement.Games;
            lbGames.Items.Clear();
            if (fType == null)
            {
                foreach (var v in g)
                    lbGames.Items.Add(v.getFriendlyName());
            }
            else
            {
                foreach (var v in g)
                    if (v.getPlatform() == fType)
                        lbGames.Items.Add(v.getFriendlyName());
            }
        }

        private void cbPlatform_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbPlatform.SelectedIndex)
            {
                case 0:
                    tbSearch_TextChanged(this, null);
                    break;
                default:
                    var filterType = DataManagement.Platforms.Find(x => x.getFriendlyName() ==
                        (string)cbPlatform.Items[cbPlatform.SelectedIndex]);
                    tbSearch_TextChanged(this, null);
                    break;
            }
        }

        private void lbGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbGames.SelectedIndex >= 0 && lbGames.SelectedIndex < lbGames.Items.Count)
            {
                var game = DataManagement.Games.Find(x => x.getFriendlyName() == (string)lbGames.SelectedItem);
                try
                {
                    lblGameName.Text = game.getFriendlyName();
                    label2.Text = FormatTime(new TimeSpan(0, 0, (int)game.getSecondsPlayed()),
                        game.getLastPlayed());
                }
                catch { }
            }
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

        private void llUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo("www.github.com/KieranMcCool/RetroStation");
            p.Start();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            var filterPlat = DataManagement.Platforms.Find(x =>
                x.getFriendlyName() == cbPlatform.Text);
            filterBy(filterPlat,
                DataManagement.Games.FindAll(x => x.getFriendlyName().ToUpper().Contains(tbSearch.Text.ToUpper())));
        }
    }
}

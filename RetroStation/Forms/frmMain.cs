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
        // Instantiates the wrapper class that deals with controller input.
        ControllerInput controllerInput = new ControllerInput();
        // Keep track of if we're playing or not
        bool playing = false;
        public frmMain()
        {
            InitializeComponent();
            // inform user of update availability if needed.
            llUpdate.Visible = !Program.updateToDate();
            
            // Maxmise window.
            this.WindowState = FormWindowState.Maximized;

            // Code Below sets up action listeners for Control interactions

            // Setup the listener for controller input
            controllerInput.ButtonPressed += Ci_ButtonPressed;

            // Sets playing to true and lanches the selected game. Waits for close and then sets playing to false.
            btnPlay.Click += (sender, e) =>
            {
                if (lbGames.SelectedIndex >= 0 && lbGames.SelectedIndex < lbGames.Items.Count)
                {
                    int sel = lbGames.SelectedIndices[0]; var p = cbPlatform.SelectedItem; playing = true;
                    DataManagement.Games.Find(x => x.getFriendlyName() == (string)lbGames.SelectedItem).launch();
                    reload(); lbGames.SelectedIndex = sel; cbPlatform.SelectedItem = p; playing = false;
                    playing = false;
                }
            };

            bulkFromDirectoryToolStripMenuItem.Click += BulkFromDirectoryToolStripMenuItem_Click;
            // Action Listener for the bulk rom import option 
            fromFileToolStripMenuItem.Click += (sender, e) =>
            {
                var dia = new frmAddGameDia();
                // Get the data for ROM Import from the add game dialog.   
                if (dia.ShowDialog() == DialogResult.OK)
                    DataManagement.Games.Add(new Game(dia.results));
                // Reload the UI to reflect changes.
                reload();
            };

            // Code to open the platform manager when the appropriate button is clicked (File -> Manage Platforms)
            managePlatformsToolStripMenuItem.Click += (sender, e) => {
                frmPlatManager fp = new frmPlatManager();
                fp.ShowDialog();  reload(); };

            // Code which changes the games shown based on filter and search settings
            Action FilterChange = () => {
                filterBy(DataManagement.Platforms.Find(x => x.getFriendlyName() == cbPlatform.Text),
                    DataManagement.Games.FindAll(x => x.getFriendlyName().ToLower().Contains(tbSearch.Text.ToLower())));
            };

            // Calls the FilterChange code when required.
            cbPlatform.SelectedIndexChanged += (sender, e) => { FilterChange(); };
            tbSearch.TextChanged += (sender, e) => { FilterChange(); };

            // Updates all UI Elements
            reload();
        }

        private void Ci_ButtonPressed(string[] actions, string[] buttonsPressed)
        {
            /* This action takes two parameters, an object (casted to either a listbox or combobox)
               and  moves its selected index by the integer argument */
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
            // if not in game
            if (!playing)
            {
                // Check actions received to see if we have to do anything
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
            // if we're in game and the quit action is received
            else if (actions.Contains("QUIT"))
            {
                try
                {
                    // Kill the running emulation.
                    Program.Play.Kill();
                    playing = false;
                } catch { }
            }
        }

        private int pyMod(int a, int b)
        {   
            // .net's mod function sucks, this is how python does it.
            return ((a % b) + b) % b;
        }

        private void reload()
        {
            // Reload game and platform information
            DataManagement.loadSupported();
            DataManagement.loadGames();
            // Clear game list and platform list
            lbGames.Items.Clear();
            cbPlatform.Items.Clear();
            // Add an all filter for platforms
            cbPlatform.Items.Add("All");
            
            // Foreach platform, add to platform filters
            foreach (var p in DataManagement.Platforms)
            {
                cbPlatform.Items.Add(p.getFriendlyName());
            }

            /// set the platform filter to all
            cbPlatform.SelectedIndex = 0;
            // If there's games, update the selected game to game 0 for controller to work.
            if (lbGames.Items.Count != 0)
                lbGames.SelectedIndex = 0;
            // Remove all search parameters.
            tbSearch.Text = "";

            // Update Game display.
            filterBy(DataManagement.Platforms.Find(x => x.getFriendlyName() == cbPlatform.Text),
                    DataManagement.Games.FindAll(x => x.getFriendlyName().ToLower().Contains(tbSearch.Text.ToLower())));

        }

        private void BulkFromDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Init move as false - Let's not delete the originals without permission ;)
            bool move = false;
            // As for permission to delete originals (move is way quicker than just a raw copy.)
            if (MessageBox.Show("Do you want to delete the original files?",
                "Import Options", MessageBoxButtons.YesNo) == DialogResult.Yes)
                // if yes, set move to true/
                move = true;

            // Initialise a folder browser dialog to get a directory to import form.
            var dia = new FolderBrowserDialog();
            string dir = "";
            // dir = selected dir
            if (dia.ShowDialog() == DialogResult.OK)
                dir = dia.SelectedPath;

            // Foreach file in the selected directory
            foreach (string s in Directory.GetFiles(dir, "*"))
            {
                // get info on file
                var fi = new FileInfo(s);
                // init a new game
                Game g;
                // Get the platform for it.
                string platform = DataManagement.getPlatform(s);
                // If we found one
                if (platform != null)
                {
                    // create a new game
                    g = new Game(new string[]
                    {
                        s, fi.Name.Replace(fi.Extension, "").Replace(".", ""),
                        platform, move.ToString()
                    });
                }
            }
            // reloads all UI elements.
            reload();
        }

        private void filterBy(Platform fType = null, List<Game> g = null)
        {
            // if we don't have a game list to filter by
            if (g == null)
                // use all games as game list
                g = DataManagement.Games;
            // Clear the items already in the list
            lbGames.Items.Clear();
            // If no filter type
            if (fType == null)
            {
                // add every game
                foreach (var v in g)
                    lbGames.Items.Add(v.getFriendlyName());
            }
            else
            {
                // add only games from the game list which satisfy the filter type.
                foreach (var v in g)
                    if (v.getPlatform() == fType)
                        lbGames.Items.Add(v.getFriendlyName());
            }
        }

        private void lbGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if valid game selected
            if (lbGames.SelectedIndex >= 0 && lbGames.SelectedIndex < lbGames.Items.Count)
            {
                // Get info on game and display.
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
            // Picks the most appropriate time scale to display time played in.
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
            // Starts a web browser to show the download page of the project. 
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo("www.github.com/KieranMcCool/RetroStation");
            p.Start();
        }
    }
}

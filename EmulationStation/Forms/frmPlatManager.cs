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

namespace RetroStation
{
    public partial class frmPlatManager : Form
    {
        public frmPlatManager()
        {
            InitializeComponent();
        }

        private void frmPlatManager_Load(object sender, EventArgs e)
        {
            reload();
            btnClose.Click += (s, notE) => this.Close();
        }

        private void reload()
        {
            lvPlatforms.Items.Clear();
            foreach (var v in DataManagement.Platforms)
            {
                string[] split = v.getCSVLine().Split(',');
                lvPlatforms.Items.Add(new ListViewItem(new string[] {
                    split[0], split[1], split[2], joinArray(split, ", ", 3)
                }));
            }
        }

        private string joinArray(string[] s, string connector, int index)
        {
            string building = s[index];
            for(int i = index + 1; i < s.Length; i++)
            {
                building += string.Format("{0}{1}",
                    connector, s[i]);
            }
            return building;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int editing = searchPlatform(lvPlatforms.SelectedItems[0].Text);
            frmAddPlatform aPlat = new frmAddPlatform(DataManagement.Platforms[editing]);
            if(aPlat.ShowDialog() == DialogResult.OK)
                DataManagement.removePlatform(editing);
            reload();
        }

        private int searchPlatform(string fName)
        {
            return DataManagement.Platforms.IndexOf(DataManagement.Platforms.Find(x =>
                x.getFriendlyName() == fName));
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            frmAddPlatform aPlat = new frmAddPlatform();
            aPlat.ShowDialog();
            reload();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int removing = searchPlatform(lvPlatforms.SelectedItems[0].Text);
            DataManagement.removePlatform(removing);
            reload();
        }
    }
}

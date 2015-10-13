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

namespace EmulationStation
{
    public partial class frmAddGameDia : Form
    {
        public frmAddGameDia()
        {
            InitializeComponent();
            foreach (Platform p in Program.Platforms)
                cbPlatform.Items.Add(p.getFriendlyName());
        }

        public string[] results = new string[4];

        private void btnAdd_Click(object sender, EventArgs e)
        {
            results[0] = tbPath.Text;
            results[1] = tbName.Text;
            results[2] = (string)cbPlatform.SelectedItem;
            results[3] = cbDeleteOriginalFile.Checked.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDirChange_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog() { Multiselect = false };
            if (fd.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = fd.FileName;
                var fi = new System.IO.FileInfo(fd.FileName);
                tbName.Text = fi.Name.Replace(fi.Extension, "");
                string p = Program.getPlatform(fd.FileName);
                if (p != null)
                    cbPlatform.SelectedItem = p;           
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}

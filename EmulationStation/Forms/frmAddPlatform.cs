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
    public partial class frmAddPlatform : Form
    {
        public frmAddPlatform()
        {
            InitializeComponent();
        }

        public frmAddPlatform(Platform p)
        {
            InitializeComponent();
            tbCLI.Text = p.getCommandTemplate();
            tbDir.Text = p.getEmulatorPath();
            tbFName.Text = p.getFriendlyName();
            foreach (var v in p.getFileExtension())
                lbExtensions.Items.Add(v);
        }

        private string formatExtension(string s)
        {
            return s.Replace("*", "").Replace(".", "").ToLower();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lbExtensions.Items.Add(formatExtension(
                tbExtensionEntry.Text));
            tbExtensionEntry.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(lbExtensions.SelectedIndex != -1)
                lbExtensions.Items.RemoveAt(lbExtensions.SelectedIndex);
        }

        private string platformText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(tbFName.Text + ",");
            var s = tbDir.Text.Replace(Environment.CurrentDirectory.ToString(), "");
            sb.Append(s + ",");
            sb.Append(tbCLI.Text + ",");
            for (int i = 0; i < lbExtensions.Items.Count - 2; i++)
                sb.Append((string)lbExtensions.Items[i] + ",");
            sb.Append(lbExtensions.Items[lbExtensions.Items.Count - 1]);
            return sb.ToString();
        }
        

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataManagement.addPlatform(new Platform(platformText().Split(',').ToList()));
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDirChange_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            { Multiselect = false };
            if (ofd.ShowDialog() == DialogResult.OK)
                tbDir.Text = ofd.FileName;
        }
    }
}
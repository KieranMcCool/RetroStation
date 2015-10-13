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
    public partial class frmAddPlatform : Form
    {
        public frmAddPlatform()
        {
            InitializeComponent();
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
            Program.addPlatform(new Platform(platformText().Split(',').ToList()));
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
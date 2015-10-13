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

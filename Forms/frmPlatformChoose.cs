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
    public partial class frmPlatformChoose : Form
    {
        public frmPlatformChoose(string[] platforms, string gameName="")
        {
            InitializeComponent();
            if (gameName != "")
                lblName.Text = "Game:" + gameName;
            foreach (var p in platforms)
                lbPlatforms.Items.Add(p);
        }

        public int result;

        private void btnOk_Click(object sender, EventArgs e)
        {
            result = lbPlatforms.SelectedIndex;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

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
            foreach (var v in Program.Platforms)
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
            frmAddPlatform aPlat = new frmAddPlatform(Program.Platforms[editing]);
            if(aPlat.ShowDialog() == DialogResult.OK)
                Program.removePlatform(editing);
            reload();
        }

        private int searchPlatform(string fName)
        {
            return Program.Platforms.IndexOf(Program.Platforms.Find(x =>
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
            Program.removePlatform(removing);
            reload();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Grindstone
{
    public partial class FormProjectPicker : Form
    {
        public FormProjectPicker()
        {
            InitializeComponent();
        }

        private void FormProjectPicker_Load(object sender, EventArgs e)
        {

        }

        private void checkBoxToSrcFolder_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void projectList_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void projectList_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.projectList.SelectedIndex != -1 && e.KeyCode == Keys.Enter)
            {
                this.Close();
            }
        }
    }
}

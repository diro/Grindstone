namespace Grindstone
{
    partial class FormProjectPicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.projectList = new System.Windows.Forms.ListBox();
            this.checkBoxVivotekProject = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // projectList
            // 
            this.projectList.FormattingEnabled = true;
            this.projectList.ItemHeight = 12;
            this.projectList.Location = new System.Drawing.Point(-1, -1);
            this.projectList.Name = "projectList";
            this.projectList.Size = new System.Drawing.Size(285, 124);
            this.projectList.TabIndex = 0;
            this.projectList.DoubleClick += new System.EventHandler(this.projectList_DoubleClick);
            this.projectList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.projectList_KeyUp);
            // 
            // checkBoxToSrcFolder
            // 
            this.checkBoxVivotekProject.AutoSize = true;
            this.checkBoxVivotekProject.Checked = true;
            this.checkBoxVivotekProject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVivotekProject.Location = new System.Drawing.Point(3, 127);
            this.checkBoxVivotekProject.Name = "checkBoxToSrcFolder";
            this.checkBoxVivotekProject.Size = new System.Drawing.Size(152, 16);
            this.checkBoxVivotekProject.TabIndex = 1;
            this.checkBoxVivotekProject.Text = "VIVOTEK Folder Structure";
            this.checkBoxVivotekProject.UseVisualStyleBackColor = true;
            this.checkBoxVivotekProject.CheckedChanged += new System.EventHandler(this.checkBoxToSrcFolder_CheckedChanged);
            // 
            // FormProjectPicker
            // 
            this.ClientSize = new System.Drawing.Size(284, 146);
            this.Controls.Add(this.checkBoxVivotekProject);
            this.Controls.Add(this.projectList);
            this.Name = "FormProjectPicker";
            this.Load += new System.EventHandler(this.FormProjectPicker_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ListBox projectList;
        public System.Windows.Forms.CheckBox checkBoxVivotekProject;
    }
}

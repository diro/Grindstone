﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Grindstone
{
   
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CommandAddClass
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("47174d19-f19f-47cf-90ae-5f8a17553fc4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAddClass"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CommandAddClass(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CommandAddClass Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CommandAddClass(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>


       
        private void MenuItemCallback(object sender, EventArgs e)
        {
            FormProjectPicker form = new FormProjectPicker();

            var DTE = (DTE2)this.ServiceProvider.GetService(typeof(DTE));
            var solution = (IVsSolution)this.ServiceProvider.GetService(typeof(IVsSolution));

            List<EnvDTE.Project> projectList = new List<EnvDTE.Project>();
            foreach (IVsHierarchy hier in Utility.GetProjectsInSolution(solution, __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION))
            {
                EnvDTE.Project project = Utility.GetDTEProject(hier);
                if (project != null)
                {
                    projectList.Add(project);
                    form.projectList.Items.Add(project.Name);
                }
            }

            form.ShowDialog();

            if (form.projectList.SelectedItem == null)
                return;

            String selectedProjectName = form.projectList.SelectedItem.ToString();
            if (selectedProjectName == "")
                return;

            String className = GetSelectionClassName(DTE);
            EnvDTE.Project targetProject = projectList[form.projectList.SelectedIndex];
            Utility.AddClassToProject(targetProject, className, form.checkBoxVivotekProject.Checked);
        }


        private static String GetSelectionClassName(DTE2 DTE)
        {
            ProjectItem projectItem = DTE.ActiveDocument.ProjectItem;
            TextSelection selection = (TextSelection)projectItem.Document.Selection;
            return selection.Text;            
        }
    }
}

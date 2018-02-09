using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Grindstone
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CommandGenerateInterface
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("47174d19-f19f-47cf-90ae-5f8a17553fc4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandGenerateInterface"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CommandGenerateInterface(Package package)
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
        public static CommandGenerateInterface Instance
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
            Instance = new CommandGenerateInterface(package);
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
            var DTE = (DTE2)this.ServiceProvider.GetService(typeof(DTE));
            var solution = (IVsSolution)this.ServiceProvider.GetService(typeof(IVsSolution));

            FormProjectPicker form = new FormProjectPicker();

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

            EnvDTE.Project targetProject = projectList[form.projectList.SelectedIndex];

            TextSelection sel =
           (TextSelection)DTE.ActiveDocument.Selection;
            CodeClass sourceClass = (CodeClass)sel.ActivePoint.get_CodeElement(vsCMElement.vsCMElementClass);
            List<string> functionList = new List<string>();
            string interfaceName = "I" + sourceClass.Name;
            Utility.AddClassToProject(targetProject, interfaceName, false);

            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
                AddFunctionToClass(targetProject, sourceClass, interfaceName);
                timer.Dispose();
            },
                        null, 100, System.Threading.Timeout.Infinite);
            
        }

        private static void AddFunctionToClass(Project targetProject, CodeClass cls, string interfaceName)
        {
            CodeClass targetClass = null;

            int tryCount = 0;
            do
            {
                try
                {
                    targetClass = (CodeClass) targetProject.CodeModel.CodeElements.Item(interfaceName);
                    System.Threading.Thread.Sleep(10);
                }
                catch
                {

                }
            } while (targetClass == null && tryCount < 1024);

            if (targetClass == null)
            {
                return;
            }

            foreach (CodeElement elem in cls.Members)
            {
                if (elem.Kind == vsCMElement.vsCMElementFunction)
                {
                    if (elem.Name != cls.Name &&
                        elem.Name != "~" + cls.Name)
                    {
                        CodeFunction originalFunction = (CodeFunction) elem;

                        if (originalFunction.Access == vsCMAccess.vsCMAccessPublic)
                        {
                            CodeFunction newFunction = targetClass.AddFunction(elem.Name, vsCMFunction.vsCMFunctionPure | vsCMFunction.vsCMFunctionVirtual, originalFunction.Type, -1);
                            int paramIdx = 0;
                            foreach (CodeParameter param in originalFunction.Parameters)
                            {
                                string oldParamsName = param.Name;
                                newFunction.AddParameter(oldParamsName, param.Type, ++paramIdx);
                            }
                        }
                    }
                }
            }

            return;
        }
    }
}

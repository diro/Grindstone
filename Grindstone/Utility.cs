using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;

namespace Grindstone
{
    class Utility
    {
        public static EnvDTE.Project GetDTEProject(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            object obj;
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
            return obj as EnvDTE.Project;
        }
        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution, __VSENUMPROJFLAGS flags)
        {
            if (solution == null)
                yield break;

            IEnumHierarchies enumHierarchies;
            Guid guid = Guid.Empty;
            solution.GetProjectEnum((uint)flags, ref guid, out enumHierarchies);
            if (enumHierarchies == null)
                yield break;

            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            uint fetched;
            while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1)
            {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                    yield return hierarchy[0];
            }
        }
        public static void AddClassToProject(EnvDTE.Project targetProject, string className, bool withCppFile = true, bool VVTKProjectStruct = true)
        {
            String classHeaderFileame = className + ".h";
            String classImplementFileName = className + ".cpp";

            String vcprojectFolder = System.IO.Path.GetDirectoryName(targetProject.FullName);
            String projectFolder = vcprojectFolder;
            String sourceFolder = vcprojectFolder;
            String includeFolder = vcprojectFolder;
            if (VVTKProjectStruct)
            {
                projectFolder = System.IO.Directory.GetParent(vcprojectFolder).FullName;

                sourceFolder = System.IO.Path.Combine(projectFolder, "src");
                includeFolder = System.IO.Path.Combine(projectFolder, "inc");
                includeFolder = System.IO.Path.Combine(includeFolder, targetProject.Name);

                System.IO.Directory.CreateDirectory(sourceFolder);
                System.IO.Directory.CreateDirectory(includeFolder);
            }

            System.IO.File.AppendAllText(System.IO.Path.Combine(includeFolder, classHeaderFileame), "class " + className + "\n{\nprivate:\n\npublic:\n};\n");
            if (withCppFile)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(sourceFolder, classImplementFileName), "#include <" + classHeaderFileame + ">");
            }

            if (VVTKProjectStruct)
            {
                var newProjectItem = targetProject.ProjectItems.AddFromFile("../inc/" + targetProject.Name + "/" + classHeaderFileame);
                if (withCppFile)
                {
                    targetProject.ProjectItems.AddFromFile("../src/" + classImplementFileName);
                }
            }
            else
            {
                var newProjectItem = targetProject.ProjectItems.AddFromFile(classHeaderFileame);
                if (withCppFile)
                {
                    targetProject.ProjectItems.AddFromFile(classImplementFileName);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;

namespace Grindstone
{
    public static class CodeElementHelper
    {
        public static CodeElement[] GetCodeElementAtCursor(DTE2 dte)
        {
            try
            {
                var cursorTextPoint = GetCursorTextPoint(dte);

                if (cursorTextPoint != null)
                {
                    var activeDocument = dte.ActiveDocument;
                    var projectItem = activeDocument.ProjectItem;
                    var codeElements = projectItem.FileCodeModel.CodeElements;
                    return GetCodeElementAtTextPoint(codeElements, cursorTextPoint).ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[DBG][EXC] - " + ex.Message + " " + ex.StackTrace);
            }

            return null;
        }

        private static TextPoint GetCursorTextPoint(DTE2 dte)
        {
            var cursorTextPoint = default(TextPoint);

            try
            {
                var objTextDocument = (TextDocument)dte.ActiveDocument.Object();
                cursorTextPoint = objTextDocument.Selection.ActivePoint;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[DBG][EXC] - " + ex.Message + " " + ex.StackTrace);
            }

            return cursorTextPoint;
        }

        private static List<CodeElement> GetCodeElementAtTextPoint(CodeElements codeElements, TextPoint objTextPoint)
        {
            var returnValue = new List<CodeElement>();

            if (codeElements == null)
                return null;

            foreach (CodeElement element in codeElements)
            {
                if (element.StartPoint.GreaterThan(objTextPoint))
                {
                    // The code element starts beyond the point
                }
                else if (element.EndPoint.LessThan(objTextPoint))
                {
                    // The code element ends before the point
                }
                else
                {
                    if (element.Kind == vsCMElement.vsCMElementClass ||
                        element.Kind == vsCMElement.vsCMElementProperty ||
                        element.Kind == vsCMElement.vsCMElementVariable ||
                        element.Kind == vsCMElement.vsCMElementPropertySetStmt ||
                        element.Kind == vsCMElement.vsCMElementFunction)
                    {
                        returnValue.Add(element);
                    }

                    var memberElements = GetCodeElementMembers(element);
                    var objMemberCodeElement = GetCodeElementAtTextPoint(memberElements, objTextPoint);

                    if (objMemberCodeElement != null)
                    {
                        returnValue.AddRange(objMemberCodeElement);
                    }

                    break;
                }
            }

            return returnValue;
        }

        private static CodeElements GetCodeElementMembers(CodeElement codeElement)
        {
            CodeElements codeElements = null;

            if (codeElement is CodeNamespace)
            {
                codeElements = (codeElement as CodeNamespace).Members;
            }
            else if (codeElement is CodeType)
            {
                codeElements = (codeElement as CodeType).Members;
            }
            else if (codeElement is CodeFunction)
            {
                codeElements = (codeElement as CodeFunction).Parameters;
            }

            return codeElements;
        }
    }
}

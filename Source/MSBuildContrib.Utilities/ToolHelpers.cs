//------------------------------------------------------------------------------
// MSBuildContrib
//
// ToolHelpers.cs
//
//------------------------------------------------------------------------------
// Copyright (C) 2007-2008 Scott Dorman (sj_dorman@hotmail.com)
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307 USA
//------------------------------------------------------------------------------
using System;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildContrib.Utilities
{
    /// <summary>
    /// Groups a set of useful helper methods for different command line tools.
    /// </summary>
    public static class ToolHelpers
    {
        #region events
        #endregion

        #region class-wide fields
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods
        #endregion

        #endregion

        #region public properties and methods

        #region properties
        #endregion

        #region methods

        #region FindFxCop
        /// <summary>
        /// Determines if FxCop is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for FxCop.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if FxCop is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindFxCop(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            string toolName = "Microsoft FxCop";
            string fxCop132Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), toolName + " 1.32");
            string fxCop135Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), toolName + " 1.35");
            string fxCop136Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), toolName + " 1.36");

            if (Directory.Exists(fxCop136Path))
            {
                toolPath.ItemSpec = fxCop136Path;
            }
            else if (Directory.Exists(fxCop135Path))
            {
                toolPath.ItemSpec = fxCop135Path;
            }
            else if (Directory.Exists(fxCop132Path))
            {
                toolPath.ItemSpec = fxCop132Path;
            }
            else
            {
                toolPath.ItemSpec = String.Empty;
            }

            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindHtmlHelpWorkshop
        /// <summary>
        /// Determines if the HTML Help Workshop is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for the HTML Help Workshop.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if the HTML Help Workshop is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindHtmlHelpWorkshop(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            string toolName = "HTML Help Workshop";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), toolName);

            if (Directory.Exists(path))
            {
                toolPath.ItemSpec = path;
            }

            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindNDoc
        /// <summary>
        /// Determines if NDoc is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for NDoc.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if NDoc is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindNDoc(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            string toolName = "NDoc 1.3";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), toolName);

            if (Directory.Exists(path))
            {
                toolPath.ItemSpec = path;
            }

            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindNUnit
        /// <summary>
        /// Determines if NUnit is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for NUnit.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if NUnit is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindNUnit(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            string toolName = "NUnit 2.2";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), toolName);

            if (Directory.Exists(path))
            {
                toolPath.ItemSpec = path;
            }

            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindSandcastle
        /// <summary>
        /// Determines if Microsoft Sandcastle is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for Microsoft Sandcastle.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if Microsoft Sandcastle is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindSandcastle(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            string toolName = "Sandcastle";
            string sandcastlePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), toolName);

            if (Directory.Exists(sandcastlePath))
            {
                toolPath.ItemSpec = sandcastlePath;
            }

            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindSandcastleHelpFileBuilder
        /// <summary>
        /// Determines if Sandcastle Help File Builder is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for Sandcastle Help File Builder.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if Sandcastle Help File Builder is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindSandcastleHelpFileBuilder(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            string toolName = "Sandcastle Help File Builder";
            string sandcastlePath = PathHelpers.CombinePaths(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EWSoftware", toolName);

            if (Directory.Exists(sandcastlePath))
            {
                toolPath.ItemSpec = sandcastlePath;
            }

            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindVisualBasic
        /// <summary>
        /// Determines if Microsoft Visual Basic is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for Microsoft Visual Basic.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if Microsoft Visual Basic is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindVisualBasic(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            string toolName = "Microsoft Visual Basic";
            using (Microsoft.Win32.RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\6.0"))
            {
                if (regkey != null)
                {
                    using (Microsoft.Win32.RegistryKey setupKey = regkey.OpenSubKey("Setup"))
                    {
                        if (setupKey != null)
                        {
                            using (Microsoft.Win32.RegistryKey visualBasicKey = setupKey.OpenSubKey(toolName))
                            {
                                if (visualBasicKey != null)
                                {
                                    toolPath.ItemSpec = (string)visualBasicKey.GetValue("ProductDir", "");
                                }
                            }
                        }
                    }
                }
            }
            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindVisualStudio2003
        /// <summary>
        /// Determines if Microsoft Visual Studio 2003 is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for Microsoft Visual Studio 2003.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if Microsoft Visual Studio 2003 is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindVisualStudio2003(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();
            string toolName = "Microsoft Visual Studio 2003";

            using (Microsoft.Win32.RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\7.1"))
            {
                if (regkey != null)
                {
                    toolPath.ItemSpec = (string)regkey.GetValue("toolPath", "");
                }
            }
            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindVisualStudio2005
        /// <summary>
        /// Determines if Microsoft Visual Studio 2005 is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for Microsoft Visual Studio 2005.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if Microsoft Visual Studio 2005 is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindVisualStudio2005(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();
            string toolName = "Microsoft Visual Studio 2005";

            using (Microsoft.Win32.RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\8.0"))
            {
                if (regkey != null)
                {
                    toolPath.ItemSpec = (string)regkey.GetValue("toolPath", "");
                }
            }
            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindVisualStudio2008
        /// <summary>
        /// Determines if Microsoft Visual Studio 2008 is installed.
        /// </summary>
        /// <param name="toolPath">The installation folder for Microsoft Visual Studio 2008.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if Microsoft Visual Studio 2008 is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
        public static bool FindVisualStudio2008(out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();
            string toolName = "Microsoft Visual Studio 2008";

            using (Microsoft.Win32.RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\9.0"))
            {
                if (regkey != null)
                {
                    toolPath.ItemSpec = (string)regkey.GetValue("toolPath", "");
                }
            }
            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #region FindVisualStudioTool
        /// <summary>
        /// Determines if the specified Visual Studio tool is installed.
        /// </summary>
        /// <param name="toolName">The name of the command line tool.</param>
        /// <param name="toolPath">The installation folder for the tool.</param>
        /// <param name="log">The logger used by the calling task.</param>
        /// <returns><see langword="true"/> if the tool is installed; otherwise <see langword="false"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public static bool FindVisualStudioTool(string toolName, out ITaskItem toolPath, TaskLoggingHelper log)
        {
            bool toolFound = false;
            toolPath = new TaskItem();

            bool vs2003Found;
            ITaskItem vs2003toolPath;
            bool vs2005Found;
            ITaskItem vs2005toolPath;
            bool vs2008Found;
            ITaskItem vs2008toolPath;
            
            vs2003Found = FindVisualStudio2003(out vs2003toolPath, log);
            vs2005Found = FindVisualStudio2005(out vs2005toolPath, log);
            vs2008Found = FindVisualStudio2008(out vs2008toolPath, log);

            if (vs2003Found)
            {
                if (File.Exists(Path.Combine(vs2003toolPath.ItemSpec, toolName)))
                {
                    toolPath = vs2003toolPath;
                }
            }
            if (vs2005Found)
            {
                if (File.Exists(Path.Combine(vs2005toolPath.ItemSpec, toolName)))
                {
                    toolPath = vs2005toolPath;
                }
            }
            if (vs2008Found)
            {
                if (File.Exists(Path.Combine(vs2008toolPath.ItemSpec, toolName)))
                {
                    toolPath = vs2008toolPath;
                }
            }

            toolFound = (toolPath.ItemSpec.Length != 0);
            if (log != null)
            {
                if (toolFound)
                {
                    log.LogMessageFromResources("ToolFound", toolName, toolPath);
                }
                else
                {
                    log.LogMessageFromResources("ToolNotFound", toolName);
                }
            }

            return toolFound;
        }
        #endregion

        #endregion

        #endregion
    }
}
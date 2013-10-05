//------------------------------------------------------------------------------
// MSBuildContrib
//
// GetEnvironment.cs
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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;
using MSBuildContrib.Utilities;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Retrieves information about the execution environment.
    /// </summary>
    /// <example>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <GetEnvironment>
    ///    <Output TaskParameter="OSVersion" PropertyName="OSVersion" />
    ///    <Output TaskParameter="ProcessorCount" PropertyName="ProcessorCount" />
    ///    <Output TaskParameter="UserInteractive" PropertyName="UserInteractive" />
    ///    <Output TaskParameter="LogicalDrives" PropertyName="LogicalDrives" />
    /// </GetEnvironment>
    ///     ]]>
    ///   </code>
    /// </example>
    public class GetEnvironment : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private ITaskItem applicationDataDirectory;
        private ITaskItem commonApplicationDataDirectory;
        private ITaskItem commonProgramFilesDirectory;
        private ITaskItem currentDirectory;
        private ITaskItem desktopDirectory;
        private bool fxCopFound;
        private ITaskItem fxCopPath;
        private bool htmlHelpWorkshopFound;
        private ITaskItem htmlHelpWorkshopPath;
        private string[] logicalDrives;
        private bool ndocFound;
        private ITaskItem ndocPath;
        private bool nunitFound;
        private ITaskItem nunitPath;
        private string oSVersion;
        private int processorCount;
        private ITaskItem programFilesDirectory;
        private bool sandCastleFound;
        private bool sandCastleHelpFileBuilderFound;
        private ITaskItem sandCastleHelpFileBuilderPath;
        private ITaskItem sandCastlePath;
        private ITaskItem systemDirectory;
        private ITaskItem tempDirectory;
        private bool userInteractive;
        private bool vbFound;
        private ITaskItem vbPath;
        private bool vs2003Found;
        private ITaskItem vs2003Path;
        private bool vs2005Found;
        private ITaskItem vs2005Path;
        private bool vs2008Found;
        private ITaskItem vs2008Path;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region ApplicationDataDirectory
        /// <summary>
        /// The directory that serves as a common repository for application-specific data for the current roaming user.
        /// </summary>
        [Output]
        public ITaskItem ApplicationDataDirectory
        {
            get
            {
                return this.applicationDataDirectory;
            }
        } 
        #endregion

        #region CommonApplicationDataDirectory
        /// <summary>
        /// The directory that serves as a common repository for application-specific data that is used by all users.
        /// </summary>
        [Output]
        public ITaskItem CommonApplicationDataDirectory
        {
            get
            {
                return this.commonApplicationDataDirectory;
            }
        } 
        #endregion

        #region CommonProgramFilesDirectory
        /// <summary>
        /// The directory for components that are shared across applications.
        /// </summary>
        [Output]
        public ITaskItem CommonProgramFilesDirectory
        {
            get
            {
                return this.commonProgramFilesDirectory;
            }
        } 
        #endregion

        #region CurrentDirectory
        /// <summary>
        /// The current working directory.
        /// </summary>
        [Output]
        public ITaskItem CurrentDirectory
        {
            get
            {
                return this.currentDirectory;
            }
        } 
        #endregion

        #region DesktopDirectory
        /// <summary>
        /// The directory used to physically store file objects on the desktop.
        /// </summary>
        [Output]
        public ITaskItem DesktopDirectory
        {
            get
            {
                return this.desktopDirectory;
            }
        } 
        #endregion

        #region FxCopPath
        /// <summary>
        /// The install directory for Microsoft FxCop.
        /// </summary>
        [Output]
        public ITaskItem FxCopPath
        {
            get
            {
                return this.fxCopPath;
            }
        } 
        #endregion

        #region HtmlHelpWorkshopPath
        /// <summary>
        /// The install directory for the HTML Help Workshop.
        /// </summary>
        [Output]
        public ITaskItem HtmlHelpWorkshopPath
        {
            get
            {
                return this.htmlHelpWorkshopPath;
            }
        } 
        #endregion

        #region IsFxCopInstalled
        /// <summary>
        /// Indicates if Microsoft FxCop is installed.
        /// </summary>
        [Output]
        public bool IsFxCopInstalled
        {
            get
            {
                return this.fxCopFound;
            }
            set
            {
                this.fxCopFound = value;
            }
        } 
        #endregion

        #region IsHtmlHelpWorkshopInstalled
        /// <summary>
        /// Indicates if the HTML Help Workshop is installed.
        /// </summary>
        [Output]
        public bool IsHtmlHelpWorkshopInstalled
        {
            get
            {
                return this.htmlHelpWorkshopFound;
            }
            set
            {
                this.htmlHelpWorkshopFound = value;
            }
        } 
        #endregion

        #region IsNDocInstalled
        /// <summary>
        /// Indicates if NDoc is installed.
        /// </summary>
        [Output]
        public bool IsNDocInstalled
        {
            get
            {
                return this.ndocFound;
            }
            set
            {
                this.ndocFound = value;
            }
        } 
        #endregion

        #region IsNUnitInstalled
        /// <summary>
        /// Indicates if NUnit is installed.
        /// </summary>
        [Output]
        public bool IsNUnitInstalled
        {
            get
            {
                return this.nunitFound;
            }
            set
            {
                this.nunitFound = value;
            }
        } 
        #endregion

        #region IsSandcastleHelpFileBuilderInstalled
        /// <summary>
        /// Indicates if the Sandcastle Help File Builder is installed.
        /// </summary>
        [Output]
        public bool IsSandcastleHelpFileBuilderInstalled
        {
            get
            {
                return this.sandCastleHelpFileBuilderFound;
            }
            set
            {
                this.sandCastleHelpFileBuilderFound = value;
            }
        } 
        #endregion

        #region IsSandcastleInstalled
        /// <summary>
        /// Indicates if Sandcastle is installed.
        /// </summary>
        [Output]
        public bool IsSandcastleInstalled
        {
            get
            {
                return this.sandCastleFound;
            }
            set
            {
                this.sandCastleFound = value;
            }
        } 
        #endregion

        #region IsVisualBasicInstalled
        /// <summary>
        /// Indicates if Visual Basic 6.0 is installed.
        /// </summary>
        [Output]
        public bool IsVisualBasicInstalled
        {
            get
            {
                return this.vbFound;
            }
            set
            {
                this.vbFound = value;
            }
        } 
        #endregion

        #region IsVisualStudio2003Installed
        /// <summary>
        /// Indicates if Visual Studio 2003 is installed.
        /// </summary>
        [Output]
        public bool IsVisualStudio2003Installed
        {
            get
            {
                return this.vs2003Found;
            }
            set
            {
                this.vs2003Found = value;
            }
        } 
        #endregion

        #region IsVisualStudio2005Installed
        /// <summary>
        /// Indicates if Visual Studio 2005 is installed.
        /// </summary>
        [Output]
        public bool IsVisualStudio2005Installed
        {
            get
            {
                return this.vs2005Found;
            }
            set
            {
                this.vs2005Found = value;
            }
        } 
        #endregion

        #region IsVisualStudio2008Installed
        /// <summary>
        /// Indicates if Visual Studio 2008 is installed.
        /// </summary>
        [Output]
        public bool IsVisualStudio2008Installed
        {
            get
            {
                return this.vs2008Found;
            }
            set
            {
                this.vs2008Found = value;
            }
        } 
        #endregion

        #region LogicalDrives
        /// <summary>
        /// Returns the names of the logical drives on the current computer.
        /// </summary>
        [Output]
        public string[] LogicalDrives
        {
            get
            {
                return this.logicalDrives;
            }
            set
            {
                this.logicalDrives = value;
            }
        }
        #endregion

        #region NDocPath
        /// <summary>
        /// The install directory for NDoc.
        /// </summary>
        [Output]
        public ITaskItem NDocPath
        {
            get
            {
                return this.ndocPath;
            }
        } 
        #endregion

        #region NUnitPath
        /// <summary>
        /// The install directory for NUnit.
        /// </summary>
        [Output]
        public ITaskItem NUnitPath
        {
            get
            {
                return this.nunitPath;
            }
        } 
        #endregion

        #region OSVersion
        /// <summary>
        /// The current platform identifier and version number.
        /// </summary>
        [Output]
        public string OSVersion
        {
            get
            {
                return this.oSVersion;
            }
            set
            {
                this.oSVersion = value;
            }
        } 
        #endregion

        #region ProcessorCount
        /// <summary>
        /// The number of processors on the current machine.
        /// </summary>
        [Output]
        public int ProcessorCount
        {
            get
            {
                return this.processorCount;
            }
            set
            {
                this.processorCount = value;
            }
        } 
        #endregion

        #region ProgramFilesDirectory
        /// <summary>
        /// The program files directory.
        /// </summary>
        [Output]
        public ITaskItem ProgramFilesDirectory
        {
            get
            {
                return this.programFilesDirectory;
            }
        } 
        #endregion

        #region SandcastleHelpFileBuilderPath
        /// <summary>
        /// The install directory for the Sandcastle Help File Builder.
        /// </summary>
        [Output]
        public ITaskItem SandcastleHelpFileBuilderPath
        {
            get
            {
                return this.sandCastleHelpFileBuilderPath;
            }
        } 
        #endregion

        #region SandcastlePath
        /// <summary>
        /// The install directory for Sandcastle.
        /// </summary>
        [Output]
        public ITaskItem SandcastlePath
        {
            get
            {
                return this.sandCastlePath;
            }
        } 
        #endregion

        #region SystemDirectory
        /// <summary>
        /// The System directory.
        /// </summary>
        [Output]
        public ITaskItem SystemDirectory
        {
            get
            {
                return this.systemDirectory;
            }
        } 
        #endregion

        #region TempDirectory
        /// <summary>
        /// The current system's temporary folder.
        /// </summary>
        [Output]
        public ITaskItem TempDirectory
        {
            get
            {
                return this.tempDirectory;
            }
        } 
        #endregion

        #region UserInteractive
        /// <summary>
        /// Indicates if the current processor is running in user interactive mode.
        /// </summary>
        [Output]
        public bool UserInteractive
        {
            get
            {
                return this.userInteractive;
            }
            set
            {
                this.userInteractive = value;
            }
        } 
        #endregion

        #region VisualBasicPath
        /// <summary>
        /// The install directory for Visual Basic 6.0.
        /// </summary>
        [Output]
        public ITaskItem VisualBasicPath
        {
            get
            {
                return this.vbPath;
            }
        } 
        #endregion

        #region VisualStudio2003Path
        /// <summary>
        /// The install directory for Visual Studio 2003.
        /// </summary>
        [Output]
        public ITaskItem VisualStudio2003Path
        {
            get
            {
                return this.vs2003Path;
            }
        } 
        #endregion

        #region VisualStudio2005Path
        /// <summary>
        /// The install directory for Visual Studio 2005.
        /// </summary>
        [Output]
        public ITaskItem VisualStudio2005Path
        {
            get
            {
                return this.vs2005Path;
            }
        } 
        #endregion

        #region VisualStudio2008Path
        /// <summary>
        /// The install directory for Visual Studio 2008.
        /// </summary>
        [Output]
        public ITaskItem VisualStudio2008Path
        {
            get
            {
                return this.vs2008Path;
            }
        } 
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEnvironment"/> class.
        /// </summary>
        public GetEnvironment()
            : base(Properties.TaskMessages.ResourceManager, "Help_")
        {
        }
        #endregion

        #region Execute
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>Returns <see langword="true" /> if the task successfully
        /// executed; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Execute()
        {
            bool flag = true;

            try
            {
                this.currentDirectory = new TaskItem(Environment.CurrentDirectory);
                this.oSVersion = Environment.OSVersion.ToString();
                this.processorCount = Environment.ProcessorCount;
                this.systemDirectory = new TaskItem(Environment.SystemDirectory);
                this.userInteractive = Environment.UserInteractive;
                this.logicalDrives = Environment.GetLogicalDrives();
                this.applicationDataDirectory = new TaskItem(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                this.commonApplicationDataDirectory = new TaskItem(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                this.commonProgramFilesDirectory = new TaskItem(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
                this.desktopDirectory = new TaskItem(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                this.programFilesDirectory = new TaskItem(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
                this.tempDirectory = new TaskItem(Path.GetTempPath());

                fxCopFound = ToolHelpers.FindFxCop(out fxCopPath, base.Log);
                htmlHelpWorkshopFound = ToolHelpers.FindHtmlHelpWorkshop(out htmlHelpWorkshopPath, base.Log);
                ndocFound = ToolHelpers.FindNDoc(out ndocPath, base.Log);
                nunitFound = ToolHelpers.FindNUnit(out nunitPath, base.Log);
                vbFound = ToolHelpers.FindVisualBasic(out vbPath, base.Log);
                vs2003Found = ToolHelpers.FindVisualStudio2003(out vs2003Path, base.Log);
                vs2005Found = ToolHelpers.FindVisualStudio2005(out vs2005Path, base.Log);
                vs2008Found = ToolHelpers.FindVisualStudio2008(out vs2008Path, base.Log);
                sandCastleFound = ToolHelpers.FindSandcastle(out sandCastlePath, base.Log);
                sandCastleHelpFileBuilderFound = ToolHelpers.FindSandcastleHelpFileBuilder(out sandCastleHelpFileBuilderPath, base.Log);
            }
            catch (InvalidOperationException ex)
            {
                Log.LogErrorWithCodeFromResources("GetEnvironment_Error", ex.Message);
                flag = false;
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("GetEnvironment_Error", ex.Message);
                flag = false;
            }

            return flag;
        }
        #endregion

        #endregion

        #endregion
    }
}
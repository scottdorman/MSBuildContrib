//------------------------------------------------------------------------------
// MSBuildContrib
//
// Move.cs
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
using System.Collections;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Moves files on the filesystem to a new location.
    /// </summary>
    /// <remarks>
    /// Either the <see cref="DestinationFolder"/> or the 
    /// <see cref="DestinationFiles"/> parameter must be specified, but not 
    /// both. If both are specified, the task fails and an error is logged.
    /// </remarks>
    /// <example>
    /// <para>The following example moves the items in the MySourceFiles item
    /// collection into the folder c:\MyProject\Destination.</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <MySourceFiles Include="a.cs;b.cs;c.cs" />
    /// </ItemGroup>
    /// <Move SourceFiles="@(MySourceFiles)" DestinationFolder="c:\MyProject\Destination" />
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    /// <para>The following example renames a file</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <Move SourceFiles="a.cs" DestinationFiles="b.cs" />
    ///     ]]>
    ///   </code>
    /// </example>
    public class Move : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private ITaskItem[] destinationFiles;
        private ITaskItem destinationFolder;
        private ITaskItem[] movedFiles;
        private bool overwriteExistingFiles;
        private bool overwriteReadOnlyFiles;
        private bool skipUnchangedFiles;
        private ITaskItem[] sourceFiles;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region DeleteExistingFile
        private void DeleteExistingFile(string file, bool logActivity)
        {
            if (File.Exists(file))
            {
                if (logActivity)
                {                    
                    Log.LogMessageFromResources(MessageImportance.Low, "Move_RemovingExistingFile", file);
                }
                File.Delete(file);
            }
        }
        #endregion

        #region DoMoveIfNecessary
        private bool DoMoveIfNecessary(string sourceFile, string destinationFile)
        {
            bool flag = true;
            try
            {
                if (this.skipUnchangedFiles && IsMatchingSizeAndTimeStamp(sourceFile, destinationFile))
                {
                    Log.LogMessageFromResources(MessageImportance.Low, "Move_DidNotMoveBecauseOfFileMatch", sourceFile, destinationFile, "SkipUnchangedFiles", "true");
                    return flag;
                }
                if (String.Compare(sourceFile, destinationFile, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    flag = MoveFileWithLogging(sourceFile, destinationFile);
                }
            }
            catch (PathTooLongException ex)
            {
                Log.LogErrorWithCodeFromResources("Move_Error", sourceFile, destinationFile, ex.Message);
                flag = false;
            }
            catch (IOException ex)
            {
                if (PathsAreIdentical(sourceFile, destinationFile))
                {
                    return flag;
                }
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Move_Error", sourceFile, destinationFile, ex.Message);
                flag = false;
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Move_Error", sourceFile, destinationFile, ex.Message);
                flag = false;
            }
            return flag;
        } 
        #endregion

        #region InitializeDestinationFiles
        private bool InitializeDestinationFiles()
        {
            if (this.destinationFiles == null)
            {
                this.destinationFiles = new ITaskItem[this.sourceFiles.Length];
                for (int i = 0; i < this.sourceFiles.Length; i++)
                {
                    string str;
                    try
                    {
                        str = Path.Combine(this.destinationFolder.ItemSpec, Path.GetFileName(this.sourceFiles[i].ItemSpec));
                    }
                    catch (ArgumentException exception)
                    {
                        this.destinationFiles = new ITaskItem[0];
                        Log.LogErrorWithCodeFromResources("Move_Error", this.sourceFiles[i].ItemSpec, this.destinationFolder.ItemSpec, exception.Message);
                        return false;
                    }
                    this.destinationFiles[i] = new TaskItem(str);
                    this.sourceFiles[i].CopyMetadataTo(this.destinationFiles[i]);
                }
            }
            return true;
        } 
        #endregion

        #region IsMatchingSizeAndTimeStamp
        private static bool IsMatchingSizeAndTimeStamp(string sourceFile, string destinationFile)
        {
            FileInfo info = new FileInfo(sourceFile);
            FileInfo info2 = new FileInfo(destinationFile);

            if (!info2.Exists)
            {
                return false;
            }
            if (info.LastWriteTime != info2.LastWriteTime)
            {
                return false;
            }
            if (info.Length != info2.Length)
            {
                return false;
            }
            return true;
        } 
        #endregion

        #region MakeFileWriteable
        private void MakeFileWriteable(string file, bool logActivity)
        {
            if (File.Exists(file) && (FileAttributes.ReadOnly == (File.GetAttributes(file) & FileAttributes.ReadOnly)))
            {
                if (logActivity)
                {
                    Log.LogMessageFromResources(MessageImportance.Low, "Move_RemovingReadOnlyAttribute", file);
                }
                File.SetAttributes(file, FileAttributes.Normal);
            }
        }
        #endregion

        #region MoveFileWithLogging
        private bool MoveFileWithLogging(string sourceFile, string destinationFile)
        {
            if (Directory.Exists(destinationFile))
            {
                Log.LogErrorWithCodeFromResources("Move_DestinationIsDirectory", sourceFile, destinationFile);
                return false;
            }
            if (Directory.Exists(sourceFile))
            {
                Log.LogErrorWithCodeFromResources("Move_SourceIsDirectory", sourceFile);
                return false;
            }
            string directoryName = Path.GetDirectoryName(destinationFile);
            if (((directoryName != null) && (directoryName.Length > 0)) && !Directory.Exists(directoryName))
            {
                Log.LogMessageFromResources(MessageImportance.Normal, "Move_CreatesDirectory", directoryName);
                Directory.CreateDirectory(directoryName);
            }
            if (this.overwriteReadOnlyFiles)
            {
                this.MakeFileWriteable(destinationFile, true);
            }
            if (this.overwriteExistingFiles)
            {
                this.DeleteExistingFile(destinationFile, true);
            }
            Log.LogMessageFromResources(MessageImportance.Normal, "Move_FileComment", sourceFile, destinationFile);
            File.Move(sourceFile, destinationFile);
            this.MakeFileWriteable(destinationFile, false);
            return true;
        }
        #endregion

        #region PathsAreIdentical
        private static bool PathsAreIdentical(string source, string destination)
        {
            string fullPath = Path.GetFullPath(source);
            string strB = Path.GetFullPath(destination);
            return (String.Compare(fullPath, strB, StringComparison.OrdinalIgnoreCase) == 0);
        } 
        #endregion

        #region ValidateInputs
        private bool ValidateInputs()
        {
            if ((this.destinationFiles == null) && (this.destinationFolder == null))
            {
                Log.LogErrorWithCodeFromResources("Move_NeedsDestination", "DestinationFiles", "DestinationDirectory");
                return false;
            }
            if ((this.destinationFiles != null) && (this.destinationFolder != null))
            {
                Log.LogErrorWithCodeFromResources("Move_ExactlyOneTypeOfDestination", "DestinationFiles", "DestinationDirectory");
                return false;
            }
            if ((this.destinationFiles != null) && (this.destinationFiles.Length != this.sourceFiles.Length))
            {
                Log.LogErrorWithCodeFromResources("General_TwoVectorsMustHaveSameLength", this.destinationFiles.Length, this.sourceFiles.Length, "DestinationFiles", "SourceFiles");
                return false;
            }
            return true;
        } 
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region DestinationFiles
        /// <summary>
        /// Specifies the list of files to copy the source files to. This list
        /// is expected to be a one-to-one mapping with the list specified in
        /// the <see cref="SourceFiles"/> parameter. That is, the first file
        /// specified in <see cref="SourceFiles"/> will be copied to the first
        /// location specified in <see cref="DestinationFiles"/>, and so forth.
        /// </summary>
        [Output]
        public ITaskItem[] DestinationFiles
        {
            get
            {
                return this.destinationFiles;
            }
            set
            {
                this.destinationFiles = value;
            }
        } 
        #endregion

        #region DestinationFolder
        /// <summary>
        /// Specifies the directory to which you want to copy the files. This must be a directory, not a file. If the directory does not exist, it is created automatically. 
        /// </summary>
        public ITaskItem DestinationFolder
        {
            get
            {
                return this.destinationFolder;
            }
            set
            {
                this.destinationFolder = value;
            }
        } 
        #endregion

        #region MovedFiles
        /// <summary>
        /// Contains the items that were successfully moved.
        /// </summary>
        [Output]
        public ITaskItem[] MovedFiles
        {
            get
            {
                return this.movedFiles;
            }
        }
        #endregion

        #region OverwriteExistingFiles
        /// <summary>
        /// Overwrite files even if they already exist.
        /// </summary>
        public bool OverwriteExistingFiles
        {
            get
            {
                return this.overwriteExistingFiles;
            }
            set
            {
                this.overwriteExistingFiles = value;
            }
        }
        #endregion

        #region OverwriteReadOnlyFiles
        /// <summary>
        /// Overwrite files even if they are marked as read only files.
        /// </summary>
        public bool OverwriteReadOnlyFiles
        {
            get
            {
                return this.overwriteReadOnlyFiles;
            }
            set
            {
                this.overwriteReadOnlyFiles = value;
            }
        } 
        #endregion

        #region SkipUnchangedFiles
        /// <summary>
        /// If <see langword="true"/>, skips the moving of files that are 
        /// unchanged between the source and destination. The 
        /// <see cref="Move"/> task considers files to be unchanged if they
        /// have the same size and the same last modified time.
        /// </summary>
        public bool SkipUnchangedFiles
        {
            get
            {
                return this.skipUnchangedFiles;
            }
            set
            {
                this.skipUnchangedFiles = value;
            }
        } 
        #endregion

        #region SourceFiles
        /// <summary>
        /// Specifies the files to move.
        /// </summary>
        [Required]
        public ITaskItem[] SourceFiles
        {
            get
            {
                return this.sourceFiles;
            }
            set
            {
                this.sourceFiles = value;
            }
        } 
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class.
        /// </summary>
        public Move()
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
            if ((this.sourceFiles == null) || (this.sourceFiles.Length == 0))
            {
                this.destinationFiles = new TaskItem[0];
                this.movedFiles = new TaskItem[0];
                return true;
            }
            if (!this.ValidateInputs() || !this.InitializeDestinationFiles())
            {
                return false;
            }

            bool flag = true;
            ArrayList list = new ArrayList();
            for (int i = 0; i < this.sourceFiles.Length; i++)
            {
                if (this.DoMoveIfNecessary(this.sourceFiles[i].ItemSpec, this.destinationFiles[i].ItemSpec))
                {
                    this.sourceFiles[i].CopyMetadataTo(this.destinationFiles[i]);
                    list.Add(this.destinationFiles[i]);
                }
                else
                {
                    flag = false;
                }
            }
            this.movedFiles = (ITaskItem[])list.ToArray(typeof(ITaskItem));
            return flag;
        }
        #endregion

        #endregion

        #endregion
    }
}
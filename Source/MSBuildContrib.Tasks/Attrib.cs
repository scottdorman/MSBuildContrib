//------------------------------------------------------------------------------
// MSBuildContrib
//
// Attrib.cs
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
// Copyright (C) 2001-2003 Gerry Shaw ((gerry_shaw@yahoo.com)
// Copyright Chris Jenkin (oneinchhard@hotmail.com)
// 2007-December: Adapted from the NAnt AttribTask
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;
using MSBuildContrib.Utilities;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Changes the attributes of files and/or directories.
    /// </summary>
    /// <example>
    ///   <para>Make file Readonly, Hidden and System.</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <Attrib InputFiles="file1.txt" ReadOnly="true" Hidden="true" System="true" />
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>Clear Hidden and System.</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <Attrib InputFiles="file1.txt" Hidden="false" System="false" />
    ///     ]]>
    ///   </code>
    /// </example>
    public class Attrib : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private Dictionary<string, bool> attributeBag;
        private ITaskItem[] changedFiles;
        private ITaskItem[] inputFiles;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods
        
        #region CalculateAttributes
        private FileAttributes CalculateAttributes(FileAttributes currentAttributes)
        {
            if (this.attributeBag.ContainsKey("Archive"))
            {
                if (this.attributeBag["Archive"])
                {
                    currentAttributes |= FileAttributes.Archive;
                }
                else
                {
                    currentAttributes &= ~FileAttributes.Archive;
                }
            }
            if (this.attributeBag.ContainsKey("Encrypted"))
            {
                if (this.attributeBag["Encrypted"])
                {
                    currentAttributes |= FileAttributes.Encrypted;
                }
                else
                {
                    currentAttributes &= ~FileAttributes.Encrypted;
                }
            }
            if (this.attributeBag.ContainsKey("Hidden"))
            {
                if (this.attributeBag["Hidden"])
                {
                    currentAttributes |= FileAttributes.Hidden;
                }
                else
                {
                    currentAttributes &= ~FileAttributes.Hidden;
                }
            }
            if (this.attributeBag.ContainsKey("Normal"))
            {
                if (this.attributeBag["Normal"])
                {
                    currentAttributes = FileAttributes.Normal;
                }
                else
                {
                    currentAttributes &= ~FileAttributes.Normal;
                }
            }
            if (this.attributeBag.ContainsKey("ReadOnly"))
            {
                if (this.attributeBag["ReadOnly"])
                {
                    currentAttributes |= FileAttributes.ReadOnly;
                }
                else
                {
                    currentAttributes &= ~FileAttributes.ReadOnly;
                }
            }
            if (this.attributeBag.ContainsKey("System"))
            {
                if (this.attributeBag["System"])
                {
                    currentAttributes |= FileAttributes.System;
                }
                else
                {
                    currentAttributes &= ~FileAttributes.System;
                }
            }
            return currentAttributes;
        } 
        #endregion

        #region UpdateAttributes
        private bool UpdateAttributes(string path)
        {
            bool flag = true;

            try
            {
                FileSystemInfo info;
                if (FileHelpers.IsDirectory(path))
                {
                    info = new DirectoryInfo(path);
                }
                else
                {
                    info = new FileInfo(path);
                }

                FileAttributes currentAttributes = info.Attributes;
                FileAttributes proposedAttributes = CalculateAttributes(currentAttributes);

                if (currentAttributes != proposedAttributes)
                {
                    info.Attributes = proposedAttributes;
                    Log.LogMessageFromResources(MessageImportance.Normal, "Attrib_Comment", info.Attributes.ToString(), path);
                }
            }
            catch (IOException ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Attrib_Error", path, ex.Message);
                flag = false;
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Attrib_Error", path, ex.Message);
                flag = false;
            }
            return flag;
        } 
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region Archive
        /// <summary>
        /// Set the archive attribute. The default is <see langword="false" />.
        /// </summary>
        public bool Archive
        {
            get
            {
                return (this.attributeBag.ContainsKey("Archive") ? this.attributeBag["Archive"] : false);
            }
            set
            {
                this.attributeBag["Archive"] = value;
            }
        }
        #endregion

        #region ChangedFiles
        /// <summary>
        /// The files changed.
        /// </summary>
        [Output]
        public ITaskItem[] ChangedFiles
        {
            get
            {
                return this.changedFiles;
            }
        }
        #endregion

        #region Encrypted
        /// <summary>
        /// Set the encrypted attribute. The default is <see langword="false" />.
        /// </summary>
        public bool Encrypted
        {
            get
            {
                return (this.attributeBag.ContainsKey("Encrypted") ? this.attributeBag["Encrypted"] : false);
            }
            set
            {
                this.attributeBag["Encrypted"] = value;
            }
        }
        #endregion

        #region Hidden
        /// <summary>
        /// Set the hidden attribute. The default is <see langword="false" />.
        /// </summary>
        public bool Hidden
        {
            get
            {
                return (this.attributeBag.ContainsKey("Hidden") ? this.attributeBag["Hidden"] : false);
            }
            set
            {
                this.attributeBag["Hidden"] = value;
            }
        }
        #endregion

        #region InputFiles
        /// <summary>
        /// The set of files to change attributes.
        /// </summary>
        [Required]
        public ITaskItem[] InputFiles
        {
            get
            {
                return inputFiles;
            }
            set
            {
                inputFiles = value;
            }
        }
        #endregion

        #region Normal
        /// <summary>
        /// Set the normal attribute. The default is <see langword="false" />.
        /// </summary>
        /// <remarks>This attribute is only valid if used alone.</remarks>
        public bool Normal
        {
            get
            {
                return (this.attributeBag.ContainsKey("Normal") ? this.attributeBag["Normal"] : false);
            }
            set
            {
                this.attributeBag["Normal"] = value;
            }
        }
        #endregion

        #region ReadOnly
        /// <summary>
        /// Set the read-only attribute. The default is <see langword="false" />.
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return (this.attributeBag.ContainsKey("ReadOnly") ? this.attributeBag["ReadOnly"] : false);
            }
            set
            {
                this.attributeBag["ReadOnly"] = value;
            }
        }
        #endregion

        #region System
        /// <summary>
        /// Set the system attribute. The default is <see langword="false" />.
        /// </summary>
        public bool System
        {
            get
            {
                return (this.attributeBag.ContainsKey("System") ? this.attributeBag["System"] : false);
            }
            set
            {
                this.attributeBag["System"] = value;
            }
        }
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Attrib"/> class.
        /// </summary>
        public Attrib()
            : base(Properties.TaskMessages.ResourceManager, "Help_")
        {
            attributeBag = new Dictionary<string, bool>();
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
            if ((this.inputFiles == null) || (this.inputFiles.Length == 0))
            {
                this.changedFiles = new TaskItem[0];
                return true;
            }

            bool flag = true;
            ArrayList list = new ArrayList();
            for (int i = 0; i < this.inputFiles.Length; i++)
            {
                if (this.UpdateAttributes(this.inputFiles[i].ItemSpec))
                {
                    list.Add(this.inputFiles[i]);
                }
                else
                {
                    flag = false;
                }
            }
            this.changedFiles = (ITaskItem[])list.ToArray(typeof(ITaskItem));
            return flag;
        }
        #endregion

        #endregion

        #endregion
    }
}
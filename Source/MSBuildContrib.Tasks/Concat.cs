//------------------------------------------------------------------------------
// MSBuildContrib
//
// Concat.cs
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
// Copyright (C) 2002 Tomas Restrepo (tomasr@mvps.org)
// 2007-December: Adapted from the NAntContrib ConcatTask
//------------------------------------------------------------------------------
using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Concatenates a set of files.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This task takes a set of input files in a fileset
    /// and concatenates them into a single file. You can 
    /// either replace the output file, or append to it 
    /// by using the append attribute.
    /// </para>
    /// <para>
    /// The order the files are concatenated in is not
    /// especified.
    /// </para>
    /// </remarks>
    /// <example>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <ChecksumFiles Include="**\*.txt" />
    /// </ItemGroup>
    /// <Concat InputFiles="@(CodeStatsItems)" Destination="Full.txt" Append="true" />
    ///     ]]>
    ///   </code>
    /// </example>
    public class Concat : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private const int BlockSize = 64 * 1024;

        private bool append;
        private ITaskItem destination;
        private ITaskItem[] inputFiles;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region AppendFile
        private bool AppendFile(string path, FileStream output)
        {
            bool flag = true;
            try
            {
                byte[] buffer = new byte[BlockSize];
                int bytesRead = 0;

                using (FileStream input = File.OpenRead(path))
                {
                    while ((bytesRead = input.Read(buffer, 0, BlockSize)) != 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Log.LogErrorWithCodeFromResources("Concat_Error", path, output.Name, ex.Message);
                flag = false;
            }
            catch (IOException ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Concat_Error", path, output.Name, ex.Message);
                flag = false;
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Concat_Error", path, output.Name, ex.Message);
                flag = false;
            } 
            return flag;
        } 
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region Append
        /// <summary>
        /// Name of Algorithm to use when calculating
        /// the Concat. Can be MD5 or SHA1.
        /// </summary>
        public bool Append
        {
            get
            {
                return this.append;
            }
            set
            {
                this.append = value;
            }
        }
        #endregion

        #region Destination
        /// <summary>
        /// Name of the destination file.
        /// </summary>
        [Required]
        public ITaskItem Destination
        {
            get
            {
                return this.destination;
            }
            set
            {
                this.destination = value;
            }
        }
        #endregion

        #region InputFiles
        /// <summary>
        /// The set of files to concatenate.
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

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Concat"/> class.
        /// </summary>
        public Concat()
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
            if ((this.inputFiles == null) || (this.inputFiles.Length == 0))
            {
                return true;
            }

            bool flag = true;
            FileMode mode = (this.append) ? FileMode.Append | FileMode.OpenOrCreate : FileMode.Create;

            try
            {
                using (FileStream output = File.Open(this.destination.ItemSpec, mode))
                {
                    for (int i = 0; i < inputFiles.Length; i++)
                    {
                        flag &= AppendFile(inputFiles[i].ItemSpec, output);
                    }
                }
            }
            catch (PathTooLongException ex)
            {
                Log.LogErrorWithCodeFromResources("Concat_FailedToCreateOutputFile", this.destination.ItemSpec, ex.Message);
                flag = false;
            }
            catch (FileNotFoundException ex)
            {
                Log.LogErrorWithCodeFromResources("Concat_FailedToCreateOutputFile", this.destination.ItemSpec, ex.Message);
                flag = false;
            }
            catch (IOException ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Concat_FailedToCreateOutputFile", this.destination.ItemSpec, ex.Message);
                flag = false;
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Concat_FailedToCreateOutputFile", this.destination.ItemSpec, ex.Message);
                flag = false;
            } 

            return flag;
        }
        #endregion

        #endregion

        #endregion
    }
}
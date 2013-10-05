//------------------------------------------------------------------------------
// MSBuildContrib
//
// Checksum.cs
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
// 2007-December: Adapted from the NAntContrib ChecksumTask
//------------------------------------------------------------------------------
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Calculates checksums for a set of files.
    /// </summary>
    /// <remarks>
    /// This task takes a set of input files in a fileset
    /// and calculates a checksum for each one of them. 
    /// You can specify the algorithm to use when calculating
    /// the checksum value (MD5 or SHA1, for example).
    /// The calculated value is saved to a file with the same
    /// name as the input file and an added extension either
    /// based on the algorithm name (e.g. .MD5), or whatever 
    /// is specified through the fileext a
    /// </remarks>
    /// <example>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <ChecksumFiles Include="**\*.dll" />
    /// </ItemGroup>
    /// <Checksum InputFiles="@(CodeStatsItems)" Algorithm="md5" Extension="MD5" />
    ///     ]]>
    ///   </code>
    /// </example>
    public class Checksum : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private ChecksumAlgorithms algorithm;
        private string extension;
        private ITaskItem[] inputFiles;
        private HashAlgorithm provider;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region CalculateChecksum
        private bool CalculateChecksum(string path)
        {
            bool flag = true;

            try
            {
                string outputFile = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", path, this.extension);
                byte[] checksum;
                using (FileStream stream = File.OpenRead(path))
                {
                    checksum = provider.ComputeHash(stream);
                }

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < checksum.Length; i++)
                {
                    builder.Append(String.Format(CultureInfo.InvariantCulture, "{0:x2}", checksum[i]));
                }
                File.WriteAllText(outputFile, builder.ToString());
            }
            catch (FileNotFoundException ex)
            {
                Log.LogErrorWithCodeFromResources("Checksum_Error", path, ex.Message);
                flag = false;
            }
            catch (IOException ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Checksum_Error", path, ex.Message);
                flag = false;
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Checksum_Error", path, ex.Message);
                flag = false;
            }
            return flag;
        } 
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region Algorithm
        /// <summary>
        /// The name of the algorithm to use when calculating the checksum.
        /// </summary>
        /// <remarks>Valid algorithms are MD5 or SHA1.
        /// </remarks>
        public string Algorithm
        {
            get
            {
                return this.algorithm.ToString();
            }
            set
            {
                if (!Enum.IsDefined(typeof(ChecksumAlgorithms), value))
                {
                    throw new ArgumentOutOfRangeException("value", String.Format(CultureInfo.CurrentUICulture, Properties.Resources.InvalidEnumValue, value));
                }
                this.algorithm = (ChecksumAlgorithms)Enum.Parse(typeof(ChecksumAlgorithms), value);
            }
        }
        #endregion

        #region Extension
        /// <summary>
        /// The generated checksum file's name will be the original filename
        /// with "." and <see cref="Extension"/> added to it.
        /// </summary>
        /// <remarks>The default is to use the name of the 
        /// <see cref="Algorithm"/>.
        /// </remarks>
        public string Extension
        {
            get
            {
                return this.extension;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    this.extension = value;
                }
            }
        }
        #endregion

        #region InputFiles
        /// <summary>
        /// The set of files used to compute checksums.
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
        /// Initializes a new instance of the <see cref="Checksum"/> class.
        /// </summary>
        public Checksum()
            : base(Properties.TaskMessages.ResourceManager, "Help_")
        {
            this.algorithm = ChecksumAlgorithms.MD5;
            this.extension = this.algorithm.ToString();
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
            provider = HashAlgorithm.Create(this.algorithm.ToString());

            for (int i = 0; i < inputFiles.Length; i++)
            {
                flag &= CalculateChecksum(inputFiles[i].ItemSpec);
            }

            return flag;
        }
        #endregion

        #endregion

        #endregion
    }
}
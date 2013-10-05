//------------------------------------------------------------------------------
// MSBuildContrib
//
// CheckDiskspace.cs
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Reports any local fixed disks that are less than the minimum available
    /// space.
    /// </summary>
    /// <example>
    /// <para>Reports any disks that are less than 3GB (default size).</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <CheckDiskspace />
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    /// <para>Reports any disks that are less than 5GB.</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <CheckDiskspace MinimumDiskspace="5368709120" />
    ///     ]]>
    ///   </code>
    /// </example>
    /// <remarks>When any disk falls below 1GB, an error is reported.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diskspace")]
    public class CheckDiskspace : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private const long OneGB = 1073741824;

        private long minimumDiskspace;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods
        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region MinimumDiskspace
        /// <summary>
        /// The minimum amount of disk space (in bytes) available before a warning is issued.
        /// </summary>
        /// <remarks>The default is 3GB.</remarks>
        [Required]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diskspace")]
        public long MinimumDiskspace
        {
            get
            {
                return minimumDiskspace;
            }
            set
            {
                this.minimumDiskspace = value;
            }
        }
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDiskspace"/> class.
        /// </summary>
        public CheckDiskspace()
            : base(Properties.TaskMessages.ResourceManager, "Help_")
        {
            this.minimumDiskspace = OneGB * 3;
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
            bool success = true;
            foreach (string drive in Environment.GetLogicalDrives())
            {
                DriveInfo driveInfo = new DriveInfo(drive);
                if (driveInfo.DriveType == DriveType.Fixed)
                {
                    if (driveInfo.IsReady)
                    {
                        Log.LogMessageFromResources(MessageImportance.Low, "CheckDiskspace_Message", drive, driveInfo.AvailableFreeSpace / OneGB, driveInfo.TotalSize / OneGB, driveInfo.TotalFreeSpace / OneGB);
                        if (driveInfo.AvailableFreeSpace < OneGB)
                        {
                            Log.LogErrorWithCodeFromResources("CheckDiskspace_LowDiskspaceCriticalMesssage", drive, driveInfo.AvailableFreeSpace / OneGB, driveInfo.TotalSize / OneGB);
                            success &= false;
                            continue;
                        }
                        if (driveInfo.AvailableFreeSpace < this.minimumDiskspace)
                        {
                            Log.LogWarningWithCodeFromResources("CheckDiskspace_LowDiskspaceMesssage", drive, driveInfo.AvailableFreeSpace / OneGB);
                        }
                    }
                    else
                    {
                        Log.LogErrorFromResources("CheckDiskspace_DriveNotReadyError", drive);
                        success &= false;
                    }
                }
            }
            return success;
        }
        #endregion

        #endregion

        #endregion
    }
}
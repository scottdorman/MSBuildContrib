//------------------------------------------------------------------------------
// MSBuildContrib
//
// FileHelpers.cs
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
using System.IO;
using System;

namespace MSBuildContrib.Utilities
{
    /// <summary>
    /// Groups a set of useful file related helper methods.
    /// </summary>
    public static class FileHelpers
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

        #region IsDirectory
        /// <summary>
        /// Indicates if the specified path is a directory or a file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(string path)
        {
            bool isDirectory = false;

            if (String.CompareOrdinal(Path.GetPathRoot(path), path) == 0)
            {
                // The path we were given was a drive root, so treat it as a directory;
                isDirectory = true;
            }
            else
            {
                if (String.CompareOrdinal(Path.GetDirectoryName(path), path) == 0)
                {
                    // The path we were given is the same as the directory name, so
                    // treat it as a directory;
                    isDirectory = true;
                }
            }
            return isDirectory;
        }
        #endregion

        #region SetReadOnly
        /// <summary>
        /// Sets or clears the read-only attribute on the specified file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="makeReadOnly">Set or clear the read-only attribute.</param>
        public static void SetReadOnly(string path, bool makeReadOnly)
        {
            FileAttributes attributes = File.GetAttributes(path);

            if (makeReadOnly)
            {
                File.SetAttributes(path, attributes | FileAttributes.ReadOnly);
            }
            else
            {
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                }
            }
        } 
        #endregion

        #endregion

        #endregion
    }
}
//------------------------------------------------------------------------------
// MSBuildContrib
//
// PathHelpers.cs
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
using System.Collections.Generic;
using System.IO;

namespace MSBuildContrib.Utilities
{
    /// <summary>
    /// Groups a set of useful path related helper methods.
    /// </summary>
    public static class PathHelpers
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

        #region CombinePaths
        /// <summary>
        /// Combines multiple path strings.
        /// </summary>
        /// <param name="paths">The array of strings to combine.</param>
        /// <returns>A string containing the combined paths. If one of the specified paths is a zero-length string,
        /// this method returns the other path.</returns>
        public static string CombinePaths(params string[] paths)
        {
            string returnPath = String.Empty;
            if (paths.Length == 1)
            {
                returnPath = paths[0];
            }
            else if (paths.Length == 2)
            {
                returnPath = Path.Combine(paths[0], paths[1]);
            }
            else
            {
                returnPath = Path.Combine(paths[0], paths[1]);

                int count = 2;
                while (count < paths.Length)
                {
                    returnPath = Path.Combine(returnPath, paths[count++]);
                }
            }

            return returnPath;
        }
        #endregion

        #region RelativePathTo
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromDirectory">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string RelativePathTo(string fromDirectory, string toPath)
        {
            if (fromDirectory == null)
            {
                throw new ArgumentNullException("fromDirectory");
            }

            if (toPath == null)
            {
                throw new ArgumentNullException("toPath");
            }

            if (fromDirectory.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.Argument_StringZeroLength, "fromDirectory");
            }

            if (toPath.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.Argument_StringZeroLength, "toPath");
            }

            bool isRooted = Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(toPath);

            if (isRooted)
            {
                bool isDifferentRoot = String.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), StringComparison.OrdinalIgnoreCase) != 0;

                if (isDifferentRoot)
                {
                    //throw new ArgumentException(string.Format("The paths '{0} and '{1}' have different path roots.", fromDirectory, toPath));
                    return toPath;
                }
            }

            List<string> relativePath = new List<string>();
            string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);
            string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);
            int length = Math.Min(fromDirectories.Length, toDirectories.Length);
            int lastCommonRoot = -1;

            // find common root
            for (int x = 0; x < length; x++)
            {
                if (String.Compare(fromDirectories[x], toDirectories[x], StringComparison.OrdinalIgnoreCase) != 0)
                {
                    break;
                }
                lastCommonRoot = x;
            }

            if (lastCommonRoot == -1)
            {
                //throw new ArgumentException(string.Format("The paths '{0} and '{1}' do not have a common prefix path.", fromDirectory, toPath));
                return toPath;
            }

            // add relative folders in from path
            for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
            {
                if (fromDirectories[x].Length > 0)
                {
                    relativePath.Add("..");
                }
            }

            // add to folders to path
            for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
            {
                relativePath.Add(toDirectories[x]);
            }

            // create relative path
            return String.Join(Path.DirectorySeparatorChar.ToString(), relativePath.ToArray()); ;
        }
        #endregion

        #endregion

        #endregion
    }
}
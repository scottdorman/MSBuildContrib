//------------------------------------------------------------------------------
// MSBuildContrib
//
// Helpers.cs
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
using System.Globalization;

namespace MSBuildContrib.Utilities
{
    /// <summary>
    /// Groups a set of useful helper methods.
    /// </summary>
    public static class Helpers
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

        #region InvariantToString
        /// <summary>
        /// Converts the numeric value of this instance to it's equivalent
        /// string representation using the Invariant Culture.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string InvariantToString(this int source)
        {
            return source.ToString(CultureInfo.InvariantCulture);
        } 
        #endregion

        #endregion

        #endregion
    }
}
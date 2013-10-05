//------------------------------------------------------------------------------
// MSBuildContrib
//
// MatchCollection.cs
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
// Copyright (C) 2004 Manfred Doetter (mdoetter@users.sourceforge.net)
// 2007-December: Adapted from the NAntContrib GrepTask
//------------------------------------------------------------------------------
using System.Collections.ObjectModel;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    ///  A strongly-typed collection of <see cref="Match"/> instances.
    /// </summary>
    internal class MatchCollection : Collection<Match>
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

        #region Add
        /// <summary>
        /// Adds all <see cref="Match"/> instances <paramref name="matches" />
        /// to this collection.
        /// </summary>
        /// <param name="matches">Collection of <see cref="Match" /> instances to add.</param>
        public void Add(MatchCollection matches)
        {
            foreach (Match match in matches)
            {
                this.Add(match);
            }
        }
        #endregion

        #endregion

        #endregion
    }
}
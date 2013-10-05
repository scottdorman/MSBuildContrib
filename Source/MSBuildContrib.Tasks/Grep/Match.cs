//------------------------------------------------------------------------------
// MSBuildContrib
//
// Match.cs
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
using System;
using System.Collections;
using System.Xml;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Encapsulation of a match of a regular-expression with the
    /// associated named capture-groups.
    /// </summary>
    internal class Match
    {
        #region events
        #endregion

        #region class-wide fields
        private Hashtable values = new Hashtable();
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods
        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region indexer
        /// <summary>
        /// Gets or sets the value for a capture group.
        /// </summary>
        public string this[string name]
        {
            get
            {
                return (string)values[name];
            }
            set
            {
                values[name] = value;
            }
        }
        #endregion

        #endregion

        #region methods

        #region WriteXml
        /// <summary>
        /// Writes this match to an <see cref="XmlWriter" />.
        /// </summary>
        /// <param name="xmlWriter">The <see cref="XmlWriter" /> to write to.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Match");
            foreach (string groupName in values.Keys)
            {
                xmlWriter.WriteElementString(groupName, this[groupName]);
            }
            xmlWriter.WriteEndElement();
        }
        #endregion

        #endregion

        #endregion
    }
}
//------------------------------------------------------------------------------
// MSBuildContrib
//
// Pattern.cs
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MSBuildContrib.Utilities;

using RegexMatch = System.Text.RegularExpressions.Match;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Encapsulation of a search pattern.
    /// </summary>
    internal class Pattern
    {
        #region events
        #endregion

        #region class-wide fields
        private readonly Regex scanner;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region ConcatenateCaptures
        /// <summary>
        /// Concatenates the captures of <paramref name="group" /> to a string.
        /// </summary>
        /// <param name="group"><see cref="Group" /> containing the captures.</param>
        /// <returns>
        /// <see cref="string" /> containg the concatenated captures.
        /// </returns>
        /// <remarks>
        /// A named-group can captured multiple times, when the regular
        /// expression has a quantifier, e.g. (// (?'Text'.*) )* will match
        /// multiline comments with group <i>Text</i> having a capture for
        /// every line.
        /// </remarks>
        private static string ConcatenateCaptures(Group group)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Capture capture in group.Captures)
            {
                sb.Append(capture.Value);
            }
            return sb.ToString();
        }
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties
        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Pattern" /> class from 
        /// a regular-expression.
        /// </summary>
        /// <param name="regex">The regular-expression.</param>
        public Pattern(string regex)
        {
            scanner = new Regex(regex);
        }
        #endregion

        #region Extract
        /// <summary>
        /// Extracts the matches of this pattern from <paramref name="source" />.
        /// </summary>
        /// <param name="fileName">The name of the file associated with <paramref name="source" />.</param>
        /// <param name="source">The source string</param>
        /// <returns>
        /// A collection of found matches.
        /// </returns>
        public MatchCollection Extract(string fileName, string source)
        {
            MatchCollection resultMatches = new MatchCollection();
            LineCounter lineCounter = new LineCounter(source);

            RegexMatch regexMatch = scanner.Match(source);
            while (regexMatch.Success)
            {
                Match match = new Match();
                match["Path"] = Path.GetDirectoryName(fileName);
                match["File"] = Path.GetFileName(fileName);
                match["LineNumber"] = lineCounter.CountTo(regexMatch.Index).InvariantToString();
                foreach (string groupName in scanner.GetGroupNames())
                {
                    // ignore default-names like '0', '1' ... as produced
                    // by the Regex class
                    if (Char.IsLetter(groupName[0]) || (groupName[0] == '_'))
                    {
                        match[groupName] = ConcatenateCaptures(regexMatch.Groups[groupName]);
                    }
                }
                resultMatches.Add(match);
                regexMatch = regexMatch.NextMatch();
            }
            return resultMatches;
        }
        #endregion

        #endregion

        #endregion
    }
}
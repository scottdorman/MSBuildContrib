//------------------------------------------------------------------------------
// MSBuildContrib
//
// LineCounter.cs
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
using System.Text.RegularExpressions;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// This purpose of this class is to get the line-numbers within 
    /// a string for a specific position of a character 
    /// (an index, as returned by the <see cref="Regex" /> class).
    /// </summary>
    internal class LineCounter
    {
        #region events
        #endregion

        #region class-wide fields
        private int currentLine;
        private int currentPos;
        private string line;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region Count
        /// <summary>
        /// Counts the number of occurences of <paramref name="c" /> in the 
        /// range from <paramref name="start" /> to <paramref name="end" /> in 
        /// string <paramref name="str" />.
        /// </summary>
        /// <param name="str"><see cref="string" /> to count in.</param>
        /// <param name="c">Character to count.</param>
        /// <param name="start">Start of range.</param>
        /// <param name="end">End of range.</param>
        /// <returns>
        /// The number of occurences of <paramref name="c" /> in the range from 
        /// <paramref name="start" /> to <paramref name="end" /> in string 
        /// <paramref name="str" />.
        /// </returns>
        private static int Count(string str, char c, int start, int end)
        {
            int lines = 0;
            for (int i = start; i < end; i++)
            {
                if (str[i] == c)
                {
                    lines++;
                }
            }
            return lines;
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
        /// Constructs a line-counter for a <see cref="string" />.
        /// </summary>
        /// <param name="line"><see cref="string" /> for which lines are counted.</param>
        public LineCounter(string line)
        {
            this.line = line;
            this.currentLine = 1;
        }
        #endregion

        #region CountTo
        /// <summary>
        /// Counts the line-numbers until the position <paramref name="pos" />
        /// is reached.
        /// </summary>
        /// <param name="pos">Index into the string given during construction </param>
        /// <returns>
        /// The number of lines.
        /// </returns>
        public int CountTo(int pos)
        {
            if (currentPos <= pos)
            {
                currentLine += Count(line, '\n', currentPos, pos);
            }
            else
            {
                currentLine -= Count(line, '\n', pos, currentPos);
            }
            currentPos = pos;
            return currentLine;
        }
        #endregion

        #endregion

        #endregion
    }
}
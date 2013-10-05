//------------------------------------------------------------------------------
// MSBuildContrib
//
// FileCodeCountInfo.cs
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
// Copyright (C) 2003 Brant Carter (brantcarter@hotmail.com)
// 2007-December: Adapted from the NAntContrib CodeStatsTask
//------------------------------------------------------------------------------
namespace MSBuildContrib.Common
{
    internal struct FileCodeCountInfo
    {
        #region events
        #endregion

        #region class-wide fields
        private int lineCount;
        private int commentLineCount;
        private int emptyLineCount;
        private string fileName;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods
        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region CommentLineCount
        public int CommentLineCount
        {
            get
            {
                return commentLineCount;
            }
        } 
        #endregion

        #region EmptyLineCount
        public int EmptyLineCount
        {
            get
            {
                return emptyLineCount;
            }
        } 
        #endregion

        #region FileName
        public string FileName
        {
            get
            {
                return fileName;
            }
        } 
        #endregion

        #region LineCount
        public int LineCount
        {
            get
            {
                return lineCount;
            }
        }
        #endregion
        
        #endregion

        #region methods

        #region constructor
        public FileCodeCountInfo(string fileName, int lineCount, int commentLineCount, int emptyLineCount)
        {
            this.fileName = fileName;
            this.lineCount = lineCount;
            this.commentLineCount = commentLineCount;
            this.emptyLineCount = emptyLineCount;
        }
        #endregion

        #endregion

        #endregion
    }
}

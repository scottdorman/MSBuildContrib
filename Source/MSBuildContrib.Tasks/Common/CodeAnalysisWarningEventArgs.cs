//------------------------------------------------------------------------------
// MSBuildContrib
//
// CodeAnalysisWarningEventArgs.cs
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
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using MSBuildContrib.Tasks;

namespace MSBuildContrib.Common
{
    [Serializable, ComVisible(true)]
    internal class CodeAnalysisWarningEventArgs : BuildWarningEventArgs, ICodeAnalysisEventArgs
    {
        #region events
        #endregion

        #region class-wide fields
        private string xml;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods
        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region IsErrorEvent
        public bool IsErrorEvent
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Xml
        public string Xml
        {
            get
            {
                return this.xml;
            }
        }
        #endregion

        #endregion

        #region methods

        #region constructors

        #region CodeAnalysisWarningEventArgs()
        public CodeAnalysisWarningEventArgs()
        {
        }
        #endregion

        #region CodeAnalysisWarningEventArgs(string subcategory, string code, string file, int lineNumber, string message, string helpKeyword, string xml)
        public CodeAnalysisWarningEventArgs(string subcategory, string code, string file, int lineNumber, string message, string helpKeyword, string xml)
            : base(subcategory, code, file, lineNumber, 0, 0, 0, message, helpKeyword, typeof(FxCop).FullName)
        {
            this.xml = xml;
        }
        #endregion

        #endregion

        #endregion

        #endregion
    }
}
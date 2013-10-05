//------------------------------------------------------------------------------
// MSBuildContrib
//
// ExceptionHandling.cs
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
using System.Reflection;
using System.Security;

namespace MSBuildContrib.Common
{
    internal static class ExceptionHandling
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

        #region IsCriticalException
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsCriticalException(Exception e)
        {
            return (((e is StackOverflowException) || (e is OutOfMemoryException)) || ((e is ExecutionEngineException) || (e is AccessViolationException)));
        }
        #endregion

        #region NotExpectedException
        public static bool NotExpectedException(Exception e)
        {
            return (((!(e is UnauthorizedAccessException) && !(e is ArgumentNullException)) && (!(e is PathTooLongException) && !(e is DirectoryNotFoundException))) && ((!(e is NotSupportedException) && !(e is ArgumentException)) && (!(e is SecurityException) && !(e is IOException))));
        }
        #endregion

        #region NotExpectedReflectionException
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool NotExpectedReflectionException(Exception e)
        {
            return ((((!(e is TypeLoadException) && !(e is MethodAccessException)) && (!(e is MissingMethodException) && !(e is MemberAccessException))) && ((!(e is BadImageFormatException) && !(e is ReflectionTypeLoadException)) && (!(e is CustomAttributeFormatException) && !(e is TargetParameterCountException)))) && (((!(e is InvalidCastException) && !(e is AmbiguousMatchException)) && (!(e is InvalidFilterCriteriaException) && !(e is TargetException))) && (!(e is MissingFieldException) && NotExpectedException(e))));
        }
        #endregion

        #endregion

        #endregion
    }
}
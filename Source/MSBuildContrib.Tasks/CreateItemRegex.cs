//------------------------------------------------------------------------------
// MSBuildContrib
//
// CreateItemRegex.cs
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
// 2008-January: Adapted from the NAnt Regex
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Creates an item based on the evaluation of a regular expression. The named groups
    /// in the regular expression are created as item metadata.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="Pattern" /> attribute must contain one or more 
    /// <see href="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpcongroupingconstructs.asp">
    /// named grouping constructs</see>, which represents the names of the 
    /// properties to be set. These named grouping constructs can be enclosed 
    /// by angle brackets (?&lt;name&gt;) or single quotes (?'name').
    /// </para>
    /// <note>
    /// In the build file, use the XML element <![CDATA[&lt;]]> to specify &lt;, 
    /// and <![CDATA[&gt;]]> to specify &gt;.
    /// </note>
    /// <note>
    /// The named grouping construct must not contain any punctuation and it 
    /// cannot begin with a number.
    /// </note>
    /// </remarks>
    /// <example>
    ///   <para>
    ///   Find the last word in the given string and stores it in the property 
    ///   <c>lastword</c>.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <CreateItemRegex Pattern="(?'lastword'\w+)$" Input="This is a test sentence">
    ///    <Output TaskParameter="Item" ItemName="lastword" />
    /// </CreateItemRegex>
    /// <Message Text="@(lastword)"/>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>
    ///   Split the full filename and extension of a filename.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <CreateItemRegex Pattern="^(?'name'.*)\.(?'ext'\w+)$" Input="d:\Temp\SomeDir\SomeDir\bla.xml">
    ///    <Output TaskParameter="Item" ItemName="RegexProperties" />
    /// </CreateItemRegex>
    /// <Message Text="%(RegexProperties.name)"/>
    /// <Message Text="%(RegexProperties.ext)"/>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>
    ///   Split the path and the filename. (This checks for <c>/</c> or <c>\</c> 
    ///   as the path separator).
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <CreateItemRegex Pattern="^(?'path'.*(\\|/)|(/|\\))(?'file'.*)$" Input="d:\Temp\SomeDir\SomeDir\bla.xml">
    ///    <Output TaskParameter="Item" ItemName="RegexProperties" />
    /// </CreateItemRegex>
    /// <Message Text="%(RegexProperties.path)"/>
    /// <Message Text="%(RegexProperties.file)"/>
    ///     ]]>
    ///   </code>
    ///   <para>
    ///   Results in path=<c>d:\Temp\SomeDir\SomeDir\</c> and file=<c>bla.xml</c>.
    ///   </para>
    /// </example>
    public class CreateItemRegex : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private string input;
        private ITaskItem item;
        private RegexOptions options = RegexOptions.None;
        private string pattern; 

        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods
        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region Input
        /// <summary>
        /// The input for the regular expression.
        /// </summary>
        [Required]
        public string Input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
            }
        }
        #endregion

        #region Item
        /// <summary>
        /// The item that contains the resulting metadata from
        /// the regular expression.
        /// </summary>
        [Output]
        public ITaskItem Item
        {
            get
            {
                return this.item;
            }
        }
        #endregion
        
        #region Options
        /// <summary>
        /// A comma separated list of options to pass to the regex engine. The
        /// default is <see cref="RegexOptions.None" />.
        /// </summary>
        public RegexOptions Options
        {
            get
            {
                return options;
            }
            set
            {
                options = value;
            }
        }
        #endregion

        #region Pattern
        /// <summary>
        /// The regular expression to be evalued.
        /// </summary>
        /// <remarks>
        /// The pattern must contain one or more named constructs, which may 
        /// not contain any punctuation and cannot begin with a number.
        /// </remarks>
        [Required]
        public string Pattern
        {
            get
            {
                return pattern;
            }
            set
            {
                pattern = value;
            }
        }
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateItemRegex"/> class.
        /// </summary>
        public CreateItemRegex()
            : base(MSBuildContrib.Tasks.Properties.TaskMessages.ResourceManager, "Help_")
        {
        }
        #endregion

        #region Execute
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>Returns <see langword="true" /> if the task successfully
        /// executed; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Execute()
        {
            bool success = true;
            Regex regex = null;

            try
            {
                regex = new Regex(this.pattern, this.options);
            }
            catch (ArgumentException ex)
            {
                Log.LogErrorWithCodeFromResources("CreateItemRegex_InvalidPattern", this.pattern, ex.Message);
                success = false;
            }

            RegexMatch match = regex.Match(Input);

            if (match == RegexMatch.Empty)
            {
                Log.LogErrorWithCodeFromResources("CreateItemRegex_NoMatchFound", this.pattern, this.input);
                success = false;
            }
            else
            {
                // we start the iteration at 1, since the collection of groups 
                // always starts with a group which matches the entire input and 
                // is named '0', this group is of no interest to us
                if (match.Groups.Count == 2)
                {
                    //Log.LogMessageFromResources(MessageImportance.Low, "CreatePropertyRegex_Message", groupName, match.Groups[groupName].Value);
                    this.item = new TaskItem(match.Groups[1].Value);
                }
                else
                {
                    this.item = new TaskItem(this.input);
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        string groupName = regex.GroupNameFromNumber(i);
                        Log.LogMessageFromResources(MessageImportance.Low, "CreateItemRegex_Message", groupName, match.Groups[groupName].Value);
                        item.SetMetadata(groupName, match.Groups[groupName].Value);
                    }
                }
            }
            return success;
        }
        #endregion

        #endregion

        #endregion

    }
}
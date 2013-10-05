using System;
//------------------------------------------------------------------------------
// MSBuildContrib
//
// Grep.cs
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Searches files for a regular-expression and produces an XML report of 
    /// the matches.
    /// </summary>
    /// <example>
    ///     <para>
    ///         Extract all <i>TODO:</i>, <i>UNDONE:</i> or <i>HACK:</i>-
    ///         comment-tags from C# source files and write them to a file
    ///         <i>out.xml</i>. (An xslt-stylesheet could then transform it to
    ///         a nice html-page for your project-homepage, but that is beyond
    ///         the scope of this example.) 
    ///     </para>
    ///     <para>
    ///         <i>Path</i>, <i>File</i> and <i>LineNumber</i> are automatically
    ///         generated elements.
    ///     </para>
    ///     <code lang="MSBuild">
    ///         <![CDATA[
    /// <ItemGroup>
    ///    <CommentScrubItems Include="**\*.cs" />
    /// </ItemGroup>
    /// <Grep InputFiles="@(CommentScrubItems)" OutputFile="out.xml" Pattern="// (?'Type'TODO|UNDONE|HACK): (\[(?'Author'\w*),(?'Date'.*)\])? (?'Text'[^\n\r]*)"  />
    ///         ]]>
    ///     </code>
    ///     <para>
    ///         The resulting XML file for a comment-tag  
    ///         'TODO: [md, 14-02-2004] comment this method'
    ///         will look like
    ///     </para>
    ///     <code lang="MSBuild">
    ///         <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?> 
    /// <Matches>
    ///     <Match>
    ///         <Type>TODO</Type> 
    ///         <Text>comment this method</Text> 
    ///         <Path>C:\MyProjects\MyPath</Path>
    ///         <File>MyFile.cs</Filename> 
    ///         <LineNumber>146</LineNumber> 
    ///         <Author>md</Author>
    ///         <Date>14-02-2004</Date>
    ///     </Match>
    ///     ...
    /// </Matches>
    ///         ]]>
    ///     </code> 
    /// </example>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Grep")]
    public class Grep : Task
    {
        #region events
        private delegate void WriteMethod(MatchCollection matches, StreamWriter writer);
        #endregion

        #region class-wide fields
        private ITaskItem[] inputFiles;
        private ITaskItem outputFile;
        private string pattern;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region WriteMatches
        /// <summary>
        /// Writes the specified matches to <see cref="OutputFile" />.
        /// </summary>
        /// <param name="matches">The collection of matches to write.</param>
        private void WriteMatches(MatchCollection matches)
        {
            WriteMethod writeMethod = new WriteMethod(this.WriteXml);
            using (StreamWriter writer = new StreamWriter(outputFile.ItemSpec))
            {
                writeMethod(matches, writer);
            }
        }
        #endregion

        #region WriteXml
        /// <summary>
        /// Writes the collection of matches to the specified <see cref="StreamWriter" />
        /// in XML format.
        /// </summary>
        /// <param name="matches">The matches to write.</param>
        /// <param name="writer"><see cref="StreamWriter" /> to write the matches to.</param>
        private void WriteXml(MatchCollection matches, StreamWriter writer)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Matches");
            foreach (Match match in matches)
            {
                match.WriteXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
        }
        #endregion

        #endregion

        #endregion

        #region public properties and methods
        
        #region properties
        
        #region InputFiles
        /// <summary>
        /// The set of files in which the expression is searched.
        /// </summary>
        public ITaskItem[] InputFiles
        {
            get { return inputFiles; 
            }
            set { inputFiles = value; 
            }
        }
        #endregion

        #region OutputFile
        /// <summary>
        /// Specifies the name of the output file.
        /// </summary>
        public ITaskItem OutputFile
        {
            get
            {
                return outputFile;
            }
            set
            {
                outputFile = value;
            }
        }
        #endregion

        #region Pattern
        /// <summary>
        /// Specifies the regular-expression to search for.
        /// </summary>
        [Required]
        public string Pattern
        {
            get { return pattern; 
            }
            set { pattern = value; 
            }
        }
        #endregion

        #endregion

        #region methods
        
        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Grep"/> class.
        /// </summary>
        public Grep()
            : base(Properties.TaskMessages.ResourceManager, "Help_")
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
            bool flag = true;

            try
            {
                MatchCollection matches = new MatchCollection();
                Pattern pattern = new Pattern(Pattern);

                Log.LogMessageFromResources(MessageImportance.Normal, "Grep_OutputFile", outputFile.ItemSpec);

                foreach (ITaskItem item in InputFiles)
                {
                    using (StreamReader reader = new StreamReader(item.ItemSpec))
                    {
                        string fileContent = reader.ReadToEnd();
                        matches.Add(pattern.Extract(item.ItemSpec, fileContent));
                    }
                }

                WriteMatches(matches);
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }
                Log.LogErrorWithCodeFromResources("Grep_Error", ex.Message);
                flag = false;
            }
            return flag;
        }
        #endregion

        #endregion

        #endregion
    }
}
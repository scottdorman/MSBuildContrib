//------------------------------------------------------------------------------
// MSBuildContrib
//
// CodeStats.cs
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;
using MSBuildContrib.Utilities;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Generates statistics from source code.
    /// </summary>
    /// <remarks>
    /// Scans files in a fileset counting lines.
    /// </remarks>
    /// <example>
    ///   <para>
    ///   Generate statistics for a set of C# and VB.NET sources, applying 
    ///   different labels for both.
    ///   </para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <CodeStatsItems Include="**\*.cs">
    ///       <Label>C#</Label>
    ///    </CodeStatItems>
    ///    <CodeStatsItems Include="**\*.vb">
    ///       <Label>VB.NET</Label>
    ///    </CodeStatsItems>
    /// </ItemGroup>
    /// <Codestats InputFiles="@(CodeStatsItems)" OutputFile="test.xml" Append="true" BuildName="MyBuildName" />
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>
    ///   Generate statistics for all C# sources and only output a summary to 
    ///   the log.
    ///   </para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <CodeStatsItems Include="**\*.cs">
    ///       <Label>C#</Label>
    ///    </CodeStatItems>
    /// </ItemGroup>
    /// <Codestats InputFiles="@(CodeStatsItems)" OutputFile="test.xml" Summarize="true" />
    ///     ]]>
    ///   </code>
    /// </example>
    public class CodeStats : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private const string CommentDblSlash = "//";
        private const string CommentSingleQuote = "'";
        private const string CommentSlashStar = "/*";
        private const string CommentStarSlash = "*/";

        private bool append;
        private string buildName;
        private List<FileCodeCountInfo> codeCountInfo = new List<FileCodeCountInfo>();
        private Dictionary<String, List<String>> codeStatsGroups = new Dictionary<string, List<String>>();
        private int commentLinesCount;
        private int emptyLinesCount;
        private ITaskItem[] inputFiles;
        private int lineCount;
        private ITaskItem outputFile;
        private bool summarize;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region CountLines
        private void CountLines(List<string> files, string label)
        {
            this.lineCount = 0;
            this.commentLinesCount = 0;
            this.emptyLinesCount = 0;
            this.codeCountInfo = new List<FileCodeCountInfo>(files.Count);

            FileCodeCountInfo fileInfo;
            foreach (string file in files)
            {
                fileInfo = CountFile(file);
                this.lineCount += fileInfo.LineCount;
                this.emptyLinesCount += fileInfo.EmptyLineCount;
                this.commentLinesCount += fileInfo.CommentLineCount;
                this.codeCountInfo.Add(fileInfo);
            }

            codeCountInfo.TrimExcess();
            Log.LogMessageFromResources(MessageImportance.Normal, "CodeStats_FileInfo", label, lineCount, commentLinesCount, emptyLinesCount);
        }
        #endregion

        #region FileCodeCountInfo
        private FileCodeCountInfo CountFile(string fileName)
        {
            int fileLineCount = 0;
            int fileCommentLineCount = 0;
            int fileEmptyLineCount = 0;
            bool inComment = false;

            using (StreamReader sr = File.OpenText(fileName))
            {
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    if (line != null)
                    {
                        line = line.Trim();
                        if (line.Length == 0)
                        {
                            fileEmptyLineCount++;
                        }
                        else if (line.StartsWith(CommentSlashStar, StringComparison.OrdinalIgnoreCase))
                        {
                            fileCommentLineCount++;
                            // we're only in comment block if it was not closed
                            // on same line
                            if (line.IndexOf(CommentStarSlash, StringComparison.OrdinalIgnoreCase) == -1)
                            {
                                inComment = true;
                            }
                        }
                        else if (inComment)
                        {
                            fileCommentLineCount++;
                            // check if comment block is closed in line
                            if (line.IndexOf(CommentStarSlash, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                inComment = false;
                            }
                        }
                        else if (line.StartsWith(CommentDblSlash, StringComparison.OrdinalIgnoreCase))
                        {
                            fileCommentLineCount++;
                        }
                        else if (line.StartsWith(CommentSingleQuote, StringComparison.OrdinalIgnoreCase))
                        {
                            fileCommentLineCount++;
                        }
                        fileLineCount++;
                    }
                }
                sr.Close();
            }

            if (!Summarize)
            {
                Log.LogMessageFromResources(MessageImportance.Normal, "CodeStats_Summary", fileName, fileLineCount, fileCommentLineCount, fileEmptyLineCount);
            }

            return new FileCodeCountInfo(fileName, fileLineCount, fileCommentLineCount, fileEmptyLineCount);
        }
        #endregion
        
        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region Append
        /// <summary>
        /// Specifies whether the results should be appended to the output file.
        /// The default is <see langword="false" />.
        /// </summary>
        public bool Append
        {
            get
            {
                return this.append;
            }
            set
            {
                this.append = value;
            }
        }
        #endregion

        #region BuildName
        /// <summary>
        /// An identifier to be able to track which build last updated the 
        /// code stats file.
        /// </summary>
        public string BuildName
        {
            get
            {
                return this.buildName;
            }
            set
            {
                this.buildName = value;
            }
        } 
        #endregion

        #region InputFiles
        /// <summary>
        /// The set of files which will be analyzed.
        /// </summary>
        [Required]
        public ITaskItem[] InputFiles
        {
            get
            {
                return inputFiles;
            }
            set
            {
                inputFiles = value;
            }
        }
        #endregion

        #region OutputFile
        /// <summary>
        /// The name of the file to save the output to (in XML).
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

        #region Summarize
        /// <summary>
        /// If you only want to show summary stats for the whole fileset
        /// </summary>
        public bool Summarize
        {
            get
            {
                return this.summarize;
            }
            set
            {
                this.summarize = value;
            }
        }
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStats"/> class.
        /// </summary>
        public CodeStats()
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
            bool success = true;

            try
            {
                string codeStatLabel = "Source Code";
                for (int i = 0; i < inputFiles.Length; i++)
                {
                    if (inputFiles[i].MetadataCount > 0)
                    {
                        codeStatLabel = inputFiles[i].GetMetadata("Label");
                    }
                    if (!codeStatsGroups.ContainsKey(codeStatLabel))
                    {
                        codeStatsGroups.Add(codeStatLabel, new List<String>());
                    }
                    codeStatsGroups[codeStatLabel].Add(inputFiles[i].ItemSpec);
                }

                XmlDocument doc = new XmlDocument();
                XmlNode codeSummaries = null;

                // if the output file is specified then write to it
                if (this.outputFile != null)
                {
                    // if append file is true and the file already exist.
                    // assume it is in the right format to be able to append the new
                    // xml nodes to it
                    if (Append && File.Exists(this.outputFile.ItemSpec))
                    {
                        // load the existing document
                        doc.Load(this.outputFile.ItemSpec);

                        // select the root node so that we can append to it
                        codeSummaries = doc.SelectSingleNode("//code-summaries");
                    }
                    else
                    {
                        // if not appending to the document and creating new
                        // create the Xml declaration and append it to the document
                        XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                        doc.AppendChild(docNode);

                        // add the code-summaries root element
                        codeSummaries = doc.CreateElement("code-summaries");
                        doc.AppendChild(codeSummaries);
                    }

                    // create code summary note to write all of the information
                    // for this run into
                    XmlNode summaryNode = doc.CreateElement("code-summary");

                    // add the date
                    XmlAttribute date = doc.CreateAttribute("date");
                    date.Value = XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Local);
                    summaryNode.Attributes.Append(date);

                    // add the buildname
                    XmlAttribute buildName = doc.CreateAttribute("buildname");
                    buildName.Value = BuildName;
                    summaryNode.Attributes.Append(buildName);

                    foreach (KeyValuePair<string, List<String>> kvp in codeStatsGroups)
                    {
                        // count the lines for the fileset for this CodeStatsType
                        CountLines(kvp.Value, kvp.Key);

                        // create the linecount node
                        XmlNode lineCountNode = doc.CreateElement("linecount");

                        // add the label
                        XmlAttribute label = doc.CreateAttribute("label");
                        label.Value = kvp.Key;
                        lineCountNode.Attributes.Append(label);

                        // add the total line count
                        XmlAttribute totalLineCount = doc.CreateAttribute("totalLineCount");
                        totalLineCount.Value = lineCount.InvariantToString();
                        lineCountNode.Attributes.Append(totalLineCount);

                        // add the empty line count
                        XmlAttribute emptyLineCount = doc.CreateAttribute("emptyLineCount");
                        emptyLineCount.Value = emptyLinesCount.InvariantToString();
                        lineCountNode.Attributes.Append(emptyLineCount);

                        // add the comment line count
                        XmlAttribute commentLineCount = doc.CreateAttribute("commentLineCount");
                        commentLineCount.Value = commentLinesCount.InvariantToString();
                        lineCountNode.Attributes.Append(commentLineCount);

                        // if not showing only summary.  display the line count information
                        // for each of the files within the fileset
                        if (!Summarize)
                        {
                            // create file summaries node
                            XmlNode fileSummaries = doc.CreateElement("file-summaries");

                            foreach (FileCodeCountInfo fileCodeCountInfo in this.codeCountInfo)
                            {
                                // create a file summary node
                                XmlElement fileNode = doc.CreateElement("file-summary");

                                // add the file name
                                fileNode.SetAttribute("name", fileCodeCountInfo.FileName);

                                // add the total line count
                                fileNode.SetAttribute("totalLineCount", fileCodeCountInfo.LineCount.InvariantToString());

                                // add the empty line count
                                fileNode.SetAttribute("emptyLineCount", fileCodeCountInfo.EmptyLineCount.InvariantToString());

                                // add the comment line count
                                fileNode.SetAttribute("commentLineCount", fileCodeCountInfo.CommentLineCount.InvariantToString());

                                // append the file summary node to the file summaries node
                                fileSummaries.AppendChild(fileNode);
                            }

                            // add the file summaries node to the line count node
                            lineCountNode.AppendChild(fileSummaries);
                        }

                        //add the line count node to the code summary node
                        summaryNode.AppendChild(lineCountNode);
                    }

                    // add the code summary node to the code summaries node
                    codeSummaries.AppendChild(summaryNode);

                    // save the xml document
                    doc.Save(outputFile.ItemSpec);
                }
            }
            catch (Exception ex)
            {
                if (ExceptionHandling.NotExpectedException(ex))
                {
                    throw;
                }

                Log.LogErrorWithCodeFromResources("CodeStats_Error", ex.Message);
                success = false;
            }

            return success;
        }
        #endregion

        #endregion
        
        #endregion
    }
}
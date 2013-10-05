//------------------------------------------------------------------------------
// MSBuildContrib
//
// FxCop.cs
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
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;
using MSBuildContrib.Utilities;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class FxCop : ToolTask
    {
        #region events
        #endregion

        #region class-wide fields
        private string actualLogFile;
        private int analysisTimeout;
        private bool applyLogFileXsl;
        private string[] assemblies;
        private string consoleXsl;
        private string culture;
        private string[] dependentAssemblyPaths;
        private string[] dictionaries;
        private ArrayList filesWritten;
        private bool forceOutput;
        private bool generateSuccessFile;
        private bool ignoreGeneratedCode;
        private bool ignoreInvalidTargets;
        private string[] imports;
        private string logFile;
        private string logFileXsl;
        private bool m_hasFatalError;
        private int numErrors;
        private int numWarnings;
        private bool outputToBuildLog;
        private bool outputToConsole;
        private bool overrideRuleVisibilities;
        private string platformPath;
        private string project;
        private bool quiet;
        private string[] references;
        private string[] ruleAssemblies;
        private string[] rules;
        private const int s_assemblyReferencesError = 0x200;
        private const int s_commandlineSwitchError = 0x80;
        private const int s_outputError = 0x40;
        private const int s_unknownError = 0x1000000;
        private string saveMessagesToReport;
        private bool searchGlobalAssemblyCache;
        private string successFile;
        private bool summary;
        private string toolName;
        private bool treatWarningsAsErrors;
        private bool updateProject;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region AddQuotes
        private static string AddQuotes(string s)
        {
            s = s.Trim("\"".ToCharArray());
            return ("\"" + s + "\"");
        } 
        #endregion

        #region AppendResponseFileSwitch
        private static void AppendResponseFileSwitch(StringBuilder commandline, string switchstr)
        {
            commandline.Append(" " + switchstr);
        } 
        #endregion

        #region AppendResponseFileSwitch
        private static void AppendResponseFileSwitch(StringBuilder commandline, string switchstr, string parameter)
        {
            commandline.Append(" " + switchstr + parameter);
        } 
        #endregion

        #region LogEngineWarning
        private void LogEngineWarning(string details, string keyword)
        {
            BuildWarningEventArgs e = new BuildWarningEventArgs(null, null, string.Empty, 0, 0, 0, 0, details, keyword, typeof(FxCop).FullName);
            base.BuildEngine.LogWarningEvent(e);
            this.numWarnings++;
        } 
        #endregion

        #region LogException
        private bool LogException(XmlNode exceptionMessage)
        {
            string str = null;
            string str2 = null;
            string str3 = null;
            string str6;
            XmlAttribute attribute = exceptionMessage.Attributes["CheckId"];
            if (attribute != null)
            {
                str = attribute.Value;
                str2 = exceptionMessage.Attributes["Category"].Value;
                str3 = exceptionMessage.Attributes["Target"].Value;
            }
            string keyword = exceptionMessage.Attributes["Keyword"].Value;
            XmlAttribute attribute2 = exceptionMessage.Attributes["TreatAsWarning"];
            XmlElement element = (XmlElement)exceptionMessage.SelectSingleNode("ExceptionMessage");
            string innerText = element.InnerText;
            if (str != null)
            {
                str6 = base.Log.FormatResourceString("FxCop_RuleError", new object[] { keyword, str2, str, str3, innerText });
            }
            else
            {
                str6 = base.Log.FormatString("{0} : {1}", new object[] { keyword, innerText });
            }
            if (((attribute2 == null) || this.TreatWarningsAsErrors) || !bool.Parse(attribute2.Value))
            {
                this.LogFatalError(str6, keyword);
                return false;
            }
            this.LogEngineWarning(str6, keyword);
            return true;
        }
        #endregion

        #region LogFatalError
        private void LogFatalError(string details, string keyword)
        {
            BuildErrorEventArgs e = new BuildErrorEventArgs(null, null, string.Empty, 0, 0, 0, 0, details, keyword, typeof(FxCop).FullName);
            base.BuildEngine.LogErrorEvent(e);
            this.numErrors++;
        } 
        #endregion

        #region LogWarningFromMessageElement
        private bool LogWarningFromMessageElement(XmlElement messageElement, bool logMessage)
        {
            string file = null;
            int lineNumber = 0;
            string message = "";
            bool flag = false;
            string attribute = messageElement.GetAttribute("CheckId");
            string str4 = messageElement.GetAttribute("Category");
            XmlNodeList list = messageElement.SelectNodes(".//Issue");
            if (this.treatWarningsAsErrors)
            {
                flag = true;
            }
            for (int i = 0; i < list.Count; i++)
            {
                XmlElement element = (XmlElement)list[i];
                if (element != null)
                {
                    if (messageElement.GetAttribute("BreaksBuild") == bool.TrueString)
                    {
                        flag = true;
                    }
                    message = element.InnerText;
                    string str6 = element.GetAttribute("Path");
                    string str8 = element.GetAttribute("File");
                    string s = element.GetAttribute("Line");
                    if (((str6.Length != 0) && (s.Length != 0)) && (str8.Length != 0))
                    {
                        file = Path.Combine(str6, str8);
                        lineNumber = int.Parse(s, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        file = string.Empty;
                        lineNumber = -1;
                    }
                }
                string xml = null;
                if (i == 0)
                {
                    XmlElement newChild = messageElement.OwnerDocument.CreateElement("CodeItem");
                    messageElement.AppendChild(newChild);
                    XmlNodeList list2 = messageElement.SelectNodes("ancestor::*");
                    XmlElement element3 = newChild;
                    bool flag2 = true;
                    foreach (XmlNode node in list2)
                    {
                        XmlElement element4 = (XmlElement)node;
                        string localName = element4.LocalName;
                        if ((localName != "FxCopReport") && (localName != "Messages"))
                        {
                            if (flag2)
                            {
                                XmlElement element5 = element3.OwnerDocument.CreateElement(localName);
                                element3.AppendChild(element5);
                                element3 = element5;
                                flag2 = false;
                                continue;
                            }
                            XmlElement element6 = element3.OwnerDocument.CreateElement(localName);
                            element6.SetAttribute("Name", element4.GetAttribute("Name"));
                            element3.AppendChild(element6);
                            element3 = element6;
                            flag2 = true;
                        }
                    }
                    xml = messageElement.OuterXml;
                }
        
                if (flag)
                {
                    CodeAnalysisErrorEventArgs e = new CodeAnalysisErrorEventArgs(str4, attribute, file, lineNumber, message, attribute, xml);
                    if (logMessage)
                    {
                        base.BuildEngine.LogErrorEvent(e);
                    }
                    this.numErrors++;
                }
                else
                {
                    CodeAnalysisWarningEventArgs args2 = new CodeAnalysisWarningEventArgs(str4, attribute, file, lineNumber, message, attribute, xml);
                    if (logMessage)
                    {
                        base.BuildEngine.LogWarningEvent(args2);
                    }
                    this.numWarnings++;
                }
            }
            return !flag;
        }
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region AlternativeToolName
        /// <summary>
        /// 
        /// </summary>
        public string AlternativeToolName
        {
            get
            {
                return this.toolName;
            }
            set
            {
                this.toolName = value;
            }
        } 
        #endregion

        #region AnalysisTimeout
        /// <summary>
        /// Modifies the deadlock timeout.
        /// </summary>
        public int AnalysisTimeout
        {
            get
            {
                return this.analysisTimeout;
            }
            set
            {
                this.analysisTimeout = value;
            }
        } 
        #endregion

        #region ApplyLogFileXsl
        /// <summary>
        /// Applies the XSL transformation that is specified in <see cref="LogFileXsl"/> to the analysis report before saving the file.
        /// </summary>
        public bool ApplyLogFileXsl
        {
            get
            {
                return this.applyLogFileXsl;
            }
            set
            {
                this.applyLogFileXsl = value;
            }
        } 
        #endregion

        #region Assemblies
        /// <summary>
        /// Specifies the target assemblies to analyze.
        /// </summary>
        /// <remarks>If you specify a directory, FxCopCmd tries to analyze all files
        /// that have the .exe or .dll extension.</remarks>
        public string[] Assemblies
        {
            get
            {
                return this.assemblies;
            }
            set
            {
                this.assemblies = value;
            }
        } 
        #endregion

        #region ConsoleXsl
        /// <summary>
        /// Specifies the XSL or XSLT file that contains a transformation to be applied to the 
        /// analysis output before it is displayed on the console.
        /// </summary>
        /// <remarks>This option overrides the default XSL file applied to the analysis output.</remarks>
        public string ConsoleXsl
        {
            get
            {
                return this.consoleXsl;
            }
            set
            {
                this.consoleXsl = value;
            }
        } 
        #endregion

        #region Culture
        /// <summary>
        /// Customizes the language used by the IdentifiersShouldBeSpelledCorrectly and 
        /// CompoundWordsShouldBeCasedCorrectly rules to determine the locale should be
        /// used to spell check the identifiers.
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="Culture"/> is used to indicate the language of your API identifiers; 
        /// it is not, however, used to determine the language of your ResX-based resources, which use
        /// the AssemblyCultureAttribute and the NeutralResourcesLanguageAttribute attributes.</para>
        /// <para>AssemblyCultureAttribute is applied to satellitte assemblies and should never be 
        /// placed on an assembly with code, whereas NeutralResourcesLanguageAttribute is applied
        /// to an assembly with code to indicate the neutral culture of the assembly.</para>
        /// </remarks>
        public string Culture
        {
            get
            {
                return this.culture;
            }
            set
            {
                this.culture = value;
            }
        } 
        #endregion

        #region DependentAssemblyPaths
        /// <summary>
        /// Specifies additional directories to search for assembly dependencies.
        /// </summary>
        /// <remarks>The target assembly directory and the current working directory
        /// are always searched.</remarks>
        public string[] DependentAssemblyPaths
        {
            get
            {
                return this.dependentAssemblyPaths;
            }
            set
            {
                this.dependentAssemblyPaths = value;
            }
        } 
        #endregion

        #region Dictionaries
        /// <summary>
        /// Specifies custom dictionaries that should be used by the spelling rules
        /// to prevent them from firing on rules that are not in the standard
        /// dictionary, such as company and product names.
        /// </summary>
        public string[] Dictionaries
        {
            get
            {
                return this.dictionaries;
            }
            set
            {
                this.dictionaries = value;
            }
        } 
        #endregion

        #region FilesWritten
        /// <summary>
        /// The files created by the code analysis.
        /// </summary>
        [Output]
        public ITaskItem[] FilesWritten
        {
            get
            {
                return (ITaskItem[])this.filesWritten.ToArray(typeof(ITaskItem));
            }
        } 
        #endregion

        #region ForceOutput
        /// <summary>
        /// 
        /// </summary>
        public bool ForceOutput
        {
            get
            {
                return this.forceOutput;
            }
            set
            {
                this.forceOutput = value;
            }
        } 
        #endregion

        #region GenerateSuccessFile
        /// <summary>
        /// 
        /// </summary>
        public bool GenerateSuccessFile
        {
            get
            {
                return this.generateSuccessFile;
            }
            set
            {
                this.generateSuccessFile = value;
            }
        } 
        #endregion

        #region IgnoreGeneratedCode
        /// <summary>
        /// Do not analyze tool generated code.
        /// </summary>
        public bool IgnoreGeneratedCode
        {
            get
            {
                return this.ignoreGeneratedCode;
            }
            set
            {
                this.ignoreGeneratedCode = value;
            }
        } 
        #endregion

        #region IgnoreInvalidTargets
        /// <summary>
        /// Do not attempt to analyze invalid targets.
        /// </summary>
        public bool IgnoreInvalidTargets
        {
            get
            {
                return this.ignoreInvalidTargets;
            }
            set
            {
                this.ignoreInvalidTargets = value;
            }
        } 
        #endregion

        #region Imports
        /// <summary>
        /// Specifies the names of analysis reports or project files to import. Any messages
        /// in the imported files that are marked as excluded are not included in the 
        /// analysis results.
        /// </summary>
        /// <remarks>
        /// If analysis results are saved to a project file by using the <see cref="UpdateProject"/>
        /// option, the imported messages are not saved.
        /// </remarks>
        public string[] Imports
        {
            get
            {
                return this.imports;
            }
            set
            {
                this.imports = value;
            }
        } 
        #endregion

        #region LogFile
        /// <summary>
        /// Specifies the file name for the analysis report.
        /// </summary>
        /// <remarks>
        /// <para>If the file exists, it is overwritten without warning. If no items are
        /// reported by the analysis and the file does not exist, it is not created.
        /// If the file exists, it is deleted.</para>
        /// <para>By default, the file includes an xml-style sheet processing instruction that
        /// references FxCopReport.xsl. The file is saved in XML format unless
        /// <see cref="ApplyLogFileXsl"/> is specified.</para>
        /// </remarks>
        public string LogFile
        {
            get
            {
                return this.logFile;
            }
            set
            {
                this.logFile = value;
            }
        } 
        #endregion

        #region LogFileXsl
        /// <summary>
        /// Specifies the XSL or XSLT file that contains a transformation to be applied to the 
        /// analysis output.
        /// </summary>
        /// <remarks>This option overrides the default XSL file applied to the analysis output.</remarks>
        public string LogFileXsl
        {
            get
            {
                return this.logFileXsl;
            }
            set
            {
                this.logFileXsl = value;
            }
        } 
        #endregion

        #region OutputToBuildLog
        /// <summary>
        /// Directs analysis output to the build log as build warnings or errors..
        /// </summary>
        public bool OutputToBuildLog
        {
            get
            {
                return this.outputToBuildLog;
            }
            set
            {
                this.outputToBuildLog = value;
            }
        }
        #endregion

        #region OutputToConsole
        /// <summary>
        /// Directs analysis output to the console. By default, the XSL file
        /// FxCopConsoleOutput.xsl is applied to the output before it is displayed.
        /// </summary>
        public bool OutputToConsole
        {
            get
            {
                return this.outputToConsole;
            }
            set
            {
                this.outputToConsole = value;
            }
        } 
        #endregion

        #region OverrideRuleVisibilities
        /// <summary>
        /// 
        /// </summary>
        public bool OverrideRuleVisibilities
        {
            get
            {
                return this.overrideRuleVisibilities;
            }
            set
            {
                this.overrideRuleVisibilities = value;
            }
        } 
        #endregion

        #region PlatformPath
        /// <summary>
        /// Specifies the location of the version of Mscorlib.dll that was
        /// used when the target assemblies were built, if this version is
        /// not installed on the computer that is running the code analysis.
        /// Typically, this option does not have to be set.
        /// </summary>
        public string PlatformPath
        {
            get
            {
                return this.platformPath;
            }
            set
            {
                this.platformPath = value;
            }
        } 
        #endregion

        #region Project
        /// <summary>
        /// Specifies the file name of an FxCop project file.
        /// </summary>
        public string Project
        {
            get
            {
                return this.project;
            }
            set
            {
                this.project = value;
            }
        } 
        #endregion

        #region Quiet
        /// <summary>
        /// Outputs verbose information during analysis.
        /// </summary>
        public bool Quiet
        {
            get
            {
                return this.quiet;
            }
            set
            {
                this.quiet = value;
            }
        } 
        #endregion

        #region References
        /// <summary>
        /// Specifies additional directories to search for assembly dependencies.
        /// </summary>
        /// <remarks>The target assembly directory and the current working directory
        /// are always searched.</remarks>
        public string[] References
        {
            get
            {
                return this.references;
            }
            set
            {
                this.references = value;
            }
        } 
        #endregion

        #region RuleAssemblies
        /// <summary>
        /// Specifies the location of rule libraries to load.
        /// </summary>
        /// <remarks> If you specify a directory, FxCopCmd will try
        /// to load all files that have the .dll extension.
        /// </remarks>
        public string[] RuleAssemblies
        {
            get
            {
                return this.ruleAssemblies;
            }
            set
            {
                this.ruleAssemblies = value;
            }
        } 
        #endregion

        #region Rules
        /// <summary>
        /// Specifies rules to include or suppress.
        /// </summary>
        /// <remarks>
        /// <para>The rule must be in the format: Category#RuleID. For example, to suppress
        /// rule CA1300 in the Microsoft.Globalization category the rule specified would be:
        /// Microsoft.Globalization#CA1300.</para>
        /// <para>To suppress the rule, precede the rule name with a minus (-).</para>
        /// </remarks>
        public string[] Rules
        {
            get
            {
                return this.rules;
            }
            set
            {
                this.rules = value;
            }
        } 
        #endregion

        #region SaveMessagesToReport
        /// <summary>
        /// 
        /// </summary>
        public string SaveMessagesToReport
        {
            get
            {
                return this.saveMessagesToReport;
            }
            set
            {
                this.saveMessagesToReport = value;
            }
        } 
        #endregion

        #region SearchGlobalAssemblyCache
        /// <summary>
        /// Search the Global Assembly Cache (GAC) for missing references.
        /// </summary>
        public bool SearchGlobalAssemblyCache
        {
            get
            {
                return this.searchGlobalAssemblyCache;
            }
            set
            {
                this.searchGlobalAssemblyCache = value;
            }
        } 
        #endregion

        #region SuccessFile
        /// <summary>
        /// 
        /// </summary>
        public string SuccessFile
        {
            get
            {
                if (((this.successFile == null) && (this.actualLogFile != null)) && ((this.Assemblies != null) && (this.Assemblies.Length == 1)))
                {
                    string path = this.Assemblies[0];
                    string str2 = Path.Combine(Path.GetDirectoryName(this.actualLogFile), Path.GetFileName(path));
                    this.successFile = str2 + ".lastcodeanalysissucceeded";
                }
                return this.successFile;
            }
            set
            {
                this.successFile = value;
            }
        } 
        #endregion

        #region Summary
        /// <summary>
        /// Include a summary report that has the informational messages returned by FxCopCmd.
        /// </summary>
        /// <remarks>The summary shows the number of items found, how many items were new, and the running time for the analysis.</remarks>
        public bool Summary
        {
            get
            {
                return this.summary;
            }
            set
            {
                this.summary = value;
            }
        } 
        #endregion

        #region ToolName
        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value>The name of the executable file to run.</value>
        protected override string ToolName
        {
            get
            {
                return this.toolName;
            }
        } 
        #endregion

        #region TreatWarningsAsErrors
        /// <summary>
        /// Treat all Code Analysis warnings as build errors.
        /// </summary>
        public bool TreatWarningsAsErrors
        {
            get
            {
                return this.treatWarningsAsErrors;
            }
            set
            {
                this.treatWarningsAsErrors = value;
            }
        } 
        #endregion

        #region UpdateProject
        /// <summary>
        /// Saves the results of the analysis in the project file.
        /// </summary>
        /// <remarks> This option is ignored if the <see cref="Project"/> option is not specified.</remarks>
        public bool UpdateProject
        {
            get
            {
                return this.updateProject;
            }
            set
            {
                this.updateProject = value;
            }
        } 
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FxCop"/> class.
        /// </summary>
        public FxCop()
            : base(Properties.TaskMessages.ResourceManager, "Help_")
        {
            this.filesWritten = new ArrayList();
            this.toolName = "FxCopCmd.exe";
            this.analysisTimeout = 120;
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
            if (!File.Exists(this.GenerateFullPathToTool()))
            {
                Log.LogErrorWithCodeFromResources("FxCop_FailedToLocate");
                return false;
            }

            BuildMessageEventArgs e = new BuildMessageEventArgs(base.Log.FormatResourceString("FxCop_PerformingCodeAnalysis", new object[0]), null, this.ToString(), MessageImportance.High);
            base.BuildEngine.LogMessageEvent(e);

            bool flag = true;
            this.numErrors = 0;
            this.numWarnings = 0;
            try
            {
                if (this.LogFile != null)
                {
                    this.actualLogFile = this.LogFile;
                }
                else
                {
                    try
                    {
                        this.actualLogFile = Path.GetTempFileName();
                    }
                    catch (IOException exception)
                    {
                        throw new IOException(base.Log.FormatResourceString("General_FailedCreatingTempFile", new object[] { exception.Message }), exception);
                    }
                }

                flag = flag && base.Execute();
                if (File.Exists(this.actualLogFile) && !this.m_hasFatalError)
                {
                    try
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(this.actualLogFile);
                        XmlElement documentElement = document.DocumentElement;
                        foreach (XmlNode node in documentElement.SelectNodes(".//Exception"))
                        {
                            flag = this.LogException(node) && flag;
                        }
                        foreach (XmlNode node2 in documentElement.SelectNodes(".//Message[@Status='Active']"))
                        {
                            XmlElement messageElement = (XmlElement)node2;
                            flag = this.LogWarningFromMessageElement(messageElement, this.outputToBuildLog) && flag;
                        }
                    }
                    catch (XmlException)
                    {
                        this.LogFatalError(base.Log.FormatResourceString("FxCop_FailedReadingReport", new object[] { this.actualLogFile }), "CA0501");
                    }
                }
                base.Log.LogMessageFromResources(MessageImportance.High, "FxCop_AnalysisComplete", new object[] { this.numErrors, this.numWarnings });
            }
            finally
            {
                if (this.LogFile == null)
                {
                    string successFile = this.SuccessFile;
                    if ((successFile != null) && File.Exists(successFile))
                    {
                        File.Delete(successFile);
                    }
                    File.Delete(this.actualLogFile);
                }
                else
                {
                    string path = this.SuccessFile;
                    if (File.Exists(this.LogFile))
                    {
                        this.filesWritten.Add(new TaskItem(this.LogFile));
                    }
                    if ((path != null) && File.Exists(path))
                    {
                        this.filesWritten.Add(new TaskItem(path));
                    }
                }
            }
            this.m_hasFatalError = false;
            return flag;
        }
        #endregion

        #region GenerateFullPathToTool
        /// <summary>
        /// Returns the fully qualified path to the executable file.
        /// </summary>
        /// <returns>The fully qualified path to the executable file.</returns>
        protected override string GenerateFullPathToTool()
        {
            if (String.IsNullOrEmpty(this.ToolPath))
            {
                ITaskItem fullPath;
                if (ToolHelpers.FindFxCop(out fullPath, base.Log))
                {
                    this.ToolPath = fullPath.ItemSpec;
                }
            }

            return Path.Combine(this.ToolPath, this.ToolName);
        } 
        #endregion

        #region GenerateResponseFileCommands
        /// <summary>
        /// Returns a string value containing the command line arguments to add to the response (.rsp) file before running the executable file.
        /// </summary>
        /// <returns>A <see cref="String"/> value containing the command line arguments to add to the response (.rsp) file before running the executable file.</returns>
        protected override string GenerateResponseFileCommands()
        {
            StringBuilder commandline = new StringBuilder();
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            AppendResponseFileSwitch(commandline, "/outputCulture:", currentUICulture.LCID.ToString(CultureInfo.InvariantCulture.NumberFormat));
            AppendResponseFileSwitch(commandline, "/out:", AddQuotes(this.actualLogFile));

            if (!string.IsNullOrEmpty(this.Project))
            {
                AppendResponseFileSwitch(commandline, "/project:", AddQuotes(this.Project));
            }

            if (this.UpdateProject)
            {
                AppendResponseFileSwitch(commandline, "/update");
            }

            if (this.Assemblies != null)
            {
                foreach (string str in this.Assemblies)
                {
                    AppendResponseFileSwitch(commandline, "/file:", AddQuotes(str));
                }
            }

            if (this.Dictionaries != null)
            {
                foreach (string dictionary in this.Dictionaries)
                {
                    AppendResponseFileSwitch(commandline, "/dictionary:", AddQuotes(dictionary));
                }
            }

            if (!string.IsNullOrEmpty(this.LogFileXsl))
            {
                AppendResponseFileSwitch(commandline, "/outxsl:", AddQuotes(this.LogFileXsl));
            }

            if (this.ApplyLogFileXsl)
            {
                AppendResponseFileSwitch(commandline, "/applyoutxsl");
            }

            if (!string.IsNullOrEmpty(this.PlatformPath))
            {
                AppendResponseFileSwitch(commandline, "/platform:", AddQuotes(this.PlatformPath));
            }

            if (this.DependentAssemblyPaths != null)
            {
                foreach (string dependentAssembly in this.DependentAssemblyPaths)
                {
                    AppendResponseFileSwitch(commandline, "/directory:", AddQuotes(dependentAssembly));
                }
            }

            if (this.References != null)
            {
                Hashtable hashtable = new Hashtable();
                foreach (string reference in this.References)
                {
                    hashtable[AddQuotes(Path.GetDirectoryName(reference))] = null;
                }
                foreach (string directory in hashtable.Keys)
                {
                    AppendResponseFileSwitch(commandline, "/directory:", directory);
                }
            }

            if (this.OutputToConsole)
            {
                AppendResponseFileSwitch(commandline, "/console");
            }

            if (!string.IsNullOrEmpty(this.ConsoleXsl))
            {
                AppendResponseFileSwitch(commandline, "/consolexsl:", AddQuotes(this.ConsoleXsl));
            }

            if (this.Imports != null)
            {
                foreach (string import in this.Imports)
                {
                    AppendResponseFileSwitch(commandline, "/import:", AddQuotes(import));
                }
            }

            if (this.RuleAssemblies == null)
            {
                this.RuleAssemblies = new string[] { Path.Combine(base.ToolPath, "Rules") };
            }

            foreach (string ruleAssembly in this.RuleAssemblies)
            {
                AppendResponseFileSwitch(commandline, "/rule:", AddQuotes(ruleAssembly));
            }

            if (this.Rules != null)
            {
                foreach (string ruleId in this.Rules)
                {
                    AppendResponseFileSwitch(commandline, "/ruleid:", ruleId);
                }
            }

            if (this.Summary)
            {
                AppendResponseFileSwitch(commandline, "/summary");
            }

            if (this.Quiet)
            {
                AppendResponseFileSwitch(commandline, "/quiet");
            }

            if (this.SearchGlobalAssemblyCache)
            {
                AppendResponseFileSwitch(commandline, "/searchgac");
            }

            if (this.IgnoreInvalidTargets)
            {
                AppendResponseFileSwitch(commandline, "/ignoreinvalidtargets");
            }

            if (this.ForceOutput)
            {
                AppendResponseFileSwitch(commandline, "/forceoutput");
            }

            if (this.GenerateSuccessFile)
            {
                AppendResponseFileSwitch(commandline, "/successfile");
            }

            if (this.IgnoreGeneratedCode)
            {
                AppendResponseFileSwitch(commandline, "/ignoregeneratedcode");
            }

            if (!string.IsNullOrEmpty(this.Culture))
            {
                AppendResponseFileSwitch(commandline, "/culture:", this.Culture);
            }

            if (this.OverrideRuleVisibilities)
            {
                AppendResponseFileSwitch(commandline, "/overriderulevisibilities");
            }

            if (!string.IsNullOrEmpty(this.saveMessagesToReport))
            {
                AppendResponseFileSwitch(commandline, "/saveMessagesToReport:", this.saveMessagesToReport);
            }

            AppendResponseFileSwitch(commandline, "/timeout:", this.analysisTimeout.ToString(CultureInfo.InvariantCulture));

            return commandline.ToString();
        }
        #endregion

        #region HandleTaskExecutionErrors
        /// <summary>
        /// Handles execution errors raised by the executable file.
        /// </summary>
        /// <returns><see langword="true"/> if the method runs successfully; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This method is called only if the executable file returns a non-zero exit code.
        /// The return value of this method is used as the return value of the task.
        /// </remarks>
        protected override bool HandleTaskExecutionErrors()
        {
            string details;
            string keyword;

            if ((base.ExitCode & s_outputError) == s_outputError)
            {
                details = Properties.TaskMessages.FxCop_OutputError;
                keyword = "CA0501";
                this.LogFatalError(details, keyword);
                this.m_hasFatalError = true;
            }
            else if ((base.ExitCode & s_commandlineSwitchError) == s_commandlineSwitchError)
            {
                details = Properties.TaskMessages.CommandLineSwitchError;
                keyword = "CA0059";
                this.LogFatalError(details, keyword);
                this.m_hasFatalError = true;
            }
            else
            {
                if ((base.ExitCode == s_assemblyReferencesError) && !this.TreatWarningsAsErrors)
                {
                    return true;
                }
                if ((base.ExitCode & s_unknownError) == s_unknownError)
                {
                    details = Properties.TaskMessages.FxCop_UnknownError;
                    keyword = "CA0001";
                    this.LogFatalError(details, keyword);
                    this.m_hasFatalError = true;
                }
            }
            return false;
        }
        #endregion

        #endregion

        #endregion
    }
}
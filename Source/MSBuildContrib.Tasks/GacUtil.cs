//------------------------------------------------------------------------------
// MSBuildContrib
//
// GacUtil.cs
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
// Copyright (C) 2006 Paul Welter
// 2007-December: Adapted from the MSBuild Community Tasks GacUtil class
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuildContrib.Common;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Install or uninstall assemblies into the GAC.
    /// </summary>
    /// <example>
    /// <para>Install a DLL into the GAC.</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <Assemblies Include="MSBuildContrib.Common.Tasks.dll" />
    /// </ItemGroup>
    /// <GacUtil Command="Install" Assemblies="@(Assemblies)" Force="true" />
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    /// <para>Uninstall a DLL into the GAC.</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <Assemblies Include="MSBuildContrib.Common.Tasks.dll" />
    /// </ItemGroup>
    /// <GacUtil Command="Uninstall" Assemblies="@(Assemblies)" Force="true" />
    ///     ]]>
    ///   </code>
    /// </example>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gac")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Util")]
    public class GacUtil : ToolTask
    {
        #region events
        #endregion

        #region class-wide fields
        private ITaskItem[] assemblies;
        private string assemblyListFile;
        private GacUtilCommands command;
        private bool force;
        private TargetDotNetFrameworkVersion targetFramework = TargetDotNetFrameworkVersion.VersionLatest;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region WriteAssemblyFileList
        private void WriteAssemblyFileList(string path, ITaskItem[] items)
        {
            List<string> lines = new List<string>();

            if (items.Length > 0)
            {
                if (this.command == GacUtilCommands.Install)
                {
                    foreach (ITaskItem item in items)
                    {
                        lines.Add(item.ItemSpec);
                    }
                }
                else
                {
                    foreach (ITaskItem item in items)
                    {
                        string assemblyPath = item.ItemSpec;
                        try
                        {
                            Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                            AssemblyName assemblyName = assembly.GetName();
                            lines.Add(String.Format(CultureInfo.InvariantCulture, "{0}, processorArchitecture={1}", assemblyName.FullName, assemblyName.ProcessorArchitecture));
                        }
                        catch (FileNotFoundException ex)
                        {
                            Log.LogErrorWithCodeFromResources("GacUtil_UnregisterAsmFileDoesNotExist", assemblyPath, ex.Message);
                        }
                        catch (FileLoadException ex)
                        {
                            Log.LogErrorWithCodeFromResources("GacUtil_UnregisterAsmFailedToLoadAssembly", assemblyPath, ex.Message);
                        }
                        catch (IOException ex)
                        {
                            Log.LogErrorWithCodeFromResources("GacUtil_GeneralIoException", assemblyPath, ex.Message);
                        }
                        catch (BadImageFormatException ex)
                        {
                            Log.LogErrorWithCodeFromResources("GacUtil_BadImage", assemblyPath, ex.Message);
                        }
                        catch (Exception ex)
                        {
                            if (ExceptionHandling.NotExpectedException(ex))
                            {
                                throw;
                            }
                            Log.LogErrorWithCodeFromResources("GacUtil_Error", "unregister", assemblyPath, ex.Message);
                        }
                    }
                }
                File.WriteAllLines(path, lines.ToArray());
            }
        } 
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region Assemblies
        /// <summary>
        /// The list of assemblies to install or uninstall.
        /// </summary>
        [Required]
        public ITaskItem[] Assemblies
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

        #region Command
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>One of the <see cref="GacUtilCommands"/> values.</value>
        public string Command
        {
            get
            {
                return this.command.ToString();
            }
            set
            {
                if (!Enum.IsDefined(typeof(GacUtilCommands), value))
                {
                    throw new ArgumentOutOfRangeException("value", String.Format(CultureInfo.CurrentUICulture, Properties.Resources.InvalidEnumValue, value));
                }
                this.command = (GacUtilCommands)Enum.Parse(typeof(GacUtilCommands), value);
            }
        } 
        #endregion

        #region Force
        /// <summary>
        /// Gets or sets a value indicating whether to force reinstall of an assembly.
        /// </summary>
        /// <value><see langword="true"/> if force; otherwise, <see langword="false"/>.</value>
        public bool Force
        {
            get
            {
                return this.force;
            }
            set
            {
                this.force = value;
            }
        } 
        #endregion

        #region TargetFramework
        /// <summary>
        /// The .NET Framework version of the GAC.
        /// </summary>
        /// <value>A string representing one of the <see cref="TargetDotNetFrameworkVersion"/> values.</value>
        public string TargetFramework
        {
            get
            {
                return this.targetFramework.ToString();
            }
            set
            {
                this.targetFramework = (TargetDotNetFrameworkVersion)Enum.Parse(typeof(TargetDotNetFrameworkVersion), value, true); ;
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
                return "gacutil.exe";
            }
        } 
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GacUtil"/> class.
        /// </summary>
        public GacUtil()
            : base(Properties.TaskMessages.ResourceManager, "Help_")
        {
            try
            {
                this.assemblyListFile = Path.GetTempFileName();
            }
            catch (IOException ex)
            {
                Log.LogErrorWithCodeFromResources("General_FailedCreatingTempFile", ex.Message);
            }
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
                this.assemblyListFile = Path.GetTempFileName();
            }
            catch (IOException ex)
            {
                Log.LogErrorWithCodeFromResources("General_FailedCreatingTempFile", ex.Message);
                flag = false;
            }

            if (flag)
            {
                try
                {
                    WriteAssemblyFileList(this.assemblyListFile, this.assemblies);
                    flag = base.Execute();
                }
                catch (FileNotFoundException ex)
                {
                    Log.LogErrorWithCodeFromResources("GacUtil_GeneralError", (this.command == GacUtilCommands.Install ? "register" : "unregister"), ex.Message);
                    flag = false;
                }
                catch (Exception ex)
                {
                    if (ExceptionHandling.NotExpectedException(ex))
                    {
                        throw;
                    }
                    Log.LogErrorWithCodeFromResources("GacUtil_GeneralError", "unregister", (this.command == GacUtilCommands.Install ? "register" : "unregister"), ex.Message);
                    flag = false;
                }
                finally
                {
                    File.Delete(this.assemblyListFile);
                }
            }
            return flag;
        } 
        #endregion

        #region GenerateCommandLineCommands
        /// <summary>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </summary>
        /// <returns>A string value containing the command line arguments to pass directly to the executable file.</returns>
        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();
            builder.AppendSwitch("/nologo");
            if (this.force)
            {
                builder.AppendSwitch("/f");
            }
            switch (this.command)
            {
                case GacUtilCommands.Install:
                    builder.AppendSwitch("/il");
                    break;

                case GacUtilCommands.Uninstall:
                    builder.AppendSwitch("/ul");
                    break;
            }
            builder.AppendFileNameIfNotNull(this.assemblyListFile);
            return builder.ToString();
        } 
        #endregion

        #region GenerateFullPathToTool
        /// <summary>
        /// Returns the fully qualified path to the executable file.
        /// </summary>
        /// <returns>The fully qualified path to the executable file.</returns>
        protected override string GenerateFullPathToTool()
        {
            return ToolLocationHelper.GetPathToDotNetFrameworkSdkFile(this.ToolName, this.targetFramework);
        } 
        #endregion

        #region LogToolCommand
        /// <summary>
        /// Logs the starting point of the run to all registered loggers.
        /// </summary>
        /// <param name="message">A descriptive message to provide loggers, usually the command line and switches.</param>
        protected override void LogToolCommand(string message)
        {
            base.Log.LogCommandLine(MessageImportance.Low, message);
        } 
        #endregion
        
        #endregion

        #endregion
    }
}
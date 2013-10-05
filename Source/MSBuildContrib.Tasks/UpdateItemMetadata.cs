//------------------------------------------------------------------------------
// MSBuildContrib
//
// UpdateItemMetadata.cs
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
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildContrib.Tasks
{
    /// <summary>
    /// Adds or updates metadata to an item.
    /// </summary>
    /// <example>
    /// <para>Updates the existing AdditionalProperties metadata to include the value
    /// "SecondTest=No".</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <Files Include="MSBuildContrib.Common.Tasks.dll">
    ///       <AdditionalProperties>TestFile=True</AdditionalProperties>
    ///    </Files>
    /// </ItemGroup>
    /// <UpdateItemMetadata Items="@(Files)" MetadataName="AdditionalProperties" Metadata="SecondTest=No">
    ///    <Output TaskParameter="ResultItems" ItemName="UpdatedItems" />
    /// </UpdateItemMetadata>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    /// <para>Adds an additional metadata field named "SecondTest" whose value is "No".</para>
    ///   <code lang="MSBuild">
    ///     <![CDATA[
    /// <ItemGroup>
    ///    <Files Include="MSBuildContrib.Common.Tasks.dll">
    ///       <AdditionalProperties>TestFile=True</AdditionalProperties>
    ///    </Files>
    /// </ItemGroup>
    /// <UpdateItemMetadata Items="@(Files)" Metadata="SecondTest=No">
    ///    <Output TaskParameter="ResultItems" ItemName="UpdatedItems" />
    /// </UpdateItemMetadata>
    ///     ]]>
    ///   </code>
    /// </example>
    public class UpdateItemMetadata : Task
    {
        #region events
        #endregion

        #region class-wide fields
        private ITaskItem[] items;
        private string[] metadata;
        private string metadataName;
        private ITaskItem[] resultItems;
        #endregion

        #region private and internal properties and methods

        #region properties
        #endregion

        #region methods

        #region ParseMetadata
        private IDictionary<string, string> ParseMetadata(string[] metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata"); 
            }
            
            IDictionary<string, string> dictionary;
            dictionary = new Dictionary<string, string>(metadata.Length);
            for (int i = 0; i < metadata.Length; i++)
            {
                Log.LogMessage(metadata[i]);
                string[] parts = metadata[i].Split('=');
                if (parts == null || parts.Length < 2)
                {
                    continue;
                }
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                Log.LogMessage(key);
                Log.LogMessage(value);
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #region Items
        /// <summary>
        /// Source items that will have extra metadata attached to it
        /// </summary>
        [Required]
        public ITaskItem[] Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        } 
        #endregion

        #region Metadata
        /// <summary>
        /// The key/value pair of metadata to be applied to the items.
        /// </summary>
        /// <remarks>If <see cref="MetadataName"/> is not provided, a new metadata item will be
        /// created for each key/value pair. If <see cref="MetadataName"/> is provided, a new
        /// metadata item will be created (or an existing one updated) with the values provided.
        /// </remarks>
        [Required]
        public string[] Metadata
        {
            get
            {
                return this.metadata;
            }
            set
            {
                this.metadata = value;
            }
        } 
        #endregion

        #region MetadataName
        /// <summary>
        /// The name of an existing metadata field whose value should be updated to include
        /// the values specified in <see cref="Metadata"/>.
        /// </summary>
        /// <remarks>If the name does not exist, a new metadata item will be created.</remarks>
        public string MetadataName
        {
            get
            {
                return this.metadataName;
            }
            set
            {
                this.metadataName = value;
            }
        } 
        #endregion

        #region ResultItems
        /// <summary>
        /// Gets the result items.
        /// </summary>
        /// <value>The result items.</value>
        [Output]
        public ITaskItem[] ResultItems
        {
            get
            {
                return this.resultItems;
            }
        }
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateItemRegex"/> class.
        /// </summary>
        public UpdateItemMetadata()
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
            this.resultItems = new ITaskItem[items.Length];
            for (int i = 0; i < resultItems.Length; i++)
            {
                ITaskItem newItem = new TaskItem(items[i]);
                this.items[i].CopyMetadataTo(newItem);
                //Need to set this so it is not reset
                newItem.SetMetadata("RecursiveDir", items[i].GetMetadata("RecursiveDir"));

                if (String.IsNullOrEmpty(this.metadataName))
                {
                    IDictionary<string, string> metadataBag = ParseMetadata(this.metadata);
                    foreach (string metadataName in metadataBag.Keys)
                    {
                        newItem.SetMetadata(metadataName, metadataBag[metadataName]);
                    }
                }
                else
                {
                    System.Collections.ArrayList names = new System.Collections.ArrayList(newItem.MetadataNames);
                    string value = String.Empty;
                    if (names.Contains(this.metadataName))
                    {
                        value = String.Format(CultureInfo.InvariantCulture, "{0};{1}", newItem.GetMetadata(this.metadataName), String.Join(";", this.metadata));
                    }
                    else
                    {
                        value = String.Join(";", this.metadata);
                    }
                    newItem.SetMetadata(this.metadataName, value);
                }
                this.resultItems[i] = newItem;
            }

            return success;
        }
        #endregion

        #endregion

        #endregion
    }
}
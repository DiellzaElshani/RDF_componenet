/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Base;
using BH.Engine.Adapters.RDF.Types;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace GraphWebsite
{
    public static partial class Query
    {
        private static Dictionary<Type, string> m_cachedTypeFilePaths = new Dictionary<Type, string>();
        private static Dictionary<string, string> m_validRepositoryRootPaths = new Dictionary<string, string>();

        [Description("The method will look for a file named using standard BHoM filename convention for Types. " +
            "For example, for the type `BH.oM.Structure.Elements.Bar`, the method will look for 'Bar.cs', and the filepath will have to contain the 'namespaceGroup' called `Structure`.")]
        public static string FilePathFromLocalRepository(this Type type, LocalRepositorySettings settings, bool getRelativePath = false)
        {
            // Null guards
            if (type == null || !type.IsBHoMType() || type is ICustomRDFType)
                return null;

            if (settings == null)
                settings = new LocalRepositorySettings();

            // Custom type guard
            if (type != typeof(CustomObjectType) && type is CustomObjectType)
            {
                // if the type is a subtype of `CustomObjectType`, do not attempt to retrieve its file path.
                Log.RecordWarning($"Can not compute the file path for type `{type.FullName}` that is derived from {nameof(CustomObjectType)}.", true);
                return null;
            }

            string repositoryRoot = settings.GitRootPath;

            if (m_validRepositoryRootPaths.ContainsKey(settings.GitRootPath))
                repositoryRoot = m_validRepositoryRootPaths[repositoryRoot];

            if (string.IsNullOrWhiteSpace(repositoryRoot))
            {
                Log.RecordNote($"No path specified in in {nameof(LocalRepositorySettings)}.{nameof(LocalRepositorySettings.GitRootPath)}." +
                    "\nAn attempt to find a valid root path on disk will now be done." +
                    "\nIgnore this message if you don't get any error.");
                TryGetRepositoryRootPath(out repositoryRoot);
            }

            if (!repositoryRoot.IsValidRepositoryRootPath())
            {
                if (!string.IsNullOrWhiteSpace(repositoryRoot))
                    Log.RecordWarning($"The path `{repositoryRoot}` that was specified in {nameof(LocalRepositorySettings)}.{nameof(LocalRepositorySettings.GitRootPath)} does not point to a valid repository root folder. " +
                        $"A valid 'repository root path' points to a directory that contains, among other repositories, also the BHoM repository.");
                else
                    Log.RecordWarning($"No repository root path specified in {nameof(LocalRepositorySettings)}.{nameof(LocalRepositorySettings.GitRootPath)}.");

                Log.RecordWarning($"An attempt to find a valid root path on disk will now be done.", doNotRepeat:true);

                if (TryGetRepositoryRootPath(out repositoryRoot))
                {
                    Log.RecordMessage(Log.MessageLevel.Warning, $"Using `{repositoryRoot}` as repository root path.");

                    m_validRepositoryRootPaths[settings.GitRootPath] = repositoryRoot;
                }
                else
                {
                    Log.RecordError($"Could not find Local repository directory on disk at path: {repositoryRoot}." +
                        $"Please specify a valid repository root path in {nameof(LocalRepositorySettings)}.{nameof(LocalRepositorySettings.GitRootPath)}." +
                        $"A valid 'repository root path' points to a directory that contains, among other repositories, also the BHoM repository.", doNotRepeat: true);
                    return null;
                }
            }

            string filepath = null;

            // Check the cached types first.
            if ((m_cachedTypeFilePaths?.TryGetValue(type, out filepath) ?? false) && !filepath.IsNullOrEmpty())
                return getRelativePath ? filepath?.Replace(repositoryRoot, "") : filepath;

            string typeNameValidChars = type.NameValidChars();
            HashSet<string> allFilePaths = Compute.FilesInRepo(repositoryRoot, settings);

            string nameSpaceGroup = type.Namespace.Split('.')[2]; // [2] selects anything exactly after `BH.oM.` or `BH.Engine.`

            if (nameSpaceGroup == "Adapters" || nameSpaceGroup == "External")
                nameSpaceGroup = type.Namespace.Split('.')[3];

            if (nameSpaceGroup == "Base")
                nameSpaceGroup = "BHoM";
            else
                nameSpaceGroup += "_oM";

            List<string> matchingFilePaths = allFilePaths?.Where(p =>
                Path.GetDirectoryName(p).Contains($"{nameSpaceGroup}") &&
                !Path.GetDirectoryName(p).Contains("Engine") && // believe it or not, it's useful to remove certain exceptions.
                Path.GetFileNameWithoutExtension(p) == typeNameValidChars).ToList();

            if (matchingFilePaths.Count() > 1)
            {
                Log.RecordWarning($"Found more than one matching filepath for `{type.FullName}`: {string.Join(", ", matchingFilePaths)}", true);

                // Store null in cache, which is better than having to reperform the search.
                m_cachedTypeFilePaths[type] = null;

                return null;
            }

            // Try removing the namespaceGroup condition and see if we can find at least one match. Can help for types e.g. Structure_oM hosted in `Structural` folders (e.g. StructuralEngineering_Toolkit).
            if (!matchingFilePaths.Any())
            {
                matchingFilePaths = allFilePaths?.Where(p =>
                    //Path.GetDirectoryName(p).Contains($"_oM") &&
                    !Path.GetDirectoryName(p).Contains("_Engine") &&  // believe it or not, it's useful to remove certain exceptions.
                    Path.GetFileNameWithoutExtension(p) == typeNameValidChars).ToList();
            }

            // Try checking if filename *contains* the type name. Helps for files with non compliant prefixes/suffixes (e.g. _IExecuteCommand.cs in BHoM_Adapter).
            if (!matchingFilePaths.Any())
            {
                matchingFilePaths = allFilePaths?.Where(p =>
                    //Path.GetDirectoryName(p).Contains($"_oM") &&
                    !Path.GetDirectoryName(p).Contains("_Engine") &&  // believe it or not, it's useful to remove certain exceptions.
                    Path.GetFileNameWithoutExtension(p).Contains(typeNameValidChars)).ToList();
            }

            // Try lowercasing
            if (!matchingFilePaths.Any())
            {
                // Try removing the namespaceGroup condition and see if we can find at least one match. Can help for types e.g. Structure_oM hosted in `Structural` folders (e.g. StructuralEngineering_Toolkit).
                matchingFilePaths = allFilePaths?.Where(p =>
                    //Path.GetDirectoryName(p).Contains($"_oM") &&
                    !Path.GetDirectoryName(p).Contains("_Engine") &&  // believe it or not, it's useful to remove certain exceptions.
                    Path.GetFileNameWithoutExtension(p).ToLower() == typeNameValidChars.ToLower()).ToList();
            }

            if (!matchingFilePaths.Any() && !m_FilesCachefileWasRefreshed)
            {
                // Try refreshing the Files cache file and restart the process.
                RefreshFilesCacheFile(settings);
                return FilePathFromLocalRepository(type, settings, getRelativePath);
            }


            if (matchingFilePaths.Count != 1)
                Log.RecordWarning($"Could not find filepath for Type `{typeNameValidChars}`", true);

            filepath = matchingFilePaths.FirstOrDefault();

            // Store in cache.
            if (!filepath.IsNullOrEmpty())
                m_cachedTypeFilePaths[type] = filepath;
            else
            {
                Log.RecordWarning($"Could not compute repository filepath for type {type.FullName}.", true);
            }

            return getRelativePath ? filepath?.Replace(repositoryRoot, "") : filepath; // if not found, this returns null.
        }

        private static void RefreshFilesCacheFile(LocalRepositorySettings settings)
        {
            if (m_FilesCachefileWasRefreshed)
                return; // Was already refreshed in this session

            string cacheFilePath = Path.Combine(settings.CacheRootPath, settings.CacheFileName_RepositoryAllFilePaths);

            if (File.Exists(cacheFilePath))
                File.Delete(cacheFilePath);

            m_FilesCachefileWasRefreshed = true;
        }

        private static bool m_FilesCachefileWasRefreshed = false;
    }
}


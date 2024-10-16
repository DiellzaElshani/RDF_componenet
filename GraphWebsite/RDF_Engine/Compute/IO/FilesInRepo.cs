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

using BH.oM.Adapters.RDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace GraphWebsite
{
    public static partial class Compute
    {
        [Description("Returns All `.cs` file paths found in the specified Git root (e.g. C:/Users/username/GitHub)." +
            "LocalRepositorySettings can be input to specify how to cache this operation to speed it up after its first computationl.")]
        public static HashSet<string> FilesInRepo(string gitRootPath, LocalRepositorySettings settings = null)
        {
            if (settings == null)
                settings = new LocalRepositorySettings();

            if (string.IsNullOrWhiteSpace(gitRootPath) || !Directory.Exists(gitRootPath))
                return null;

            // Read from cached memory.
            if (m_allCsFilePaths != null)
                return m_allCsFilePaths;

            string[] files = null;
            string cacheFilePath = Path.Combine(settings.CacheRootPath, settings.CacheFileName_RepositoryAllFilePaths);

            bool cacheFileReadCorrectly = false;
            if (settings.ReadCacheFiles && !string.IsNullOrWhiteSpace(cacheFilePath) && File.Exists(cacheFilePath))
            {
                // Read from cached disk file.
                files = File.ReadAllLines(cacheFilePath);

                cacheFileReadCorrectly = files?.Any() ?? false;

                // For safety, let`s check if the first 10 files exist on disk
                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        cacheFileReadCorrectly = false;
                        break;
                    }
                }
            }

            if (!cacheFileReadCorrectly)
            {
                // Read the filesystem and get the .cs files.
                files = Directory.GetFiles(gitRootPath, "*.cs", SearchOption.AllDirectories);
                files = files.Where(f =>
                    !f.Contains("TemporaryGeneratedFile_") &&
                    !f.EndsWith("AssemblyInfo.cs") &&
                    !f.EndsWith("Resources.Designer.cs") &&
                    !f.EndsWith("AssemblyAttributes.cs") &&
                    !(f.Contains("packages") && !f.Contains("src")) // removes nuget package source files
                    ).ToArray();
            }

            if (files.IsNullOrEmpty())
            {
                Log.RecordError("Could not compute the file paths.");
                return null;
            }

            // Cache the results to disk.
            if ((settings.WriteCacheFiles && files != null && !string.IsNullOrWhiteSpace(cacheFilePath)) || !cacheFileReadCorrectly)
            {
                if (File.Exists(cacheFilePath))
                    try
                    {
                        File.Delete(cacheFilePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath));
                        File.WriteAllLines(cacheFilePath, files);
                    }
                    catch { }
            }

            // Cache in memory.
            m_allCsFilePaths = new HashSet<string>(files);

            return m_allCsFilePaths;
        }

        // All ".cs" file paths found in the specified Root repository (e.g. C:/Users/alombardi/GitHub).
        private static HashSet<string> m_allCsFilePaths;
    }
}


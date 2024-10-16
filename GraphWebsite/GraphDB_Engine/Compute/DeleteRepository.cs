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

using BH.Engine.Adapters.RDF;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Shacl.Validation;

namespace GraphWebsite
{
    public static partial class Compute
    {
        [Description("Delete data from a GraphDB repository using its REST API.")]
        [Input("serverAddress", "Localhost address where GraphDB is exposed. This can be changed from GraphDB settings file.")]
        [Input("repositoryName", "GraphDB repository name where the graph data is stored.")]
        [Input("run", "Activate the deletion.")]
        public static void DeleteRepository(string serverAddress = "http://localhost:7200/", string repositoryName = "BHoMVisualization", bool run = false)
        {
            var client = new HttpClient();

            // HTTP Delete Request, new endpoint (with {RepositoryID} at the end)
            var endpointDeletRepo = new Uri(serverAddress + "rest/repositories/" + repositoryName);
            var result2 = client.DeleteAsync(endpointDeletRepo).Result;
        }
    }
}


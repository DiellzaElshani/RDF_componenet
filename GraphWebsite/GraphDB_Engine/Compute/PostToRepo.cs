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
using BH.UI.Engine.GraphDB;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GraphWebsite
{
    public static partial class Compute
    {
        [Description("Push RDF data from a TTL file to GraphDB using its REST API.")]
        [Input("TTLfilePath", "Turtle file where your RDF data was saved.")]
        [Input("serverAddress", "Localhost address where GraphDB is exposed. This can be changed from GraphDB settings file.")]
        [Input("repositoryName", "GraphDB repository name where the graph data is stored.")]
        [Input("run", "Activate the push.")]
        public static async Task<bool> PostToRepo(string TTLfilePath, string username = "" ,  string serverAddress = "http://localhost:7200/", string repositoryName = "BHoMVisualization", string graphName = "defaultGraph", bool clearGraph = false, bool run = false)
        {
            if (!run)
            {
                Log.RecordWarning("To push data to GraphDB press the Button or switch the Toggle to true");
                return false;
            }


            // Documentation in GraphDB: http://localhost:7200/webapi

            var httpClient = new HttpClient();


            if (!Uri.TryCreate(serverAddress, UriKind.Absolute, out Uri validServerAddress))
            {
                Log.RecordError($"Invalid URI input in {nameof(serverAddress)}");
            }

            // Set the GraphDB REST API URL for creating a repository
            string apiUrl = $"{serverAddress}rest/repositories";


            // Check if Login is required

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var responseSec = await httpClient.GetAsync(serverAddress + "rest/security");

            if (!responseSec.IsSuccessStatusCode)
            {
                Log.RecordWarning("Security request failed");
                return false;
            }

            var content = await responseSec.Content.ReadAsStringAsync();

            // trigger log in depening on security request result

            if (bool.Parse(content)) 
            {

                // retrive Login data
                LoginDataRetriever retriever = new LoginDataRetriever();
                string password = retriever.RetrievePassword(serverAddress, username);
                // if (string.IsNullOrEmpty(password)) // To-Do put into class for better reuse in pull / graphDBAdapter to-do password check
                

                password = retriever.RetrievePassword(serverAddress, username);
                var byteArray = Encoding.ASCII.GetBytes( username + ":" + password);
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(byteArray));

                var responseLogin = await httpClient.GetAsync(serverAddress + "rest/security");

                if (!responseLogin.IsSuccessStatusCode)
                {
                    Log.RecordWarning("Login request failed");
                    return false;
                }
                var contentLogin = await responseLogin.Content.ReadAsStringAsync();
                if(!bool.Parse(contentLogin))
                {
                    Log.RecordWarning("Login credentials invalid");
                    return false    ;
                }
            }

            // Read the configuration template
            string configTemplate = File.ReadAllText(@"C:\ProgramData\BHoM\Assemblies\repository-config.ttl");
            string config = configTemplate.Replace("{{REPOSITORY_ID}}", repositoryName);

            //// Read the configuration template
            HttpContent fileStreamContent = new StringContent(config, Encoding.UTF8, "application/xml");

            // Create the request content
            var formData = new MultipartFormDataContent();
            formData.Add(fileStreamContent, name: "config", fileName: "repo-config.ttl");

            // Send a POST request to create the repository
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, formData);

            // Check if the response is successful
            if (!response.IsSuccessStatusCode)
                Log.RecordWarning($"Failed to create repository '{repositoryName}': {response.ReasonPhrase}");

            // Check if user wants to clear the namedGraph before pushing
            if (clearGraph==true) 
            {
                var clearResponse = await httpClient.DeleteAsync(serverAddress + "repositories/" + repositoryName + "/rdf-graphs/" + graphName);
                if (!clearResponse.IsSuccessStatusCode)
                {
                    Log.RecordWarning("namedGraph was not cleared");
                    return false;
                }
            }

            // Post Data to Repository (also update data)
            String ttlBHoMFile = File.ReadAllText(TTLfilePath);
            StringContent ttlFile = new StringContent(ttlBHoMFile);
            ttlFile.Headers.ContentType = new MediaTypeHeaderValue("text/turtle");


            var endpointRepoPostData = new Uri(serverAddress + "repositories/" + repositoryName + "/rdf-graphs/" + graphName);
            var resultData = await httpClient.PutAsync(endpointRepoPostData, ttlFile);
            //string jsonData = resultData.Content.ReadAsStringAsync().Result;

            return true;
        }
    }
}


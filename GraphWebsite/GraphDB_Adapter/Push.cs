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

using BH.Adapter;
using BH.Engine.Adapters.RDF;
using BH.oM.Adapter;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.RDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapters;
using Log = BH.Engine.Adapters.RDF.Log;
using Compute = BH.Engine.Adapters.GraphDB.Compute;
using BH.Adapters.TTL;
using System.IO;

namespace GraphWebsite
{
    public partial class GraphDBAdapter : BHoMAdapter
    {
        public override List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.UpdateOrCreateOnly, ActionConfig actionConfig = null)
        {

            string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string TTLfilepath = Path.Combine(userDirectory, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_GraphDBPush.ttl");

            // Creates a Turtle file.
            TTLAdapter ttlAdapter = new TTLAdapter(TTLfilepath, m_graphSettings, m_localRepositorySettings);
            ttlAdapter.Push(objects);

            // Start the actual task we care about (don't await it)
            Task<bool> task = Compute.PostToRepo(TTLfilepath, m_username, m_serverAddress, m_repositoryName, m_graphName, false, true);

            // AL: Keep these comments for reference.
            // The below code should work, but for some reason PostToRepo doesn't state its completion.
            // So we cannot wait for it to complete -- we are forced to return from the Push even though we don't know about its actual completion.
            // This highlights the need for async Adapter methods in BHoM. 

            //// Create the timeout task (don't await it)
            //var timeoutTask = Task.Delay(m_pushTimeoutMillisec);
            //// Run the task and timeout in parallel
            //Task.WhenAny(task, timeoutTask).Wait();

            //if (task.Status != TaskStatus.RanToCompletion)
            //{
            //    Log.RecordError($"Encountered timeout for Push, to increase timeout duration, increase in {nameof(GraphDBAdapter)}");
            //    return null;
            //}


            return objects.ToList();
        }
    }
}

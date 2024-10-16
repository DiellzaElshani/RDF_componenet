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
        public override IEnumerable<object> Pull(oM.Data.Requests.IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            List<object> pullResult = new List<object>();
            // Posts the content of the Turtle file to GraphDB.
            var result = Compute.PullFromRepo(m_serverAddress, m_repositoryName, run:true);

            pullResult.Add(result);

            return pullResult;
        }
    }
}

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
using VDS.RDF.Ontology;
using BH.Adapters;
using Log = BH.Engine.Adapters.RDF.Log;
using Convert = BH.Engine.Adapters.Markdown.Convert;
using Compute = BH.Engine.Adapters.Markdown.Compute;

namespace GraphWebsite

	public partial class MarkdownAdapter : BHoMAdapter
    {
        public override List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.AdapterDefault, ActionConfig actionConfig = null)
        {
            List<Type> types = objects.OfType<Type>().ToList();

            if (types.Count != objects.Count())
            {
                Log.RecordWarning($"The {nameof(MarkdownAdapter)} only supports the Push of Types, not of object instances. It is designed to build only an ontology's T-box.");
            }

            return new List<object>() { Convert.ToMarkdown(types, m_graphSettings, m_localRepositorySettings) };
        }
    }
}

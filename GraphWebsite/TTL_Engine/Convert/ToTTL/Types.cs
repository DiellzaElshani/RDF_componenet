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


using BH.Engine.Adapters.TTL;
using BH.oM.Adapters.RDF;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GraphWebsite
{
	public static partial class Convert
	{
		[Description("Computes a TTL T-Box ontology with the input Types." +
			"To compute an ontology that includes both T-Box and A-Box, use the TTLGraph method that takes a list of IObjects, and provide input objects (instances) instead of Types.")]
		public static string ToTTL(this List<Type> types, GraphSettings graphSettings = null, LocalRepositorySettings localRepositorySettings = null)
		{
			localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();
			graphSettings = graphSettings ?? new GraphSettings();

			CSharpGraph cSharpGraph = BH.Engine.Adapters.RDF.Compute.CSharpGraph(types, graphSettings);

			string TTL = cSharpGraph.ToTTL(localRepositorySettings);

			return TTL;
		}
	}
}


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
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace GraphWebsite
{
	public partial class TTLAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Adapter for TTL.")]
        [Output("The created TTL adapter.")]
        public TTLAdapter(string filepath = null, GraphSettings graphSettings = null, LocalRepositorySettings localRepositorySettings = null)
        {
            m_AdapterSettings.DefaultPushType = BH.oM.Adapter.PushType.CreateOnly; // Adapter `Push` Action simply calls "Create" method.

            m_filepath = filepath;
            m_graphSettings = graphSettings ?? new GraphSettings();
            m_localRepositorySettings = localRepositorySettings;
        }

        private readonly string m_filepath;
        private GraphSettings m_graphSettings = new GraphSettings();
        private readonly LocalRepositorySettings m_localRepositorySettings;
    }
}

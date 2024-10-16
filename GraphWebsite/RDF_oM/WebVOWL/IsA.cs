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

using System.ComponentModel;

namespace GraphWebsite
{
	[Description("Can be used to describe that a thing belongs to a class; or that a thing is equivalent to another thing. " +
        "When used for BHoM types, this describes C# interface implementation.")]
    public class IsA : IRelation
    {
        [Description("Thing that can be included in the class, or that is equivalent to something else.")]
        public object Subject { get; set; }
        [Description("The class that can include the thing, or another thing that is equivalent to the thing.")]
        public object Object { get; set; }

        public bool IsBidirectional { get; set; } = false;
    }
}


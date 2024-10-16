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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Base;

namespace GraphWebsite
{
    public static partial class Query
    {
        public static bool IsListOfOntologyClasses(this Type sourceType, TBoxSettings tBoxSettings, object sourceObj = null)
        {
            tBoxSettings = tBoxSettings ?? new TBoxSettings();

            if (sourceType == null)
                Log.RecordError($"Argument {nameof(sourceType)} cannot be null", typeof(ArgumentNullException));

            // Make sure the type is a List.
            if (!sourceType.IsList())
                return false;

            // Check the List generic argument.
            Type[] genericArgs = sourceType.GetGenericArguments();

            if (genericArgs.Length != 1)
                return false;

            // If the List generic arg can be translated to an Ontology class, job done.
            if (genericArgs.First() != typeof(System.Object))
                return genericArgs.First().IsOntologyClass(tBoxSettings);

            // If the List generic arg is System.Object, the objects may still be Ontology classes that have been boxed.
            if (sourceObj != null && genericArgs.First() == typeof(System.Object))
            {
                List<object> objList = sourceObj as List<object>;

                if (objList == null)
                    return false;

                // Unbox the objects and see if their actual type is an Ontology class.
                return objList.All(o => o.GetType().IsOntologyClass(tBoxSettings));
            }

            return false;
        }

        public static bool? IsListOfOntologyClasses(this IndividualObjectProperty iop, TBoxSettings tBoxSettings)
        {
            Type rangeType = iop.RangeIndividual?.GetType();

            return IsListOfOntologyClasses(rangeType, tBoxSettings, iop.RangeIndividual);
        }

        private static bool IsList(this Type t)
        {
            if (t == null)
                return false;

            return typeof(IList).IsAssignableFrom(t);
        }
    }
}


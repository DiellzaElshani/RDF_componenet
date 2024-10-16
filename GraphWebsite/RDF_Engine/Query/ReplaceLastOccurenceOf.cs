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

using BH.oM.Base;
using BH.oM.Adapters.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphWebsite
{
    public static partial class Query
    {
        public static string ReplaceLastOccurenceOf(this string text, char ch, string replaceWith)
        {
            var lastOccurrence = text.LastIndexOf(ch);
            if (lastOccurrence != -1) 
                text = text.Remove(lastOccurrence, 1).Insert(lastOccurrence, replaceWith);

            return text;
        }

        public static string EnsureEndingDot(this string text)
        {
            text = text.TrimEnd(' ');
            text = text.TrimEnd('\n');

            if (text.EndsWith("."))
                return text;

            var lastSemicolon = text.LastIndexOf(';');

            if (text.EndsWith(";"))
                text = text.Remove(lastSemicolon, 1);

            return text + " .";
        }
    }
}


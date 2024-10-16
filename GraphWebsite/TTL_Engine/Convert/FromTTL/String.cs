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
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using VDS.RDF.Ontology;

namespace GraphWebsite
{
	public static partial class Convert
    {
        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        public static List<object> ToCSharpObjects(this string TTLOntology, GraphSettings graphSettings = null)
        {
            graphSettings = graphSettings ?? ExtractOntologySettings(TTLOntology);

            OntologyGraph ont = BH.Engine.Adapters.RDF.Convert.ToDotNetRDF(TTLOntology);
            return BH.Engine.Adapters.RDF.Convert.FromDotNetRDF(ont, graphSettings);
        }

        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        [MultiOutputAttribute(0,"KG","Ontology produced")]
        [MultiOutputAttribute(1, "OS", "Ontology Settings used to construct this KG")]
        public static Output<List<object>, GraphSettings> FromTTL(string TTLtext)
        {
            if (string.IsNullOrWhiteSpace(TTLtext))
                return new Output<List<object>, GraphSettings>();

            GraphSettings graphSettings = ExtractOntologySettings(TTLtext);

            Output<List<object>, GraphSettings> output = new Output<List<object>, GraphSettings>
            {
                Item1 = GraphWebsite.Convert.ToCSharpObjects(TTLtext, graphSettings),
                Item2 = graphSettings
            };
            return output;
        }


        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        public static Output<List<object>, GraphSettings> FromTTL(string TTLfilePath, bool active = false)
        {
            if (!active)
                return new Output<List<object>, GraphSettings>();

            string TTLtext = File.ReadAllText(TTLfilePath);
            Output<List<object>, GraphSettings> readTTLOutput = FromTTL(TTLtext);

            return readTTLOutput;
        }

        public static GraphSettings ExtractOntologySettings(string TTLtext)
        {
            string graphSettingsDeclaration = $"# {nameof(GraphSettings)}: ";

            foreach (var line in TTLtext.SplitToLines())
            {
                if (line.Contains(graphSettingsDeclaration))
                    return BH.Engine.Adapters.RDF.Convert.FromBase64JsonSerialized(line.Replace(graphSettingsDeclaration, "")) as GraphSettings ?? new GraphSettings();
            }

            return new GraphSettings();
        }

        private static IEnumerable<string> SplitToLines(this string input)
        {
            if (input == null)
            {
                yield break;
            }

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader?.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}


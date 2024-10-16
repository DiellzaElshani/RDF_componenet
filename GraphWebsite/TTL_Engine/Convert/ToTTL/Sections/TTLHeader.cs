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

namespace GraphWebsite
{
	public partial class Convert
    {
        private static string TTLHeader(GraphSettings graphSettings, 
            bool includeOwl = true, bool includeRdf = true, bool includeRdfs = true, bool includeXml = true, bool includeXsd = true)
        {
            string header = $"@prefix : <{graphSettings.OntologyBaseAddress}/> .";
            if (includeOwl) header += "\n@prefix owl: <http://www.w3.org/2002/07/owl#> .";
            if (includeRdf) header += "\n@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .";
            if (includeXml) header += "\n@prefix xml: <http://www.w3.org/XML/1998/namespace> .";
            if (includeXsd) header += "\n@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .";
            if (includeRdfs) header += "\n@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .";
            if (includeRdfs) header += "\n@prefix dc: <http://purl.org/dc/elements/1.1/> .";

            header += "\n@base   " + $@"<{graphSettings.OntologyBaseAddress}> .";
            

            header += "\n";

            header += "\n"+$@"<{graphSettings.OntologyBaseAddress}> rdf:type owl:Ontology;
                          dc:title ""{graphSettings.OntologyTitle}""@en;
                          dc:description ""{graphSettings.OntologyDescription}""@en.";

            return header;
        }
    }
}


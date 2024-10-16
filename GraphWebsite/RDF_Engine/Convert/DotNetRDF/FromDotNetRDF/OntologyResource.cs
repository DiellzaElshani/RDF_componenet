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
using VDS.RDF;
using VDS.RDF.Ontology;
using VDS.RDF.Query.Algebra;
using VDS.RDF.Nodes;

namespace GraphWebsite
{
    public static partial class Convert
    {
        /*************************************/
        /*          Public methods           */
        /*************************************/

        public static object FromDotNetRDF(this OntologyResource individual, OntologyGraph dotNetRDFOntology)
        {
            if (individual == null || dotNetRDFOntology == null)
                return null;

            Type individualType = individual.EquivalentType();
            List<PropertyInfo> typeProperties = individualType.GetProperties().ToList();
            typeProperties.AddRange(individualType.BaseTypes().SelectMany(t => t.GetProperties()));

            object resultObject = null;
            try
            {
                resultObject = Activator.CreateInstance(individualType);
            }
            catch (Exception e)
            {
                Log.RecordWarning($"The conversion does not support the type {individualType.FullName} yet. Error:\n{e.Message}", exceptionType: typeof(ArgumentException));
                return null;
            }

            if (resultObject == null)
                return null;

            bool isCustomType = typeof(Types.CustomObjectType).IsAssignableFrom(individualType.GetType());

            if (isCustomType)
                ((CustomObject)resultObject).CustomData[new TBoxSettings().CustomobjectsTypeKey] = individualType.Name;

            // Get the equivalent properties
            var ontprops = dotNetRDFOntology.OwlProperties.Where(p => p.UsedBy.Any(n => n.Types.Any(uriN => uriN.ToString().Contains(individualType.FullName))));
            HashSet<OntologyProperty> propertyNodes = new HashSet<OntologyProperty>(ontprops);

            foreach (OntologyProperty propertyNode in propertyNodes)
            {
                resultObject.SetOntologyProperty(propertyNode, dotNetRDFOntology, individual, individualType, typeProperties);
            }

            return resultObject;


        }

        /*************************************/
        /*          Private methods          */
        /*************************************/

        private static void SetOntologyProperty(this object resultObject, OntologyProperty propertyNode, OntologyGraph dotNetRDFOntology, OntologyResource individual, Type individualType, List<PropertyInfo> typeProperties)
        {
            if (!propertyNode.Label.FirstOrDefault()?.Value.Contains("BH.oM") ?? false)
            { 
                // This is a custom property.
                // We should skip it because its deserialization is already covered 
                // by the CustomData dictionary deserialization.
                return; 
            }

            Triple predicateNode = propertyNode.TriplesWithPredicate
             .Where(t => (t.Subject as UriNode)?.Uri.Segments.LastOrDefault() == individual.Resource.Uri().Segments.LastOrDefault())
             .FirstOrDefault();

            if (predicateNode == null)
                return;

            string propertyFullName = predicateNode.Predicate.BHoMSegment();
            object propertyValue = null;
            Type propertyType = typeof(object);

            LiteralNode literalNode = predicateNode.Object as LiteralNode;
            if (literalNode != null)
            {
                propertyValue = literalNode.GetPropertyValue();
                propertyType = literalNode.GetPropertyType();
            }

            if (propertyFullName.EndsWith(nameof(IBHoMObject.BHoM_Guid)))
                propertyType = typeof(Guid);

            UriNode uriNode = predicateNode.Object as UriNode;
            if (uriNode != null)
            {
                // Check if it is a List.
                // Could not find a more reliable way that checking the uri address for mentions of "rdf" and "seq".
                string typeAddress = uriNode.ToString().ToLower();
                
                if (typeAddress.EndsWith(m_graphSettings.ABoxSettings.SequenceIndentifierSuffix)) // TODO: make more robust with more unique characters, i.e. change the ToTTL method and this check.
                {
                    // We know it's a list.
                    SortedDictionary<int, int> listIdx_tripleIdx = new SortedDictionary<int, int>();

                    Triple triplePointingToListNode = null;
                    for (int i = 0; i < individual.TriplesWithSubject.Count(); i++)
                    {
                        Triple triple = individual.TriplesWithSubject.ElementAtOrDefault(i);

                        if (triple.Predicate.Uri().ToString().Contains(propertyFullName))
                        {
                            triplePointingToListNode = individual.TriplesWithSubject.ElementAt(i);
                        }
                    }

                    var listNode = triplePointingToListNode.Object;

                    // Get all the triples in the Graph.
                    var allTriples = dotNetRDFOntology.Triples;

                    // TODO: Find the triples that have as a subject the listNode
                    var triplesWithListNodeSubject = allTriples.Where(t => t.Subject.Uri() == listNode.Uri()).ToList();

                    for (int i = 0; i < triplesWithListNodeSubject.Count(); i++)
                    {
                        Triple triple = triplesWithListNodeSubject.ElementAtOrDefault(i);

                        if (!triple.Predicate.Uri().ToString().Contains("_")) // TODO: the check needs to be more robust.
                        {
                            // I.e. we need to verify that `Uri().ToString()` ends with an underscore followed by any integer number.
                            continue;
                        }

                        int rdfListIndexFound = -1;
                        string predicateUri = (triple?.Predicate as UriNode)?.Uri.ToString();
                        string listIdxString = predicateUri?.Split('_')?.LastOrDefault();

                        if (int.TryParse(listIdxString, out rdfListIndexFound))
                            listIdx_tripleIdx[rdfListIndexFound] = i;
                        else
                        {
                            // Reached end of the sequence.
                            break;
                        }
                    }

                    List<object> listValues = new List<object>();
                    foreach (var kv in listIdx_tripleIdx)
                    {
                        Triple listItemTriple = triplesWithListNodeSubject.ElementAt(kv.Value);
                        OntologyResource listIndividual = listItemTriple.Object.IndividualOntologyResource(dotNetRDFOntology);

                        object convertedIndividual;

                        if (listIndividual == null && listItemTriple.Object is VDS.RDF.LiteralNode literal)
                        {
                            // Take the literal and convert it to the correct type.
                            string XsdTypeName = literal.DataType.Fragment.TrimStart('#');
                            if (OntologyDataTypeMap.FromOntologyDataType.TryGetValue(XsdTypeName, out Type typeToConvertTo))
                                convertedIndividual = System.Convert.ChangeType(literal.Value, typeToConvertTo);
                            else
                            {
                                // It's either a string, or we keep its string value.
                                convertedIndividual = literal.Value;
                            }
                        }
                        else
                            convertedIndividual = listIndividual.FromDotNetRDF(dotNetRDFOntology);
                        
                        listValues.Add(convertedIndividual);
                    }

                    propertyValue = listValues;
                }
                else
                {
                    OntologyResource relatedIndividual = uriNode.IndividualOntologyResource(dotNetRDFOntology);
                    propertyValue = relatedIndividual.FromDotNetRDF(dotNetRDFOntology);
                }
            }

            if (predicateNode.Object is BlankNode)
            {
                BlankNode listStart = (BlankNode)predicateNode.Object;

                string nextId = listStart.InternalID;

                List<object> listValues = new List<object>();
                Type listItemsType = null;

                for (int i = 0; i < dotNetRDFOntology.Triples.Count; i++)
                {
                    Triple triple = dotNetRDFOntology.Triples.ElementAt(i);
                    BlankNode bn = triple.Subject as BlankNode;

                    if (bn == null)
                        continue;

                    if (bn.InternalID == nextId)
                    {
                        LiteralNode valueNode = triple.Object as LiteralNode;

                        if (valueNode != null)
                        {
                            if (listItemsType == null)
                                listItemsType = GetPropertyType(valueNode);

                            object listItemValue = ConvertValue(valueNode.GetPropertyValue(), listItemsType);
                            listValues.Add(listItemValue);
                            continue;
                        }

                        BlankNode objectBn = triple.Object as BlankNode;

                        if (objectBn == null)
                            break;

                        nextId = objectBn.InternalID;
                    }
                }

                propertyValue = listValues;
                propertyType = typeof(List<>).MakeGenericType(listItemsType ?? typeof(object));
            }

            PropertyInfo correspondingPInfo = typeProperties.FirstOrDefault(pi => pi.FullNameValidChars() == propertyFullName);

            var bhomObjProperties = typeof(IBHoMObject).GetProperties().Concat(typeof(BHoMObject).GetProperties());
            correspondingPInfo = correspondingPInfo ?? bhomObjProperties.SingleOrDefault(p => p.FullNameValidChars().Contains(propertyFullName));

            if (correspondingPInfo == null)
            {
                string propertyName = propertyFullName.Split('.').LastOrDefault();
                correspondingPInfo = new Types.CustomPropertyInfo(individualType as Types.CustomObjectType, new KeyValuePair<string, Type>(propertyName, propertyType));
            }

            object convertedValue = ConvertValue(propertyValue, correspondingPInfo.PropertyType);

            SetValueOnResultObject(ref resultObject, correspondingPInfo, convertedValue);
        }

        /*************************************/

        private static object GetPropertyValue(this LiteralNode literalNode)
        {
            if (literalNode == null)
                return null;

            if (literalNode.DataType == null)
                return literalNode.Value; // When the datatype is null, assume this is a string and just return the value (#110).
            else if (literalNode.DataType.AbsolutePath.EndsWith(typeof(Base64JsonSerialized).FullName))
                return literalNode.Value.FromBase64JsonSerialized();
            else
                return literalNode.Value;
        }

        /*************************************/

        private static Type GetPropertyType(this LiteralNode literalNode)
        {
            if (literalNode.DataType == null)
                return typeof(string); // When the datatype is null, assume this is a string and just return the value (#110).

            return GetPropertyType(literalNode.DataType.Fragment);
        }

        private static Type GetPropertyType(this string valueName)
        {
            return OntologyDataTypeMap.FromOntologyDataType.Where(kv => kv.Key.Contains(valueName.OnlyAlphabetic())).FirstOrDefault().Value ?? typeof(object);
        }

        /*************************************/

        private static void SetValueOnResultObject(ref object resultObject, PropertyInfo pInfo, object convertedValue)
        {
            // If the property is CustomData, first check whether it wasn't already set (e.g. via a CustomPropertyInfo set).
            if (pInfo.Name == nameof(BHoMObject.CustomData) && resultObject is BHoMObject && convertedValue is Dictionary<string, object>)
            {
                BHoMObject resultBHoMObj = (BHoMObject)resultObject;

                if (resultBHoMObj?.CustomData != null)
                {
                    // Only add CustomData keys that are not already set.
                    foreach (var kv in (Dictionary<string, object>)convertedValue)
                    {
                        if (!resultBHoMObj.CustomData.ContainsKey(kv.Key))
                            resultBHoMObj.CustomData[kv.Key] = kv.Value;
                    }
                    return;
                }
            }

            // Set the convertedValue to the property.
            try
            {
                pInfo.SetValue(resultObject, convertedValue);
                return;
            }
            catch { }

            var field = resultObject.GetType().GetField($"<{pInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
            {
                try
                {
                    field.SetValue(resultObject, convertedValue);
                    return;
                }
                catch { }
            }
            else
            {
                // Try set as Custom Data.
                if (resultObject is BHoMObject)
                    try
                    {
                        (resultObject as dynamic).CustomData[pInfo.Name] = convertedValue;
                        return;
                    }
                    catch { }
            }

            Log.RecordWarning($"Could not set property `{pInfo.Name}` on a {resultObject.GetType().FullName}.");
        }

        /*************************************/

        private static object ConvertValue(object value, Type destinationType)
        {
            object convertedValue = null;

            try
            {
                convertedValue = System.Convert.ChangeType(value, destinationType);
                return convertedValue;
            }
            catch { }


            if (convertedValue == null)
            {
                if (destinationType == typeof(Guid) && Guid.TryParse(value.ToString(), out Guid guid))
                    return guid;

                if (typeof(IList).IsAssignableFrom(destinationType))
                {
                    List<object> valueList = value as List<object>;

                    if (valueList != null)
                    {
                        // Convert list of objects to list of specific inner type
                        Type listGenericArgument = destinationType.GetGenericArguments()[0];
                        var methodInfo = typeof(Queryable).GetMethod("OfType");
                        var genericMethod = methodInfo?.MakeGenericMethod(listGenericArgument);
                        try
                        {
                            Type listType = typeof(List<>).MakeGenericType(listGenericArgument);
                            IList list = Activator.CreateInstance(listType) as IList;
                            IList listOfDeserializedType = null;
                            foreach (var item in valueList)
                            {
                                if (item is string st && Convert.TryDeserializeBase64Json(st, out object innerRes))
                                {
                                    // This is required since we decided to separate geometry: DataType lists wouldn't support Base64JsonSerialized as DataType,
                                    // so we store Base64JsonSerialized as xls:string, and this further check is required.

                                    if (listOfDeserializedType == null)
                                    {
                                        listType = typeof(List<>).MakeGenericType(innerRes.GetType());
                                        listOfDeserializedType = Activator.CreateInstance(listType) as IList;
                                    }

                                    listOfDeserializedType.Add(innerRes);
                                }
                                else
                                    list.Add(item);
                            }

                            if (listOfDeserializedType != null)
                                return listOfDeserializedType;
                            else
                                return list;
                        }
                        catch { }
                    }
                }
            }

            if (value is string str && Convert.TryDeserializeBase64Json(str, out convertedValue))
                return convertedValue;

            // Fallback: try keeping the unconverted value.
            return value;
        }

        /*************************************/

        private static Uri Uri(this INode node)
        {
            return (node as UriNode)?.Uri;
        }
    }
}

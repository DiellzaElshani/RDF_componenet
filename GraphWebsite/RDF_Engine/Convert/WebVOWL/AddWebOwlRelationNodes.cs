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


using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Adapters.RDF;
using Newtonsoft.Json.Linq;
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
    public static partial class Convert
    {
        private static void AddWebOwlRelationNodes(IsSubclassOf isSubclassOf,
                                                    JArray classArray,
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedWebVOWLNodeIds,
                                                    LocalRepositorySettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = isSubclassOf.UniqueNodeId();
            if (addedWebVOWLNodeIds.Contains(propertyTypeRelationId))
                return;

            Type domainType = isSubclassOf.Subject as Type;
            Type rangeType = isSubclassOf.Object as Type;

            // Filter using input exceptions.
            if (exceptions?.Contains(rangeType.FullName) ?? false)
                return;

            // Filter using default exceptions. These apply only when the domainType namespace is not "BH.oM.Base". Useful to remove uninteresting relations.
            //if ((!domainType?.Namespace.StartsWith("BH.oM.Base") ?? false) && (rangeType.Name == "BHoMObject"))
            //    return;

            if (!rangeType.IsBHoMType())
                return;

            if (domainType == null || rangeType == null)
            {
                Log.RecordError($"Cannot add IsA relation `{isSubclassOf.UniqueNodeId()}`");
                return;
            }

            string propertyTypeNodeId = rangeType.UniqueNodeId();

            // See if we have yet to add a Node for the Relation.Object type.
            if (!addedWebVOWLNodeIds.Contains(propertyTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(propertyTypeNodeId, rangeType.OntologyUri(new TBoxSettings(), settings), rangeType.DescriptiveName(true), !rangeType.IsInNamespace(internalNamespaces) ?? false);

                addedWebVOWLNodeIds.Add(propertyTypeNodeId);
            }

            // Add the "IsSubclassOf" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "rdfs:subClassOf");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, new Uri(typeof(IsSubclassOf).OntologyUri(new TBoxSettings(), settings).ToString(), UriKind.Absolute), typeof(IsA).Name, attributes: new List<string>() { "object" }, domain: new List<string>() { domainType.UniqueNodeId() }, range: new List<string>() { rangeType.UniqueNodeId() }, label_iriBased: null);
            addedWebVOWLNodeIds.Add(propertyTypeRelationId);

            // Add other relations for the range PropertyTypeNode
            if (recursionLevel > 0)
            {
                var relations = rangeType.RelationsFromType();
                foreach (IRelation relation in relations)
                    AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedWebVOWLNodeIds, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(IsAListOf isAListOfRelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedWebVOWLNodeIds,
                                                    LocalRepositorySettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = isAListOfRelation.UniqueNodeId();
            if (addedWebVOWLNodeIds.Contains(propertyTypeRelationId))
                return;

            Type domainType = isAListOfRelation.Object as Type;
            Type rangeType = isAListOfRelation.Object as Type;

            if (exceptions?.Contains(rangeType.FullName) ?? false)
                return;

            if (domainType == null || rangeType == null)
            {
                Log.RecordError($"Cannot add IsAListOf relation `{isAListOfRelation.UniqueNodeId()}`");
                return;
            }

            string propertyTypeNodeId = rangeType.UniqueNodeId();

            // See if we have yet to add a Node for the Relation.Object type.
            if (!addedWebVOWLNodeIds.Contains(propertyTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(propertyTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(propertyTypeNodeId, rangeType.OntologyUri(new TBoxSettings(), settings), rangeType.DescriptiveName(true), !rangeType.IsInNamespace(internalNamespaces) ?? false);

                addedWebVOWLNodeIds.Add(propertyTypeNodeId);
            }

            // Add the "IsAListOf" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:ObjectProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, new Uri(typeof(IsAListOf).OntologyUri(new TBoxSettings(), settings).ToString(), UriKind.Absolute), typeof(IsAListOf).Name, attributes: new List<string>() { "object" }, domain: new List<string>() { domainType.UniqueNodeId() }, range: new List<string>() { rangeType.UniqueNodeId() }, label_iriBased: null);

            // Add other relations for the range PropertyTypeNode
            if (recursionLevel > 0)
            {
                var relations = rangeType.RelationsFromType();
                foreach (IRelation relation in relations)
                    AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedWebVOWLNodeIds, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(IsA isARelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedWebVOWLNodeIds,
                                                    LocalRepositorySettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = isARelation.UniqueNodeId();
            if (addedWebVOWLNodeIds.Contains(propertyTypeRelationId))
                return;

            Type domainType = isARelation.Subject as Type;
            Type rangeType = isARelation.Object as Type;

            // Filter specific exceptions
            if (exceptions?.Contains(rangeType.FullNameValidChars()) ?? false)
                return;

            // Filter using default exceptions. These apply only when the domainType namespace is not "BH.oM.Base". Useful to remove uninteresting relations.
            //if ((!domainType?.Namespace.StartsWith("BH.oM.Base") ?? false) && (rangeType.Name == "IObject" || rangeType.Name == "IBHoMObject"))
            //    return;

            if (domainType == null || rangeType == null)
            {
                Log.RecordError($"Cannot add IsA relation `{isARelation.UniqueNodeId()}`");
                return;
            }

            if (!domainType.IsBHoMType() || !rangeType.IsBHoMType())
            {
                // Do not record this relationship.
                return;
            }

            // See if we have yet to add a Node for the Relation.Subject (domain) type.
            string domainTypeNodeId = AddWebOwlClassNodes(domainType, classArray, classAttributeArray, addedWebVOWLNodeIds, settings, internalNamespaces);

            // See if we have yet to add a Node for the Relation.Object (range) type.
            string rangeTypeNodeId = AddWebOwlClassNodes(rangeType, classArray, classAttributeArray, addedWebVOWLNodeIds, settings, internalNamespaces);

            // Add the "IsA" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:ObjectProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, typeof(IsA).OntologyUri(new TBoxSettings(), settings), typeof(IsA).Name, false, new List<string>() { "object" }, typeof(IsA).DescriptionInAttribute(), new List<string>() { domainTypeNodeId }, new List<string>() { rangeTypeNodeId });
            addedWebVOWLNodeIds.Add(propertyTypeRelationId);

            // Add other relations for the range PropertyTypeNode
            if (recursionLevel > 0)
            {
                var relations = rangeType.RelationsFromType();
                foreach (IRelation relation in relations)
                    AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedWebVOWLNodeIds, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(HasProperty hasPropertyRelation,
                                                JArray classArray,
                                                JArray classAttributeArray,
                                                HashSet<string> addedWebVOWLNodeIds,
                                                LocalRepositorySettings settings,
                                                JArray propertyArray = null,
                                                JArray propertyAttributeArray = null,
                                                HashSet<string> internalNamespaces = null,
                                                HashSet<string> exceptions = null,
                                                int recursionLevel = 0)
        {
            // This is the `IRelation.Object` or "range" (to avoid confusion on names)
            Type domainType = hasPropertyRelation.Subject as Type;
            PropertyInfo rangePropertyInfo = hasPropertyRelation.Object as PropertyInfo;

            if (domainType == null)
            {
                Log.RecordError($"The {nameof(HasProperty)} relation `{hasPropertyRelation.UniqueNodeId()}` has its {nameof(IRelation.Subject)} of type `{hasPropertyRelation.Object.GetType().FullName}` instead of {nameof(Type)}.");
                return;
            }

            if (rangePropertyInfo == null)
            {
                Log.RecordError($"The {nameof(HasProperty)} relation `{hasPropertyRelation.UniqueNodeId()}` has its {nameof(IRelation.Object)} of type `{hasPropertyRelation.Object.GetType().FullName}` instead of {nameof(PropertyInfo)}.");
                return;
            }

            // Check if the property type is a BHoM type.
            // If the property type is a BHoM type, we need to add a "IsA" relation to a node that represents its property type.
            // We do this by:
            // - creating a node containing the name of the property ("PropertyNameNode"). This is linked to the parent type by a "HasProperty" relation.
            // - creating a node for the property type, if it doesn't exist already, or retrieve its ID if exists already ("PropertyTypeNode"). This is linked to the "PropertyNameNode" by a "IsA" relation.
            if (rangePropertyInfo.PropertyType.IsBHoMType() && propertyArray != null && propertyAttributeArray != null)
            {
                // Add the PropertyNameNode. This node will contain the name of the property.
                string propertyNameNodeId = rangePropertyInfo.UniqueNodeId();
                if (!addedWebVOWLNodeIds.Contains(propertyNameNodeId))
                {
                    classArray.AddToIdTypeArray(propertyNameNodeId, "owl:Class");
                    classAttributeArray.AddToAttributeArray(propertyNameNodeId, rangePropertyInfo.OntologyURI(new TBoxSettings(), settings), rangePropertyInfo.DescriptiveName(), false, null, rangePropertyInfo.DescriptionInAttribute());
                    addedWebVOWLNodeIds.Add(propertyNameNodeId);
                }

                // Add the "HasProperty" relation between the parent type and the PropertyNameNode.
                string classHasPropertyNameRelationId = domainType.UniqueNodeId() + "-HasProperty-" + propertyNameNodeId;
                if (!addedWebVOWLNodeIds.Contains(classHasPropertyNameRelationId))
                {
                    propertyArray.AddToIdTypeArray(classHasPropertyNameRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(classHasPropertyNameRelationId, hasPropertyRelation.GetType().OntologyUri(new TBoxSettings(), settings), hasPropertyRelation.DescriptiveName(), false, new List<string>() { "object" }, domain: new List<string>() { domainType.UniqueNodeId() }, range: new List<string>() { propertyNameNodeId });
                    addedWebVOWLNodeIds.Add(classHasPropertyNameRelationId);
                }

                // Now deal with the property Type.

                // See if we have yet to add a Node for the property type.
                string propertyTypeNodeId = AddWebOwlClassNodes(rangePropertyInfo.PropertyType, classArray, classAttributeArray, addedWebVOWLNodeIds, settings, internalNamespaces);

                // Add the "IsA" relation between the PropertyNameNode and the PropertyTypeNode.
                string propertyNameIsATypeRelationId = propertyNameNodeId + "-IsA-" + propertyTypeNodeId;
                if (!addedWebVOWLNodeIds.Contains(propertyNameIsATypeRelationId))
                {
                    propertyArray.AddToIdTypeArray(propertyNameIsATypeRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(propertyNameIsATypeRelationId, typeof(IsA).OntologyUri(new TBoxSettings(), settings), typeof(IsA).Name, false, new List<string>() { "object" }, domain: new List<string>() { propertyNameNodeId }, range: new List<string>() { propertyTypeNodeId });
                    addedWebVOWLNodeIds.Add(propertyNameIsATypeRelationId);
                }

                // Add other relations for this PropertyTypeNode
                if (recursionLevel > 0)
                {
                    var relations = rangePropertyInfo.PropertyType.RelationsFromType();
                    foreach (IRelation relation in relations)
                        AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedWebVOWLNodeIds, settings: settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
                }

                return;
            }

            // If the property type is a IEnumerable with a BHoM type in it, we need to add a "IsAListOf" relation to a node that represents its property type. 
            List<Type> genericBHoMArgs = rangePropertyInfo.PropertyType.GetGenericArguments().Where(t => t.IsBHoMType()).ToList();
            if (typeof(IEnumerable).IsAssignableFrom(rangePropertyInfo.PropertyType) && genericBHoMArgs.Count == 1)
            {
                // Add the PropertyNameNode. This node will contain the name of the property.
                string propertyNameNodeId = rangePropertyInfo.DeclaringType.FullName + "." + rangePropertyInfo.Name;
                if (!addedWebVOWLNodeIds.Contains(propertyNameNodeId))
                {
                    classArray.AddToIdTypeArray(propertyNameNodeId, "owl:Class");
                    classAttributeArray.AddToAttributeArray(propertyNameNodeId, rangePropertyInfo.OntologyURI(new TBoxSettings(), settings), rangePropertyInfo.DescriptiveName());
                    addedWebVOWLNodeIds.Add(propertyNameNodeId);
                }

                // Add the "HasProperty" relation between the parent type and the PropertyNameNode.
                string classHasPropertyNameRelationId = domainType.UniqueNodeId() + "-HasProperty-" + propertyNameNodeId;
                if (!addedWebVOWLNodeIds.Contains(classHasPropertyNameRelationId))
                {
                    propertyArray.AddToIdTypeArray(classHasPropertyNameRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(classHasPropertyNameRelationId, hasPropertyRelation.GetType().OntologyUri(new TBoxSettings(), settings), hasPropertyRelation.GetType().Name, false, new List<string>() { "object" }, domain: new List<string>() { domainType.UniqueNodeId() }, range: new List<string>() { propertyNameNodeId });
                    addedWebVOWLNodeIds.Add(classHasPropertyNameRelationId);
                }

                // Now deal with the property Type. In this case, the PropertyTypeNode will contain the generic argument type.
                Type ienumerableType = genericBHoMArgs.First();

                // See if we have yet to add a Node for the property type.
                string propertyTypeNodeId = AddWebOwlClassNodes(ienumerableType, classArray, classAttributeArray, addedWebVOWLNodeIds, settings, internalNamespaces);

                // Add the IsAListOf relation.
                string propertyNameIsATypeRelationId = propertyNameNodeId + "-IsAListOf-" + propertyTypeNodeId;
                if (!addedWebVOWLNodeIds.Contains(propertyNameIsATypeRelationId))
                {
                    propertyArray.AddToIdTypeArray(propertyNameIsATypeRelationId, "owl:ObjectProperty");
                    propertyAttributeArray.AddToAttributeArray(propertyNameIsATypeRelationId, typeof(IsAListOf).OntologyUri(new TBoxSettings(), settings), typeof(IsAListOf).Name, false, new List<string>() { "object" }, domain: new List<string>() { propertyNameNodeId }, range: new List<string>() { propertyTypeNodeId });
                    addedWebVOWLNodeIds.Add(propertyNameIsATypeRelationId);
                }

                // Add other relations for this PropertyTypeNode
                if (recursionLevel > 0)
                {
                    var relations = ienumerableType.RelationsFromType();
                    foreach (IRelation relation in relations)
                        AddWebOwlRelationNodes(relation as dynamic, classArray, classAttributeArray, addedWebVOWLNodeIds, settings, propertyArray: propertyArray, propertyAttributeArray: propertyAttributeArray, internalNamespaces: internalNamespaces, exceptions: exceptions, recursionLevel: recursionLevel - 1);
                }
                return;
            }

            // For all other cases - add DataType

            // Add the class node for the Range of the HasProperty relation. 
            string rangeClassId = rangePropertyInfo.UniqueNodeId();
            if (!addedWebVOWLNodeIds.Contains(rangeClassId))
            {
                classArray.AddToIdTypeArray(rangeClassId, "owl:Class"); // Can be changed to `owl:Class` to allow URI link
                classAttributeArray.AddToAttributeArray(rangeClassId, rangePropertyInfo.OntologyURI(new TBoxSettings(), settings), rangePropertyInfo.DescriptiveName(), false, new List<string>() { "datatype" });
                addedWebVOWLNodeIds.Add(rangeClassId);
            }

            // Add the relation connection.
            string propertyTypeRelationId = hasPropertyRelation.UniqueNodeId();
            if (!addedWebVOWLNodeIds.Contains(propertyTypeRelationId))
            {
                propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:DatatypeProperty");
                propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, hasPropertyRelation.GetType().OntologyUri(new TBoxSettings(), settings), hasPropertyRelation.GetType().Name, false, new List<string>() { "datatype" }, domain: new List<string>() { domainType.UniqueNodeId() }, range: new List<string>() { rangeClassId });
                addedWebVOWLNodeIds.Add(propertyTypeRelationId);
            }
        }

        /***************************************************/

        private static void AddWebOwlRelationNodes(RequiresProperty requiresPropertyRelation,
                                                    JArray classArray,
                                                    JArray classAttributeArray,
                                                    HashSet<string> addedWebVOWLNodeIds,
                                                    LocalRepositorySettings settings,
                                                    JArray propertyArray = null,
                                                    JArray propertyAttributeArray = null,
                                                    HashSet<string> internalNamespaces = null,
                                                    HashSet<string> exceptions = null,
                                                    int recursionLevel = 0)
        {
            string propertyTypeRelationId = requiresPropertyRelation.UniqueNodeId();
            if (addedWebVOWLNodeIds.Contains(propertyTypeRelationId))
                return;

            Type domainType = requiresPropertyRelation.Subject as Type;
            PropertyInfo rangePi = requiresPropertyRelation.Object as PropertyInfo;

            if (domainType == null || rangePi == null)
            {
                Log.RecordError($"Cannot add requiresPropertyRelation `{requiresPropertyRelation.UniqueNodeId()}`");
                return;
            }

            if (!domainType.IsBHoMType())
            {
                // Do not record this relationship.
                return;
            }

            string domainTypeNodeId = domainType.UniqueNodeId();
            string rangeTypeNodeId = rangePi.UniqueNodeId();

            // See if we have yet to add a Node for the Relation.Subject (domain) type.
            if (!addedWebVOWLNodeIds.Contains(domainTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(domainTypeNodeId, "owl:Class");

                classAttributeArray.AddToAttributeArray(domainTypeNodeId, rangePi.OntologyURI(new TBoxSettings(), settings), rangePi.DescriptiveName(true), false);

                addedWebVOWLNodeIds.Add(domainTypeNodeId);
            }

            // See if we have yet to add a Node for the Relation.Object (range) type.
            if (!addedWebVOWLNodeIds.Contains(rangeTypeNodeId))
            {
                // We need to add the type of this property as a node.
                classArray.AddToIdTypeArray(rangeTypeNodeId, "rdfs:Datatype"); // Can be changed to `owl:Class` to allow URI link

                classAttributeArray.AddToAttributeArray(rangeTypeNodeId, rangePi.OntologyURI(new TBoxSettings(), settings), rangePi.DescriptiveName(true), false, new List<string> { "datatype" });

                addedWebVOWLNodeIds.Add(rangeTypeNodeId);
            }

            // Add the "RequiresProperty" relation to link this property to the corresponding type node.
            propertyArray.AddToIdTypeArray(propertyTypeRelationId, "owl:DatatypeProperty");
            propertyAttributeArray.AddToAttributeArray(propertyTypeRelationId, typeof(RequiresProperty).OntologyUri(new TBoxSettings(), settings), typeof(RequiresProperty).Name, false, new List<string>() { "datatype" }, domain: new List<string>() { domainType.UniqueNodeId() }, range: new List<string>() { rangePi.UniqueNodeId() });
            addedWebVOWLNodeIds.Add(propertyTypeRelationId);
        }
    }
}

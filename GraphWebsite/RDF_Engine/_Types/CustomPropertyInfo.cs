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

using BH.Engine.Adapters.RDF;
using BH.oM.Base;
using BH.oM.Adapters.RDF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace GraphWebsite
{
    /// <summary>
    /// Type used to represent the properties of a CustomObjectType type. See the description of CustomObjectType for more info.
    /// These PropertyInfos correspond to keys in the CustomData dictionary of a CustomObject.
    /// </summary>
    public class CustomPropertyInfo : PropertyInfo
    {
        // ************************************ //
        // Public properties                    //
        // ************************************ //

        public override Type PropertyType { get; }

        public override PropertyAttributes Attributes { get; }

        public override bool CanRead { get; } = true;

        public override bool CanWrite { get; } = true;

        public override string Name { get; }

        public override Type DeclaringType { get; }

        public override Type ReflectedType => throw new NotImplementedException();

        // ************************************ //
        // Ctor                                 //
        // ************************************ //

        public CustomPropertyInfo(Type declaringType, string propertyName, Type propertyType)
        {
            if (typeof(CustomObjectType).IsAssignableFrom(declaringType))
                throw new ArgumentException("Cannot create custom property info for a non Custom type via this ctor.");

            Name = propertyName;
            PropertyType = propertyType;
            DeclaringType = declaringType;
        }
        
        public CustomPropertyInfo(CustomObjectType declaringCustomObjectType, string propertyName, Type propertyType)
        {
            Name = propertyName;
            PropertyType = propertyType;
            DeclaringType = declaringCustomObjectType;

            // The ontological uri of this property is the Ontological Uri of the parent type
            // followed by an hashtag and the name of this property.
            //OntologicalUri = Query.CombineUris(declaringCustomObjectType.OntologicalUri + $"#{Name}");
        }

        public CustomPropertyInfo(CustomObjectType declaringCustomObjectType, KeyValuePair<string, Type> propertyTypes) : this(declaringCustomObjectType, propertyTypes.Key, propertyTypes.Value)
        {

        }

        // ************************************ //
        // Public methods                       //
        // ************************************ //

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            BHoMObject co = obj as BHoMObject;
            if (co == null)
                throw new ArgumentException($"The input object must be a {nameof(BHoMObject)}.");

            object value = null;
            if (!co.CustomData.TryGetValue(this.Name, out value))
                throw new ArgumentException($"The input object does not have a {this.Name} key.");

            return value;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            BHoMObject co = obj as BHoMObject;
            if (co == null)
                throw new ArgumentException($"The input object must be a {nameof(BHoMObject)}.");

            co.CustomData[this.Name] = value;
        }

        // ------------------------------------------------------------------------ //
        // Not implemented stuff, required for inheritance but otherwise not useful //
        // ------------------------------------------------------------------------ //

        #region NotImplementedStuff

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}


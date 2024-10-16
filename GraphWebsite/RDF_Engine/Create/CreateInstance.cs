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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphWebsite
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an instance of the given type, by using a specified contructor chosen via its index. 0 is the first ctor, 1 the second, and so on. Ctor input args can be specified.")]
        public static object CreateInstance(Type type, int ctorIndex = 0, params object[] args)
        {
            ConstructorInfo ctor = type.GetConstructors()[ctorIndex];

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            NewExpression newExp = GetNewExpression(ctor, param);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);

            //compile it
            ObjectActivator compiledActivator = (ObjectActivator)lambda.Compile();

            // Create the instance
            object instance = compiledActivator(args);

            return instance;
        }

        /***************************************************/

        [Description("Creates an instance of the given type, by using a specified contructor chosen via its index. 0 is the first ctor, 1 the second, and so on. Ctor input args can be specified.")]
        public static T CreateInstance<T>(int ctorIndex = 0, params object[] args)
        {
            ConstructorInfo ctor = typeof(T).GetConstructors()[ctorIndex];
            ObjectActivator<T> createdActivator = GetActivator<T>(ctor);
            T instance = createdActivator(args);

            return instance;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static NewExpression GetNewExpression(ConstructorInfo ctor, ParameterExpression param)
        {
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            return newExp;
        }

        /***************************************************/

        private static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            NewExpression newExp = GetNewExpression(ctor, param);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        delegate T ObjectActivator<T>(params object[] args);
        delegate object ObjectActivator(params object[] args);
    }
}

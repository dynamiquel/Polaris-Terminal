//  This file is part of Polaris-Terminal - A developer console for Unity.
//  https://github.com/dynamiquel/Polaris-Options
//  Copyright (c) 2020 dynamiquel

//  Parts of this file is part of Reactor-Developer-Console v1.2.
//  https://github.com/mustafayaya/Reactor-Developer-Console
//  Copyright (c) 2019 Mustafa Yaya

//  MIT License
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Polaris.Terminal
{
    public static class Utility
    {
        public static IEnumerable<Command> GetTypesWithCommandAttribute(System.Reflection.Assembly[] assemblies)
        {
            return from assembly in assemblies from type in assembly.GetTypes() 
                where type.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0 && 
                      type.BaseType == typeof(Command) select (Command)Activator.CreateInstance(type);
        }
        
        public static bool TryConvert<T>(object parameter, out T convertedParameter)
        {
            try
            {
                convertedParameter = (T)Convert.ChangeType(parameter, typeof(T));
                return true;
            }
            catch (Exception e)
            {
                convertedParameter = default;
                return false;
            }
        }
        
        // Forked from Reactor-Developer-Console.
        // Check each parameter in constructor and try to fill the parameters
        public static List<object> GetConstructorParametersList(ConstructorInfo constructorInfo, string[] parameters)
        {
            var argsList = new List<object>();
            var i = 0;

            foreach (ParameterInfo parameterInfo in constructorInfo.GetParameters())
            {
                if (parameterInfo.ParameterType == typeof(string))
                    argsList.Add(parameters[i]);
                else if (parameterInfo.ParameterType == typeof(int))
                {
                    if (int.TryParse(parameters[i], out var result))
                        argsList.Add(result);
                    else
                        return null;
                }
                else if (parameterInfo.ParameterType == typeof(float))
                {
                    if (float.TryParse(parameters[i], out var result))
                        argsList.Add(result);
                    else
                        return null;
                }

                i++;
            }
            
            return argsList;
        }
    }
}
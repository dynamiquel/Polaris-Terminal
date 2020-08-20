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
        
        // Forked from Reactor-Developer-Console v1.2 (https://github.com/mustafayaya/Reactor-Developer-Console)
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
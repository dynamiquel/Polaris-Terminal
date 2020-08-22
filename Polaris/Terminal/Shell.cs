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
using UnityEngine;

namespace Polaris.Terminal
{
    /// <summary>
    /// Responsible for storing and executing commands.
    /// </summary>
    public static class Shell
    {
        public static Dictionary<string, Command> Commands {get; private set;} = new Dictionary<string, Command>();

        [RuntimeInitializeOnLoadMethod]
        private static void RegisterCommands()
        {
            var commands = Utility.GetTypesWithCommandAttribute(AppDomain.CurrentDomain.GetAssemblies());

            foreach (var command in commands)
            {
                if (Commands.ContainsValue(command))
                    continue;

                var fields = command.GetType().GetFields();

                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<CommandParameterAttribute>() == null) 
                        continue;

                    // Forked from Reactor-Developer-Console.
                    var commandParameterType = typeof(CommandParameter<>);
                    var commandParameterTypeGeneric = commandParameterType.MakeGenericType(field.FieldType);
                    var commandParameter = (CommandParameter)Activator.CreateInstance(commandParameterTypeGeneric, command, field);
                    var commandParameterAttribute = field.GetCustomAttribute<CommandParameterAttribute>();
                    
                    // Gets the value of the current field and sets it as the default value.
                    commandParameter.DefaultValue = field.GetValue(command);
                    // Adds a reference to the field to the Command.
                    command.Parameters.Add(commandParameterAttribute.description, commandParameter);
                    // Uhh. Not me.
                    commandParameterAttribute.commandParameter = commandParameter;
                }

                // Cache the commandId. Not sure if it's an expensive process but it seems like it.
                var commandId = command.Id;
                if (string.IsNullOrWhiteSpace(commandId))
                {
                    Terminal.Log(new LogMessage
                    {
                        Content = $"The command '{command.GetHashCode()}' does not have an applicable ID. Command will be ignored.",
                        LogSource = LogSource.Commands,
                        LogType = LogType.System
                    });
                    
                    continue;
                }

                if (Commands.ContainsKey(commandId))
                {
                    Terminal.Log(new LogMessage
                    {
                        Content = $"Multiple commands found with the same unique ID '{commandId}'. The command '{command.GetHashCode()}' will be ignored.",
                        LogSource = LogSource.Commands,
                        LogType = LogType.System
                    });
                    
                    continue;
                }
                
                Commands.Add(commandId, command);
                Terminal.Log(new LogMessage
                {
                    Content = $"Added the '{command.Id}' command to the shell.",
                    LogSource = LogSource.Commands,
                    LogType = LogType.System
                });
            }
        }

        public static IEnumerable<Command> GetCommands(string query)
        {
            var potentialCommands = new List<Command>();

            // Check every command and compare them to input
            foreach (var command in Commands)
            {
                if (query.Length > 0 && query.Length <= command.Key.Length)
                    if (string.Equals(command.Key.Substring(0, query.Length), query, StringComparison.CurrentCultureIgnoreCase))
                        potentialCommands.Add(command.Value);
            }

            return potentialCommands.OrderBy(x => x.Id);
        }

        public static LogMessage Execute(QueryInfo queryInfo)
        {
            if (!Commands.ContainsKey(queryInfo.Command))
                return new LogMessage($"The command '{queryInfo.Command}' does not exist.");

            var command = Commands[queryInfo.Command];

            if (command.PermissionLevel > Terminal.Settings.PermissionLevel)
                return new LogMessage($"You do not have the permission to use the '{queryInfo.Command}' command.");

            // If the command has parameters.
            if (command.Parameters != null)
            {
                // TODO: Changes need to be made to allow default values to be automatically used.
                if (queryInfo.Parameters.Count < command.Parameters?.Count)
                    return new LogMessage(
                        $"Not enough parameters sent for the '{command.Id}' command. Expected {command.Parameters.Count}; received {queryInfo.Parameters.Count}.");

                int i = 0;
                foreach (var parameter in command.Parameters)
                {
                    object query;
                    
                    // If query is a '&', use the default value for that parameter.
                    if (queryInfo.Parameters[i] == "&")
                    {
                        query = parameter.Value.DefaultValue;
                    }
                    else
                    {
                        Type parameterType = parameter.Value.GetType().GenericTypeArguments[0];
                        MethodInfo method = typeof(Shell).GetMethod("ParamQuery");
                        MethodInfo genericMethod = method.MakeGenericMethod(parameterType);

                        query = genericMethod.Invoke(typeof(Shell), new object[] {queryInfo.Parameters[i]});
                    }
                    
                    if (query != null)
                        command.Parameters[parameter.Key].Value = query;

                    i++;
                }
            }

            LogMessage result;

            try
            {
                result = command.Execute();
            }
            catch (Exception e)
            {
                var errorMessage = Terminal.Settings.ShowStackTraceOnError ? e.ToString() : e.Message;
                result = new LogMessage
                {
                    Content = errorMessage,
                    LogType = LogType.Error,
                    LogSource = LogSource.Commands
                };
            }
            
            return result;
        }
        
        // Forked from Reactor-Developer-Console.
        // Make query with given parameter and type.
        public static object ParamQuery<T>(string parameter)
        {
            // UNITY-SPECIFIC
            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                Component query = null;

                var go = GameObject.Find(parameter);
                if (go != null && go.TryGetComponent(typeof(T), out query))
                {
                    return query;
                } 
                if (go != null && !go.TryGetComponent(typeof(T), out query))
                {
                    Terminal.LogError(parameter + " doesn't have a " + typeof(T));
                    return query;
                }
                return query;
            }

            // If parameter string is convertible directly to T return converted.
            if (Utility.TryConvert<T>(parameter, out var convertedParameter))
                return convertedParameter;

            var parameters = parameter.Split(',');
            // Get constructors of T.
            var constructors = typeof(T).GetConstructors();
            ConstructorInfo constructor = null;

            // Get the possible decelerations from constructors.
            foreach (ConstructorInfo constructorInfo in constructors)
            {
                if (constructorInfo.GetParameters().Length == parameters.Length)
                    // Move with this deceleration.
                    constructor = constructorInfo; 
            }
            
            if (constructor != null)
            {
                var constructionsParametersList = Utility.GetConstructorParametersList(constructor, parameters);
                if (constructionsParametersList != null)
                    return (T)Activator.CreateInstance(typeof(T), constructionsParametersList.ToArray());
            }
            
            return null;
        }
        
        [UnityEngine.Scripting.Preserve]
        private static void UsedOnlyForAOTCodeGeneration()
        {
            try
            {
                new CommandParameter<bool>(null, null);
                ParamQuery<bool>(null);
                
                new CommandParameter<int>(null, null);
                ParamQuery<int>(null);
                
                new CommandParameter<float>(null, null);
                ParamQuery<float>(null);
                
                new CommandParameter<double>(null, null);
                ParamQuery<double>(null);
                
                new CommandParameter<Vector3>(null, null);
                ParamQuery<Vector3>(null);

                new CommandParameter<Quaternion>(null, null);
                ParamQuery<Quaternion>(null);
                
                new CommandParameter<Transform>(null, null);
                ParamQuery<Transform>(null);
                
                new CommandParameter<Rigidbody>(null, null);
                ParamQuery<Rigidbody>(null);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    "This method is used for AOT code generation only. Do not call it at runtime.");
            }
        }
    }
}
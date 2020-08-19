using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Polaris.Terminal
{
    /// <summary>
    /// Responsible for storing and executing commands.
    /// </summary>
    public static class Shell
    {
        public static bool Started { get; }
        public static Dictionary<string, Command> Commands {get; private set;} = new Dictionary<string, Command>();

        static Shell()
        {
            Started = true;
            RegisterCommands();
        }
        
        private static void RegisterCommands()
        {
            var commands = Utility.GetTypesWithCommandAttribute(System.AppDomain.CurrentDomain.GetAssemblies());

            foreach (var command in commands)
            {
                if (Commands.ContainsValue(command))
                    continue;

                var fields = command.GetType().GetFields();

                Terminal.Log(fields.Length);
                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<CommandParameterAttribute>() == null) 
                        continue;
                    
                    // Forked from Reactor-Developer-Console-1.2
                    var commandParameterType = typeof(CommandParameter<>);
                    var commandParameterTypeGeneric = commandParameterType.MakeGenericType(field.FieldType);
                    var commandParameter = Activator.CreateInstance(commandParameterTypeGeneric, command, field);
                    var commandParameterAttribute = field.GetCustomAttribute<CommandParameterAttribute>();
                    command.Parameters.Add(commandParameterAttribute.description, (CommandParameter)commandParameter);
                    commandParameterAttribute.commandParameter = (CommandParameter)commandParameter;
                    
                    Terminal.Log(new LogMessage
                    {
                        Content = $"The command '{command.Name}' has {command.Parameters.Count} parameters.",
                        LogSource = LogSource.Commands,
                        LogType = LogType.System
                    });
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

        private static void GetCommands()
        {
            
        }

        public static LogMessage Execute(QueryInfo queryInfo)
        {
            if (!Commands.ContainsKey(queryInfo.Command))
                return new LogMessage($"The command '{queryInfo.Command}' does not exist.");

            var command = Commands[queryInfo.Command];

            if (command.PermissionLevel > Terminal.Settings.PermissionLevel)
                return new LogMessage($"You do not have the permission to use the '{queryInfo.Command}' command.");

            if (queryInfo.Parameters.Count < command.Parameters?.Count)
                return new LogMessage($"Not enough parameters sent for the '{command.Id}' command. Expected {command.Parameters.Count}; received {queryInfo.Parameters.Count}.");

            int i = 0;
            foreach (var parameter in command.Parameters)
            {
                Type parameterType = parameter.Value.genericType;
                MethodInfo method = typeof(Shell).GetMethod("ParamQuery");
                MethodInfo genericMethod = method.MakeGenericMethod(parameterType);

                var query = genericMethod.Invoke(typeof(Shell), new object[] {queryInfo.Parameters[i]});
                Terminal.Log(query.ToString());

                i++;
            }

            LogMessage result;

            try
            {
                result = command.Execute();
            }
            catch (Exception e)
            {
                string errorMessage = Terminal.Settings.ShowStackTrace ? e.ToString() : e.Message;
                result = new LogMessage
                {
                    Content = errorMessage,
                    LogType = LogType.Error,
                    LogSource = LogSource.Commands
                };
            }
            
            return result;
        }
        
        // Forked from Reactor-Developer-Console-1.2
        public static object ParamQuery<T>(string parameter)//Make query with given parameter and type
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

            else
            {
                if (Utility.TryConvert<T>(parameter, out var convertedParameter))//If parameter string is convertable directly to T return converted 
                {
                    return convertedParameter;
                }

                object query = null;
                var parameters = parameter.Split(',');
                var constructors = (typeof(T)).GetConstructors();//Get constructors of T
                ConstructorInfo _constructor = null;

                foreach (ConstructorInfo constructorInfo in constructors)//Get the possible declerations from constructors
                {
                    if (constructorInfo.GetParameters().Length == parameters.Length)
                    {
                        _constructor = constructorInfo;//Move with this decleration
                    }
                }
                if (_constructor != null)
                {
                    var constructionsParametersList = Utility.GetConstructorParametersList(_constructor, parameters);
                    if (constructionsParametersList != null)
                    {
                        query = (T)Activator.CreateInstance(typeof(T), constructionsParametersList.ToArray());
                        return query;

                    }
                    return null;
                }
                return null;
            }
        }
    }
}
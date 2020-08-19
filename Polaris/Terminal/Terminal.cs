using System;
using System.Collections.Generic;
using System.IO;
using Polaris.Core;
using UnityEngine;

namespace Polaris.Terminal
{
    /// <summary>
    /// Responsible for processing logs and communicating to the shell.
    /// </summary>
    public static class Terminal
    {
        public static event Action<LogMessage> OnLog;
        public static TStack<string> InputHistory { get; private set; }
        public static Settings Settings { get; } = new Settings();
        
        private static FileLogger fileLogger = new FileLogger();

        static Terminal()
        {
            OnStart();
        }

        private static void OnStart()
        {
            // UNITY-SPECIFIC
            if (Settings.OutputUnityLogs)
                Application.logMessageReceived += HandleUnityLog;
            else
                Application.logMessageReceived -= HandleUnityLog;

            if (Settings.EnableHistory)
                InitialiseHistory();
        }

        // Processes the input and gives the information to the shell so a command can be executed.
        public static void Execute(string input, bool logInput = false)
        {
            if (logInput)
            {
                Log(new LogMessage(input, LogType.User));
                
                if (Settings.EnableHistory)
                    WriteHistory(input);
            }
            
            // NEW SHELL
            var query = ProcessCommandQuery(input);
            Log(new LogMessage
            {
                Content = query.ToString(),
                LogSource = LogSource.Commands,
                LogType = LogType.System
            });
            var result = Shell.Execute(query);
            
            
            /* OLD SHELL
            var queryInfo = ProcessCommandQuery(input);
            var result = Polaris.Debug.Terminal.Shell.Execute(queryInfo);
            var newResult = new LogMessage(result.Context);
            */

            if (!string.IsNullOrEmpty(result.Content))
                Log(result);
        }

        #region Logging
        
        // More convenient way to a log a normal message.
        /// <summary>
        /// Logs a message to the Terminal.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
            if (message == null)
                Log(new LogMessage("null"));
            else
                Log(new LogMessage(message.ToString()));
        }

        // More convenient way to log a normal message with a log source.
        /// <summary>
        /// Logs a message with a specified Log Source to the Terminal.
        /// </summary>
        /// <param name="logSource"></param>
        /// <param name="message"></param>
        public static void Log(LogSource logSource, object message)
        {
            Log(new LogMessage
            {
                Content = message.ToString(),
                LogSource = logSource
            });
        }

        /// <summary>
        /// Logs a message with a custom colour to the Terminal.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="colour"></param>
        public static void Log(object message, Colour colour)
        {
            Log(new LogMessage
            {
                Content = message.ToString(),
                Colour = colour
            });
        }
        
        // Sends the log message to all terminals and the log file (if enabled).
        /// <summary>
        /// Logs a fully customisable LogMessage to the Terminal.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(LogMessage message)
        {
            // Sends the log message to all subscribers.
            OnLog?.Invoke(message);
        }

        /// <summary>
        /// Logs a warning to the Terminal.
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(object message)
        {
            Log(new LogMessage(message.ToString(), LogType.Warning));
        }

        /// <summary>
        /// Logs a warning with a specified Log Source to the Terminal.
        /// </summary>
        /// <param name="logSource"></param>
        /// <param name="message"></param>
        public static void LogWarning(LogSource logSource, object message)
        {
            Log(new LogMessage
            {
                Content = message.ToString(),
                LogSource = logSource,
                LogType = LogType.Warning
            });
        }

        /// <summary>
        /// Logs an assertion to the Terminal.
        /// </summary>
        /// <param name="message"></param>
        public static void LogAssertion(object message)
        {
            Log(new LogMessage(message.ToString(), LogType.Assertion));
        }
        
        /// <summary>
        /// Logs an assertion with a specified Log Source to the Terminal.
        /// </summary>
        /// <param name="logSource"></param>
        /// <param name="message"></param>
        public static void LogAssertion(LogSource logSource, object message)
        {
            Log(new LogMessage
            {
                Content = message.ToString(),
                LogSource = logSource,
                LogType = LogType.Assertion
            });
        }

        /// <summary>
        /// Logs an error to the Terminal.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
            Log(new LogMessage(message.ToString(), LogType.Error));
        }
        
        /// <summary>
        /// Logs an error with a specified Log Source to the Terminal.
        /// </summary>
        /// <param name="logSource"></param>
        /// <param name="message"></param>
        public static void LogError(LogSource logSource, object message)
        {
            Log(new LogMessage
            {
                Content = message.ToString(),
                LogSource = logSource,
                LogType = LogType.Error
            });
        }
        
        #endregion

        // UNITY-SPECIFIC
        // Converts Unity's built in log messages to Terminal log messages.
        private static void HandleUnityLog(string logMessage, string stackTrace, UnityEngine.LogType type)
        {
            switch (type)
            {
                case UnityEngine.LogType.Error:
                    Log(new LogMessage(logMessage, LogType.Error, default, LogSource.Unity));
                    break;
                case UnityEngine.LogType.Assert:
                    Log(new LogMessage(logMessage, LogType.Assertion, default, LogSource.Unity));
                    break;
                case UnityEngine.LogType.Warning:
                    Log(new LogMessage(logMessage, LogType.Warning, default, LogSource.Unity));
                    break;
                case UnityEngine.LogType.Log:
                    Log(new LogMessage(logMessage, LogType.Log, default, LogSource.Unity));
                    break;
                case UnityEngine.LogType.Exception:
                    Log(new LogMessage(logMessage, LogType.Error, default, LogSource.Unity));
                    break;
                default:
                    Log(new LogMessage(logMessage, LogType.Log, default, LogSource.Unity));
                    break;
            }
        }

        private static QueryInfo ProcessCommandQuery(string query)
        {
            var queryInfo = new QueryInfo(string.Empty);
            
            // Removes the excess spacing.
            query = query.Trim();

            var inputParams = new List<string>();
            var charList = query.ToCharArray();
            var readingParam = false;
            var param = string.Empty;

            for (var i = 0; i < query.Length; i++)
            {
                // Start the grouping
                if (charList[i] == '(' && !readingParam)
                {
                    readingParam = true;
                }
                else if (charList[i] != ')' && readingParam && charList[i] != '(')
                {
                    param += charList[i].ToString();
                }
                else if (charList[i] != ' ' && !readingParam)
                {
                    param += charList[i].ToString();
                }
                else if (charList[i] == ' ' && !readingParam)
                {
                    inputParams.Add(param);
                    param = "";
                }
                // End the grouping
                else if (charList[i] == ')' && readingParam)
                {
                    readingParam = false;
                }
                
                if (i == query.Length - 1 && !string.IsNullOrEmpty(param))
                {
                    inputParams.Add(param);
                    param = "";
                }
            }
            
            for (var i = inputParams.Count - 1; i >= 0; i--)
            {
                // Removes the excess spacing.
                inputParams[i] = inputParams[i].Trim();
            }

            if (inputParams.Count > 0)
            {
                queryInfo.Command = inputParams[0];
                inputParams.RemoveAt(0);

                if (inputParams.Count > 0)
                    queryInfo.AddParameters(inputParams);
            }

            return queryInfo;
        }

        

        // Appends the input to the end of the history stack and file so it can be retrieved later.
        private static void WriteHistory(string input)
        {
            if (InputHistory == null)
                return;
            
            if (string.IsNullOrWhiteSpace(input))
                return;
            
            InputHistory.Push(input);
            
            IO.AppendToHistory(input);
        }

        // Initialises the history stack and if a history file exists, pushes its data to it.
        private static void InitialiseHistory()
        {
            var savedHistory = IO.GetHistory();
            InputHistory = savedHistory ?? new TStack<string>();
        }
        
        // EXTRACTED FROM OLD TERMINAL
        // Splits the string into an array of arguments that can
        // be individually accessed. [0] is command.
        // Brackets can be used to group arguments that contain
        // spaces.
        // Currently doesn't support groups in groups, e.g. (())
        static string[] ProcessParameters(string input)
        {
            // Removes the excess spacing.
            input = input.Trim();

            List<string> inputParams = new List<string>();
            var charList = input.ToCharArray();
            bool readingParam = false;
            string param = "";

            for (int i = 0; i < input.Length; i++)
            {
                // Start the grouping
                if (charList[i] == '(' && !readingParam)
                {
                    readingParam = true;
                }
                else if (charList[i] != ')' && readingParam && charList[i] != '(')
                {
                    param += charList[i].ToString();
                }
                else if (charList[i] != ' ' && !readingParam)
                {
                    param += charList[i].ToString();
                }
                else if (charList[i] == ' ' && !readingParam)
                {
                    inputParams.Add(param);
                    param = "";
                }
                // End the grouping
                else if (charList[i] == ')' && readingParam)
                {
                    readingParam = false;
                }
                if (i == input.Length - 1 && !string.IsNullOrEmpty(param))
                {
                    inputParams.Add(param);
                    param = "";
                }
            }

            // Removes the excess spacing.
            for (var i = 0; i < inputParams.Count; i++)
                inputParams[i] = inputParams[i].Trim();

            return inputParams.ToArray();
        }      
    }
}
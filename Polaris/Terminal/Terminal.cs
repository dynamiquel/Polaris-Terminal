//  This file is part of Polaris-Terminal - A developer console for Unity.
//  https://github.com/dynamiquel/Polaris-Options
//  Copyright (c) 2020 dynamiquel

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
using System.Diagnostics;
using Polaris.Core;
using UnityEngine;

namespace Polaris.Terminal
{
    /// <summary>
    /// Responsible for processing logs and communicating to the shell.
    /// </summary>
    public static class Terminal
    {
        /// <summary>
        /// Subscribe to receive log messages from the Terminal.
        /// </summary>
        public static event Action<LogMessage> OnLog;
        /// <summary>
        /// The queries the user has submitted to Terminal.Execute().
        /// </summary>
        public static TStack<string> InputHistory { get; private set; }
        /// <summary>
        /// The Terminal's settings.
        /// </summary>
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
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="query">The command ID and parameter info (separated by spaces).</param>
        /// <param name="logInput">Enabled: Query is added to Terminal.InputHistory
        /// (enable if this is being executed from a GUI).</param>
        public static void Execute(string query, bool logInput = false)
        {
            if (logInput)
            {
                Log(new LogMessage(query, LogType.User));
                
                if (Settings.EnableHistory)
                    WriteHistory(query);
            }
            
            // NEW SHELL
            // Splits the input query from a single string to a command ID and parameters.
            var processedQuery = QueryInfo.FromString(query);
            var result = Shell.Execute(processedQuery);

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
            // Appends the current stack trace to the message (if enabled).
            if ((Settings.ShowStackTrace & message.LogType) == message.LogType)
                message.Content += $"\n\t{StackTraceUtility.ExtractStackTrace().Split('\n')[2]}\n";

            // Sends the log message to all subscribers.
            OnLog?.Invoke(message);
        }
        
        // Sends the log message to all terminals and the log file (if enabled).
        /// <summary>
        /// Logs a fully customisable LogMessage to the Terminal.
        /// Ignores Terminal.Settings.ShowStackTrace.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="forceStackTrace">Should the Stack Trace be included?</param>
        public static void Log(LogMessage message, bool forceStackTrace)
        {
            // Appends the current stack trace to the message (if enabled).
            if (forceStackTrace)
                message.Content += $"\n\t{StackTraceUtility.ExtractStackTrace().Split('\n')[2]}\n";

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
        public static void LogAssert(object message)
        {
            Log(new LogMessage(message.ToString(), LogType.Assertion));
        }
        
        /// <summary>
        /// Logs an assertion with a specified Log Source to the Terminal.
        /// </summary>
        /// <param name="logSource"></param>
        /// <param name="message"></param>
        public static void LogAssert(LogSource logSource, object message)
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

        /// <summary>
        /// Checks for a condition; if the condition is false, outputs the stack trace.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            if (condition) 
                return;
            
            Log(new LogMessage
            {
                Content = $"{StackTraceUtility.ExtractStackTrace().Split('\n')[1]}",
                LogType = LogType.Assertion
            }, false);
        }
        
        /// <summary>
        /// Checks for a condition; if the condition is false, outputs the message and the stack trace.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            if (condition) 
                return;
            
            Log(new LogMessage
            {
                Content = $"{message}\n\t{StackTraceUtility.ExtractStackTrace().Split('\n')[1]}\n",
                LogType = LogType.Assertion
            }, false);
        }
        
        /// <summary>
        /// Checks for a condition; if the condition is false, outputs the message and the stack trace.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void Assert(bool condition, LogMessage message)
        {
            if (condition) 
                return;
            
            message.Content += $"\n\t{StackTraceUtility.ExtractStackTrace().Split('\n')[1]}\n";
            Log(message, false);
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
    }
}
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
using System.Globalization;
using System.Text;

namespace Polaris.Terminal
{
    /// <summary>
    /// Responsible for logging all the Terminal logs to a text file.
    /// </summary>
    public class FileLogger
    {
        private static string logFilePath;
        private static bool logFileCreated = false;
        
        public FileLogger()
        {
            if (!Terminal.Settings.OutputToFile) 
                return;
            
            CreateLogFile();
            Terminal.OnLog += WriteLogToFile;

            CompressOldLogFiles();
        }

        /// <summary>
        /// Tells Polaris.Terminal.IO to clear all the old log files.
        /// </summary>
        public static void ClearLogs()
        {
            IO.ClearLogs();
        }
        
        /// <summary>
        /// Tells Polaris.Terminal.IO to create a new log file.
        /// </summary>
        private static void CreateLogFile()
        {
            var time = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
            logFilePath = IO.CreateLog(time);
        }

        // Formats the given log and appends it to the current log file.
        private static void WriteLogToFile(LogMessage logMessage)
        {
            if (!Terminal.Settings.OutputToFile)
                return;
            
            var formattedLogMessage = new StringBuilder();

            formattedLogMessage.Append($"{logMessage.Timestamp.ToString(CultureInfo.CurrentCulture)} ");
            formattedLogMessage.Append($"[{logMessage.LogSource.ToString()}] ");
            formattedLogMessage.Append($"[{logMessage.LogType.ToString()}] ");
            formattedLogMessage.Append(logMessage.Content);
            
            // If the log file somehow wasn't created on start up, try to create it.
            if (string.IsNullOrEmpty(logFilePath))
                CreateLogFile();
            
            // Tells Polaris.Terminal.IO to append the current message to the end of the given text file.
            IO.AppendToLog(formattedLogMessage.ToString(), logFilePath);
        }

        /// <summary>
        /// Tells Polaris.Terminal.IO to compress the old log files.
        /// </summary>
        private static void CompressOldLogFiles()
        {
            IO.CompressOldLogs(logFilePath);
        }
    }
}
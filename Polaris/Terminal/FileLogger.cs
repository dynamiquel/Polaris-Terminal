using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

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
        
        // Tells Polaris.Terminal.IO to create a new log file.
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

        // Tells Polaris.Terminal.IO to compress the old log files.
        private static void CompressOldLogFiles()
        {
            IO.CompressOldLogs(logFilePath);
        }
    }
}
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
using System.IO;
using Polaris.IO.Compression;
using Polaris.Terminal.Unity;

namespace Polaris.Terminal
{
    /// <summary>
    /// An I/O wrapper/compatibilty layer for Polaris Terminal.
    /// Designed to make it easy to change how I/O is done in Polaris Terminal.
    /// </summary>
    public static class IO
    {
        // Doesn't update when Terminal.Settings.Directory changes. Changes will be applied after game restart.
        private static readonly string HistoryPath = Path.Combine(Terminal.Settings.Directory, "History");

        /// <summary>
        /// Returns the user's input history as a TStack.
        /// </summary>
        /// <returns></returns>
        public static TStack<string> GetHistory()
        {
            var path = HistoryPath;

            if (File.Exists(path))
            {
                var lines = new TStack<string>();

                using (var sr = File.OpenText(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        lines.Push(line);
                }

                return lines;
            }

            return null;
        }

        /// <summary>
        /// Pushes given input to the history file so it can be persistent.
        /// </summary>
        /// <param name="line"></param>
        public static void AppendToHistory(string line)
        {
            // If file is more than a MB, empty it.
            // If the file doesn't exist, create it.
            if (!File.Exists(HistoryPath) || new FileInfo(HistoryPath).Length > Terminal.Settings.HistoryMaxSize)
            {
                Polaris.IO.Directory.Create(Terminal.Settings.Directory);
                Polaris.IO.Text.Write(HistoryPath, string.Empty);
            }

            // If file is more than a MB, empty it.

            using (StreamWriter sw = File.AppendText(HistoryPath))
                sw.WriteLine(line);
        }

        /// <summary>
        /// Creates a log file and returns its file location.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateLog(string fileName)
        {
            var path = Path.Combine(Terminal.Settings.Directory, "Logs", $"{fileName}.log");
            
            if (!File.Exists(path))
            {
                Polaris.IO.Directory.Create(Path.Combine(Terminal.Settings.Directory, "Logs"));
                Polaris.IO.Text.Write(path, "Polaris - Log Time! [Polaris Terminal 2.0 - Preview 1]");
            }

            return path;
        }

        /// <summary>
        /// Appends the line to the log file found in the specified path.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="logFilePath"></param>
        public static void AppendToLog(string line, string logFilePath)
        {
            using (StreamWriter sw = File.AppendText(logFilePath))
                sw.WriteLine(line);
        }

        // Compresses all the old log files using .NET's GZip compression.
        public static void CompressOldLogs(string currentLogFile)
        {
            DirectoryInfo logDirectory = new DirectoryInfo(Path.Combine(Terminal.Settings.Directory, "Logs"));
                
            foreach (var fileToCompress in logDirectory.GetFiles("*.log"))
            {
                // ReSharper disable once PossibleNullReferenceException
                // Excludes the current log file.
                if (Path.GetFileName(currentLogFile).Equals(fileToCompress.Name))
                    continue;
                
                try
                {
                    using (FileStream fileStream = fileToCompress.OpenRead())
                    {
                        Polaris.IO.Text.Write($"{fileToCompress.FullName}.gz", fileStream, CompressionType.Lzma);
                    }
                    
                    fileToCompress.Delete();
                }
                catch (Exception e)
                {
                    Terminal.LogWarning($"Could not compress previous log file: {fileToCompress.FullName}. Reason: {e}.");
                }
            }
        }

        /// <summary>
        /// Attempts to delete all archived log files.
        /// </summary>
        public static void ClearLogs()
        {
            DirectoryInfo logDirectory = new DirectoryInfo(Path.Combine(Terminal.Settings.Directory, "Logs"));

            foreach (var logFile in logDirectory.GetFiles("*.gz"))
            {
                try
                {
                    logFile.Delete();
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static void SaveCommonTerminalSettings(CommonTerminalSettings settings)
        {
            try
            {
                Polaris.IO.Yaml.Write(System.IO.Path.Combine(Terminal.Settings.Directory, "Settings"), settings);
            }
            catch (Exception e)
            {
                Terminal.LogWarning($"Could not save Terminal settings.\nReason: {e}.");
            }
        }

        public static CommonTerminalSettings GetCommonTerminalSettings()
        {
            try
            {
                return Polaris.IO.Yaml.Read<CommonTerminalSettings>(System.IO.Path.Combine(Terminal.Settings.Directory,
                    "Settings"));
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
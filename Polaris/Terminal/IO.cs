﻿using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace Polaris.Terminal
{
    /// <summary>
    /// An I/O wrapper/compatibilty layer for Polaris Terminal.
    /// Designed to make it easy to change how I/O is done in Polaris Terminal.
    /// </summary>
    public static class IO
    {
        // Doesn't update when Terminal.Settings.Directory changes. Changes will be applied after game restart.
        private static readonly string HistoryPath = Path.Combine(Terminal.Settings.Directory, "history");

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

        public static void AppendToHistory(string line)
        {
            // Pushes given input to the history file so it can be persistent.
            var path = HistoryPath;

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Terminal.Settings.Directory);
                using (StreamWriter sw = File.CreateText(path))
                { }
            }

            // If file is more than a MB, empty it.
            if (new FileInfo(path).Length > Terminal.Settings.HistoryMaxSize)
                using (StreamWriter sw = File.CreateText(path))
                { }

            using (StreamWriter sw = File.AppendText(path))
                sw.WriteLine(line);
        }

        public static string CreateLog(string fileName)
        {
            // Implement Polaris paths later
            var path = Path.Combine(Terminal.Settings.Directory, $"{fileName}.log");
            
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Terminal.Settings.Directory);
                using (StreamWriter sw = File.CreateText(path))
                    sw.WriteLine("Polaris - Log Time! [Polaris Terminal 2.0 - Preview 1]");
            }

            return path;
        }

        public static void AppendToLog(string line, string logFilePath)
        {
            using (StreamWriter sw = File.AppendText(logFilePath))
                sw.WriteLine(line);
        }

        // Compresses all the old log files using .NET's GZip compression.
        public static void CompressOldLogs(string currentLogFile)
        {
            DirectoryInfo logDirectory = new DirectoryInfo(Terminal.Settings.Directory);
                
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
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                        {
                            using (GZipStream compressionStream =
                                new GZipStream(compressedFileStream, CompressionMode.Compress))
                            {
                                fileStream.CopyTo(compressionStream);
                            }
                        }
                    }
                
                    fileToCompress.Delete();
                }
                catch (Exception e)
                {
                    Terminal.LogWarning($"Could not compress previous log file: {fileToCompress.FullName}. Reason: {e}.");
                }
            }
        }

        public static void SaveCommonTerminalSettings()
        {
            
        }

        public static object GetCommonTerminalSettings()
        {
            return null;
        }
    }
}
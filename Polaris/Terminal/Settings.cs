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
using UnityEngine;

namespace Polaris.Terminal
{
    /// <summary>
    /// Responsible for storing the configuration for the terminal controller.
    /// </summary>
    public class Settings
    {
        private bool _utcTime;
        public bool UtcTime
        {
            get => _utcTime;
            set
            {
                _utcTime = value;
                OnSettingsModified?.Invoke();
            }
        }
        
        private LogType _showStackTrace = LogType.Assertion | LogType.Error;
        /// <summary>
        /// Disabling can slightly reduce CPU and memory usage at the cost of less descriptions about the error.
        /// It is recommended to disable this in release builds.
        /// </summary>
        public LogType ShowStackTrace
        {
            get => _showStackTrace;
            set
            {
                _showStackTrace = value;
                OnSettingsModified?.Invoke();
            }
        }

        public bool ShowStackTraceOnError
        {
            get => (_showStackTrace & LogType.Error) == LogType.Error;
            set
            {
                _showStackTrace |= LogType.Error;
                OnSettingsModified?.Invoke();
            }
        }
        public bool ShowStackTraceOnWarning
        {
            get => (_showStackTrace & LogType.Warning) == LogType.Warning;
            set
            {
                _showStackTrace |= LogType.Warning;
                OnSettingsModified?.Invoke();
            }
        }
        public bool ShowStackTraceOnAssertion
        {
            get => (_showStackTrace & LogType.Assertion) == LogType.Assertion;
            set
            {
                _showStackTrace |= LogType.Assertion;
                OnSettingsModified?.Invoke();
            }
        }
        
        private bool _outputToFile = true;
        /// <summary>
        /// Enabled: Logs are saved into a text file.
        /// Disabling can slightly reduce CPU usage.
        /// </summary>
        public bool OutputToFile
        {
            get => _outputToFile;
            set
            {
                _outputToFile = value;
                OnSettingsModified?.Invoke();
            }
        }

        private byte _maxTerminals = byte.MaxValue;
        /// <summary>
        /// The more terminals present that output the same messages, the more CPU and memory usage.
        /// </summary>
        public byte MaxTerminals
        {
            get => _maxTerminals;
            set
            {
                _maxTerminals = value;
                OnSettingsModified?.Invoke();
            }
        }

        private bool _outputUnityLogs = true;
        /// <summary>
        /// Enabled: Unity logs are also outputted as Terminal logs.
        /// </summary>
        public bool OutputUnityLogs
        {
            get => _outputUnityLogs;
            set
            {
                _outputUnityLogs = value;
                OnSettingsModified?.Invoke();
            }
        }

        private bool _enableHistory = true;
        /// <summary>
        /// Enabled: Queries the user has executed will be saved.
        /// Disabling can slightly reduce CPU usage if input is being spammed.
        /// </summary>
        public bool EnableHistory
        {
            get => _enableHistory;
            set
            {
                _enableHistory = value;
                OnSettingsModified?.Invoke();
            }
        }
        
        private double _historyMaxSize = 1000000;
        /// <summary>
        /// The max number of bytes the history file can be.
        /// Measured in bytes. The smaller the size, the quicker to load.
        /// The entire history file is cached in to memory so make sure it isn't too large.
        /// </summary>
        public double HistoryMaxSize
        {
            get => _historyMaxSize;
            set
            {
                _historyMaxSize = value;
                OnSettingsModified?.Invoke();
            }
        }

        private PermissionLevel _permissionLevel = PermissionLevel.Normal;
        public PermissionLevel PermissionLevel
        {
            get => _permissionLevel;
            set
            {
                _permissionLevel = value;
                OnSettingsModified?.Invoke();
            }
        }

        private string _directory = Path.Combine(Application.persistentDataPath, "Logs");
        /// <summary>
        /// The directory where Terminal-related files will be saved and loaded.
        /// </summary>
        public string Directory
        {
            get => _directory;
            set
            {
                _directory = value;
                OnSettingsModified?.Invoke();
            }
        }

        /// <summary>
        /// Subscribe to this event if you want to do something when the Settings change, such as updating values.
        /// </summary>
        public event Action OnSettingsModified;
    }
}
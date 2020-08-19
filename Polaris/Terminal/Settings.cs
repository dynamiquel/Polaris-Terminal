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

        // Disabling can slightly reduce memory usage at the cost of less descriptions about the error.
        // It is recommended to disable this in release builds.
        private bool _showStackTrace = true;
        public bool ShowStackTrace
        {
            get => _showStackTrace;
            set
            {
                _showStackTrace = value;
                OnSettingsModified?.Invoke();
            }
        }

        private float _fontSize = 28f;
        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnSettingsModified?.Invoke();
            }
        }

        // Disabling can slightly reduce CPU usage.
        private bool _outputToFile = true;
        public bool OutputToFile
        {
            get => _outputToFile;
            set
            {
                _outputToFile = value;
                OnSettingsModified?.Invoke();
            }
        }

        // The more terminals present that output the same messages, the more CPU and memory usage.
        private byte _maxTerminals = byte.MaxValue;
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
        public bool OutputUnityLogs
        {
            get => _outputUnityLogs;
            set
            {
                _outputUnityLogs = value;
                OnSettingsModified?.Invoke();
            }
        }

        // Disabling can slightly reduce CPU usage if input is being spammed.
        private bool _enableHistory = true;
        public bool EnableHistory
        {
            get => _enableHistory;
            set
            {
                _enableHistory = value;
                OnSettingsModified?.Invoke();
            }
        }

        // Measured in bytes. The smaller the size, the quicker to load.
        // The entire history file is cached in to memory so make sure it isn't too large.
        private double _historyMaxSize = 1000000;
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
        public string Directory
        {
            get => _directory;
            set
            {
                _directory = value;
                OnSettingsModified?.Invoke();
            }
        }

        // Subscribe to this event if you want to do something when the Settings change, such as updating values.
        public event Action OnSettingsModified;
    }
}
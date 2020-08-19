using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Polaris.Terminal;
using UnityEngine;

namespace Polaris.Debug.Terminal
{
    // This class gives gives the backend functionality of the developer
    // terminal. It also allows efficient use of having multiple terminals
    // (like having multiple PowerShell windows open).
    public static class Terminal
    {
        #region Variables -> Public

        // Groups the terminal's settings into one object.
        public static TerminalSettings terminalSettings = new TerminalSettings(true, true, true, SearchMode.Awake_Destroy, false, 1);

        // Stores the user's input history. In future, replace with a text file.
        public static TStack<string> inputHistory = ReadHistory();

        #endregion

        #region Variables -> Private

        //static TerminalInputField InputField = new TerminalInputField();

        // The Terminal UIs that the terminal will log to.
        static List<TerminalUI> terminals = new List<TerminalUI>();

        static DateTime timeStarted = DateTime.UtcNow;

        #endregion

        #region Methods -> Public

        static Terminal()
        {
            if (terminalSettings.IncludeUnityLogs)
                Application.logMessageReceived += HandleLog;
            else
                Application.logMessageReceived -= HandleLog;
        }

        // Retrieves the input and processes it.
        public static void ParseInput(string input)
        {
            //Log(input.ToTOutput(LogType.User));
            Polaris.Terminal.Terminal.Log(new LogMessage(input, Polaris.Terminal.LogType.User));

            WriteHistory(input);
         
            // Processes the given string into an array of arguments.
            string[] args = ProcessParameters(input);

            if (args.Length > 0)
                Execute(args);
        }    

        // Gives the command to the shell and logs the user input.
        public static void Execute(string[] args)
        {
            /*TOutput result = Shell.Execute(args);

            if (result.Context != string.Empty)
                Polaris.Terminal.Terminal.Log(new LogMessage(result.Context, Polaris.Terminal.LogType.Error));*/
        }

        // Logs the message into the terminal.
        public static void Log(TOutput message)
        {
            var messageFormatted = new StringBuilder();
            messageFormatted.Append($"<color=#{ColorUtility.ToHtmlStringRGB(message.Colour)}>");

            // If the message is a user log, add a symbol to make it clear in the interface.
            if (message.LogType == LogType.User)
                messageFormatted.Append("> ");

            messageFormatted.Append($"{message.Context}</color>");

            if (terminalSettings.OutputToFile)
                WriteLogToFile(message);
            
            // Outputs the logged message into every terminal found in the game.
            if (terminals.Count > 0)
                foreach (var item in terminals)
                    item.Log(messageFormatted.ToString());
        }

        public static void Log(object message)
        {
            // Converts the unspecified object to a string;
            // Converts the string into a TOutput.
            Log(message.ToTOutput(LogType.Log));
        }

        // Easily accessible methods to allow quick output to the terminal;
        // a replacement for Unity's Debug.Log methods.
        public static void LogWarning(object message)
        {
            Log(message.ToTOutput(LogType.Warning));
        }

        public static void LogError(object message)
        {
            Log(message.ToTOutput(LogType.Error));
        }
        
        // If the Awake search mode is enabled, the adds the given terminal
        // to the terminals list.
        public static void AddTerminalUI(TerminalUI terminalUI)
        {
            if (terminals.Count >= terminalSettings.MaxTerminals)
                GameObject.Destroy(terminalUI.gameObject);
            else
                if (terminalSettings.SearchMode == SearchMode.Awake_Destroy)
                    if (!terminals.Contains(terminalUI))
                    {
                        terminals.Add(terminalUI);
                        Log($"{terminals.Count} terminals active.");
                    }
        }

        // If the Awake search mode is enabled, removes the given terminal
        // from the terminals list.
        public static void RemoveTerminalUI(TerminalUI terminalUI)
        {
            if (terminalSettings.SearchMode == SearchMode.Awake_Destroy)
                if (terminals.Contains(terminalUI))
                {
                    terminals.Remove(terminalUI);
                    Log($"{terminals.Count} terminals active.");
                }
        }

        // Alternate way of getting the curently active terminals;
        // Not as efficient but under certain circumstances, may be required.
        // Requires every Terminal UI game object to be tagged with the TerminalTag.
        public static void GetPresentTerminalUIs()
        {
            if (terminalSettings.SearchMode == SearchMode.Tag)
            {
                GameObject[] terminalGOs = GameObject.FindGameObjectsWithTag(Settings.TerminalTag);

                foreach (var item in terminalGOs)
                {
                    if (terminals.Count >= terminalSettings.MaxTerminals)
                        GameObject.Destroy(item);
                    else
                        if (item.TryGetComponent(out TerminalUI terminalUI))
                                terminals.Add(terminalUI);
                }
            }
        }

        public static string[] GetPotentialCommands(string query)
        {
            var potentialCommands = new List<string>();

            foreach (var command in Shell.Commands) //Check every command and compare them to input
                if (query.Length > 0 && query.Length <= command.Key.Length)
                    if (command.Key.Substring(0, query.Length).ToLower() == query.ToLower())
                        potentialCommands.Add($"{command.Key} - {command.Value.Description}");

            potentialCommands.Sort();

            return potentialCommands.ToArray();
        }

        #endregion

        #region Methods -> Private

        // Logs all the Unity Debug.Log messages to the terminal.
        static void HandleLog(string logMessage, string stackTrace, UnityEngine.LogType type)
        {
            switch (type)
            {
                case UnityEngine.LogType.Error:
                    LogError($"[Unity {type.ToString()}] {logMessage}");
                    break;
                case UnityEngine.LogType.Warning:
                    LogWarning($"[Unity {type.ToString()}] {logMessage}");
                    break;
                case UnityEngine.LogType.Assert:
                    LogWarning($"[Unity {type.ToString()}] {logMessage}");
                    break;
                case UnityEngine.LogType.Exception:
                    LogError($"[Unity {type.ToString()}] {logMessage}");
                    break;
                default:
                    Log($"[Unity {type.ToString()}] {logMessage}");
                    break;
            }
        }

        // Splits the string into an array of arguments that can
        // be individually accessed. [0] is command.
        // Brackets can be used to group arguments that contain
        // spaces.
        // Currently doesn't support groups in groups, e.g. (())
        static string[] ProcessParameters(string input)
        {
            // Removes the excess spacing.
            input.Trim();

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

            foreach (var item in inputParams)
            {
                // Removes the excess spacing.
                item.Trim();
            }

            return inputParams.ToArray();
        }      

        static void WriteLogToFile(TOutput tString)
        {
            string time = timeStarted.ToString("yyyy-MM-dd HH-mm-ss");
            // Implement Polaris paths later
            string path = Path.Combine(Application.persistentDataPath, "UserData", "0", "UserLogs", $"{time}.log");

            if (!File.Exists(path))
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/UserData/0/UserLogs");
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Polaris - Log Time!");
                }
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                var sb = new StringBuilder();
                sb.Append($"{DateTime.UtcNow.ToString()} ");

                switch (tString.LogType)
                {
                    case LogType.User:
                        sb.Append("[USER] ");
                        break;
                    case LogType.Log:
                        sb.Append("[LOG] ");
                        break;
                    case LogType.Warning:
                        sb.Append("[WARNING] ");
                        break;
                    case LogType.Error:
                        sb.Append("[ERROR] ");
                        break;
                    default:
                        sb.Append("[UNKNOWN TYPE] ");
                        break;
                }

                sb.Append(tString.Context);

                sw.WriteLine(sb.ToString());
            }
        }

        static void WriteHistory(string input)
        {
            // Pushes given input to history stack.
            inputHistory.Push(input);

            if (terminalSettings.EnablePersistentHistory)
            {
                // Pushes given input to the history file so it can be persistent.
                string path = Path.Combine(Application.persistentDataPath, "UserData", "0", "UserLogs", $"history.log");

                if (!File.Exists(path))
                {
                    Directory.CreateDirectory($"{Application.persistentDataPath}/UserData/0/UserLogs");
                    using (StreamWriter sw = File.CreateText(path))
                    { }
                }

                // If file is more than a MB, empty it.
                if (new FileInfo(path).Length > 1000000)
                    using (StreamWriter sw = File.CreateText(path))
                    { }

                using (StreamWriter sw = File.AppendText(path))
                    sw.WriteLine(input);
            }
        }

        // If a history file exists, reads it and stores it in the history TStack.
        static TStack<string> ReadHistory()
        {
            TStack<string> lines = new TStack<string>();

            if (terminalSettings.EnablePersistentHistory)
            {
                string path = Path.Combine(Application.persistentDataPath, "UserData", "0", "UserLogs", $"history.log");

                if (File.Exists(path))
                {
                    using (var sr = File.OpenText(path))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                            lines.Push(line);
                    }
                }
            }

            return lines;
        }

        #endregion
    }
}
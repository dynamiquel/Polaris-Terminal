using System.Collections;
using System.Collections.Generic;
using System.Text;
using Polaris.Terminal;
using TMPro;
using UnityEngine;

namespace Polaris.Debug.Terminal
{
    // The interface of Terminal that displays the output of the
    // Terminal and sends input to the Terminal.
    public class TerminalUI : MonoBehaviour
    {
        #region Variables -> Public

        // Unity interactions.
        public TextMeshProUGUI _outputText;
        public static TextMeshProUGUI outputText;
        public TMP_InputField inputText;
        public GameObject potentialCommandsSection;
        public GameObject potentialCommandPrefab;

        // TerminalUI Settings
        // Prevents the terminal from destroying when scene is changed.
        // Must be set before Awake().
        public bool dontDestroyOnSceneChange = true;
        public bool includeTimestamps = true;
        public bool displayPotentialCommands = true;
        public float fontSize;

        #endregion

        #region Variables -> Private

        // Current history index of this particular interface.
        int inputHistoryIndex = -1;

        #endregion

        #region Methods -> Unity

        private void Awake()
        {
            if (dontDestroyOnSceneChange)
                DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            outputText = _outputText;
            
            Polaris.Terminal.Terminal.OnLog += Log2;
            
            // Automatically starts the shell if it hasn't already started. Adds commands at startup.
            var startShell = Polaris.Terminal.Shell.Started;
            
            //Terminal.GetPresentTerminalUIs();
            //Terminal.AddTerminalUI(this);
        }

        private void Update()
        {
            UserInput();
        }

        private void OnDestroy()
        {
            // Removes this terminal interface from the terminals list.
            Terminal.RemoveTerminalUI(this);
        }

        #endregion

        #region Methods -> Public

        public void Log(string message)
        {
            // Output message into terminal.
            outputText.text += $"\n{message}";
        }

        private void Log2(LogMessage logMessage)
        {
            var messageFormatted = new StringBuilder();
            messageFormatted.Append($"<color=#{logMessage.Colour.HexRgb}>");

            // If the message is a user log, add a symbol to make it clear in the interface.
            if (logMessage.LogType == Polaris.Terminal.LogType.User)
                messageFormatted.Append("> ");

            messageFormatted.Append($"{logMessage.Content}</color>");
            
            outputText.text += $"\n{messageFormatted}";
        }

        public void On_InputChange()
        {
            var potentialCommands = Terminal.GetPotentialCommands(inputText.text);

            foreach (Transform item in potentialCommandsSection.transform)
                Destroy(item.gameObject);

            foreach (var item in potentialCommands)
            {
                GameObject tempGO = Instantiate(potentialCommandPrefab);
                tempGO.GetComponent<Transform>().SetParent(potentialCommandsSection.transform);
                tempGO.GetComponent<TextMeshProUGUI>().text = item;
            }
        }

        #endregion

        #region Methods -> Private

        // Sends the input to the terminal.
        void ParseInput()
        {
            string input = inputText.text;
            inputText.text = string.Empty;
            Polaris.Terminal.Terminal.Execute(input, true);

            // Reset the history index when input given.
            inputHistoryIndex = -1;
        }

        // Displays the selected history string in the input text box.
        void InputHistoryHandler()
        {
            if (inputHistoryIndex >= Polaris.Terminal.Terminal.InputHistory.Count)
                inputHistoryIndex = Polaris.Terminal.Terminal.InputHistory.Count - 1;
         
            if (inputHistoryIndex == -1)
                inputText.text = string.Empty;
            else
                inputText.text = Polaris.Terminal.Terminal.InputHistory.Peek(inputHistoryIndex);
        }

        // Listens to user input.
        void UserInput()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                ParseInput();
            // Goes one up in the history.
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputHistoryIndex++;
                InputHistoryHandler();
            }
            // Goes one down in the history.
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (inputHistoryIndex > -1)
                    inputHistoryIndex--;
                InputHistoryHandler();
            }
            // Quick access to go to top of history.
            else if (Input.GetKeyDown(KeyCode.Home))
            {
                inputHistoryIndex = Terminal.inputHistory.Count - 1;
                InputHistoryHandler();
            }
            // Quick access to go to bottom of history.
            else if (Input.GetKeyDown(KeyCode.End))
            {
                inputHistoryIndex = -1;
                InputHistoryHandler();
            }
        }

        #endregion
    }
}

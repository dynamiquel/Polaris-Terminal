using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Polaris.Terminal.Unity
{
    public class CommonTerminal : MonoBehaviour, ITerminalUI
    {
        protected enum TerminalOverlaySize
        {
            Fullscreen,
            Halfscreen,
            Topbar
        }
        
        #region Fields -> Unity Serialisable

        [Header("Base Options")] 
        [SerializeField] private string titleName = "Terminal";
        [SerializeField] [Tooltip("Should the timestamp of the message be displayed?")]
        private bool includeTimestamps = true;
        
        [SerializeField] [Tooltip("Should the Log Type of the message be displayed?")]
        private bool includeLogType;
        
        [SerializeField] [Tooltip("Should the Log Source of the message be displayed?")]
        private bool includeLogSource;
        
        [SerializeField] private bool dontDestroyOnSceneChange = true;
        [SerializeField] private bool displayPredictions = true;
        
        [SerializeField] [Tooltip("Which Log Types should be outputted to the Terminal?" +
                                  "\n\nIgnore the Everything and Im Just Here Coz Unity Dumb options.")]
        private LogType logTypesToOutput = LogType.All;

        [SerializeField] [Tooltip("Which Log Sources should be outputted to the Terminal?")]
        LogSource logSourcesToOutput;
        
        [SerializeField] [Min(10)]
        [Tooltip("The maximum number of lines the Terminal should store." +
                 "\n\nThis will help reduce memory usage and slow down garbage collection.")]
        private int maxCharacters = 50000;
        
        [Header("Overlay Options")]
        [SerializeField] [Tooltip("Determines how much much of the screen the Terminal uses." +
                                  "\nFullscreen: Terminal uses the entire screen." +
                                  "\nHalfscreen: Terminal uses 1/3 of the screen (like most games)." +
                                  "\nTerminal becomes a banner at the top of the screen. No input can be given.")]
        private TerminalOverlaySize overlaySize;
        
        [Range(0, 1)] [SerializeField]
        private float _overlayOpacity = 1f;
        public float OverlayOpacity
        {
            get => _overlayOpacity;
            set => SetOverlayOpacity(value);
        }
        
        [SerializeField] [Tooltip("Should the cursor be enabled when the Terminal is enabled?")]
        private bool enableCursor;

        [Header("Pinned Options")]
        [SerializeField] [Range(0, 1)] 
        private float pinnedOpacity = 1f;
        [SerializeField] [Tooltip("Determines how much much of the screen the Terminal uses when pinned." +
                                  "\nFullscreen: Terminal uses the entire screen." +
                                  "\nHalfscreen: Terminal uses 1/3 of the screen (like most games)." +
                                  "\nTerminal becomes a banner at the top of the screen. No input can be given.")]
        private TerminalOverlaySize pinnedOverlaySize;

        [Header("User Input")] 
        [SerializeField] private string togglePin = "Toggle Terminal Pin";
        [SerializeField] private string clear = "Terminal Clear";
        [SerializeField] private string toggleSettings = "Toggle Terminal Settings";
        [SerializeField] private string togglePause = "Toggle Terminal Pause";

        [Header("Audio")] 
        [SerializeField] [Range(0f, 1f)]
        private float volume = 1f;
        [SerializeField] [Tooltip("Should notification sounds be played when the Terminal is closed?")]
        private bool playSoundsWhenClosed = true;
        [SerializeField] [Tooltip("Should notification sounds be played when the Terminal is pinned?")]
        private bool playSoundsWhenPinned = true;
        [SerializeField] [Tooltip("Should only one sound be played at any given time?\n\nIf disabled, overlapping may occur.")] 
        private bool oneSoundAtATime = true;
        [SerializeField] [Tooltip("Which Log Types should play a notification sound when a message is received?")]
        private LogType playSoundForLogTypes = LogType.None;
        [SerializeField] [Tooltip("The sound played when a message with LogType.Log is received.")] 
        private AudioClip logSound;
        [SerializeField] [Tooltip("The sound played when a message with LogType.System is received.")]
        private AudioClip systemSound;
        [SerializeField] [Tooltip("The sound played when a message with LogType.Warning is received.")] 
        private AudioClip warningSound;
        [SerializeField] [Tooltip("The sound played when a message with LogType.Assertion is received.")] 
        private AudioClip assertionSound;
        [SerializeField] [Tooltip("The sound played when a message with LogType.Error is received.")] 
        private AudioClip errorSound;

        [Header("Main Components")]
        [Header("Components - Don't edit unless you know what you're doing!")]
        [SerializeField] private GameObject window;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI outputText;
        [SerializeField] private Button inputButton;

        [Header("Title Bar Components")] 
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button pinButton;
        [SerializeField] private TextMeshProUGUI pinText;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private TextMeshProUGUI pauseText;

        // Used to change the Terminal's opacity.
        [Header("Background Components")] 
        [SerializeField] private Image titleBar;
        [SerializeField] private Image outputSection;
        [SerializeField] private Image inputSection;
        
        #endregion


        #region Properties -> Public
        
        private bool _active;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                SetupOverlay();
            }
        }

        private bool _paused;
        public bool Paused
        {
            get => _paused;
            set
            {
                _paused = value;
                pauseText.text = Paused ? "\uE768" : "\uE769";
            }
        }
        
        #endregion


        #region Fields/Properties -> Protected
        
        private bool _pinned;
        protected bool Pinned
        {
            get => _pinned;
            set
            {
                _pinned = value;
                pinText.text = Pinned ? "\uE77A" : "\uE840";
            }
        }
        
        #endregion

        
        #region Fields -> Private

        private GameObject thisGameObject;
        
        private AudioSource audioSource;
        private LogType currentlyPlayedLogType;

        private int inputHistoryIndex;
        private CursorLockMode previousCursorLockMode;
        private float previousOverlayOpacity;
        
        #endregion

        
        #region Methods -> Unity

        protected void Awake()
        {
            // Caching purposes.
            thisGameObject = gameObject;
            
            audioSource = thisGameObject.AddComponent<AudioSource>();
            InitialiseAudio();

            // Subscribes to the Terminal.OnLog event in order to receive logs.
            Terminal.OnLog += Write;

            if (dontDestroyOnSceneChange)
                DontDestroyOnLoad(thisGameObject);
            
            // Load config from serialised file.
            Pinned = false;

            SubscribeToUIEvents();
        }

        protected void Start()
        {
            Active = false;
        }

        protected void Update()
        {
            UserInput();
        }

        // Updates certain values if values in the Inspector have changed.
        protected void OnValidate()
        {
            // Prevents anything from being done before Awake() is called.
            if (thisGameObject == null)
                return;
            
            // If only the Inspector supported properties...
            if (Math.Abs(_overlayOpacity - previousOverlayOpacity) > .01f)
                SetOverlayOpacity(_overlayOpacity);
            
            // Update audio stuff.
            InitialiseAudio();
        }
        
        #endregion


        #region Methods -> Public

        public void Write(LogMessage logMessage)
        {
            // Ignores the message if this component is disabled or paused.
            if (!enabled || Paused)
                return;
            
            // Ignores the message if it has a log type or source that the terminal does not accept.
            if ((logTypesToOutput & logMessage.LogType) != logMessage.LogType || 
                (logSourcesToOutput & logMessage.LogSource) != logMessage.LogSource)
                return;
            
            if (outputText.text.Length > maxCharacters)
                outputText.text = outputText.text.Remove(0, outputText.text.Length - maxCharacters);
                
            var formattedMessage = new StringBuilder();

            // Adds colour.
            formattedMessage.Append($"\n<color=#{logMessage.Colour.HexRgb}>");

            // Adds timestamp.
            if (includeTimestamps)
                formattedMessage.Append($"[{logMessage.Timestamp:HH:mm:ss}] ");

            // Starts the bold text.
            formattedMessage.Append("<b>");
            
            if (logMessage.LogType == LogType.User)
                formattedMessage.Append("> ");
            else
            {
                // Adds the log source.
                if (includeLogSource)
                    formattedMessage.Append($"[{logMessage.LogSource.ToString()}] ");
                
                // Adds the log type.
                if (includeLogType)
                    formattedMessage.Append($"[{logMessage.LogType.ToString()}] ");
            }

            // Ends the bold text, adds the content, and ends the colour.
            formattedMessage.Append($"</b>{logMessage.Content}</color>");
            
            outputText.text += formattedMessage;
            
            if (Active || Pinned && playSoundsWhenPinned || !Pinned && playSoundsWhenClosed)
                PlaySound(logMessage.LogType);
        }

        public void Clear()
        {
            outputText.text = string.Empty;
        }

        public void ParseInput()
        {
            var text = inputField.text;
            
            if (!string.IsNullOrWhiteSpace(text))
                Terminal.Execute(text, true);
            
            inputField.text = string.Empty;
            inputField.ActivateInputField();

            // Reset the history index when input given.
            inputHistoryIndex = -1;
        }
        
        #endregion


        #region Methods -> Protected

        protected void UserInput()
        {
            if (Active)
            {
                if (Input.GetButtonDown(togglePin))
                    Pinned = !Pinned;
                else if (Input.GetButtonDown(clear))
                    Clear();
                else if (Input.GetButtonDown(toggleSettings))
                    OpenSettings();
                else if (Input.GetButtonDown(togglePause))
                    Paused = !Paused;

                if (!Pinned)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        inputHistoryIndex++;
                        PopulateInputFieldWithHistory();
                    }
                    else if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        inputHistoryIndex--;
                        PopulateInputFieldWithHistory();
                    }
                    else if (Input.GetKeyDown(KeyCode.Home))
                    {
                        inputHistoryIndex = Terminal.InputHistory.Count - 1;
                        PopulateInputFieldWithHistory();
                    }
                    else if (Input.GetKeyDown(KeyCode.End))
                    {
                        inputHistoryIndex = -1;
                        PopulateInputFieldWithHistory();
                    }
                }
            }
        }

        #endregion


        #region Methods -> Private
        
        private void SubscribeToUIEvents()
        {
            var notAssignedErrorMsg = $"{thisGameObject.name}:{{0}} is not assigned. The Terminal may not work correctly.";

            if (inputButton == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(inputButton)));
            else
                inputButton.onClick.AddListener(ParseInput);

            if (inputField == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(inputField)));
            else
                inputField.onSubmit.AddListener(HandleInputFieldParseInput);

            if (closeButton == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(closeButton)));
            else
                closeButton.onClick.AddListener(HandleCloseButton);

            if (pinButton == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(pinButton)));
            else
                pinButton.onClick.AddListener(HandleTogglePinButton);

            if (settingsButton == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(settingsButton)));
            else
                settingsButton.onClick.AddListener(OpenSettings);

            if (clearButton == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(clearButton)));
            else
                clearButton.onClick.AddListener(Clear);
            
            if (pauseButton == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(pauseButton)));
            else
                pauseButton.onClick.AddListener(HandleTogglePauseButton);
        }
        
        // Yeah, it's not efficient but it's not like you'll be spamming the toggle button.
        private void SetupOverlay()
        {
            // If Terminal is Active, enable all interactions and use the desired main aesthetic.
            // If Terminal is not Active, but Pinned, disable all interactions and use the desired pinned aesthetic.
            // If Terminal is not Active nor Pinned, disable everything.
            
            window.SetActive(Active || Pinned);

            // Setup interactions.
            var imageComponents = GetComponentsInChildren<Image>();
            foreach (var imageComponent in imageComponents)
                imageComponent.raycastTarget = Active;
            
            var buttonComponents = GetComponentsInChildren<Button>();
            foreach (var buttonComponent in buttonComponents)
                buttonComponent.interactable = Active;

            var inputFieldComponents = GetComponentsInChildren<TMP_InputField>();
            foreach (var inputFieldComponent in inputFieldComponents)
                inputFieldComponent.interactable = Active;

            // Setup background opacity.
            if (Active)
            {
                titleText.text = titleName;
                SetOverlayOpacity(_overlayOpacity);
                
                // Focus on the input field.
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();

                if (enableCursor)
                    Cursor.lockState = previousCursorLockMode;
            }
            else
            {
                if (Pinned)
                {
                    titleText.text = $"{titleName} (Pinned)";
                    SetOverlayOpacity(pinnedOpacity, true);
                }

                if (enableCursor)
                    Cursor.lockState = previousCursorLockMode;
            }
        }
        
        private void SetOverlayOpacity(float opacity, bool pinned = false)
        {
            var notAssignedErrorMsg =
                $"{thisGameObject.name}:{{0}} is not assigned. Opacity will not change.";

            if (pinned)
                pinnedOpacity = opacity;
            else
                _overlayOpacity = opacity;
            
            previousOverlayOpacity = _overlayOpacity;

            Color colour;
            
            // Set Title Bar background opacity.
            if (titleBar == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(titleBar)));
            else
            {
                colour = titleBar.color;
                colour.a = opacity;
                titleBar.color = colour;
            }
            
            if (outputSection == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(outputSection)));
            else
            {
                // Set Output Section background opacity.
                colour = outputSection.color;
                colour.a = opacity;
                outputSection.color = colour;
            }

            if (inputSection == null)
                Terminal.LogWarning(LogSource.UI, string.Format(notAssignedErrorMsg, nameof(inputSection)));
            else
            {
                // Set Input Section background opacity.
                colour = inputSection.color;
                colour.a = opacity;
                inputSection.color = colour;
            }
        }

        private void PopulateInputFieldWithHistory()
        {
            if (inputHistoryIndex >= Terminal.InputHistory.Count)
                inputHistoryIndex = Terminal.InputHistory.Count - 1;
            else if (inputHistoryIndex < -1)
                inputHistoryIndex = -1;

            if (inputHistoryIndex == -1)
                inputField.text = string.Empty;
            else
                inputField.text = Terminal.InputHistory.Peek(inputHistoryIndex);
        }
        
        private List<Command> GetPotentialCommands(string query)
        {
            return null;
        }
        
        /// <summary>
        /// Plays a certain notification sound, depending on the LogType.
        /// </summary>
        /// <param name="logType"></param>
        private void PlaySound(LogType logType)
        {
            // Ignore if sounds were disabled for the given LogType.
            if ((playSoundForLogTypes & logType) != logType) 
                return;

            AudioClip audioClipToPlay;
                
            switch (logType)
            {
                case LogType.None:
                case LogType.User:
                case LogType.ImJustHereCozUnityDumb:
                    return;
                case LogType.Log:
                    audioClipToPlay = logSound;
                    break;
                case LogType.System:
                    audioClipToPlay = systemSound;
                    break;
                case LogType.Warning:
                    audioClipToPlay = warningSound;
                    break;
                case LogType.Assertion:
                    audioClipToPlay = assertionSound;
                    break;
                case LogType.Error:
                    audioClipToPlay = errorSound;
                    break;
                default:
                    return;
            }
            
            // Ignore if the audio clip is null.
            /*if (audioClipToPlay == null) 
                return;
            */
            if (oneSoundAtATime && audioSource.isPlaying)
            {
                // Ignore if the selected clip is already been played.
                if (audioSource.clip == audioClipToPlay)
                    return;

                // Ignore if the current clip is a higher priority then the selected clip (Error > Log).
                if (logType < currentlyPlayedLogType)
                    return;
            }

            // Plays the corresponding audio clip for the given LogType.
            currentlyPlayedLogType = logType;
            audioSource.clip = audioClipToPlay;
            audioSource.Play();
        }

        private void InitialiseAudio()
        {
            audioSource.volume = volume;
        }

        private void OpenSettings()
        {
            
        }

        private void HandleTogglePauseButton()
        {
            Paused = !Paused;
        }

        private void HandleInputFieldParseInput(string input)
            => ParseInput();
        
        private void HandleCloseButton()
            => Active = false;

        private void HandleTogglePinButton()
            => Pinned = !Pinned;
        
        #endregion
    }
}
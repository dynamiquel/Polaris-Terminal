using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Polaris.Terminal.Unity
{
    public class CommandPrediction : MonoBehaviour
    {
        [Header("Command Prediction Components")] [SerializeField]
        private GameObject predictedCommandPrefab;

        public bool Active { get; private set; }

        private int _index = 0;
        public int Index
        {
            get => _index;
            set
            {
                // I hate this code. buttons.Count = none selected. Wanted to use -1 but that would have required
                // some adjustments elsewhere.
                // Somewhat inefficient but it's not like its being spammed since doesn't matter too much.
                if (value > commandBackgrounds.Count)
                    return;
                
                if (value < 0)
                    value = commandBackgrounds.Count - 1;

                _index = value;

                if (selectedCommand != null)
                {
                    var color = selectedCommand.color;
                    color.a = float.Epsilon;
                    selectedCommand.color = color;
                }
                
                selectedCommand = Index == commandBackgrounds.Count ? null : commandBackgrounds[Index];

                if (selectedCommand != null)
                {
                    var color2 = selectedCommand.color;
                    color2.a = .8f;
                    selectedCommand.color = color2;
                }
            }
        }

        public string SelectedPrediction => Index >= 0 ? commandBackgrounds[Index].GetComponentInChildren<TextMeshProUGUI>().text : 
            commandBackgrounds[0].GetComponentInChildren<TextMeshProUGUI>().text;

        private Transform _transform;
        private readonly List<Image> commandBackgrounds = new List<Image>();
        private Image background;
        private Image selectedCommand;

        void Awake()
        {
            _transform = transform;
            background = GetComponent<Image>();
        }

        public void Query(string query)
        {
            // Maybe a bit overkill but it's an existing implementation that does the job.
            // Splits the command and parameters from the input. We don't actually care about the parameter, just the
            // number of parameters.
            var queryInfo = QueryInfo.FromString(query);
            var queriedCommands = Shell.GetCommands(queryInfo.Command);

            // Gets all the commands that have at least the same number of parameters as the input.
            var potentialCommands = new List<Command>();
            foreach (var potentialCommand in queriedCommands)
                if (potentialCommand.Parameters.Count >= queryInfo.Parameters.Count)
                    potentialCommands.Add(potentialCommand);

            // Clear the predicted commands.
            foreach (var button in commandBackgrounds)
                Destroy(button.gameObject);
            commandBackgrounds.Clear();

            // Reset the index to none.
            _index = commandBackgrounds.Count;
            
            Active = potentialCommands.Count > 0;
            background.enabled = Active;

            // Create the predicted commands.
            foreach (var item in potentialCommands)
            {
                GameObject tempGO = Instantiate(predictedCommandPrefab, _transform);

                var sb = new StringBuilder();
                sb.Append(item.Id);
                foreach (var parameter in item.Parameters)
                    sb.Append($" ({parameter.Key})");

                tempGO.GetComponentInChildren<TextMeshProUGUI>().text = sb.ToString();
                commandBackgrounds.Add(tempGO.GetComponent<Image>());
            }
        }
    }
}
using UnityEngine;

namespace Polaris.Debug.Terminal
{
    public enum LogType
    {
        User,
        Log,
        Warning,
        Error
    }

    // This class is simply a string that has a colour with it. Helps the console know
    // what colour to display with the string.
    public class TOutput
    {
        public string Context { get; set; }
        public Color Colour { get; set; }
        public LogType LogType { get; set; }

        public TOutput(string context, LogType logType = LogType.Log, Color colour = new Color())
        {
            this.Context = context;
            this.LogType = logType;

            if (colour != new Color())
            {
                Colour = colour;
            }
            else
            {
                switch (logType)
                {
                    case LogType.User:
                        this.Colour = Debug.Colour.User;
                        break;
                    case LogType.Log:
                        this.Colour = Debug.Colour.Default;
                        break;
                    case LogType.Warning:
                        this.Colour = Debug.Colour.Warning;
                        break;
                    case LogType.Error:
                        this.Colour = Debug.Colour.Error;
                        break;
                }
            }
        }
    }
}

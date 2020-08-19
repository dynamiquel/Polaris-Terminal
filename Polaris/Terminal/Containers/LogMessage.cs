using System;
using Polaris.Core;

namespace Polaris.Terminal
{
    /// <summary>
    /// The objects that are sent as 'logs' throughout Polaris Terminal.
    /// The object contains the message as a string, as well as other important data.
    /// Each LogMessage is 17 bytes (excl. Content).
    /// </summary>
    public class LogMessage
    {
        public string Content { get; set; } = string.Empty;
        public Colour Colour { get; set; } = default;
        private LogType _logType = LogType.Log;
        public LogType LogType
        {
            get => _logType;
            set
            {
                _logType = value;

                if (Colour.IsEmpty)
                    Colour = DetermineColour(value);
            }
        }
        public LogSource LogSource { get; set; } = LogSource.Generic;
        public DateTime Timestamp { get; set; } = Terminal.Settings.UtcTime ? DateTime.UtcNow : DateTime.Now;

        public LogMessage(string content, LogType logType = LogType.Log, Colour colour = default, LogSource logSource = LogSource.Generic)
        {
            Content = content;
            LogType = logType;
            LogSource = logSource;

            Colour = colour.IsEmpty ? DetermineColour(LogType) : colour;
        }

        // Constructor used for object-initialisation.
        // It is recommended to use object-initialisation over construction if you are changing multiple defaults,
        // just because it looks better :)
        public LogMessage() { }

        private static Colour DetermineColour(LogType logType)
        {
            switch (logType)
            {
                case LogType.User:
                    return Colour.User;
                case LogType.System:
                    return Colour.System;
                case LogType.Warning:
                    return Colour.Warning;
                case LogType.Assertion:
                    return Colour.Assertion;
                case LogType.Error:
                    return Colour.Error;
                case LogType.Log:
                    return Colour.System;
                default:
                    return Colour.System;
            }
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
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
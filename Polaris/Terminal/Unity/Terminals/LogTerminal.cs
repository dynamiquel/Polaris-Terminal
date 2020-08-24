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

using System.Text;

namespace Polaris.Terminal.Unity
{
    public class LogTerminal : CommonTerminal
    {
        public override void Write(LogMessage logMessage)
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
        
        public override void Clear()
        {
            outputText.text = string.Empty;
        }
    }
}
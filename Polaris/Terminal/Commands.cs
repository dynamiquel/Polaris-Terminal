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

namespace Polaris.Terminal
{
    public class Commands
    {
        [Command("hello")]
        public class Hello : Command
        {
            public override string Name { get; protected set; } = "Hello";
            public override string Category { get; protected set; } = "Misc";
            public override string Description { get; protected set; } = "Don't be rude, say hello";

            public override LogMessage Execute()
            {
                return new LogMessage("Hello, World!");
            }
        }
        
        [Command("beep")]
        public class Beep : Command
        {
            public override string Name { get; protected set; } = "Beep";
            public override string Category { get; protected set; } = "Misc";
            public override string Description { get; protected set; } = "Plays a beep sound";
            
            public override LogMessage Execute()
            {
                System.Media.SystemSounds.Beep.Play();
                return new LogMessage("Beep!");
            }
        }
        
        [Command("help")]
        public class Help : Command
        {
            public override string Name { get; protected set; } = "Help";
            public override string Category { get; protected set; } = "Misc";
            public override string Description { get; protected set; } = "Shows what a command does and how to use it";

            [CommandParameter("command ID")] 
            public string commandId = "help";

            public override LogMessage Execute()
            {
                if (Shell.Commands.ContainsKey(commandId))
                {
                    var sb = new StringBuilder();

                    if (Shell.Commands.TryGetValue(commandId, out var cmd))
                    {
                        /*var requiredArguments = Shell.GetRequiredArguments(cmd);
    
                        var requiredArgumentsSB = new StringBuilder();
                        foreach (var item in requiredArguments)
                            requiredArgumentsSB.Append($"({item}) ");
    */
                        sb.AppendLine($"Manual (help) for the '{cmd.Name}' command");
                        sb.AppendLine($"\tCommand: {cmd.Id}");
                        if (!string.IsNullOrEmpty(cmd.Category))
                            sb.AppendLine($"\tCategory: {cmd.Category}");
                        
                        if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                        {
                            sb.Append($"\tRequired Arguments:");

                            foreach (var parameter in cmd.Parameters)
                                sb.Append($" {parameter.Key} ({parameter.Value.genericType}),");

                            sb.Append("\n");
                        }

                        if (!string.IsNullOrEmpty(cmd.SoftParameterInfo))
                            sb.AppendLine($"\tOptional Arguments: {cmd.SoftParameterInfo}");
                        sb.AppendLine($"\tPermission Level: {cmd.PermissionLevel.ToString()}");
                        if (!string.IsNullOrEmpty(cmd.Description))
                            sb.AppendLine($"\tDescription: {cmd.Description}");
                        if (!string.IsNullOrEmpty(cmd.Manual))
                            sb.AppendLine($"\tManual: {cmd.Manual}");
                    }
                    else
                        sb.AppendLine($"The command '{commandId}' is not valid.");

                    return new LogMessage(sb.ToString());
                }
                
                return new LogMessage($"The command '{commandId}' is not valid.");
            }
        }
    }
}
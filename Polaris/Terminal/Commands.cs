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
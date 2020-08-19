using System.Collections.Generic;
using System.Linq;
using Polaris.Terminal;
using Command = Polaris.Debug.Terminal.Commands.Command;

namespace Polaris.Debug.Terminal
{
    // This class stores all the available commands and allows the commands to be executed.
    public static class Shell
    {
        #region Variables -> Public

        // The stored commands. The string key allows quick access to the command by command name.
        // A dictionary is faster than a switch statement.
        static Dictionary<string, Command> _commands = new Dictionary<string, Command>();
        public static Dictionary<string, Command> Commands
        {
            get => _commands;
            private set { }
        }

        #endregion

        static Shell()
        {
            CreateCommands();
        }

        #region Methods -> Public

        public static void RegisterCommand(Command command)
        {
            Commands[command.Cmd] = command;
            Terminal.Log(string.Format("'{0}' command was added to the console.", command.Cmd));
        }

        // Finds a command with a given key and executes it with the given arguments.
        public static TOutput Execute(QueryInfo queryInfo)
        {
            // If the command could not be found, then it is likely, it's not a valid command.
            if (!_commands.ContainsKey(queryInfo.Command))
            {
                return $"'{queryInfo.Command}' does not exist.".ToTOutput(LogType.Error);
            }

            var tempCmd = _commands[queryInfo.Command];
            // If the user doesn't have permission to use the command, then return an error message.
            if (Settings.CurrentPermissionLevel < tempCmd.PermissionLevel)
                return "You do not have permission to use this command.".ToTOutput(LogType.Error);

            // If the user hasn't entered all the required arguments, then return an error message.
            if (queryInfo.Parameters.Count < tempCmd.RequiredArguments.Count)
            {
                IList<string> requiredArguments = GetRequiredArguments(tempCmd);
                var sb = new System.Text.StringBuilder("You have not entered all the required arguments. ");

                foreach (var item in requiredArguments)
                    sb.Append($"({item}) ");

                return sb.ToTOutput(LogType.Error);
            }

            // Check if all the given arguments are the correct type.
            if (tempCmd.RequiredArguments.Count > 0)
            {
                for (int i = 0; i < tempCmd.RequiredArguments.Count; i++)
                {
                    if (!tempCmd.RequiredArguments[i].IsValid(queryInfo.Parameters[i]))
                        return $"Argument Error: {tempCmd.RequiredArguments[i].Error} at position {i}.".ToTOutput(LogType.Error);
                }
            }        

            // Executes the given command and returns the string result.
            TOutput result = _commands[queryInfo.Command].Execute(queryInfo.Parameters.ToArray());
            return result;
        }

        #endregion

        #region Methods -> Private

        // Creates an object of all commands and registers them.
        static void CreateCommands()
        {
            new Commands.Commands();
        }

        public static IList<string> GetRequiredArguments(Command cmd)
        {
            var requiredArguments = new List<string>();

            for (int i = 0; i < cmd.RequiredArguments.Count; i++)
            {
                string temp = cmd.RequiredArguments[i].GetType().ToString();
                requiredArguments.Add(temp);
            }

            return requiredArguments;
        }

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polaris.Debug.Terminal.Commands
{
    public abstract class Command
    {
        // A display name (doesn't affect functionality).
        public string Name { get; set; }
        // The command needed to be entered. Must include no spaces.
        public string Cmd { get; set; }
        // A display category (doesn't affect functionality).
        public string Category { get; set; }
        // The PermissionLevel required by the user to execute the command.
        public PermissionLevel PermissionLevel { get; set; }
        // A brief description of the command (doesn't affect functionality).
        public string Description { get; set; }
        // Brief info on type of arguments that the command can accept and in which order
        // (doesn't affect functionality).
        public string ArgumentInfo { get; set; }
        // A detailed manual on what the command does and how to use it
        // (doesn't affect functionality).
        public string Manual { get; set; }
        // The type of arguments that must be given for the command to execute.
        // The position the argument is in the array, is the position it must
        // be entered in. E.g. args[2] must be RequiredArguments[2].
        // Optional arguments must be entered after all required arguments.
        // E.g. if ReqiredArguments max index = 3, then args[4] is optional.
        // If this doesn't work properly with your command, then just use
        // entirely optional arguments that are checked in the Execute method.
        //public Arguments.Argument[] RequiredArguments { get; private set; } // If doesn't work, can try Generics. Argument<int> and use if statements.
        public List<Argument> RequiredArguments { get; set; }

        public Command()
        {
            RequiredArguments = new List<Argument>();
        }

        // Used to execute the command.
        public abstract TOutput Execute(string[] args);

        // Registers the command to the shell.
        public void Register()
        {
            Shell.RegisterCommand(this);
        }
    }
}
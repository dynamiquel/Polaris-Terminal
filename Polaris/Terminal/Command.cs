using System;
using System.Collections.Generic;

namespace Polaris.Terminal
{
    /// <summary>
    /// Responsible for executing logic.
    /// </summary>
    public class Command
    {
        public string Id =>
            ((CommandAttribute) Attribute.GetCustomAttribute(GetType(), typeof(CommandAttribute)))
            .commandId;
        public virtual string Name {get; protected set;} = string.Empty;
        public virtual string Category {get; protected set;} = string.Empty;
        public virtual string Description {get; protected set;} = string.Empty;
        public virtual string SoftParameterInfo {get; protected set;} = string.Empty;
        public virtual string Manual { get; protected set; } = string.Empty;
        public virtual PermissionLevel PermissionLevel { get; protected set; } = PermissionLevel.Normal;
        public Dictionary<string, CommandParameter> Parameters {get; private set;} = new Dictionary<string, CommandParameter>();

        public virtual LogMessage Execute()
        {
            return new LogMessage
            {
                LogSource = LogSource.Commands,
                LogType = LogType.System
            };
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal sealed class CommandAttribute : System.Attribute
    {
        public readonly string commandId;
        
        public CommandAttribute(string commandId)
        {
            this.commandId = commandId;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    internal sealed class CommandParameterAttribute : System.Attribute
    {
        public readonly string description;
        public CommandParameter commandParameter;
        
        public CommandParameterAttribute(string description)
        {
            this.description = description;
        }
    }
    
    // Forked from Reactor-Developer-Console-1.2.
    public abstract class CommandParameter
    {
        private object value;
        private object defaultValue;
        public Type genericType;
        public Command command; //Invokable command that uses this as a parameter
        public System.Reflection.FieldInfo fieldInfo; // Field name of command linked to this parameter

        public object Value
        {
            get => value;
            set 
            {
                this.value = value;
                fieldInfo.SetValue(command, value);
            }
        }

        public object DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }
    }

    public class CommandParameter<TOption> : CommandParameter
    {
        public new TOption Value
        {
            get
            {
                if (base.Value == null)
                    return default;
                
                return (TOption) base.Value;
            }
        }

        public new TOption DefaultValue
        {
            get
            {
                if (base.DefaultValue == null)
                    return default;

                return (TOption) base.DefaultValue;
            }
        }
        
        public CommandParameter(Command parentCommand, System.Reflection.FieldInfo fieldInfo)
        {
            genericType = typeof(TOption);
            command = parentCommand;
            this.fieldInfo = fieldInfo;
        }
    }
}
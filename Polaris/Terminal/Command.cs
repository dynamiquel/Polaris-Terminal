//  This file is part of Polaris-Terminal - A developer console for Unity.
//  https://github.com/dynamiquel/Polaris-Options
//  Copyright (c) 2020 dynamiquel

//  Parts of this file is part of Reactor-Developer-Console v1.2.
//  https://github.com/mustafayaya/Reactor-Developer-Console
//  Copyright (c) 2019 Mustafa Yaya

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
    
    // Forked from Reactor-Developer-Console.
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
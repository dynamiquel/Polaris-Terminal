using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Polaris.Debug.Terminal
{
    public abstract class Argument
    {
        // The type-specific error message.
        public abstract string Error { get; set; }

        public abstract bool IsValid(string value);

        public abstract object GetValue();

        new public abstract Type GetType();
    }

    public class Argument<T> : Argument
    {
        T value;

        public override string Error { get => $"{typeof(T)} Error!"; set => throw new NotImplementedException(); }

        public override bool IsValid(string value)
        {
            if (value == null)
                return false;

            if (typeof(T) == typeof(string))
            {
                this.value = (T)(object)value;
                return true;
            }
            else if (typeof(T) == typeof(Commands.Command))
            {
                if (Shell.Commands.ContainsKey(value))
                {
                    this.value = (T)(object)Shell.Commands[value];
                    return true;
                }
                return false;
            }
            else if (typeof(T) == typeof(int))
            {
                bool parsed = int.TryParse(value, out int result);
                Terminal.Log(value);
                if (parsed)
                {
                    this.value = (T)(object)result;
                    return true;
                }
                return false;
            }
            else if (typeof(T) == typeof(bool))
            {
                bool parsed = bool.TryParse(value, out bool result);
                Terminal.Log(value);
                if (parsed)
                {
                    this.value = (T)(object)result;
                    return true;
                }
                return false;
            }
            else if (typeof(T) == typeof(float))
            {
                bool parsed = float.TryParse(value, out float result);
                Terminal.Log(value);
                if (parsed)
                {
                    this.value = (T)(object)result;
                    return true;
                }
                return false;
            }
            else if (typeof(T) == typeof(Transform))
            {
                try
                {
                    var transform = GameObject.Find(value).transform;
                    this.value = (T)(object)transform;
                    return true;
                }
                catch
                {
                    Terminal.LogError($"Could not find transform: {value}");
                    return false;
                }
            }
            else if (typeof(T) == typeof(Rigidbody))
            {
                try
                {
                    var rb = GameObject.Find(value).transform.GetComponent<Rigidbody>();
                    this.value = (T)(object)rb;
                    return true;
                }
                catch
                {
                    Terminal.LogError($"Could not find rigidbody: {value}");
                    return false;
                }
            }
            else if (typeof(T) == typeof(Vector3))
            {
                bool valid = Utility.GetVector3FromString(value, out Vector3 result);

                if (valid)
                {
                    this.value = (T)(object)result;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public override object GetValue()
        {
            return value;
        }

        public override Type GetType()
        {
            return typeof(T);
        }
    }
}
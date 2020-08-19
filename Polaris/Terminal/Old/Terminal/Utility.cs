using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polaris.Debug.Terminal
{
    public static class Utility
    {
        public static TOutput ToTOutput(this object context, LogType logType = LogType.Log, Color colour = new Color())
        {
            var tString = new TOutput(context.ToString(), logType, colour);
            return tString;
        }

        public static TOutput NewTOutput(object context, LogType logType = LogType.Log, Color colour = new Color())
        {
            return context.ToTOutput(logType, colour);
        }

        public static string ToString(this TOutput tString)
        {
            return tString.Context;
        }

        public static bool GetVector3FromString(string data, out Vector3 result)
        {
            List<float> vectorCrenditicals = new List<float>();
            string crenditical = "";
            char[] chars = data.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != ',')
                {
                    crenditical += chars[i];
                }
                if (chars[i] == ',')
                {
                    float parseResult = 0;
                    if (float.TryParse(crenditical, out parseResult))
                    {
                        vectorCrenditicals.Add(parseResult);
                        crenditical = "";
                    }
                    else
                    {
                        result = Vector3.zero;
                        return false;
                    }
                }
                if (i == chars.Length - 1)
                {
                    float parseResult = 0;
                    if (float.TryParse(crenditical, out parseResult))
                    {

                        vectorCrenditicals.Add(parseResult);
                        crenditical = "";
                    }
                }
            }
            if (vectorCrenditicals.Count == 3)
            {
                result = new Vector3(vectorCrenditicals[0], vectorCrenditicals[1], vectorCrenditicals[2]);
                return true;
            }
            else
            {

                result = Vector3.zero;
                return false;
            }
        }
    }
}
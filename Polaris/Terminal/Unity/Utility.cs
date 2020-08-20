using System.Collections.Generic;
using Polaris.Core;
using UnityEngine;

namespace Polaris.Terminal.Unity
{
    // Unity-specific utility.
    public static class Utility
    {
        public static Color32 ToUnityColor32(this Colour colour)
        {
            return new Color32(colour.R, colour.G, colour.B, colour.A);
        }
        
        // Forked from Reactor-Developer-Console v1.2 (https://github.com/mustafayaya/Reactor-Developer-Console)
        public static bool GetVector3FromString(string data, out Vector3 result)
        {
            var vectorCredentials = new List<float>();
            var credential = "";
            var chars = data.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] != ',')
                {
                    credential += chars[i];
                }
                if (chars[i] == ',')
                {
                    if (float.TryParse(credential, out var parseResult))
                    {
                        vectorCredentials.Add(parseResult);
                        credential = "";
                    }
                    else
                    {
                        result = Vector3.zero;
                        return false;
                    }
                }
                if (i == chars.Length - 1)
                {
                    if (float.TryParse(credential, out var parseResult))
                    {
                        vectorCredentials.Add(parseResult);
                        credential = "";
                    }
                }
            }
            if (vectorCredentials.Count == 3)
            {
                result = new Vector3(vectorCredentials[0], vectorCredentials[1], vectorCredentials[2]);
                return true;
            }
            
            result = Vector3.zero;
            return false;
        }

        // Forked from Reactor-Developer-Console v1.2 (https://github.com/mustafayaya/Reactor-Developer-Console)
        public static bool GetQuaternionFromString(string data, out Quaternion result)
        {
            var vectorCredentials = new List<float>();
            var credential = "";
            var chars = data.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] != ',')
                {
                    credential += chars[i];
                }
                if (chars[i] == ',')
                {
                    if (float.TryParse(credential, out var parseResult))
                    {
                        vectorCredentials.Add(parseResult);
                        credential = "";
                    }
                    else
                    {
                        result = Quaternion.identity;
                        return false;
                    }
                }
                if (i == chars.Length - 1)
                {
                    if (float.TryParse(credential, out var parseResult))
                    {
                        vectorCredentials.Add(parseResult);
                        credential = "";
                    }
                }
            }
            if (vectorCredentials.Count == 4)
            {
                result = new Quaternion(vectorCredentials[0], vectorCredentials[1], vectorCredentials[2], vectorCredentials[2]);
                return true;
            }

            result = Quaternion.identity;
            return false;
        }
    }
}
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
        
        // Forked from Reactor-Developer-Console.
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

        // Forked from Reactor-Developer-Console.
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
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

using System;
using System.Globalization;

namespace Polaris.Core
{
    /// <summary>
    /// Represents a RGBA (red, green, blue, alpha) colour.
    /// A mix between System.Drawing.Color and UnityEngine.Color32.
    /// Designed to keep Terminal as close to .NET as possible.
    /// </summary>
    public struct Colour
    {
        public byte R {get; set;}
        public byte G {get; set;}
        public byte B {get; set;}
        public byte A {get; set;}
        public uint Rgba => BitConverter.ToUInt32(GetBytes(), 0);

        public bool IsEmpty => Equals(default(Colour));

        public string HexRgba
        {
            get
            {
                var hexString = BitConverter.ToString(GetBytes())
                .Replace("-", string.Empty);
                
                return hexString;
            }
        }
        public string HexRgb
        {
            get
            {
                var hexString = BitConverter.ToString(GetBytes(false))
                .Replace("-", string.Empty);
                
                return hexString;
            }
        }

        public static Colour FromHexString(string hex)
        {
            if (hex.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) ||
                hex.StartsWith("&H", StringComparison.CurrentCultureIgnoreCase) ||
                hex.StartsWith("#", StringComparison.CurrentCultureIgnoreCase)) 
            {
                hex = hex.Substring(2);
            }

            var success = uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var rgba);
            
            return success ? new Colour(rgba) : new Colour();
        }

        public Colour(byte r, byte g, byte b, byte a = 0xFF)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Colour(uint rgba, bool includeAlpha = true)
        {
            var bytes = BitConverter.GetBytes(rgba);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            R = bytes[0];
            G = bytes[1];
            B = bytes[2];

            A = includeAlpha ? bytes[3] : (byte) 255;
        }

        public override string ToString()
        {
            return HexRgba;
        }

        private byte[] GetBytes(bool includeAlpha = true)
        {
            return includeAlpha ? new byte[] {R, G, B, A} : new byte[] {R, G, B};
        }

        #region DefinedColours

        public static Colour InputField => new Colour(0xFF, 0xFF, 0xFF, 0xFF);
        public static Colour User => new Colour(0xB9, 0xE2, 0xEB, 0xFF);
        public static Colour System => new Colour(0xE8, 0xE8, 0xE8, 0xFF);
        public static Colour Warning => new Colour(0xFF, 0xBC, 0x05, 0xFF);
        public static Colour Assertion => new Colour(0xFF, 0xBC, 0x05, 0xFF);
        public static Colour Error => new Colour(0xE8, 0x02, 0x02, 0xFF);
        
        // You can add your own defined colours here...

        #endregion
    }
}
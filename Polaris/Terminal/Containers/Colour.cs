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
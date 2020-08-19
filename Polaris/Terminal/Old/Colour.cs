using UnityEngine;

namespace Polaris.Debug
{
    // This class just allows the program to reference colours that can easily be modified below.
    public static class Colour
    {
        public static Color Error = new Color32(0xE8, 0x02, 0x02, 0xFF);
        public static Color Warning = new Color32(0xFF, 0xBC, 0x05, 0xFF);
        public static Color Default = new Color32(0xE8, 0xE8, 0xE8, 0xFF);
        public static Color User = new Color32(0xB9, 0xE2, 0xEB, 0xFF);
        public static Color InputField = Color.white;
    }
}

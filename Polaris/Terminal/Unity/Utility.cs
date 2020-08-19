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
    }
}
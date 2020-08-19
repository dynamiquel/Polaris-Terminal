using System;

namespace Polaris.Terminal
{
    [Flags]
    public enum LogType : byte
    {
        None = 0,
        All = User | System | Log | Warning | Assertion | Error,
        User = 1 << 0,
        System = 1 << 1,
        Log = 1 << 2,
        Warning = 1 << 3,
        Assertion = 1 << 4,
        Error = 1 << 5,
        ImJustHereCozUnityDumb = 1 << 6,
    }

    [Flags]
    public enum LogSource
    {
        None = 0,
        Unity = 1 << 0,
        Polaris = 1 << 1,
        Generic = 1 << 2,
        IO = 1 << 3,
        Config = 1 << 4,
        Networking = 1 << 5,
        UI = 1 << 6,
        Audio = 1 << 7,
        AI = 1 << 8,
        Editor = 1 << 9,
        VR = 1 << 10,
        Scene = 1 << 11,
        OS = 1 << 12,
        Input = 1 << 13,
        Commands = 1 << 14,
        // Add your own LogSources here.
    }

    public enum PermissionLevel
    {
        Normal = 0,
        // Ideal for commands that you want players to access, but at a cost, such as no achievements or custom games
        // only.
        // i.e. teleport, flight, spawning weapons.
        Cheats = 1,
        // Ideal for commands that only developers and modders should have access to.
        // i.e. changing lighting, time, and other engine-level operations.
        Developer = 2
    }
}
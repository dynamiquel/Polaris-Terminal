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
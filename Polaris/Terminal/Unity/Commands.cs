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

using UnityEngine;

namespace Polaris.Terminal.Unity
{
    public class Commands
    {
        [Command("sphere")]
        public class Sphere : Command
        {
            public override string Name { get; protected set; } = "Spawn Sphere";
            public override string Category { get; protected set; } = "Misc";
            public override string Description { get; protected set; } = "Spawns a sphere in the centre of the screen.";

            [CommandParameter("position relative to camera")]
            public Vector3 position = new Vector3(0, 0, 10);
            
            public override LogMessage Execute()
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.AddComponent<Rigidbody>();

                var transform = Camera.main.transform;
                var ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out var hit))
                    sphere.transform.position = hit.point;
                else
                    sphere.transform.position = Camera.main.transform.position + position;

                return new LogMessage($"Sphere created at: {sphere.transform.position.ToString()}");
            }
        }
    }
}
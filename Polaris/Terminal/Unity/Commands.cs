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
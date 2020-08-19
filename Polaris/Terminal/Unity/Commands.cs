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

            public override LogMessage Execute()
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.AddComponent<Rigidbody>();

                RaycastHit hit;
                var transform = Camera.main.transform;
                var ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out hit))
                    sphere.transform.position = hit.point;
                else
                    sphere.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5f;

                return new LogMessage($"Sphere created at: {sphere.transform.position.ToString()}");
            }
        }
    }
}
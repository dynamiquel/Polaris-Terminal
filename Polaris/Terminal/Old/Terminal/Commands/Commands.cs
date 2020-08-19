using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Polaris.Debug.Terminal.Commands
{
    public class Commands
    {
        public Commands()
        {
            // Register your commands here.
            new Quit().Register();
            new Manual().Register();
            new All().Register();
            new Beep().Register();
            new Move().Register();
            new Sphere().Register();
            new MaxFPS().Register();
            new LoadLevel().Register();
            new Echo().Register();
            new Ping().Register();
            new Timescale().Register();
            new FixedTimeStep().Register();
            new FixedTimeStepHz().Register();
            new Hourglass().Register();
            new DataPath().Register();
            new Flush().Register();
            new GPUMemory().Register();
            new Memory().Register();
            new PhysGravity().Register();
            new PhysBounceThreshold().Register();
            new PhysSleepThreshold().Register();
            new PhysSleepVelocity().Register();
            new PhysMaxAngular().Register();
            new PhysClothGravity().Register();
            new ReloadScene().Register();
            new MasterVolume().Register();
            new AddForce().Register();
            new SetMass().Register();
            new SetDrag().Register();
            new FreezeRotation().Register();
            new UseGravity().Register();
        }
    }

    class Quit : Command
    {
        public Quit()
        {
            Name = "Quit";
            Cmd = "quit";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Quits the application.";
            ArgumentInfo = "";
            Manual = Description;
        }

        public override TOutput Execute(string[] args)
        {
            Terminal.Log("Quit Requested");
            Application.Quit();

            return "Quitting...".ToTOutput();
        }
    }

    class Manual : Command
    {
        public Manual()
        {
            Name = "Help";
            Cmd = "man";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Shows what a command does and how to use it.";
            ArgumentInfo = "";
            Manual = "When a command is given as an argument, the information of the command is given.";
            RequiredArguments.Add(new Argument<Command>());
        }

        public override TOutput Execute(string[] args)
        {
            var sb = new StringBuilder();

            if (Shell.Commands.ContainsKey(args[0]))
            {
                Shell.Commands.TryGetValue(args[0], out Command cmd);

                var requiredArguments = Shell.GetRequiredArguments(cmd);

                var requiredArgumentsSB = new StringBuilder();
                foreach (var item in requiredArguments)
                    requiredArgumentsSB.Append($"({item}) ");

                sb.AppendLine($"Manual (help) for the '{cmd.Name}' command");
                sb.AppendLine($"\tCategory: {cmd.Category}");
                sb.AppendLine($"\tCommand: {cmd.Cmd}");
                sb.AppendLine($"\tRequired Arguments: {requiredArgumentsSB}");
                sb.AppendLine($"\tOptional Arguments: {cmd.ArgumentInfo}");
                sb.AppendLine($"\tPermission Level: {cmd.PermissionLevel.ToString()}");
                sb.AppendLine($"\tDescription: {cmd.Description}");
                sb.AppendLine($"\tManual: {cmd.Manual}");
            }
            else
                Terminal.LogError("Command name is not valid.");

            return sb.ToTOutput(LogType.Log);
        }
    }

    class All : Command
    {
        public All()
        {
            Name = "All Commands";
            Cmd = "all";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Lists all of the available commands you can use.";
            ArgumentInfo = "";
            Manual = Description;
        }

        public override TOutput Execute(string[] args)
        {
            var sb = new StringBuilder();
            
            foreach (var item in Shell.Commands)
            {
                if (item.Value.PermissionLevel <= Settings.CurrentPermissionLevel)
                    sb.AppendLine($"Command: {item.Value.Name} - {item.Value.Cmd} {item.Value.ArgumentInfo}");
            }

            return sb.ToTOutput();
        }
    }

    class Beep : Command
    {
        public Beep()
        {
            Name = "Beep";
            Cmd = "beep";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Plays a beep sound.";
            ArgumentInfo = "";
            Manual = Description;
        }

        public override TOutput Execute(string[] args)
        {
            System.Media.SystemSounds.Beep.Play();

            return "Beep!".ToTOutput();
        }
    }

    class Move : Command
    {
        public Move()
        {
            Name = "Move";
            Cmd = "move";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Moves an object to a given position.";
            ArgumentInfo = "";
            Manual = Description;
            RequiredArguments.AddRange(new List<Argument> { new Argument<Transform>(), new Argument<Vector3>()});
        }

        public override TOutput Execute(string[] args)
        {
            var transform = (Transform)RequiredArguments[0].GetValue();
            var position = (Vector3)RequiredArguments[1].GetValue();
            transform.position = position;

            return $"Moved object '{transform.name}' to {transform.position}.".ToTOutput();
        }
    }

    class Sphere : Command
    {
        public Sphere()
        {
            Name = "Spawn Sphere";
            Cmd = "sphere";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Spawns a sphere in the centre of the screen.";
            ArgumentInfo = "";
            Manual = Description;
        }

        public override TOutput Execute(string[] args)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.AddComponent<Rigidbody>();

            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); ;
            if (Physics.Raycast(ray, out hit))
                sphere.transform.position = hit.point;
            else
                sphere.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5f;

            return $"Sphere created at: {sphere.transform.position.ToString()}".ToTOutput();
        }
    }

    class MaxFPS : Command
    {
        public MaxFPS()
        {
            Name = "Max FPS";
            Cmd = "fps_max";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Limits the frame rate with the given integer.";
            ArgumentInfo = "";
            Manual = Description;
            RequiredArguments.Add(new Argument<int>());
        }

        public override TOutput Execute(string[] args)
        {
            int maxFPS = (int)RequiredArguments[0].GetValue();

            Application.targetFrameRate = maxFPS;

            return $"Frame rate limited to '{maxFPS}' frames per second.".ToTOutput();
        }
    }

    class LoadLevel : Command
    {
        public LoadLevel()
        {
            Name = "Load Level";
            Cmd = "load_level";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Developer;
            Description = "Load the level with the given ID.";
            ArgumentInfo = "";
            Manual = Description;
            RequiredArguments.Add(new Argument<int>());
        }

        public override TOutput Execute(string[] args)
        {
            int levelId = (int)RequiredArguments[0].GetValue();

            try
            {
                if (levelId < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelId);
                    return $"Loading level with ID: {levelId}".ToTOutput();
                }
                else
                    return $"Unable to load level '{levelId}' as it does not exist in the current build.".ToTOutput(LogType.Warning);
            }
            catch
            {
                return $"Unable to load level '{levelId}' as it does not exist.".ToTOutput(LogType.Warning);
            }
        }
    }

    class Echo : Command
    {
        public Echo()
        {
            Name = "Echo";
            Cmd = "echo";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Terminal repeats what you enter.";
            ArgumentInfo = "";
            Manual = "";
            RequiredArguments.Add(new Argument<string>());
        }

        public override TOutput Execute(string[] args)
        {
            var echo = (string)RequiredArguments[0].GetValue();
          
            return echo.ToTOutput(LogType.Log);
        }
    }

    class Ping : Command
    {
        public Ping()
        {
            Name = "Ping";
            Cmd = "ping";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Terminal repeats what you enter.";
            ArgumentInfo = "";
            Manual = "";
            RequiredArguments.Add(new Argument<string>());
        }

        public override TOutput Execute(string[] args)
        {
            var targetAddress = (string)RequiredArguments[0].GetValue();
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();

            // Wait 10 seconds for a reply.
            int timeout = 1000;

            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(targetAddress);
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions(64, true);
            System.Net.NetworkInformation.PingReply reply = null;
            try
            {
                reply = pingSender.Send(targetAddress, timeout, buffer, options);

            }
            catch (System.Exception ex)
            {
                return $"Transmit failed. {ex.Message}".ToTOutput(LogType.Error);

            }

            return $"Reply from {reply.Address}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms Status={reply.Status}.".ToTOutput();
        }
    }

    class Timescale : Command
    {
        public Timescale()
        {
            Name = "Timescale";
            Cmd = "timescale";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the timescale of the game.";
            ArgumentInfo = "(float)";
            Manual = "The timescale multiplier will be set to the given float.\nExample: 0.5 = half-speed; 2 = double-speed.";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            Time.timeScale = value;

            return $"Timescale set to {value}".ToTOutput();
        }
    }

    class FixedTimeStep : Command
    {
        public FixedTimeStep()
        {
            Name = "Fixed Time-step";
            Cmd = "time_step";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets how often FixedUpdate is executed in seconds.";
            ArgumentInfo = "(float)";
            Manual = "The fixed time-step will be set to the given float in seconds. Example: 0.0167 = FixedUpdate will be called every 16.7ms (60Hz).";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            Time.fixedDeltaTime = value;

            return $"Fixed time-step set to {value}s ({1 / value}Hz).".ToTOutput();
        }
    }

    class FixedTimeStepHz : Command
    {
        public FixedTimeStepHz()
        {
            Name = "Fixed Time-step in Hertz";
            Cmd = "time_step_hz";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets how often FixedUpdate is executed in hertz.";
            ArgumentInfo = "(float)";
            Manual = "The fixed time-step will be set to the given float in hertz. Example: 60 = FixedUpdate will be called 60 times per seconds (16.7ms).";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            var s = 1 / value;
            Time.fixedDeltaTime = s;

            return $"Fixed time-step set to {value}Hz ({s}s).".ToTOutput();
        }
    }

    class Hourglass : Command
    {
        public Hourglass()
        {
            Name = "Hourglass";
            Cmd = "hourglass";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Outputs the time the app has been running for since start-up.";
            ArgumentInfo = "";
            Manual = "";
        }

        public override TOutput Execute(string[] args)
        {
            return $"Engine has been running for {(int)Time.realtimeSinceStartup} seconds.".ToTOutput();
        }
    }

    class DataPath : Command
    {
        public DataPath()
        {
            Name = "Data Path";
            Cmd = "data_path";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Outputs the app's data path.";
            ArgumentInfo = "";
            Manual = "";
        }

        public override TOutput Execute(string[] args)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Engine's data path: {Application.dataPath}");
            sb.AppendLine($"Engine's persistent data path: {Application.persistentDataPath}");
            sb.AppendLine($"Engine's temporary cache path: {Application.temporaryCachePath}");
            sb.AppendLine($"Engine's streaming assets path: {Application.streamingAssetsPath}");

            return sb.ToTOutput();
        }
    }

    class Flush : Command
    {
        public Flush()
        {
            Name = "Flush";
            Cmd = "flush";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Clear cache memory.";
            ArgumentInfo = "";
            Manual = "";
        }

        public override TOutput Execute(string[] args)
        {
            var cacheCount = Caching.cacheCount;

            return $"Cleared {cacheCount} cache(s).".ToTOutput();
        }
    }

    class GPUMemory : Command
    {
        public GPUMemory()
        {
            Name = "GPU Memory Stats";
            Cmd = "gpu_memory_stats";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Outputs the amount of allocated memory for the GPU.";
            ArgumentInfo = "";
            Manual = "";
        }

        public override TOutput Execute(string[] args)
        {
            var bytes = UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver();

            return $"Allocated GPU Memory: {bytes / 1000000}MB.".ToTOutput();
        }
    }

    class Memory : Command
    {
        public Memory()
        {
            Name = "Memory Stats";
            Cmd = "memory_stats";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Outputs the allocated and reserved memory.";
            ArgumentInfo = "";
            Manual = "";
        }

        public override TOutput Execute(string[] args)
        {
            var bytes = new List<long>();

            // Mono-allocated
            bytes.Add(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong());
            // Mono-reserved
            bytes.Add(UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong());
            // Unity-allocated
            bytes.Add(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong());
            // Unity-reserved
            bytes.Add(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong());

            var sb = new StringBuilder();

            sb.AppendLine($"Mono allocated memory: {bytes[0] / 1000000}MB");
            sb.AppendLine($"Mono reserved memory: {bytes[1] / 1000000}MB");
            sb.AppendLine($"Unity allocated memory: {bytes[2] / 1000000}MB");
            sb.AppendLine($"Unity reserved memory: {bytes[3] / 1000000}MB.");

            return sb.ToTOutput();
        }
    }

    class PhysGravity : Command
    {
        public PhysGravity()
        {
            Name = "Physics Gravity";
            Cmd = "phys_gravity";
            Category = "Physics";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the gravity.";
            ArgumentInfo = "(Vector3)";
            Manual = "";
            RequiredArguments.Add(new Argument<Vector3>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (Vector3)RequiredArguments[0].GetValue();
            Physics.gravity = value;

            return $"Gravity set to {Physics.gravity.ToString()}.".ToTOutput();
        }
    }

    class PhysBounceThreshold : Command
    {
        public PhysBounceThreshold()
        {
            Name = "Physics Bounce Threshold";
            Cmd = "phys_bounce_threshold";
            Category = "Physics";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the bounce threshold.";
            ArgumentInfo = "(float)";
            Manual = "";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            Physics.bounceThreshold = value;

            return $"Bounce threshold set to {Physics.bounceThreshold}.".ToTOutput();
        }
    }

    class PhysSleepThreshold : Command
    {
        public PhysSleepThreshold()
        {
            Name = "Physics Sleep Threshold";
            Cmd = "phys_sleep_threshold";
            Category = "Physics";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the sleep threshold.";
            ArgumentInfo = "(float)";
            Manual = "";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            Physics.sleepThreshold = value;

            return $"Sleep threshold set to {Physics.sleepThreshold}.".ToTOutput();
        }
    }

    class PhysSleepVelocity : Command
    {
        public PhysSleepVelocity()
        {
            Name = "Physics Sleep Velocity";
            Cmd = "phys_sleep_velocity";
            Category = "Physics";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the sleep velocity.";
            ArgumentInfo = "";
            Manual = "";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            Physics.sleepVelocity = value;

            return $"Sleep velocity set to {Physics.sleepVelocity}.".ToTOutput();
        }
    }

    class PhysMaxAngular : Command
    {
        public PhysMaxAngular()
        {
            Name = "Physics Max Angular Speed";
            Cmd = "phys_max_angular";
            Category = "Physics";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the max angular speed.";
            ArgumentInfo = "";
            Manual = "";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            Physics.defaultMaxAngularSpeed = value;

            return $"Gravity set to {Physics.defaultMaxAngularSpeed}.".ToTOutput();
        }
    }

    class PhysClothGravity : Command
    {
        public PhysClothGravity()
        {
            Name = "Physics Cloth Gravity";
            Cmd = "phys_cloth_gravity";
            Category = "Physics";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the cloth gravity.";
            ArgumentInfo = "";
            Manual = "";
            RequiredArguments.Add(new Argument<Vector3>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (Vector3)RequiredArguments[0].GetValue();
            Physics.clothGravity = value;

            return $"Gravity set to {Physics.clothGravity.ToString()}.".ToTOutput();
        }
    }

    class ReloadScene : Command
    {
        public ReloadScene()
        {
            Name = "Reload Scene";
            Cmd = "reload_scene";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Reloads the current scene.";
            ArgumentInfo = "";
            Manual = "";
        }

        public override TOutput Execute(string[] args)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            return $"Reloading scene...".ToTOutput();
        }
    }

    class MasterVolume : Command
    {
        public MasterVolume()
        {
            Name = "Master Volume";
            Cmd = "master_volume";
            Category = "Misc";
            PermissionLevel = PermissionLevel.Normal;
            Description = "Sets the master volume.";
            ArgumentInfo = "(float)";
            Manual = "Sets the master volume to the given decimal (0 to 1.0).";
            RequiredArguments.Add(new Argument<float>());
        }

        public override TOutput Execute(string[] args)
        {
            var value = (float)RequiredArguments[0].GetValue();
            AudioListener.volume = value;

            return $"Master volume set to {AudioListener.volume}.".ToTOutput();
        }
    }

    class AddForce : Command
    {
        public AddForce()
        {
            Name = "Add Force";
            Cmd = "add_force";
            Category = "Rigidbody";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Adds force to a rigidbody.";
            ArgumentInfo = "(Rigidbody) (Vector3)";
            Manual = "For the given rigidbody, adds the Vector3 force.";
            RequiredArguments.AddRange(new List<Argument> { new Argument<Rigidbody>(), new Argument<Vector3>() });
        }

        public override TOutput Execute(string[] args)
        {
            var rb = (Rigidbody)RequiredArguments[0].GetValue();
            var force = (Vector3)RequiredArguments[1].GetValue();
            rb.AddForce(force);

            return $"Force of {force.ToString()} applied to object: {rb.name}.".ToTOutput();
        }
    }

    class SetMass : Command
    {
        public SetMass()
        {
            Name = "Set Mass";
            Cmd = "set_mass";
            Category = "Rigidbody";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the mass of the given rigidbody.";
            ArgumentInfo = "(Rigidbody) (float)";
            Manual = "For the given rigidbody, sets the mass to the given float.";
            RequiredArguments.AddRange(new List<Argument> { new Argument<Rigidbody>(), new Argument<float>() });
        }

        public override TOutput Execute(string[] args)
        {
            var rb = (Rigidbody)RequiredArguments[0].GetValue();
            var mass = (float)RequiredArguments[1].GetValue();
            rb.mass = mass;

            return $"Mass of {mass} applied to object: {rb.name}.".ToTOutput();
        }
    }

    class SetDrag : Command
    {
        public SetDrag()
        {
            Name = "Set Drag";
            Cmd = "set_drag";
            Category = "Rigidbody";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Sets the drag of the given rigidbody.";
            ArgumentInfo = "(Rigidbody) (float)";
            Manual = "For the given rigidbody, sets the drag to the given float.";
            RequiredArguments.AddRange(new List<Argument> { new Argument<Rigidbody>(), new Argument<float>() });
        }

        public override TOutput Execute(string[] args)
        {
            var rb = (Rigidbody)RequiredArguments[0].GetValue();
            var drag = (float)RequiredArguments[1].GetValue();
            rb.mass = drag;

            return $"Drag of {drag} applied to object: {rb.name}.".ToTOutput();
        }
    }

    class FreezeRotation : Command
    {
        public FreezeRotation()
        {
            Name = "Freeze Rotation";
            Cmd = "freeze_rotation";
            Category = "Rigidbody";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Freezes the rotation of the given rigidbody.";
            ArgumentInfo = "(Rigidbody) (bool)";
            Manual = "For the given rigidbody, freeze its rotation.";
            RequiredArguments.AddRange(new List<Argument> { new Argument<Rigidbody>(), new Argument<bool>() });
        }

        public override TOutput Execute(string[] args)
        {
            var rb = (Rigidbody)RequiredArguments[0].GetValue();
            var value = (bool)RequiredArguments[1].GetValue();
            rb.freezeRotation = value;

            return $"Rotation of object '{rb.name}' frozen? {value}.".ToTOutput();
        }
    }

    class UseGravity : Command
    {
        public UseGravity()
        {
            Name = "Use Gravity";
            Cmd = "use_gravity";
            Category = "Rigidbody";
            PermissionLevel = PermissionLevel.Cheats;
            Description = "Freezes the position of the given rigidbody.";
            ArgumentInfo = "(Rigidbody) (bool)";
            Manual = "For the given rigidbody, freeze its rotation.";
            RequiredArguments.AddRange(new List<Argument> { new Argument<Rigidbody>(), new Argument<bool>() });
        }

        public override TOutput Execute(string[] args)
        {
            var rb = (Rigidbody)RequiredArguments[0].GetValue();
            var value = (bool)RequiredArguments[1].GetValue();
            rb.useGravity = value;

            return $"Does object '{rb.name}' have gravity? {value}.".ToTOutput();
        }
    }
}

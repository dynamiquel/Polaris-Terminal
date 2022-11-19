# Polaris Terminal
Terminal is a subset of Polaris that allows Unity developers to implement a featured developer console into their game.

For some reason, Unity doesn't come with a built-in developer console that allows you to execute commands or even easily see your logs in-game. This system is designed to fill that gap (and a bit more).

<img src="https://user-images.githubusercontent.com/50085636/202845425-8884e73d-3b2a-46b4-8f3a-e8eddbbf9ab3.png">

## Features
### Commands
- Commands
- Commands can be executed through scripts
- Permission Levels for Commands - i.e. Normal, Cheats, Developer
- Command Parameters
- Default values for Command Parameters
- Persistent input history
- Input prediction and hints (only really works with 16:9 aspect ratios)

### Logging
- Log messages with timestamps
- Logs Unity messages
- Choose a log type for a message - i.e. Log, Warning, Error
- Choose a log source (category) for a message - i.e. Generic, UI, Networking
- Choose a colour for a message - i.e. #RRGGBBAA
- Outputs messages to a text file
- Compresses old log text files
- In-game GUI
- Log mode (far less overhead, no filtering), recommended to use in release builds

<img src="https://user-images.githubusercontent.com/50085636/202845483-820d7260-d8fa-4402-84ac-4090d974db05.png" height="40%" width="40%" >

### Extensibility
- Easily add your own commands
- Easily create your own UI

### Desired features
- Debug mode (filtering, similar to Unity's Console), recommended to use for debugging.

---

## Design
Terminal is split into two main parts, the Terminal Controller (just called Terminal in the code because who can be bothered writing that out everytime) and the Shell.

### Shell
The Shell is where commands are stored, sent and processed. Developers don't need to know too much about the Shell as you shouldn't need to interact with it, unless you want to make changes to how commands are processed. 

The main method in Shell is `Shell.Execute(QueryInfo)`. This method takes in a **QueryInfo**, which is essentially a Command and CommandParameters, and attempts to find and execute a Command with the matching information.

### Terminal Controller
The Terminal Controller is what you'll be interacting with the most. It's basically responsible for receiving log messages and redirecting them to where they need to go, such as GUIs and text files. The Terminal Controller has an event `Terminal.OnLog(LogMessage)` which is invoked everytime a message has been recieved.

The Termimal Controller also has its own **Execute** method, `Execute(string)`. This method creates a QueryInfo out of the string, passes it to the Shell and logs any return message from the Command.

### Front-end
The back-end is the important part of Terminal. The front-end solutions provided by Terminal should be considered as extras and is not required to use Terminal. You can very easily create your own UI which uses Terminal as its backend.

---

## How to use
### Adding the Terminal GUI
A Terminal GUI is included in the package. However, you do not need to use this GUI as you can make your own GUI. All you need to do is subscribe to the `Terminal.OnLog(LogMessage)` event to receive the messages. Whatever you do with those messages are up to you.

#### CommonTerminal
CommonTerminal is an abstract Terminal UI that is used as a base. It can't do any logging on its own.

CommonTerminal has many options that the developers and users can use. This includes:
- Changing the name of Terminal
- Including timestamps
- Including the LogType
- Including the LogSource
- Don't destroy on scene change
- Which LogTypes to output
- Which LogSources to output
- Maximum number of characters
- Size (Fullscreen, Halfscreen, Topbar)
- Opacity
- Enable cursor
- Pinning the Terminal (visible but you can't interact with it)
- Pin size
- Pin opacity
- Clearing the current log messages

These options can currently only be edited through the Inspector (for default values) or through the *Terminal/Settings.yaml* file.

#### LogTerminal
The LogTerminal is built on top of CommonTerminal and is a traditional type of developer console, where it simply outputs text; nothing else. The advantage of this approach is that is uses very little memory compared to a **filtered** approach, like the one built in to Unity. This is recommended for release builds.

You will find a prefab for LogTerminal in the `Plugins/Polaris/Terminal/Prefabs/` folder. Just put it in your scene as a root object.

#### FilteredTerminal (WIP)
The FilteredTerminal is built on top of CommonTerminal and has options to filter out LogTypes, LogSources and a search bar. It is very similar to the Unity Console. However, it uses a lot more memory than LogTerminal. For this reason, it is recommended for only debugging.

#### CombinedTerminal (WIP)
The CombinedTerminal will simply be a LogTerminal and FilteredTerminal which can be switched at will.

### Logging
The best way to log messages is to use `Terminal.Log(LogMessage message)`. This will give you the most customisation, but also takes longer to write. But don't worry, some shortcuts are included.

1. `Terminal.Log(object)` (equivalent of [Debug.Log(object)](https://docs.unity3d.com/ScriptReference/Debug.Log.html))
2. `Terminal.LogWarning(object)` (equivalent of [Debug.LogWarning(object)](https://docs.unity3d.com/ScriptReference/Debug.LogWarning.html))
3. `Terminal.LogAssertion(object)` (equivalent of [Debug.LogAssertion(object)](https://docs.unity3d.com/ScriptReference/Debug.LogAssertion.html))
4. `Terminal.LogError(object)` (equivalent of [Debug.LogError(object)](https://docs.unity3d.com/ScriptReference/Debug.LogError.html))

#### Colour
If you want to quickly add some colour to your message to make it stand out more in the in-game UIs, you can use the alternative invoke: `Terminal.Log(object, Colour)`. This also works with LogWarning and LogError.

Don't worry too much about the **Colour** struct. It's simply a hybrid between [System.Drawing.Color](https://docs.microsoft.com/en-us/dotnet/api/system.drawing.color?view=netcore-3.1) and [UnityEngine.Color32](https://docs.unity3d.com/ScriptReference/Color32.html).

#### Sources (categories)
If you're coming from Unreal Engine, you're probably used to using categories for your logs, right? Well you can also use: `Terminal.Log(LogSource, object)` to make it easier to see where your messages are coming from, or if you want to apply some filtering.

#### Low-level logging
If you want to know how to use the more customisable `Terminal.Log(LogMessage message)`, then this is how you would do it:
```cs
Terminal.Log(new LogMessage
{
    Content = "This is a black warning message from UI",
    Colour = new Colour(0, 0, 0, 255),
    LogType = LogType.Warning,
    LogSource = LogSource.UI
});
```
There's also a constructor version if you prefer constructors `Terminal.Log(new LogMessage(string, LogType, Colour, LogSource))`.

#### Unity Debug.Logs
Since Terminal also handles normal Unity Logs, you can just use the traditional Unity way to log messages, however, this will give you the least amount of customisation.

### Commands
#### Executing
There are two ways to execute commands. The main way, which is `Terminal.Execute(string input, bool logInput = false)` is used if you wish to execute commands through code, or implementing your own GUI. The other way is through the supplied CommonTerminal. Simply open a CommonTerminal and enter the command and its arguments.

The anatomy of a command is `commandName argument1 argument2 etc`. For example, `help sphere` will give you information about the sphere command.
If your argument contains spaces, you can use ( and ) to group all the characters into a single argument. For example, `sphere (0, 0, 5)` will spawn a sphere at 0, 0, 5.

#### Creating
You may want to create your own commands for your particular games (otherwise what's the point, right?). Creating commands is very simple and is very similar to [Reactor-Developer-Console](https://github.com/mustafayaya/Reactor-Developer-Console).

Inherit from the Polaris.Terminal.Command class and include override the `Execute()` method like so:
```cs
public class HelloWorld : Command
{
    public override LogMessage Execute()
    {
        return new LogMessage("Hello, World!);
    }
}
```

Great, now you have a command. But in order for the command to be recognised, you need to add the Command attribute, which looks like this: `[Command(string commandId)]`. The commandId is the unique identifier of the command and is what will be used to execute it. Now our command should look like this:
```cs
[Command("hello")]
public class HelloWorld : Command
{
    public override LogMessage Execute()
    {
        return new LogMessage("Hello, World!);
    }
}
```

Now when we run the **hello** command using a Terminal GUI or `Terminal.Execute("hello")`, **Hello, World!** will be logged.

Command also contains some other overrides if you wish to give it more detail. These include:
```cs
string Name;
string Category;
string Description;
string Manual;
PermissionLevel PermissionLevel;
```

If you want to add parameters to your commands, then add a public member variable to your Command of the type you want your parameter and add the `[CommandParameter(string description)]` attribute to it. For example, if we wanted our HelloWorld command to print "Hello, " + a string parameter, we can do this:
```cs
[Command("hello")]
public class HelloWorld : Command
{
    [CommandParameter("this is just a description")] 
    public string value;
    
    public override LogMessage Execute()
    {
        return new LogMessage($"Hello, {value}!);
    }
}
```

Then all we do to execute it is `Terminal.Execute("hello Mars")`, which will log **Hello, Mars!**

You can also set a default value for each Command Parameter.
```cs
[Command("hello")]
public class HelloWorld : Command
{
    [CommandParameter("this is just a description")] 
    public string value = "Saturn";
    
    public override LogMessage Execute()
    {
        return new LogMessage($"Hello, {value}!);
    }
}
```
Now if no parameter was given, the value "Saturn" will be used, which will log **Hello, Saturn!**. Note: Currently, Terminal doesn't use default values when no parameter is given. Instead if you wish to use the default value, the `&` string must be used. E.g. `Terminal.Execute("hello &")` will log **Hello, Saturn!**.

---

## Options
Terminal comes with loads of options for developers and even users to choose.

### Terminal Controller options
These options are stored in the Polaris.Terminal.Settings class and by default are only accessible to developers. However, since they're public, developers can implement their own way for users to change them.

- Use UTC time (default: disabled)
- Show stack trace (default: enabled)
- Output to file (default: enabled)
- Output Unity logs (default: enabled)
- Enable history (default: enabled)
- History max size (default: 1MB)
- Permission level (default: PermissionLevel.Normal)

### Terminal GUI options
Read CommonTerminal to see the options for the included Terminal GUIs.


## Performance
**Note: These tests do not soley measure Terminal's performance. They include a snapshot of a Standalone Windows IL2CPP build of a URP game which only contains Terminal.**

Terminal is very performant. Even with messages being sent every single frame to a LogTerminal and a text file, CPU usage stays under 2% (Ryzen 5 2600) with very little memory usage (70MB).

However, this is not the case when `LogTerminal.outputText` (a TextMeshProUGUI component on the included LogTerminal) is being rendered. The CPU usage becomes ~13% and memory usage becomes 300MB. If this performance cost is too much for you but you still wish to use other parts of Terminal, you can create your own GUI for Terminal.

## Dependencies
- Unity 2018.4 or later
- [Polaris IO](https://github.com/dynamiquel/Polaris-IO) 2009 or later

## Adding to your Unity project
<ol>
  <li>Go to the <a href="https://github.com/dynamiquel/Polaris-Terminal/releases/latest">latest release</a> and download the desired version.</li>
  <li>Extract the <i><b>Polaris</b></i> folder to your project's <i><b>Assets > Plugins</b></i> folder.</li>
</ol>
  
### Credits
The Shell is influenced by <a href="https://github.com/mustafayaya/Reactor-Developer-Console">Reactor Developer Console</a>. Credits are given where due.

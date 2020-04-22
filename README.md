# Polaris Terminal
Polaris is a Game SDK for Unity that provides almost essential features. These features make sure your development time stays in what makes your game special.

## Introduction
Terminal is a subset of Polaris that allows Unity developers to implement a featured developer console into their game.

For some reason, Unity doesn't come with a built-in developer console that allows you to execute commands or even easily see your logs in-game. This system is designed to fill that gap (and a bit more).

## Notable features
- Commands
- Parameters
- Log messages (with colour)
- In-game UI
- Log mode (far less overhead, no filtering), recommended to use in release builds.
- Debug mode (filtering, similar to Unity's Console), recommended to use for debugging.
- Outputs to file
- Compresses old log files
- Persistent input history
- Input prediction and hints
- Permission Levels (Normal, Cheats, Developer)
- Easily add your own commands
- Easily create your own Terminal UI using ITerminal
- Commands can be executed through scripts

## Prerequisites
<ul>
  <li>Unity 2018.4 or later</li>
</ul>

## Adding to your Unity project
<ol>
  <li>Go to the <a href="https://github.com/dynamiquel/Polaris-Terminal/releases/latest">latest release</a> and download the desired version.</li>
  <li>Extract the <i><b>Polaris</b></i> folder to your project's <i><b>Assets > Plugins</b></i> folder.</li>
</ol>
  
### Credits
Influenced by <a href="https://github.com/mustafayaya/Reactor-Developer-Console">Reactor Developer Console</a>. Credits are given where due.

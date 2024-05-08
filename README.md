This application was created using Godot 4.0 (.NET Version). Please use this specific version to avoid breaking changes or unexpected behavior.

OPS Pro is a simulator application for the One Piece TCG card game. The application is currently under development, but it's already possible to use it to see how it works.
I started it to learn and try to understand how a turn-based card game works.

Support or contact me :
<a href="https://discord.gg/2Cr6UdskdQ"><img src="https://discordapp.com/api/guilds/1237756823474536458/widget.png?style=banner2" alt="Discord server"></a>

---

Jump to:
* [Dependencies](#dependencies)
* [Installation](#install)
* [Features](#features)
* [Screenshots](#screenshots)
* [Todo & Ideas](#todo)
* [Known bugs](#known-bugs)
* [License](#license)

# <a name=“dependencies”></a>Dependencies
* Visual Studio 2022
* .NET 6
* [Godot 4.0](https://github.com/godotengine/godot-builds/releases/download/4.0-stable/Godot_v4.0-stable_mono_win64.zip) (.NET Version)
* [OPS Pro Server](https://github.com/Kakumi/OPS-PRO-Server) running on a server or your machine

# <a name=“install”></a>Install
No installation file is available at the moment, as the application is still under development and unusable without a server.

In the case of development, here's how to proceed.
* Download OPS Pro Server and open the solution.
* In `appsettings.Development.json` or `appsettings.Production.json` modify the line: `“CardsPath”: “path\to\cards.json”` to target the JSON card data file. **This file is generated via an external tool that will be made available later. In the meantime, you'll find the file on the OPS Pro Server.
* Launch OPS Pro Server (Profile: `Run Dev` or `Run Prod`)
	* Dev is used to automate certain actions without depending on other players.
* Download OPS Pro and open the solution.
* In OPS Pro `Launch Profile` edit `Executable` and enter your path to Godot, e.g.: `C:\Path\ToGodot_v4.0-stable_mono_win64\Godot_v4.0-stable_mono_win64.exe`.
* In `Dependencies` -> `Assemblies` fix to missing or invalid DLL. You should target `OPSProServer.Contracts.dll` available after building OPS Pro Server, goto `OPSProServer\bin\Debug\net7.0\OPSProServer.Contracts.dll`.
* Launch OPS Pro (Profile: `Run`)

# <a name=“features”></a>Features
* Settings
	* Online Username
	* APP Background
	* APP Music
	* APP Theme
	* Languages
* Download cards image automatically
* Deck creator
* Card creator (not finished but working)
* Playing online

# <a name=“screenshots”></a>Screenshots
## Custom Card Creator
![](images/card_creator.png?raw=true)

## Deck Creator
![](images/deck_generator.png?raw=true)

## Settings
![](images/settings.png?raw=true)

## Rooms
![](images/room_list.png?raw=true)

## Game
![](images/game.png?raw=true)

# <a name=“todo”></a>TODO
* Fix some bugs
* Animations
* Add realtime chat (using SignalR or similar)
* Missing translations
* Add rules selection (No rules, 2024 January rules, Custom rules, ...) (User can choose a rule while creating the room and also check for deck compatiliby with deck creator)

# <a name=“known-bugs”></a>Known Bugs
* Sometimes some fatal errors occurred (app does not crash)
* Rooms list is not updated after the game ends
* When a popup is visible, the app cannot be close

# <a name=“license”></a>Licence
OPS Pro can be used by anyone for any purpose allowed by the permissive MIT License. Be sure to check Godot Licence too.
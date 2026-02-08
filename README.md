# comboom.sucht Menu

A vMenu-style FiveM server menu using LemonUI with Nord theme, pma-voice/radio integration, team management, and racing features.

## Features

- **Player Options**: Godmode, invisibility, unlimited stamina, fast run/swim, super jump, no ragdoll, wanted level control
- **Vehicle Options**: Vehicle godmode, repair, wash, flip, delete, freeze, invisibility
- **Weather Options**: Dynamic weather toggle, blackout mode, weather type selection
- **Time Options**: Freeze time, set time of day with quick presets
- **Teams**: Join teams with automatic pma-voice radio frequency assignment
- **Voice Settings**: Voice proximity control, radio channel configuration (pma-voice integration)

## Requirements

- FiveM Server
- [pma-voice](https://github.com/AvarianKnight/pma-voice) resource installed

## Installation

### 1. Get Required DLLs

You need to obtain these DLLs and place them in the `lib/` folder:

- `CitizenFX.Core.dll` - From FiveM SDK or your FiveM client installation
- `CitizenFX.Core.Server.dll` - From FiveM SDK
- `LemonUI.FiveM.dll` - Build from [LemonUI repository](https://github.com/LemonUIbyLemon/LemonUI)
- `Newtonsoft.Json.dll` - From NuGet or any .NET project

**Finding FiveM DLLs:**

- Client DLLs: `%localappdata%\FiveM\FiveM.app\citizen\clr2\lib\mono\4.5\`
- Server DLLs: Your FXServer installation `citizen/clr2/lib/mono/4.5/`

### 2. Build the Project

```bash
cd fivem-cbps-menu
dotnet build -c Release
```

Or build in Visual Studio by opening `CBPSMenu.sln`.

### 3. Deploy to Server

Copy the following to your FiveM server resources folder:

```
fivem-cbps-menu/
├── Client/bin/Release/CBPSMenu.net.dll
├── Server/bin/Release/CBPSMenu.Server.net.dll
├── lib/LemonUI.FiveM.dll
├── lib/Newtonsoft.Json.dll
├── config.json
└── fxmanifest.lua
```

### 4. Configure Permissions

Add to your `server.cfg`:

```cfg
# Start the resource
ensure fivem-cbps-menu

# Example ACE permissions
add_ace group.admin cbps.Everything allow

# Or grant specific permissions:
add_ace group.moderator cbps.PlayerOptions.God allow
add_ace group.moderator cbps.VehicleOptions.Repair allow
add_ace group.user cbps.PlayerOptions.Menu allow
add_ace group.user cbps.Teams.JoinTeam allow
```

## Configuration

Edit `config.json` to customize:

```json
{
  "menuTitle": "CBPS Menu",
  "menuSubtitle": "Server Menu",
  "menuKey": "M",
  "theme": {
    "headerColor": { "r": 94, "g": 129, "b": 172, "a": 255 },
    "highlightColor": { "r": 136, "g": 192, "b": 208, "a": 255 }
  },
  "teams": {
    "Team A": { "frequency": 100, "color": "#BF616A" },
    "Team B": { "frequency": 200, "color": "#A3BE8C" }
  }
}
```

## Permissions Reference

| Permission                       | Description                    |
| -------------------------------- | ------------------------------ |
| `cbps.Everything`                | All permissions                |
| `cbps.PlayerOptions.Menu`        | Access to player options menu  |
| `cbps.PlayerOptions.God`         | Godmode                        |
| `cbps.VehicleOptions.Menu`       | Access to vehicle options menu |
| `cbps.VehicleOptions.Repair`     | Repair vehicle                 |
| `cbps.Teams.Menu`                | Access to teams menu           |
| `cbps.Teams.JoinTeam`            | Join a team                    |
| `cbps.VoiceChat.Menu`            | Access to voice settings       |
| `cbps.WeatherOptions.SetWeather` | Change weather                 |
| `cbps.TimeOptions.SetTime`       | Change time                    |

## License

MIT License - See LICENSE file

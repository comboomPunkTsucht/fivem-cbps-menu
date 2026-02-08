# Installation Guide

Follow these steps to install **comboom.sucht menu** on your FiveM server.

## 📥 Download

Get the latest release from [GitHub Releases](https://github.com/comboomPunkTsucht/fivem-cbps-menu/releases/latest).

## 🛠️ Installation

### 1. Extract to Resources

```
resources/
└── comboom.sucht-menu/
    ├── CBPSMenu.net.dll
    ├── CBPSMenu.Server.net.dll
    ├── LemonUI.FiveM.dll
    └── fxmanifest.lua
```

### 2. Add to server.cfg

```cfg
# Load configuration and permissions
exec @comboom.sucht-menu/config.cfg

# pma-voice for radio features
ensure pma-voice
ensure pma-radio

# comboom.sucht Menu
ensure comboom.sucht-menu
```

## ⚙️ Configuration

The menu comes with a `config.cfg` file that handles all permissions and settings.
You can modify this file directly or override matching ConVars in your `server.cfg`.

### Basic Settings (in config.cfg)

```cfg
setr cbps_menu_title "My Server Menu"
# ... see config.cfg for all options
```

## 🔑 Key Permissions

Permissions are defined in `config.cfg`. Here are the main categories:

| Permission              | Description                               |
| :---------------------- | :---------------------------------------- |
| `cbps.Everything`       | All permissions (Admin)                   |
| `cbps.OnlinePlayers`    | Online Players menu (Kick, Ban, Teleport) |
| `cbps.PlayerOptions`    | Player Options menu (Godmode, etc.)       |
| `cbps.VehicleOptions`   | Vehicle Options menu                      |
| `cbps.VehicleSpawner`   | Vehicle Spawner menu                      |
| `cbps.SavedVehicles`    | Saved Vehicles menu                       |
| `cbps.PersonalVehicle`  | Personal Vehicle menu                     |
| `cbps.PlayerAppearance` | Player Appearance menu                    |
| `cbps.WeaponOptions`    | Weapon Options menu                       |
| `cbps.WeaponLoadouts`   | Weapon Loadouts menu                      |
| `cbps.MiscSettings`     | Misc Settings menu                        |
| `cbps.Recording`        | Recording menu                            |
| `cbps.TimeOptions`      | Time Options menu                         |
| `cbps.WeatherOptions`   | Weather Options menu                      |
| `cbps.Teams`            | Teams menu                                |
| `cbps.VoiceChat`        | Voice Chat settings                       |
| `cbps.Staff`            | Staff access (if restricted)              |

## 🎮 Keybindings

| Key    | Action        |
| ------ | ------------- |
| **F1** | Open Menu     |
| **F2** | Toggle NoClip |

## 🔧 Building from Source

```bash
cd fivem-cbps-menu
dotnet build -c Release
```

Output files in `dist/` folder.

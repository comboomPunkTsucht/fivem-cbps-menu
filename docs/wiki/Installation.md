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
    ├── Newtonsoft.Json.dll
    ├── config.json
    └── fxmanifest.lua
```

### 2. Add to server.cfg

```cfg
# comboom.sucht Menu
ensure comboom.sucht-menu

# Optional: pma-voice for radio features
ensure pma-voice
```

### 3. Configure Permissions

```cfg
# Admin (Full Access)
add_ace group.admin cbps.Everything allow

# Moderator
add_ace group.moderator cbps.POMenu allow
add_ace group.moderator cbps.VOMenu allow
add_ace group.moderator cbps.OPMenu allow

# Basic User
add_ace group.user cbps.POMenu allow
add_ace group.user cbps.VSMenu allow
add_ace group.user cbps.TMMenu allow
```

## ⚙️ Configuration

Edit `config.json`:

```json
{
  "menuTitle": "comboom.sucht Menu",
  "menuSubtitle": "vMenu Clone",
  "menuKey": "F1",
  "theme": {
    "headerColor": { "r": 94, "g": 129, "b": 172, "a": 255 },
    "highlightColor": { "r": 136, "g": 192, "b": 208, "a": 255 },
    "backgroundColor": { "r": 46, "g": 52, "b": 64, "a": 200 }
  }
}
```

## 🔑 Key Permissions

| Permission        | Description            |
| ----------------- | ---------------------- |
| `cbps.Everything` | All permissions        |
| `cbps.POMenu`     | Player Options menu    |
| `cbps.VOMenu`     | Vehicle Options menu   |
| `cbps.VSMenu`     | Vehicle Spawner menu   |
| `cbps.WPMenu`     | Weapon Options menu    |
| `cbps.PAMenu`     | Player Appearance menu |
| `cbps.SVMenu`     | Saved Vehicles menu    |
| `cbps.PVMenu`     | Personal Vehicle menu  |
| `cbps.WLMenu`     | Weapon Loadouts menu   |
| `cbps.MSMenu`     | Misc Settings menu     |
| `cbps.RECMenu`    | Recording menu         |
| `cbps.OPMenu`     | Online Players menu    |
| `cbps.WOMenu`     | Weather Options menu   |
| `cbps.TOMenu`     | Time Options menu      |
| `cbps.TMMenu`     | Teams menu             |
| `cbps.VCMenu`     | Voice Settings menu    |
| `cbps.NoClip`     | NoClip permission      |
| `cbps.Staff`      | Staff-only features    |

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

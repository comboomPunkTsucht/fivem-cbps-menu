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

Configuration is handled via **ConVars** in your `server.cfg`.

### Basic Settings

```cfg
setr cbps_menu_title "My Server Menu"
setr cbps_menu_subtitle "Server Menu"
setr cbps_menu_key "M"
```

### Theme Colors

```cfg
# Format: "r,g,b,a"
setr cbps_header_color "94, 129, 172, 255"
setr cbps_highlight_color "136, 192, 208, 255"
setr cbps_background_color "46, 52, 64, 200"
setr cbps_text_color "255, 236, 239, 244"
```

### Teams Configuration

```cfg
# Format: "Name:Freq:ColorHex;Name:Freq:ColorHex"
setr cbps_teams "Police:101:#0000FF;EMS:102:#FF0000;Mechanic:103:#FFA500"
```

### Other Settings

```cfg
setr cbps_default_proximity "15.0"
setr cbps_enable_radio_default "true"
setr cbps_banner_dictionary "commonmenu"
setr cbps_banner_texture "interaction_bgd"
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

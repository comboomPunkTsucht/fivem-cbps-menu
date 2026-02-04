# Installation Guide

Follow these steps to install and configure **comboom.sucht menu** on your FiveM server.

## 📥 Prerequisites

Before installing, ensure your server has the following dependencies running:

1.  **[pma-voice](https://github.com/AvarianKnight/pma-voice)**
    _Required for voice proximity features._
2.  **[pma-radio](https://github.com/AvarianKnight/pma-radio)**
    _Required for radio frequency management._

## 🛠️ Installation Steps

### 1. Download Release

Download the latest `fivem-cbps-menu.zip` from the [Releases](https://github.com/your-repo/releases) page.

### 2. Extract Files

Extract the contents of the zip file into your server's `resources` directory.
Your folder structure should look like this:

```
resources/
└── fivem-cbps-menu/
    ├── CBPSMenu.net.dll        # Client Assembly
    ├── CBPSMenu.Server.net.dll # Server Assembly
    ├── LemonUI.FiveM.dll       # UI Library
    ├── config.lua              # Configuration
    └── fxmanifest.lua          # Resource Manifest
```

### 3. Configure Server

Open your `server.cfg` file and add the following lines **in this order**:

```cfg
# Dependencies
ensure pma-voice
ensure pma-radio

# Menu System
ensure fivem-cbps-menu
```

> [!IMPORTANT]
> `pma-radio` MUST be started for the Radio features in the Voice Menu to function correctly.

### 4. Permissions (ACE)

To grant administrative access (required for Online Players menu actions like Kick/Ban), add the following ACE permissions to your `server.cfg`:

```cfg
# Admin Group (Full Access)
add_ace group.admin cbpsMenu.Everything allow

# Moderator Group (Specific Access)
add_ace group.moderator cbpsMenu.OnlinePlayers.Teleport allow
add_ace group.moderator cbpsMenu.OnlinePlayers.Spectate allow
```

## ⚙️ Configuration

You can customize keybinds and other settings in `config.lua`.

```lua
Config.MenuKey = 'F1' -- Default key to open menu
Config.Voice.Enabled = true
Config.Race.Enabled = true
```

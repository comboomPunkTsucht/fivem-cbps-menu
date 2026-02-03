# CBPS Menu

A comprehensive FiveM menu resource with all VMenu functions, pma-voice/pma-radio control, team management, race features, **character creation**, **custom themes**, **configurable keybindings**, and **controller support**. Built with LemonUI for a native GTA-style interface with full customization and **persistent settings**.

## 🆕 New Features (v2.0)

### Character Creation & Management
- **Create Characters**: Randomize and customize your appearance
- **Save Unlimited Characters**: Save as many character presets as you want
- **Load Anytime**: Switch between saved characters instantly
- **Default Character**: Set a character to auto-load on spawn
- **Persistent Storage**: All characters saved client-side

### Configurable Keybindings
- **FiveM Native Integration**: Use FiveM's built-in keybinding system
- **Customizable**: Players can rebind keys in FiveM settings
- **Controller Support**: Full Xbox/PlayStation controller support
- **D-Pad Navigation**: Navigate menus with controller
- **Configurable Buttons**: All controller buttons customizable

### Custom Theme Creator
- **RGB Color Picker**: Create themes with custom colors
- **Unlimited Themes**: Create as many custom themes as you want
- **Persistent Storage**: Custom themes saved client-side
- **Live Preview**: See theme changes in real-time

### Race Persistence
- **Save Race Templates**: Save races between server restarts
- **Load Saved Races**: Instantly load pre-made races
- **JSON Storage**: Races saved to file for backup

### Comprehensive Settings System
- **Everything Saveable**: All player preferences automatically saved
- **Server Defaults**: Admin-configurable default settings
- **Auto-Sync**: Settings sync between sessions
- **Export/Import**: Backup and restore settings

## Features

### Player Options
- **Health & Armor**: Heal and restore armor
- **God Mode**: Toggle invincibility
- **Invisibility**: Become invisible to other players
- **Noclip**: Fly through objects and terrain
- **Super Jump**: Jump higher than normal
- **Fast Run**: Increased running speed
- **Teleport to Waypoint**: Instantly travel to your map waypoint
- **Clear Wanted Level**: Remove police wanted level
- **Suicide**: Kill your character

### Vehicle Options
- **Vehicle Spawner**: Spawn vehicles from multiple categories
  - Super Cars
  - Sports Cars
  - SUVs
  - Sedans
  - Motorcycles
  - Emergency Vehicles
- **Vehicle Management**:
  - Repair vehicle
  - Clean vehicle
  - Flip vehicle upright
  - Boost vehicle speed
  - Max upgrade (engine, brakes, transmission, etc.)
  - Toggle vehicle invincibility
  - Rainbow paint mode
  - Delete vehicle

### Weapon Options
- **Weapon Categories**:
  - Melee
  - Handguns
  - Submachine Guns
  - Shotguns
  - Assault Rifles
  - Sniper Rifles
  - Heavy Weapons
- **Weapon Features**:
  - Give all weapons
  - Remove all weapons
  - Infinite ammo
  - No reload

### Voice & Radio (pma-voice + pma-radio Integration)
- **Voice Control**:
  - Adjustable voice range (3m - 30m)
  - Quick range toggle with ALT key
  - Voice mute/unmute
- **Radio Control**:
  - Set radio frequency (1.0 - 999.9)
  - Turn radio on/off
  - Volume control

### Team Management
- **Team Features**:
  - Create teams (up to 8 members)
  - Join/leave teams
  - Invite nearby players
  - Kick team members
  - Team chat
  - Auto-assigned team colors
  - Team invitation system with Y/N acceptance

### Race Functions
- **Race Creation**:
  - Create custom races
  - Add up to 20 checkpoints
  - Clear checkpoints
  - Start race countdown
- **Race Participation**:
  - Join available races
  - Real-time checkpoint tracking
  - Race timer
  - Position tracking
  - Leaderboard

### World Options
- **Weather Control**: Change weather (EXTRASUNNY, CLEAR, CLOUDS, RAIN, THUNDER, SNOW, etc.)
- **Time Control**: Set time of day (0-23 hours)

### Theme Customization
Choose from 9 beautiful themes:
- **Blue** (Default)
- **Red**
- **Green**
- **Purple**
- **Orange**
- **Yellow**
- **Pink**
- **Dark**
- **Light**

Each theme customizes the menu banner, highlights, and text colors for a personalized experience.

## Installation

1. Download and install dependencies:
   - [LemonUI](https://github.com/LemonUIbyLemon/LemonUI)
   - [pma-voice](https://github.com/AvarianKnight/pma-voice)
   - [pma-radio](https://github.com/AvarianKnight/pma-radio)

2. Place `cbps-menu` in your FiveM server's resources folder

3. Add to your `server.cfg`:
```cfg
ensure lemonui
ensure pma-voice
ensure pma-radio
ensure cbps-menu
```

4. Restart your server

## Configuration

Edit `config.lua` to customize the menu:

### Menu Settings
```lua
Config.MenuKey = 'F1' -- Key to open the menu
Config.MenuTitle = 'CBPS Menu'
```

### Permission Settings
```lua
Config.AdminOnly = false -- Set to true to restrict menu to admins only
Config.Admins = {'license:xxx', 'steam:xxx'} -- List of admin identifiers
```

### Theme Settings
```lua
Config.DefaultTheme = 'blue' -- Default theme
Config.AllowThemeChange = true -- Allow players to change theme
```

### Voice Settings
```lua
Config.Voice = {
    Enabled = true,
    DefaultRange = 5.0,
    Ranges = {3.0, 5.0, 10.0, 15.0, 20.0, 30.0},
    ShowUI = true
}
```

### Radio Settings
```lua
Config.Radio = {
    Enabled = true,
    MaxFrequency = 999.9,
    MinFrequency = 1.0
}
```

### Team Settings
```lua
Config.Team = {
    Enabled = true,
    MaxTeamSize = 8
}
```

### Race Settings
```lua
Config.Race = {
    Enabled = true,
    MaxCheckpoints = 20,
    CheckpointRadius = 10.0,
    CountdownTime = 5
}
```

## Usage

### Opening the Menu
- Press **F1** (or configured key) to open/close the menu
- Navigate with **Arrow Keys** or **Mouse**
- Select items with **Enter** or **Click**
- Go back with **Backspace** or **ESC**

### Voice Range Quick Toggle
- Press **ALT** to cycle through voice ranges quickly

### Team Invitations
- When invited to a team:
  - Press **Y** to accept
  - Press **N** to decline

## Features Overview

### VMenu Compatibility
This menu includes all major VMenu features:
- Complete player control
- Vehicle spawning and modification
- Weapon management
- World settings

### Additional Features
- **pma-voice integration**: Full voice range control
- **pma-radio integration**: Complete radio frequency management
- **Team system**: Organize players into teams
- **Race system**: Create and run custom races
- **Themeable UI**: 9 different color themes

## Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Contact the development team

## Credits

- **LemonUI**: Native GTA-style menu framework
- **pma-voice**: Proximity voice chat
- **pma-radio**: Radio communication system

## License

See LICENSE file for details.

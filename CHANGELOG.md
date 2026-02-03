# Changelog

All notable changes to CBPS Menu will be documented in this file.

## [2.0.0] - 2026-02-03

### Added - Major Update

#### Character Creation & Management System
- **Character Creator**: Randomize and customize player appearance
- **Save Characters**: Save unlimited character presets with custom names
- **Load Characters**: Switch between saved characters instantly
- **Default Character**: Set a character to auto-load on player spawn
- **Delete Characters**: Remove unwanted character presets
- **Character Manager Menu**: New submenu with 6 character management options
- **Persistent Storage**: All characters saved to client KVP storage

#### Configurable Keybindings
- **FiveM Integration**: Uses FiveM's native RegisterKeyMapping system
- **Player Customizable**: Players can rebind keys in FiveM settings
- **Config.Keybinds**: Configure default keybindings in config.lua
- **Multiple Keybinds**: Menu open, voice range, noclip toggle
- **Commands**: /cbps_menu, /cbps_voice_range, /cbps_noclip

#### Controller Support
- **Full Xbox/PlayStation Support**: Complete controller button mapping
- **D-Pad Navigation**: Navigate menus with directional pad
- **Button Configuration**: All buttons configurable in config.lua
- **Controller Settings**: Navigate/Select/Back/OpenMenu controls
- **Auto-Detection**: Controller input automatically detected

#### Custom Theme Creator
- **RGB Color Picker**: Create themes with custom RGB colors (0-255)
- **Unlimited Themes**: Create as many custom themes as desired
- **Theme Management**: Manage and delete custom themes
- **Persistent Storage**: Custom themes saved to client KVP
- **Live Preview**: Changes apply immediately
- **Config.AllowCustomThemes**: Enable/disable custom theme creation

#### Race Persistence System
- **Save Race Templates**: Save races with checkpoints to JSON file
- **Load Race Templates**: Load saved races after server restart
- **Template Management**: View, load, and delete saved race templates
- **JSON Storage**: Races saved to cbps_races.json
- **Config.Race.SaveRaces**: Enable/disable race persistence
- **Metadata**: Track creator, creation date, checkpoint count

#### Comprehensive Settings Management
- **Auto-Save**: All player preferences automatically saved
- **11 Categories**: Theme, character, menu, voice, radio, player, vehicle, weapon, world, race, team, UI
- **Client Storage**: Settings saved to client KVP (cbps_player_settings)
- **Server Defaults**: Server-configurable default settings
- **Settings Sync**: Auto-sync between sessions
- **Export/Import**: Backup and restore settings
- **Auto-Save Timer**: Save every 60 seconds
- **On-Disconnect Save**: Save when player leaves

#### Server Default Configuration
- **server_defaults.json**: Server-side default settings file
- **Admin Commands**: Configure server defaults via commands
- **Auto-Sync**: Clients automatically receive server defaults
- **Override Protection**: Server settings can enforce certain values
- **Per-Category Defaults**: Set defaults for each settings category

### Commands Added

#### Player Commands
- `/cbps_menu` - Open/close the menu (alternative to keybind)
- `/cbps_voice_range` - Cycle through voice ranges
- `/cbps_noclip` - Toggle noclip mode (if enabled in config)
- `/cbps_settings_export` - Export all settings to console (F8)
- `/cbps_settings_reset` - Reset all settings to defaults
- `/cbps_settings_save` - Manually save settings

#### Admin Commands
- `/cbps_set_default <category> <key> <value>` - Set a server default setting
- `/cbps_reset_defaults` - Reset server defaults to file

### Files Added

#### Client Scripts
- `client/character.lua` - Character creation, save, load, delete system
- `client/settings.lua` - Comprehensive player settings management

#### Server Scripts
- `server/settings.lua` - Server default settings management

#### Documentation
- `NEW_FEATURES.md` - Comprehensive documentation in German and English

### Files Modified

#### Configuration
- `config.lua` - Added Keybinds, Controller, AllowCustomThemes sections
- `fxmanifest.lua` - Added new script files

#### Client Scripts
- `client/main.lua` - RegisterKeyMapping, controller support, custom theme storage
- `client/menu.lua` - Character Manager menu, theme creator, race templates

#### Server Scripts
- `server/race.lua` - Race persistence with save/load/delete functionality

#### Documentation
- `README.md` - Updated with v2.0 features and highlights

### Storage Systems

#### Client-Side (KVP)
- `cbps_player_settings` - All player preferences and settings
- `cbps_custom_themes` - Player-created custom themes
- `cbps_saved_characters` - Character appearance presets
- `cbps_current_theme` - Currently selected theme
- `cbps_default_character` - Default character for auto-load

#### Server-Side (JSON)
- `server_defaults.json` - Server-configurable default settings
- `cbps_races.json` - Saved race templates with checkpoints

### Technical Improvements
- FiveM native keybinding system integration
- Efficient KVP storage for client preferences
- JSON file persistence for server data
- Auto-save with throttling (60 second intervals)
- Deep merge algorithm for settings inheritance
- Controller button abstraction layer
- Resource-aware save on stop/disconnect

### Configuration Options Added
- `Config.Keybinds` - Keyboard and controller keybinding configuration
- `Config.Controller` - Controller button mappings and navigation
- `Config.AllowCustomThemes` - Enable/disable custom theme creation
- `Config.Race.SaveRaces` - Enable/disable race persistence
- `Config.Race.RaceSaveFile` - Filename for race template storage

---

## [1.0.0] - 2026-02-03

### Added - Initial Release

#### Core Features
- **LemonUI Integration**: Native GTA-style menu interface using LemonUI framework
- **Theme System**: 9 customizable color themes (Blue, Red, Green, Purple, Orange, Yellow, Pink, Dark, Light)
- **Configuration System**: Comprehensive config.lua with all customizable options

#### Player Options (VMenu Parity)
- Heal Player - Restore health to maximum
- Give Armor - Restore armor to 100
- God Mode - Toggle invincibility
- Invisibility - Toggle player visibility
- Noclip - Fly through terrain with WASD controls
- Super Jump - Enhanced jumping ability
- Fast Run - Increased running speed
- Teleport to Waypoint - Instant teleportation to map marker
- Clear Wanted Level - Remove police stars
- Suicide - Kill your character

#### Vehicle System (VMenu Parity)
- **Vehicle Spawner**: 150+ vehicles across 6 categories
  - Super Cars (24 vehicles)
  - Sports Cars (50+ vehicles)
  - SUVs (25+ vehicles)
  - Sedans (25+ vehicles)
  - Motorcycles (40+ vehicles)
  - Emergency Vehicles (15+ vehicles)
- **Vehicle Options**:
  - Repair Vehicle
  - Clean Vehicle
  - Flip Vehicle upright
  - Boost Vehicle speed
  - Max Upgrade (all performance mods)
  - Vehicle Invincibility toggle
  - Rainbow Paint Mode (animated colors)
  - Delete Vehicle

#### Weapon System (VMenu Parity)
- **Weapon Spawner**: 50+ weapons across 7 categories
  - Melee Weapons
  - Handguns
  - Submachine Guns
  - Shotguns
  - Assault Rifles
  - Sniper Rifles
  - Heavy Weapons
- **Weapon Options**:
  - Give Individual Weapons
  - Give All Weapons
  - Remove All Weapons
  - Infinite Ammo toggle
  - No Reload toggle

#### Voice & Radio Integration
- **pma-voice Integration**:
  - Adjustable voice ranges (3m - 30m)
  - 6 preset range options
  - Quick ALT key range cycling
  - Voice mute/unmute toggle
- **pma-radio Integration**:
  - Set radio frequency (1.0 - 999.9)
  - Turn radio on/off
  - Frequency validation
  - Full pma-radio compatibility

#### Team Management System
- Create Teams (up to 8 members by default)
- Leave Team
- Invite Nearby Players (within 10m)
- Kick Team Members
- Team Chat (dedicated channel)
- Team Invitation System (Y/N to accept/decline)
- Auto-assigned Team Colors (6 colors)
- Automatic cleanup on player disconnect
- Team notifications for all members

#### Race System
- **Race Creation**:
  - Create custom races
  - Add up to 20 checkpoints
  - Clear all checkpoints
  - Start race with countdown
- **Race Participation**:
  - Join available races
  - Leave race
  - Real-time checkpoint tracking
  - Race timer display
  - Position tracking and leaderboard
  - Visual checkpoint markers and blips
  - Automatic finish detection

#### World Options (VMenu Parity)
- **Weather Control**: 12 weather types
  - EXTRASUNNY, CLEAR, CLOUDS, OVERCAST
  - RAIN, THUNDER, CLEARING, NEUTRAL
  - SNOW, BLIZZARD, SNOWLIGHT, XMAS
- **Time Control**: Set time of day (0-23 hours)
- Synchronized across all players on server

#### Settings & Configuration
- Menu Key Configuration (default F1)
- Admin-only Mode (optional)
- Admin Identifier System
- Default Theme Selection
- Theme Change Permissions
- Voice Range Configuration
- Radio Frequency Limits
- Team Size Limits
- Race Checkpoint Limits
- Feature Enable/Disable Toggles

#### Documentation
- Comprehensive README.md with feature overview
- Detailed INSTALLATION.md with step-by-step guide
- Complete FEATURES.md listing all capabilities
- Inline code comments
- Configuration examples
- Troubleshooting guide

#### Technical Implementation
- Client-server architecture
- Event-driven design
- Modular code structure
- Export functions for integration
- Proper resource cleanup
- Optimized performance
- ~2,000 lines of Lua code
- 18 Lua files (8 client, 7 server, 3 shared)

### Dependencies
- LemonUI - Menu framework
- pma-voice - Voice chat system
- pma-radio - Radio communication system

---

## Version Format

Versions follow [Semantic Versioning](https://semver.org/):
- MAJOR version for incompatible API changes
- MINOR version for new functionality in a backwards compatible manner
- PATCH version for backwards compatible bug fixes

## Categories

- **Added**: New features
- **Changed**: Changes to existing functionality
- **Deprecated**: Soon-to-be removed features
- **Removed**: Now removed features
- **Fixed**: Bug fixes
- **Security**: Security fixes
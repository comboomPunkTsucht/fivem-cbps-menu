# Changelog

All notable changes to CBPS Menu will be documented in this file.

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

### Notes
This is the initial release of CBPS Menu, providing a complete sandbox menu system for FiveM servers. The menu includes all major VMenu features plus modern additions like voice/radio control, team management, and a race system. Built with LemonUI for a native GTA-style interface with full theme customization.

### Known Issues
None - this is a fresh implementation

### Future Considerations
- Additional vehicle categories
- More weapon options
- Enhanced team features
- Race leaderboards with persistence
- Custom keybinds
- Permission levels
- Integration with ESX/QBCore frameworks

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
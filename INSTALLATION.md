# CBPS Menu - Installation Guide

## Prerequisites

Before installing CBPS Menu, ensure you have the following dependencies installed on your FiveM server:

1. **LemonUI** - Native GTA-style menu framework
   - Download: https://github.com/LemonUIbyLemon/LemonUI
   - Latest version required

2. **pma-voice** - Proximity voice chat system
   - Download: https://github.com/AvarianKnight/pma-voice
   - Follow their installation instructions

3. **pma-radio** - Radio communication system
   - Download: https://github.com/AvarianKnight/pma-radio
   - Follow their installation instructions

## Installation Steps

### Step 1: Install Dependencies

1. Download and install LemonUI:
   ```
   - Place the lemonui resource in your resources folder
   - Ensure it's started before cbps-menu
   ```

2. Download and install pma-voice:
   ```
   - Place the pma-voice resource in your resources folder
   - Configure according to their documentation
   ```

3. Download and install pma-radio:
   ```
   - Place the pma-radio resource in your resources folder
   - Configure according to their documentation
   ```

### Step 2: Install CBPS Menu

1. Download or clone the cbps-menu repository

2. Place the `cbps-menu` folder in your server's resources directory:
   ```
   server-data/
   └── resources/
       ├── lemonui/
       ├── pma-voice/
       ├── pma-radio/
       └── cbps-menu/    <- Place here
   ```

### Step 3: Configure server.cfg

Add the following lines to your `server.cfg` in the correct order:

```cfg
# Voice and radio systems
ensure pma-voice
ensure pma-radio

# LemonUI framework
ensure lemonui

# CBPS Menu
ensure cbps-menu
```

**Important:** The order matters! Ensure dependencies are loaded before cbps-menu.

### Step 4: Configure the Menu

1. Open `cbps-menu/config.lua`

2. Customize the settings:

```lua
-- Change menu key (default: F1)
Config.MenuKey = 'F1'

-- Enable admin-only mode (optional)
Config.AdminOnly = false
Config.Admins = {
    'license:your_license_here',
    'steam:your_steam_id_here'
}

-- Choose default theme
Config.DefaultTheme = 'blue' -- Options: blue, red, green, purple, orange, yellow, pink, dark, light

-- Configure voice ranges
Config.Voice.Ranges = {3.0, 5.0, 10.0, 15.0, 20.0, 30.0}

-- Configure team size
Config.Team.MaxTeamSize = 8

-- Configure race settings
Config.Race.MaxCheckpoints = 20
Config.Race.CheckpointRadius = 10.0
```

### Step 5: Start the Server

1. Start or restart your FiveM server

2. Check the console for any errors:
   ```
   [CBPS Menu] Starting server...
   [CBPS Menu] Server started successfully!
   ```

3. Join your server and press F1 (or your configured key) to open the menu

## Troubleshooting

### Menu doesn't open
- Check that LemonUI is properly installed and started
- Verify the menu key in config.lua matches your keyboard
- Check F8 console for errors

### Voice/Radio features don't work
- Ensure pma-voice and pma-radio are properly installed
- Check that both resources are started before cbps-menu
- Verify pma-voice and pma-radio configurations

### "Not authorized" message
- Check Config.AdminOnly setting
- Verify your identifier is in Config.Admins list
- Use license: or steam: prefix correctly

### Vehicles don't spawn
- Ensure the vehicle model names are correct
- Check that the models exist in your server
- Verify you have appropriate permissions

### Themes don't change
- Ensure Config.AllowThemeChange = true
- Check that you're applying the theme from the Settings menu
- Try reopening the menu after theme change

## Testing

After installation, test the following features:

1. **Menu Access**: Press F1 to open the menu
2. **Player Options**: Try healing and armor
3. **Vehicle Spawner**: Spawn a vehicle from any category
4. **Voice Control**: Press ALT to cycle voice ranges
5. **Team System**: Create a team and invite someone
6. **Race System**: Create a race and add checkpoints
7. **Theme Change**: Go to Settings and change the theme

## Getting Help

If you encounter issues:

1. Check the console (F8 in-game) for errors
2. Check server console for error messages
3. Verify all dependencies are installed correctly
4. Review the configuration file for typos
5. Open an issue on GitHub with:
   - Server console logs
   - Client console logs (F8)
   - Steps to reproduce the issue
   - FiveM version and server artifacts

## Advanced Configuration

### Adding Custom Vehicle Categories

Edit `config.lua` and add to `Config.VehicleCategories`:

```lua
{
    name = 'Custom Category',
    vehicles = {'vehicle1', 'vehicle2', 'vehicle3'}
}
```

### Adding Custom Weapon Categories

Edit `config.lua` and add to `Config.WeaponCategories`:

```lua
{
    name = 'Custom Weapons',
    weapons = {'WEAPON_NAME1', 'WEAPON_NAME2'}
}
```

### Creating Custom Themes

Edit `config.lua` and add to `Config.Themes`:

```lua
customtheme = {
    name = 'My Custom Theme',
    banner = {r = 100, g = 150, b = 200, a = 255},
    highlight = {r = 100, g = 150, b = 200, a = 255},
    textColor = {r = 255, g = 255, b = 255, a = 255}
}
```

## Performance Considerations

- The menu is optimized for minimal performance impact
- Voice range cycling uses a keybind thread (negligible impact)
- Team and race systems only run when active
- All features can be disabled in config.lua if not needed

## Security

- Admin identifiers should be kept private
- Set Config.AdminOnly = true for production servers
- Review permissions before deploying
- Keep dependencies up to date

## Updates

To update CBPS Menu:

1. Backup your config.lua
2. Replace all files with new version
3. Restore your config.lua (merge any new options)
4. Restart the resource: `restart cbps-menu`

## License

See LICENSE file for usage terms and conditions.
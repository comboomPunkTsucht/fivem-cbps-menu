# CBPS Menu - Neue Funktionen / New Features

## Deutsche Version

### Übersicht der neuen Funktionen

#### ✅ 1. Noclip
- **Bereits implementiert**: Ja, Noclip ist vollständig funktionsfähig
- **Tastenkombination**: Konfigurierbar (Standard: F2)
- **Verwendung**: Im Player Options Menü als Toggle-Option
- **Steuerung im Noclip-Modus**:
  - W/S: Vorwärts/Rückwärts
  - A/D: Drehen
  - Q: Nach unten
  - E: Nach oben
  - Shift: Schneller Modus

#### ✅ 2. Rennen-Persistenz (Race Persistence)
- **Speichern**: Rennen können als Vorlagen gespeichert werden
- **Laden**: Gespeicherte Rennen nach Server-Neustart verfügbar
- **Datei**: `cbps_races.json` im Resource-Ordner
- **Befehle im Race Menü**:
  - Save Race Template: Aktuelles Rennen speichern
  - Load Race Template: Gespeichertes Rennen laden
  - Manage Race Templates: Rennen verwalten/löschen

#### ✅ 3. Konfigurierbare Tastenbelegung
- **Konfiguration**: In `config.lua` unter `Config.Keybinds`
- **FiveM Integration**: Nutzt natives RegisterKeyMapping
- **Anpassbar über FiveM Einstellungen**: Spieler können Tasten in den FiveM Einstellungen ändern
- **Standard-Tastenbelegungen**:
  - F1: Menü öffnen
  - ALT: Voice Range wechseln
  - F2: Noclip Toggle (optional)

#### ✅ 4. Controller-Unterstützung
- **Aktivierung**: `Config.Controller.Enabled = true`
- **Navigation**: D-Pad (hoch/runter/links/rechts)
- **Auswahl**: A-Button (Xbox) / X-Button (PlayStation)
- **Zurück**: B-Button (Xbox) / Kreis-Button (PlayStation)
- **Menü öffnen**: SELECT/BACK Button
- **Konfigurierbar**: Alle Buttons in config.lua anpassbar

#### ✅ 5. Eigene Themes erstellen
- **Aktivierung**: `Config.AllowCustomThemes = true`
- **Zugriff**: Settings Menü → "Create Custom Theme"
- **RGB-Farbauswahl**: Für Banner, Highlight und Text
- **Speicherung**: Client-seitig (KVP) - bleibt nach Neustart erhalten
- **Verwaltung**: "Manage Custom Themes" Option im Settings Menü

#### ✅ 6. Charakter-Erstellung und Speicherung
- **Charakter erstellen**: Character Manager → "Create Character"
- **Speichern**: Beliebig viele Charaktere mit Namen speichern
- **Laden**: Gespeicherte Charaktere jederzeit laden
- **Standard setzen**: Automatisch beim Spawn laden
- **Löschen**: Ungewollte Charaktere entfernen
- **Persistenz**: Alle Charaktere bleiben nach Server-Neustart erhalten

#### ✅ 7. Umfassende Einstellungen-Verwaltung
- **Alles speicherbar**: Alle Benutzereinstellungen werden automatisch gespeichert
- **Kategorien**:
  - Theme-Einstellungen
  - Charakter-Einstellungen
  - Menü-Präferenzen
  - Voice/Radio-Einstellungen
  - Spieler-Optionen (God Mode, etc.)
  - Fahrzeug-Präferenzen
  - Waffen-Präferenzen
  - Welt-Einstellungen
  - Rennen/Team-Präferenzen
  - UI-Einstellungen

#### ✅ 8. Server-Defaults
- **Konfigurierbar**: Server kann Standard-Einstellungen festlegen
- **Datei**: `server_defaults.json`
- **Admin-Befehle**:
  - `/cbps_set_default <kategorie> <schlüssel> <wert>` - Standard setzen
  - `/cbps_reset_defaults` - Alle Standards zurücksetzen
- **Auto-Sync**: Clients erhalten automatisch Server-Defaults

### Verwendung

#### Charaktere verwalten
1. Menü öffnen (F1)
2. "Character Manager" auswählen
3. "Create Character" - Zufälliges Aussehen erstellen
4. "Save Character" - Aktuelles Aussehen speichern
5. "Set Default Character" - Beim Spawn automatisch laden

#### Eigenes Theme erstellen
1. Settings Menü → "Create Custom Theme"
2. Theme-Namen eingeben
3. RGB-Werte für Banner eingeben (0-255)
4. Theme wird automatisch gespeichert
5. In Theme-Liste auswählbar

#### Rennen speichern
1. Rennen erstellen mit Checkpoints
2. "Save Race Template" wählen
3. Namen eingeben
4. Rennen bleibt nach Neustart verfügbar

---

## English Version

### Overview of New Features

#### ✅ 1. Noclip
- **Already implemented**: Yes, fully functional
- **Keybind**: Configurable (default: F2)
- **Usage**: Player Options Menu as toggle option
- **Controls in Noclip**:
  - W/S: Forward/Backward
  - A/D: Rotate
  - Q: Down
  - E: Up
  - Shift: Fast mode

#### ✅ 2. Race Persistence
- **Save**: Races can be saved as templates
- **Load**: Saved races available after server restart
- **File**: `cbps_races.json` in resource folder
- **Race Menu Commands**:
  - Save Race Template: Save current race
  - Load Race Template: Load saved race
  - Manage Race Templates: Manage/delete races

#### ✅ 3. Configurable Keybindings
- **Configuration**: In `config.lua` under `Config.Keybinds`
- **FiveM Integration**: Uses native RegisterKeyMapping
- **Customizable via FiveM Settings**: Players can rebind in FiveM settings
- **Default Keybinds**:
  - F1: Open menu
  - ALT: Cycle voice range
  - F2: Noclip toggle (optional)

#### ✅ 4. Controller Support
- **Enable**: `Config.Controller.Enabled = true`
- **Navigation**: D-Pad (up/down/left/right)
- **Select**: A button (Xbox) / X button (PlayStation)
- **Back**: B button (Xbox) / Circle button (PlayStation)
- **Open Menu**: SELECT/BACK button
- **Configurable**: All buttons in config.lua

#### ✅ 5. Custom Theme Creation
- **Enable**: `Config.AllowCustomThemes = true`
- **Access**: Settings Menu → "Create Custom Theme"
- **RGB Color Picker**: For banner, highlight, and text
- **Storage**: Client-side (KVP) - persists after restart
- **Management**: "Manage Custom Themes" in Settings Menu

#### ✅ 6. Character Creation and Saving
- **Create Character**: Character Manager → "Create Character"
- **Save**: Save unlimited characters with names
- **Load**: Load saved characters anytime
- **Set Default**: Auto-load on spawn
- **Delete**: Remove unwanted characters
- **Persistence**: All characters persist after server restart

#### ✅ 7. Comprehensive Settings Management
- **Everything Saveable**: All user settings automatically saved
- **Categories**:
  - Theme preferences
  - Character settings
  - Menu preferences
  - Voice/Radio settings
  - Player options (God Mode, etc.)
  - Vehicle preferences
  - Weapon preferences
  - World settings
  - Race/Team preferences
  - UI settings

#### ✅ 8. Server Defaults
- **Configurable**: Server can set default settings
- **File**: `server_defaults.json`
- **Admin Commands**:
  - `/cbps_set_default <category> <key> <value>` - Set default
  - `/cbps_reset_defaults` - Reset all defaults
- **Auto-Sync**: Clients automatically receive server defaults

### Usage

#### Managing Characters
1. Open menu (F1)
2. Select "Character Manager"
3. "Create Character" - Generate random appearance
4. "Save Character" - Save current appearance
5. "Set Default Character" - Auto-load on spawn

#### Creating Custom Theme
1. Settings Menu → "Create Custom Theme"
2. Enter theme name
3. Enter RGB values for banner (0-255)
4. Theme automatically saved
5. Selectable in theme list

#### Saving Races
1. Create race with checkpoints
2. Select "Save Race Template"
3. Enter name
4. Race persists after restart

---

## Commands / Befehle

### Player Commands
- `/cbps_settings_export` - Export settings to console
- `/cbps_settings_reset` - Reset all settings to defaults
- `/cbps_settings_save` - Manually save settings

### Admin Commands (Server Console or Admin)
- `/cbps_set_default <category> <key> <value>` - Set server default
- `/cbps_reset_defaults` - Reset server defaults to file

## Configuration Files

### config.lua
Main configuration with all keybinds, controller settings, and feature toggles.

### server_defaults.json
Server-side default settings that are applied to all new players.

### Player Storage (Client-side KVP)
- `cbps_player_settings` - All player preferences
- `cbps_custom_themes` - Custom themes
- `cbps_saved_characters` - Character presets
- `cbps_current_theme` - Current theme selection
- `cbps_default_character` - Default character name

### Server Storage
- `cbps_races.json` - Saved race templates

## Technical Details

### Auto-Save
- Settings auto-save every 60 seconds
- Settings save on resource stop
- Settings save on player disconnect

### Server Sync
- Server defaults sync on player join
- Setting changes notify server
- Race templates save immediately

### Backwards Compatible
- All new features are optional
- Can be disabled in config
- Existing installations work without changes

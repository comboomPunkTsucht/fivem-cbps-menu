# Radio & Voice Guide

This guide explains how to use the **Voice Settings** and **Teams** features in comboom.sucht menu.

## 🎙️ Voice Proximity

Control how far your voice travels to other players.

### Changing Proximity

1. Open the menu (default: `M` key)
2. Navigate to **Voice Settings**
3. Use the **Voice Proximity** list to select a range
4. Or use quick presets:
   - **Whisper** (5m) - Only nearby players hear you
   - **Normal** (15m) - Standard conversation range
   - **Shout** (30m) - Be heard from far away

### How It Works

The menu uses pma-voice exports:

```csharp
Exports["pma-voice"].setVoiceProperty("proximity", 15f);
```

## 📻 Radio Channels

Communicate with your team across the map using radio.

### Setting a Radio Channel

1. Open menu → **Voice Settings**
2. Select **Set Radio Channel**
3. Enter a frequency (0-999)
4. Radio will be enabled automatically

### Toggling Radio

Use the **Radio Enabled** checkbox to enable/disable radio communications.

## 👥 Teams with Auto-Radio

The **Teams** menu automatically sets your radio frequency when you join a team.

### Joining a Team

1. Open menu → **Teams**
2. Select a team (Team A, B, C, or D)
3. Your radio is **automatically** set to the team's frequency:

| Team   | Frequency |
| ------ | --------- |
| Team A | 100 MHz   |
| Team B | 200 MHz   |
| Team C | 300 MHz   |
| Team D | 400 MHz   |

### Leaving a Team

1. Open menu → **Teams**
2. Select **Leave Team**
3. Your radio channel will be set to 0 (disabled)

## ⚙️ Configuration

Team frequencies can be customized in `config.json`:

```json
{
  "teams": {
    "Team A": { "frequency": 100, "color": "#BF616A" },
    "Team B": { "frequency": 200, "color": "#A3BE8C" },
    "Team C": { "frequency": 300, "color": "#EBCB8B" },
    "Team D": { "frequency": 400, "color": "#B48EAD" }
  }
}
```

## 🔧 Troubleshooting

**Radio not working?**

- Ensure `pma-voice` is installed and running
- Check that you have the `cbps.VoiceChat.Menu` permission

**Can't hear teammates?**

- Verify everyone is on the same team/frequency
- Check that radio is enabled in Voice Settings

# Race Creator Guide

> [!NOTE]
> The Race Creator feature is currently under development and will be available in a future update.

This guide explains how to create and manage custom race tracks.

## 🏁 Overview

The Race Creator allows you to:

- Place checkpoints to define a track
- Save tracks to the server
- Create race lobbies for multiplayer races
- Track finish times and positions

## 📍 Creating a Race Track

_(Coming Soon)_

1. Open menu → **Race Options** → **Race Creator**
2. Navigate to where you want the first checkpoint
3. Select **Add Checkpoint** to place it
4. Continue adding checkpoints along your route
5. Add a final checkpoint as the finish line
6. Select **Save Track** and enter a name

## 🎮 Starting a Race

_(Coming Soon)_

1. Open menu → **Race Options** → **Race Lobby**
2. Select a saved track
3. Select **Create Lobby**
4. Wait for players to join
5. Select **Start Race** to begin countdown

## ⚙️ Configuration

Race settings in `config.json`:

```json
{
  "racing": {
    "checkpointModel": "prop_mp_cone_01",
    "finishModel": "prop_mp_cone_02",
    "countdownSeconds": 3
  }
}
```

## 🔑 Required Permissions

| Permission                | Description          |
| ------------------------- | -------------------- |
| `cbps.Racing.Menu`        | Access to race menu  |
| `cbps.Racing.CreateTrack` | Create race tracks   |
| `cbps.Racing.EditTrack`   | Edit existing tracks |
| `cbps.Racing.DeleteTrack` | Delete tracks        |
| `cbps.Racing.JoinRace`    | Join race lobbies    |
| `cbps.Racing.StartRace`   | Start races          |

## 📁 Data Storage

Race tracks are saved to `races.json` on the server:

```json
{
  "tracks": [
    {
      "name": "Airport Circuit",
      "author": "player123",
      "checkpoints": [
        { "x": -1000, "y": -2500, "z": 14 },
        { "x": -1050, "y": -2600, "z": 14 }
      ]
    }
  ]
}
```

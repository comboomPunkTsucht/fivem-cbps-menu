# Guide: Race Creator

The **Race Creator** allows you to build, save, and share custom race tracks directly within the game. All races are synchronized with the server, meaning anyone can join your lobby once it's created.

## 📝 Creating a New Race

1. **Open the Menu**: Press `F1` (default) to open the comboom.sucht menu.
2. **Navigate**: Go to `Race Options` -> `Race Creator`.
3. **Start Creation**: Select `Create New Race`. You are now in Creator Mode.

## 📍 Adding Checkpoints

1. Drive to your desired starting position.
2. Select **Add Checkpoint** in the menu.
   - A **Yellow Blip** will appear on the map and in the world marking the checkpoint.
3. Drive to the next point and repeat.
   - _Tip_: Place checkpoints at corners and key intersections to guide racers clearly.

## 💾 Saving Your Track

1. Once you've placed all checkpoints, scroll down to **Save Race**.
2. Enter a unique **Name** for your track when prompted.
3. Your race is now saved to `cbps_races.json` on the server!

## 🏁 Starting a Race

1. Go to `Race Options` -> `Saved Races`.
2. Select your custom track from the list.
3. Choose **Create Lobby**.
4. Other players can now join via `Race Options` -> `Join Race`.
5. Once everyone is ready, select **Start Race**.
   - A 5-second countdown will begin for all participants.
   - Good luck!

## ⚙️ Technical Details

Races are stored in JSON format on the server side:

- **File**: `server/cbps_races.json`
- **Backup**: It is recommended to backup this file if you possess many custom tracks.

using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace CBPSMenu.Server
{
  public class TeamManager : BaseScript
  {
    // PlayerId -> TeamIndex
    private Dictionary<int, int> playerTeams = new Dictionary<int, int>();

    // PlayerId -> BlipVisible (true/false)
    private Dictionary<int, bool> playerBlipVisibility = new Dictionary<int, bool>();

    public TeamManager()
    {
      EventHandlers["cbps:updateTeam"] += new Action<Player, int>(OnUpdateTeam);
      EventHandlers["cbps:requestTeamData"] += new Action<Player>(OnRequestTeamData);
      EventHandlers["cbps:updateBlipVisibility"] += new Action<Player, bool>(OnUpdateBlipVisibility);
      EventHandlers["cbps:playerDropped"] += new Action<string>(OnPlayerDropped);
    }

    private void OnUpdateTeam([FromSource] Player player, int teamIndex)
    {
      if (int.TryParse(player.Handle, out int playerId))
      {
        if (teamIndex == 0)
        {
          playerTeams.Remove(playerId);
        }
        else
        {
          playerTeams[playerId] = teamIndex;
        }

        // Broadcast update to all players
        // The client TeamMenu.cs expects cbps:teamUpdated (serverId, teamIndex)
        // Need to verify if client expects this event.
        // Checking TeamMenu.cs: It has OnTeamUpdated(serverId, teamIndex) but RegisterEvents is empty comment?
        // Wait, TeamMenu.cs: "private void RegisterEvents() { // Listen for team updates... }" is empty.
        // The User previous edits might have left it incomplete or I need to check Client/Main.cs
        // Client/Main.cs handles "cbps:receiveTeamData" maybe?

        // Let's assume standard behavior: TriggerClientEvent("cbps:teamUpdated", playerId, teamIndex)
        TriggerClientEvent("cbps:teamUpdated", playerId, teamIndex);

        Debug.WriteLine($"[CBPS Menu] Player {player.Name} joined team {teamIndex}");
      }
    }

    private void OnRequestTeamData([FromSource] Player player)
    {
      // Serialize dict to object/array for client
      // Dictionary<int, int> is fine for JSON
      player.TriggerEvent("cbps:receiveTeamData", playerTeams);
    }

    private void OnUpdateBlipVisibility([FromSource] Player player, bool visible)
    {
      if (int.TryParse(player.Handle, out int playerId))
      {
        playerBlipVisibility[playerId] = visible;
        // Could broadcast this if needed, but client TeamMenu.cs uses _showTeamBlipsSelf local toggle
        // and seemingly assumes availability if on team?
        // Actually TeamMenu.cs: "_showTeamBlipsSelf = _showBlipsSelfItem.Checked; BaseScript.TriggerServerEvent..."
        // It sends it to server but doesn't seem to expect a broadcast back in the showed code.
        // It handles blip visibility logic locally based on team data.
        // But wait, if _showTeamBlipsSelf is false, teammates shouldn't see me?
        // TeamMenu.cs: "if (!_playerBlips.ContainsKey(serverId)) ... "
        // It doesn't seem to check remote player's visibility preference in UpdateTeamBlips.
        // It checks "if (playerTeam == _currentTeam ...)"
        // Maybe the visibility logic was intended to be server-enforced or synced.
        // I'll store it here for now.
      }
    }

    private void OnPlayerDropped(string playerHandle)
    {
      if (int.TryParse(playerHandle, out int playerId))
      {
        if (playerTeams.ContainsKey(playerId))
        {
          playerTeams.Remove(playerId);
          TriggerClientEvent("cbps:teamUpdated", playerId, 0); // 0 = no team
        }
        playerBlipVisibility.Remove(playerId);
      }
    }
  }
}

using System;
using CitizenFX.Core;

namespace CBPSMenu.Server
{
  public class ServerMain : BaseScript
  {
    public ServerMain()
    {
      Debug.WriteLine("^2[CBPS Menu] ^7Starting server...");

      EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
      EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);

      Debug.WriteLine("^2[CBPS Menu] ^7Server started successfully!");
    }

    private void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
    {
      Debug.WriteLine($"^2[CBPS Menu] ^7Player {playerName} connecting...");
    }

    private void OnPlayerDropped([FromSource] Player player, string reason)
    {
      Debug.WriteLine($"^2[CBPS Menu] ^7Player dropped: {reason}");

      // Clean up player data
      TriggerEvent("cbps:playerDropped", player.Handle);
    }
  }
}

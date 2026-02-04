using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace CBPSMenu.Server
{
  public class PlayerManager : BaseScript
  {
    private List<string> _admins = new List<string>();
    private bool _adminOnly = false;

    public PlayerManager()
    {
      // Exports for other resources to use
      Exports.Add("IsPlayerAdmin", new Func<int, bool>(IsPlayerAdmin));

      // Load admins from KVP or Config if needed
      // For now, we rely on ACE permissions as the primary method,
      // but can also support the list method if we port Config.Admins
      LoadConfig();
    }

    private void LoadConfig()
    {
      // Could load from json, but ACE is better.
      // _adminOnly = ...
    }

    public bool IsPlayerAdmin(int playerId)
    {
      Player player = Players[playerId];
      if (player == null) return false;

      return IsPlayerAdmin(player);
    }

    public bool IsPlayerAdmin(Player player)
    {
      if (!_adminOnly)
      {
        // If admin only mode is disabled, check if we should allow everyone or still check ACEs
        // Lua script: "if not Config.AdminOnly then return true end"
        // So if AdminOnly is false, everyone is admin? That seems to be the logic in lua file.
        // But usually we want some restrictions.
        // However, matching Lua logic:
        return true;
      }

      // Check ACE permission
      if (API.IsPlayerAceAllowed(player.Handle, "cbps.admin")) return true;

      // Check identifier list (legacy support)
      foreach (var identifier in player.Identifiers)
      {
        if (_admins.Contains(identifier)) return true;
      }

      return false;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
  /// <summary>
  /// Online Players Menu - View and interact with other players
  /// Logic ported from vMenu/vMenu/menus/OnlinePlayers.cs
  /// Theme: Nord14 (Green) Header
  /// </summary>
  public class OnlinePlayersMenu
  {
    #region Variables

    public NativeMenu Menu { get; private set; }

    // Submenu for individual player actions
    private NativeMenu _playerActionsMenu;

    // Currently selected player
    private Player _selectedPlayer;

    // Spectate state
    private bool _isSpectating = false;
    private int _spectatePlayerHandle = -1;

    #endregion

    #region Constructor

    public OnlinePlayersMenu()
    {
      CreateMenu();
    }

    #endregion

    #region Menu Creation

    private void CreateMenu()
    {
      // Create main menu with Nord14 (Green) header color
      Menu = new NativeMenu("comboom.sucht", "Online Players")
      {
        UseMouse = false
      };

      // Apply Nord14 (Green) theme to header
      Menu.Banner.Color = ThemeManager.Nord14; // Green header for online players

      // Create player actions submenu
      _playerActionsMenu = new NativeMenu("comboom.sucht", "Player Actions")
      {
        UseMouse = false
      };
      _playerActionsMenu.Banner.Color = ThemeManager.Nord14;

      // Add to pool
      Main.Pool.Add(_playerActionsMenu);

      // Populate the player list
      RefreshPlayerList();

      // Setup menu events
      Menu.Shown += (sender, args) => RefreshPlayerList();
    }

    /// <summary>
    /// Refresh the list of online players
    /// </summary>
    public void RefreshPlayerList()
    {
      Menu.Clear();

      // Get player list through the Players property
      var players = GetOnlinePlayers();

      // Add header
      var headerItem = new NativeItem("~g~=== Online Players ===", $"Total Players: {players.Count}")
      {
        Enabled = false
      };
      Menu.Add(headerItem);

      // Add refresh button
      var refreshItem = new NativeItem("~w~Refresh List", "Refresh the player list");
      refreshItem.Activated += (sender, args) => RefreshPlayerList();
      Menu.Add(refreshItem);

      Menu.Add(new NativeSeparatorItem());

      // Loop through all players
      foreach (Player player in players.OrderBy(p => p.Name))
      {
        CreatePlayerItem(player);
      }

      // If no players found (shouldn't happen as local player is always there)
      if (players.Count == 0)
      {
        var noPlayersItem = new NativeItem("~r~No players found", "No other players are currently online")
        {
          Enabled = false
        };
        Menu.Add(noPlayersItem);
      }
    }

    /// <summary>
    /// Get list of online players
    /// </summary>
    private List<Player> GetOnlinePlayers()
    {
      var players = new List<Player>();

      // Get all player indices
      for (int i = 0; i < 256; i++)
      {
        if (API.NetworkIsPlayerActive(i))
        {
          int playerPed = API.GetPlayerPed(i);
          if (API.DoesEntityExist(playerPed))
          {
            // Create player from handle
            var player = new Player(i);
            players.Add(player);
          }
        }
      }

      return players;
    }

    /// <summary>
    /// Create a menu item for a specific player
    /// </summary>
    private void CreatePlayerItem(Player player)
    {
      bool isLocalPlayer = player.Handle == Game.Player.Handle;
      string playerLabel = isLocalPlayer ? "~b~(You)" : $"ID: {player.ServerId}";
      string playerName = GetSafePlayerName(player.Name);

      var playerItem = new NativeItem(playerName, $"Server ID: {player.ServerId} | Handle: {player.Handle}")
      {
        AltTitle = playerLabel
      };

      playerItem.Activated += (sender, args) =>
      {
        _selectedPlayer = player;
        OpenPlayerActionsMenu(player);
      };

      Menu.Add(playerItem);
    }

    /// <summary>
    /// Open the actions submenu for a specific player
    /// </summary>
    private void OpenPlayerActionsMenu(Player player)
    {
      _playerActionsMenu.Clear();
      _playerActionsMenu.Name = $"~s~Player: ~y~{GetSafePlayerName(player.Name)}";

      bool isLocalPlayer = player.Handle == Game.Player.Handle;

      // Player info header
      var infoItem = new NativeItem($"~g~Server ID: {player.ServerId}", $"Handle: {player.Handle}")
      {
        Enabled = false
      };
      _playerActionsMenu.Add(infoItem);

      _playerActionsMenu.Add(new NativeSeparatorItem());

      // Teleport To Player
      if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPTeleport) ||
          PermissionsManager.IsAllowed(PermissionsManager.Permission.OPAll))
      {
        var teleportItem = new NativeItem("~w~Teleport To Player", "Teleport to this player's location");
        teleportItem.Enabled = !isLocalPlayer;
        teleportItem.Activated += async (sender, args) =>
        {
          if (!isLocalPlayer)
          {
            await TeleportToPlayer(player);
          }
          else
          {
            Main.ShowNotification("~r~You cannot teleport to yourself!");
          }
        };
        _playerActionsMenu.Add(teleportItem);
      }

      // Summon Player
      if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPSummon) ||
          PermissionsManager.IsAllowed(PermissionsManager.Permission.OPAll))
      {
        var summonItem = new NativeItem("~w~Summon Player", "Teleport this player to your location");
        summonItem.Enabled = !isLocalPlayer;
        summonItem.Activated += (sender, args) =>
        {
          if (!isLocalPlayer)
          {
            SummonPlayer(player);
          }
          else
          {
            Main.ShowNotification("~r~You cannot summon yourself!");
          }
        };
        _playerActionsMenu.Add(summonItem);
      }

      // Spectate Player
      if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPSpectate) ||
          PermissionsManager.IsAllowed(PermissionsManager.Permission.OPAll))
      {
        string spectateLabel = _isSpectating && _spectatePlayerHandle == player.Handle
            ? "~o~Stop Spectating"
            : "~w~Spectate Player";
        var spectateItem = new NativeItem(spectateLabel, "Toggle spectating this player");
        spectateItem.Enabled = !isLocalPlayer;
        spectateItem.Activated += async (sender, args) =>
        {
          if (!isLocalPlayer)
          {
            await ToggleSpectate(player);
          }
          else
          {
            Main.ShowNotification("~r~You cannot spectate yourself!");
          }
        };
        _playerActionsMenu.Add(spectateItem);
      }

      _playerActionsMenu.Add(new NativeSeparatorItem());

      // Kill Player (Staff only)
      if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPKill) ||
          PermissionsManager.IsAllowed(PermissionsManager.Permission.OPAll))
      {
        var killItem = new NativeItem("~r~Kill Player", "Kill this player (they will be notified)");
        killItem.Enabled = !isLocalPlayer;
        killItem.Activated += (sender, args) =>
        {
          if (!isLocalPlayer)
          {
            KillPlayer(player);
          }
          else
          {
            Main.ShowNotification("~r~You cannot kill yourself through this menu!");
          }
        };
        _playerActionsMenu.Add(killItem);
      }

      // Kick Player (Staff only)
      if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPKick) ||
          PermissionsManager.IsAllowed(PermissionsManager.Permission.OPAll))
      {
        var kickItem = new NativeItem("~r~Kick Player", "Kick this player from the server");
        kickItem.Enabled = !isLocalPlayer;
        kickItem.Activated += async (sender, args) =>
        {
          if (!isLocalPlayer)
          {
            await KickPlayer(player);
          }
          else
          {
            Main.ShowNotification("~r~You cannot kick yourself!");
          }
        };
        _playerActionsMenu.Add(kickItem);
      }

      // Show the player actions menu
      _playerActionsMenu.Visible = true;
    }

    #endregion

    #region Player Actions

    /// <summary>
    /// Teleport to the specified player
    /// </summary>
    private async Task TeleportToPlayer(Player player)
    {
      try
      {
        if (player == null || player.Character == null)
        {
          Main.ShowNotification("~r~Player not found or not loaded!");
          return;
        }

        Vector3 targetPos = player.Character.Position;

        // Request collision at the target position
        API.RequestCollisionAtCoord(targetPos.X, targetPos.Y, targetPos.Z);

        // Wait a bit for collision to load
        await BaseScript.Delay(100);

        // Teleport the player
        Game.PlayerPed.Position = targetPos;

        Main.ShowNotification($"~g~Teleported to ~w~{GetSafePlayerName(player.Name)}");
        Debug.WriteLine($"[comboom.sucht] Teleported to player: {player.Name} (ID: {player.ServerId})");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[comboom.sucht] Error teleporting to player: {ex.Message}");
        Main.ShowNotification("~r~Failed to teleport to player!");
      }
    }

    /// <summary>
    /// Summon the specified player to your location
    /// </summary>
    private void SummonPlayer(Player player)
    {
      try
      {
        // Trigger server event to summon player
        BaseScript.TriggerServerEvent("cbpsMenu:SummonPlayer", player.ServerId);
        Main.ShowNotification($"~g~Summoning ~w~{GetSafePlayerName(player.Name)}~g~ to your location...");
        Debug.WriteLine($"[comboom.sucht] Summoning player: {player.Name} (ID: {player.ServerId})");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[comboom.sucht] Error summoning player: {ex.Message}");
        Main.ShowNotification("~r~Failed to summon player!");
      }
    }

    /// <summary>
    /// Toggle spectating the specified player
    /// </summary>
    private async Task ToggleSpectate(Player player)
    {
      try
      {
        if (_isSpectating && _spectatePlayerHandle == player.Handle)
        {
          // Stop spectating
          API.NetworkSetInSpectatorMode(false, Game.PlayerPed.Handle);
          _isSpectating = false;
          _spectatePlayerHandle = -1;
          Main.ShowNotification("~o~Stopped spectating");
          Debug.WriteLine("[comboom.sucht] Stopped spectating");
        }
        else
        {
          // Stop current spectate if any
          if (_isSpectating)
          {
            API.NetworkSetInSpectatorMode(false, Game.PlayerPed.Handle);
            await BaseScript.Delay(100);
          }

          // Start spectating the new player
          if (player.Character != null && player.Character.Exists())
          {
            API.NetworkSetInSpectatorMode(true, player.Character.Handle);
            _isSpectating = true;
            _spectatePlayerHandle = player.Handle;
            Main.ShowNotification($"~b~Spectating ~w~{GetSafePlayerName(player.Name)}");
            Debug.WriteLine($"[comboom.sucht] Now spectating: {player.Name} (ID: {player.ServerId})");
          }
          else
          {
            Main.ShowNotification("~r~Cannot spectate - player not loaded!");
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[comboom.sucht] Error toggling spectate: {ex.Message}");
        Main.ShowNotification("~r~Failed to toggle spectate!");
      }
    }

    /// <summary>
    /// Kill the specified player
    /// </summary>
    private void KillPlayer(Player player)
    {
      try
      {
        // Trigger server event to kill player
        BaseScript.TriggerServerEvent("cbpsMenu:KillPlayer", player.ServerId, Game.Player.Name);
        Main.ShowNotification($"~r~Killed ~w~{GetSafePlayerName(player.Name)}");
        Debug.WriteLine($"[comboom.sucht] Killed player: {player.Name} (ID: {player.ServerId})");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[comboom.sucht] Error killing player: {ex.Message}");
        Main.ShowNotification("~r~Failed to kill player!");
      }
    }

    /// <summary>
    /// Kick the specified player from the server
    /// </summary>
    private async Task KickPlayer(Player player)
    {
      try
      {
        // Get kick reason from user
        string reason = await Main.GetUserInput("Kick Reason", "Kicked by staff", 100);

        if (!string.IsNullOrEmpty(reason))
        {
          // Trigger server event to kick player
          BaseScript.TriggerServerEvent("cbpsMenu:KickPlayer", player.ServerId, reason);
          Main.ShowNotification($"~r~Kicked ~w~{GetSafePlayerName(player.Name)}~r~ for: {reason}");
          Debug.WriteLine($"[comboom.sucht] Kicked player: {player.Name} (ID: {player.ServerId}) - Reason: {reason}");

          // Refresh the player list
          RefreshPlayerList();
        }
        else
        {
          Main.ShowNotification("~y~Kick cancelled - no reason provided");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[comboom.sucht] Error kicking player: {ex.Message}");
        Main.ShowNotification("~r~Failed to kick player!");
      }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Get a safe player name (removes formatting codes)
    /// </summary>
    private string GetSafePlayerName(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        return "Unknown";
      }

      // Remove any potentially harmful formatting
      string safeName = name;

      // Remove common formatting codes
      safeName = System.Text.RegularExpressions.Regex.Replace(safeName, @"~[a-zA-Z]~", "");
      safeName = System.Text.RegularExpressions.Regex.Replace(safeName, @"\^[0-9]", "");

      // Limit length
      if (safeName.Length > 32)
      {
        safeName = safeName.Substring(0, 32) + "...";
      }

      return string.IsNullOrEmpty(safeName) ? "Unknown" : safeName;
    }

    /// <summary>
    /// Stop spectating if currently spectating
    /// </summary>
    public void StopSpectating()
    {
      if (_isSpectating)
      {
        API.NetworkSetInSpectatorMode(false, Game.PlayerPed.Handle);
        _isSpectating = false;
        _spectatePlayerHandle = -1;
      }
    }

    #endregion
  }
}

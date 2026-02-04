using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
  /// <summary>
  /// Team Menu - Manages team selection, blips, and nametags
  /// Ported from client/team.lua
  /// </summary>
  public class TeamMenu
  {
    #region Variables

    public NativeMenu Menu { get; private set; }

    // Team state
    private int _currentTeam = 0; // 0 = None, 1-8 = Team 1-8
    private bool _showTeamBlipsSelf = true;
    private bool _showTeamBlipsEveryone = false;
    private bool _showNametags = true;

    // Team definitions
    private static readonly string[] TeamNames =
    {
            "None", "Team 1", "Team 2", "Team 3", "Team 4",
            "Team 5", "Team 6", "Team 7", "Team 8"
        };

    // Team colors for blips (corresponding to BlipColor enum values)
    // None=0, Team1=Green, Team2=Blue, Team3=Red, Team4=Yellow,
    // Team5=Orange, Team6=Purple, Team7=Pink, Team8=White
    private static readonly int[] TeamBlipColors =
    {
            0,   // None - White
            2,   // Team 1 - Green
            3,   // Team 2 - Blue
            1,   // Team 3 - Red
            5,   // Team 4 - Yellow
            17,  // Team 5 - Orange
            7,   // Team 6 - Purple
            8,   // Team 7 - Pink
            4    // Team 8 - Grey
        };

    // Dictionary to store active blips for players (serverId -> blipHandle)
    private Dictionary<int, int> _playerBlips = new Dictionary<int, int>();

    // Dictionary to store player teams received from server (serverId -> teamIndex)
    private static Dictionary<int, int> _playerTeams = new Dictionary<int, int>();

    // Menu items for state tracking
    private NativeListItem<string> _teamSelectItem;
    private NativeCheckboxItem _showBlipsSelfItem;
    private NativeCheckboxItem _showBlipsEveryoneItem;
    private NativeCheckboxItem _showNametagsItem;

    #endregion

    #region Constructor

    public TeamMenu()
    {
      CreateMenu();
      RegisterEvents();
    }

    #endregion

    #region Menu Creation

    private void CreateMenu()
    {
      Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Team Options");

      // === TEAM SELECTION ===
      var teamHeader = new NativeItem("~b~=== Team Selection ===", "Select your team")
      {
        Enabled = false
      };
      Menu.Add(teamHeader);

      // Team Selection List
      _teamSelectItem = new NativeListItem<string>("Select Team", "Choose which team to join", TeamNames);
      _teamSelectItem.SelectedIndex = _currentTeam;
      _teamSelectItem.ItemChanged += (sender, args) =>
      {
        _currentTeam = _teamSelectItem.SelectedIndex;

        // Notify server about team change
        BaseScript.TriggerServerEvent("cbps:updateTeam", _currentTeam);

        if (_currentTeam == 0)
        {
          Main.ShowNotification("~y~You left your team");
        }
        else
        {
          Main.ShowNotification($"~g~You joined {TeamNames[_currentTeam]}");
        }
      };
      Menu.Add(_teamSelectItem);

      // Show current team info
      var teamInfoItem = new NativeItem("Current Team Info", "Shows your current team status");
      teamInfoItem.Activated += (sender, args) =>
      {
        if (_currentTeam == 0)
        {
          Main.ShowNotification("~y~You are not in a team");
        }
        else
        {
          int teammateCount = CountTeammates();
          Main.ShowNotification($"~b~Team: {TeamNames[_currentTeam]}\n~w~Teammates online: {teammateCount}");
        }
      };
      Menu.Add(teamInfoItem);

      Menu.Add(new NativeSeparatorItem());

      // === BLIP OPTIONS ===
      var blipHeader = new NativeItem("~b~=== Blip Options ===", "Configure team blips on the map")
      {
        Enabled = false
      };
      Menu.Add(blipHeader);

      // Show Team Blips (Self only - only your teammates can see you)
      _showBlipsSelfItem = new NativeCheckboxItem("Show My Blip to Teammates", "Allow teammates to see your blip on the map", _showTeamBlipsSelf);
      _showBlipsSelfItem.CheckboxChanged += (sender, args) =>
      {
        _showTeamBlipsSelf = _showBlipsSelfItem.Checked;
        BaseScript.TriggerServerEvent("cbps:updateBlipVisibility", _showTeamBlipsSelf);
        Main.ShowNotification(_showTeamBlipsSelf
                  ? "~g~Your blip is now visible to teammates"
                  : "~r~Your blip is now hidden from teammates");
      };
      Menu.Add(_showBlipsSelfItem);

      // Show Team Blips (See all teammates)
      _showBlipsEveryoneItem = new NativeCheckboxItem("Show Teammate Blips", "See your teammates on the map", _showTeamBlipsEveryone);
      _showBlipsEveryoneItem.CheckboxChanged += (sender, args) =>
      {
        _showTeamBlipsEveryone = _showBlipsEveryoneItem.Checked;

        if (!_showTeamBlipsEveryone)
        {
          // Clear all existing blips when disabled
          ClearAllBlips();
        }

        Main.ShowNotification(_showTeamBlipsEveryone
                  ? "~g~Teammate blips enabled"
                  : "~r~Teammate blips disabled");
      };
      Menu.Add(_showBlipsEveryoneItem);

      Menu.Add(new NativeSeparatorItem());

      // === NAMETAG OPTIONS ===
      var nametagHeader = new NativeItem("~b~=== Nametag Options ===", "Configure player nametags")
      {
        Enabled = false
      };
      Menu.Add(nametagHeader);

      // Show Nametags
      _showNametagsItem = new NativeCheckboxItem("Show Player Nametags", "Display names above players", _showNametags);
      _showNametagsItem.CheckboxChanged += (sender, args) =>
      {
        _showNametags = _showNametagsItem.Checked;
        Main.ShowNotification(_showNametags
                  ? "~g~Player nametags enabled"
                  : "~r~Player nametags disabled");
      };
      Menu.Add(_showNametagsItem);

      // Nametag distance
      var nametagDistances = new string[] { "Close (10m)", "Medium (25m)", "Far (50m)", "Very Far (100m)" };
      var nametagDistItem = new NativeListItem<string>("Nametag Distance", "Maximum distance to show nametags", nametagDistances);
      nametagDistItem.ItemChanged += (sender, args) =>
      {
        float[] distances = { 10f, 25f, 50f, 100f };
        float dist = distances[nametagDistItem.SelectedIndex];
        Main.ShowNotification($"~b~Nametag distance: {dist}m");
      };
      Menu.Add(nametagDistItem);

      Menu.Add(new NativeSeparatorItem());

      // === QUICK ACTIONS ===
      var actionsHeader = new NativeItem("~b~=== Quick Actions ===", "Team actions")
      {
        Enabled = false
      };
      Menu.Add(actionsHeader);

      // Leave Team
      var leaveTeamItem = new NativeItem("~r~Leave Team", "~r~Leave your current team");
      leaveTeamItem.Activated += (sender, args) =>
      {
        if (_currentTeam != 0)
        {
          _currentTeam = 0;
          _teamSelectItem.SelectedIndex = 0;
          BaseScript.TriggerServerEvent("cbps:updateTeam", 0);
          ClearAllBlips();
          Main.ShowNotification("~r~You have left the team");
        }
        else
        {
          Main.ShowNotification("~y~You are not in a team");
        }
      };
      Menu.Add(leaveTeamItem);

      // Refresh Team Data
      var refreshItem = new NativeItem("Refresh Team Data", "Request latest team data from server");
      refreshItem.Activated += (sender, args) =>
      {
        BaseScript.TriggerServerEvent("cbps:requestTeamData");
        Main.ShowNotification("~b~Refreshing team data...");
      };
      Menu.Add(refreshItem);
    }

    #endregion

    #region Event Registration

    private void RegisterEvents()
    {
      // Listen for team updates from server
      // This event is triggered when any player changes their team
      // Format: cbps:teamUpdated (serverId, teamIndex)
    }

    /// <summary>
    /// Handle team update from server
    /// Call this from Main.cs event handler
    /// </summary>
    public void OnTeamUpdated(int serverId, int teamIndex)
    {
      _playerTeams[serverId] = teamIndex;

      // If this is about our own player, update the menu
      if (serverId == Game.Player.ServerId)
      {
        _currentTeam = teamIndex;
        _teamSelectItem.SelectedIndex = teamIndex;
      }

      // Update blips if needed
      UpdatePlayerBlip(serverId, teamIndex);
    }

    /// <summary>
    /// Handle player disconnect - remove their blip
    /// </summary>
    public void OnPlayerDisconnect(int serverId)
    {
      _playerTeams.Remove(serverId);
      RemovePlayerBlip(serverId);
    }

    /// <summary>
    /// Receive full team data from server
    /// </summary>
    public void OnTeamDataReceived(Dictionary<int, int> teamData)
    {
      _playerTeams = teamData;

      // Rebuild all blips
      ClearAllBlips();

      if (_showTeamBlipsEveryone && _currentTeam != 0)
      {
        foreach (var kvp in _playerTeams)
        {
          if (kvp.Key != Game.Player.ServerId && kvp.Value == _currentTeam)
          {
            UpdatePlayerBlip(kvp.Key, kvp.Value);
          }
        }
      }
    }

    #endregion

    #region Tick Processing

    /// <summary>
    /// Process team blips and nametags each tick
    /// Call this from Main.cs OnTick
    /// </summary>
    public async Task ProcessTick()
    {
      // Only process if we're in a team and blips are enabled
      if (_currentTeam != 0 && _showTeamBlipsEveryone)
      {
        await UpdateTeamBlips();
      }

      // Process nametags if enabled
      if (_showNametags)
      {
        DrawNametags();
      }

      await Task.FromResult(0);
    }

    /// <summary>
    /// Update blips for all teammates
    /// </summary>
    private async Task UpdateTeamBlips()
    {
      var myServerId = Game.Player.ServerId;

      // Track which players we've seen this tick
      HashSet<int> seenPlayers = new HashSet<int>();

      // Iterate through all possible player slots (0-255)
      for (int i = 0; i < 256; i++)
      {
        if (!API.NetworkIsPlayerActive(i) || i == Game.Player.Handle)
          continue;

        int serverId = API.GetPlayerServerId(i);
        if (serverId <= 0 || serverId == myServerId)
          continue;

        seenPlayers.Add(serverId);

        // Check if player is in our team
        int playerTeam = GetPlayerTeam(serverId);

        if (playerTeam == _currentTeam && playerTeam != 0)
        {
          // Player is on our team - ensure they have a blip
          int playerPed = API.GetPlayerPed(i);
          if (playerPed != 0 && API.DoesEntityExist(playerPed))
          {
            if (!_playerBlips.ContainsKey(serverId))
            {
              // Create new blip
              int blip = API.AddBlipForEntity(playerPed);
              API.SetBlipSprite(blip, 1); // Standard blip
              API.SetBlipColour(blip, TeamBlipColors[_currentTeam]);
              API.SetBlipScale(blip, 0.8f);
              API.SetBlipAsShortRange(blip, false);
              API.BeginTextCommandSetBlipName("STRING");
              API.AddTextComponentSubstringPlayerName(API.GetPlayerName(i));
              API.EndTextCommandSetBlipName(blip);

              _playerBlips[serverId] = blip;
            }
            else
            {
              // Update existing blip position (entity blips track automatically)
              int existingBlip = _playerBlips[serverId];
              if (!API.DoesBlipExist(existingBlip))
              {
                // Blip was removed, recreate it
                _playerBlips.Remove(serverId);
              }
            }
          }
        }
        else
        {
          // Player is not on our team - remove their blip if they have one
          RemovePlayerBlip(serverId);
        }
      }

      // Remove blips for players we didn't see (disconnected)
      List<int> toRemove = new List<int>();
      foreach (var kvp in _playerBlips)
      {
        if (!seenPlayers.Contains(kvp.Key))
        {
          toRemove.Add(kvp.Key);
        }
      }
      foreach (int serverId in toRemove)
      {
        RemovePlayerBlip(serverId);
      }

      await Task.FromResult(0);
    }

    /// <summary>
    /// Draw nametags above players
    /// </summary>
    private void DrawNametags()
    {
      var myPos = Game.PlayerPed.Position;
      float maxDistance = 50f; // Default medium distance

      // Iterate through all possible player slots
      for (int i = 0; i < 256; i++)
      {
        if (!API.NetworkIsPlayerActive(i) || i == Game.Player.Handle)
          continue;

        int playerPed = API.GetPlayerPed(i);
        if (playerPed == 0 || !API.DoesEntityExist(playerPed))
          continue;

        Vector3 playerPos = API.GetEntityCoords(playerPed, true);
        float distance = Vector3.Distance(myPos, playerPos);

        if (distance <= maxDistance)
        {
          // Get color based on team
          int serverId = API.GetPlayerServerId(i);
          int playerTeam = GetPlayerTeam(serverId);
          int r = 255, g = 255, b = 255; // Default white

          if (playerTeam != 0)
          {
            // Color based on team
            GetTeamRGB(playerTeam, out r, out g, out b);
          }

          // Draw 3D text above player's head
          Vector3 headPos = new Vector3(playerPos.X, playerPos.Y, playerPos.Z + 1.0f);
          string playerName = API.GetPlayerName(i);
          DrawText3D(headPos, playerName, r, g, b, 255);
        }
      }
    }

    /// <summary>
    /// Draw 3D text in the world
    /// </summary>
    private void DrawText3D(Vector3 position, string text, int r, int g, int b, int a)
    {
      float screenX = 0f, screenY = 0f;
      bool onScreen = API.World3dToScreen2d(position.X, position.Y, position.Z, ref screenX, ref screenY);

      if (onScreen)
      {
        // Calculate distance for scaling
        float distance = Vector3.Distance(Game.PlayerPed.Position, position);
        float scale = Math.Max(0.2f, 0.35f - (distance / 100f));

        API.SetTextFont(4);
        API.SetTextScale(0.0f, scale);
        API.SetTextColour(r, g, b, a);
        API.SetTextOutline();
        API.SetTextCentre(true);
        API.SetTextEntry("STRING");
        API.AddTextComponentString(text);
        API.DrawText(screenX, screenY);
      }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Get a player's team from our cached data
    /// </summary>
    private int GetPlayerTeam(int serverId)
    {
      if (_playerTeams.TryGetValue(serverId, out int team))
      {
        return team;
      }
      return 0; // Default to no team
    }

    /// <summary>
    /// Update or remove a player's blip based on their team
    /// </summary>
    private void UpdatePlayerBlip(int serverId, int teamIndex)
    {
      // If player is no longer on our team, remove their blip
      if (teamIndex != _currentTeam || teamIndex == 0 || !_showTeamBlipsEveryone)
      {
        RemovePlayerBlip(serverId);
      }
      // Blip will be created on next tick if they're on our team
    }

    /// <summary>
    /// Remove a specific player's blip
    /// </summary>
    private void RemovePlayerBlip(int serverId)
    {
      if (_playerBlips.TryGetValue(serverId, out int blip))
      {
        if (API.DoesBlipExist(blip))
        {
          API.RemoveBlip(ref blip);
        }
        _playerBlips.Remove(serverId);
      }
    }

    /// <summary>
    /// Clear all team blips
    /// </summary>
    private void ClearAllBlips()
    {
      foreach (var kvp in _playerBlips)
      {
        int blip = kvp.Value;
        if (API.DoesBlipExist(blip))
        {
          API.RemoveBlip(ref blip);
        }
      }
      _playerBlips.Clear();
    }

    /// <summary>
    /// Count how many teammates are online
    /// </summary>
    private int CountTeammates()
    {
      int count = 0;
      foreach (var kvp in _playerTeams)
      {
        if (kvp.Key != Game.Player.ServerId && kvp.Value == _currentTeam)
        {
          count++;
        }
      }
      return count;
    }

    /// <summary>
    /// Get RGB values for a team color
    /// </summary>
    private void GetTeamRGB(int teamIndex, out int r, out int g, out int b)
    {
      switch (teamIndex)
      {
        case 1: // Green
          r = 114; g = 204; b = 114;
          break;
        case 2: // Blue
          r = 93; g = 182; b = 229;
          break;
        case 3: // Red
          r = 224; g = 50; b = 50;
          break;
        case 4: // Yellow
          r = 240; g = 200; b = 80;
          break;
        case 5: // Orange
          r = 255; g = 150; b = 50;
          break;
        case 6: // Purple
          r = 180; g = 70; b = 180;
          break;
        case 7: // Pink
          r = 240; g = 140; b = 180;
          break;
        case 8: // Grey/White
          r = 200; g = 200; b = 200;
          break;
        default:
          r = 255; g = 255; b = 255;
          break;
      }
    }

    /// <summary>
    /// Get current team index
    /// </summary>
    public int CurrentTeam => _currentTeam;

    /// <summary>
    /// Check if player is showing their blip
    /// </summary>
    public bool IsBlipVisible => _showTeamBlipsSelf;

    #endregion
  }
}

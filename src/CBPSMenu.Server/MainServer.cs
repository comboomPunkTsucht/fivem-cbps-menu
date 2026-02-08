using System;
using System.Collections.Generic;

using CitizenFX.Core;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Server
{
    /// <summary>
    /// Main server entry point for comboom.sucht Menu.
    /// </summary>
    public class MainServer : BaseScript
    {
        /// <summary>
        /// Track team memberships.
        /// </summary>
        private readonly Dictionary<string, string> playerTeams = new Dictionary<string, string>();

        /// <summary>
        /// Current weather state.
        /// </summary>
        private string currentWeather = "CLEAR";
        private bool dynamicWeather = true;

        /// <summary>
        /// Current time state.
        /// </summary>
        private int currentHour = 12;
        private int currentMinute = 0;
        private bool timeFrozen = false;

        public MainServer()
        {
            Debug.WriteLine("[comboom.sucht Menu] Server initialized.");
        }

        /// <summary>
        /// Client requests permissions on spawn.
        /// </summary>
        [EventHandler("cbps:requestPermissions")]
        private void OnRequestPermissions([FromSource] Player player)
        {
            if (player == null) return;

            PermissionsManager.SetPermissionsForPlayer(player);
            Debug.WriteLine($"[comboom.sucht Menu] Sent permissions to {player.Name}");
        }

        /// <summary>
        /// Player joins a team.
        /// </summary>
        [EventHandler("cbps:joinTeam")]
        private void OnJoinTeam([FromSource] Player player, string teamName, int frequency)
        {
            if (player == null) return;

            if (!PermissionsManager.IsAllowed(PermissionsManager.Permission.TMJoinTeam, player))
            {
                return;
            }

            playerTeams[player.Handle] = teamName;
            Debug.WriteLine($"[comboom.sucht Menu] {player.Name} joined team {teamName} on frequency {frequency}");

            // Broadcast to other players if needed
            TriggerClientEvent("cbps:playerJoinedTeam", player.Handle, player.Name, teamName);
        }

        /// <summary>
        /// Player leaves a team.
        /// </summary>
        [EventHandler("cbps:leaveTeam")]
        private void OnLeaveTeam([FromSource] Player player, string teamName)
        {
            if (player == null) return;

            if (playerTeams.ContainsKey(player.Handle))
            {
                playerTeams.Remove(player.Handle);
            }
            Debug.WriteLine($"[comboom.sucht Menu] {player.Name} left team {teamName}");

            TriggerClientEvent("cbps:playerLeftTeam", player.Handle, player.Name, teamName);
        }

        /// <summary>
        /// Set weather from client request.
        /// </summary>
        [EventHandler("cbps:setWeather")]
        private void OnSetWeather([FromSource] Player player, string weather)
        {
            if (player == null) return;

            if (!PermissionsManager.IsAllowed(PermissionsManager.Permission.WOSetWeather, player))
            {
                return;
            }

            currentWeather = weather;
            TriggerClientEvent("cbps:syncWeather", weather);
            Debug.WriteLine($"[comboom.sucht Menu] Weather set to {weather} by {player.Name}");
        }

        /// <summary>
        /// Set dynamic weather toggle.
        /// </summary>
        [EventHandler("cbps:setDynamicWeather")]
        private void OnSetDynamicWeather([FromSource] Player player, bool enabled)
        {
            if (player == null) return;

            if (!PermissionsManager.IsAllowed(PermissionsManager.Permission.WODynamic, player))
            {
                return;
            }

            dynamicWeather = enabled;
            TriggerClientEvent("cbps:syncDynamicWeather", enabled);
            Debug.WriteLine($"[comboom.sucht Menu] Dynamic weather set to {enabled} by {player.Name}");
        }

        /// <summary>
        /// Set time from client request.
        /// </summary>
        [EventHandler("cbps:setTime")]
        private void OnSetTime([FromSource] Player player, int hour, int minute, int second)
        {
            if (player == null) return;

            if (!PermissionsManager.IsAllowed(PermissionsManager.Permission.TOSetTime, player))
            {
                return;
            }

            currentHour = hour;
            currentMinute = minute;
            TriggerClientEvent("cbps:syncTime", hour, minute, second);
            Debug.WriteLine($"[comboom.sucht Menu] Time set to {hour:00}:{minute:00} by {player.Name}");
        }

        /// <summary>
        /// Set time frozen state.
        /// </summary>
        [EventHandler("cbps:setTimeFrozen")]
        private void OnSetTimeFrozen([FromSource] Player player, bool frozen)
        {
            if (player == null) return;

            if (!PermissionsManager.IsAllowed(PermissionsManager.Permission.TOFreezeTime, player))
            {
                return;
            }

            timeFrozen = frozen;
            TriggerClientEvent("cbps:syncTimeFrozen", frozen);
            Debug.WriteLine($"[comboom.sucht Menu] Time frozen set to {frozen} by {player.Name}");
        }

        /// <summary>
        /// When a player drops, clean up their team assignment.
        /// </summary>
        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if (player == null) return;

            if (playerTeams.ContainsKey(player.Handle))
            {
                var team = playerTeams[player.Handle];
                playerTeams.Remove(player.Handle);
                Debug.WriteLine($"[comboom.sucht Menu] {player.Name} disconnected, removed from team {team}");
            }
        }
    }
}

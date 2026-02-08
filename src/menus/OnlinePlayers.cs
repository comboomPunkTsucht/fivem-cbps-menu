using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Online Players submenu - vMenu clone.
    /// </summary>
    public class OnlinePlayers
    {
        private NativeMenu menu;
        private List<NativeMenu> playerMenus = new List<NativeMenu>();

        private void CreateMenu()
        {
            menu = new NativeMenu("Online Players", "Manage Online Players");

            RefreshPlayerList();
        }

        public void RefreshPlayerList()
        {
            if (menu == null) return;

            menu.Clear();
            playerMenus.Clear();

            var refreshBtn = new NativeItem("~g~Refresh Player List", "Update the list of online players.");
            refreshBtn.Activated += (s, e) => RefreshPlayerList();
            menu.Add(refreshBtn);

            // Use CitizenFX.Core.PlayerList for safer iteration
            var players = new PlayerList();
            foreach (Player player in players)
            {
                int playerId = player.Handle; // Local Handle (int)
                int serverId = player.ServerId; // Server ID (int)
                string playerName = player.Name;

                if (!string.IsNullOrEmpty(playerName))
                {
                    // Pass explicit types to avoid dynamic binding
                    var playerMenu = CreatePlayerMenuById(playerId, serverId, playerName);
                    playerMenus.Add(playerMenu);

                    var playerBtn = new NativeItem($"[{serverId}] {playerName}", $"Manage player {playerName}.") { AltTitle = "→→→" };
                    menu.Add(playerBtn);
                }
            }
        }

        private NativeMenu CreatePlayerMenuById(int playerId, int serverId, string playerName)
        {
            var playerMenu = new NativeMenu(playerName, $"Player: {playerName} (ID: {serverId})");

            #region Teleport Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPTeleport))
            {
                var teleportTo = new NativeItem("Teleport To Player", $"Teleport to {playerName}.");
                teleportTo.Activated += (s, e) =>
                {
                    var targetPed = GetPlayerPed(playerId);
                    if (targetPed > 0)
                    {
                        var coords = GetEntityCoords(targetPed, true);
                        SetPedCoordsKeepVehicle(PlayerPedId(), coords.X, coords.Y, coords.Z);
                        Notify.Success($"Teleported to {playerName}.");
                    }
                    else
                    {
                        Notify.Error("Could not find player.");
                    }
                };
                playerMenu.Add(teleportTo);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPSummon))
            {
                var summon = new NativeItem("Summon Player", $"Teleport {playerName} to you.");
                summon.Activated += (s, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:SummonPlayer", serverId);
                    Notify.Info($"Summon request sent for {playerName}.");
                };
                playerMenu.Add(summon);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPWaypoint))
            {
                var setWaypoint = new NativeItem("Set Waypoint to Player", $"Set waypoint to {playerName}'s location.");
                setWaypoint.Activated += (s, e) =>
                {
                    var targetPed = GetPlayerPed(playerId);
                    if (targetPed > 0)
                    {
                        var coords = GetEntityCoords(targetPed, true);
                        SetNewWaypoint(coords.X, coords.Y);
                        Notify.Success($"Waypoint set to {playerName}.");
                    }
                };
                playerMenu.Add(setWaypoint);
            }

            #endregion

            #region Spectate

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPSpectate))
            {
                var spectate = new NativeItem("Spectate Player", $"Spectate {playerName}.");
                spectate.Activated += async (s, e) =>
                {
                    var targetPed = GetPlayerPed(playerId);
                    if (targetPed > 0 && targetPed != PlayerPedId())
                    {
                        if (NetworkIsInSpectatorMode())
                        {
                            NetworkSetInSpectatorMode(false, targetPed);
                            Notify.Info("Stopped spectating.");
                        }
                        else
                        {
                            var coords = GetEntityCoords(targetPed, true);
                            RequestCollisionAtCoord(coords.X, coords.Y, coords.Z);
                            await BaseScript.Delay(1000);
                            NetworkSetInSpectatorMode(true, targetPed);
                            Notify.Info($"Spectating {playerName}. Use this again to stop.");
                        }
                    }
                    else
                    {
                        Notify.Error("Cannot spectate this player.");
                    }
                };
                playerMenu.Add(spectate);
            }

            #endregion

            #region Identifiers

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPIdentifiers))
            {
                var showIds = new NativeItem("Show Identifiers", $"Display {playerName}'s identifiers.");
                showIds.Activated += (s, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:RequestPlayerIdentifiers", serverId);
                };
                playerMenu.Add(showIds);
            }

            #endregion

            #region Kill Player

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPKill))
            {
                var killPlayer = new NativeItem("~r~Kill Player", $"Kill {playerName}.");
                killPlayer.Activated += (s, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:KillPlayer", serverId);
                    Notify.Info($"Kill request sent for {playerName}.");
                };
                playerMenu.Add(killPlayer);
            }

            #endregion

            #region Kick/Ban

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPKick))
            {
                var kickPlayer = new NativeItem("~r~Kick Player", $"Kick {playerName} from the server.");
                kickPlayer.Activated += async (s, e) =>
                {
                    var reason = await GetUserInput("Enter kick reason", "Kicked by admin", 100);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        BaseScript.TriggerServerEvent("cbps:KickPlayer", serverId, reason);
                        Notify.Success($"Kicked {playerName}.");
                    }
                };
                playerMenu.Add(kickPlayer);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPTempBan))
            {
                var tempBanPlayer = new NativeItem("~r~Temp Ban Player", $"Temporarily ban {playerName}.");
                tempBanPlayer.Activated += async (s, e) =>
                {
                    var reason = await GetUserInput("Enter ban reason", "Temp banned by admin", 100);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        BaseScript.TriggerServerEvent("cbps:TempBanPlayer", serverId, reason, 24);
                        Notify.Success($"Temp banned {playerName} for 24 hours.");
                    }
                };
                playerMenu.Add(tempBanPlayer);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPPermBan))
            {
                var permBanPlayer = new NativeItem("~r~~h~PERMANENT BAN~h~", $"Permanently ban {playerName}.");
                permBanPlayer.Activated += async (s, e) =>
                {
                    var reason = await GetUserInput("Enter permanent ban reason", "Permanently banned by admin", 100);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        BaseScript.TriggerServerEvent("cbps:PermBanPlayer", serverId, reason);
                        Notify.Success($"Permanently banned {playerName}.");
                    }
                };
                playerMenu.Add(permBanPlayer);
            }

            #endregion

            return playerMenu;
        }

        private async Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            AddTextEntry("FMMC_KEY_TIP1", windowTitle);
            DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText, "", "", "", maxLength);
            while (UpdateOnscreenKeyboard() == 0)
            {
                await BaseScript.Delay(0);
            }
            if (UpdateOnscreenKeyboard() == 1)
            {
                return GetOnscreenKeyboardResult();
            }
            return null;
        }

        public NativeMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }

        public List<NativeMenu> GetPlayerMenus() => playerMenus;
    }
}

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

            var players = new PlayerList();
            foreach (Player player in players)
            {
                if (player != null)
                {
                    var playerMenu = CreatePlayerMenu(player);
                    playerMenus.Add(playerMenu);

                    var playerBtn = new NativeItem($"[{player.ServerId}] {player.Name}", $"Manage player {player.Name}.") { AltTitle = "→→→" };
                    menu.Add(playerBtn);
                }
            }
        }

        private NativeMenu CreatePlayerMenu(Player player)
        {
            var playerMenu = new NativeMenu(player.Name, $"Player: {player.Name} (ID: {player.ServerId})");

            #region Teleport Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPTeleport))
            {
                var teleportTo = new NativeItem("Teleport To Player", $"Teleport to {player.Name}.");
                teleportTo.Activated += (s, e) =>
                {
                    var targetPed = GetPlayerPed(player.Handle);
                    if (targetPed > 0)
                    {
                        var coords = GetEntityCoords(targetPed, true);
                        SetPedCoordsKeepVehicle(Game.PlayerPed.Handle, coords.X, coords.Y, coords.Z);
                        Notify.Success($"Teleported to {player.Name}.");
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
                var summon = new NativeItem("Summon Player", $"Teleport {player.Name} to you.");
                summon.Activated += (s, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:SummonPlayer", player.ServerId);
                    Notify.Info($"Summon request sent for {player.Name}.");
                };
                playerMenu.Add(summon);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPWaypoint))
            {
                var setWaypoint = new NativeItem("Set Waypoint to Player", $"Set waypoint to {player.Name}'s location.");
                setWaypoint.Activated += (s, e) =>
                {
                    var targetPed = GetPlayerPed(player.Handle);
                    if (targetPed > 0)
                    {
                        var coords = GetEntityCoords(targetPed, true);
                        SetNewWaypoint(coords.X, coords.Y);
                        Notify.Success($"Waypoint set to {player.Name}.");
                    }
                };
                playerMenu.Add(setWaypoint);
            }

            #endregion

            #region Spectate

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPSpectate))
            {
                var spectate = new NativeItem("Spectate Player", $"Spectate {player.Name}.");
                spectate.Activated += async (s, e) =>
                {
                    var targetPed = GetPlayerPed(player.Handle);
                    if (targetPed > 0 && targetPed != Game.PlayerPed.Handle)
                    {
                        if (NetworkIsInSpectatorMode())
                        {
                            NetworkSetInSpectatorMode(false, targetPed);
                            Notify.Info("Stopped spectating.");
                        }
                        else
                        {
                            RequestCollisionAtCoord(GetEntityCoords(targetPed, true).X, GetEntityCoords(targetPed, true).Y, GetEntityCoords(targetPed, true).Z);
                            await BaseScript.Delay(1000);
                            NetworkSetInSpectatorMode(true, targetPed);
                            Notify.Info($"Spectating {player.Name}. Use this again to stop.");
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
                var showIds = new NativeItem("Show Identifiers", $"Display {player.Name}'s identifiers.");
                showIds.Activated += (s, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:RequestPlayerIdentifiers", player.ServerId);
                };
                playerMenu.Add(showIds);
            }

            #endregion

            #region Kill Player

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPKill))
            {
                var killPlayer = new NativeItem("~r~Kill Player", $"Kill {player.Name}.");
                killPlayer.Activated += (s, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:KillPlayer", player.ServerId);
                    Notify.Info($"Kill request sent for {player.Name}.");
                };
                playerMenu.Add(killPlayer);
            }

            #endregion

            #region Kick/Ban

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPKick))
            {
                var kickPlayer = new NativeItem("~r~Kick Player", $"Kick {player.Name} from the server.");
                kickPlayer.Activated += async (s, e) =>
                {
                    var reason = await GetUserInput("Enter kick reason", "Kicked by admin", 100);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        BaseScript.TriggerServerEvent("cbps:KickPlayer", player.ServerId, reason);
                        Notify.Success($"Kicked {player.Name}.");
                    }
                };
                playerMenu.Add(kickPlayer);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPTempBan))
            {
                var tempBanPlayer = new NativeItem("~r~Temp Ban Player", $"Temporarily ban {player.Name}.");
                tempBanPlayer.Activated += async (s, e) =>
                {
                    var reason = await GetUserInput("Enter ban reason", "Temp banned by admin", 100);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        BaseScript.TriggerServerEvent("cbps:TempBanPlayer", player.ServerId, reason, 24); // 24 hour ban
                        Notify.Success($"Temp banned {player.Name} for 24 hours.");
                    }
                };
                playerMenu.Add(tempBanPlayer);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPPermBan))
            {
                var permBanPlayer = new NativeItem("~r~~h~PERMANENT BAN~h~", $"Permanently ban {player.Name}.");
                permBanPlayer.Activated += async (s, e) =>
                {
                    var reason = await GetUserInput("Enter permanent ban reason", "Permanently banned by admin", 100);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        BaseScript.TriggerServerEvent("cbps:PermBanPlayer", player.ServerId, reason);
                        Notify.Success($"Permanently banned {player.Name}.");
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

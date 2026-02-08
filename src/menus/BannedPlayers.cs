using System;
using System.Collections.Generic;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Banned Players submenu - vMenu clone.
    /// Server-side functionality handles actual banning.
    /// </summary>
    public class BannedPlayers
    {
        private NativeMenu menu;
        private List<BannedPlayerData> bannedPlayers = new List<BannedPlayerData>();

        private void CreateMenu()
        {
            menu = new NativeMenu("Banned Players", "View and Manage Bans");

            // Request ban list from server
            BaseScript.TriggerServerEvent("cbps:RequestBanList");

            RefreshMenu();
        }

        public void UpdateBanList(List<BannedPlayerData> bans)
        {
            bannedPlayers = bans ?? new List<BannedPlayerData>();
            RefreshMenu();
        }

        private void RefreshMenu()
        {
            menu.Clear();

            #region Refresh Button

            var refreshBtn = new NativeItem("~g~Refresh Ban List", "Request updated ban list from server.");
            refreshBtn.Activated += (s, e) =>
            {
                BaseScript.TriggerServerEvent("cbps:RequestBanList");
                Notify.Info("Requesting ban list...");
            };
            menu.Add(refreshBtn);

            #endregion

            #region Ban Stats

            var statsItem = new NativeItem($"Total Bans: {bannedPlayers.Count}", "Number of banned players.");
            menu.Add(statsItem);

            #endregion

            #region Banned Players List

            if (bannedPlayers.Count == 0)
            {
                var noBans = new NativeItem("~c~No Banned Players", "No one is currently banned.");
                menu.Add(noBans);
            }
            else
            {
                foreach (var player in bannedPlayers)
                {
                    var playerSubmenu = new NativeMenu(player.Name, $"Ban Details: {player.Name}");
                    var playerBtn = new NativeItem(player.Name, $"Banned: {player.BanDate}") { AltTitle = "→→→" };
                    menu.Add(playerBtn);

                    // Ban info
                    var reasonItem = new NativeItem($"Reason: {player.Reason}", "Ban reason.");
                    playerSubmenu.Add(reasonItem);

                    var dateItem = new NativeItem($"Date: {player.BanDate}", "When the ban was issued.");
                    playerSubmenu.Add(dateItem);

                    var typeItem = new NativeItem($"Type: {(player.IsPermanent ? "Permanent" : "Temporary")}", "Ban type.");
                    playerSubmenu.Add(typeItem);

                    if (!player.IsPermanent)
                    {
                        var expiresItem = new NativeItem($"Expires: {player.ExpireDate}", "When the ban expires.");
                        playerSubmenu.Add(expiresItem);
                    }

                    var bannedByItem = new NativeItem($"Banned By: {player.BannedBy}", "Admin who issued the ban.");
                    playerSubmenu.Add(bannedByItem);

                    // Unban option
                    if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPUnban))
                    {
                        var unbanBtn = new NativeItem("~g~Unban Player", "Remove this player's ban.");
                        unbanBtn.Activated += (s, e) =>
                        {
                            BaseScript.TriggerServerEvent("cbps:UnbanPlayer", player.Identifier);
                            Notify.Info($"Unban request sent for {player.Name}.");
                        };
                        playerSubmenu.Add(unbanBtn);
                    }
                }
            }

            #endregion

            #region Search

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPUnban))
            {
                var searchUnban = new NativeItem("Search & Unban by Identifier", "Enter a player identifier to unban.");
                searchUnban.Activated += async (s, e) =>
                {
                    var identifier = await GetUserInput("Enter player identifier", "", 100);
                    if (!string.IsNullOrEmpty(identifier))
                    {
                        BaseScript.TriggerServerEvent("cbps:UnbanPlayer", identifier);
                        Notify.Info("Unban request sent.");
                    }
                };
                menu.Add(searchUnban);
            }

            #endregion
        }

        private async System.Threading.Tasks.Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
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
    }

    public class BannedPlayerData
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Reason { get; set; }
        public string BanDate { get; set; }
        public string ExpireDate { get; set; }
        public bool IsPermanent { get; set; }
        public string BannedBy { get; set; }
    }
}

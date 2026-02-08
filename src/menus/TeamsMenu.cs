using System;
using System.Collections.Generic;

using CitizenFX.Core;

using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Teams Menu - Allows players to join teams with automatic radio frequency assignment.
    /// Integrates with pma-voice and pma-radio.
    /// </summary>
    public class TeamsMenu
    {
        private NativeMenu menu;
        private static ExportDictionary exports;

        public string CurrentTeam { get; private set; } = null;

        public static void SetExports(ExportDictionary exp)
        {
            exports = exp;
        }

        private void CreateMenu()
        {
            menu = new NativeMenu("Teams", "Join a team to communicate via radio");

            // Info item showing current team
            var currentTeamItem = new NativeItem("Current Team", "Your current team assignment.");
            currentTeamItem.AltTitle = CurrentTeam ?? "None";
            menu.Add(currentTeamItem);

            menu.Add(new NativeSeparatorItem());

            // Team selection items
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.TMJoinTeam))
            {
                foreach (var team in Config.Teams)
                {
                    var teamItem = new NativeItem(team.Key, $"Join {team.Key} and set radio to {team.Value.Frequency} MHz.");
                    teamItem.Activated += (sender, e) =>
                    {
                        JoinTeam(team.Key, team.Value.Frequency);
                        currentTeamItem.AltTitle = team.Key;
                    };
                    menu.Add(teamItem);
                }
            }

            // Leave Team button
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.TMLeaveTeam))
            {
                menu.Add(new NativeSeparatorItem());

                var leaveTeamItem = new NativeItem("~r~Leave Team", "Leave your current team and disable radio.");
                leaveTeamItem.Activated += (sender, e) =>
                {
                    LeaveTeam();
                    currentTeamItem.AltTitle = "None";
                };
                menu.Add(leaveTeamItem);
            }
        }

        /// <summary>
        /// Joins a team and sets the pma-radio frequency.
        /// </summary>
        private void JoinTeam(string teamName, int frequency)
        {
            CurrentTeam = teamName;

            try
            {
                // Set the radio channel via pma-voice/pma-radio exports
                if (exports != null)
                {
                    exports["pma-voice"].setRadioChannel(frequency);
                    exports["pma-voice"].setVoiceProperty("radioEnabled", true);
                }

                Notify.Success($"Joined {teamName}. Radio set to {frequency} MHz.");

                // Notify server of team join
                BaseScript.TriggerServerEvent("cbps:joinTeam", teamName, frequency);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error setting radio channel: {ex.Message}");
                Notify.Error("Failed to set radio channel. Is pma-voice installed?");
            }
        }

        /// <summary>
        /// Leaves the current team and disables radio.
        /// </summary>
        private void LeaveTeam()
        {
            var previousTeam = CurrentTeam;
            CurrentTeam = null;

            try
            {
                // Disable radio channel
                if (exports != null)
                {
                    exports["pma-voice"].setRadioChannel(0);
                }

                Notify.Info($"Left {previousTeam}. Radio disabled.");

                // Notify server of team leave
                BaseScript.TriggerServerEvent("cbps:leaveTeam", previousTeam);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error disabling radio: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the menu, creating it if necessary.
        /// </summary>
        public NativeMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }
    }
}

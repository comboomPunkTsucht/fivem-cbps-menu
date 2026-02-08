using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Client.Data;
using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Player Options submenu - Full vMenu clone.
    /// </summary>
    public class PlayerOptions
    {
        private NativeMenu menu;
        private NativeMenu vehicleAutoPilotMenu;
        private NativeMenu customDrivingStyleMenu;

        // Player state
        public bool PlayerGodMode { get; private set; } = false;
        public bool PlayerInvisible { get; private set; } = false;
        public bool PlayerFastRun { get; private set; } = false;
        public bool PlayerFastSwim { get; private set; } = false;
        public bool PlayerSuperJump { get; private set; } = false;
        public bool PlayerNoRagdoll { get; private set; } = false;
        public bool PlayerNeverWanted { get; private set; } = false;
        public bool PlayerIgnored { get; private set; } = false;
        public bool PlayerStayInVehicle { get; private set; } = false;
        public bool PlayerUnlimitedStamina { get; private set; } = false;
        public bool PlayerFrozen { get; private set; } = false;
        public int CurrentScenarioIndex { get; private set; } = 0;
        public int CurrentBloodIndex { get; private set; } = 0;
        public int DrivingStyleIndex { get; private set; } = 0;

        private readonly List<string> drivingStyles = new List<string>
        {
            "Normal", "Rushed", "Avoid Highways", "Drive In Reverse", "Custom"
        };

        private readonly List<string> armorTypes = new List<string>
        {
            "No Armor", "Light Armor", "Standard Armor", "Heavy Armor", "Super Armor", "Ultra Armor"
        };

        private int[] customDrivingStyleFlags;

        private void CreateMenu()
        {
            menu = new NativeMenu("Player Options", "Configure player settings");

            // Create submenus
            vehicleAutoPilotMenu = new NativeMenu("Auto Pilot", "Vehicle auto pilot options");
            customDrivingStyleMenu = new NativeMenu("Custom Driving Style", "Custom style: 0");

            customDrivingStyleFlags = new int[31];

            #region Checkboxes

            // Godmode checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POGod))
            {
                var godModeCheckbox = new NativeCheckboxItem("Godmode", "Makes you invincible.", PlayerGodMode);
                godModeCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerGodMode = godModeCheckbox.Checked;
                    SetEntityInvincible(Game.PlayerPed.Handle, PlayerGodMode);
                };
                menu.Add(godModeCheckbox);
            }

            // Invisible checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POInvisible))
            {
                var invisibleCheckbox = new NativeCheckboxItem("Invisible", "Makes you invisible to yourself and others.", PlayerInvisible);
                invisibleCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerInvisible = invisibleCheckbox.Checked;
                    SetEntityVisible(Game.PlayerPed.Handle, !PlayerInvisible, false);
                };
                menu.Add(invisibleCheckbox);
            }

            // Unlimited Stamina checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POUnlimitedStamina))
            {
                var staminaCheckbox = new NativeCheckboxItem("Unlimited Stamina", "Run forever without slowing down or taking damage.", PlayerUnlimitedStamina);
                staminaCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerUnlimitedStamina = staminaCheckbox.Checked;
                    StatSetInt((uint)GetHashKey("MP0_STAMINA"), staminaCheckbox.Checked ? 100 : 0, true);
                };
                menu.Add(staminaCheckbox);
            }

            // Fast Run checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POFastRun))
            {
                var fastRunCheckbox = new NativeCheckboxItem("Fast Run", "Get ~g~Snail~s~ powers and run very fast!", PlayerFastRun);
                fastRunCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerFastRun = fastRunCheckbox.Checked;
                    SetRunSprintMultiplierForPlayer(Game.Player.Handle, fastRunCheckbox.Checked ? 1.49f : 1f);
                };
                menu.Add(fastRunCheckbox);
            }

            // Fast Swim checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POFastSwim))
            {
                var fastSwimCheckbox = new NativeCheckboxItem("Fast Swim", "Get ~g~Snail 2.0~s~ powers and swim super fast!", PlayerFastSwim);
                fastSwimCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerFastSwim = fastSwimCheckbox.Checked;
                    SetSwimMultiplierForPlayer(Game.Player.Handle, fastSwimCheckbox.Checked ? 1.49f : 1f);
                };
                menu.Add(fastSwimCheckbox);
            }

            // Super Jump checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POSuperjump))
            {
                var superJumpCheckbox = new NativeCheckboxItem("Super Jump", "Get ~g~Snail 3.0~s~ powers and jump like a champ!", PlayerSuperJump);
                superJumpCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerSuperJump = superJumpCheckbox.Checked;
                };
                menu.Add(superJumpCheckbox);
            }

            // No Ragdoll checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PONoRagdoll))
            {
                var noRagdollCheckbox = new NativeCheckboxItem("No Ragdoll", "Disables player ragdoll, makes you not fall off your bike anymore.", PlayerNoRagdoll);
                noRagdollCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerNoRagdoll = noRagdollCheckbox.Checked;
                    SetPedCanRagdoll(Game.PlayerPed.Handle, !PlayerNoRagdoll);
                };
                menu.Add(noRagdollCheckbox);
            }

            // Never Wanted checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PONeverWanted))
            {
                var neverWantedCheckbox = new NativeCheckboxItem("Never Wanted", "Disables all wanted levels.", PlayerNeverWanted);
                neverWantedCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerNeverWanted = neverWantedCheckbox.Checked;
                    SetMaxWantedLevel(neverWantedCheckbox.Checked ? 0 : 5);
                    if (neverWantedCheckbox.Checked)
                    {
                        ClearPlayerWantedLevel(Game.Player.Handle);
                    }
                };
                menu.Add(neverWantedCheckbox);
            }

            #endregion

            #region List Items

            // Set Wanted Level list
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POSetWanted))
            {
                var wantedLevels = new List<string> { "No Wanted Level", "1", "2", "3", "4", "5" };
                var setWantedLevel = new NativeListItem<string>("Set Wanted Level", "Set your wanted level by selecting a value.", wantedLevels.ToArray());
                setWantedLevel.SelectedIndex = GetPlayerWantedLevel(Game.Player.Handle);
                setWantedLevel.ItemChanged += (sender, e) =>
                {
                    var level = wantedLevels.IndexOf(e.Object);
                    SetPlayerWantedLevel(Game.Player.Handle, level, false);
                    SetPlayerWantedLevelNow(Game.Player.Handle, false);
                };
                menu.Add(setWantedLevel);
            }

            // Set Armor Type list
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POMaxArmor))
            {
                var setArmorItem = new NativeListItem<string>("Set Armor Type", "Set the armor level/type for your player.", armorTypes.ToArray());
                setArmorItem.ItemChanged += (sender, e) =>
                {
                    Game.PlayerPed.Armor = setArmorItem.SelectedIndex * 20;
                    Notify.Success($"Armor set to {armorTypes[setArmorItem.SelectedIndex]}.");
                };
                menu.Add(setArmorItem);
            }

            // Set Blood Level list
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POSetBlood))
            {
                var setBloodLevel = new NativeListItem<string>("Set Blood Level", "Sets your players blood level.", BloodTypes.BloodList.ToArray());
                setBloodLevel.ItemChanged += (sender, e) =>
                {
                    CurrentBloodIndex = BloodTypes.BloodList.IndexOf(e.Object);
                    ApplyPedDamagePack(Game.PlayerPed.Handle, BloodTypes.BloodList[CurrentBloodIndex], 100f, 100f);
                };
                menu.Add(setBloodLevel);
            }

            #endregion

            #region Buttons

            // Clear Blood button
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POClearBlood))
            {
                var clearBloodBtn = new NativeItem("Clear Blood", "Clear the blood off your player.");
                clearBloodBtn.Activated += (sender, e) =>
                {
                    Game.PlayerPed.ClearBloodDamage();
                    Game.PlayerPed.ResetVisibleDamage();
                    ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 0, "ALL");
                    ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 1, "ALL");
                    ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 2, "ALL");
                    ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 3, "ALL");
                    ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 4, "ALL");
                    ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 5, "ALL");
                    Notify.Success("Blood cleared.");
                };
                menu.Add(clearBloodBtn);
            }

            // Everyone Ignores Player checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POIgnored))
            {
                var ignoredCheckbox = new NativeCheckboxItem("Everyone Ignore Player", "Everyone will leave you alone.", PlayerIgnored);
                ignoredCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerIgnored = ignoredCheckbox.Checked;
                    SetEveryoneIgnorePlayer(Game.Player.Handle, PlayerIgnored);
                    SetPoliceIgnorePlayer(Game.Player.Handle, PlayerIgnored);
                    SetPlayerCanBeHassledByGangs(Game.Player.Handle, !PlayerIgnored);
                };
                menu.Add(ignoredCheckbox);
            }

            // Stay In Vehicle checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POStayInVehicle))
            {
                var stayInVehicleCheckbox = new NativeCheckboxItem("Stay In Vehicle", "When enabled, NPCs will not be able to drag you out of your vehicle.", PlayerStayInVehicle);
                stayInVehicleCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerStayInVehicle = stayInVehicleCheckbox.Checked;
                };
                menu.Add(stayInVehicleCheckbox);
            }

            // Heal Player button
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POMaxHealth))
            {
                var healPlayerBtn = new NativeItem("Heal Player", "Give the player max health.");
                healPlayerBtn.Activated += (sender, e) =>
                {
                    Game.PlayerPed.Health = Game.PlayerPed.MaxHealth;
                    Notify.Success("Player healed.");
                };
                menu.Add(healPlayerBtn);
            }

            // Clean Player Clothes button
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POCleanPlayer))
            {
                var cleanPlayerBtn = new NativeItem("Clean Player Clothes", "Clean your player clothes.");
                cleanPlayerBtn.Activated += (sender, e) =>
                {
                    Game.PlayerPed.ClearBloodDamage();
                    Notify.Success("Player clothes have been cleaned.");
                };
                menu.Add(cleanPlayerBtn);
            }

            // Dry Player Clothes button
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PODryPlayer))
            {
                var dryPlayerBtn = new NativeItem("Dry Player Clothes", "Make your player clothes dry.");
                dryPlayerBtn.Activated += (sender, e) =>
                {
                    Game.PlayerPed.WetnessHeight = 0f;
                    Notify.Success("Player is now dry.");
                };
                menu.Add(dryPlayerBtn);
            }

            // Wet Player Clothes button
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POWetPlayer))
            {
                var wetPlayerBtn = new NativeItem("Wet Player Clothes", "Make your player clothes wet.");
                wetPlayerBtn.Activated += (sender, e) =>
                {
                    Game.PlayerPed.WetnessHeight = 2f;
                    Notify.Success("Player is now wet.");
                };
                menu.Add(wetPlayerBtn);
            }

            // Commit Suicide button
            var suicidePlayerBtn = new NativeItem("~r~Commit Suicide", "Kill yourself by taking the pill. Or by using a pistol if you have one.");
            suicidePlayerBtn.Activated += async (sender, e) =>
            {
                await CommitSuicide();
            };
            menu.Add(suicidePlayerBtn);

            #endregion

            #region Vehicle Auto Pilot

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POVehicleAutoPilotMenu))
            {
                var vehicleAutoPilotBtn = new NativeItem("Vehicle Auto Pilot Menu", "Manage vehicle auto pilot options.")
                {
                    AltTitle = "→→→"
                };
                menu.Add(vehicleAutoPilotBtn);

                // Create auto pilot submenu
                var drivingStyleList = new NativeListItem<string>("Driving Style", "Set the driving style for auto pilot.", drivingStyles.ToArray());
                drivingStyleList.ItemChanged += (sender, e) =>
                {
                    DrivingStyleIndex = drivingStyles.IndexOf(e.Object);
                    var style = GetStyleFromIndex(DrivingStyleIndex);
                    SetDriveTaskDrivingStyle(Game.PlayerPed.Handle, style);
                    Notify.Info($"Driving style set to: ~r~{drivingStyles[DrivingStyleIndex]}~s~.");
                };
                vehicleAutoPilotMenu.Add(drivingStyleList);

                // Custom Driving Style submenu button
                var customStyleBtn = new NativeItem("Custom Driving Style", "Select a custom driving style.")
                {
                    AltTitle = "→→→"
                };
                vehicleAutoPilotMenu.Add(customStyleBtn);

                // Create custom driving style checkboxes
                CreateCustomDrivingStyleMenu();

                var startDrivingWaypoint = new NativeItem("Drive To Waypoint", "Make your player ped drive your vehicle to your waypoint.");
                startDrivingWaypoint.Activated += (sender, e) =>
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        if (Game.PlayerPed.CurrentVehicle.Driver == Game.PlayerPed)
                        {
                            if (IsWaypointActive())
                            {
                                var style = GetStyleFromIndex(DrivingStyleIndex);
                                DriveToWp(style);
                                Notify.Info("Your player ped is now driving to your waypoint.");
                            }
                            else
                            {
                                Notify.Error("You need a waypoint before you can drive to it!");
                            }
                        }
                        else
                        {
                            Notify.Error("You must be the driver of this vehicle!");
                        }
                    }
                    else
                    {
                        Notify.Error("You need to be in a vehicle first!");
                    }
                };
                vehicleAutoPilotMenu.Add(startDrivingWaypoint);

                var startDrivingRandomly = new NativeItem("Drive Around Randomly", "Make your player ped drive your vehicle randomly around the map.");
                startDrivingRandomly.Activated += (sender, e) =>
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        if (Game.PlayerPed.CurrentVehicle.Driver == Game.PlayerPed)
                        {
                            var style = GetStyleFromIndex(DrivingStyleIndex);
                            DriveWander(style);
                            Notify.Info("Your player ped is now driving around randomly.");
                        }
                        else
                        {
                            Notify.Error("You must be the driver of this vehicle!");
                        }
                    }
                    else
                    {
                        Notify.Error("You need to be in a vehicle first!");
                    }
                };
                vehicleAutoPilotMenu.Add(startDrivingRandomly);

                var stopDriving = new NativeItem("Stop Driving", "The player ped will find a suitable place to stop the vehicle.");
                stopDriving.Activated += async (sender, e) =>
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        var veh = Game.PlayerPed.CurrentVehicle;
                        if (veh != null && veh.Exists())
                        {
                            var outPos = new Vector3();
                            if (GetNthClosestVehicleNode(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, 3, ref outPos, 0, 0, 0))
                            {
                                Notify.Info("Finding a suitable place to park...");
                                ClearPedTasks(Game.PlayerPed.Handle);
                                TaskVehiclePark(Game.PlayerPed.Handle, veh.Handle, outPos.X, outPos.Y, outPos.Z, Game.PlayerPed.Heading, 3, 60f, true);
                                while (Game.PlayerPed.Position.DistanceToSquared2D(outPos) > 3f)
                                {
                                    await BaseScript.Delay(0);
                                }
                                SetVehicleHalt(veh.Handle, 3f, 0, false);
                                ClearPedTasks(Game.PlayerPed.Handle);
                                Notify.Info("The player ped has stopped driving.");
                            }
                        }
                    }
                    else
                    {
                        ClearPedTasks(Game.PlayerPed.Handle);
                    }
                };
                vehicleAutoPilotMenu.Add(stopDriving);

                var forceStopDriving = new NativeItem("Force Stop Driving", "This will stop the driving task immediately.");
                forceStopDriving.Activated += (sender, e) =>
                {
                    ClearPedTasks(Game.PlayerPed.Handle);
                    Notify.Info("Driving task cancelled.");
                };
                vehicleAutoPilotMenu.Add(forceStopDriving);
            }

            #endregion

            #region Player Scenarios

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POScenarios))
            {
                var playerScenarios = new NativeListItem<string>("Player Scenarios", "Select a scenario and hit enter to start it.", PedScenarios.Scenarios.ToArray());
                playerScenarios.ItemChanged += (sender, e) =>
                {
                    CurrentScenarioIndex = PedScenarios.Scenarios.IndexOf(e.Object);
                    PlayScenario(PedScenarios.ScenarioNames[PedScenarios.Scenarios[CurrentScenarioIndex]]);
                };
                menu.Add(playerScenarios);

                var stopScenario = new NativeItem("Force Stop Scenario", "This will force a playing scenario to stop immediately.");
                stopScenario.Activated += (sender, e) =>
                {
                    ClearPedTasksImmediately(Game.PlayerPed.Handle);
                    Notify.Info("Scenario stopped.");
                };
                menu.Add(stopScenario);
            }

            #endregion

            // Freeze Player checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POFreeze))
            {
                var freezeCheckbox = new NativeCheckboxItem("Freeze Player", "Freezes your current location.", PlayerFrozen);
                freezeCheckbox.CheckboxChanged += (sender, e) =>
                {
                    PlayerFrozen = freezeCheckbox.Checked;
                    FreezeEntityPosition(Game.PlayerPed.Handle, PlayerFrozen);
                };
                menu.Add(freezeCheckbox);
            }
        }

        private void CreateCustomDrivingStyleMenu()
        {
            var knownNames = new Dictionary<int, string>
            {
                { 0, "Stop for vehicles" },
                { 1, "Stop for pedestrians" },
                { 2, "Swerve around all vehicles" },
                { 3, "Steer around stationary vehicles" },
                { 4, "Steer around pedestrians" },
                { 5, "Steer around objects" },
                { 6, "Don't steer around player pedestrian" },
                { 7, "Stop at traffic lights" },
                { 8, "Go off-road when avoiding" },
                { 9, "Allow going wrong way" },
                { 10, "Go in reverse gear" },
                { 11, "Use wander fallback" },
                { 12, "Avoid restricted areas" },
                { 13, "Prevent background pathfinding" },
                { 14, "Adjust cruise speed based on road" },
                { 18, "Use shortcut links" },
                { 19, "Change lanes around obstructions" },
                { 21, "Use switched-off nodes" },
                { 22, "Prefer navmesh route" },
                { 23, "Plane taxi mode" },
                { 24, "Force straight line" },
                { 25, "Use string pulling at junctions" },
                { 29, "Avoid highways (if possible)" },
                { 30, "Force join in road direction" }
            };

            for (var i = 0; i < 31; i++)
            {
                var name = knownNames.ContainsKey(i) ? knownNames[i] : "~r~Unknown Flag";
                var checkbox = new NativeCheckboxItem(name, "Toggle this driving style flag.", false);
                var index = i;
                checkbox.CheckboxChanged += (sender, e) =>
                {
                    customDrivingStyleFlags[index] = checkbox.Checked ? 1 : 0;
                    if (DrivingStyleIndex == 4)
                    {
                        var style = GetCustomDrivingStyle();
                        customDrivingStyleMenu.Name = $"Custom style: {style}";
                        SetDriveTaskDrivingStyle(Game.PlayerPed.Handle, style);
                        Notify.Custom("Driving style updated.");
                    }
                };
                customDrivingStyleMenu.Add(checkbox);
            }
        }

        private int GetCustomDrivingStyle()
        {
            var binaryString = "";
            for (var i = 30; i >= 0; i--)
            {
                binaryString += customDrivingStyleFlags[i];
            }
            return (int)Convert.ToUInt32(binaryString, 2);
        }

        private int GetStyleFromIndex(int index)
        {
            return index switch
            {
                0 => 443,      // normal
                1 => 575,      // rushed
                2 => 536871355, // Avoid highways
                3 => 1467,     // Go in reverse
                4 => GetCustomDrivingStyle(), // custom
                _ => 0
            };
        }

        private void PlayScenario(string scenarioName)
        {
            if (IsPedUsingScenario(Game.PlayerPed.Handle, scenarioName))
            {
                ClearPedTasksImmediately(Game.PlayerPed.Handle);
                return;
            }
            ClearPedTasksImmediately(Game.PlayerPed.Handle);
            TaskStartScenarioInPlace(Game.PlayerPed.Handle, scenarioName, 0, true);
        }

        private void DriveToWp(int style)
        {
            if (!IsWaypointActive()) return;

            var waypoint = GetBlipInfoIdCoord(GetFirstBlipInfoId(8));
            var veh = Game.PlayerPed.CurrentVehicle;

            ClearPedTasks(Game.PlayerPed.Handle);
            TaskVehicleDriveToCoordLongrange(Game.PlayerPed.Handle, veh.Handle, waypoint.X, waypoint.Y, waypoint.Z, 30f, style, 10f);
        }

        private void DriveWander(int style)
        {
            var veh = Game.PlayerPed.CurrentVehicle;
            ClearPedTasks(Game.PlayerPed.Handle);
            TaskVehicleDriveWander(Game.PlayerPed.Handle, veh.Handle, 25f, style);
        }

        private async Task CommitSuicide()
        {
            if (HasPedGotWeapon(Game.PlayerPed.Handle, (uint)GetHashKey("WEAPON_PISTOL"), false))
            {
                SetCurrentPedWeapon(Game.PlayerPed.Handle, (uint)GetHashKey("WEAPON_PISTOL"), true);
                await BaseScript.Delay(500);
                TaskPlayAnim(Game.PlayerPed.Handle, "MP_SUICIDE", "PILL", 8f, -8f, -1, 0, 0, false, false, false);
                await BaseScript.Delay(3000);
                SetPedShootsAtCoord(Game.PlayerPed.Handle, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z + 1f, true);
                Game.PlayerPed.Kill();
            }
            else
            {
                RequestAnimDict("MP_SUICIDE");
                while (!HasAnimDictLoaded("MP_SUICIDE"))
                {
                    await BaseScript.Delay(0);
                }
                TaskPlayAnim(Game.PlayerPed.Handle, "MP_SUICIDE", "PILL", 8f, -8f, -1, 0, 0, false, false, false);
                await BaseScript.Delay(2500);
                Game.PlayerPed.Kill();
            }
        }

        public NativeMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }

        public NativeMenu GetAutoPilotMenu() => vehicleAutoPilotMenu;
        public NativeMenu GetCustomDrivingStyleMenu() => customDrivingStyleMenu;
    }
}

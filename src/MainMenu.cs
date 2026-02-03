using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI;
using LemonUI.Menus;

namespace CBPSMenu
{
    public class MainMenu : BaseScript
    {
        private ObjectPool _pool;
        private NativeMenu _mainMenu;
        private NativeMenu _playerMenu;
        private NativeMenu _vehicleMenu;
        private NativeMenu _weaponsMenu;
        private NativeMenu _worldMenu;
        private NativeMenu _settingsMenu;

        // Player state tracking
        private bool _godMode = false;
        private bool _invisible = false;
        private bool _noclip = false;
        private bool _superJump = false;
        private bool _fastRun = false;
        private bool _infiniteAmmo = false;
        private bool _noReload = false;
        private bool _vehicleInvincible = false;

        // Theme colors
        private System.Drawing.Color _bannerColor = System.Drawing.Color.FromArgb(255, 0, 120, 215);

        public MainMenu()
        {
            _pool = new ObjectPool();
            
            CreateMainMenu();
            CreatePlayerMenu();
            CreateVehicleMenu();
            CreateWeaponsMenu();
            CreateWorldMenu();
            CreateSettingsMenu();

            Tick += OnTick;
            
            // Register command to open menu
            API.RegisterCommand("cbps_menu", new Action<int, List<object>, string>((source, args, raw) =>
            {
                ToggleMenu();
            }), false);

            // Register noclip command
            API.RegisterCommand("cbps_noclip", new Action<int, List<object>, string>((source, args, raw) =>
            {
                ToggleNoclip();
            }), false);

            // Register reset command
            API.RegisterCommand("cbps_reset", new Action<int, List<object>, string>((source, args, raw) =>
            {
                ResetPlayerState();
            }), false);

            // Register key mappings
            API.RegisterKeyMapping("cbps_menu", "Open CBPS Menu", "keyboard", "F1");
            API.RegisterKeyMapping("cbps_noclip", "Toggle Noclip", "keyboard", "F2");
            API.RegisterKeyMapping("cbps_reset", "Reset Player State", "keyboard", "F9");

            Debug.WriteLine("[CBPS Menu] Menu initialized successfully!");
        }

        private void CreateMainMenu()
        {
            _mainMenu = new NativeMenu("CBPS Menu", "Main Menu")
            {
                UseMouse = false
            };
            _mainMenu.Banner.Color = _bannerColor;
            _pool.Add(_mainMenu);
        }

        private void CreatePlayerMenu()
        {
            _playerMenu = new NativeMenu("CBPS Menu", "Player Options")
            {
                UseMouse = false
            };
            _playerMenu.Banner.Color = _bannerColor;
            _pool.Add(_playerMenu);

            // Add submenu to main menu
            var playerSubmenuItem = _mainMenu.AddSubMenu(_playerMenu);
            playerSubmenuItem.Title = "Player Options";
            playerSubmenuItem.Description = "Manage your player";

            // Heal Player
            var healItem = new NativeItem("Heal Player", "Restore health to maximum");
            healItem.Activated += (sender, args) =>
            {
                var playerPed = Game.PlayerPed;
                playerPed.Health = playerPed.MaxHealth;
                ShowNotification("~g~Health restored!");
            };
            _playerMenu.Add(healItem);

            // Give Armor
            var armorItem = new NativeItem("Give Armor", "Give full armor");
            armorItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.Armor = 100;
                ShowNotification("~b~Armor restored!");
            };
            _playerMenu.Add(armorItem);

            // God Mode
            var godModeItem = new NativeCheckboxItem("God Mode", "Toggle invincibility", _godMode);
            godModeItem.CheckboxChanged += (sender, args) =>
            {
                _godMode = godModeItem.Checked;
                Game.PlayerPed.IsInvincible = _godMode;
                ShowNotification(_godMode ? "~g~God Mode: ON" : "~r~God Mode: OFF");
            };
            _playerMenu.Add(godModeItem);

            // Invisible
            var invisibleItem = new NativeCheckboxItem("Invisible", "Toggle invisibility", _invisible);
            invisibleItem.CheckboxChanged += (sender, args) =>
            {
                _invisible = invisibleItem.Checked;
                Game.PlayerPed.IsVisible = !_invisible;
                ShowNotification(_invisible ? "~g~Invisible: ON" : "~r~Invisible: OFF");
            };
            _playerMenu.Add(invisibleItem);

            // Noclip
            var noclipItem = new NativeCheckboxItem("Noclip", "Toggle noclip mode (Use F2 for quick toggle)", _noclip);
            noclipItem.CheckboxChanged += (sender, args) =>
            {
                _noclip = noclipItem.Checked;
                ApplyNoclipState();
            };
            _playerMenu.Add(noclipItem);

            // Super Jump
            var superJumpItem = new NativeCheckboxItem("Super Jump", "Toggle super jump", _superJump);
            superJumpItem.CheckboxChanged += (sender, args) =>
            {
                _superJump = superJumpItem.Checked;
                ShowNotification(_superJump ? "~g~Super Jump: ON" : "~r~Super Jump: OFF");
            };
            _playerMenu.Add(superJumpItem);

            // Fast Run
            var fastRunItem = new NativeCheckboxItem("Fast Run", "Toggle fast run", _fastRun);
            fastRunItem.CheckboxChanged += (sender, args) =>
            {
                _fastRun = fastRunItem.Checked;
                if (!_fastRun)
                {
                    API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, 1.0f);
                }
                ShowNotification(_fastRun ? "~g~Fast Run: ON" : "~r~Fast Run: OFF");
            };
            _playerMenu.Add(fastRunItem);

            // Teleport to Waypoint
            var teleportItem = new NativeItem("Teleport to Waypoint", "Teleport to your map waypoint");
            teleportItem.Activated += async (sender, args) =>
            {
                await TeleportToWaypoint();
            };
            _playerMenu.Add(teleportItem);

            // Clear Wanted Level
            var clearWantedItem = new NativeItem("Clear Wanted Level", "Remove all wanted stars");
            clearWantedItem.Activated += (sender, args) =>
            {
                Game.Player.WantedLevel = 0;
                ShowNotification("~g~Wanted level cleared!");
            };
            _playerMenu.Add(clearWantedItem);

            // Suicide
            var suicideItem = new NativeItem("Suicide", "~r~Kill yourself");
            suicideItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.Health = 0;
                ShowNotification("~r~You have committed suicide");
            };
            _playerMenu.Add(suicideItem);
        }

        private void CreateVehicleMenu()
        {
            _vehicleMenu = new NativeMenu("CBPS Menu", "Vehicle Options")
            {
                UseMouse = false
            };
            _vehicleMenu.Banner.Color = _bannerColor;
            _pool.Add(_vehicleMenu);

            var vehicleSubmenuItem = _mainMenu.AddSubMenu(_vehicleMenu);
            vehicleSubmenuItem.Title = "Vehicle Options";
            vehicleSubmenuItem.Description = "Manage vehicles";

            // Spawn Vehicle
            var spawnItem = new NativeItem("Spawn Vehicle", "Enter vehicle model name to spawn");
            spawnItem.Activated += async (sender, args) =>
            {
                var input = await GetUserInput("Enter vehicle model", "", 32);
                if (!string.IsNullOrEmpty(input))
                {
                    await SpawnVehicle(input);
                }
            };
            _vehicleMenu.Add(spawnItem);

            // Repair Vehicle
            var repairItem = new NativeItem("Repair Vehicle", "Fix current vehicle");
            repairItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    vehicle.Repair();
                    ShowNotification("~g~Vehicle repaired!");
                }
                else
                {
                    ShowNotification("~r~You are not in a vehicle!");
                }
            };
            _vehicleMenu.Add(repairItem);

            // Clean Vehicle
            var cleanItem = new NativeItem("Clean Vehicle", "Clean current vehicle");
            cleanItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    vehicle.DirtLevel = 0f;
                    ShowNotification("~g~Vehicle cleaned!");
                }
                else
                {
                    ShowNotification("~r~You are not in a vehicle!");
                }
            };
            _vehicleMenu.Add(cleanItem);

            // Flip Vehicle
            var flipItem = new NativeItem("Flip Vehicle", "Flip vehicle right-side up");
            flipItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    vehicle.PlaceOnGround();
                    ShowNotification("~g~Vehicle flipped!");
                }
                else
                {
                    ShowNotification("~r~You are not in a vehicle!");
                }
            };
            _vehicleMenu.Add(flipItem);

            // Vehicle Invincible
            var invincibleItem = new NativeCheckboxItem("Vehicle Invincible", "Toggle vehicle invincibility", _vehicleInvincible);
            invincibleItem.CheckboxChanged += (sender, args) =>
            {
                _vehicleInvincible = invincibleItem.Checked;
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    vehicle.IsInvincible = _vehicleInvincible;
                    ShowNotification(_vehicleInvincible ? "~g~Vehicle Invincible: ON" : "~r~Vehicle Invincible: OFF");
                }
                else
                {
                    ShowNotification("~r~You are not in a vehicle!");
                    invincibleItem.Checked = false;
                    _vehicleInvincible = false;
                }
            };
            _vehicleMenu.Add(invincibleItem);

            // Delete Vehicle
            var deleteItem = new NativeItem("Delete Vehicle", "~r~Delete current vehicle");
            deleteItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    vehicle.Delete();
                    ShowNotification("~r~Vehicle deleted!");
                }
                else
                {
                    ShowNotification("~r~You are not in a vehicle!");
                }
            };
            _vehicleMenu.Add(deleteItem);
        }

        private void CreateWeaponsMenu()
        {
            _weaponsMenu = new NativeMenu("CBPS Menu", "Weapon Options")
            {
                UseMouse = false
            };
            _weaponsMenu.Banner.Color = _bannerColor;
            _pool.Add(_weaponsMenu);

            var weaponsSubmenuItem = _mainMenu.AddSubMenu(_weaponsMenu);
            weaponsSubmenuItem.Title = "Weapon Options";
            weaponsSubmenuItem.Description = "Manage weapons";

            // Give All Weapons
            var giveAllItem = new NativeItem("Give All Weapons", "Give all available weapons");
            giveAllItem.Activated += (sender, args) =>
            {
                foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                {
                    Game.PlayerPed.Weapons.Give(weapon, 999, false, true);
                }
                ShowNotification("~g~All weapons given!");
            };
            _weaponsMenu.Add(giveAllItem);

            // Remove All Weapons
            var removeAllItem = new NativeItem("Remove All Weapons", "~r~Remove all weapons");
            removeAllItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.Weapons.RemoveAll();
                ShowNotification("~r~All weapons removed!");
            };
            _weaponsMenu.Add(removeAllItem);

            // Infinite Ammo
            var infiniteAmmoItem = new NativeCheckboxItem("Infinite Ammo", "Toggle infinite ammo", _infiniteAmmo);
            infiniteAmmoItem.CheckboxChanged += (sender, args) =>
            {
                _infiniteAmmo = infiniteAmmoItem.Checked;
                ShowNotification(_infiniteAmmo ? "~g~Infinite Ammo: ON" : "~r~Infinite Ammo: OFF");
            };
            _weaponsMenu.Add(infiniteAmmoItem);

            // No Reload
            var noReloadItem = new NativeCheckboxItem("No Reload", "Toggle no reload", _noReload);
            noReloadItem.CheckboxChanged += (sender, args) =>
            {
                _noReload = noReloadItem.Checked;
                ShowNotification(_noReload ? "~g~No Reload: ON" : "~r~No Reload: OFF");
            };
            _weaponsMenu.Add(noReloadItem);
        }

        private void CreateWorldMenu()
        {
            _worldMenu = new NativeMenu("CBPS Menu", "World Options")
            {
                UseMouse = false
            };
            _worldMenu.Banner.Color = _bannerColor;
            _pool.Add(_worldMenu);

            var worldSubmenuItem = _mainMenu.AddSubMenu(_worldMenu);
            worldSubmenuItem.Title = "World Options";
            worldSubmenuItem.Description = "Change world settings";

            // Weather list
            var weatherItem = new NativeListItem<string>("Weather", "Change the weather",
                "EXTRASUNNY", "CLEAR", "CLOUDS", "OVERCAST", "RAIN",
                "THUNDER", "CLEARING", "NEUTRAL", "SNOW", "BLIZZARD",
                "SNOWLIGHT", "XMAS");
            weatherItem.ItemChanged += (sender, args) =>
            {
                var weather = weatherItem.SelectedItem;
                API.SetWeatherTypeNowPersist(weather);
                ShowNotification($"~b~Weather changed to: {weather}");
            };
            _worldMenu.Add(weatherItem);

            // Set Time
            var timeItem = new NativeItem("Set Time", "Change the time of day");
            timeItem.Activated += async (sender, args) =>
            {
                var input = await GetUserInput("Enter hour (0-23)", "", 2);
                if (int.TryParse(input, out int hour) && hour >= 0 && hour <= 23)
                {
                    API.NetworkOverrideClockTime(hour, 0, 0);
                    ShowNotification($"~b~Time set to: {hour}:00");
                }
                else
                {
                    ShowNotification("~r~Invalid hour!");
                }
            };
            _worldMenu.Add(timeItem);
        }

        private void CreateSettingsMenu()
        {
            _settingsMenu = new NativeMenu("CBPS Menu", "Settings")
            {
                UseMouse = false
            };
            _settingsMenu.Banner.Color = _bannerColor;
            _pool.Add(_settingsMenu);

            var settingsSubmenuItem = _mainMenu.AddSubMenu(_settingsMenu);
            settingsSubmenuItem.Title = "Settings";
            settingsSubmenuItem.Description = "Menu settings";

            // Theme selection
            var themeItem = new NativeListItem<string>("Menu Theme", "Change menu color theme",
                "Blue", "Red", "Green", "Purple", "Orange", "Yellow", "Pink", "Dark");
            themeItem.ItemChanged += (sender, args) =>
            {
                switch (themeItem.SelectedItem)
                {
                    case "Blue": _bannerColor = System.Drawing.Color.FromArgb(255, 0, 120, 215); break;
                    case "Red": _bannerColor = System.Drawing.Color.FromArgb(255, 220, 20, 60); break;
                    case "Green": _bannerColor = System.Drawing.Color.FromArgb(255, 34, 139, 34); break;
                    case "Purple": _bannerColor = System.Drawing.Color.FromArgb(255, 138, 43, 226); break;
                    case "Orange": _bannerColor = System.Drawing.Color.FromArgb(255, 255, 140, 0); break;
                    case "Yellow": _bannerColor = System.Drawing.Color.FromArgb(255, 255, 215, 0); break;
                    case "Pink": _bannerColor = System.Drawing.Color.FromArgb(255, 255, 20, 147); break;
                    case "Dark": _bannerColor = System.Drawing.Color.FromArgb(255, 30, 30, 30); break;
                }
                ApplyThemeToAllMenus();
                ShowNotification($"~g~Theme changed to: {themeItem.SelectedItem}");
            };
            _settingsMenu.Add(themeItem);

            // Keybindings info
            var keybindItem = new NativeItem("View Keybindings", "Show current keybindings");
            keybindItem.Activated += (sender, args) =>
            {
                ShowNotification("~b~Menu: F1 | Noclip: F2 | Reset: F9");
            };
            _settingsMenu.Add(keybindItem);
        }

        private void ApplyThemeToAllMenus()
        {
            _mainMenu.Banner.Color = _bannerColor;
            _playerMenu.Banner.Color = _bannerColor;
            _vehicleMenu.Banner.Color = _bannerColor;
            _weaponsMenu.Banner.Color = _bannerColor;
            _worldMenu.Banner.Color = _bannerColor;
            _settingsMenu.Banner.Color = _bannerColor;
        }

        private async Task OnTick()
        {
            _pool.Process();

            // Handle noclip movement
            if (_noclip)
            {
                await HandleNoclipMovement();
            }

            // Handle super jump
            if (_superJump)
            {
                API.SetSuperJumpThisFrame(Game.Player.Handle);
            }

            // Handle fast run
            if (_fastRun)
            {
                API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, 1.49f);
            }

            // Handle infinite ammo
            if (_infiniteAmmo)
            {
                var weapon = Game.PlayerPed.Weapons.Current;
                if (weapon != null)
                {
                    weapon.Ammo = weapon.MaxAmmo;
                }
            }

            // Handle no reload
            if (_noReload)
            {
                var weapon = Game.PlayerPed.Weapons.Current;
                if (weapon != null)
                {
                    weapon.AmmoInClip = weapon.MaxAmmoInClip;
                }
            }

            await Task.FromResult(0);
        }

        private void ToggleMenu()
        {
            if (_pool.AreAnyVisible)
            {
                _pool.HideAll();
            }
            else
            {
                _mainMenu.Visible = true;
            }
        }

        private void ToggleNoclip()
        {
            _noclip = !_noclip;
            ApplyNoclipState();
            
            // Update checkbox if menu is visible
            foreach (var item in _playerMenu.Items)
            {
                if (item is NativeCheckboxItem checkbox && checkbox.Title == "Noclip")
                {
                    checkbox.Checked = _noclip;
                    break;
                }
            }
        }

        private void ApplyNoclipState()
        {
            var playerPed = Game.PlayerPed;

            if (_noclip)
            {
                playerPed.IsInvincible = true;
                playerPed.IsVisible = false;
                API.SetEntityCollision(playerPed.Handle, false, false);
                API.FreezeEntityPosition(playerPed.Handle, true);
                ShowNotification("~g~Noclip: ON");
            }
            else
            {
                // Restore collision first
                API.SetEntityCollision(playerPed.Handle, true, true);
                API.FreezeEntityPosition(playerPed.Handle, false);
                // Restore invincibility based on godMode state
                playerPed.IsInvincible = _godMode;
                // Restore visibility based on invisible state
                playerPed.IsVisible = !_invisible;
                ShowNotification("~r~Noclip: OFF");
            }
        }

        private async Task HandleNoclipMovement()
        {
            var playerPed = Game.PlayerPed;
            var position = playerPed.Position;
            var speed = 1.0f;

            // Shift for faster movement
            if (API.IsControlPressed(0, 21))
            {
                speed = 5.0f;
            }

            // W - Forward
            if (API.IsControlPressed(0, 32))
            {
                var forward = API.GetEntityForwardVector(playerPed.Handle);
                position += new Vector3(forward.X, forward.Y, 0) * speed;
            }

            // S - Backward
            if (API.IsControlPressed(0, 33))
            {
                var forward = API.GetEntityForwardVector(playerPed.Handle);
                position -= new Vector3(forward.X, forward.Y, 0) * speed;
            }

            // A - Rotate left
            if (API.IsControlPressed(0, 34))
            {
                playerPed.Heading += 3.0f;
            }

            // D - Rotate right
            if (API.IsControlPressed(0, 35))
            {
                playerPed.Heading -= 3.0f;
            }

            // Q - Down
            if (API.IsControlPressed(0, 44))
            {
                position.Z -= speed;
            }

            // E - Up
            if (API.IsControlPressed(0, 38))
            {
                position.Z += speed;
            }

            playerPed.Position = position;

            await Task.FromResult(0);
        }

        private void ResetPlayerState()
        {
            var playerPed = Game.PlayerPed;

            // Reset all toggles
            _noclip = false;
            _godMode = false;
            _invisible = false;
            _superJump = false;
            _fastRun = false;

            // Restore player to normal state
            API.SetEntityCollision(playerPed.Handle, true, true);
            API.FreezeEntityPosition(playerPed.Handle, false);
            playerPed.IsInvincible = false;
            playerPed.IsVisible = true;
            API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, 1.0f);

            // Update checkboxes in menu
            foreach (var item in _playerMenu.Items)
            {
                if (item is NativeCheckboxItem checkbox)
                {
                    checkbox.Checked = false;
                }
            }

            ShowNotification("~g~Player state reset!");
        }

        private async Task TeleportToWaypoint()
        {
            var waypoint = World.WaypointPosition;
            if (waypoint == Vector3.Zero)
            {
                ShowNotification("~r~No waypoint set!");
                return;
            }

            var playerPed = Game.PlayerPed;
            
            // Try to get ground Z
            float groundZ = 0f;
            bool found = false;
            
            for (float z = 1000f; z >= 0f && !found; z -= 25f)
            {
                var testPos = new Vector3(waypoint.X, waypoint.Y, z);
                API.RequestCollisionAtCoord(testPos.X, testPos.Y, testPos.Z);
                await Delay(50);
                
                float resultZ = 0f;
                if (API.GetGroundZFor_3dCoord(testPos.X, testPos.Y, testPos.Z, ref resultZ, false))
                {
                    groundZ = resultZ;
                    found = true;
                }
            }

            if (!found)
            {
                groundZ = waypoint.Z;
            }

            playerPed.Position = new Vector3(waypoint.X, waypoint.Y, groundZ + 1f);
            ShowNotification("~g~Teleported to waypoint!");
        }

        private async Task SpawnVehicle(string modelName)
        {
            var model = new Model(modelName);

            if (!model.IsValid || !model.IsVehicle)
            {
                ShowNotification($"~r~Invalid vehicle model: {modelName}");
                return;
            }

            await model.Request(10000);

            if (!model.IsLoaded)
            {
                ShowNotification("~r~Failed to load vehicle model!");
                return;
            }

            var playerPed = Game.PlayerPed;
            var position = playerPed.Position + playerPed.ForwardVector * 5f;
            var heading = playerPed.Heading;

            var vehicle = await World.CreateVehicle(model, position, heading);
            
            if (vehicle != null)
            {
                playerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                ShowNotification($"~g~Vehicle spawned: {modelName}");
            }
            else
            {
                ShowNotification("~r~Failed to spawn vehicle!");
            }

            model.MarkAsNoLongerNeeded();
        }

        private async Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            API.DisplayOnscreenKeyboard(1, windowTitle, "", defaultText, "", "", "", maxLength);

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                await Delay(0);
            }

            if (API.UpdateOnscreenKeyboard() == 1)
            {
                return API.GetOnscreenKeyboardResult();
            }

            return null;
        }

        private void ShowNotification(string message)
        {
            API.SetNotificationTextEntry("STRING");
            API.AddTextComponentString(message);
            API.DrawNotification(false, true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.UI;

using LemonUI;
using LemonUI.Elements;
using LemonUI.Menus;

using CBPSMenu.Client.Menus;
using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client
{
    /// <summary>
    /// Main entry point for comboom.sucht Menu - Full vMenu Clone.
    /// </summary>
    public class MainMenu : BaseScript
    {
        private const string VERSION = "1.0.0";
        /// <summary>
        /// The object pool that handles all menus.
        /// </summary>
        private readonly ObjectPool pool = new ObjectPool();

        /// <summary>
        /// The main menu instance.
        /// </summary>
        private NativeMenu mainMenu;

        /// <summary>
        /// Submenu instances - ALL vMenu menus.
        /// </summary>
        private OnlinePlayers onlinePlayersMenu;
        private PlayerOptions playerOptionsMenu;
        private VehicleOptions vehicleOptionsMenu;
        private VehicleSpawner vehicleSpawnerMenu;
        private WeaponOptions weaponOptionsMenu;
        private PlayerAppearance playerAppearanceMenu;
        private WeatherOptions weatherOptionsMenu;
        private TimeOptions timeOptionsMenu;
        private MiscSettings miscSettingsMenu;
        private SavedVehicles savedVehiclesMenu;
        private PersonalVehicle personalVehicleMenu;
        private WeaponLoadouts weaponLoadoutsMenu;
        private Recording recordingMenu;
        private BannedPlayers bannedPlayersMenu;
        private TeamsMenu teamsMenu;
        private VoiceSettings voiceSettingsMenu;

        /// <summary>
        /// Whether the menu is currently open.
        /// </summary>
        public static bool IsMenuOpen => _instance?.mainMenu?.Visible ?? false;

        private static MainMenu _instance;
        public static MainMenu Instance => _instance;
        public PlayerList CurrentPlayerList => Players;

        // Tick loop state references
        private PlayerOptions _playerOpts;
        private VehicleOptions _vehicleOpts;
        private MiscSettings _miscSettings;

        /// <summary>
        /// Constructor - initializes the menu system.
        /// </summary>
        public MainMenu()
        {
            _instance = this;

            // Load configuration
            Config.Load();

            // Register event handlers
            EventHandlers["cbps:SetPermissions"] += new Action<string>(OnSetPermissions);
            EventHandlers["onClientResourceStart"] += new Action<string>(OnResourceStart);
            EventHandlers["cbps:ShowNotification"] += new Action<string, string>(OnShowNotification);
            EventHandlers["cbps:SummonToPlayer"] += new Action<float, float, float>(OnSummonToPlayer);
            EventHandlers["cbps:ReceiveBanList"] += new Action<string>(OnReceiveBanList);

            // Register tick handler
            Tick += OnTick;
            Tick += OnPlayerStateTick;

            Debug.WriteLine("[comboom.sucht Menu] Client initialized - Full vMenu Clone.");
        }

        /// <summary>
        /// Called when the resource starts.
        /// </summary>
        private void OnResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName)
            {
                return;
            }

            // Request permissions from server
            TriggerServerEvent("cbps:requestPermissions");

            // Register the menu toggle keybind
            RegisterKeyMapping("cbps_menu_toggle", "Open comboom.sucht Menu", "keyboard", Config.MenuKey);
            RegisterCommand("cbps_menu_toggle", new Action<int, List<object>, string>((source, args, raw) =>
            {
                Debug.WriteLine($"[DEBUG] Toggle Request. Current Visible: {mainMenu?.Visible}");
                // Fallback check included in IsAllowed
                if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPMenu, true))
                {
                    if (mainMenu != null) mainMenu.Visible = !mainMenu.Visible;
                }
            }), false);

            // Register NoClip command
            RegisterKeyMapping("cbps_noclip", "Toggle NoClip", "keyboard", "F2");
            RegisterCommand("cbps_noclip", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (PermissionsManager.IsAllowed(PermissionsManager.Permission.NoClip))
                {
                    Notify.Info("NoClip toggled (placeholder).");
                }
            }), false);
        }

        /// <summary>
        /// Called when permissions are received from the server.
        /// </summary>
        private void OnSetPermissions(string permissions)
        {
            PermissionsManager.SetPermissions(permissions);

            // Pass exports to menus that need them
            TeamsMenu.SetExports(Exports);
            VoiceSettings.SetExports(Exports);

            CreateMenus();
            Debug.WriteLine("[comboom.sucht Menu] Permissions set and menus created.");
        }

        private void OnShowNotification(string type, string message)
        {
            switch (type.ToLower())
            {
                case "success": Notify.Success(message); break;
                case "error": Notify.Error(message); break;
                case "info": Notify.Info(message); break;
                default: Notify.Custom(message); break;
            }
        }

        private void OnSummonToPlayer(float x, float y, float z)
        {
            SetPedCoordsKeepVehicle(Game.PlayerPed.Handle, x, y, z);
            Notify.Info("You have been summoned.");
        }

        private void OnReceiveBanList(string dataString)
        {
            if (bannedPlayersMenu != null)
            {
                try
                {
                    var list = new List<BannedPlayerData>();
                    if (!string.IsNullOrEmpty(dataString))
                    {
                        var entries = dataString.Split(new[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var entry in entries)
                        {
                            var parts = entry.Split('|');
                            if (parts.Length >= 7)
                            {
                                list.Add(new BannedPlayerData
                                {
                                    Name = parts[0],
                                    Identifier = parts[1],
                                    Reason = parts[2],
                                    BanDate = parts[3],
                                    ExpireDate = parts[4],
                                    IsPermanent = bool.Parse(parts[5]),
                                    BannedBy = parts[6]
                                });
                            }
                        }
                    }
                    bannedPlayersMenu.UpdateBanList(list);
                }
                catch { }
            }
        }

        /// <summary>
        /// Creates all menus and submenus.
        /// </summary>
        private void CreateMenus()
        {
            // Prevent duplicate initialization
            if (mainMenu != null)
            {
                return;
            }

            // Create the main menu
            mainMenu = new NativeMenu(Config.MenuTitle, Config.MenuSubtitle);
            ApplyTheme(mainMenu);
            pool.Add(mainMenu);

            // Create submenus based on permissions
            CreateSubmenus();
        }

        /// <summary>
        /// Creates submenus based on player permissions.
        /// </summary>
        private void CreateSubmenus()
        {
            // =====================================================
            // ONLINE PLAYERS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPMenu))
            {
                onlinePlayersMenu = new OnlinePlayers();
                var onlineMenu = onlinePlayersMenu.GetMenu();
                ApplyTheme(onlineMenu);
                pool.Add(onlineMenu);

                var onlinePlayersItem = mainMenu.AddSubMenu(onlineMenu);
                onlinePlayersItem.Title = "Online Players";
                onlinePlayersItem.Description = "Manage online players.";
            }

            // =====================================================
            // PLAYER OPTIONS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.POMenu))
            {
                playerOptionsMenu = new PlayerOptions();
                _playerOpts = playerOptionsMenu;
                var playerMenu = playerOptionsMenu.GetMenu();
                ApplyTheme(playerMenu);
                pool.Add(playerMenu);

                var playerOptionsItem = mainMenu.AddSubMenu(playerMenu);
                playerOptionsItem.Title = "Player Options";
                playerOptionsItem.Description = "Godmode, fast run, wanted level, etc.";

                // Add AutoPilot submenu
                var autoPilotMenu = playerOptionsMenu.GetAutoPilotMenu();
                if (autoPilotMenu != null)
                {
                    ApplyTheme(autoPilotMenu);
                    pool.Add(autoPilotMenu);
                }
            }

            // =====================================================
            // VEHICLE OPTIONS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOMenu))
            {
                vehicleOptionsMenu = new VehicleOptions();
                _vehicleOpts = vehicleOptionsMenu;
                var vehicleMenu = vehicleOptionsMenu.GetMenu();
                ApplyTheme(vehicleMenu);
                pool.Add(vehicleMenu);

                var vehicleOptionsItem = mainMenu.AddSubMenu(vehicleMenu);
                vehicleOptionsItem.Title = "Vehicle Options";
                vehicleOptionsItem.Description = "Godmode, repair, doors, windows, etc.";

                // Add submenus
                AddSubmenusIfNotNull(vehicleOptionsMenu.GetGodMenu(), vehicleOptionsMenu.GetDoorsMenu(), vehicleOptionsMenu.GetWindowsMenu());
            }

            // =====================================================
            // VEHICLE SPAWNER
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VSMenu))
            {
                vehicleSpawnerMenu = new VehicleSpawner();
                var spawnerMenu = vehicleSpawnerMenu.GetMenu();
                ApplyTheme(spawnerMenu);
                pool.Add(spawnerMenu);

                var vehicleSpawnerItem = mainMenu.AddSubMenu(spawnerMenu);
                vehicleSpawnerItem.Title = "Vehicle Spawner";
                vehicleSpawnerItem.Description = "Spawn vehicles by category.";
            }

            // =====================================================
            // SAVED VEHICLES
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.SVMenu))
            {
                savedVehiclesMenu = new SavedVehicles();
                var savedMenu = savedVehiclesMenu.GetMenu();
                ApplyTheme(savedMenu);
                pool.Add(savedMenu);

                var savedVehiclesItem = mainMenu.AddSubMenu(savedMenu);
                savedVehiclesItem.Title = "Saved Vehicles";
                savedVehiclesItem.Description = "Save and spawn your vehicles.";
            }

            // =====================================================
            // PERSONAL VEHICLE
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PVMenu))
            {
                personalVehicleMenu = new PersonalVehicle();
                var personalMenu = personalVehicleMenu.GetMenu();
                ApplyTheme(personalMenu);
                pool.Add(personalMenu);

                var personalVehicleItem = mainMenu.AddSubMenu(personalMenu);
                personalVehicleItem.Title = "Personal Vehicle";
                personalVehicleItem.Description = "Manage your personal vehicle.";
            }

            // =====================================================
            // PLAYER APPEARANCE
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PAMenu))
            {
                playerAppearanceMenu = new PlayerAppearance();
                var appearanceMenu = playerAppearanceMenu.GetMenu();
                ApplyTheme(appearanceMenu);
                pool.Add(appearanceMenu);

                var playerAppearanceItem = mainMenu.AddSubMenu(appearanceMenu);
                playerAppearanceItem.Title = "Player Appearance";
                playerAppearanceItem.Description = "Change ped, customize appearance.";

                // Add submenus
                AddSubmenusIfNotNull(playerAppearanceMenu.GetSpawnPedMenu(), playerAppearanceMenu.GetSavedPedsMenu(), playerAppearanceMenu.GetCustomizationMenu());
            }

            // =====================================================
            // WEAPON OPTIONS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPMenu))
            {
                weaponOptionsMenu = new WeaponOptions();
                var weaponMenu = weaponOptionsMenu.GetMenu();
                ApplyTheme(weaponMenu);
                pool.Add(weaponMenu);

                var weaponOptionsItem = mainMenu.AddSubMenu(weaponMenu);
                weaponOptionsItem.Title = "Weapon Options";
                weaponOptionsItem.Description = "Spawn and manage weapons.";
            }

            // =====================================================
            // WEAPON LOADOUTS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WLMenu))
            {
                weaponLoadoutsMenu = new WeaponLoadouts();
                var loadoutsMenu = weaponLoadoutsMenu.GetMenu();
                ApplyTheme(loadoutsMenu);
                pool.Add(loadoutsMenu);

                var weaponLoadoutsItem = mainMenu.AddSubMenu(loadoutsMenu);
                weaponLoadoutsItem.Title = "Weapon Loadouts";
                weaponLoadoutsItem.Description = "Save and equip weapon loadouts.";
            }

            // =====================================================
            // WEATHER OPTIONS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WOMenu))
            {
                weatherOptionsMenu = new WeatherOptions();
                var weatherMenu = weatherOptionsMenu.GetMenu();
                ApplyTheme(weatherMenu);
                pool.Add(weatherMenu);

                var weatherOptionsItem = mainMenu.AddSubMenu(weatherMenu);
                weatherOptionsItem.Title = "Weather Options";
                weatherOptionsItem.Description = "Change the weather.";
            }

            // =====================================================
            // TIME OPTIONS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.TOMenu))
            {
                timeOptionsMenu = new TimeOptions();
                var timeMenu = timeOptionsMenu.GetMenu();
                ApplyTheme(timeMenu);
                pool.Add(timeMenu);

                var timeOptionsItem = mainMenu.AddSubMenu(timeMenu);
                timeOptionsItem.Title = "Time Options";
                timeOptionsItem.Description = "Change the time.";
            }

            // =====================================================
            // MISC SETTINGS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSMenu))
            {
                miscSettingsMenu = new MiscSettings();
                _miscSettings = miscSettingsMenu;
                var miscMenu = miscSettingsMenu.GetMenu();
                ApplyTheme(miscMenu);
                pool.Add(miscMenu);

                var miscSettingsItem = mainMenu.AddSubMenu(miscMenu);
                miscSettingsItem.Title = "Misc Settings";
                miscSettingsItem.Description = "Teleport, display, and other settings.";
            }

            // =====================================================
            // RECORDING
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.RECMenu))
            {
                recordingMenu = new Recording();
                var recMenu = recordingMenu.GetMenu();
                ApplyTheme(recMenu);
                pool.Add(recMenu);

                var recordingItem = mainMenu.AddSubMenu(recMenu);
                recordingItem.Title = "Recording";
                recordingItem.Description = "Recording and Rockstar Editor.";
            }

            // =====================================================
            // BANNED PLAYERS (Admin Only)
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.OPUnban))
            {
                bannedPlayersMenu = new BannedPlayers();
                var bannedMenu = bannedPlayersMenu.GetMenu();
                ApplyTheme(bannedMenu);
                pool.Add(bannedMenu);

                var bannedPlayersItem = mainMenu.AddSubMenu(bannedMenu);
                bannedPlayersItem.Title = "Banned Players";
                bannedPlayersItem.Description = "View and manage bans.";
            }

            // =====================================================
            // TEAMS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.TMMenu))
            {
                teamsMenu = new TeamsMenu();
                var teamMenu = teamsMenu.GetMenu();
                ApplyTheme(teamMenu);
                pool.Add(teamMenu);

                var teamsMenuItem = mainMenu.AddSubMenu(teamMenu);
                teamsMenuItem.Title = "Teams";
                teamsMenuItem.Description = "Join a team and communicate via radio.";
            }

            // =====================================================
            // VOICE SETTINGS
            // =====================================================
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VCMenu))
            {
                voiceSettingsMenu = new VoiceSettings();
                var voiceMenu = voiceSettingsMenu.GetMenu();
                ApplyTheme(voiceMenu);
                pool.Add(voiceMenu);

                var voiceSettingsItem = mainMenu.AddSubMenu(voiceMenu);
                voiceSettingsItem.Title = "Voice Settings";
                voiceSettingsItem.Description = "Configure voice chat and radio settings.";
            }

            // =====================================================
            // ABOUT
            // =====================================================
            var aboutItem = new NativeItem("About comboom.sucht Menu", "vMenu Clone by comboom.sucht");
            aboutItem.Description = $"Full vMenu clone with 16+ menus. Version {VERSION}";
            mainMenu.Add(aboutItem);
        }

        private void AddSubmenusIfNotNull(params NativeMenu[] menus)
        {
            foreach (var m in menus)
            {
                if (m != null)
                {
                    ApplyTheme(m);
                    pool.Add(m);
                }
            }
        }

        /// <summary>
        /// Applies the configured theme to a menu.
        /// </summary>
        private void ApplyTheme(NativeMenu menu)
        {
            menu.Banner = new ScaledTexture(
                PointF.Empty,
                new SizeF(0, 107),
                Config.BannerDictionary,
                Config.BannerTexture
            );
            menu.Banner.Color = Config.HeaderColor;
            // Note: AccentColor and DescriptionBackColor not available in LemonUI 2.2.0
        }

        /// <summary>
        /// Tick handler - processes the menu pool.
        /// </summary>
        private async Task OnTick()
        {
            pool.Process();
            await Delay(0);
        }

        /// <summary>
        /// Player state tick handler - applies persistent settings.
        /// </summary>
        private async Task OnPlayerStateTick()
        {
            // Super jump
            if (_playerOpts != null && _playerOpts.PlayerSuperJump)
            {
                SetSuperJumpThisFrame(Game.Player.Handle);
            }

            // Stay in vehicle
            if (_playerOpts != null && _playerOpts.PlayerStayInVehicle && Game.PlayerPed.IsInVehicle())
            {
                SetPedCanBeDraggedOut(Game.PlayerPed.Handle, false);
            }

            // Vehicle god mode
            if (_vehicleOpts != null && _vehicleOpts.VehicleGodMode && Game.PlayerPed.IsInVehicle())
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                veh.IsInvincible = true;
                veh.CanBeVisiblyDamaged = false;

                if (_vehicleOpts.VehicleGodEngine)
                {
                    veh.EngineHealth = 1000f;
                }
                if (_vehicleOpts.VehicleGodAutoRepair && veh.HealthFloat < 900f)
                {
                    veh.Repair();
                }
            }

            // Keep vehicle clean
            if (_vehicleOpts != null && _vehicleOpts.VehicleNeverDirty && Game.PlayerPed.IsInVehicle())
            {
                Game.PlayerPed.CurrentVehicle.DirtLevel = 0f;
            }

            // Engine always on
            if (_vehicleOpts != null && _vehicleOpts.VehicleEngineAlwaysOn && Game.PlayerPed.IsInVehicle())
            {
                SetVehicleEngineOn(Game.PlayerPed.CurrentVehicle.Handle, true, true, false);
            }

            // No bike helmet
            if (_vehicleOpts != null && _vehicleOpts.VehicleNoBikeHelmet)
            {
                SetPedHelmet(Game.PlayerPed.Handle, false);
            }

            // Show coordinates
            if (_miscSettings != null && _miscSettings.ShowCoordinates)
            {
                var coords = Game.PlayerPed.Position;
                DrawText2D(0.5f, 0.0f, $"X: {coords.X:F2}  Y: {coords.Y:F2}  Z: {coords.Z:F2}");
            }

            // Show location
            if (_miscSettings != null && _miscSettings.ShowLocation)
            {
                var streetName1 = (uint)0;
                var streetName2 = (uint)0;
                var coords = Game.PlayerPed.Position;
                GetStreetNameAtCoord(coords.X, coords.Y, coords.Z, ref streetName1, ref streetName2);
                var street = GetStreetNameFromHashKey(streetName1);
                var cross = GetStreetNameFromHashKey(streetName2);
                var zone = GetLabelText(GetNameOfZone(coords.X, coords.Y, coords.Z));

                var locationText = string.IsNullOrEmpty(cross) ? $"{street}, {zone}" : $"{street} / {cross}, {zone}";
                DrawText2D(0.5f, 0.025f, locationText);
            }

            await Delay(0);
        }

        private void DrawText2D(float x, float y, string text)
        {
            SetTextFont(0);
            SetTextProportional(true);
            SetTextScale(0.0f, 0.35f);
            SetTextColour(255, 255, 255, 255);
            SetTextDropshadow(0, 0, 0, 0, 255);
            SetTextEdge(1, 0, 0, 0, 255);
            SetTextDropShadow();
            SetTextOutline();
            SetTextCentre(true);
            SetTextEntry("STRING");
            AddTextComponentString(text);
            DrawText(x, y);
        }
    }
}

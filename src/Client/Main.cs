using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI;
using LemonUI.Menus;
using LemonUI.Elements;

using CBPSMenu.Client.Menus;
using CBPSMenu.Client.Managers;
using CBPSMenu.Shared;

namespace CBPSMenu.Client
{
    /// <summary>
    /// Main entry point for CBPS Menu - A vMenu-style menu using LemonUI
    /// </summary>
    public class Main : BaseScript
    {
        #region Variables

        /// <summary>
        /// The LemonUI ObjectPool that manages all menus
        /// </summary>
        public static ObjectPool Pool { get; private set; }

        /// <summary>
        /// The main menu instance
        /// </summary>
        public static NativeMenu MainMenu { get; private set; }

        /// <summary>
        /// Menu key mapping
        /// </summary>
        public static string MenuToggleKey { get; private set; } = "F1";

        /// <summary>
        /// Noclip key mapping
        /// </summary>
        public static string NoClipKey { get; private set; } = "F2";

        /// <summary>
        /// Reset key mapping
        /// </summary>
        public static string ResetKey { get; private set; } = "F9";

        /// <summary>
        /// Whether debug mode is enabled
        /// </summary>
        public static bool DebugMode { get; set; } = false;

        // Menu instances
        public static PlayerMenu PlayerMenuInstance { get; private set; }
        public static VehicleMenu VehicleMenuInstance { get; private set; }
        public static VehicleSpawnerMenu VehicleSpawnerMenuInstance { get; private set; }
        public static SavedVehiclesMenu SavedVehiclesMenuInstance { get; private set; }
        public static WeaponMenu WeaponMenuInstance { get; private set; }
        public static WorldMenu WorldMenuInstance { get; private set; }
        public static VoiceMenu VoiceMenuInstance { get; private set; }
        public static RaceMenu RaceMenuInstance { get; private set; }
        public static SettingsMenu SettingsMenuInstance { get; private set; }
        public static OnlinePlayersMenu OnlinePlayersMenuInstance { get; private set; }
        public static TeamMenu TeamMenuInstance { get; private set; }

        // Manager instances
        public static PlayerManager PlayerManagerInstance { get; private set; }
        public static VehicleManager VehicleManagerInstance { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor - initializes the menu system
        /// </summary>
        public Main()
        {
            // Initialize the LemonUI ObjectPool
            Pool = new ObjectPool();

            // Initialize managers first
            InitializeManagers();

            // Create the main menu
            CreateMainMenu();

            // Initialize all sub-menus
            InitializeMenus();

            // Register key mappings
            RegisterKeyMappings();

            // Register tick handler
            Tick += OnTick;

            // Register team event handlers
            RegisterTeamEvents();

            // Register race event handlers
            RegisterRaceEvents();

            Debug.WriteLine("[comboom.sucht] Menu system initialized successfully!");
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initialize all manager instances
        /// </summary>
        private void InitializeManagers()
        {
            PlayerManagerInstance = new PlayerManager();
            VehicleManagerInstance = new VehicleManager();

            Debug.WriteLine("[comboom.sucht] Managers initialized");
        }

        /// <summary>
        /// Create the main menu with Nord theme
        /// </summary>
        private void CreateMainMenu()
        {
            MainMenu = new NativeMenu("comboom.sucht", "Main Menu")
            {
                UseMouse = false
            };

            // Apply Nord theme
            ThemeManager.ApplyNordTheme(MainMenu);

            Pool.Add(MainMenu);

            Debug.WriteLine("[comboom.sucht] Main menu created");
        }

        /// <summary>
        /// Initialize all sub-menus and add them to the main menu
        /// </summary>
        private void InitializeMenus()
        {
            // Create all menu instances
            PlayerMenuInstance = new PlayerMenu();
            VehicleMenuInstance = new VehicleMenu();
            VehicleSpawnerMenuInstance = new VehicleSpawnerMenu();
            SavedVehiclesMenuInstance = new SavedVehiclesMenu();
            WeaponMenuInstance = new WeaponMenu();
            WorldMenuInstance = new WorldMenu();
            VoiceMenuInstance = new VoiceMenu();
            RaceMenuInstance = new RaceMenu();
            SettingsMenuInstance = new SettingsMenu();
            OnlinePlayersMenuInstance = new OnlinePlayersMenu();
            TeamMenuInstance = new TeamMenu();

            // Add submenus to main menu
            AddSubmenuToMain(PlayerMenuInstance.Menu, "Player Options", "Modify your player settings");
            AddSubmenuToMain(OnlinePlayersMenuInstance.Menu, "Online Players", "View and interact with other players");
            AddSubmenuToMain(VehicleMenuInstance.Menu, "Vehicle Options", "Modify your current vehicle");
            AddSubmenuToMain(VehicleSpawnerMenuInstance.Menu, "Vehicle Spawner", "Spawn vehicles by category");
            AddSubmenuToMain(SavedVehiclesMenuInstance.Menu, "Saved Vehicles", "Manage your saved vehicles");
            AddSubmenuToMain(WeaponMenuInstance.Menu, "Weapon Options", "Modify your weapons");
            AddSubmenuToMain(WorldMenuInstance.Menu, "World Options", "Change time and weather");
            AddSubmenuToMain(VoiceMenuInstance.Menu, "Voice Options", "Manage voice chat settings");
            AddSubmenuToMain(TeamMenuInstance.Menu, "Team Menu", "Team selection and blips");
            AddSubmenuToMain(RaceMenuInstance.Menu, "Race Menu", "Create and join races");
            AddSubmenuToMain(SettingsMenuInstance.Menu, "Settings", "Menu settings and keybindings");

            Debug.WriteLine("[comboom.sucht] All menus initialized");
        }

        /// <summary>
        /// Helper method to add a submenu to the main menu
        /// </summary>
        private void AddSubmenuToMain(NativeMenu submenu, string title, string description)
        {
            Pool.Add(submenu);
            var submenuItem = MainMenu.AddSubMenu(submenu);
            submenuItem.Title = title;
            submenuItem.Description = description;
        }

        /// <summary>
        /// Register all key mappings
        /// </summary>
        private void RegisterKeyMappings()
        {
            // Menu toggle command
            API.RegisterCommand("cbps_menu", new Action<int, List<object>, string>((source, args, raw) =>
            {
                ToggleMenu();
            }), false);

            // Noclip toggle command
            API.RegisterCommand("cbps_noclip", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (PlayerManagerInstance != null)
                {
                    PlayerManagerInstance.ToggleNoclip();
                }
            }), false);

            // Reset player state command
            API.RegisterCommand("cbps_reset", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (PlayerManagerInstance != null)
                {
                    PlayerManagerInstance.ResetPlayerState();
                }
            }), false);

            // Register key mappings
            API.RegisterKeyMapping("cbps_menu", "Open CBPS Menu", "keyboard", MenuToggleKey);
            API.RegisterKeyMapping("cbps_noclip", "Toggle Noclip", "keyboard", NoClipKey);
            API.RegisterKeyMapping("cbps_reset", "Reset Player State", "keyboard", ResetKey);

            Debug.WriteLine("[comboom.sucht] Key mappings registered");
        }

        /// <summary>
        /// Register team-related event handlers from server
        /// </summary>
        private void RegisterTeamEvents()
        {
            // Handle team update from server (when any player changes team)
            EventHandlers["cbps:teamUpdated"] += new Action<int, int>((serverId, teamIndex) =>
            {
                if (TeamMenuInstance != null)
                {
                    TeamMenuInstance.OnTeamUpdated(serverId, teamIndex);
                }
            });

            // Handle player disconnect
            EventHandlers["cbps:playerDisconnected"] += new Action<int>((serverId) =>
            {
                if (TeamMenuInstance != null)
                {
                    TeamMenuInstance.OnPlayerDisconnect(serverId);
                }
            });

            // Handle full team data sync from server
            EventHandlers["cbps:teamDataSync"] += new Action<IDictionary<string, object>>((data) =>
            {
                if (TeamMenuInstance != null)
                {
                    var teamData = new System.Collections.Generic.Dictionary<int, int>();
                    foreach (var kvp in data)
                    {
                        teamData[Convert.ToInt32(kvp.Key)] = Convert.ToInt32(kvp.Value);
                    }
                    TeamMenuInstance.OnTeamDataReceived(teamData);
                }
            });

            Debug.WriteLine("[comboom.sucht] Team event handlers registered");
        }

        /// <summary>
        /// Register race-related event handlers from server
        /// </summary>
        private void RegisterRaceEvents()
        {
            // Race created by server
            EventHandlers["cbps:raceCreated"] += new Action<int>((raceId) =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnRaceCreated(raceId);
                }
            });

            // Receive saved race templates from server
            EventHandlers["cbps:receiveSavedRaceTemplates"] += new Action<dynamic>((templates) =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnReceiveSavedTemplates(templates);
                }
            });

            // Race template loaded from server
            EventHandlers["cbps:raceTemplateLoaded"] += new Action<dynamic>((checkpoints) =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnRaceTemplateLoaded(checkpoints);
                }
            });

            // Joined a race
            EventHandlers["cbps:joinedRace"] += new Action<int, dynamic>((raceId, checkpoints) =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnJoinedRace(raceId, checkpoints);
                }
            });

            // Race started
            EventHandlers["cbps:raceStarted"] += new Action<int>((countdown) =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnRaceStarted(countdown);
                }
            });

            // Checkpoint reached
            EventHandlers["cbps:checkpointReached"] += new Action<int>((checkpointNum) =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnCheckpointReached(checkpointNum);
                }
            });

            // Race finished
            EventHandlers["cbps:raceFinished"] += new Action<int, long>((position, time) =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnRaceFinished(position, time);
                }
            });

            // Left race
            EventHandlers["cbps:leftRace"] += new Action(() =>
            {
                if (RaceMenuInstance != null)
                {
                    RaceMenuInstance.OnLeftRace();
                }
            });

            Debug.WriteLine("[comboom.sucht] Race event handlers registered");
        }

        #endregion

        #region Tick Handler

        /// <summary>
        /// Main tick handler - processes menus and managers
        /// </summary>
        private async Task OnTick()
        {
            // Process the LemonUI ObjectPool
            Pool.Process();

            // Process managers
            await PlayerManagerInstance.ProcessTick();
            await VehicleManagerInstance.ProcessTick();

            // Process team menu (blips and nametags)
            if (TeamMenuInstance != null)
            {
                await TeamMenuInstance.ProcessTick();
            }

            await Task.FromResult(0);
        }

        #endregion

        #region Menu Control Methods

        /// <summary>
        /// Toggle the main menu visibility
        /// </summary>
        public static void ToggleMenu()
        {
            if (Pool.AreAnyVisible)
            {
                Pool.HideAll();
            }
            else
            {
                MainMenu.Visible = true;
            }
        }

        /// <summary>
        /// Show a notification to the player
        /// </summary>
        public static void ShowNotification(string message)
        {
            API.SetNotificationTextEntry("STRING");
            API.AddTextComponentString(message);
            API.DrawNotification(false, true);
        }

        /// <summary>
        /// Get user input from on-screen keyboard
        /// </summary>
        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            API.DisplayOnscreenKeyboard(1, windowTitle, "", defaultText, "", "", "", maxLength);

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                await BaseScript.Delay(0);
            }

            if (API.UpdateOnscreenKeyboard() == 1)
            {
                return API.GetOnscreenKeyboardResult();
            }

            return null;
        }

        #endregion
    }
}

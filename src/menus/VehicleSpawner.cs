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
    /// Vehicle Spawner submenu - vMenu clone.
    /// </summary>
    public class VehicleSpawner
    {
        private NativeMenu menu;

        // Spawn options
        public bool SpawnInVehicle { get; private set; } = true;
        public bool ReplaceVehicle { get; private set; } = true;

        private readonly Dictionary<string, List<string>> VehicleClasses = new Dictionary<string, List<string>>
        {
            { "Compacts", new List<string> { "blista", "brioso", "dilettante", "issi2", "panto", "prairie", "rhapsody" } },
            { "Sedans", new List<string> { "asea", "asterope", "cog55", "cognoscenti", "emperor", "fugitive", "glendale", "ingot", "intruder", "premier", "primo", "regina", "schafter2", "stanier", "stratum", "tailgater", "washington" } },
            { "SUVs", new List<string> { "baller", "baller2", "cavalcade", "cavalcade2", "contender", "dubsta", "dubsta2", "fq2", "granger", "gresley", "habanero", "huntley", "landstalker", "mesa", "mesa2", "patriot", "radi", "rocoto", "seminole", "serrano", "xls" } },
            { "Coupes", new List<string> { "cogcabrio", "exemplar", "f620", "felon", "felon2", "jackal", "oracle", "oracle2", "sentinel", "sentinel2", "windsor", "windsor2", "zion", "zion2" } },
            { "Muscle", new List<string> { "blade", "buccaneer", "chino", "coquette3", "dominator", "dukes", "faction", "faction2", "gauntlet", "hermes", "hotknife", "lurcher", "moonbeam", "nightshade", "phoenix", "picador", "ratloader", "ruiner", "sabregt", "slamvan", "stalion", "tampa", "vigero", "virgo", "voodoo" } },
            { "Sports Classics", new List<string> { "casco", "coquette2", "jb700", "manana", "monroe", "peyote", "pigalle", "stinger", "stingergt", "stirling", "tornado", "tornado2", "tornado3", "tornado4", "ztype" } },
            { "Sports", new List<string> { "alpha", "banshee", "blista2", "blista3", "buffalo", "buffalo2", "carbonizzare", "comet2", "coquette", "elegy", "elegy2", "feltzer2", "fusilade", "futo", "jester", "jester2", "khamelion", "kuruma", "kuruma2", "lynx", "massacro", "massacro2", "ninef", "ninef2", "omnis", "penumbra", "rapidgt", "rapidgt2", "schafter3", "schafter4", "schwartzer", "sultan", "surano", "tropos" } },
            { "Super", new List<string> { "adder", "banshee2", "bullet", "cheetah", "entityxf", "fmj", "infernus", "le7b", "osiris", "penetrator", "pfister811", "prototipo", "reaper", "sultanrs", "t20", "turismor", "tyrus", "vacca", "voltic", "voltic2", "zentorno" } },
            { "Motorcycles", new List<string> { "akuma", "avarus", "bagger", "bati", "bati2", "bf400", "carbonrs", "chimera", "cliffhanger", "daemon", "daemon2", "defiler", "double", "enduro", "esskey", "faggio", "faggio2", "faggio3", "gargoyle", "hakuchou", "hakuchou2", "hexer", "innovation", "lectro", "manchez", "nemesis", "nightblade", "pcj", "ratbike", "ruffian", "sanchez", "sanchez2", "sanctus", "shotaro", "sovereign", "thrust", "vader", "vindicator", "vortex", "wolfsbane", "zombiea", "zombieb" } },
            { "Off-Road", new List<string> { "bfinjection", "bifta", "blazer", "blazer2", "blazer3", "blazer4", "blazer5", "bodhi2", "brawler", "dubsta3", "dune", "dune2", "dune3", "dune4", "dune5", "dloader", "insurgent", "insurgent2", "kalahari", "marshall", "mesa3", "monster", "rancherxl", "rebel", "rebel2", "sandking", "sandking2", "technical", "technical2", "trophy", "trophy2" } },
            { "Industrial", new List<string> { "bulldozer", "cutter", "dock_handler", "dump", "flatbed", "guardian", "handler", "mixer", "mixer2", "rubble", "tiptruck", "tiptruck2" } },
            { "Utility", new List<string> { "airtug", "caddy", "caddy2", "caddy3", "docktug", "forklift", "mower", "ripley", "sadler", "sadler2", "scrap", "towtruck", "towtruck2", "tractor", "tractor2", "tractor3", "utillitruck", "utillitruck2", "utillitruck3" } },
            { "Vans", new List<string> { "bison", "bison2", "bison3", "bobcatxl", "boxville", "boxville2", "boxville3", "boxville4", "burrito", "burrito2", "burrito3", "burrito4", "burrito5", "camper", "gburrito", "gburrito2", "journey", "minivan", "minivan2", "paradise", "pony", "pony2", "rumpo", "rumpo2", "rumpo3", "speedo", "speedo2", "surfer", "surfer2", "taco", "youga", "youga2" } },
            { "Cycles", new List<string> { "bmx", "cruiser", "fixter", "scorcher", "tribike", "tribike2", "tribike3" } },
            { "Boats", new List<string> { "dinghy", "dinghy2", "dinghy3", "dinghy4", "jetmax", "marquis", "seashark", "seashark2", "seashark3", "speeder", "speeder2", "squalo", "submersible", "submersible2", "suntrap", "toro", "toro2", "tropic", "tropic2", "tug" } },
            { "Helicopters", new List<string> { "akula", "annihilator", "buzzard", "buzzard2", "cargobob", "cargobob2", "cargobob3", "cargobob4", "frogger", "frogger2", "havok", "hunter", "maverick", "polmav", "savage", "seasparrow", "skylift", "supervolito", "supervolito2", "swift", "swift2", "valkyrie", "valkyrie2", "volatus" } },
            { "Planes", new List<string> { "alphaz1", "avenger", "avenger2", "besra", "blimp", "blimp2", "blimp3", "bombushka", "cargoplane", "cuban800", "dodo", "duster", "howard", "hydra", "jet", "lazer", "luxor", "luxor2", "mammatus", "microlight", "miljet", "mogul", "molotok", "nimbus", "nokota", "pyro", "rogue", "seabreeze", "shamal", "starling", "stunt", "titan", "tula", "velum", "velum2", "vestra", "volatol" } },
            { "Service", new List<string> { "airbus", "brickade", "bus", "coach", "pbus2", "rallytruck", "rentalbus", "taxi", "tourbus", "trash", "trash2" } },
            { "Emergency", new List<string> { "ambulance", "fbi", "fbi2", "firetruk", "lguard", "pbus", "police", "police2", "police3", "police4", "policeb", "policeold1", "policeold2", "policet", "polmav", "pranger", "predator", "riot", "riot2", "sheriff", "sheriff2" } },
            { "Military", new List<string> { "apc", "barracks", "barracks2", "barracks3", "barrage", "chernobog", "crusader", "halftrack", "khanjali", "rhino", "scarab", "scarab2", "scarab3", "thruster", "trailersmall2" } },
            { "Commercial", new List<string> { "benson", "biff", "hauler", "hauler2", "mule", "mule2", "mule3", "packer", "phantom", "phantom2", "phantom3", "pounder", "pounder2", "stockade", "stockade3", "terbyte" } },
            { "Trains", new List<string> { "freight", "freightcar", "freightcont1", "freightcont2", "freightgrain", "tankercar", "metrotrain" } },
        };

        private void CreateMenu()
        {
            menu = new NativeMenu("Vehicle Spawner", "Spawn Vehicles");

            #region Spawn Options

            var spawnInside = new NativeCheckboxItem("Spawn Inside Vehicle", "Automatically get into spawned vehicles.", SpawnInVehicle);
            spawnInside.CheckboxChanged += (s, e) => SpawnInVehicle = spawnInside.Checked;
            menu.Add(spawnInside);

            var replacePrevious = new NativeCheckboxItem("Replace Previous Vehicle", "Delete your previous vehicle when spawning.", ReplaceVehicle);
            replacePrevious.CheckboxChanged += (s, e) => ReplaceVehicle = replacePrevious.Checked;
            menu.Add(replacePrevious);

            #endregion

            #region Spawn By Name

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VSSpawnByName))
            {
                var spawnByName = new NativeItem("Spawn By Name", "Spawn a vehicle by entering its model name.");
                spawnByName.Activated += async (s, e) =>
                {
                    var input = await GetUserInput("Enter vehicle model name", "", 30);
                    if (!string.IsNullOrEmpty(input))
                    {
                        await SpawnVehicle(input);
                    }
                };
                menu.Add(spawnByName);
            }

            #endregion

            #region Vehicle Categories

            foreach (var category in VehicleClasses)
            {
                var categoryPermission = GetCategoryPermission(category.Key);
                if (PermissionsManager.IsAllowed(categoryPermission))
                {
                    var categoryMenu = new NativeMenu(category.Key, $"{category.Key} Vehicles");
                    var categoryBtn = new NativeItem(category.Key, $"Spawn {category.Key.ToLower()} vehicles.") { AltTitle = "→→→" };
                    menu.Add(categoryBtn);

                    foreach (var vehicle in category.Value)
                    {
                        var vehName = vehicle;
                        var vehicleItem = new NativeItem(GetVehicleDisplayName(vehName), $"Spawn {vehName}.");
                        vehicleItem.Activated += async (s, e) =>
                        {
                            await SpawnVehicle(vehName);
                        };
                        categoryMenu.Add(vehicleItem);
                    }
                }
            }

            #endregion
        }

        private PermissionsManager.Permission GetCategoryPermission(string category)
        {
            return category switch
            {
                "Compacts" => PermissionsManager.Permission.VSCompacts,
                "Sedans" => PermissionsManager.Permission.VSSedans,
                "SUVs" => PermissionsManager.Permission.VSSUVs,
                "Coupes" => PermissionsManager.Permission.VSCoupes,
                "Muscle" => PermissionsManager.Permission.VSMuscle,
                "Sports Classics" => PermissionsManager.Permission.VSSportsClassic,
                "Sports" => PermissionsManager.Permission.VSSports,
                "Super" => PermissionsManager.Permission.VSSuper,
                "Motorcycles" => PermissionsManager.Permission.VSMotorcycles,
                "Off-Road" => PermissionsManager.Permission.VSOffRoad,
                "Boats" => PermissionsManager.Permission.VSBoats,
                "Helicopters" => PermissionsManager.Permission.VSHelicopters,
                "Planes" => PermissionsManager.Permission.VSPlanes,
                "Emergency" => PermissionsManager.Permission.VSEmergency,
                "Military" => PermissionsManager.Permission.VSMilitary,
                _ => PermissionsManager.Permission.VSAll
            };
        }

        private string GetVehicleDisplayName(string modelName)
        {
            var hash = (uint)GetHashKey(modelName);
            var name = GetDisplayNameFromVehicleModel(hash);
            if (string.IsNullOrEmpty(name) || name == "CARNOTFOUND")
            {
                return modelName.ToUpper();
            }
            return GetLabelText(name);
        }

        private async Task SpawnVehicle(string modelName)
        {
            var modelHash = (uint)GetHashKey(modelName);

            if (!IsModelInCdimage(modelHash))
            {
                Notify.Error($"Model ~r~{modelName}~s~ not found.");
                return;
            }

            if (!IsModelAVehicle(modelHash))
            {
                Notify.Error($"~r~{modelName}~s~ is not a vehicle model.");
                return;
            }

            RequestModel(modelHash);
            while (!HasModelLoaded(modelHash))
            {
                await BaseScript.Delay(0);
            }

            // Delete previous vehicle if option is set
            if (ReplaceVehicle && Game.PlayerPed.IsInVehicle())
            {
                var oldVeh = Game.PlayerPed.CurrentVehicle;
                Game.PlayerPed.Task.LeaveVehicle();
                await BaseScript.Delay(500);
                oldVeh.Delete();
            }

            var pos = Game.PlayerPed.Position;
            var heading = Game.PlayerPed.Heading;

            var vehicle = new Vehicle(CreateVehicle(modelHash, pos.X, pos.Y, pos.Z, heading, true, false));
            vehicle.PlaceOnGround();
            vehicle.NeedsToBeHotwired = false;
            vehicle.PreviouslyOwnedByPlayer = true;
            vehicle.IsPersistent = true;

            SetModelAsNoLongerNeeded(modelHash);

            if (SpawnInVehicle)
            {
                Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            }

            Notify.Success($"Spawned ~g~{GetVehicleDisplayName(modelName)}~s~.");
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
    }
}

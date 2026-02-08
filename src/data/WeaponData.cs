using System.Collections.Generic;

namespace CBPSMenu.Client.Data
{
    /// <summary>
    /// Weapon data - ported from vMenu/data/ValidWeapons.cs
    /// </summary>
    public static class WeaponData
    {
        public static readonly Dictionary<string, List<WeaponInfo>> WeaponCategories = new Dictionary<string, List<WeaponInfo>>
        {
            { "Melee", new List<WeaponInfo>
                {
                    new WeaponInfo("Knife", "WEAPON_KNIFE"),
                    new WeaponInfo("Nightstick", "WEAPON_NIGHTSTICK"),
                    new WeaponInfo("Hammer", "WEAPON_HAMMER"),
                    new WeaponInfo("Baseball Bat", "WEAPON_BAT"),
                    new WeaponInfo("Golf Club", "WEAPON_GOLFCLUB"),
                    new WeaponInfo("Crowbar", "WEAPON_CROWBAR"),
                    new WeaponInfo("Bottle", "WEAPON_BOTTLE"),
                    new WeaponInfo("Dagger", "WEAPON_DAGGER"),
                    new WeaponInfo("Hatchet", "WEAPON_HATCHET"),
                    new WeaponInfo("Knuckle Dusters", "WEAPON_KNUCKLE"),
                    new WeaponInfo("Machete", "WEAPON_MACHETE"),
                    new WeaponInfo("Flashlight", "WEAPON_FLASHLIGHT"),
                    new WeaponInfo("Switchblade", "WEAPON_SWITCHBLADE"),
                    new WeaponInfo("Pool Cue", "WEAPON_POOLCUE"),
                    new WeaponInfo("Pipe Wrench", "WEAPON_WRENCH"),
                    new WeaponInfo("Battle Axe", "WEAPON_BATTLEAXE"),
                    new WeaponInfo("Stone Hatchet", "WEAPON_STONE_HATCHET"),
                }
            },
            { "Handguns", new List<WeaponInfo>
                {
                    new WeaponInfo("Pistol", "WEAPON_PISTOL"),
                    new WeaponInfo("Pistol Mk II", "WEAPON_PISTOL_MK2"),
                    new WeaponInfo("Combat Pistol", "WEAPON_COMBATPISTOL"),
                    new WeaponInfo("AP Pistol", "WEAPON_APPISTOL"),
                    new WeaponInfo("Stun Gun", "WEAPON_STUNGUN"),
                    new WeaponInfo("Pistol .50", "WEAPON_PISTOL50"),
                    new WeaponInfo("SNS Pistol", "WEAPON_SNSPISTOL"),
                    new WeaponInfo("SNS Pistol Mk II", "WEAPON_SNSPISTOL_MK2"),
                    new WeaponInfo("Heavy Pistol", "WEAPON_HEAVYPISTOL"),
                    new WeaponInfo("Vintage Pistol", "WEAPON_VINTAGEPISTOL"),
                    new WeaponInfo("Flare Gun", "WEAPON_FLAREGUN"),
                    new WeaponInfo("Marksman Pistol", "WEAPON_MARKSMANPISTOL"),
                    new WeaponInfo("Heavy Revolver", "WEAPON_REVOLVER"),
                    new WeaponInfo("Heavy Revolver Mk II", "WEAPON_REVOLVER_MK2"),
                    new WeaponInfo("Double Action Revolver", "WEAPON_DOUBLEACTION"),
                    new WeaponInfo("Up-n-Atomizer", "WEAPON_RAYPISTOL"),
                    new WeaponInfo("Ceramic Pistol", "WEAPON_CERAMICPISTOL"),
                    new WeaponInfo("Navy Revolver", "WEAPON_NAVYREVOLVER"),
                    new WeaponInfo("Perico Pistol", "WEAPON_GADGETPISTOL"),
                }
            },
            { "Submachine Guns", new List<WeaponInfo>
                {
                    new WeaponInfo("Micro SMG", "WEAPON_MICROSMG"),
                    new WeaponInfo("SMG", "WEAPON_SMG"),
                    new WeaponInfo("SMG Mk II", "WEAPON_SMG_MK2"),
                    new WeaponInfo("Assault SMG", "WEAPON_ASSAULTSMG"),
                    new WeaponInfo("Combat PDW", "WEAPON_COMBATPDW"),
                    new WeaponInfo("Machine Pistol", "WEAPON_MACHINEPISTOL"),
                    new WeaponInfo("Mini SMG", "WEAPON_MINISMG"),
                    new WeaponInfo("Unholy Hellbringer", "WEAPON_RAYCARBINE"),
                }
            },
            { "Shotguns", new List<WeaponInfo>
                {
                    new WeaponInfo("Pump Shotgun", "WEAPON_PUMPSHOTGUN"),
                    new WeaponInfo("Pump Shotgun Mk II", "WEAPON_PUMPSHOTGUN_MK2"),
                    new WeaponInfo("Sawed-Off Shotgun", "WEAPON_SAWNOFFSHOTGUN"),
                    new WeaponInfo("Assault Shotgun", "WEAPON_ASSAULTSHOTGUN"),
                    new WeaponInfo("Bullpup Shotgun", "WEAPON_BULLPUPSHOTGUN"),
                    new WeaponInfo("Musket", "WEAPON_MUSKET"),
                    new WeaponInfo("Heavy Shotgun", "WEAPON_HEAVYSHOTGUN"),
                    new WeaponInfo("Double Barrel Shotgun", "WEAPON_DBSHOTGUN"),
                    new WeaponInfo("Sweeper Shotgun", "WEAPON_AUTOSHOTGUN"),
                    new WeaponInfo("Combat Shotgun", "WEAPON_COMBATSHOTGUN"),
                }
            },
            { "Assault Rifles", new List<WeaponInfo>
                {
                    new WeaponInfo("Assault Rifle", "WEAPON_ASSAULTRIFLE"),
                    new WeaponInfo("Assault Rifle Mk II", "WEAPON_ASSAULTRIFLE_MK2"),
                    new WeaponInfo("Carbine Rifle", "WEAPON_CARBINERIFLE"),
                    new WeaponInfo("Carbine Rifle Mk II", "WEAPON_CARBINERIFLE_MK2"),
                    new WeaponInfo("Advanced Rifle", "WEAPON_ADVANCEDRIFLE"),
                    new WeaponInfo("Special Carbine", "WEAPON_SPECIALCARBINE"),
                    new WeaponInfo("Special Carbine Mk II", "WEAPON_SPECIALCARBINE_MK2"),
                    new WeaponInfo("Bullpup Rifle", "WEAPON_BULLPUPRIFLE"),
                    new WeaponInfo("Bullpup Rifle Mk II", "WEAPON_BULLPUPRIFLE_MK2"),
                    new WeaponInfo("Compact Rifle", "WEAPON_COMPACTRIFLE"),
                    new WeaponInfo("Military Rifle", "WEAPON_MILITARYRIFLE"),
                }
            },
            { "Light Machine Guns", new List<WeaponInfo>
                {
                    new WeaponInfo("MG", "WEAPON_MG"),
                    new WeaponInfo("Combat MG", "WEAPON_COMBATMG"),
                    new WeaponInfo("Combat MG Mk II", "WEAPON_COMBATMG_MK2"),
                    new WeaponInfo("Gusenberg Sweeper", "WEAPON_GUSENBERG"),
                }
            },
            { "Sniper Rifles", new List<WeaponInfo>
                {
                    new WeaponInfo("Sniper Rifle", "WEAPON_SNIPERRIFLE"),
                    new WeaponInfo("Heavy Sniper", "WEAPON_HEAVYSNIPER"),
                    new WeaponInfo("Heavy Sniper Mk II", "WEAPON_HEAVYSNIPER_MK2"),
                    new WeaponInfo("Marksman Rifle", "WEAPON_MARKSMANRIFLE"),
                    new WeaponInfo("Marksman Rifle Mk II", "WEAPON_MARKSMANRIFLE_MK2"),
                }
            },
            { "Heavy Weapons", new List<WeaponInfo>
                {
                    new WeaponInfo("RPG", "WEAPON_RPG"),
                    new WeaponInfo("Grenade Launcher", "WEAPON_GRENADELAUNCHER"),
                    new WeaponInfo("Smoke Grenade Launcher", "WEAPON_GRENADELAUNCHER_SMOKE"),
                    new WeaponInfo("Minigun", "WEAPON_MINIGUN"),
                    new WeaponInfo("Firework Launcher", "WEAPON_FIREWORK"),
                    new WeaponInfo("Railgun", "WEAPON_RAILGUN"),
                    new WeaponInfo("Homing Launcher", "WEAPON_HOMINGLAUNCHER"),
                    new WeaponInfo("Compact Grenade Launcher", "WEAPON_COMPACTLAUNCHER"),
                    new WeaponInfo("Widowmaker", "WEAPON_RAYMINIGUN"),
                }
            },
            { "Throwables", new List<WeaponInfo>
                {
                    new WeaponInfo("Grenade", "WEAPON_GRENADE"),
                    new WeaponInfo("BZ Gas", "WEAPON_BZGAS"),
                    new WeaponInfo("Molotov", "WEAPON_MOLOTOV"),
                    new WeaponInfo("Sticky Bomb", "WEAPON_STICKYBOMB"),
                    new WeaponInfo("Proximity Mine", "WEAPON_PROXMINE"),
                    new WeaponInfo("Snowball", "WEAPON_SNOWBALL"),
                    new WeaponInfo("Pipe Bomb", "WEAPON_PIPEBOMB"),
                    new WeaponInfo("Baseball", "WEAPON_BALL"),
                    new WeaponInfo("Tear Gas", "WEAPON_SMOKEGRENADE"),
                    new WeaponInfo("Flare", "WEAPON_FLARE"),
                }
            },
            { "Miscellaneous", new List<WeaponInfo>
                {
                    new WeaponInfo("Jerry Can", "WEAPON_PETROLCAN"),
                    new WeaponInfo("Fire Extinguisher", "WEAPON_FIREEXTINGUISHER"),
                    new WeaponInfo("Parachute", "GADGET_PARACHUTE"),
                    new WeaponInfo("Hazmat Suit", "WEAPON_HAZARDCAN"),
                }
            },
        };

        public static readonly List<string> WeaponTints = new List<string>
        {
            "Normal",
            "Green",
            "Gold",
            "Pink",
            "Army",
            "LSPD",
            "Orange",
            "Platinum"
        };
    }

    public class WeaponInfo
    {
        public string Name { get; set; }
        public string Hash { get; set; }

        public WeaponInfo(string name, string hash)
        {
            Name = name;
            Hash = hash;
        }
    }
}

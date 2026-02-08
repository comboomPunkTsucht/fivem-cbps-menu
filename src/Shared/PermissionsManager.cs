using System;
using System.Collections.Generic;
using System.Linq;

using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Shared
{
    public static class PermissionsManager
    {
        public enum Permission
        {
            // Global
            Everything,
            DontKickMe,
            DontBanMe,
            NoClip,
            Staff,

            // Online Players
            OPMenu,
            OPAll,
            OPTeleport,
            OPWaypoint,
            OPSpectate,
            OPSendMessage,
            OPIdentifiers,
            OPSummon,
            OPKill,
            OPKick,
            OPPermBan,
            OPTempBan,
            OPUnban,

            // Player Options
            POMenu,
            POAll,
            POGod,
            POInvisible,
            POFastRun,
            POFastSwim,
            POSuperjump,
            PONoRagdoll,
            PONeverWanted,
            POSetWanted,
            POClearBlood,
            POIgnored,
            POStayInVehicle,
            POMaxHealth,
            POMaxArmor,
            POCleanPlayer,
            PODryPlayer,
            POWetPlayer,
            POFreeze,
            POScenarios,
            POUnlimitedStamina,
            POSetBlood,
            POVehicleAutoPilotMenu,

            // Vehicle Options
            VOMenu,
            VOAll,
            VOGod,
            VOKeepClean,
            VORepair,
            VOWash,
            VOEngine,
            VODestroyEngine,
            VOSpeedLimiter,
            VOChangePlate,
            VOColors,
            VOFreeze,
            VOInvisible,
            VOFlip,
            VODelete,
            VOFixOrDestroyTires,
            VODoors,
            VOWindows,
            VOLights,
            VOAlarm,
            VOCycleSeats,
            VOBikeSeatbelt,
            VONoSiren,
            VONoHelmet,
            VOTorqueMultiplier,
            VOPowerMultiplier,

            // Vehicle Spawner
            VSMenu,
            VSAll,
            VSSpawnByName,
            VSAddon,
            VSCompacts,
            VSSedans,
            VSSUVs,
            VSCoupes,
            VSMuscle,
            VSSportsClassic,
            VSSports,
            VSSuper,
            VSMotorcycles,
            VSOffRoad,
            VSBoats,
            VSHelicopters,
            VSPlanes,
            VSEmergency,
            VSMilitary,

            // Weapon Options
            WPMenu,
            WPAll,
            WPSpawn,
            WPGetAll,
            WPRemoveAll,
            WPUnlimitedAmmo,
            WPNoReload,
            WPSetAmmo,
            WPSpawnAmmo,
            WPTints,
            WPComponents,

            // Time Options
            TOMenu,
            TOAll,
            TOFreezeTime,
            TOSetTime,

            // Weather Options
            WOMenu,
            WOAll,
            WODynamic,
            WOBlackout,
            WOSetWeather,

            // Misc Settings
            MSMenu,
            MSAll,
            MSTeleportToWp,
            MSTeleportLocations,
            MSTeleportToCoord,
            MSShowCoordinates,
            MSShowLocation,
            MSLockCameraX,
            MSLockCameraY,
            MSNightVision,
            MSThermalVision,
            MSRestoreAppearance,
            MSRestoreWeapons,
            MSClearArea,

            // Teams (New)
            TMMenu,
            TMAll,
            TMJoinTeam,
            TMLeaveTeam,
            TMViewMembers,

            // Voice Settings (New - pma-voice)
            VCMenu,
            VCAll,
            VCSetProximity,
            VCSetRadioChannel,

            // Racing (New)
            RCMenu,
            RCAll,
            RCCreateTrack,
            RCEditTrack,
            RCDeleteTrack,
            RCJoinRace,
            RCStartRace,

            // Player Appearance
            PAMenu,
            PAAll,
            PASpawnPed,
            PASavedPeds,
            PACustomize,
            PAAccessories,

            // Saved Vehicles
            SVMenu,
            SVAll,
            SVSaveVehicle,
            SVSpawnVehicle,
            SVDeleteVehicle,

            // Personal Vehicle
            PVMenu,
            PVAll,
            PVSetPersonal,
            PVSummon,
            PVLock,

            // Weapon Loadouts
            WLMenu,
            WLAll,
            WLSave,
            WLEquip,
            WLDelete,

            // Recording
            RECMenu,
            RECAll,
            RECStart,
            RECEditor,
            RECCamera,

            // Online Players - Unban
            OPUnban,
        }

        public static Dictionary<Permission, bool> Permissions { get; private set; } = new Dictionary<Permission, bool>();
        public static bool ArePermissionsSetup { get; set; } = false;

#if SERVER
        /// <summary>
        /// Public function to check if a permission is allowed (server-side).
        /// </summary>
        public static bool IsAllowed(Permission permission, Player source) => IsAllowedServer(permission, source);

        /// <summary>
        /// Public function to check if a permission is allowed (server-side).
        /// </summary>
        public static bool IsAllowed(Permission permission, string playerHandle) => IsAllowedServer(permission, playerHandle);
#else
        /// <summary>
        /// Public function to check if a permission is allowed (client-side).
        /// </summary>
        public static bool IsAllowed(Permission permission, bool checkAnyway = false) => IsAllowedClient(permission, checkAnyway);

        private static readonly Dictionary<Permission, bool> allowedPerms = new Dictionary<Permission, bool>();

        /// <summary>
        /// Private function that handles client side permission requests.
        /// </summary>
        private static bool IsAllowedClient(Permission permission, bool checkAnyway)
        {
            if (ArePermissionsSetup || checkAnyway)
            {
                var staffPermissionAllowed = (
                    Permissions.ContainsKey(Permission.Staff) && Permissions[Permission.Staff]
                ) || (
                    Permissions.ContainsKey(Permission.Everything) && Permissions[Permission.Everything]
                );

                if (allowedPerms.ContainsKey(permission) && allowedPerms[permission])
                {
                    return true;
                }
                else if (!allowedPerms.ContainsKey(permission))
                {
                    allowedPerms[permission] = false;
                }

                // Get a list of all permissions that are (parents) of the current permission
                var permissionsToCheck = GetPermissionAndParentPermissions(permission);

                // Check if any of those permissions is allowed
                if (permissionsToCheck.Any(p => Permissions.ContainsKey(p) && Permissions[p]))
                {
                    allowedPerms[permission] = true;
                    return true;
                }
            }
            return false;
        }
#endif

#if SERVER
        /// <summary>
        /// Checks if the player is allowed that specific permission.
        /// </summary>
        private static bool IsAllowedServer(Permission permission, Player source)
        {
            if (source == null)
            {
                return false;
            }

            return IsAllowedServer(permission, source.Handle);
        }

        /// <summary>
        /// Checks if the player is allowed that specific permission.
        /// </summary>
        private static bool IsAllowedServer(Permission permission, string playerHandle)
        {
            if (!DoesPlayerExist(playerHandle))
            {
                return false;
            }

            return IsPlayerAceAllowed(playerHandle, GetAceName(permission));
        }
#endif

        private static readonly Dictionary<Permission, List<Permission>> parentPermissions = new Dictionary<Permission, List<Permission>>();

        /// <summary>
        /// Gets the current permission and all parent permissions.
        /// </summary>
        public static List<Permission> GetPermissionAndParentPermissions(Permission permission)
        {
            if (parentPermissions.ContainsKey(permission))
            {
                return parentPermissions[permission];
            }
            else
            {
                var list = new List<Permission>() { Permission.Everything, permission };
                var permStr = permission.ToString();

                // If the first 2 characters are both uppercase
                if (permStr.Length >= 2 && permStr.Substring(0, 2).ToUpper() == permStr.Substring(0, 2))
                {
                    if (permStr.Substring(2) is not ("All" or "Menu"))
                    {
                        list.AddRange(Enum.GetValues(typeof(Permission)).Cast<Permission>().Where(a => a.ToString() == permStr.Substring(0, 2) + "All"));
                    }
                }
                parentPermissions[permission] = list;
                return list;
            }
        }

#if SERVER
        /// <summary>
        /// Sets the permissions for a specific player.
        /// </summary>
        public static void SetPermissionsForPlayer([FromSource] Player player)
        {
            if (player == null)
            {
                return;
            }

            var perms = new Dictionary<Permission, bool>();

            // Loop through all permissions and check if they're allowed
            foreach (var p in Enum.GetValues(typeof(Permission)))
            {
                var permission = (Permission)p;
                if (!perms.ContainsKey(permission))
                {
                    perms.Add(permission, IsAllowed(permission, player));
                }
            }

            // Send the permissions to the client
            player.TriggerEvent("cbps:SetPermissions", Newtonsoft.Json.JsonConvert.SerializeObject(perms));
        }
#else
        /// <summary>
        /// Sets the permission (client side event handler).
        /// </summary>
        public static void SetPermissions(string permissions)
        {
            Permissions = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<Permission, bool>>(permissions);
            ArePermissionsSetup = true;
        }
#endif

#if SERVER
        /// <summary>
        /// Gets the full permission ace name for the specific Permission enum.
        /// </summary>
        private static string GetAceName(Permission permission)
        {
            var name = permission.ToString();
            var prefix = "cbps.";

            switch (name.Substring(0, 2))
            {
                case "OP":
                    prefix += "OnlinePlayers";
                    break;
                case "PO":
                    prefix += "PlayerOptions";
                    break;
                case "VO":
                    prefix += "VehicleOptions";
                    break;
                case "VS":
                    prefix += "VehicleSpawner";
                    break;
                case "TO":
                    prefix += "TimeOptions";
                    break;
                case "WO":
                    prefix += "WeatherOptions";
                    break;
                case "TM":
                    prefix += "Teams";
                    break;
                case "VC":
                    prefix += "VoiceChat";
                    break;
                case "RC":
                    prefix += "Racing";
                    break;
                default:
                    return prefix + name;
            }

            return prefix + "." + name.Substring(2);
        }
#endif
    }
}

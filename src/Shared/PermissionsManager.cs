using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace CBPSMenu.Shared
{
  /// <summary>
  /// Manages ACE-based permissions for menu access control
  /// Simplified version based on vMenu's PermissionsManager
  /// Uses server-side permission checks via events
  /// </summary>
  public static class PermissionsManager
  {
    #region Permission Enum

    /// <summary>
    /// All available permissions in the menu system
    /// </summary>
    public enum Permission
    {
      // Global Permissions
      Everything,
      Staff,
      NoClip,

      // Online Players Menu
      OPMenu,
      OPAll,
      OPTeleport,
      OPSummon,
      OPSpectate,
      OPKill,
      OPKick,
      OPBan,

      // Player Options
      POMenu,
      POAll,
      POGod,
      POInvisible,
      POFastRun,
      POSuperjump,
      PONeverWanted,

      // Vehicle Options
      VOMenu,
      VOAll,
      VOGod,
      VORepair,
      VOSpawn,

      // Weapon Options
      WPMenu,
      WPAll,
      WPSpawn,
      WPUnlimitedAmmo,

      // World Options
      WOMenu,
      WOAll,
      WOSetTime,
      WOSetWeather,

      // Voice Options
      VCMenu,
      VCAll,
    }

    #endregion

#if !SERVER
    #region Permission Cache

    /// <summary>
    /// Cached permissions from server
    /// </summary>
    private static Dictionary<Permission, bool> _cachedPermissions = new Dictionary<Permission, bool>();

    /// <summary>
    /// Whether permissions have been loaded from server
    /// </summary>
    public static bool PermissionsLoaded { get; private set; } = false;

    #endregion

    #region Permission Checking

    /// <summary>
    /// Check if the current player is allowed a specific permission
    /// Uses cached permissions from server
    /// </summary>
    /// <param name="permission">The permission to check</param>
    /// <returns>True if allowed, false otherwise</returns>
    public static bool IsAllowed(Permission permission)
    {
      try
      {
        // If permissions haven't been loaded yet, allow by default (server will validate)
        if (!PermissionsLoaded)
        {
          // Request permissions from server if not loaded
          RequestPermissions();
          return true; // Default to true, server will validate on action
        }

        // Check if player has the "Everything" permission (admin override)
        if (permission != Permission.Everything)
        {
          if (_cachedPermissions.ContainsKey(Permission.Everything) && _cachedPermissions[Permission.Everything])
          {
            return true;
          }
        }

        // Check the "All" permission for the category
        Permission allPerm = GetCategoryAllPermission(permission);
        if (allPerm != permission && _cachedPermissions.ContainsKey(allPerm) && _cachedPermissions[allPerm])
        {
          return true;
        }

        // Check the specific permission
        if (_cachedPermissions.ContainsKey(permission))
        {
          return _cachedPermissions[permission];
        }

        return false;
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[comboom.sucht] Error checking permission {permission}: {ex.Message}");
        return false;
      }
    }

    /// <summary>
    /// Check if the current player has staff permissions
    /// </summary>
    public static bool IsStaff()
    {
      return IsAllowed(Permission.Staff) || IsAllowed(Permission.Everything);
    }

    /// <summary>
    /// Get the "All" permission for a category (e.g., OPTeleport -> OPAll)
    /// </summary>
    private static Permission GetCategoryAllPermission(Permission permission)
    {
      string name = permission.ToString();
      if (name.Length >= 2)
      {
        string prefix = name.Substring(0, 2);
        string allName = prefix + "All";
        if (Enum.TryParse<Permission>(allName, out Permission allPerm))
        {
          return allPerm;
        }
      }
      return permission;
    }

    #endregion

    #region Server Communication

    /// <summary>
    /// Request permissions from the server
    /// </summary>
    public static void RequestPermissions()
    {
      BaseScript.TriggerServerEvent("cbpsMenu:RequestPermissions");
    }

    /// <summary>
    /// Set permissions from server response
    /// Called by event handler in Main.cs
    /// </summary>
    public static void SetPermissions(Dictionary<string, bool> permissions)
    {
      _cachedPermissions.Clear();

      foreach (var kvp in permissions)
      {
        if (Enum.TryParse<Permission>(kvp.Key, out Permission perm))
        {
          _cachedPermissions[perm] = kvp.Value;
        }
      }

      PermissionsLoaded = true;
      Debug.WriteLine($"[comboom.sucht] Loaded {_cachedPermissions.Count} permissions from server");
    }

    /// <summary>
    /// Set a single permission (for testing or manual override)
    /// </summary>
    public static void SetPermission(Permission permission, bool allowed)
    {
      _cachedPermissions[permission] = allowed;
    }

    /// <summary>
    /// Grant all permissions (for testing)
    /// </summary>
    public static void GrantAllPermissions()
    {
      foreach (Permission perm in Enum.GetValues(typeof(Permission)))
      {
        _cachedPermissions[perm] = true;
      }
      PermissionsLoaded = true;
      Debug.WriteLine("[comboom.sucht] Granted all permissions (debug mode)");
    }

    #endregion

    #region ACE Name Conversion

    /// <summary>
    /// Convert a Permission enum to its ACE permission name
    /// Format: cbpsMenu.Category.Permission
    /// </summary>
    public static string GetAceName(Permission permission)
    {
      string name = permission.ToString();
      string prefix = "cbpsMenu.";

      // Determine the category based on the first 2 characters
      if (name.Length >= 2)
      {
        string categoryPrefix = name.Substring(0, 2);

        switch (categoryPrefix)
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
          case "WP":
            prefix += "WeaponOptions";
            break;
          case "WO":
            prefix += "WorldOptions";
            break;
          case "VC":
            prefix += "VoiceChat";
            break;
          default:
            // Global permissions like Everything, Staff, NoClip
            return prefix + name;
        }

        // Add the permission suffix (everything after the 2-char prefix)
        return prefix + "." + name.Substring(2);
      }

      return prefix + name;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Get a list of all permissions the current player has
    /// Useful for debugging
    /// </summary>
    public static List<Permission> GetAllowedPermissions()
    {
      var allowed = new List<Permission>();

      foreach (Permission perm in Enum.GetValues(typeof(Permission)))
      {
        if (IsAllowed(perm))
        {
          allowed.Add(perm);
        }
      }

      return allowed;
    }

    /// <summary>
    /// Debug: Print all permissions to console
    /// </summary>
    public static void DebugPrintPermissions()
    {
      Debug.WriteLine("[comboom.sucht] === Permission Debug ===");
      foreach (Permission perm in Enum.GetValues(typeof(Permission)))
      {
        string aceName = GetAceName(perm);
        bool allowed = IsAllowed(perm);
        Debug.WriteLine($"  {perm} ({aceName}): {(allowed ? "ALLOWED" : "DENIED")}");
      }
      Debug.WriteLine("[comboom.sucht] === End Permission Debug ===");
    }

    #endregion
#endif
  }
}

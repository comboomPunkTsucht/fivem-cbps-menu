using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace CBPSMenu.Server
{
  public class ConfigManager : BaseScript
  {
    private const string ServerDefaultsFile = "server_defaults.json";

    // Dictionary matching the structure of serverDefaults in settings.lua
    private Dictionary<string, object> serverDefaults = new Dictionary<string, object>();
    private Dictionary<string, Dictionary<string, Dictionary<string, object>>> playerSettings = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

    public ConfigManager()
    {
      InitializeDefaults();
      LoadServerDefaults();
      RegisterEvents();
      RegisterCommands();
    }

    private void InitializeDefaults()
    {
      // Mimic the lua structure
      serverDefaults["theme"] = new Dictionary<string, object> { { "current", "blue" }, { "customThemes", new List<object>() } };
      serverDefaults["voice"] = new Dictionary<string, object> { { "defaultRange", 5.0 }, { "currentRangeIndex", 2 } };
      // ... add other sections as needed or load dynamically
    }

    private void RegisterEvents()
    {
      EventHandlers["cbps:requestServerDefaults"] += new Action<Player>(OnRequestServerDefaults);
      EventHandlers["cbps:settingChanged"] += new Action<Player, string, string, object>(OnSettingChanged);
    }

    private void RegisterCommands()
    {
      API.RegisterCommand("cbps_set_default", new Action<int, List<object>, string>((source, args, raw) =>
      {
        if (source != 0) // Valid player
        {
          if (!API.IsPlayerAceAllowed(source.ToString(), "cbps.admin"))
          {
            TriggerClientEvent(Players[source], "cbps:showNotification", "~r~Admin only command");
            return;
          }
        }

        if (args.Count < 3)
        {
          Debug.WriteLine("[CBPS Menu] Usage: cbps_set_default <category> <key> <value>");
          return;
        }

        string category = args[0].ToString();
        string key = args[1].ToString();
        string valueStr = args[2].ToString();
        object value = valueStr;

        if (valueStr == "true") value = true;
        else if (valueStr == "false") value = false;
        else if (double.TryParse(valueStr, out double d)) value = d;

        dynamic catObj;
        if (serverDefaults.TryGetValue(category, out object obj))
        {
          catObj = obj; // Assuming it's a Dictionary or IDictionary
                        // This is tricky with strongly typed C# vs dynamic Lua.
                        // For now, simpler implementation: use dynamic or Dictionary<string, object> nesting.
          ((Dictionary<string, object>)catObj)[key] = value;
        }
        else
        {
          var newCat = new Dictionary<string, object>();
          newCat[key] = value;
          serverDefaults[category] = newCat;
        }

        SaveServerDefaults();
        Debug.WriteLine($"[CBPS Menu] Server default updated: {category}.{key} = {value}");

      }), false);

      API.RegisterCommand("cbps_reset_defaults", new Action<int, List<object>, string>((source, args, raw) =>
      {
        if (source != 0 && !API.IsPlayerAceAllowed(source.ToString(), "cbps.admin")) return;

        SaveServerDefaults();
        Debug.WriteLine("[CBPS Menu] Server defaults reset/saved");
      }), false);
    }

    private void LoadServerDefaults()
    {
      string data = API.LoadResourceFile(API.GetCurrentResourceName(), ServerDefaultsFile);
      if (!string.IsNullOrEmpty(data))
      {
        try
        {
          serverDefaults = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
          Debug.WriteLine("[CBPS Menu] Server defaults loaded");
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"[CBPS Menu] Error loading defaults: {ex.Message}");
        }
      }
      else
      {
        SaveServerDefaults();
      }
    }

    private void SaveServerDefaults()
    {
      string data = JsonConvert.SerializeObject(serverDefaults, Formatting.Indented);
      API.SaveResourceFile(API.GetCurrentResourceName(), ServerDefaultsFile, data, -1);
    }

    private void OnRequestServerDefaults([FromSource] Player player)
    {
      player.TriggerEvent("cbps:receiveServerDefaults", serverDefaults);
    }

    private void OnSettingChanged([FromSource] Player player, string category, string key, object value)
    {
      string identifier = player.Identifiers["license"];
      if (string.IsNullOrEmpty(identifier)) return;

      if (!playerSettings.ContainsKey(identifier))
        playerSettings[identifier] = new Dictionary<string, Dictionary<string, object>>();

      if (!playerSettings[identifier].ContainsKey(category))
        playerSettings[identifier][category] = new Dictionary<string, object>();

      playerSettings[identifier][category][key] = value;

      Debug.WriteLine($"[CBPS Menu] Setting changed for {player.Name}: {category}.{key}");
    }
  }
}

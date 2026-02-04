using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace CBPSMenu.Server
{
  public class RaceManager : BaseScript
  {
    private const string RaceSaveFile = "cbps_races.json";
    private const int CountdownTime = 5;

    private Dictionary<int, Race> races = new Dictionary<int, Race>();
    private Dictionary<int, int> playerRaces = new Dictionary<int, int>(); // playerId -> raceId
    private List<RaceTemplate> savedRaceTemplates = new List<RaceTemplate>();
    private int nextRaceId = 1;

    public RaceManager()
    {
      LoadSavedRaces();
      RegisterEvents();
    }

    private void RegisterEvents()
    {
      EventHandlers["cbps:saveRaceTemplate"] += new Action<Player, string>(OnSaveRaceTemplate);
      EventHandlers["cbps:loadRaceTemplate"] += new Action<Player, int>(OnLoadRaceTemplate);
      EventHandlers["cbps:getSavedRaceTemplates"] += new Action<Player>(OnGetSavedRaceTemplates);
      EventHandlers["cbps:deleteRaceTemplate"] += new Action<Player, int>(OnDeleteRaceTemplate);
      EventHandlers["cbps:createRace"] += new Action<Player>(OnCreateRace);
      EventHandlers["cbps:addRaceCheckpoint"] += new Action<Player, dynamic>(OnAddRaceCheckpoint);
      EventHandlers["cbps:clearRaceCheckpoints"] += new Action<Player>(OnClearRaceCheckpoints);
      EventHandlers["cbps:joinRace"] += new Action<Player>(OnJoinRace);
      EventHandlers["cbps:leaveRace"] += new Action<Player>(OnLeaveRace);
      EventHandlers["cbps:startRace"] += new Action<Player>(OnStartRace);
      EventHandlers["cbps:reachedCheckpoint"] += new Action<Player, int>(OnReachedCheckpoint);
      EventHandlers["cbps:finishRace"] += new Action<Player, long>(OnFinishRace);
      EventHandlers["cbps:playerDropped"] += new Action<string>(OnPlayerDropped); // Triggered by ServerMain
    }

    private void LoadSavedRaces()
    {
      string data = API.LoadResourceFile(API.GetCurrentResourceName(), RaceSaveFile);
      if (!string.IsNullOrEmpty(data))
      {
        try
        {
          savedRaceTemplates = JsonConvert.DeserializeObject<List<RaceTemplate>>(data) ?? new List<RaceTemplate>();
          Debug.WriteLine($"[CBPS Menu] Loaded {savedRaceTemplates.Count} race templates");
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"[CBPS Menu] Error loading saved races: {ex.Message}");
          savedRaceTemplates = new List<RaceTemplate>();
        }
      }
      else
      {
        Debug.WriteLine("[CBPS Menu] No saved races found, starting fresh");
      }
    }

    private void SaveRacesToFile()
    {
      try
      {
        string data = JsonConvert.SerializeObject(savedRaceTemplates, Formatting.Indented);
        API.SaveResourceFile(API.GetCurrentResourceName(), RaceSaveFile, data, -1);
        Debug.WriteLine($"[CBPS Menu] Saved {savedRaceTemplates.Count} race templates");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"[CBPS Menu] Error saving races: {ex.Message}");
      }
    }

    private void OnSaveRaceTemplate([FromSource] Player player, string raceName)
    {
      int playerId = int.Parse(player.Handle);
      if (!playerRaces.ContainsKey(playerId))
      {
        player.TriggerEvent("cbps:showNotification", "~r~No active race to save");
        return;
      }

      int raceId = playerRaces[playerId];
      if (!races.ContainsKey(raceId)) return;

      Race race = races[raceId];
      if (race.CreatorId != playerId)
      {
        player.TriggerEvent("cbps:showNotification", "~r~You can only save races you created");
        return;
      }

      if (race.Checkpoints.Count == 0)
      {
        player.TriggerEvent("cbps:showNotification", "~r~Cannot save race with no checkpoints");
        return;
      }

      var template = new RaceTemplate
      {
        Name = raceName,
        Checkpoints = new List<Vector3Serializable>(race.Checkpoints),
        CreatedBy = player.Name,
        CreatedAt = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
      };

      savedRaceTemplates.Add(template);
      SaveRacesToFile();

      player.TriggerEvent("cbps:showNotification", $"~g~Race template \"{raceName}\" saved!");
      Debug.WriteLine($"[CBPS Menu] Race template \"{raceName}\" saved by {player.Name}");
    }

    private void OnLoadRaceTemplate([FromSource] Player player, int templateIndex)
    {
      // Lua tables are 1-indexed, List is 0-indexed.
      // The client sends the 1-based index from the loop.
      int index = templateIndex - 1;

      if (index < 0 || index >= savedRaceTemplates.Count)
      {
        player.TriggerEvent("cbps:showNotification", "~r~Race template not found");
        return;
      }

      var template = savedRaceTemplates[index];
      int playerId = int.Parse(player.Handle);

      int raceId = nextRaceId++;
      var race = new Race
      {
        Id = raceId,
        CreatorId = playerId,
        Checkpoints = new List<Vector3Serializable>(template.Checkpoints),
        Participants = new List<int> { playerId },
        TemplateName = template.Name
      };

      races[raceId] = race;
      playerRaces[playerId] = raceId;

      player.TriggerEvent("cbps:raceCreated", raceId);

      // Send checkpoints as dynamic object/list for client compatibility
      // Converting Vector3Serializable back to simple objects if needed,
      // but JsonConvert should handle it correctly if client expects {x,y,z}
      player.TriggerEvent("cbps:raceTemplateLoaded", template.Checkpoints);

      player.TriggerEvent("cbps:showNotification", $"~g~Loaded race template: {template.Name}");
      Debug.WriteLine($"[CBPS Menu] Race template \"{template.Name}\" loaded by {player.Name}");
    }

    private void OnGetSavedRaceTemplates([FromSource] Player player)
    {
      // Serialize to anonymous objects for client to match Lua structure if needed
      // But JsonConvert will serialize properties as camelCase if configured or PascalCase default.
      // Lua expects: { name, createdBy, createdAt, checkpoints: [{x,y,z}] }
      // Let's create a list of anonymous objects to ensure lowerCamelCase for properties
      var templates = new List<object>();
      foreach (var t in savedRaceTemplates)
      {
        var checkpoints = new List<object>();
        foreach (var cp in t.Checkpoints)
        {
          checkpoints.Add(new { x = cp.X, y = cp.Y, z = cp.Z });
        }

        templates.Add(new
        {
          name = t.Name,
          createdBy = t.CreatedBy,
          createdAt = t.CreatedAt,
          checkpoints = checkpoints
        });
      }

      player.TriggerEvent("cbps:receiveSavedRaceTemplates", templates);
    }

    private void OnDeleteRaceTemplate([FromSource] Player player, int templateIndex)
    {
      int index = templateIndex - 1;
      if (index < 0 || index >= savedRaceTemplates.Count)
      {
        player.TriggerEvent("cbps:showNotification", "~r~Race template not found");
        return;
      }

      string templateName = savedRaceTemplates[index].Name;
      savedRaceTemplates.RemoveAt(index);
      SaveRacesToFile();

      player.TriggerEvent("cbps:showNotification", $"~g~Deleted race template: {templateName}");
      Debug.WriteLine($"[CBPS Menu] Race template \"{templateName}\" deleted by {player.Name}");
    }

    private void OnCreateRace([FromSource] Player player)
    {
      int playerId = int.Parse(player.Handle);
      int raceId = nextRaceId++;

      var race = new Race
      {
        Id = raceId,
        CreatorId = playerId,
        Participants = new List<int> { playerId }
      };

      races[raceId] = race;
      playerRaces[playerId] = raceId;

      player.TriggerEvent("cbps:raceCreated", raceId);
      Debug.WriteLine($"[CBPS Menu] Race created by {player.Name} (ID: {raceId})");
    }

    private void OnAddRaceCheckpoint([FromSource] Player player, dynamic coords)
    {
      int playerId = int.Parse(player.Handle);
      if (!playerRaces.ContainsKey(playerId)) return;

      int raceId = playerRaces[playerId];
      if (!races.ContainsKey(raceId)) return;

      Race race = races[raceId];
      if (race.CreatorId != playerId) return;

      // Coords comes as ExpandoObject or similar from msgpack
      // We'll trust it functions as a dict or dynamic with x, y, z
      float x, y, z;
      try
      {
        // Handling dynamic input carefully
        var dict = (IDictionary<string, object>)coords;
        x = Convert.ToSingle(dict["x"]);
        y = Convert.ToSingle(dict["y"]);
        z = Convert.ToSingle(dict["z"]);
      }
      catch
      {
        try
        {
          // Fallback check if it's directly properties
          x = (float)coords.x;
          y = (float)coords.y;
          z = (float)coords.z;
        }
        catch { return; }
      }

      race.Checkpoints.Add(new Vector3Serializable(x, y, z));
    }

    private void OnClearRaceCheckpoints([FromSource] Player player)
    {
      int playerId = int.Parse(player.Handle);
      if (!playerRaces.ContainsKey(playerId)) return;

      int raceId = playerRaces[playerId];
      if (!races.ContainsKey(raceId)) return;

      Race race = races[raceId];
      if (race.CreatorId != playerId) return;

      race.Checkpoints.Clear();
    }

    private void OnJoinRace([FromSource] Player player)
    {
      int playerId = int.Parse(player.Handle);

      // Find available race
      Race availableRace = null;
      foreach (var race in races.Values)
      {
        if (!race.Started && race.Checkpoints.Count > 0)
        {
          availableRace = race;
          break;
        }
      }

      if (availableRace == null)
      {
        player.TriggerEvent("cbps:showNotification", "~r~No available races");
        return;
      }

      availableRace.Participants.Add(playerId);
      playerRaces[playerId] = availableRace.Id;

      var checkpoints = new List<object>();
      foreach (var cp in availableRace.Checkpoints)
      {
        checkpoints.Add(new { x = cp.X, y = cp.Y, z = cp.Z });
      }

      player.TriggerEvent("cbps:joinedRace", availableRace.Id, checkpoints);
    }

    private void OnLeaveRace([FromSource] Player player)
    {
      int playerId = int.Parse(player.Handle);
      RemovePlayerFromRace(playerId);
    }

    private void RemovePlayerFromRace(int playerId)
    {
      if (!playerRaces.ContainsKey(playerId)) return;
      int raceId = playerRaces[playerId];

      if (!races.ContainsKey(raceId)) return;
      Race race = races[raceId];

      if (race.Participants.Contains(playerId))
      {
        race.Participants.Remove(playerId);
      }

      playerRaces.Remove(playerId);
      TriggerClientEvent(Players[playerId], "cbps:leftRace");

      if (race.Participants.Count == 0)
      {
        races.Remove(raceId);
        Debug.WriteLine($"[CBPS Menu] Race deleted (ID: {raceId})");
      }
    }

    private void OnStartRace([FromSource] Player player)
    {
      int playerId = int.Parse(player.Handle);
      if (!playerRaces.ContainsKey(playerId)) return;

      int raceId = playerRaces[playerId];
      if (!races.ContainsKey(raceId)) return;

      Race race = races[raceId];
      if (race.CreatorId != playerId) return;

      if (race.Checkpoints.Count == 0)
      {
        player.TriggerEvent("cbps:showNotification", "~r~No checkpoints set");
        return;
      }

      race.Started = true;

      foreach (int participantId in race.Participants)
      {
        Player participant = Players[participantId];
        if (participant != null)
        {
          participant.TriggerEvent("cbps:raceStarted", CountdownTime);
        }
      }

      Debug.WriteLine($"[CBPS Menu] Race started (ID: {raceId})");
    }

    private void OnReachedCheckpoint([FromSource] Player player, int checkpointNum)
    {
      player.TriggerEvent("cbps:checkpointReached", checkpointNum);
    }

    private void OnFinishRace([FromSource] Player player, long time)
    {
      int playerId = int.Parse(player.Handle);
      if (!playerRaces.ContainsKey(playerId)) return;

      int raceId = playerRaces[playerId];
      if (!races.ContainsKey(raceId)) return;

      Race race = races[raceId];

      race.Finished.Add(new RaceResult
      {
        PlayerId = playerId,
        PlayerName = player.Name,
        Time = time
      });

      int position = race.Finished.Count;
      player.TriggerEvent("cbps:raceFinished", position, time);

      foreach (int participantId in race.Participants)
      {
        if (participantId != playerId)
        {
          Player participant = Players[participantId];
          if (participant != null)
          {
            participant.TriggerEvent("cbps:showNotification", $"~b~{player.Name} finished in position {position}");
          }
        }
      }

      playerRaces.Remove(playerId);
    }

    private void OnPlayerDropped(string playerHandle)
    {
      if (int.TryParse(playerHandle, out int playerId))
      {
        RemovePlayerFromRace(playerId);
      }
    }

    #region Helper Classes

    private class Race
    {
      public int Id { get; set; }
      public int CreatorId { get; set; }
      public List<Vector3Serializable> Checkpoints { get; set; } = new List<Vector3Serializable>();
      public List<int> Participants { get; set; } = new List<int>();
      public bool Started { get; set; }
      public List<RaceResult> Finished { get; set; } = new List<RaceResult>();
      public string TemplateName { get; set; }
    }

    private class RaceResult
    {
      public int PlayerId { get; set; }
      public string PlayerName { get; set; }
      public long Time { get; set; }
    }

    public class RaceTemplate
    {
      public string Name { get; set; }
      public List<Vector3Serializable> Checkpoints { get; set; }
      public string CreatedBy { get; set; }
      public long CreatedAt { get; set; }
    }

    public class Vector3Serializable
    {
      public float X { get; set; }
      public float Y { get; set; }
      public float Z { get; set; }

      public Vector3Serializable() { }
      public Vector3Serializable(float x, float y, float z) { X = x; Y = y; Z = z; }
    }

    #endregion
  }
}

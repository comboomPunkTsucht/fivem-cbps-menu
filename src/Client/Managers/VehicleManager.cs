using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace CBPSMenu.Client.Managers
{
    /// <summary>
    /// Manages vehicle-related functionality
    /// Similar to vMenu's vehicle management
    /// </summary>
    public class VehicleManager
    {
        #region Vehicle State

        public bool VehicleInvincible { get; set; } = false;
        public bool VehicleEngineAlwaysOn { get; set; } = false;
        public bool VehicleNoSiren { get; set; } = false;
        public bool VehicleNoBikeFall { get; set; } = false;
        public bool VehicleInfiniteFuel { get; set; } = false;

        // Saved vehicles storage
        private const string SAVED_VEHICLES_KEY = "cbps_saved_vehicles";
        private Dictionary<string, SavedVehicle> _savedVehicles = new Dictionary<string, SavedVehicle>();

        #endregion

        #region Saved Vehicle Data Structure

        public class SavedVehicle
        {
            public string Name { get; set; }
            public string Model { get; set; }
            public int PrimaryColor { get; set; }
            public int SecondaryColor { get; set; }
            public int PearlescentColor { get; set; }
            public int WheelColor { get; set; }
            public int WheelType { get; set; }
            public int Livery { get; set; }
            public float DirtLevel { get; set; }
            public Dictionary<int, int> Mods { get; set; }
            public Dictionary<int, bool> Extras { get; set; }
            public bool CustomTires { get; set; }
            public string LicensePlate { get; set; }
            public int LicensePlateStyle { get; set; }
            public int WindowTint { get; set; }
        }

        #endregion

        #region Constructor

        public VehicleManager()
        {
            LoadSavedVehicles();
            Debug.WriteLine("[comboom.sucht] VehicleManager initialized");
        }

        #endregion

        #region Tick Processing

        /// <summary>
        /// Process vehicle state each tick
        /// </summary>
        public async Task ProcessTick()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;

            if (vehicle != null && vehicle.Exists())
            {
                // Handle vehicle invincibility
                if (VehicleInvincible)
                {
                    vehicle.IsInvincible = true;
                    vehicle.CanTiresBurst = false;
                }

                // Handle engine always on
                if (VehicleEngineAlwaysOn)
                {
                    vehicle.IsEngineRunning = true;
                }

                // Handle no siren
                if (VehicleNoSiren)
                {
                    API.SetVehicleHasMutedSirens(vehicle.Handle, true);
                }

                // Handle no bike fall
                if (VehicleNoBikeFall && vehicle.Model.IsBike)
                {
                    API.SetPedCanBeKnockedOffVehicle(Game.PlayerPed.Handle, 1);
                }
            }

            await Task.FromResult(0);
        }

        #endregion

        #region Vehicle Actions

        /// <summary>
        /// Spawn a vehicle by model name
        /// </summary>
        public async Task<Vehicle> SpawnVehicle(string modelName)
        {
            var model = new Model(modelName);

            if (!model.IsValid || !model.IsVehicle)
            {
                Main.ShowNotification($"~r~Invalid vehicle model: {modelName}");
                return null;
            }

            await model.Request(10000);

            if (!model.IsLoaded)
            {
                Main.ShowNotification("~r~Failed to load vehicle model!");
                return null;
            }

            var playerPed = Game.PlayerPed;
            var position = playerPed.Position + playerPed.ForwardVector * 5f;
            var heading = playerPed.Heading;

            var vehicle = await World.CreateVehicle(model, position, heading);

            if (vehicle != null)
            {
                playerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                Main.ShowNotification($"~g~Vehicle spawned: {modelName}");
            }
            else
            {
                Main.ShowNotification("~r~Failed to spawn vehicle!");
            }

            model.MarkAsNoLongerNeeded();
            return vehicle;
        }

        /// <summary>
        /// Repair the current vehicle
        /// </summary>
        public void RepairVehicle()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                vehicle.Repair();
                Main.ShowNotification("~g~Vehicle repaired!");
            }
            else
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
            }
        }

        /// <summary>
        /// Clean the current vehicle
        /// </summary>
        public void CleanVehicle()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                vehicle.DirtLevel = 0f;
                Main.ShowNotification("~g~Vehicle cleaned!");
            }
            else
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
            }
        }

        /// <summary>
        /// Flip the current vehicle upright
        /// </summary>
        public void FlipVehicle()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                vehicle.PlaceOnGround();
                Main.ShowNotification("~g~Vehicle flipped!");
            }
            else
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
            }
        }

        /// <summary>
        /// Delete the current vehicle
        /// </summary>
        public void DeleteVehicle()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                vehicle.Delete();
                Main.ShowNotification("~r~Vehicle deleted!");
            }
            else
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
            }
        }

        /// <summary>
        /// Toggle vehicle invincibility
        /// </summary>
        public void ToggleVehicleInvincible()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                VehicleInvincible = !VehicleInvincible;
                vehicle.IsInvincible = VehicleInvincible;
                vehicle.CanTiresBurst = !VehicleInvincible;
                Main.ShowNotification(VehicleInvincible ? "~g~Vehicle Invincible: ON" : "~r~Vehicle Invincible: OFF");
            }
            else
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
            }
        }

        /// <summary>
        /// Set vehicle primary color
        /// </summary>
        public void SetPrimaryColor(int colorIndex)
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                int secondary = 0;
                API.GetVehicleColours(vehicle.Handle, ref colorIndex, ref secondary);
                API.SetVehicleColours(vehicle.Handle, colorIndex, secondary);
                Main.ShowNotification("~g~Primary color set!");
            }
            else
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
            }
        }

        /// <summary>
        /// Set vehicle secondary color
        /// </summary>
        public void SetSecondaryColor(int colorIndex)
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                int primary = 0;
                API.GetVehicleColours(vehicle.Handle, ref primary, ref colorIndex);
                API.SetVehicleColours(vehicle.Handle, primary, colorIndex);
                Main.ShowNotification("~g~Secondary color set!");
            }
            else
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
            }
        }

        #endregion

        #region Saved Vehicles

        /// <summary>
        /// Save the current vehicle
        /// </summary>
        public void SaveCurrentVehicle(string name)
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null)
            {
                Main.ShowNotification("~r~You are not in a vehicle!");
                return;
            }

            var savedVehicle = new SavedVehicle
            {
                Name = name,
                Model = vehicle.Model.Hash.ToString(),
                DirtLevel = vehicle.DirtLevel,
                LicensePlate = vehicle.Mods.LicensePlate,
                LicensePlateStyle = (int)vehicle.Mods.LicensePlateStyle,
                WindowTint = (int)vehicle.Mods.WindowTint,
                WheelType = (int)vehicle.Mods.WheelType,
                CustomTires = vehicle.Mods[VehicleModType.FrontWheel].Variation,
                Mods = new Dictionary<int, int>(),
                Extras = new Dictionary<int, bool>()
            };

            // Get colors
            int primary = 0, secondary = 0;
            API.GetVehicleColours(vehicle.Handle, ref primary, ref secondary);
            savedVehicle.PrimaryColor = primary;
            savedVehicle.SecondaryColor = secondary;

            int pearlescent = 0, wheelColor = 0;
            API.GetVehicleExtraColours(vehicle.Handle, ref pearlescent, ref wheelColor);
            savedVehicle.PearlescentColor = pearlescent;
            savedVehicle.WheelColor = wheelColor;

            // Get mods
            for (int i = 0; i < 50; i++)
            {
                var modIndex = vehicle.Mods[(VehicleModType)i].Index;
                if (modIndex >= 0)
                {
                    savedVehicle.Mods[i] = modIndex;
                }
            }

            // Get extras
            for (int i = 0; i < 15; i++)
            {
                savedVehicle.Extras[i] = API.IsVehicleExtraTurnedOn(vehicle.Handle, i);
            }

            // Get livery
            savedVehicle.Livery = API.GetVehicleLivery(vehicle.Handle);

            _savedVehicles[name] = savedVehicle;
            SaveVehiclesToStorage();
            Main.ShowNotification($"~g~Vehicle saved as: {name}");
        }

        /// <summary>
        /// Spawn a saved vehicle
        /// </summary>
        public async Task SpawnSavedVehicle(string name)
        {
            if (!_savedVehicles.ContainsKey(name))
            {
                Main.ShowNotification("~r~Saved vehicle not found!");
                return;
            }

            var savedVehicle = _savedVehicles[name];
            var model = new Model(int.Parse(savedVehicle.Model));

            await model.Request(10000);

            if (!model.IsLoaded)
            {
                Main.ShowNotification("~r~Failed to load vehicle model!");
                return;
            }

            var playerPed = Game.PlayerPed;
            var position = playerPed.Position + playerPed.ForwardVector * 5f;
            var heading = playerPed.Heading;

            var vehicle = await World.CreateVehicle(model, position, heading);

            if (vehicle != null)
            {
                // Apply colors
                API.SetVehicleColours(vehicle.Handle, savedVehicle.PrimaryColor, savedVehicle.SecondaryColor);
                API.SetVehicleExtraColours(vehicle.Handle, savedVehicle.PearlescentColor, savedVehicle.WheelColor);

                // Apply mods
                vehicle.Mods.InstallModKit();
                vehicle.Mods.WheelType = (VehicleWheelType)savedVehicle.WheelType;

                foreach (var mod in savedVehicle.Mods)
                {
                    API.SetVehicleMod(vehicle.Handle, mod.Key, mod.Value, savedVehicle.CustomTires);
                }

                // Apply extras
                foreach (var extra in savedVehicle.Extras)
                {
                    API.SetVehicleExtra(vehicle.Handle, extra.Key, !extra.Value);
                }

                // Apply other properties
                vehicle.DirtLevel = savedVehicle.DirtLevel;
                vehicle.Mods.LicensePlate = savedVehicle.LicensePlate;
                vehicle.Mods.LicensePlateStyle = (LicensePlateStyle)savedVehicle.LicensePlateStyle;
                vehicle.Mods.WindowTint = (VehicleWindowTint)savedVehicle.WindowTint;

                if (savedVehicle.Livery >= 0)
                {
                    API.SetVehicleLivery(vehicle.Handle, savedVehicle.Livery);
                }

                playerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
                Main.ShowNotification($"~g~Spawned saved vehicle: {name}");
            }
            else
            {
                Main.ShowNotification("~r~Failed to spawn vehicle!");
            }

            model.MarkAsNoLongerNeeded();
        }

        /// <summary>
        /// Delete a saved vehicle
        /// </summary>
        public void DeleteSavedVehicle(string name)
        {
            if (_savedVehicles.ContainsKey(name))
            {
                _savedVehicles.Remove(name);
                SaveVehiclesToStorage();
                Main.ShowNotification($"~r~Deleted saved vehicle: {name}");
            }
            else
            {
                Main.ShowNotification("~r~Saved vehicle not found!");
            }
        }

        /// <summary>
        /// Get all saved vehicle names
        /// </summary>
        public List<string> GetSavedVehicleNames()
        {
            return new List<string>(_savedVehicles.Keys);
        }

        /// <summary>
        /// Load saved vehicles from KVP storage
        /// </summary>
        private void LoadSavedVehicles()
        {
            var json = API.GetResourceKvpString(SAVED_VEHICLES_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    _savedVehicles = JsonConvert.DeserializeObject<Dictionary<string, SavedVehicle>>(json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[comboom.sucht] Error loading saved vehicles: {ex.Message}");
                    _savedVehicles = new Dictionary<string, SavedVehicle>();
                }
            }
        }

        /// <summary>
        /// Save vehicles to KVP storage
        /// </summary>
        private void SaveVehiclesToStorage()
        {
            var json = JsonConvert.SerializeObject(_savedVehicles);
            API.SetResourceKvp(SAVED_VEHICLES_KEY, json);
        }

        #endregion
    }
}

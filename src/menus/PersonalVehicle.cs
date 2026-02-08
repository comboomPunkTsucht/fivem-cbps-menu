using System;
using System.Threading.Tasks;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Personal Vehicle submenu - vMenu clone.
    /// </summary>
    public class PersonalVehicle
    {
        private NativeMenu menu;

        // Personal vehicle state
        private Vehicle _personalVehicle = null;
        private int _personalVehicleNetId = 0;

        public bool VehicleLocked { get; private set; } = false;

        private void CreateMenu()
        {
            menu = new NativeMenu("Personal Vehicle", "Manage Your Personal Vehicle");

            RefreshMenu();
        }

        public void RefreshMenu()
        {
            menu.Clear();

            #region Set Personal Vehicle

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PVSetPersonal))
            {
                var setPersonal = new NativeItem("Set Current Vehicle as Personal", "Mark your current vehicle as personal.");
                setPersonal.Activated += (s, e) =>
                {
                    if (!Game.PlayerPed.IsInVehicle())
                    {
                        Notify.Error("You must be in a vehicle.");
                        return;
                    }

                    if (Game.PlayerPed.CurrentVehicle.Driver != Game.PlayerPed)
                    {
                        Notify.Error("You must be the driver.");
                        return;
                    }

                    _personalVehicle = Game.PlayerPed.CurrentVehicle;
                    _personalVehicle.IsPersistent = true;
                    _personalVehicleNetId = VehToNet(_personalVehicle.Handle);

                    // Create blip for personal vehicle
                    var blip = _personalVehicle.AttachBlip();
                    blip.Sprite = BlipSprite.PersonalVehicleCar;
                    blip.Color = BlipColor.Blue;
                    blip.Name = "Personal Vehicle";
                    blip.IsShortRange = true;

                    RefreshMenu();
                    Notify.Success("Vehicle set as personal.");
                };
                menu.Add(setPersonal);
            }

            #endregion

            if (_personalVehicle != null && _personalVehicle.Exists())
            {
                #region Teleport to Vehicle

                var teleportToVehicle = new NativeItem("Teleport to Personal Vehicle", "Teleport to your personal vehicle.");
                teleportToVehicle.Activated += (s, e) =>
                {
                    if (_personalVehicle != null && _personalVehicle.Exists())
                    {
                        var pos = _personalVehicle.Position;
                        SetPedCoordsKeepVehicle(Game.PlayerPed.Handle, pos.X, pos.Y, pos.Z + 1f);
                        Notify.Success("Teleported to personal vehicle.");
                    }
                    else
                    {
                        Notify.Error("Personal vehicle not found.");
                        _personalVehicle = null;
                        RefreshMenu();
                    }
                };
                menu.Add(teleportToVehicle);

                #endregion

                #region Summon Vehicle

                if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PVSummon))
                {
                    var summonVehicle = new NativeItem("Summon Personal Vehicle", "Teleport your personal vehicle to you.");
                    summonVehicle.Activated += (s, e) =>
                    {
                        if (_personalVehicle != null && _personalVehicle.Exists())
                        {
                            var pos = Game.PlayerPed.Position;
                            var forward = Game.PlayerPed.ForwardVector;
                            var spawnPos = pos + (forward * 5f);

                            _personalVehicle.Position = spawnPos;
                            _personalVehicle.PlaceOnGround();
                            _personalVehicle.Heading = Game.PlayerPed.Heading;

                            Notify.Success("Personal vehicle summoned.");
                        }
                        else
                        {
                            Notify.Error("Personal vehicle not found.");
                            _personalVehicle = null;
                            RefreshMenu();
                        }
                    };
                    menu.Add(summonVehicle);
                }

                #endregion

                #region Lock/Unlock

                if (PermissionsManager.IsAllowed(PermissionsManager.Permission.PVLock))
                {
                    var lockVehicle = new NativeCheckboxItem($"Lock Vehicle ({(VehicleLocked ? "Locked" : "Unlocked")})", "Lock or unlock your personal vehicle.", VehicleLocked);
                    lockVehicle.CheckboxChanged += (s, e) =>
                    {
                        if (_personalVehicle != null && _personalVehicle.Exists())
                        {
                            VehicleLocked = lockVehicle.Checked;
                            _personalVehicle.LockStatus = VehicleLocked ? VehicleLockStatus.LockedForPlayer : VehicleLockStatus.Unlocked;
                            Notify.Info($"Vehicle {(VehicleLocked ? "locked" : "unlocked")}.");
                        }
                    };
                    menu.Add(lockVehicle);
                }

                #endregion

                #region Vehicle Options

                var repairBtn = new NativeItem("Repair Personal Vehicle", "Repair your personal vehicle remotely.");
                repairBtn.Activated += (s, e) =>
                {
                    if (_personalVehicle != null && _personalVehicle.Exists())
                    {
                        _personalVehicle.Repair();
                        Notify.Success("Personal vehicle repaired.");
                    }
                };
                menu.Add(repairBtn);

                var washBtn = new NativeItem("Wash Personal Vehicle", "Clean your personal vehicle.");
                washBtn.Activated += (s, e) =>
                {
                    if (_personalVehicle != null && _personalVehicle.Exists())
                    {
                        _personalVehicle.DirtLevel = 0f;
                        _personalVehicle.Wash();
                        Notify.Success("Personal vehicle washed.");
                    }
                };
                menu.Add(washBtn);

                #endregion

                #region Remove Blip

                var removeBlip = new NativeItem("Remove Vehicle Blip", "Remove the blip from your personal vehicle.");
                removeBlip.Activated += (s, e) =>
                {
                    if (_personalVehicle != null && _personalVehicle.Exists())
                    {
                        var blips = _personalVehicle.AttachedBlips;
                        foreach (var blip in blips)
                        {
                            blip.Delete();
                        }
                        Notify.Info("Blip removed.");
                    }
                };
                menu.Add(removeBlip);

                #endregion

                #region Remove Personal Vehicle

                var removePersonal = new NativeItem("~r~Remove as Personal Vehicle", "Unmark this vehicle as personal.");
                removePersonal.Activated += (s, e) =>
                {
                    if (_personalVehicle != null && _personalVehicle.Exists())
                    {
                        var blips = _personalVehicle.AttachedBlips;
                        foreach (var blip in blips)
                        {
                            blip.Delete();
                        }
                    }
                    _personalVehicle = null;
                    _personalVehicleNetId = 0;
                    RefreshMenu();
                    Notify.Info("Personal vehicle removed.");
                };
                menu.Add(removePersonal);

                #endregion
            }
            else
            {
                var noVehicle = new NativeItem("~c~No Personal Vehicle Set", "Enter a vehicle and set it as personal.");
                menu.Add(noVehicle);
            }
        }

        public NativeMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }

        public Vehicle GetPersonalVehicle() => _personalVehicle;
    }
}

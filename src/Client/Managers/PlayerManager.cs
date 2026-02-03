using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace CBPSMenu.Client.Managers
{
    /// <summary>
    /// Manages player-related functionality
    /// Similar to vMenu's player management
    /// </summary>
    public class PlayerManager
    {
        #region Player State

        public bool GodMode { get; set; } = false;
        public bool Invisible { get; set; } = false;
        public bool Noclip { get; set; } = false;
        public bool SuperJump { get; set; } = false;
        public bool FastRun { get; set; } = false;
        public bool InfiniteAmmo { get; set; } = false;
        public bool NoReload { get; set; } = false;
        public bool UnlimitedStamina { get; set; } = false;
        public bool NoRagdoll { get; set; } = false;

        // Noclip settings
        private float _noclipSpeed = 1.0f;
        private const float _noclipFastMultiplier = 5.0f;

        #endregion

        #region Constructor

        public PlayerManager()
        {
            Debug.WriteLine("[comboom.sucht] PlayerManager initialized");
        }

        #endregion

        #region Tick Processing

        /// <summary>
        /// Process player state each tick
        /// </summary>
        public async Task ProcessTick()
        {
            var playerPed = Game.PlayerPed;
            var player = Game.Player;

            // Handle god mode
            if (GodMode)
            {
                playerPed.IsInvincible = true;
            }

            // Handle super jump
            if (SuperJump)
            {
                API.SetSuperJumpThisFrame(player.Handle);
            }

            // Handle fast run
            if (FastRun)
            {
                API.SetRunSprintMultiplierForPlayer(player.Handle, 1.49f);
            }

            // Handle unlimited stamina
            if (UnlimitedStamina)
            {
                API.RestorePlayerStamina(player.Handle, 1.0f);
            }

            // Handle no ragdoll
            if (NoRagdoll)
            {
                API.SetPedCanRagdoll(playerPed.Handle, false);
            }

            // Handle infinite ammo
            if (InfiniteAmmo)
            {
                var weapon = playerPed.Weapons.Current;
                if (weapon != null)
                {
                    weapon.Ammo = weapon.MaxAmmo;
                }
            }

            // Handle no reload
            if (NoReload)
            {
                var weapon = playerPed.Weapons.Current;
                if (weapon != null)
                {
                    weapon.AmmoInClip = weapon.MaxAmmoInClip;
                }
            }

            // Handle noclip movement
            if (Noclip)
            {
                await HandleNoclipMovement();
            }

            await Task.FromResult(0);
        }

        #endregion

        #region Player Actions

        /// <summary>
        /// Heal the player to full health
        /// </summary>
        public void HealPlayer()
        {
            var playerPed = Game.PlayerPed;
            playerPed.Health = playerPed.MaxHealth;
            Main.ShowNotification("~g~Health restored!");
        }

        /// <summary>
        /// Give the player full armor
        /// </summary>
        public void GiveArmor()
        {
            Game.PlayerPed.Armor = 100;
            Main.ShowNotification("~b~Armor restored!");
        }

        /// <summary>
        /// Toggle god mode
        /// </summary>
        public void ToggleGodMode()
        {
            GodMode = !GodMode;
            Game.PlayerPed.IsInvincible = GodMode;
            Main.ShowNotification(GodMode ? "~g~God Mode: ON" : "~r~God Mode: OFF");
        }

        /// <summary>
        /// Toggle invisibility
        /// </summary>
        public void ToggleInvisible()
        {
            Invisible = !Invisible;
            Game.PlayerPed.IsVisible = !Invisible;
            Main.ShowNotification(Invisible ? "~g~Invisible: ON" : "~r~Invisible: OFF");
        }

        /// <summary>
        /// Toggle noclip mode
        /// </summary>
        public void ToggleNoclip()
        {
            Noclip = !Noclip;
            ApplyNoclipState();
        }

        /// <summary>
        /// Apply the current noclip state to the player
        /// </summary>
        private void ApplyNoclipState()
        {
            var playerPed = Game.PlayerPed;

            if (Noclip)
            {
                playerPed.IsInvincible = true;
                playerPed.IsVisible = false;
                API.SetEntityCollision(playerPed.Handle, false, false);
                API.FreezeEntityPosition(playerPed.Handle, true);
                Main.ShowNotification("~g~Noclip: ON");
            }
            else
            {
                // Restore collision first
                API.SetEntityCollision(playerPed.Handle, true, true);
                API.FreezeEntityPosition(playerPed.Handle, false);
                // Restore invincibility based on godMode state
                playerPed.IsInvincible = GodMode;
                // Restore visibility based on invisible state
                playerPed.IsVisible = !Invisible;
                Main.ShowNotification("~r~Noclip: OFF");
            }
        }

        /// <summary>
        /// Handle noclip movement each tick
        /// </summary>
        private async Task HandleNoclipMovement()
        {
            var playerPed = Game.PlayerPed;
            var position = playerPed.Position;
            var speed = _noclipSpeed;

            // Shift for faster movement
            if (API.IsControlPressed(0, 21)) // Left Shift
            {
                speed = _noclipSpeed * _noclipFastMultiplier;
            }

            // W - Forward (horizontal only, vertical movement is handled separately with Q/E)
            // This provides more intuitive control for most use cases
            if (API.IsControlPressed(0, 32))
            {
                var forward = API.GetEntityForwardVector(playerPed.Handle);
                position += new Vector3(forward.X, forward.Y, 0) * speed;
            }

            // S - Backward (horizontal only)
            if (API.IsControlPressed(0, 33))
            {
                var forward = API.GetEntityForwardVector(playerPed.Handle);
                position -= new Vector3(forward.X, forward.Y, 0) * speed;
            }

            // A - Rotate left
            if (API.IsControlPressed(0, 34))
            {
                playerPed.Heading += 3.0f;
            }

            // D - Rotate right
            if (API.IsControlPressed(0, 35))
            {
                playerPed.Heading -= 3.0f;
            }

            // Q - Down
            if (API.IsControlPressed(0, 44))
            {
                position.Z -= speed;
            }

            // E - Up
            if (API.IsControlPressed(0, 38))
            {
                position.Z += speed;
            }

            playerPed.Position = position;

            await Task.FromResult(0);
        }

        /// <summary>
        /// Toggle super jump
        /// </summary>
        public void ToggleSuperJump()
        {
            SuperJump = !SuperJump;
            Main.ShowNotification(SuperJump ? "~g~Super Jump: ON" : "~r~Super Jump: OFF");
        }

        /// <summary>
        /// Toggle fast run
        /// </summary>
        public void ToggleFastRun()
        {
            FastRun = !FastRun;
            if (!FastRun)
            {
                API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, 1.0f);
            }
            Main.ShowNotification(FastRun ? "~g~Fast Run: ON" : "~r~Fast Run: OFF");
        }

        /// <summary>
        /// Teleport to waypoint
        /// </summary>
        public async Task TeleportToWaypoint()
        {
            var waypoint = World.WaypointPosition;
            if (waypoint == Vector3.Zero)
            {
                Main.ShowNotification("~r~No waypoint set!");
                return;
            }

            var playerPed = Game.PlayerPed;

            // Try to get ground Z
            float groundZ = 0f;
            bool found = false;

            for (float z = 1000f; z >= 0f && !found; z -= 25f)
            {
                var testPos = new Vector3(waypoint.X, waypoint.Y, z);
                API.RequestCollisionAtCoord(testPos.X, testPos.Y, testPos.Z);
                await BaseScript.Delay(50);

                float resultZ = 0f;
                if (API.GetGroundZFor_3dCoord(testPos.X, testPos.Y, testPos.Z, ref resultZ, false))
                {
                    groundZ = resultZ;
                    found = true;
                }
            }

            if (!found)
            {
                groundZ = waypoint.Z;
            }

            playerPed.Position = new Vector3(waypoint.X, waypoint.Y, groundZ + 1f);
            Main.ShowNotification("~g~Teleported to waypoint!");
        }

        /// <summary>
        /// Clear wanted level
        /// </summary>
        public void ClearWantedLevel()
        {
            Game.Player.WantedLevel = 0;
            Main.ShowNotification("~g~Wanted level cleared!");
        }

        /// <summary>
        /// Kill the player
        /// </summary>
        public void Suicide()
        {
            Game.PlayerPed.Health = 0;
            Main.ShowNotification("~r~You have committed suicide");
        }

        /// <summary>
        /// Reset player to normal state
        /// </summary>
        public void ResetPlayerState()
        {
            var playerPed = Game.PlayerPed;

            // Reset all toggles
            Noclip = false;
            GodMode = false;
            Invisible = false;
            SuperJump = false;
            FastRun = false;
            InfiniteAmmo = false;
            NoReload = false;
            UnlimitedStamina = false;
            NoRagdoll = false;

            // Restore player to normal state
            API.SetEntityCollision(playerPed.Handle, true, true);
            API.FreezeEntityPosition(playerPed.Handle, false);
            playerPed.IsInvincible = false;
            playerPed.IsVisible = true;
            API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, 1.0f);
            API.SetPedCanRagdoll(playerPed.Handle, true);

            Main.ShowNotification("~g~Player state reset!");
        }

        #endregion
    }
}

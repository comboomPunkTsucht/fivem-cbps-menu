using System;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Recording submenu - vMenu clone.
    /// Provides access to Rockstar Editor and camera controls.
    /// </summary>
    public class Recording
    {
        private NativeMenu menu;

        public bool IsRecording { get; private set; } = false;

        private void CreateMenu()
        {
            menu = new NativeMenu("Recording", "Recording & Camera Options");

            #region Recording Controls

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.RECStart))
            {
                var startRecording = new NativeItem("Start Recording", "Start recording gameplay (Rockstar Editor).");
                startRecording.Activated += (s, e) =>
                {
                    if (IsRecordingGameplayNow())
                    {
                        Notify.Error("Already recording!");
                        return;
                    }
                    StartRecording(1); // 1 = Action replay recording
                    IsRecording = true;
                    Notify.Success("Recording started.");
                };
                menu.Add(startRecording);

                var stopRecording = new NativeItem("Stop Recording", "Stop the current recording.");
                stopRecording.Activated += (s, e) =>
                {
                    if (!IsRecordingGameplayNow())
                    {
                        Notify.Error("Not currently recording.");
                        return;
                    }
                    StopRecording();
                    IsRecording = false;
                    Notify.Success("Recording stopped.");
                };
                menu.Add(stopRecording);

                var saveRecording = new NativeItem("Save Last Recording", "Save the last recorded clip.");
                saveRecording.Activated += (s, e) =>
                {
                    if (IsRecordingGameplayNow())
                    {
                        StopRecordingAndSaveClip();
                    }
                    else
                    {
                        SaveRecordingClip();
                    }
                    IsRecording = false;
                    Notify.Success("Recording saved.");
                };
                menu.Add(saveRecording);

                var discardRecording = new NativeItem("~r~Discard Recording", "Discard the current/last recording.");
                discardRecording.Activated += (s, e) =>
                {
                    if (IsRecordingGameplayNow())
                    {
                        StopRecordingAndDiscardClip();
                    }
                    else
                    {
                        DiscardRecording();
                    }
                    IsRecording = false;
                    Notify.Info("Recording discarded.");
                };
                menu.Add(discardRecording);
            }

            #endregion

            #region Rockstar Editor

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.RECEditor))
            {
                var openEditor = new NativeItem("Open Rockstar Editor", "Open the Rockstar Editor to edit clips.");
                openEditor.Activated += (s, e) =>
                {
                    if (IsRecordingGameplayNow())
                    {
                        StopRecordingAndSaveClip();
                        IsRecording = false;
                    }
                    ActivateRockstarEditor();
                    Notify.Info("Opening Rockstar Editor...");
                };
                menu.Add(openEditor);
            }

            #endregion

            #region Camera Mode

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.RECCamera))
            {
                var freeCam = new NativeItem("Toggle Free Camera", "Toggle the free camera mode.");
                freeCam.Activated += (s, e) =>
                {
                    // This is a simplified version - full implementation would involve camera manipulation
                    SetCinematicModeActive(true);
                    Notify.Info("Cinematic mode activated. Press B to exit.");
                };
                menu.Add(freeCam);

                var cinematicBars = new NativeCheckboxItem("Cinematic Mode", "Toggle cinematic black bars.", false);
                cinematicBars.CheckboxChanged += (s, e) =>
                {
                    SetCinematicModeActive(cinematicBars.Checked);
                };
                menu.Add(cinematicBars);

                var cinemaReplay = new NativeItem("Start Action Replay", "View the last 30 seconds in action replay.");
                cinemaReplay.Activated += (s, e) =>
                {
                    if (IsRecordingGameplayNow())
                    {
                        StopRecording();
                        IsRecording = false;
                    }
                    // Note: There's no direct native for action replay in single frame
                    Notify.Info("Use R* Editor for action replay viewing.");
                };
                menu.Add(cinemaReplay);
            }

            #endregion

            #region Info

            var recordingInfo = new NativeItem("~y~Recording Info", "Tips for using the recording features.");
            recordingInfo.Description = "Recordings are saved to your Rockstar Editor. Access them via Pause Menu > Game > Rockstar Editor.";
            menu.Add(recordingInfo);

            #endregion
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

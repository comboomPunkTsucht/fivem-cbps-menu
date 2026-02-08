using System;

using CitizenFX.Core;
using CitizenFX.Core.Native;

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
                    if (IsRecording)
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
                    if (!IsRecording)
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
                    StopRecordingAndSaveClip();
                    IsRecording = false;
                    Notify.Success("Recording saved.");
                };
                menu.Add(saveRecording);

                var discardRecording = new NativeItem("~r~Discard Recording", "Discard the current/last recording.");
                discardRecording.Activated += (s, e) =>
                {
                    StopRecordingAndDiscardClip();
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
                    if (IsRecording)
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
                var cinematicBars = new NativeCheckboxItem("Cinematic Mode", "Toggle cinematic black bars.", false);
                cinematicBars.CheckboxChanged += (s, e) =>
                {
                    SetCinematicModeActive(cinematicBars.Checked);
                };
                menu.Add(cinematicBars);
            }

            #endregion

            #region Info

            var recordingInfo = new NativeItem("~y~Recording Info", "Recordings saved to Rockstar Editor.");
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

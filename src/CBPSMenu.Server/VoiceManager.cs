using System;
using CitizenFX.Core;

namespace CBPSMenu.Server
{
  public class VoiceManager : BaseScript
  {
    public VoiceManager()
    {
      // Voice is primarily handled by pma-voice exports on client side.
      // Server side just needs to ensure pma-voice is running if we wanted to enforce it.
      // For now, this is a placeholder to match the empty lua file but ready for expansion.
      Debug.WriteLine("^2[CBPS Menu] ^7Voice Manager initialized");
    }
  }
}

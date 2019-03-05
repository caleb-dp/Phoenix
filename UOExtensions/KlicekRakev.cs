using System;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using Phoenix.Communication.Packets;
using System.Diagnostics;

namespace CalExtension.UOExtensions
{
  public class KlicekRakev
  {
    public static KlicekRakev Current;

    //------------------------------------------------------------------------------------------

    public KlicekRakev()
    {
      if (Current == null)
        Current = this;

      syncRoot = new object();
    }

    private object syncRoot;

    //------------------------------------------------------------------------------------------

    public static UOItem Klicek
    {
      get { return World.Player.Backpack.AllItems.FindType(0x186A, 0x0695);  }
    }

    //------------------------------------------------------------------------------------------

    private DateTime requestTime;
    private string option;

    //------------------------------------------------------------------------------------------

    [Executable]
    public bool KlicekRakevUse(string option)
    {
      if (!Klicek.Exist)
      {
        Game.PrintMessage("Nemas klicek!", MessageType.Error);
        return false;
      }

      // Wait for gump
      this.option = option;
      requestTime = DateTime.Now;

      // Use book
      Klicek.Use();
      return true;
    }

    //------------------------------------------------------------------------------------------

    [ServerMessageHandler(0x7C)]
    public CallbackResult OnWaitMenu(byte[] data, CallbackResult prevResult)
    {
      lock (syncRoot)
      {
        if (prevResult != CallbackResult.Normal)
        return prevResult;


        if (UIManager.CurrentState !=  UIManager.State.WaitMenu && !String.IsNullOrEmpty(this.option) && requestTime != null && (DateTime.Now  - requestTime).TotalSeconds < 2)
        {
          Menu packet = new Menu(data);

          if (packet.Title.ToLowerInvariant().Contains("klicek obsahuje"))
          {
            // Cancel menu
            if (this.option == null)
            {
              byte[] selectedData = PacketBuilder.ObjectPicked(packet.DialogSerial, packet.MenuSerial, 0, 0, 0);
              Core.SendToServer(selectedData);
              return CallbackResult.Eat;
            }

            // Select option
            string option = this.option.ToLowerInvariant();
            for (int i = 0; i < packet.Options.Count; i++)
            {
              if (packet.Options[i].Text.ToLowerInvariant() == option)
              {
                byte[] selectedData = PacketBuilder.ObjectPicked(packet.DialogSerial, packet.MenuSerial, (ushort)(i + 1), packet.Options[i].Artwork, packet.Options[i].Hue);
                Core.SendToServer(selectedData);
                this.option = null;
                return CallbackResult.Eat;
              }
            }

            Trace.WriteLine("Unable to find requested option. Menu passed to client.", "UIManager");
          }
          else {
            Trace.WriteLine("Unexpected menu received. Menu passed to client.", "UIManager");
          }
        }

        return CallbackResult.Normal;
      }
    }
  }
}

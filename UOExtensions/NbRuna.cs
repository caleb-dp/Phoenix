

/////////////////////////////////////////////////////////////////////////
//
//     www.ultima.smoce.net
//     Name: NB Runa univerzal v2
//
/////////////////////////////////////////////////////////////////////////
using System;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;

namespace Scripts.DarkParadise
{

  public class nbrunakopanesccc
  {
    private DateTime requestTime;
    private uint? buttonId;

    [Command("nbruna")]
    public void Usenb()
    {
      UOItem nbruna = World.Player.Backpack.AllItems.FindType(0x1F14);
      UOItem rune = new UOItem(nbruna.Serial);
      if (!rune.Exist)
        throw new ScriptErrorException("NB Rune not found.");

      // Wait for gump
      buttonId = 1;
      requestTime = DateTime.Now;
      rune.Use();
    }

    [ServerMessageHandler(0xB0)]
    public CallbackResult OnGenericGump(byte[] data, CallbackResult prevResult)
    {
      if (prevResult != CallbackResult.Normal)
        return prevResult;

      if (buttonId != null && DateTime.Now - requestTime < TimeSpan.FromSeconds(6))
      {
        // Respond automatically
        uint gumpSerial = ByteConverter.BigEndian.ToUInt32(data, 7);

        PacketWriter reply = new PacketWriter(0xB1);
        reply.WriteBlockSize();
        reply.Write(World.Player.Serial);
        reply.Write(gumpSerial);
        reply.Write(buttonId.Value);
        reply.Write(0); // Switches count
        reply.Write(0); // Entries count

        Core.SendToServer(reply.GetBytes());

        // Do not pass gump further
        buttonId = null;
        return CallbackResult.Sent;
      }

      return CallbackResult.Normal;
    }
  }
}



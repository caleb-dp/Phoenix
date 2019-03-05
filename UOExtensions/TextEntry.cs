using System;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using CalExtension;
using CalExtension.UOExtensions;
using System.Collections.Generic;

namespace CalExtension.UOExtensions
{

  public class TextEntry
  {


    //---------------------------------------------------------------------------------------------

    public static void SendText(string text)
    {
      byte[] textBytes = ByteConverter.BigEndian.AsciiEncoding.GetBytes(text);

      byte[] data = new byte[14];
      data[0] = 0x9A;
      data[1] = 0;


      List<byte> l = new List<byte>();
      l.AddRange(data);
      l.Add(1);
      l.AddRange(textBytes);
      l.Add(0);

      l[2] = (byte)l.Count;

      Core.SendToServer(l.ToArray());
    }

    //---------------------------------------------------------------------------------------------

    //   // [ClientMessageHandler(0x9A)]
    //    public CallbackResult OnTextEnter(byte[] data, CallbackResult prevResult)
    //    {
    //      if (prevResult != CallbackResult.Normal)
    //        return prevResult;

    //      Game.PrintMessage("TextEnter " + data.Length);

    //      for (int i = 0; i < data.Length; i++)
    //      {
    //        Game.PrintMessage(i + " " + data[i]);
    //      }


    ////      15:00 Phoenix: Phoenix loaded.
    ////15:00 System: Select item to use the key on.
    ////15:00 System: What would you like to name the key?
    ////15:00 Ungo: Nemusis jist salat 0
    ////15:00 Ungo: [Zvol kouzlo]
    ////15:00 Phoenix: [-1 / -1]
    ////15:00 Ungo: [Zvol kouzlo]
    ////15:00 Phoenix: AutoHeal: False
    ////15:00 Phoenix: Profile changed to 'Default'
    ////15:00 Phoenix: Dummy profile nahran
    ////15:00 Phoenix: TextEnter 28
    ////15:00 Phoenix: 0 154
    ////15:00 Phoenix: 1 0
    ////15:00 Phoenix: 2 28
    ////15:00 Phoenix: 3 0
    ////15:00 Phoenix: 4 0
    ////15:00 Phoenix: 5 0
    ////15:00 Phoenix: 6 0
    ////15:00 Phoenix: 7 0
    ////15:00 Phoenix: 8 0
    ////15:00 Phoenix: 9 0
    ////15:00 Phoenix: 10 0
    ////15:00 Phoenix: 11 0
    ////15:00 Phoenix: 12 0
    ////15:00 Phoenix: 13 0
    ////15:00 Phoenix: 14 1
    ////15:00 Phoenix: 15 88
    ////15:00 Phoenix: 16 97
    ////15:00 Phoenix: 17 97
    ////15:00 Phoenix: 18 32
    ////15:00 Phoenix: 19 115
    ////15:00 Phoenix: 20 100
    ////15:00 Phoenix: 21 97
    ////15:00 Phoenix: 22 115
    ////15:00 Phoenix: 23 100
    ////15:00 Phoenix: 24 115
    ////15:00 Phoenix: 25 32
    ////15:00 Phoenix: 26 120
    ////15:00 Phoenix: 27 0
    ////15:00 Phoenix: ByteConverter.BigEndian ToAsciiString Xaa sdasds x
    ////15:00 Phoenix: ByteConverter.BigEndian ToUnicodeString ????? 8 ?
    ////15 : 00 System: Key renamed: Key to:Xaa sdasds x
    ////15:04 System: What do you want to use this on?


    //      Game.PrintMessage("ByteConverter.BigEndian ToAsciiString " + ByteConverter.BigEndian.ToAsciiString(data, 15, data.Length - 15));
    //      Game.PrintMessage("ByteConverter.BigEndian ToUnicodeString " + ByteConverter.BigEndian.ToUnicodeString(data, 15, data.Length - 15));

    //      //Game.PrintMessage("0 " + data[0]);
    //      //Game.PrintMessage("1 " + data[1]);
    //      //Game.PrintMessage("2 " + data[2]);
    //      //Game.PrintMessage("3 " + data[3]);
    //      //Game.PrintMessage("4 " + data[4]);
    //      //Game.PrintMessage("5 " + data[5]);
    //      //Game.PrintMessage("6 " + data[6]);
    //      //Game.PrintMessage("7 " + data[7]);
    //      //if (buttonId != null && DateTime.Now - requestTime < TimeSpan.FromSeconds(6))
    //      //{
    //      //  // Respond automatically
    //      //  uint gumpSerial = ByteConverter.BigEndian.ToUInt32(data, 7);

    //      //  PacketWriter reply = new PacketWriter(0xB1);
    //      //  reply.WriteBlockSize();
    //      //  reply.Write(World.Player.Serial);
    //      //  reply.Write(gumpSerial);
    //      //  reply.Write(buttonId.Value);
    //      //  reply.Write(0); // Switches count
    //      //  reply.Write(0); // Entries count

    //      //  Core.SendToServer(reply.GetBytes());

    //      //  // Do not pass gump further
    //      //  buttonId = null;
    //      //  return CallbackResult.Sent;
    //      //}

    //      return CallbackResult.Normal;
    //    }
  }
}



using System;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;

namespace CalExtension.UOExtensions
{
  /// <summary>
  /// Cestovni kniha:
  /// 
  /// 1 - Nabit
  /// 2 - Oprav
  /// 3 - Brit
  /// 4 - Muj cech
  /// 5 - Vlastni misto
  /// 6 - 
  /// 7 -
  /// 8 - Jehlom
  /// 9 - Vesper
  /// 10 - Yew
  /// 11 - Minoc
  /// 12 - Scara 
  /// 13 - Magin
  /// 14 - Trin 
  /// 15 - Nujelm
  /// 16 - Trh
  /// 17 - Cove
  /// 18 - Occlo
  /// 19 - Moong
  /// 20 - Templar
  /// 21 - Nara
  /// 22 - Homare
  /// 23 - Zento
  /// 24 - Luna
  /// 25 - Umbra
  /// 26 - Serpent
  /// 
  /// Rune book:
  /// 
  /// 1 - Jhelom
  /// 2 - Vesper
  /// 3 - Yew
  /// 4 - Minoc
  /// 5 - Scara
  /// 6 - Magin
  /// 7 - Trin
  /// 8 - Nujelm
  /// 9 - nabit
  /// 10 - Cove
  /// 
  /// Travel book:
  /// 
  /// 5 - Brit
  /// Nabit - 9
  /// Oprav stat 2
  /// Nabit 1
  /// 
  /// Cech 3
  /// </summary>
  public class Kniha
  {
    //------------------------------------------------------------------------------------------

    public static Kniha Current;

    public Kniha()
    {
      if (Current == null)
        Current = this;
    }


    //------------------------------------------------------------------------------------------

    private DateTime requestTime;
    private uint? buttonId;

    //------------------------------------------------------------------------------------------

    [Command]
    public void TravelUtek(uint destinationId)
    {
      TravelBookUse(destinationId);
      UO.Wait(600);
      if (World.Player.Backpack.AllItems.FindType(0x0F09, 0x0B77).Exist)//invis
      {
        UO.UseType(0x0F09, 0x0B77);
      }
    }

    public static UOItem CestovniKniha
    {
      get { return World.Player.Backpack.AllItems.FindType(0x22C5, 0x0000);  }
    }

    //------------------------------------------------------------------------------------------

    [Executable]
    public void CestovniKnihaUse(uint destinationId)
    {
      if (!KnihaUse(destinationId, 0x22C5, 0x0000))
        UO.Print(0x0025, "Cestovni kniha");
    }

    public static UOItem TravelBook
    {
      get { return World.Player.Backpack.AllItems.FindType(0x0FEF, 0x0482); }
    }

    //------------------------------------------------------------------------------------------

    [Executable]
    public void TravelBookUse(uint destinationId)
    {
      if (!KnihaUse(destinationId, 0x0FEF, 0x0482))
        UO.Print(0x0025, "Travel book");
    }

    public static UOItem RuneBook
    {
      get { return World.Player.Backpack.AllItems.FindType(0x0FF0, 0x08A5); }
    }


    //------------------------------------------------------------------------------------------

    [Executable]
    public void RuneBookUse(uint destinationId)
    {
      if (!KnihaUse(destinationId, 0x0FF0, 0x08A5))
        UO.Print(0x0025, "Rune book");
    }

    public static UOItem DeadBook
    {
      get { return World.Player.Backpack.AllItems.FindType(0x0EFA, 0x0455); }
    }
    //------------------------------------------------------------------------------------------

    [Executable]
    public void DeadBookUse(uint destinationId)
    {
      if (!KnihaUse(destinationId, 0x0EFA, 0x0455))
        UO.Print(0x0025, "Dead book");
    }

    //------------------------------------------------------------------------------------------

    [Executable]
    public bool KnihaUse(uint destinationId, Graphic graphic, UOColor color)
    {
      UOItem book = UO.Backpack.AllItems.FindType(graphic, color);
      if (!book.Exist)
      {
        UO.Print(0x0025, "Nemas knihu!");
        return false;
        //throw new ScriptErrorException("Kniha not found.");
      }

      // Wait for gump
      buttonId = destinationId;
      requestTime = DateTime.Now;

      // Use book
      book.Use();
      return true;
    }

    //------------------------------------------------------------------------------------------

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

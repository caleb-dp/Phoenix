//using System;
//using Phoenix;
//using Phoenix.Communication;
//using Phoenix.WorldData;

//namespace CalExtension.UOExtensions
//{
//  /// <summary>
//  /// Trin HQ - 7
//  /// 
//  /// Nabit - 9
//  /// </summary>
//  public class TravelBook
//  {
//    private DateTime requestTime;
//    private uint? buttonId;

//    [Executable]
//    public void TravelBookUse(uint destinationId)
//    {
//      // Serial: 0x4002420E  Position: 142.100.0  Flags: 0x0000  Color: 0x0482  Graphic: 0x0FEF  Amount: 1  Layer: None  Container: 0x4026624C
//      UOItem book = UO.Backpack.AllItems.FindType(0x0FEF, 0x0482);
//      if (!book.Exist)
//        throw new ScriptErrorException("TravelBook not found.");

//      // Wait for gump
//      buttonId = destinationId;
//      requestTime = DateTime.Now;

//      // Use runebook
//      book.Use();
//    }

//    [Command]
//    public void TravelUtek(uint destinationId)
//    {
//      // Serial: 0x4013A2FB  Position: 84.120.0  Flags: 0x0020  Color: 0x0B77  Graphic: 0x0F09  Amount: 1  Layer: None Container: 0x4032F802
//      TravelBookUse(destinationId);
//      UO.Wait(600);
//      if (World.Player.Backpack.AllItems.FindType(0x0F09, 0x0B77).Exist)
//      {
//        UO.UseType(0x0F09, 0x0B77);
//      }
//    }

//    [ServerMessageHandler(0xB0)]
//    public CallbackResult OnGenericGump(byte[] data, CallbackResult prevResult)
//    {
//      if (prevResult != CallbackResult.Normal)
//        return prevResult;

//      if (buttonId != null && DateTime.Now - requestTime < TimeSpan.FromSeconds(6))
//      {
//        // Respond automatically
//        uint gumpSerial = ByteConverter.BigEndian.ToUInt32(data, 7);

//        PacketWriter reply = new PacketWriter(0xB1);
//        reply.WriteBlockSize();
//        reply.Write(World.Player.Serial);
//        reply.Write(gumpSerial);
//        reply.Write(buttonId.Value);
//        reply.Write(0); // Switches count
//        reply.Write(0); // Entries count

//        Core.SendToServer(reply.GetBytes());

//        UO.WaitMenu("", "");
//        // Do not pass gump further

//        if (buttonId == 4)
//          buttonId = 1;
//        else
//          buttonId = null;
//        return CallbackResult.Sent;
//      }

//      return CallbackResult.Normal;
//    }
//  }
//}

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
//  public class Runebook
//  {
//    private DateTime requestTime;
//    private uint? buttonId;

//    [Executable]
//    public void RuneBookUse(uint destinationId)
//    {
//      UOItem book = UO.Backpack.AllItems.FindType(0x0FF0, 0x08A5);
//      if (!book.Exist)
//        throw new ScriptErrorException("Runebook not found.");

//      // Wait for gump
//      buttonId = destinationId;
//      requestTime = DateTime.Now;

//      // Use runebook
//      book.Use();
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

//        // Do not pass gump further
//        buttonId = null;
//        return CallbackResult.Sent;
//      }

//      return CallbackResult.Normal;
//    }
//  }
//}

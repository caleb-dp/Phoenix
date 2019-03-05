using Phoenix.Communication;
using Phoenix.WorldData;

namespace Phoenix.Plugins
{
  public class DeathHandler
  {
    private TimerList<uint> display = new TimerList<uint>(10);

    [ServerMessageHandler(0x2C)]
    public CallbackResult PlayerDeath(byte[] data, CallbackResult prevResult)
    {
      PacketWriter buf = new PacketWriter(0xBA);
      buf.Write((byte)1);
      buf.Write(World.Player.X);
      buf.Write(World.Player.Y);
      Core.SendToClient(buf.GetBytes());
      UO.Print("You are DEAD!");
      return CallbackResult.Eat;
    }

    [ServerMessageHandler(0xAF)]
    public CallbackResult DeathAnimation(byte[] data, CallbackResult prevResult)
    {
      uint serial = ByteConverter.BigEndian.ToUInt32(data, 5) & 0x7FFFFFFF;
      display.AddTimer(serial);
      return CallbackResult.Normal;
    }


    [ServerMessageHandler(0x20)]
    public CallbackResult OnBlackAndWhite(byte[] data, CallbackResult prevResult)
    {

      if (ByteConverter.BigEndian.ToInt16(data, 5) != 0x0192 && ByteConverter.BigEndian.ToInt16(data, 5) != 0x0193)//0x0191  
      {
        return CallbackResult.Normal;
      }

      byte[] buffer = data;
      ByteConverter.BigEndian.ToBytes((ushort)0x03DB, buffer, 5);
      Core.SendToClient(buffer);
      return CallbackResult.Sent;
    }
  }
}
using Phoenix;

namespace Scripts.DarkParadise
{
    public class Light
    {
        public static byte Limit = 15;

        public Light()
        {
            if (Phoenix.WorldData.World.SunLight > Limit) {
                byte[] data = new byte[2];
                data[0] = 0x4F;
                data[1] = Limit;
                Core.SendToClient(data);
                UO.Print(0x055A, "Light level fixed.");
            }
        }

        [ServerMessageHandler(0x4F)]
        public CallbackResult OnSunLight(byte[] data, CallbackResult prevResult)
        {
            if (prevResult < CallbackResult.Sent) {
                if (data[1] > Limit) {
                    byte[] newData = new byte[2];
                    newData[0] = 0x4F;
                    newData[1] = Limit;
                    Core.SendToClient(newData);
                    UO.Print(0x055A,"Light level fixed.");
                    return CallbackResult.Sent;
                }
            }
            return CallbackResult.Normal;
        }
    }
}

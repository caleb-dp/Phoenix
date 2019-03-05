using System;
using System.Linq;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;

namespace CalExtension.UOExtensions
{
  public class Ship
  {
    //------------------------------------------------------------------------------------------

    public static Ship Current;

    public Ship()
    {
      if (Current == null)
        Current = this;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void LockShip()
    {
      World.FindDistance = 10;
      UOItem prkno = World.Ground.FindType(0x3EB1);
      if (!prkno.Exist)
        prkno = World.Ground.FindType(0x3EB2);
      if (!prkno.Exist)
        prkno = World.Ground.FindType(0x3E8A);
      if (!prkno.Exist)
        prkno = World.Ground.FindType(0x3E85);

      if (prkno.Exist)
      {
        if (World.Player.Backpack.Items.FindType(0x100E, 0x0000).Exist)
        {
          foreach (UOItem item in World.Player.Backpack.Items.Where(g => g.Graphic == 0x100E && g.Color == 0x0000))
          {
            Journal.Clear();
            UO.WaitTargetObject(prkno);
            item.Use();

            if (Journal.WaitForText(true, 250, "You unlock the ship", "You lock the ship"))
            {
              if (Journal.Contains(true, "You unlock the ship"))
                World.Player.PrintMessage("[Unlock...]");
              else
                World.Player.PrintMessage("[Lock...]");

              break;
            }
            else
            {
              continue;
            }
          }


        }
        else
          World.Player.PrintMessage("[Neni klic...]", MessageType.Error);
      }
      else
        World.Player.PrintMessage("[Neni lod...]", MessageType.Error);
    }

    //------------------------------------------------------------------------------------------

    protected UOItem TillerMan
    {
      get
      {
        int orig = World.FindDistance;
        World.FindDistance = 5;

        UOItem tillerMan = World.Ground.FindType(0x3E55);
        if (!tillerMan.Exist)
          tillerMan = World.Ground.FindType(0x3E4B);
        if (!tillerMan.Exist)
          tillerMan = World.Ground.FindType(0x3E50);
        if (!tillerMan.Exist)
          tillerMan = World.Ground.FindType(0x3E4E);

        World.FindDistance = orig;

        return tillerMan;
      }
    }
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void DockShipVoid()
    {
      if (Current != null)
        Current.DockShip();
    }

    [Executable]
    public bool DockShip()
    {
      if (!TillerMan.Exist)
      {
        World.Player.PrintMessage("[Neni lod...]", MessageType.Error);
        return false;
      }


      requestTime = DateTime.Now;
      dock = true;
      TillerMan.Use();
      return true;
    }


    //------------------------------------------------------------------------------------------

    private DateTime requestTime;
    private bool dock = false;  
    //------------------------------------------------------------------------------------------

    [ServerMessageHandler(0xB0)]
    public CallbackResult OnGenericGump(byte[] data, CallbackResult prevResult)
    {
      if (prevResult != CallbackResult.Normal)
        return prevResult;

      if (dock && DateTime.Now - requestTime < TimeSpan.FromSeconds(2))
      {
        // Respond automatically
        uint gumpSerial = ByteConverter.BigEndian.ToUInt32(data, 7);

        PacketWriter reply = new PacketWriter(0xB1);
        reply.WriteBlockSize();
        reply.Write(World.Player.Serial);
        reply.Write(gumpSerial);
        reply.Write(1);
        reply.Write(0); // Switches count
        reply.Write(0); // Entries count

        Core.SendToServer(reply.GetBytes());

        // Do not pass gump further
        dock = false;
        return CallbackResult.Sent;
      }
      else
        dock = false;

      return CallbackResult.Normal;
    }
  }
}

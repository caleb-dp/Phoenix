using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;

namespace CalExtension.UOExtensions
{
  public class Ship
  {
    //------------------------------------------------------------------------------------------

    public static IUOItemType ShipDeedType { get { return new UOItemTypeBase(0x14F1, 0x0000) { Name = "Ship Deed" }; } }
    public static IUOItemType ShipKeyType { get { return new UOItemTypeBase(0x100E, 0x0000) { Name = "Copper key" }; } }
    //------------------------------------------------------------------------------------------

    public static Ship Current;

    public Ship()
    {
      if (Current == null)
        Current = this;
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeBaseCollection CloseDecks
    {
      get
      {
        UOItemTypeBaseCollection col = new UOItemTypeBaseCollection();
        col.Add(new UOItemTypeBase(0x3EB1, 0x0000));
        col.Add(new UOItemTypeBase(0x3EB2, 0x0000));
        col.Add(new UOItemTypeBase(0x3E8A, 0x0000));
        col.Add(new UOItemTypeBase(0x3E85, 0x0000));

        return col;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeBaseCollection OpenDecks
    {
      get
      {
        UOItemTypeBaseCollection col = new UOItemTypeBaseCollection();
        col.Add(new UOItemTypeBase(0x3ED5, 0x0000));
        col.Add(new UOItemTypeBase(0x3ED4, 0x0000));
        col.Add(new UOItemTypeBase(0x3E89, 0x0000));
        col.Add(new UOItemTypeBase(0x3E84, 0x0000));
        return col;
      }
    }

    //---------------------------------------------------------------------------------------------
    public static UOItem LastShipKey = new UOItem(Serial.Invalid);
    public static UOItem LastShipKeyOrDefault
    {
      get
      {
        if (LastShipKey != null && LastShipKey.Exist)
          return LastShipKey;

        return World.Player.Backpack.Items.FindType(ShipKeyType.Graphic);
      }
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void EscapeToShip()
    {
      int origFindDistance = World.FindDistance;
      World.FindDistance = 20;
      List<UOItem> prkna = World.Ground.Where(i => OpenDecks.Count(d => d.Graphic == i.Graphic) > 0 || CloseDecks.Count(d => d.Graphic == i.Graphic) > 0).ToList(); //new List<UOItem>();

      ResultInfo openResult = OpenShip();
      if (openResult.Success || prkna.Count > 0)
      {
        EnterShip();

        if (LastShipKeyOrDefault.Exist)
          LockShip();
      }

      World.FindDistance = origFindDistance;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static ResultInfo OpenShip()
    {
      ResultInfo result = new ResultInfo();
      UOItem deed = World.Player.Backpack.AllItems.FindType(ShipDeedType);

      if (deed.Exist)
      {
        World.Player.PrintMessage("Umisti lod");

        TargetInfo target = new TargetInfo().GetTarget();
        if (target.Success)
        {


          List<Serial> bpkState = ItemHelper.ContainerState(World.Player.Backpack);
          Journal.Clear();
          UO.WaitTargetTile(target.StaticTarget.X, target.StaticTarget.Y, target.StaticTarget.Z, target.StaticTarget.Graphic);
          deed.Use();

          if (Journal.WaitForText(true, 500, "Terrain is too bumpy to place structure here", "The duplicate key is in your bank account"))
          {
            if (Journal.Contains(true, "The duplicate key is in your bank account"))
            {
              result.Success = true;

              List<Serial> bpkStateAfter = ItemHelper.ContainerState(World.Player.Backpack);
              List<Serial> diff = ItemHelper.ContainerStateDiff(bpkState, bpkStateAfter);
              UOItem lastKey = new UOItem(Serial.Invalid);
              foreach (Serial ds in diff)
              {
                UOItem dsItem = new UOItem(ds);
                if(dsItem.Graphic == ShipKeyType.Graphic)
                {
                  lastKey = dsItem;
                  break;
                }
              }

              if (!lastKey.Serial.IsValid)
                lastKey = World.Player.Backpack.Items.FindType(ShipKeyType.Graphic);

              if (lastKey.Serial.IsValid)
                LastShipKey = lastKey;

              result.Item = lastKey;
            }
            else
              World.Player.PrintMessage("Sem nelze..");
          }
          else
            Game.PrintMessage("Nepovedlo se...?");
        }
      }
      else
        World.Player.PrintMessage("Nemas lod", MessageType.Error);

      return result;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void EnterShip()
    {
      int origFindDistance = World.FindDistance;
      World.FindDistance = 20;

      List<UOItem> prknaOpen = World.Ground.Where(i => OpenDecks.Count(d => d.Graphic == i.Graphic) > 0).ToList(); //new List<UOItem>();
      List<UOItem> prknaClose = World.Ground.Where(i => CloseDecks.Count(d => d.Graphic == i.Graphic) > 0).ToList(); //new List<UOItem>();

      if (prknaOpen.Count == 0 && prknaClose.Count == 0)
      {
        World.Player.PrintMessage("[Neni prkno...]", MessageType.Error);
        return;
      }
      else
      {
        UOPositionBase lastPostion = (UOPositionBase)World.Player.GetPosition();

        if (prknaOpen.Count == 0)
        {
          foreach (UOItem prkno in prknaClose)
          {
            prkno.Use();
            Game.Wait(250);

            prknaOpen = World.Ground.Where(i => OpenDecks.Count(d => d.Graphic == i.Graphic) > 0).ToList();
            if (prknaOpen.Count > 0)
            {
              prknaOpen[0].Use();
              break;
            }

          }
        }
        else
        {
          prknaOpen[0].Use();
        }
        Game.Wait(250);
        UOPositionBase currentPosition = (UOPositionBase)World.Player.GetPosition();

        if (lastPostion.Distance(currentPosition) > 0)
          World.Player.PrintMessage("[ Nastoupils...]");
        else
          World.Player.PrintMessage("[ Nenastoupils...]", MessageType.Error);
      }
      
      World.FindDistance = origFindDistance;
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void LockShip()
    {
      int origFindDistance = World.FindDistance;
      World.FindDistance = 20;

      List<UOItem> prkna = World.Ground.Where(i => OpenDecks.Count(d => d.Graphic == i.Graphic) > 0 || CloseDecks.Count(d => d.Graphic == i.Graphic) > 0).ToList(); //new List<UOItem>();

      if (prkna.Count > 0)
      {
  
        if (LastShipKeyOrDefault.Exist)
        {
          foreach (UOItem prkno in prkna)
          {
            Journal.Clear();
            UO.WaitTargetObject(prkno);
            LastShipKeyOrDefault.Use();

            if (Journal.WaitForText(true, 250, "You unlock the ship", "You lock the ship"))
            {
              if (Journal.Contains(true, "You unlock the ship"))
              {
                UO.WaitTargetObject(prkno);
                LastShipKeyOrDefault.Use();

                if (Journal.WaitForText(true, 250, "You unlock the ship", "You lock the ship"))
                  World.Player.PrintMessage("[Lock...]");
              }
              else
                World.Player.PrintMessage("[Lock...]");

              Game.Wait(250);
            }
            else
            {
              Game.Wait(250);
              continue;
            }

          }
        }
        else
          World.Player.PrintMessage("[Neni klic...]", MessageType.Error);
      }
      else
        World.Player.PrintMessage("[Neni prkno...]", MessageType.Error);

      World.FindDistance = origFindDistance;
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


//result.AddMessage("Nemas lod", MessageType.Error);

//      19:22 System: You have gained some health back.
//19:22 : [Use]
//19:22 System: Vitej doma.
//19:22 : [Use]
//19:22 System: Where would you like to place the Small Ship?
//19:22 System: Terrain is too bumpy to place structure here.
//19:22 System: You have gained some health back.
//19:22 System: Where would you like to place the Small Ship?
//19:22 System: Terrain is too bumpy to place structure here.
//19:22 System: Where would you like to place the Small Ship?
//19:22 System: Terrain is too bumpy to place structure here.
//19:22 System: Where would you like to place the Small Ship?
//19:22 System: Terrain is too bumpy to place structure here.
//19:22 System: Where would you like to place the Small Ship?
//19:22 System: The duplicate key is in your bank account
//19:22 You see: a copper key
//19:23 System: You have gained some health back.

//Nose N -Close
//Serial: 0x4024E651  Name: "ship"  Position: 3186.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3EB1  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4025016D  Position: 3190.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3EB2  Amount: 0  Layer: None Container: 0x00000000

//Nose E -Close
//Serial: 0x4024E651  Name: "ship"  Position: 3188.4.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E8A  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4025016D  Name: "ship"  Position: 3188.8.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E85  Amount: 0  Layer: None Container: 0x00000000

//Nose S -Close
//Serial: 0x4024E651  Name: "ship"  Position: 3186.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3EB1  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4025016D  Name: "ship"  Position: 3190.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3EB2  Amount: 0  Layer: None Container: 0x00000000

//Nose W -Close
//Serial: 0x4024E651  Name: "ship"  Position: 3188.4.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E8A  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4025016D  Name: "ship"  Position: 3188.8.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E85  Amount: 0  Layer: None Container: 0x00000000

//Nose N -Open
//Serial: 0x4024E651  Name: "ship"  Position: 3186.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3ED5  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4025016D  Name: "ship"  Position: 3190.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3ED4  Amount: 0  Layer: None Container: 0x00000000

//Nose E -Open
//Serial: 0x4024E651  Name: "ship"  Position: 3188.4.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E89  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4025016D  Name: "ship"  Position: 3188.8.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E84  Amount: 0  Layer: None Container: 0x00000000

//Nose S -Open
//Serial: 0x4025016D  Name: "gang plank"  Position: 3190.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3ED4  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4024E651  Name: "ship"  Position: 3186.6.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3ED5  Amount: 0  Layer: None Container: 0x00000000

//Nose S -Open
//Serial: 0x4025016D  Name: "ship"  Position: 3188.8.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E84  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4024E651  Name: "ship"  Position: 3188.4.- 5  Flags: 0x0000  Color: 0x0000  Graphic: 0x3E89  Amount: 0  Layer: None Container: 0x00000000

using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using Phoenix.Communication;
using Caleb.Library.CAL.Business;

namespace CalExtension.UOExtensions
{
  public class MapLocation
  {
    //private static Dictionary<uint, ushort[]> openedMaps;
    //public static Dictionary<uint, ushort[]> OpenedMaps
    //{
    //  get
    //  {
    //    if (openedMaps == null)
    //      openedMaps = new Dictionary<uint, ushort[]>();
    //    return openedMaps;
    //  }
    //}

    private static Dictionary<string, List<uint>> openedMapsLoc;
    public static Dictionary<string, List<uint>> OpenedMapsLoc
    {
      get
      {
        if (openedMapsLoc == null)
          openedMapsLoc = new Dictionary<string, List<uint>>();
        return openedMapsLoc;
      }
    }

    [ServerMessageHandler(0x90)]
    public CallbackResult OnMapInfo(byte[] data, CallbackResult prevResult)
    {
      PacketReader reader = new PacketReader(data);
      reader.Skip(1); // opcode
      uint serial = reader.ReadUInt32();
      ushort gump = reader.ReadUInt16();
      ushort ux = reader.ReadUInt16();
      ushort uy = reader.ReadUInt16();
      ushort lx = reader.ReadUInt16();
      ushort ly = reader.ReadUInt16();
      ushort width = reader.ReadUInt16();
      ushort height = reader.ReadUInt16();

      ushort x = (ushort)(ux + 180 * (lx - ux > 361 ? -1 : 1));
      ushort y = (ushort)(uy + 180 * (ly - uy > 361 ? -1 : 1));

      UOPositionBase pos = new UOPositionBase(x, y, 0);

      if (!OpenedMapsLoc.ContainsKey(pos.ToString()))
      {
        OpenedMapsLoc.Add(pos.ToString(), new List<uint>());
        OpenedMapsLoc[pos.ToString()].Add(serial);
      }
      else
      {
        OpenedMapsLoc[pos.ToString()].Add(serial);
      }

      Game.PrintMessage(String.Format("Poklad je na {0},{1} - {2}", x,y, serial));

      return CallbackResult.Normal;// CallbackResult.Sent;
    }

    public static UOItemType mapka = new UOItemType() { Graphic = 0x14EB, Color = 0x0000 };

    [Executable]
    public static void SortMap(int maxDistance)
    {
      openedMapsLoc = null;

      Game.PrintMessage("Bagl s mapkami >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());

      if (containerFrom.Serial.IsValid && containerFrom.Exist)
      {
        foreach (UOItem item in containerFrom.Items)
        {
          if (item.Graphic == mapka.Graphic)
          {
            item.Move(1, World.Player.Backpack);
            Game.Wait();
          }
        }
      }

      foreach (UOItem item in World.Player.Backpack.Items)
      {
        if (item.Graphic == mapka.Graphic)
        {
          item.Use();
          Game.Wait();
        }
      }

      Game.PrintMessage("OpenedMapsLoc: " + OpenedMapsLoc.Count);

      Dictionary<string, List<uint>> groups = new Dictionary<string, List<uint>>();

      foreach (KeyValuePair<string, List<uint>> kvp in OpenedMapsLoc)
      {
        string[] pos = kvp.Key.Split(',');
        UOPositionBase mapPos = new UOPositionBase(ushort.Parse(pos[0]), ushort.Parse(pos[1]), 0);

        bool found = false;
        foreach (KeyValuePair<string, List<uint>> groupkvp in groups)
        {
          string[] gruppos = groupkvp.Key.Split(',');
          UOPositionBase groupmapPos = new UOPositionBase(ushort.Parse(gruppos[0]), ushort.Parse(gruppos[1]), 0);

          if (Robot.GetRelativeVectorLength(groupmapPos,mapPos) < maxDistance)
          {
            groups[groupkvp.Key].AddRange(kvp.Value);
            found = true;
            break;
          }
        }
        
        if (!found)
        {
          if (groups.ContainsKey(mapPos.ToString()))
            groups[mapPos.ToString()].AddRange(kvp.Value);
          else
            groups.Add(mapPos.ToString(), kvp.Value);
        }
      }

      //pytlik obyc 5 rad 20;
      //4 sloupce po 20

      ushort currentCol = 10;
      ushort currentRow = 1;


      foreach (KeyValuePair<string, List<uint>> kvp in groups)
      {
        ushort colItemPos = currentCol;
        foreach (uint serial in kvp.Value)
        {
          UOItem item = new UOItem(serial);
          item.Move(1, containerFrom, colItemPos, currentRow);

          colItemPos++;
          Game.Wait();
        }

        currentCol = (ushort)(currentCol + 20);

        if (currentCol > 120)
        {
          currentCol = 10;
          currentRow = (ushort)(currentRow + 16);
        }
      }
    }
  }
  
  //puvodni okopceno
  //public class Tracker
  //{
  //  private const float MapSizeMultiplier = 3f;
  //  private const ushort MapZoom = 720;

  //  [Command]
  //  public void Track()
  //  {
  //    Track(false, 0, 0);
  //  }
  //  [Command]
  //  public void Track(ushort x, ushort y)
  //  {
  //    Track(true, x, y);
  //  }
  //  private void Track(bool enable, ushort x, ushort y)
  //  {
  //    PacketWriter writer = new PacketWriter(0xba);
  //    writer.Write(enable ? (byte)0x1 : (byte)0x0);
  //    writer.Write(x);
  //    writer.Write(y);

  //    Core.SendToClient(writer.GetBytes());
  //  }

  //  [ServerMessageHandler(0x90)]
  //  public CallbackResult OnMapInfo(byte[] data, CallbackResult prevResult)
  //  {
  //    PacketReader reader = new PacketReader(data);
  //    reader.Skip(1); // opcode
  //    uint serial = reader.ReadUInt32();
  //    ushort gump = reader.ReadUInt16();
  //    ushort ux = reader.ReadUInt16();
  //    ushort uy = reader.ReadUInt16();
  //    ushort lx = reader.ReadUInt16();
  //    ushort ly = reader.ReadUInt16();
  //    ushort width = reader.ReadUInt16();
  //    ushort height = reader.ReadUInt16();

  //    ushort x = (ushort)(ux + 180 * (lx - ux > 361 ? -1 : 1));
  //    ushort y = (ushort)(uy + 180 * (ly - uy > 361 ? -1 : 1));

  //    UO.PrintInformation("Map is located at {0},{1}", x, y);
  //    Track(x, y);

  //    PacketWriter writer = new PacketWriter(0x90);
  //    writer.Write(serial);
  //    writer.Write(gump);
  //    writer.Write((ushort)(x - MapZoom));
  //    writer.Write((ushort)(y - MapZoom));
  //    writer.Write((ushort)(x + MapZoom));
  //    writer.Write((ushort)(y + MapZoom));
  //    writer.Write((ushort)(width * MapSizeMultiplier));
  //    writer.Write((ushort)(height * MapSizeMultiplier));
  //    Core.SendToClient(writer.GetBytes());

  //    return CallbackResult.Sent;
  //  }

  //  [ServerMessageHandler(0x56)]
  //  public CallbackResult OnPinInfo(byte[] data, CallbackResult prevResult)
  //  {
  //    if (data[5] > 4)
  //      return prevResult;

  //    ushort x = ByteConverter.BigEndian.ToUInt16(data, 7);
  //    ushort y = ByteConverter.BigEndian.ToUInt16(data, 9);

  //    Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(x * MapSizeMultiplier)), 0, data, 7, 2);
  //    Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(y * MapSizeMultiplier)), 0, data, 9, 2);

  //    return prevResult;
  //  }

  //  [ClientMessageHandler(0x56)]
  //  public CallbackResult OnPinAction(byte[] data, CallbackResult prevResult)
  //  {
  //    if (data[5] > 4)
  //      return prevResult;

  //    ushort x = ByteConverter.BigEndian.ToUInt16(data, 7);
  //    ushort y = ByteConverter.BigEndian.ToUInt16(data, 9);

  //    Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(x / MapSizeMultiplier)), 0, data, 7, 2);
  //    Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(y / MapSizeMultiplier)), 0, data, 9, 2);

  //    return prevResult;
  //  }
  //}
}

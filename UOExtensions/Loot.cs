using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using CalExtension.Skills;
using Scripts.DarkParadise;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml;

namespace CalExtension.UOExtensions
{
  [RuntimeObject]
  public class Loot
  {
    //---------------------------------------------------------------------------------------------

    public static Graphic BodyGraphic { get { return new Graphic(0x2006); } }
    public static string LootDefaultItemTypes = @"0x0E76:0x049A//loot bag
0x1F13:0x0000//??
0x0F0D:0x000E//lavabomb
0x0F7D:0x031D//darkblood
0x0F82:0x0000//dragonblood
0x1078:0x0615//darkhide
0x0E80:0x0123//pokladek
0x1010:0x0000//wrong klicek
0x1C18:0x0000//CRAFT OLEJ
0x0F3F:0x0000//sipy
0x1BFB:0x0000//sipky
0x166F:0x0000//parat harpye
0x1BD1:0x0000//peri
0x0EED:0x0000//GP
0x100E:0x0000//Klic Q3
0x0F27:0x0000//slzy FDD a Blood
0x0E21:0x0000//bandy
";

    //---------------------------------------------------------------------------------------------

    //private object SyncRoot;
    //public Loot()
    //{
    //  SyncRoot = new object();
    //  toDispatchItems = new List<Serial>();
    //  World.ItemAdded += World_ItemAdded;
    //}

    //private List<Serial> toDispatchItems;

    ////---------------------------------------------------------------------------------------------

    //private void World_ItemAdded(object sender, ObjectChangedEventArgs e)
    //{
    //  lock (SyncRoot)
    //  {
    //    if (!toDispatchItems.Contains(e.Serial))
    //      toDispatchItems.Add(e.Serial);

    //    if (!toDispatchItems.Contains(e.ItemSerial))
    //      toDispatchItems.Add(e.ItemSerial);
    //  }
    //}

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection DefaultLootItems
    {
      get
      {
        UOItemTypeCollection col = new UOItemTypeCollection();

        string currentLootTypes = CalebConfig.LootItemTypes;
        string[] lines = (currentLootTypes + String.Empty).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
          string line = lines[i].Trim();
          if (line.StartsWith("--") || String.IsNullOrEmpty(line))
            continue;

          string[] splitLine = line.Split(new string[] { "//" }, StringSplitOptions.None);
          string name = splitLine.Length > 1 ? splitLine[1] : String.Empty;

          try
          {
            Match m = Regex.Match(line + String.Empty, "^(?<graphic>0x[A-Za-z0-9]{4}):?(?<color>0x[A-Za-z0-9]{4})?.*$");

            Graphic g = 0;
            UOColor c = 0x0000;

            if (m.Success)
            {
              g = Graphic.Parse(m.Groups["graphic"].Value);
              
              if (m.Groups["color"].Success && !String.IsNullOrEmpty(m.Groups["color"].Value))
                c = UOColor.Parse(m.Groups["color"].Value);
            }

            if (g > 0 && !g.IsInvariant)
            {
              col.Add(new UOItemType() { Graphic = g, Color = c, Name = name });
            }


          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine("DefaultLootItems " + i + " Ex: " + ex.Message);
          }

        }

        return col;
      }
    }


    //---------------------------------------------------------------------------------------------

    public static List<LootItemTemplate> LootItems
    {
      get
      {
        EnsureLootItemConfig();
        XmlDocument doc = CalebConfig.GlobalDocument;

        List<LootItemTemplate> lootItems = new List<LootItemTemplate>();
        if (doc != null)
        {
          XmlElement itemsEl = doc.DocumentElement.SelectSingleNode("Loot/Items") as XmlElement;

          foreach (XmlElement item in itemsEl.ChildNodes)
          {
            LootItemTemplate itemTemplate = new LootItemTemplate();

            string attrValue = item.GetAttribute("Graphic");
            if (!String.IsNullOrEmpty(attrValue))
            {
              try { itemTemplate.Graphic = Graphic.Parse(attrValue); }
              catch { }
            }

            attrValue = item.GetAttribute("Color");
            if (!String.IsNullOrEmpty(attrValue))
            {
              try { itemTemplate.Color = UOColor.Parse(attrValue); }
              catch { }
            }

            attrValue = item.GetAttribute("Source");
            if (!String.IsNullOrEmpty(attrValue))
            {
              try { itemTemplate.Source = (LootItemSource)Enum.Parse(typeof(LootItemSource), attrValue); }
              catch { }
            }

            if (itemTemplate.Graphic > 0 && !itemTemplate.Graphic.IsInvariant)
              lootItems.Add(itemTemplate);
          }
        }
        return lootItems;

        //if (lootItems == null)
        //{
        //  lootItems = new UOItemTypeCollection();
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E76, Color = 0x049A });//loot bag
        //  lootItems.Add(new UOItemType() { Graphic = 0x1F13, Color = 0x0000 });
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F0D, Color = 0x000E });//lavabomb
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F7D, Color = 0x031D });//darkblood
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F82, Color = 0x0000 });//dragonblood
        //  lootItems.Add(new UOItemType() { Graphic = 0x1078, Color = 0x0615 });//darkhide
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E80, Color = 0x0123 });//pokladek
        //  lootItems.Add(new UOItemType() { Graphic = 0x1010, Color = 0x0000 });//wrong klicek
        //  lootItems.Add(new UOItemType() { Graphic = 0x1C18, Color = 0x0000 });//CRAFT OLEJ
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E34, Color = 0x0000 });//Blanky
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F3F, Color = 0x0000 });//sipy
        //  lootItems.Add(new UOItemType() { Graphic = 0x1BFB, Color = 0x0000 });//sipky
        //  lootItems.Add(new UOItemType() { Graphic = 0x166F, Color = 0x0000 });//parat harpye
        //  lootItems.Add(new UOItemType() { Graphic = 0x1BD1, Color = 0x0000 });//peri
        //  lootItems.Add(new UOItemType() { Graphic = 0x0EED, Color = 0x0000 });//GP
        //  lootItems.Add(new UOItemType() { Graphic = 0x100E, Color = 0x0000 });//Klic Q3
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F27, Color = 0x0000 });//slzy FDD a Blood
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E21, Color = 0x0000 });//bandy

        //  //0x0F27 slzy FDD a Blood
        //  //0x0F27 
        //  //
        //  //0x1BFB  
        //}
        //return lootItems;
      }
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void EnsureLootItemConfig()
    {
      XmlDocument doc = CalebConfig.GlobalDocument;

      if (doc != null)
      {
        XmlElement rootEl = doc.DocumentElement.SelectSingleNode("Loot") as XmlElement;
        if (rootEl == null)
          rootEl = doc.DocumentElement.AppendChild(doc.CreateElement("Loot")) as XmlElement;

        XmlElement items = rootEl.SelectSingleNode("Items") as XmlElement;
        if (items == null)
          items = rootEl.AppendChild(doc.CreateElement("Items")) as XmlElement;

        if (items.ChildNodes.Count == 0)
        {
          foreach (UOItemType itemType in DefaultLootItems)
          {
            XmlElement item = doc.CreateElement("LootItem");
            XmlAttribute attr = doc.CreateAttribute("Graphic");
            attr.Value = itemType.Graphic.ToString();
            item.Attributes.Append(attr);

            attr = doc.CreateAttribute("Color");
            attr.Value = itemType.Color.ToString();
            item.Attributes.Append(attr);

            attr = doc.CreateAttribute("Name");
            attr.Value = itemType.Name;
            item.Attributes.Append(attr);

            attr = doc.CreateAttribute("Source");
            attr.Value = itemType.Graphic == 0x0F0D && itemType.Color == 0x000E ? "Corpse" : "Any";
            item.Attributes.Append(attr);
            //0x0F0D:0x000E//lavabomb

            items.AppendChild(item);
          }

          Game.PrintMessage("DefaultLootItems " + DefaultLootItems.Count);
        }

        CalebConfig.GlobalCalebConfig = Utils.FormatXml(doc.DocumentElement);
      }
      else
        Game.PrintMessage("Doc " + null);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsLootItem(UOItem item)
    {
      UOItemTypeBaseCollection regs = ReagentCollection.Reagents.ToItemTypeCollection();

      if (Array.IndexOf(regs.GraphicList.ToArray(), item.Graphic) > -1)
        return true;

      UOItem cont = new UOItem(item.Container);
      LootItemSource itemSource = LootItemSource.Any;
      if (!cont.Exist)
        itemSource = LootItemSource.Ground;
      else if (cont.Graphic == 0x2006)
        itemSource = LootItemSource.Corpse;

      foreach (LootItemTemplate o in LootItems)
      {
        bool sourceMatch = (o.Source == LootItemSource.Any || o.Source == itemSource);

        if (item.Graphic == o.Graphic && o.Color == item.Color)
          return true && sourceMatch;

        if (o.Color == 0x0000 && item.Graphic == o.Graphic)
          return true && sourceMatch;


      }

      if (item.Graphic == 0x1F13) //skilpoint
        return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    private UOItem lootBag = new UOItem(Serial.Invalid);
    public UOItem LootBag
    {
      get
      {
        if (!lootBag.Exist || lootBag.Container != World.Player.Backpack.Serial)
          lootBag = World.Player.Backpack.Items.FindType(0x0E76);

        if (lootBag.Exist)
          return lootBag;

        return World.Player.Backpack;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected UOItem dwarfKnife;
    public UOItem DwarfKnife
    {
      get
      {
        if (dwarfKnife == null)
          dwarfKnife =  World.Player.Backpack.Items.FindType(0x10E4);
        return dwarfKnife;
      }
    }

    //---------------------------------------------------------------------------------------------

    public enum LootType
    {
      None = 0,
      Quick = 1,
      QuickCut = 2,
      Safe = 3,
      SafeCut = 4
    }


    //---------------------------------------------------------------------------------------------

    private List<Serial> cutedBodies = new List<Serial>();

    [Executable]
    [BlockMultipleExecutions]
    public void LootGround()
    {
      this.LootGround(LootType.Quick);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void LootGround(LootType lootType)
    {
      LootGround(lootType, false);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void LootGround(LootType lootType, bool lootAll)
    {
      //List<UOItem> searchItems = new List<UOItem>();
      List<UOItem> ground = new List<UOItem>();
      ground.AddRange(World.Ground.ToArray());

      List<UOItem> search = new List<UOItem>();

      int done = 0;
      int toDo = 0;
      int bodies = 0;

      List<Serial> bpkState = ItemHelper.ContainerState(World.Player.Backpack);

      foreach (UOItem item in ground)
      {
        if (item.Graphic == 0x2006)
        {
          bodies++;
          if (cutedBodies.Contains(item.Serial))
          {
            done++;
          }
          else
          {
            toDo++;
            if (item.Distance <= 6)
              search.Add(item);
          }
        }
      }
      
      for (int i = 0; i < search.Count; i++)//(UOItem item in search)
      {
        UOItem item = search[i];
        Game.PrintMessage(String.Format("Try Loot Body [{0}/{1}]", i + 1, search.Count), Game.Val_LightGreen);

        if (!item.Opened)
        {
          Journal.Clear();
          item.Use();
          if (Journal.WaitForText(true, 250, "You can't reach that"))
          {
            UO.PrintObject(item.Serial, Game.Val_LightGreen, "[Can't reach]");
            continue;
          }
        }

        List<UOItem> items = new List<UOItem>();
        items.AddRange(item.Items.ToArray());

        bool hasLootBag = items.Count(si => si.Graphic == 0x0E76 && si.Color == 0x049A) > 0;



        if (!hasLootBag)
        {
          foreach (UOItem bag in items.Where(b => b.Graphic == 0x0E76))
          {
            if (String.IsNullOrEmpty(bag.Name))
            {
              bag.Click();
              Game.Wait(175);


              if ((bag.Name + String.Empty).ToLower().StartsWith("loot"))
              {
                hasLootBag = true;
                break;
              }
            }
          }
        }

        //Notepad.WriteLine("LootManual: " + item.Description);
        //Notepad.WriteLine("LootManual hasLootBag: " + hasLootBag);
        //Notepad.WriteLine("LootManual items: " + items.Count);

        //return;

        if (hasLootBag || lootAll || item.Distance <= 1) //item.Items.FindType(0x0E76, 0x049A).Exist)// lootbag
        {
          UO.PrintObject(item.Serial, Game.Val_LightGreen, "[Looting...]");

          if (DwarfKnife.Exist && (hasLootBag || item.Distance <= 1) && !lootAll)//TODO predelat nejak jinak, ted kvuly tamingu aby to nelotovalo maso a kuze
          {
            toDo--;
            done++;

            foreach (UOItem lootItem in items)
            {
              if (Loot.IsLootItem(lootItem))
              {
                lootItem.Move(65000, LootBag);
                Game.Wait(300);
              }
            }

            DwarfKnife.Use();
            UO.WaitTargetObject(item);
            Game.Wait(350);

            items = new List<UOItem>();
            items.AddRange(item.Items.ToArray());
          }

          if (item.Exist)
          {

            foreach (UOItem lootItem in items)
            {
              if (Loot.IsLootItem(lootItem) || lootAll)
              {
                lootItem.Move(60000, LootBag);
                Game.Wait(425);
              }
            }
          }
        }

        //cutedBodies.Add(item.Serial);
      }

      World.Player.PrintMessage(String.Format("Bodies remain [{0}]", toDo), Game.Val_LightGreen);

      foreach (UOItem item in World.Ground)
      {
        if (item.Distance <= 6 && item.Graphic == 0x0E76 && item.Color == 0x049A)//IsLootItem(item))//jen lootbag
        {
          item.Move(60000, LootBag);
          Game.Wait(425);
        }
      }

      List<Serial> bpkAfterLoot = ItemHelper.ContainerState(World.Player.Backpack);
      List<Serial> diff = ItemHelper.ContainerStateDiff(bpkState, bpkAfterLoot);

      Game.PrintMessage("Diff... " + diff.Count);
      if (LootBag.Serial != World.Player.Backpack.Serial)
      {
        foreach (Serial lootedItem in diff)
        {
          UOItem item = new UOItem(lootedItem);

          if (item.Container == World.Player.Backpack.Serial)
          {
            if (item.Move(65000, LootBag))
            {
              Game.PrintMessage("LootItem Moved to Bag...");
            }
            Game.Wait();
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    private void LootCollection(List<UOItem> searchItems)
    {
      this.LootCollection(searchItems, false);
    }

    //---------------------------------------------------------------------------------------------

    private void LootCollection(List<UOItem> searchItems, bool lootAll)
    {
      UOItemTypeBaseCollection regs = ReagentCollection.Reagents.ToItemTypeCollection();
      foreach (UOItem item in searchItems)
      {
        bool grabed = false;

        if (lootAll || IsLootItem(item))
        {
          item.Move(1000, LootBag);
          grabed = true;
        }

        if (grabed)
          Game.Wait(435 + Core.Latency + (lootAll ? 1000 : 0));
      }
    }
    //---------------------------------------------------------------------------------------------
  }

  public class LootItemTemplate
  {
    public UOColor Color = 0x0000;
    public Graphic Graphic = 0x0000;
    public LootItemSource Source = LootItemSource.Any;
  }

  public enum LootItemSource
  {
     Any = 0, Ground = 1 , Corpse = 2
  }

}



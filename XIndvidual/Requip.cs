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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;
using CalExtension.UOExtensions;

namespace CalExtension.XIndividual
{
  public class Requip
  {
    //---------------------------------------------------------------------------------------------

    public static Serial VAL_MojeBezpecna = 0x402B9CFC;
    public static Serial VAL_MojeBezpecna_BedinkaAnimalBoxy = 0x40027781;
    public static Serial VAL_MojeGuild = 0x402FF61B;
    public static Serial VAL_MojeGuild_BedinkaEquip = 0x40027BBD;
    public static Serial VAL_MojeBezpecna_RegPytlik = 0x40328131;
    public static Serial VAL_MojeBezpecna_BedinkaSvitky = 0x4021B3C0;
    public static Serial VAL_MojeBezpecna_BedinkaIngoty = 0x401B9E24;  

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void lownekrorefull(int magRegy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();


      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();



      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((30 - prazdneLahve.Amount), 0));

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack,
        "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: 100");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag,
        "Name: teleportvitek, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45",
        "Name: efsvitek, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 55",
        "Name: parasvitek, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + "  , Color: 0x0000, X: 15, Y: 65",
        "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110",
        "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110",
        "Name: locky, Amount: 10, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65",
        //"Name: magiclocky, Amount: 30, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80",
        "Name: salat, Amount: 1, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180",
        "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6");

      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Rabbit|Sewer Rat|Giant Rat, Count: 4, X: 20, Y: 120");

      ItemRequip.RefullLahve(0, cilBag, 
        "Name: Cure, Quality: Greater, MaxItem: 2", 
        "Name: Invisibility, Quality: None, MaxItem: 2",
        "Name: Heal, Quality: Greater, MaxItem: 5",
        "Name: Strength, Quality: Greater, MaxItem: 5",
        "Name: Refresh, Quality: Total, MaxItem: 5"
        );
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 4");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxrangrefull(int magRegy, int bandy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 4");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 8");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 10");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 10");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 10");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 14, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 1, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 1, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 150, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 8", "Name: Reflex Ring, Count: 1, Amount: 8", "Name: Great Diamant Bracelet, Count: 2, Amount: 8", "Name: Great Gold Ring, Count: 1, Amount: 8");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Cat, Count: 8, X: 20, Y: 120", "Name: Panther, Count: 20, X: 40, Y: 120", "Name: Leopard, Count: 6, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: 150");
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: 250");
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 100");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxostrorefull(int magRegy, int bandy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 4");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 8");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 10");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 10");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 10");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 14, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 1, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 1, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 150, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 8", "Name: Reflex Ring, Count: 1, Amount: 8", "Name: Great Diamant Bracelet, Count: 2, Amount: 8", "Name: Great Gold Ring, Count: 1, Amount: 8");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Cat, Count: 8, X: 20, Y: 120", "Name: Bull, Count: 12, X: 40, Y: 120", "Name: Hind|Hart, Count: 20, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: 150");
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: 250");
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 100");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxiskrefull(int magRegy, int bandy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((65 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 6");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 10");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 15");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 15");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 12");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");


      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 6, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: wallofstone, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 150, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 6", "Name: Great Diamant Bracelet, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Dog|Cat, Count: 5, X: 40, Y: 120", "Name: Bull, Count: 10, X: 60, Y: 120");


      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: 150");
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxvamprefull(int magRegy, int bandy)
    {

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((70 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 6");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 10");

      if (!Potion.Blood.ContainsTopKad(cilBag))
        lahve.Add("Name: Blood, Quality: None, MaxItem: 15");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 15");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 12");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 6, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 150, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 8", "Name: Reflex Ring, Count: 1, Amount: 8", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 10");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Pig|Dog, Count: 5, X: 20, Y: 120", "Name: Bull, Count: 10, X: 60, Y: 120", "Name: Dog|Cat, Count: 5, X: 60, Y: 120");


      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Blood, Amount: 200");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxwarrefull(int magRegy, int bandy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 6");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 10");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 15");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 15");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 12");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 6, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      //ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: wallofstone, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
      //ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
     // ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 150, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: bubinek, Amount: 1, Graphic: 0x0E9C, Color: 0x0000  , X: 120, Y: 135");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: harfa, Amount: 1, Graphic: 0x0EB2, Color: 0x0000  , X: 140, Y: 135");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: loutna, Amount: 1, Graphic: 0x0EB3, Color: 0x0000  , X: 160, Y: 135");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 10", "Name: Reflex Ring, Count: 1, Amount: 10", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 10");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Chicken, Count: 10, X: 40, Y: 120", "Name: Boar|Goat, Count: 5, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxclerrefull(int magRegy, int bandy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((50 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 10");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 10");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 10");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 6, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (Game.CurrentGame.CurrentPlayer.PlayerSubClass != PlayerSubClass.Monk)
      {
        ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: greaterheal, Amount: 45, Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");
      }

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 10, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
   //   ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(magRegy, 1665) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(magRegy, 700) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(magRegy, 2040) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(magRegy, 615) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(magRegy, 1355) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(magRegy, 425) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(magRegy, 500) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(magRegy, 1525) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 180");



      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 8", "Name: Reflex Ring, Count: 1, Amount: 8");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Cat, Count: 15, X: 20, Y: 120", "Name: Bull, Count: 15, X: 40, Y: 120");


      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 150");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 225));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 225) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxbishoprefull(int magRegy, int bandy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((50 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 10");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 10");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 10");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 6, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: greaterheal, Amount: 45, Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 10, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
    //  ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(magRegy, 1430) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(magRegy, 700) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(magRegy, 2015) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(magRegy, 540) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(magRegy, 1330) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(magRegy, 200) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(magRegy, 475) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(magRegy, 1525) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 8", "Name: Reflex Ring, Count: 1, Amount: 8");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Cat, Count: 15, X: 20, Y: 120", "Name: Bull, Count: 15, X: 40, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 150");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 205));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 205) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxnekrorefull(int magRegy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 4");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 10");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 10");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 10");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: 150");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 6, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: efsvitek, Amount: 40, Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 10, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: vialofblood, Amount: 200, Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 140");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: bone, Amount: 100, Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 140");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: excap, Amount: 100, Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 140");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(magRegy, 2075) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(magRegy, 1200) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(magRegy, 2305) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(magRegy, 1355) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(magRegy, 555) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(magRegy, 875) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(magRegy, 1755) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(magRegy, 475) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 8", "Name: Reflex Ring, Count: 1, Amount: 8");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Giant Rat|Rat|Chicken, Count: 4, X: 20, Y: 120");


      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 282));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 282) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxmagrefull(int magRegy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 4");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 10");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 10");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 10");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: 100");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 6, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: efsvitek, Amount: 15, Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: wossvitek, Amount: 40, Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 105");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 10, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
     // ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 1, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");

      //     ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");



      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(magRegy, 1605) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(magRegy, 3155) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(magRegy, 1650) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(magRegy, 950) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(magRegy, 2750) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(magRegy, 800) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(magRegy, 1275) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(magRegy, 475) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Chicken, Count: 6, X: 20, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 266));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(magRegy, 266) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void maxgolemakrefull(int magRegy, int bandy)
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 6");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 10");

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem: 15");

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem: 15");

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: 12");

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + bandy);

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 8, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 2, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: locky, Amount: 15, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: magiclocky, Amount: 15, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 180");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Verite s speara, Amount: 1, Graphic: 0x1402, Color: 0x08A1, X: 90, Y: 65");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: " + magRegy + ", X: 15, Y: 180");

      ItemRequip.RefullSperkyClear(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 10", "Name: Reflex Ring, Count: 1, Amount: 10", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 10");
      ItemRequip.RefullKlamakyClear(VAL_MojeBezpecna_BedinkaAnimalBoxy, cilBag, "Name: Pig|Dog, Count: 5, X: 20, Y: 120", "Name: Chicken, Count: 5, X: 40, Y: 120", "Name: Gray Wolf|Boar, Count: 5, X: 60, Y: 120", "Name: Bull, Count: 15, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void magkpzrefull()
    {
      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();

      UO.UseObject(VAL_MojeBezpecna);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild);
      Game.Wait();
      UO.UseObject(VAL_MojeGuild_BedinkaEquip);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaAnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_RegPytlik);
      Game.Wait();
      UO.UseObject(VAL_MojeBezpecna_BedinkaSvitky);
      Game.Wait();

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 1");
      lahve.Add("Name: Heal, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Strength, Quality: Greater, MaxItem: 1");
      lahve.Add("Name: Refresh, Quality: Total, MaxItem: 1");
      lahve.Add("Name: Total Mana Refresh, Quality: None, MaxItem: 8");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 2");

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: 25");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: teleportvitek, Amount: 2, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: resssvitek, Amount: 2, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: efsvitek, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_BedinkaSvitky, cilBag, "Name: wossvitek, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 105");
      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 110");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: magregy, Amount: 250, X: 15, Y: 180");
    }

    //---------------------------------------------------------------------------------------------

    public static int GetRefullAmountReserve(int percentReserve, int amount)
    {
      if (percentReserve == 0)
        return amount;

      int result = amount;
      int sign = Math.Sign(percentReserve);
      int absReserve = Math.Abs(percentReserve);

      double d = ((double)absReserve / (double)100);
      try
      {
        double reserve = amount * d * sign;
        Game.PrintMessage("reserve: " + reserve, MessageType.Warning);
        result =  (int)Math.Round((amount + reserve) / 100, 0) * 100;
      }
      catch (Exception ex)
      {
        Game.PrintMessage("Ex: " + ex.Message, MessageType.Error);
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------
  }
}


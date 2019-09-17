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
  public partial class Requip
  {

//Serial: 0x4024ADE5  Name: "Spolek's guild secure chest"  Position: 3174.27.26  Flags: 0x0000  Color: 0x084D  Graphic: 0x0E40  Amount: 0  Layer: None Container: 0x00000000
//Serial: 0x4017B47E  Position: 74.146.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x0E75  Amount: 1  Layer: None Container: 0x4024ADE5
//Serial: 0x4034431B  Position: 98.143.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x09AA  Amount: 1  Layer: None Container: 0x4024ADE5
//Serial: 0x400AC7B3  Position: 118.144.0  Flags: 0x0000  Color: 0x0B40  Graphic: 0x09A8  Amount: 1  Layer: None Container: 0x4024ADE5
//Serial: 0x403074C3  Position: 118.127.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x09AA  Amount: 1  Layer: None Container: 0x4024ADE5



    //---------------------------------------------------------------------------------------------

    public static Serial VAL_TowerGuildSecureChest = 0x4024ADE5;
    public static Serial VAL_TowerGuildSecureChest_Svitky = 0x4017B47E;
    public static Serial VAL_TowerGuildSecureChest_AnimalBoxy = 0x4034431B;
    public static Serial VAL_TowerGuildSecureChest_SperkyRegy = 0x400AC7B3;

    public static Serial VAL_TowerGuildSecureChest_SperkyRegy_MagRegy = 0x402C332F;
    public static Serial VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy = 0x4000D42B;
    public static Serial VAL_TowerGuildSecureChest_SperkyRegy_Harfa = 0x403775D0;
    public static Serial VAL_TowerGuildSecureChest_SperkyRegy_Bubinek = 0x402F6719;
    public static Serial VAL_TowerGuildSecureChest_SperkyRegy_Loutna = 0x400174A7;

    public static Serial VAL_TowerGuildSecureChest_RybyOstani = 0x403074C3;
    public static Serial VAL_TowerGuildSecureChest_RybyOstani_Verite = 0x40274A41;
    public static Serial VAL_TowerGuildSecureChest_RybyOstani_Pracka = 0x40009A1C;

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_uni() { towerref_uni(100); }

    [Executable]
    public static void towerref_uni(int procento) { towerref_uni(procento, false); }
    /// <summary>
    /// 100% 
    /// 500 Regu od kazdeho
    /// 250 band
    /// 50 v kadi nebo 10 flasek o zakladnich // flasky se inkrementuji nad 100% pouze o max 33%  zadane hodnoty
    /// 
    /// </summary>
    /// <param name="procento"></param>
    /// <param name="supportItems"></param>
    [Executable]
    public static void towerref_uni(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);


      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 4));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 250));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 10) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento2, 4) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: bubinek, Amount: 1, Graphic: 0x0E9C, Color: 0x0000  , X: 120, Y: 120");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: harfa, Amount: 1, Graphic: 0x0EB2, Color: 0x0000  , X: 140, Y: 120");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: loutna, Amount: 1, Graphic: 0x0EB3, Color: 0x0000  , X: 160, Y: 120");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 5, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: modraryba, Amount: " + GetRefullAmountReserve(procento, 2) + ", Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: Verite s speara, Amount: 1, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");



      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(procento, 500) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat, Count: 5, X: 20, Y: 120", "Name: Bull, Count: " + GetRefullAmountReserve(procento2, 4) + ", X: 40, Y: 120");


      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento2, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento2, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Cure, Quality: Greater, Amount: " + GetRefullAmountReserve(procento2, 50));

      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: " + GetRefullAmountReserve(procento2, 50));

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_nekro() { towerref_nekro(100); }

    [Executable]
    public static void towerref_nekro(int procento)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 6");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 4");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 4");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 4));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: 150");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 10) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento2, 6) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: 40, Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 18, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 2, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 4, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 300) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 150) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 150) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(procento, 1900) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(procento, 1000) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(procento, 2400) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(procento, 1100) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(procento, 1000) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(procento, 1200) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(procento, 2300) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(procento, 1000) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat|Rabbit|Rat|Chicken, Count: 5, X: 20, Y: 120");


      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 175));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 175) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_mag() { towerref_mag(100); }

    [Executable]
    public static void towerref_mag(int procento)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 6");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 4");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 4");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 4));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: 150");


      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento2, 6) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: 15, Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: 40, Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 105");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 18, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 2, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 4, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(procento, 1605) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(procento, 3155) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(procento, 2775) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(procento, 950) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(procento, 2750) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(procento, 800) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(procento, 2400) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(procento, 1600) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat|Rabbit|Rat|Chicken, Count: 5, X: 20, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 266));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 266) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_bishop() { towerref_bishop(100); }

    [Executable]
    public static void towerref_bishop(int procento)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 4));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 500));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: 4, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento2, 6) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: greaterheal, Amount: 45, Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 5, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 2, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 4, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(procento, 1430) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(procento, 700) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(procento, 3740) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(procento, 540) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(procento, 1330) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(procento, 800) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(procento, 1600) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 141");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(procento, 2650) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat|Rabbit|Rat|Chicken, Count: 5, X: 20, Y: 120", "Name: Bull, Count: 8, X: 40, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 205));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 205) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_cler() { towerref_cler(100); }

    [Executable]
    public static void towerref_cler(int procento)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 4));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 900));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento2, 6) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: greaterheal, Amount: 45, Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 5, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 2, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 4, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(procento, 1665) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(procento, 700) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(procento, 3765) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(procento, 615) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(procento, 1355) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(procento, 1025) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(procento, 1625) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(procento, 2650) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat|Rabbit|Rat|Chicken, Count: 5, X: 20, Y: 120", "Name: Bull, Count: 8, X: 40, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 225));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 225) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_medic() { towerref_medic(100); }

    [Executable]
    public static void towerref_medic(int procento)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 4));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 1100));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento2, 6) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: greaterheal, Amount: 45, Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 75");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: paralyze, Amount: 5, Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 90");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 5, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 2, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 4, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(procento, 1665) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(procento, 700) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(procento, 3765) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(procento, 615) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(procento, 1355) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(procento, 1025) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(procento, 1625) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(procento, 2650) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat|Rabbit|Rat|Chicken, Count: 5, X: 20, Y: 120", "Name: Bull, Count: 8, X: 40, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 225));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 225) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_monk() { towerref_monk(100); }

    [Executable]
    public static void towerref_monk(int procento) { towerref_monk(procento, false); }

    [Executable]
    public static void towerref_monk(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));

      List<string> lahve = new List<string>();
      lahve.Add("Name: Cure, Quality: Greater, MaxItem: 2");
      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 4));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 1100));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: 10, Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento2, 6) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 5, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 2, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 4, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s speara, Amount: 2, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");

      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: MandrakeRoot, Amount: " + GetRefullAmountReserve(procento, 1665) + ", Graphic: " + Reagent.MandrakeRoot.Graphic + ", Color: 0x0000, X: 15, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BloodMoss, Amount: " + GetRefullAmountReserve(procento, 700) + ", Graphic: " + Reagent.BloodMoss.Graphic + ", Color: 0x0000, X: 25, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SpidersSilk, Amount: " + GetRefullAmountReserve(procento, 3765) + ", Graphic: " + Reagent.SpidersSilk.Graphic + ", Color: 0x0000, X: 35, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: SulphurousAsh, Amount: " + GetRefullAmountReserve(procento, 615) + ", Graphic: " + Reagent.SulphurousAsh.Graphic + ", Color: 0x0000, X: 45, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Garlic, Amount: " + GetRefullAmountReserve(procento, 1355) + ", Graphic: " + Reagent.Garlic.Graphic + ", Color: 0x0000, X: 55, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Nightshade, Amount: " + GetRefullAmountReserve(procento, 1025) + ", Graphic: " + Reagent.Nightshade.Graphic + ", Color: 0x0000, X: 65, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: BlackPearl, Amount: " + GetRefullAmountReserve(procento, 1625) + ", Graphic: " + Reagent.BlackPearl.Graphic + ", Color: 0x0000, X: 75, Y: 141");
      ItemRequip.RefullCommon(VAL_MojeBezpecna_RegPytlik, cilBag, "Name: Ginseng, Amount: " + GetRefullAmountReserve(procento, 2650) + ", Graphic: " + Reagent.Ginseng.Graphic + ", Color: 0x0000, X: 85, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat|Rabbit|Rat|Chicken, Count: 5, X: 20, Y: 120", "Name: Bull, Count: 8, X: 40, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");
      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 225));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 225) * 2);
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");


      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_guard() { towerref_guard(100); }

    [Executable]
    public static void towerref_guard(int procento) { towerref_guard(procento, false); }

    [Executable]
    public static void towerref_guard(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 450));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: 3, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Bubinek, cilBag, "Name: bubinek, Amount: 1, Graphic: 0x0E9C, Color: 0x0000  , X: 120, Y: 120");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Harfa, cilBag, "Name: harfa, Amount: 1, Graphic: 0x0EB2, Color: 0x0000  , X: 140, Y: 120");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Loutna, cilBag, "Name: loutna, Amount: 1, Graphic: 0x0EB3, Color: 0x0000  , X: 160, Y: 120");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento2, 450) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Squirrel|Black Cat|Cat|Chicken, Count: 5, X: 40, Y: 120", "Name: Bull frog|Goat, Count: 3, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
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
    public static void towerref_destro() { towerref_destro(100); }

    [Executable]
    public static void towerref_destro(int procento) { towerref_destro(procento, false); }

    [Executable]
    public static void towerref_destro(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 450));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: 3, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0x0F62, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento2, 450) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Chicken, Count: 5, X: 40, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
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
    public static void towerref_wh() { towerref_wh(100); }

    [Executable]
    public static void towerref_wh(int procento) { towerref_wh(procento, false); }

    [Executable]
    public static void towerref_wh(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 450));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: 3, Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Bubinek, cilBag, "Name: bubinek, Amount: 1, Graphic: 0x0E9C, Color: 0x0000  , X: 120, Y: 120");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Harfa, cilBag, "Name: harfa, Amount: 1, Graphic: 0x0EB2, Color: 0x0000  , X: 140, Y: 120");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Loutna, cilBag, "Name: loutna, Amount: 1, Graphic: 0x0EB3, Color: 0x0000  , X: 160, Y: 120");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento2, 450) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 2, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Squirrel|Black Cat|Cat|Chicken, Count: 5, X: 40, Y: 120", "Name: Bull frog|Goat, Count: 3, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
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
    public static void towerref_magic() { towerref_magic(100); }

    [Executable]
    public static void towerref_magic(int procento) { towerref_magic(procento, false); }

    [Executable]
    public static void towerref_magic(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 400));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 600) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat, Count: 4, X: 40, Y: 120", "Name: Bull|Cow, Count: 6, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 100));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_isk() { towerref_isk(100); }

    [Executable]
    public static void towerref_isk(int procento) { towerref_isk(procento, false); }

    [Executable]
    public static void towerref_isk(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 400));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");
      
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 600) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat, Count: 4, X: 40, Y: 120", "Name: Bull|Cow, Count: 6, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 100));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_teuton() { towerref_teuton(100); }

    [Executable]
    public static void towerref_teuton(int procento) { towerref_teuton(procento, false); }

    [Executable]
    public static void towerref_teuton(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((75 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 400));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: reactivsvitek, Amount: " + GetRefullAmountReserve(procento, 10) + ", Graphic: " + Magery.SpellScrool[StandardSpell.ReactiveArmor] + ", Color: 0x0000, X: 25, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: strsvitek, Amount: " + GetRefullAmountReserve(procento, 20) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Strength] + ", Color: 0x0000, X: 25, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: healsvitek, Amount: " + GetRefullAmountReserve(procento, 40) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Heal] + ", Color: 0x0000, X: 25, Y: 75");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 600) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat, Count: 4, X: 40, Y: 120", "Name: Bull|Cow, Count: 6, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 100));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_ostro() { towerref_ostro(100); }

    [Executable]
    public static void towerref_ostro(int procento) { towerref_ostro(procento, false); }

    [Executable]
    public static void towerref_ostro(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      ItemRequip.RefullToulce(VAL_TowerGuildSecureChest, 700);

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 350));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 10) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 700) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Black Cat|Cat, Count: 5, X: 40, Y: 120", "Name: Bull|Cow, Count: 10, X: 60, Y: 120", "Name: Sheep, Count: 5, X: 80, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 150));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 300));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 100");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_druid() { towerref_druid(100); }

    [Executable]
    public static void towerref_druid(int procento) { towerref_druid(procento, false); }

    [Executable]
    public static void towerref_druid(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }
      ItemRequip.RefullToulce(VAL_TowerGuildSecureChest, 700);
      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 350));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 10) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 700) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Squirrel, Count: 4, X: 40, Y: 120", "Name: Panther, Count: 12, X: 60, Y: 120", "Name: Leopard, Count: 4, X: 80, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 150));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 300));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 100");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_shaman() { towerref_shaman(100); }

    [Executable]
    public static void towerref_shaman(int procento) { towerref_shaman(procento, false); }

    [Executable]
    public static void towerref_shaman(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }
      ItemRequip.RefullToulce(VAL_TowerGuildSecureChest, 700);
      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Heal.ContainsTopKad(cilBag))
        lahve.Add("Name: Heal, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 10));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 475));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 10) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 4) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento2, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 700) + ", X: 15, Y: 141");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Bubinek, cilBag, "Name: bubinek, Amount: 1, Graphic: 0x0E9C, Color: 0x0000  , X: 120, Y: 120");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Harfa, cilBag, "Name: harfa, Amount: 1, Graphic: 0x0EB2, Color: 0x0000  , X: 140, Y: 120");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_Loutna, cilBag, "Name: loutna, Amount: 1, Graphic: 0x0EB3, Color: 0x0000  , X: 160, Y: 120");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4", "Name: Great Diamant Bracelet, Count: 1, Amount: 6", "Name: Great Gold Ring, Count: 1, Amount: 6");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Cat|Black Cat, Count: 4, X: 40, Y: 120", "Name: Panther, Count: 12, X: 60, Y: 120", "Name: Leopard, Count: 4, X: 80, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Heal, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 50");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 150));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 300));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 100");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_brujah() { towerref_brujah(100); }

    [Executable]
    public static void towerref_brujah(int procento) { towerref_brujah(procento, false); }

    [Executable]
    public static void towerref_brujah(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Blood.ContainsTopKad(cilBag))
        lahve.Add("Name: Blood, Quality: None, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 400));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: paralyze, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 75");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: bladespiritsvitek, Amount: " + GetRefullAmountReserve(procento, 12) + ", Graphic: " + Magery.SpellScrool[StandardSpell.BladeSpirit] + ", Color: 0x0000, X: 25, Y: 45");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 700) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Sheep, Count: 4, X: 40, Y: 120", "Name: Bull|Cow, Count: 8, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Blood, Amount: 200");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 100");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 100));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_tremer() { towerref_tremer(100); }

    [Executable]
    public static void towerref_tremer(int procento) { towerref_tremer(procento, false); }

    [Executable]
    public static void towerref_tremer(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Blood.ContainsTopKad(cilBag))
        lahve.Add("Name: Blood, Quality: None, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 250));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: paralyze, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Paralyze] + ", Color: 0x0000, X: 15, Y: 75");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento, 700) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Sheep, Count: 4, X: 40, Y: 120", "Name: Bull|Cow, Count: 8, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Blood, Amount: 200");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 100");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 100));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void towerref_gangrel() { towerref_gangrel(100); }

    [Executable]
    public static void towerref_gangrel(int procento) { towerref_gangrel(procento, false); }

    [Executable]
    public static void towerref_gangrel(int procento, bool supportItems)
    {
      if (procento > 500)
      {
        Game.PrintMessage("Max 500% ! - " + procento);
        return;
      }

      TargetInfo cilBag = new TargetInfo();
      Game.PrintMessage("Vyberte cilovy bag:");
      cilBag.GetTarget();
      Game.Wait(250);
      cilBag.Item.Use();
      Game.Wait(250);

      UO.UseObject(VAL_TowerGuildSecureChest);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_Svitky);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_AnimalBoxy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_SperkyRegy);
      Game.Wait();
      UO.UseObject(VAL_TowerGuildSecureChest_RybyOstani);
      Game.Wait();

      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_SperkyRegy);
      ItemHelper.OpenContainerRecursive(VAL_TowerGuildSecureChest_RybyOstani);

      int procento2 = 100;
      if (procento > 100)
      {
        double d = (double)procento;
        d = (d - 100) * 0.33;
        procento2 = 100 + (int)d;
      }

      UOItem prazdneLahve = World.Player.FindType(0x0F0E);

      ItemRequip.RefullCommon(VAL_MojeGuild_BedinkaEquip, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: " + Math.Max((45 - prazdneLahve.Amount), 0));
      List<string> lahve = new List<string>();

      if (!Potion.Cure.ContainsTopKad(cilBag))
        lahve.Add("Name: Cure, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 8));

      lahve.Add("Name: Invisibility, Quality: None, MaxItem: 2");

      if (!Potion.LavaBomb.ContainsTopKad(cilBag))
        lahve.Add("Name: Lava Bomb, MaxItem:  " + GetRefullAmountReserve(procento2, 6));

      if (!Potion.Blood.ContainsTopKad(cilBag))
        lahve.Add("Name: Blood, Quality: None, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Strength.ContainsTopKad(cilBag))
        lahve.Add("Name: Strength, Quality: Greater, MaxItem:  " + GetRefullAmountReserve(procento2, 15));

      if (!Potion.Refresh.ContainsTopKad(cilBag))
        lahve.Add("Name: Refresh, Quality: Total, MaxItem: " + GetRefullAmountReserve(procento2, 12));

      if (!Potion.Shrink.ContainsTopKad(cilBag))
        lahve.Add("Name: Shrink, Quality: None, MaxItem: 2");

      ItemRequip.RefullLahve(0, cilBag, lahve.ToArray());

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: PrazdneLahve, Graphic: 0x0F0E, Color: 0x0000, Amount: 6");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, World.Player.Backpack, "Name: Bandages, Graphic: 0x0E21, Color: 0x0000, Amount: " + GetRefullAmountReserve(procento, 600));

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: teleportvitek, Amount: " + GetRefullAmountReserve(procento2, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Teleport] + ", Color: 0x0000, X: 15, Y: 45");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: resssvitek, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: " + Magery.SpellScrool[StandardSpell.Ressurection] + ", Color: 0x0000, X: 15, Y: 60");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: bladespiritsvitek, Amount: " + GetRefullAmountReserve(procento, 12) + ", Graphic: " + Magery.SpellScrool[StandardSpell.BladeSpirit] + ", Color: 0x0000, X: 25, Y: 45");

      if (supportItems)
      {
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: wossvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.WallofStone] + ", Color: 0x0000, X: 15, Y: 75");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: efsvitek, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.EnergyField] + ", Color: 0x0000, X: 15, Y: 90");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_Svitky, cilBag, "Name: ivm, Amount: " + GetRefullAmountReserve(procento, 5) + ", Graphic: " + Magery.SpellScrool[StandardSpell.GreaterHeal] + ", Color: 0x0000, X: 15, Y: 105");

        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: vialofblood, Amount: " + GetRefullAmountReserve(procento, 16) + ", Graphic: 0x0F7D, Color:  0x0000, X: 20, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: bone, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F7E, Color:  0x0000, X: 30, Y: 131");
        ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_NekroRegy, cilBag, "Name: excap, Amount: " + GetRefullAmountReserve(procento, 8) + ", Graphic: 0x0F83, Color:  0x0000, X: 40, Y: 131");
      }

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: locky, Amount: 25, Graphic: 0x14FB, Color: 0x0000, X: 137, Y: 65");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: magiclocky, Amount: 3, Graphic: 0x14FB, Color: 0x0B18  , X: 137, Y: 80");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: modraryba, Amount: 2, Graphic: 0x09CD, Color: 0x084C, X: 137, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani, cilBag, "Name: zelenaryba, Amount: 2, Graphic: 0x09CD, Color: 0x0850, X: 120, Y: 90");
      //ItemRequip.RefullCommon(VAL_TowerGuildSecureChest, cilBag, "Name: salat, Amount: 2, Graphic: 0x09EC, Color: 0x06AB, X: 120, Y: 121");
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Verite, cilBag, "Name: Verite s zbran, Amount: 1, Graphic: 0xFFFF, Color: 0x08A1, X: 90, Y: 65");//TODO asi kudlicku pro support
      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_RybyOstani_Pracka, cilBag, "Name: pracka, Amount: 1, Graphic: 0x1008  , Color: 0x0000  , X: 110, Y: 121");

      ItemRequip.RefullCommon(VAL_TowerGuildSecureChest_SperkyRegy_MagRegy, cilBag, "Name: magregy, Amount: " + GetRefullAmountReserve(procento2, 700) + ", X: 15, Y: 141");

      ItemRequip.RefullSperkyClear(VAL_TowerGuildSecureChest_SperkyRegy, cilBag, "Name: Great Reflex Ring, Count: 1, Amount: 6", "Name: Reflex Ring, Count: 1, Amount: 4");
      ItemRequip.RefullKlamakyClear(VAL_TowerGuildSecureChest_AnimalBoxy, cilBag, "Name: Sheep, Count: 4, X: 40, Y: 120", "Name: Bull|Cow, Count: 8, X: 60, Y: 120");

      ItemRequip.RefullKade(0, cilBag, "Name: Strength, Quality: Greater, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Blood, Amount: 200");
      ItemRequip.RefullKade(0, cilBag, "Name: Nightsight, Quality: None, Amount: 100");
      ItemRequip.RefullKade(0, cilBag, "Name: Refresh, Quality: Total, Amount: 100");

      ItemRequip.RefullKade(0, cilBag, "Name: Total Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 50));
      ItemRequip.RefullKade(0, cilBag, "Name: Mana Refresh, Quality: None, Amount: " + GetRefullAmountReserve(procento, 100));
      ItemRequip.RefullKade(0, cilBag, "Name: Shrink, Quality: None, Amount: 50");

      ItemHelper.SortBasicBackpack();
      Jewelry.SetridSperky();
    }
  }
}


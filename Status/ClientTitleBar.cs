using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using System.Runtime.CompilerServices;

namespace CalExtension.UI.Status
{
  [RuntimeObject]
  public class ClientTitleBar
  {
    class ItemInfo
    {
      public string Name;
      public int Amount;

      public override string ToString()
      {
        return Name + ": " + Amount;
      }
    }

    private int lastHits;
    private int lastMana;
    private int lastStam;
    private int lastStr;
    private int lastInt;
    private int lastDex;
    private int lastGold;

    private UOPlayer player;
    private readonly List<object> data = new List<object>();

    public ClientTitleBar()
    {
      Core.LoginComplete += new EventHandler(Core_LoginComplete);
      Core.Disconnected += new EventHandler(Core_Disconnected);
      this.lastUpdate = DateTime.Now;
      if (Core.LoggedIn)
        Initialize();
    }

    void Core_Disconnected(object sender, EventArgs e)
    {
      player.Changed -= new ObjectChangedEventHandler(Player_Changed);
      data.Clear();
    }

    void Core_LoginComplete(object sender, EventArgs e)
    {
      Initialize();
    }

    private void Initialize()
    {
      data.Clear();

      data.Add("|");
      AddItem("MR", CalExtension.UOExtensions.Reagent.MandrakeRoot.Graphic, CalExtension.UOExtensions.Reagent.MandrakeRoot.Color);
      AddItem("BM", CalExtension.UOExtensions.Reagent.BloodMoss.Graphic, CalExtension.UOExtensions.Reagent.BloodMoss.Color);
      AddItem("SS", CalExtension.UOExtensions.Reagent.SpidersSilk.Graphic, CalExtension.UOExtensions.Reagent.SpidersSilk.Color);
      AddItem("SA", CalExtension.UOExtensions.Reagent.SulphurousAsh.Graphic, CalExtension.UOExtensions.Reagent.SulphurousAsh.Color);
      AddItem("GA", CalExtension.UOExtensions.Reagent.Garlic.Graphic, CalExtension.UOExtensions.Reagent.Garlic.Color);
      AddItem("NS", CalExtension.UOExtensions.Reagent.Nightshade.Graphic, CalExtension.UOExtensions.Reagent.Nightshade.Color);
      AddItem("BP", CalExtension.UOExtensions.Reagent.BlackPearl.Graphic, CalExtension.UOExtensions.Reagent.BlackPearl.Color);
      AddItem("GS", CalExtension.UOExtensions.Reagent.Ginseng.Graphic, CalExtension.UOExtensions.Reagent.Ginseng.Color);


      data.Add("|");
      AddItem("GH", CalExtension.UOExtensions.Potion.Heal.DefaultGraphic, CalExtension.UOExtensions.Potion.Heal.DefaultColor);
      AddItem("GS", CalExtension.UOExtensions.Potion.Strength.DefaultGraphic, CalExtension.UOExtensions.Potion.Strength.DefaultColor);
      AddItem("GC", CalExtension.UOExtensions.Potion.Cure.DefaultGraphic, CalExtension.UOExtensions.Potion.Cure.DefaultColor);
      AddItem("TR", CalExtension.UOExtensions.Potion.Refresh.DefaultGraphic, CalExtension.UOExtensions.Potion.Refresh.DefaultColor);
      AddItem("TMR", CalExtension.UOExtensions.Potion.TotalManaRefresh.DefaultGraphic, CalExtension.UOExtensions.Potion.TotalManaRefresh.DefaultColor);
      //AddItem("MR", CalExtension.UOExtensions.Potion.TotalManaRefresh.DefaultGraphic, CalExtension.UOExtensions.Potion.TotalManaRefresh.DefaultColor);
      data.Add("|");
      AddItem("B", 0x0E21, 0);
      AddItem("BB", 0x0E20, 0);

      player = World.Player;
      player.Changed += new ObjectChangedEventHandler(Player_Changed);

      UpdateText();
    }

    public void AddItem(string name, Graphic type, UOColor color)
    {
      var sc = new SupplyCounter(UO.Backpack, type, color);
      var item = new ItemInfo { Name = name, Amount = sc.CurrentAmount };

      data.Add(item);

      sc.AmountChanged += delegate (object sender, EventArgs e)
      {
        item.Amount = sc.CurrentAmount;
        UpdateText();
      };
    }

    void Player_Changed(object sender, ObjectChangedEventArgs e)
    {
      if (player.Hits != lastHits || player.Mana != lastMana || player.Stamina != lastStam || player.Strenght != lastStr || player.Intelligence != lastInt || player.Strenght != lastDex || player.Gold != lastGold)
      {
        UpdateText();

        lastHits = player.Hits;
        lastMana = player.Mana;
        lastStam = player.Stamina;

        lastStr = player.Strenght;
        lastInt = player.Intelligence;
        lastDex = player.Dexterity;
        lastGold = player.Gold;
      }
    }

    private DateTime lastUpdate;

    private void UpdateText()
    {
      if (!CalebConfig.UseWatcher)
      {
        StringBuilder sb = new StringBuilder();

        // Player stats
        sb.AppendFormat("UO - {0}|{1} / {2}|{3} / {4}|{5}", player.Hits, player.Strenght, player.Mana, player.Intelligence, player.Stamina, player.Dexterity);

        // Items
        foreach (var i in data)
        {
          sb.Append(" ");
          sb.Append(i);
        }

        sb.AppendFormat("|{0}", World.Player.Gold);

        if (Client.Text != sb.ToString() && (DateTime.Now - lastUpdate).TotalMilliseconds > 1000)
        {
          Client.Text = sb.ToString();
          lastUpdate = DateTime.Now;
        }
      }
    }
  }
}

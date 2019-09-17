using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using Phoenix.Communication;
using Caleb.Library.CAL.Business;
using CalExtension;
using System.Data;
using Caleb.Library;
using CalExtension.UOExtensions;
using CalExtension.Skills;
using System.Collections;

namespace DP_Scripts
{
  public class Lumberjacking2
  {
    public static Graphic HatchetGraphic = 0x0F43;
    public static Graphic LogGraphic = 0x1BDD;
    public static List<Graphic> TreeGraphic { get { return new List<Graphic>() { 0x0cca, 0x0ccb, 0x0ccc, 0x0ccd, 0x0cd0, 0x0cd3, 0x0cd6, 0x0cd8, 0x0cda, 0x0cdd, 0x0ce0, 0x0ce3, 0x0d01 }; } }



    //---------------------------------------------------------------------------------------------

    public enum LogColor
    {
      Log = 0x0000,
      Plum = 0x0979,
      Fir = 0x06D3,
      Lime = 0x0972,
      Oak = 0x05A6,
      Eobny = 0x0522,

    }


    //---------------------------------------------------------------------------------------------
    private List<string> dennyLogList;
    public Serial Container;
    public Serial InnerContainer;
    public Layer ContainerLayer = Layer.None;
    public List<IUOPosition> ContainerPosition;
    public IUOPosition LastPosition;
    public List<IUOPosition> RessPositionPath;//IUOPosition RessPosition;
    public IUOPosition StartPosition;
    public int MaxN = 0;
    public int MaxE = 0;
    public int MaxS = 0;
    public int MaxW = 0;
    public int WeightOffset = 70;
    public int NastrojLoad = 6;
    public bool WeightLimitReached { get { return World.Player.Weight > (World.Player.MaxWeight - this.WeightOffset); } }
    public bool WeightLimitReachedSmelted { get { return World.Player.Weight > (World.Player.MaxWeight - 150); } }
    public int MaxForensicSkill = 0;
    public bool EnableVisitorInfo = true;
    public VisitorInfoList Visitors;
    public int ContainerDistance = 0;
    public Graphic NastrojGraphic;
    protected Dictionary<UOColor, int> allTimeStatistic;
    protected List<IUOPosition> Trees;
    public ushort SearchSuqareSize = 300;

    public Lumberjacking2()
    {
      Game.CurrentGame.Mode = GameMode.Working;
      World.CharacterAppeared += World_CharacterAppeared;
      this.dennyLogList = new List<string>();
      this.Visitors = new VisitorInfoList();
      this.NastrojGraphic = HatchetGraphic;

      allTimeStatistic = new Dictionary<UOColor, int>();
      allTimeStatistic.Add(0x0000, 0);//log
      allTimeStatistic.Add(0x0979, 0);//Plum
      allTimeStatistic.Add(0x06D3, 0);//Fir
      allTimeStatistic.Add(0x0972, 0);//Lime
      allTimeStatistic.Add(0x05A6, 0);//Oak
      allTimeStatistic.Add(0x0522, 0);//Ebony

      Trees = new List<IUOPosition>();
    }

    //--------------------------------------------------------------------------------------------- 

    ~Lumberjacking2()
    {
      World.CharacterAppeared -= World_CharacterAppeared;
    }


    //exec startlumber2 0x40178E62 0x4010DB1C "1216.2146" "1192.2155" "" "" 0 6 true "0,0,0,0"
    //exec StartLumber2 0x402B77DF 0x402B77DF "3347.351" "3353.292|3353.288" "log" "" 0 6 true "0,0,0,0"


    //3353.292|3353.288 - muj barak Scerfice port resskriz
    //exec StartLumber2 0x402B77DF 0x402B77DF "3347.351" "" "log" "3364.378|3364.384|3368.384|3372.381|3372.378" 700 6 true "0,0,0,0" //debug

    //Muj domek                        
    //                  Kontejner  Pylik      Kontejner  Ress Vyhaz  Forensic Sekyry Navstevnici Hranice E S W N
    //exec StartLumber2 0x402B77DF 0x402B77DF "3347.351" "3353.292|3353.288" "log" "" 700 6 true "0,0,3300,0"
    //    StartLumber2 0x402DA742 0x4020B2A4 "1263.761" "973.789" "log" "" 100 6 true ""
    [Executable("StartLumber2")]
    public static void StartLumber(Serial container, Serial innerContainer, string containerPosition, string ressPosition, string deny, string trees, int maxForesnicSkill, int nastrojLoad, bool enableVisitor, string ESWN)
    {
      Lumberjacking2 lumber = new Lumberjacking2();
      lumber.StartPosition = new UOPositionBase(World.Player.X, World.Player.Y, 0);

      if (!String.IsNullOrEmpty(ESWN))
      {
        string[] esvnSplit = ESWN.Split(new char[] { ',' });

        if (esvnSplit.Length > 0)
          lumber.MaxE = Int32.Parse(esvnSplit[0]);
        if (esvnSplit.Length > 1)
          lumber.MaxS = Int32.Parse(esvnSplit[1]);
        if (esvnSplit.Length > 2)
          lumber.MaxW = Int32.Parse(esvnSplit[2]);
        if (esvnSplit.Length > 3)
          lumber.MaxN = Int32.Parse(esvnSplit[3]);
      }


      if (String.IsNullOrEmpty(trees))
      {
        int currentX = lumber.StartPosition.X.GetValueOrDefault();
        int currentY = lumber.StartPosition.Y.GetValueOrDefault();
        int minX = 0;
        int minY = 0;
        int maxX = 0;
        int maxY = 0;

        minX = (currentX - lumber.SearchSuqareSize < 0 ? 0 : currentX - lumber.SearchSuqareSize);
        maxX = (currentX + lumber.SearchSuqareSize > 6000 ? 6000 : currentX + lumber.SearchSuqareSize);
        minY = (currentY - lumber.SearchSuqareSize < 0 ? 0 : currentY - lumber.SearchSuqareSize);
        maxY = (currentY + lumber.SearchSuqareSize > 6000 ? 6000 : currentY + lumber.SearchSuqareSize);

        if (lumber.MaxE > 0)
          maxX = Math.Min(maxX, lumber.MaxE);
        if (lumber.MaxS > 0)
          maxY = Math.Min(maxY, lumber.MaxS);
        if (lumber.MaxW > 0)
          minX = Math.Max(minX, lumber.MaxW);
        if (lumber.MaxN > 0)
          minY = Math.Max(minY, lumber.MaxN);

        UOPositionCollection allTrees = new UOPositionCollection();
        allTrees.Load(String.Format("IsTree=1 AND X>={0} AND X<={1} AND Y>={2} AND Y<={3}", minX, maxX, minY, maxY));
        allTrees = allTrees.SortByOptimalTrack(new Robot().ActualPosition);

        lumber.Trees.AddRange(allTrees.ToArray());
      }
      else 
        lumber.Trees.AddRange(UOPositionBase.ParseList(trees));

      lumber.Container = container;
      lumber.InnerContainer = innerContainer;
      lumber.ContainerPosition = UOPositionBase.ParseList(containerPosition);
      lumber.RessPositionPath = UOPositionBase.ParseList(ressPosition);
      lumber.MaxForensicSkill = maxForesnicSkill;
      lumber.NastrojLoad = nastrojLoad;
      lumber.EnableVisitorInfo = enableVisitor;

      foreach (string s in deny.Split(new char[] { ',' }))
        lumber.dennyLogList.Add(s);

      UO.Print(0x0035, "StartLumber s nasledujicimi parametry:");
      UO.Print(0x0035, "Container:" + lumber.Container);
      UO.Print(0x0035, "InnerContainer:" + lumber.InnerContainer);
      UO.Print(0x0035, "ContainerPosition:" + UOPositionBase.ListToString(lumber.ContainerPosition));
      UO.Print(0x0035, "RessPositionPath:" + UOPositionBase.ListToString(lumber.RessPositionPath));
      UO.Print(0x0035, "StartPosition:" + lumber.StartPosition);
      UO.Print(0x0035, "Trees:" + lumber.Trees.Count);
      UO.Print(0x0035, "MaxForensicSkill:" + lumber.MaxForensicSkill);
      UO.Print(0x0035, "KumpLoad:" + lumber.NastrojLoad);
      UO.Print(0x0035, "EnableVisitorInfo:" + lumber.EnableVisitorInfo);
      UO.Print(0x0191, "DenyOres:" + String.Join(",", lumber.dennyLogList.ToArray()));



      lumber.Lumber();
    }

    //---------------------------------------------------------------------------------------------

    private void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      if (this.EnableVisitorInfo)
      {
        VisitorInfo info = this.Visitors[e.Serial];
        if (String.IsNullOrEmpty(info.Name))
        {
          info.Character.Click();
          Game.Wait();
          info.Name = info.Character.Name;
        }

        if (info.LastVisit == null || (DateTime.Now - info.LastVisit.VisitTime).TotalMinutes > 1)
          info.Visits.Add(new VisitInfo() { VisitTime = DateTime.Now });
      }
    }

    //---------------------------------------------------------------------------------------------

    protected Robot robot;
    public Robot Robot
    {
      get
      {
        if (this.robot == null)
        {
          this.robot = new Robot();
          //this.robot.EnableLog = true;
        }
        return this.robot;
      }
    }

    //---------------------------------------------------------------------------------------------

    public UOItem Nastroj
    {
      get
      {
        UOItem n = new UOItem(Serial.Invalid);
        if (!(n = World.Player.Layers.FindType(NastrojGraphic)).Exist)
        {
          World.Player.Backpack.Items.FindType(NastrojGraphic);
        }
        if (!n.Exist) UO.Print("Nemas nastroj!");
        return n;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected bool BezVylozit()
    {
      if (this.ContainerPosition != null)
      {
        UO.Print("BezVylozit");

        if (this.Robot.GoTo(this.ContainerPosition, this.ContainerDistance))
        {
          Serial container;

          if (this.ContainerLayer == Layer.Bank)
          {
            UO.Say("Bank");
            Game.Wait();
            container = World.Player.Layers[this.ContainerLayer];
          }
          else
            container = this.Container;

          UOItem mainCont = new UOItem(container);
          ItemHelper.EnsureContainer(mainCont);
          Game.Wait(250);
          UO.Backpack.Use();
          Game.Wait(250);

          Serial dropContainer = container;
          if (this.InnerContainer != null)
            dropContainer = this.InnerContainer;

          UOItem ingot = new UOItem(Serial.Invalid);
          while (dropContainer.IsValid && ((ingot = FindLog(UO.Backpack.Items)).Exist))
          {
            ingot.Move(100, dropContainer);
            Game.Wait();
          }

          Game.Wait();

          bool opened = false;
          while (World.Player.Backpack.AllItems.Count(NastrojGraphic) <= this.NastrojLoad)
          {
            if (!opened)
            {
              ItemHelper.OpenContainerRecursive(container);
              Game.Wait();
              opened = true;
            }
            if (mainCont.AllItems.FindType(NastrojGraphic).Exist)
            {
              mainCont.AllItems.FindType(NastrojGraphic).Move(1, World.Player.Backpack);
              Game.Wait();
            }
            else
              break;
          }

          if (this.ContainerPosition.Count > 1)
          {
            List<IUOPosition> reverse = new List<IUOPosition>();
            reverse.AddRange(this.ContainerPosition.ToArray());
            reverse.Reverse();

            this.Robot.GoTo(reverse, this.ContainerDistance);
          }

          return this.BezLast();
        }
        else
          UO.Print("BezVylozit - !GoTo ContainerPosition");
      }
      else
        UO.Print("BezVylozit - ContainerPosition == null");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected bool BezSeResnout()
    {
      bool ressSuccess = false;
      if (this.RessPositionPath != null)
      {
        UO.Print("BezSeResnout");
        int gotoDistance = this.RessPositionPath.Count == 1 ? 1 : 0;

        if (this.Robot.GoTo(this.RessPositionPath, gotoDistance))
        {
          UOCharacter resser = null;

          foreach (UOCharacter ch in World.Characters)
          {
            if (ch.Notoriety == Notoriety.Innocent || ch.Notoriety == Notoriety.Invulnerable || ch.Notoriety == Notoriety.Neutral || ch.Distance < 10)
            {
              resser = ch;
              break;
            }
          }

          if (World.Ground.FindType(0x1E5D).Exist)////0x1E5D ress kriz
          {
            UOItem ressKriz = World.Ground.FindType(0x1E5D);
            if (ressKriz > 3)
            {
              this.Robot.GoTo(ressKriz.X, ressKriz.Y, 1, 100);
            }

            ressKriz.Click();
            Game.Wait();
            World.Player.ChangeWarmode(WarmodeChange.Switch);
            Game.Wait();
            World.Player.ChangeWarmode(WarmodeChange.War);
            Game.Wait();
            ressKriz.Use();
            Game.Wait();
            World.Player.RequestStatus(1000);

            ressSuccess = !UO.Dead;
          }
          else if (resser != null && resser.Exist)
          {
            if (this.Robot.GoTo(resser.X, resser.Y, 2, 50))
            {
              resser.Click();
              Game.Wait();
              World.Player.ChangeWarmode(WarmodeChange.Switch);
              Game.Wait();
              World.Player.ChangeWarmode(WarmodeChange.War);
              resser.Tell("ress");
              Game.Wait();
              World.Player.RequestStatus(1000);

              ressSuccess = !UO.Dead;
            }
            else
              UO.Print("BezSeResnout - !Goto resser");
          }
          else
            UO.Print("BezSeResnout - !resser");
        }
        else
          UO.Print("BezSeResnout - !GoTo ForgePosition");

        if (ressSuccess && this.RessPositionPath.Count > 1)
        {
          List<IUOPosition> reverse = new List<IUOPosition>();
          reverse.AddRange(this.RessPositionPath.ToArray());
          reverse.Reverse();

          this.Robot.GoTo(reverse, gotoDistance);
        }
      }
      else
      {
        UO.Print("BezSeResnout - RessPosition == null");
        Game.Wait(1500);
      }

      return ressSuccess;
    }

    //---------------------------------------------------------------------------------------------

    public bool BezLast()
    {
      if (this.LastPosition != null)
      {
        UO.Print("BezLast");

        if (this.Robot.GoTo(this.LastPosition))
          return true;
        else
          UO.Print("BezLast - !GoTo LastPosition");
      }
      else
        UO.Print("BezLast - LastPosition == null");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected UOItem MyDeadBody()
    {
      foreach (UOItem item in World.Ground)
      {
        if (item.Distance < 3 && item.Graphic == 0x2006)
        {
          if (String.IsNullOrEmpty(item.Name))
          {
            item.Click();
            Game.Wait();
          }

          if (String.IsNullOrEmpty(World.Player.Name))
          {
            World.Player.Click();
            Game.Wait();
          }

          if ((String.Empty + item.Name).Contains("Body of " + World.Player.Name))
          {
            return item;
          }
        }
      }
      return null;
    }

    //---------------------------------------------------------------------------------------------

    protected void VylotSiTelo()
    {
      UOItem myDeadBody = this.MyDeadBody();
      if (myDeadBody != null && myDeadBody.Exist)
      {
        if (!myDeadBody.Opened)
        {
          myDeadBody.Use();
          Game.Wait();
        }

        foreach (UOItem lootItem in myDeadBody.AllItems)
        {
          lootItem.Move(10000, World.Player.Backpack);
          Game.Wait();
        }
      }

      foreach (UOItem lootItem in World.Ground)
      {
        if (lootItem.Distance < 4 && lootItem.Graphic == HatchetGraphic)
        {
          lootItem.Move(10000, World.Player.Backpack);
          Game.Wait();
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    int lastPositionIndex = -1;
    public bool GoToNext()
    {
      if (this.Trees.Count > 0)
      {
        lastPositionIndex++;

        if (lastPositionIndex > this.Trees.Count - 1)
          lastPositionIndex = 0;

       UO.Print("GoToNext - " + this.Trees[lastPositionIndex] + " - " + lastPositionIndex + "/" + this.Trees.Count);
        return this.Robot.GoTo(this.Trees[lastPositionIndex], 1, 100);
      }
      else
        UO.Print("GoToNext - Trees == 0");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    protected SekInfo SekTree(StaticTarget tree)
    {
      SekInfo info = new SekInfo();
      Journal.Clear();

      JournalEventWaiter jew = new JournalEventWaiter(true, "akce skoncila");
      Nastroj.Use();
      UO.WaitTargetTile(tree.X, tree.Y, tree.Z, tree.Graphic);
      jew.Wait(10000 + LatencyMeasurement.CurrentLatency);

      info.Mined = Journal.Contains(true, "There are no logs left here to chop");
      info.Wrong = Journal.Contains(true, "You can't think of a way to use that item", "That's too far away to chop", "Try chopping a tree");
      info.Special = Journal.Contains(true, "You found something special");

      return info;
    }

    //---------------------------------------------------------------------------------------------

    public void Lumber()
    {
      int iteration = 0;
      World.Player.RequestStatus(1000);
      World.Player.Click();
      Game.Wait();

      bool doLumber = this.GoToNext();

      while (doLumber)
      {
        UO.Print("doLumber" + iteration);
        iteration++;

        UO.Print("doLumber" + lastPositionIndex + "  / " + this.Trees.Count);

        IUOPosition current = this.Trees[lastPositionIndex];

        UOItem myDeadBody = this.MyDeadBody();
        bool dead = UO.Dead;
        bool returnToLast = false;
        bool goToNext = false;

        bool trainForensic = this.MaxForensicSkill > 0 && !dead && myDeadBody != null && myDeadBody.Exist;
        this.LastPosition = new UOPosition(World.Player.X, World.Player.Y, 0);
        UO.DeleteJournal();

        if (Game.CurrentGame.WorldSave())
        {
          Game.Wait(30000);
          if (!dead)
          {
            Nastroj.Move(1, World.Player.Backpack);
            Game.Wait(500);
            Nastroj.Click();
          }
          Game.Wait();

          if (World.Player.Backpack.AllItems.FindType(0x0FF0).Exist)
          {
            World.Player.Backpack.AllItems.FindType(0x0FF0).Move(1, World.Player.Backpack);//Sychr test na zasek
            Game.Wait();
          }
          if (myDeadBody != null && myDeadBody.Exist)
          {
            myDeadBody.Click();
          }
        }

        if (this.EnableVisitorInfo)
        {
          VisitorInfo visitor;
          if (this.NavstevnikHandle(out visitor).Result == ActionResult.Continue)
            continue;

          if (visitor != null && UO.Dead && visitor.LastVisit != null)
            visitor.LastVisit.ZabilMe = true;
        }

        if (World.Player.Warmode)
          World.Player.ChangeWarmode(WarmodeChange.Peace);

        if (dead)
        {
          Game.Wait(60000);
          this.BezSeResnout();
          this.BezLast();
          this.VylotSiTelo();
          continue;
        }

        if (World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0979).Amount >= 3
                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x06D3).Amount >= 2
                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0972).Amount >= 1
                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x05A6).Amount >= 1
                || World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0522).Amount >= 1
          )
        {
          this.BezVylozit();
          returnToLast = true;
        }

          if (this.WeightLimitReached)
        {
          this.BezVylozit();
          returnToLast = true;
        }

        if (!dead && !Nastroj.Exist)
        {
          this.BezVylozit();
          returnToLast = true;
        }

        if (returnToLast)
          this.Robot.GoTo(this.LastPosition);

        this.VylotSiTelo();
        this.DropDeny();

        if (trainForensic && SkillsHelper.GetSkillValue("ForensicEvaluation").Value < this.MaxForensicSkill)
        {
          Game.Wait(150);
          UO.Print("Train - ForensicEvaluation");
          UO.WaitTargetObject(myDeadBody);
          UO.UseSkill(StandardSkill.ForensicEvaluation);
          Journal.WaitForText(true, 2500, "this is body of","You can tell nothing about the corpse.");// Game.Wait(1500);
          Game.Wait(150);
          continue;
        }
    
        StaticTarget target = new StaticTarget(Serial.Invalid, current.X.GetValueOrDefault(), current.Y.GetValueOrDefault(), (sbyte)current.Z.GetValueOrDefault(), TreeGraphic[0]);
        SekInfo sek = this.SekTree(target);
        if (sek.Mined || sek.Wrong)
          goToNext = true;

        this.SeberLogy();

        if (goToNext)
          this.GoToNext();
      }
    }

    //---------------------------------------------------------------------------------------------

    public ActionInfo NavstevnikHandle(out VisitorInfo visitor)
    {
      ActionInfo result = new ActionInfo();

      visitor = this.Visitors.LastVisitor;
      if (visitor != null && visitor.Character.Exist && visitor.Character.Distance < 10 && !UO.Dead)
      {
        VisitInfo visit = visitor.LastVisit;
        if (visit != null)
        {
          if (!visit.Snaped)
          {
            visitor.Character.Click();
            Game.Wait(250);
            ScreenCapture.ScreenCapture capture = new ScreenCapture.ScreenCapture();
            capture.snap((visitor.Name + String.Empty).Replace(" ", "_") + visit.VisitTime.ToString("yyyyMMdd_HHmmssfff"));
            visit.Snaped = true;
          }

          if ((visitor.Character.Model == 0x0190 || visitor.Character.Model == 0x0191) &&  (visitor.Character.Notoriety == Notoriety.Murderer || visitor.Character.Notoriety == Notoriety.Criminal || visitor.Character.Notoriety == Notoriety.Enemy))//Toto male/ female 
                                                                                                                                                                       //hlasky dle deni doby atd.
          {
            if (!World.Player.Hidden)
            {
              if (visitor.KillCount > 0)
              {
                if (visit.Pruchod == 0)
                {
                  UO.Say("Jsi zpět krutý vrahu!");
                  Game.Wait(3500);
                  UO.Say("Přišel jsi opět zabít svou nebohou oběť, budiž... ");
                  Game.Wait(500);
                }
                else if (visit.Pruchod == 1)
                {
                  UO.Say("Můj nebohý úděl je již po " + (visitor.KillCount + 1) + " skonat tvou rukou..");
                  Game.Wait(6000);
                  UO.Say("Nechť jsou k tobě bozi miloství ... za zlo které pácháš ...");
                }
              }
              else if (visitor.Visits.Count > 1)
              {
                if (visit.Pruchod == 0)
                {
                  UO.Say("Zdravím tě můj největší ze zbojníků!!!");
                  Game.Wait(3500);
                  UO.Say("Jsem potěšen, že tvá velikost mě poctila svou návševou!!!");
                  Game.Wait(500);
                }
                else if (visit.Pruchod == 1)
                {
                  UO.Say("Všem jsem již vylíčil tvojí hrůznost a velikost v boji!!!");
                  Game.Wait(6000);
                  UO.Say("Pouze ta slova, je naplňují hrůzou");
                  Game.Wait(500);
                }
                else if (visit.Pruchod == 2)
                {
                  UO.Say("Věčná sláva tobě můj pane " + visitor.Name + "!!");
                  //TODO
                }
              }
              else
              {
                if (visit.Pruchod == 0)
                {
                  UO.Say("Aaaááááááá loupežník!!!");
                  Game.Wait(3500);
                  UO.Say("Vzdávam se *odhazuje sekyru*");
                  Game.Wait(500);
                  //Nastroj.DropHere();

                }
                else if (visit.Pruchod == 1)
                {
                  UO.Say("Prosím o slitovaní óóó všemocný nejrůznější vladce zdejších lesů..");
                  Game.Wait(6000);
                  UO.Say("Ušetři můj bídný nicotný život... můsím celý den stravit v lese... ");
                  Game.Wait(6000);
                  UO.Say("abych užvil/a svou rodinu...");
                }
                else if (visit.Pruchod == 2)
                {
                  UO.Say("Pokud mě ušetříš, budu šiřit slova o tvé hrůze a nepřemožitelnosti...");
                  Game.Wait(6000);
                  UO.Say("Óóó nejhruznější z nejhruznějších !!!");
                  Game.Wait(2000);
                }
                else if (visit.Pruchod == 3)
                {
                  UO.Say("Věčná sláva tobě největší " + visitor.Name + "!!");
                  //TODO
                }
              }

              visit.Pruchod++;
            }
            else
            {
              Game.Wait();
              UO.Print("Cekam v HIDU");
            }
            Game.Wait(3500);
            result.Result = ActionResult.Continue;
          }

        }
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------

    protected void SeberLogy()
    {

      foreach (UOItem log in World.Ground)
      {
        UO.DeleteJournal();
        if (log.Distance < 3 && log.Graphic == LogGraphic)
        {
          bool leave = false;
          if (log.Color == 0x0000 && Array.IndexOf(dennyLogList.ToArray(), "log") > -1)
            leave = true;
          else if (log.Color == 0x06D3 && Array.IndexOf(dennyLogList.ToArray(), "fir") > -1)
            leave = true;
          else if (log.Color == 0x0979 && Array.IndexOf(dennyLogList.ToArray(), "plum") > -1)
            leave = true;
          else if (log.Color == 0x0972 && Array.IndexOf(dennyLogList.ToArray(), "lime") > -1)
            leave = true;
          else if (log.Color == 0x05A6 && Array.IndexOf(dennyLogList.ToArray(), "oak") > -1)
            leave = true;
          else if (log.Color == 0x0522 && Array.IndexOf(dennyLogList.ToArray(), "ebony") > -1)//TODO ebony
            leave = true;

          if (!leave)
          {

            this.allTimeStatistic[log.Color]++;
            log.Move(10, World.Player.Backpack);

            if (Journal.WaitForText(true, 350, "You put the logs at your feet. It is too heavy.."))
            {
              break;
            }

            Game.Wait();
          }
        }
      }
    }


    //---------------------------------------------------------------------------------------------

    protected void DropDeny()
    {
      if (dennyLogList.Count > 0)
      {
        foreach (UOItem item in Game.CurrentGame.CurrentPlayer.Player.Backpack.AllItems)
        {
          if (item.Graphic == LogGraphic)
          {
            bool drop = false;
            if (item.Color == 0x0000 && Array.IndexOf(dennyLogList.ToArray(), "log") > -1)
              drop = true;
            else if (item.Color == 0x06D3 && Array.IndexOf(dennyLogList.ToArray(), "fir") > -1)
              drop = true;
            else if (item.Color == 0x0979 && Array.IndexOf(dennyLogList.ToArray(), "plum") > -1)
              drop = true;
            else if (item.Color == 0x0972 && Array.IndexOf(dennyLogList.ToArray(), "lime") > -1)
              drop = true;
            else if (item.Color == 0x05A6 && Array.IndexOf(dennyLogList.ToArray(), "oak") > -1)
              drop = true;
            else if (item.Color == 0x0522 && Array.IndexOf(dennyLogList.ToArray(), "ebony") > -1)//TODO ebony
              drop = true;

            if (drop)
            {
              item.DropHere();
              Game.Wait();
            }
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void PrintCurrentLogy()
    {
      Game.PrintMessage(String.Format("Plum: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0979).Amount));
      Game.PrintMessage(String.Format("Fir: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x06D3).Amount));
      Game.PrintMessage(String.Format("Lime: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0972).Amount));
      Game.PrintMessage(String.Format("Oak: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x05A6).Amount));
      Game.PrintMessage(String.Format("Ebony: {0}", World.Player.Backpack.AllItems.FindType(LogGraphic, 0x0522).Amount));
    }

    //---------------------------------------------------------------------------------------------

    protected void PrintLogy()
    {
      Game.PrintMessage(String.Format("Plum: {0}", this.allTimeStatistic[0x0979]));
      Game.PrintMessage(String.Format("Fir: {0}", this.allTimeStatistic[0x06D3]));
      Game.PrintMessage(String.Format("Lime: {0}", this.allTimeStatistic[0x0972]));
      Game.PrintMessage(String.Format("Oak: {0}", this.allTimeStatistic[0x05A6]));
      Game.PrintMessage(String.Format("Ebony: {0}", this.allTimeStatistic[0x0522]));
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindLog(ItemsCollection items)
    {
      foreach (UOItem item in items)
      {
        if (IsLog(item))
          return item;
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindLog(ItemsCollection items, UOColor color)
    {
      foreach (UOItem item in items)
      {
        if (IsLog(item, color))
          return item;
      }
      return new UOItem(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsLog(UOItem ingot)
    {
      if (ingot.Graphic == LogGraphic)
        return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsLog(UOItem ingot, UOColor color)
    {
      if (ingot.Graphic == LogGraphic && ingot.Color == color)
        return true;
      return false;
    }

    //---------------------------------------------------------------------------------------------



    //---------------------------------------------------------------------------------------------

    [Command("printtree")]
    public static void PrintAllTreesCount(int radius)
    {
      IRequestResult r = UO.WaitTargetTile(3348, 339, 4, Graphic.Invariant);
      UO.UseType(0x0F9E);
      //IClientTarget target = UIManager.Target();

      StaticTarget target = ((StaticTarget)r.GetType().GetField("Target").GetValue(r));


      {
        string name = null;
        if (target.Graphic != 0 && target.Graphic < DataFiles.Tiledata.Count)
          name = DataFiles.Tiledata.GetArt(target.Graphic).Name;

        string format = "Tile X={0} Y={1} Z={2} Graphic=0x{3:X4}";
        if (name != null && name.Length > 0)
          format += " Name={4}";

        UO.Print(format, target.X, target.Y, target.Z, target.Graphic, name);

      }
      // UO.Print(((StaticTarget)r.GetType().GetField("Target").GetValue(r))+ "" + " / " + Graphic.Invariant);

      {
        string name = null;
        if (target.Graphic != 0 && target.Graphic < DataFiles.Tiledata.Count)
          name = DataFiles.Tiledata.GetArt(target.Graphic).Name;

        string format = "Tile X={0} Y={1} Z={2} Graphic=0x{3:X4}";
        if (name != null && name.Length > 0)
          format += " Name={4}";

        Notepad.WriteLine(format, target.X, target.Y, target.Z, target.Graphic, name);
        Notepad.WriteLine();
      }
    }
      //int segmentSize = 1;
      //int direction = -1;
      //int currentRadius = 0;
      //int x = World.Player.X;
      //int y = World.Player.Y;


      //List<UOPositionBase> trees = new List<UOPositionBase>();

      //while (currentRadius < radius)
      //{
      //  x = x + segmentSize * direction;

      //  UO.WaitTargetTile(x, y, z, Graphic.Invariant);
      //  //x y 1 

      //  y = y + segmentSize * direction;

      //  //x y 2 

      //  direction = direction * -1;
      //  segmentSize++;
      //  currentRadius = segmentSize / 2;


      //}



    //---------------------------------------------------------------------------------------------

    public void RemoveTree()
    {
      StaticTarget tree = UIManager.Target();
      UO.Print(tree.Graphic);

      if (tree.Graphic != 0)
      {
        UOPosition pos = new UOPosition(tree.X, tree.Y, (ushort)tree.Z);
        if (pos.LoadByPosition())
        {
          if (pos.Remove())
            UO.Print("Strom odebran");
          else
            UO.Print("Strom se nepodarilo odebrat");
        }
        else
          UO.Print("Strom neni v DB");
      }
    }

    private static Hashtable waipointHt;
    [Executable]
    public static void PrintTree()
    {
      if (waipointHt == null)
        waipointHt = new Hashtable();


      TargetInfo info = new TargetInfo("").GetTarget();
      if (info.Success && waipointHt[info.Character.Serial] == null)
      {
        waipointHt[info.Character.Serial] = info;
        string name = String.Empty;
        if (info.Object.Exist && String.IsNullOrEmpty(info.Object.Name))
        {
          info.Object.Click();
          Game.Wait(100);
        }

        name = info.Object.Name;

        Notepad.Write(info.StaticTarget.X + "." + info.StaticTarget.Y + (String.IsNullOrEmpty(name) ? "" : "//" + name) + "|");

        info.Character.PrintMessage("[Print...]");
      }
      else
        World.Player.PrintMessage("Neni strop", Game.Val_LightPurple);
      
    }

    //---------------------------------------------------------------------------------------------

    public void AddTree()
    {
      DataTable dt = Cal.Engine.GetOtSetDataTable(new UOPosition().DbViewNameForLoad);

      if (dt.Rows.Count > 0)
        Game.PrintMessage("Tree count: " + dt.Select("IsTree=1").Length);

      StaticTarget tree = UIManager.Target();
      UO.Print(tree.Graphic);

      if (tree.Graphic != 0)
      {
        UOPosition pos = new UOPosition(tree.X, tree.Y, (ushort)tree.Z);
        pos.EnsureLoadByPosition();
        pos.IsTree = true;
        pos.TypeChecked = true;
        pos.Stepable = false;
        if (pos.Save())
          Game.PrintMessage("Tree add: " + pos.ToString());
      }
    }



    //---------------------------------------------------------------------------------------------


    [Executable]
    [BlockMultipleExecutions]
    public static void AddTreePosition()
    {
      Lumberjacking2 lumber = new Lumberjacking2();
      lumber.AddTree();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void RemoveTreePosition()
    {
      Lumberjacking2 lumber = new Lumberjacking2();
      lumber.RemoveTree();
    }
  }

    //---------------------------------------------------------------------------------------------

 

  public class SekInfo
  {
    public bool Mined = false;
    public bool Special = false;
    public bool Wrong = false;
  }



}

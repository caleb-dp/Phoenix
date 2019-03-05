//using System;
//using System.Collections.Generic;
//using System.Text;
//using Phoenix;
//using Phoenix.WorldData;
//using System.Linq;
//using System.Windows.Forms;
//using System.Threading;

//namespace Phoenix.Scripts
//{
//  public class Lumber
//  {
//    public Lumber()
//    {
//      treeGraphics = new List<Graphic>() {
//                0x0cca, 0x0ccb, 0x0ccc, 0x0ccd, 0x0cd0, 0x0cd3, 0x0cd6, 0x0cd8, 0x0cda, 0x0cdd, 0x0ce0, 0x0ce3, 0x0d01
//            };
//    }

//    private List<Graphic> treeGraphics;

//    private bool MoveToTile(StaticItem tree)
//    {
//      UO.PrintInformation("Moving to " + tree.X + "," + tree.Y + "," + tree.Z);

//      Func<bool> isInRange = new Func<bool>(() => {
//        return Math.Max(Math.Abs(World.Player.X - tree.X), Math.Abs(World.Player.Y - tree.Y)) <= 2;
//      });

//      while (!isInRange())
//      {
//        while (!isInRange() && World.Player.X > tree.X && World.Player.Y > tree.Y && MakeStep(7))
//          ;

//        while (!isInRange() && World.Player.X < tree.X && World.Player.Y < tree.Y && MakeStep(3))
//          ;

//        while (!isInRange() && World.Player.X > tree.X && World.Player.Y < tree.Y && MakeStep(5))
//          ;

//        while (!isInRange() && World.Player.X < tree.X && World.Player.Y > tree.Y && MakeStep(1))
//          ;

//        while (!isInRange() && World.Player.X > tree.X && MakeStep(6))
//          ;

//        while (!isInRange() && World.Player.X < tree.X && MakeStep(2))
//          ;

//        while (!isInRange() && World.Player.Y > tree.Y && MakeStep((byte)0))
//          ;

//        while (!isInRange() && World.Player.Y < tree.Y && MakeStep(4))
//          ;
//      }

//      return true;
//    }

//    private void HarvestTile(StaticItem tree)
//    {
//      UO.PrintInformation("Harvesting at " + tree.X + "," + tree.Y + "," + tree.Z);

//      do
//      {
//        UOItem hatchet = World.Player.Layers[Layer.LeftHand];
//        if (hatchet.Graphic != 0x0F43)
//          hatchet = World.Player.Backpack.AllItems.Where(i => i.Graphic == 0x0f43).FirstOrDefault();
//        if (hatchet == null)
//          throw new ScriptErrorException("Hatchet not found");

//        Journal.Clear();
//        UO.WaitTargetTile(tree.X, tree.Y, tree.Z, tree.Graphic);
//        hatchet.Use();
//        Journal.WaitForText("There are no logs left here to chop.", "You hack at the tree for a while, but fail to produce any useable wood.", "You put the log");
//        UO.Wait(500);
//      } while (!Journal.Contains("There are no logs left here to chop."));
//    }

//    [Executable("lumber"), BlockMultipleExecutions("lumber")]
//    public void Start()
//    {
//      Start(10);
//    }
//    [Executable("lumber"), BlockMultipleExecutions("lumber")]
//    public void Start(int radius)
//    {
//      using (Map map = new Map())
//      {
//        IEnumerable<StaticItem> trees = map.GetStaticItems(World.Player.X - radius, World.Player.Y - radius, World.Player.X + radius, World.Player.Y + radius, i => treeGraphics.Contains(i.Graphic)).OrderBy(i => Math.Max(Math.Abs(World.Player.X - i.X), Math.Abs(World.Player.Y - i.Y)));

//        while (trees.Count() > 0)
//        {
//          StaticItem tree = trees.FirstOrDefault();

//          if (MoveToTile(tree))
//            HarvestTile(tree);

//          UO.PrintInformation("Remaining " + trees.Count() + " trees");

//          trees = trees.Skip(1).OrderBy(i => Math.Max(Math.Abs(World.Player.X - i.X), Math.Abs(World.Player.Y - i.Y)));
//        }
//      }

//      UO.PrintInformation("Lumber finished");
//    }

//    #region Movement

//    private MessageCallback RegisterMessageCallback(bool client, MessageCallback callback, params byte[] opcodes)
//    {
//      foreach (byte opcode in opcodes)
//        if (client)
//          Core.RegisterClientMessageCallback(opcode, callback);
//        else
//          Core.RegisterServerMessageCallback(opcode, callback);

//      return callback;
//    }

//    private bool MakeStep(byte direction)
//    {
//      return MakeStep(direction, 0);
//    }
//    private bool MakeStep(byte direction, int depth)
//    {
//      if (depth >= 8)
//        return false;

//      while (!Step(direction))
//        MakeStep((byte)((direction + 1) % 8), ++depth);

//      return true;
//    }

//    public bool Step(byte direction)
//    {
//      Keys[] directions = {
//                    Keys.PageUp,   // 0
//                    Keys.Right,    // 1
//                    Keys.PageDown, // 2
//                    Keys.Down,     // 3
//                    Keys.End,      // 4
//                    Keys.Left,     // 5
//                    Keys.Home,     // 6
//                    Keys.Up,       // 7
//                };

//      Keys key = directions[direction];
//      return (World.Player.Direction != direction ? Step(key) : true) && Step(key);
//    }

//    public bool Step(Keys key)
//    {
//      bool result = false;

//      using (ManualResetEvent handled = new ManualResetEvent(false))
//      {
//        using (ManualResetEvent requested = new ManualResetEvent(false))
//        {
//          MessageCallback requestedCallback = RegisterMessageCallback(true, (d, p) => {
//            requested.Set();
//            handled.WaitOne(500);
//            return p;
//          }, 0x02);
//          try
//          {
//            UO.Press(key);

//            if (!requested.WaitOne(500))
//              return false;
//          }
//          finally
//          {
//            handled.Set();
//            Core.UnregisterClientMessageCallback(0x02, requestedCallback);
//          }
//        }

//        handled.Reset();

//        using (ManualResetEvent responded = new ManualResetEvent(false))
//        {
//          MessageCallback respondedCallback = RegisterMessageCallback(false, (d, p) => {
//            result = d[0] == 0x22;
//            responded.Set();
//            handled.WaitOne(500);
//            return p;
//          }, 0x21, 0x22);
//          try
//          {
//            if (!responded.WaitOne(40000))
//            {
//              UO.PrintWarning("Walk response timeout");
//              return false;
//            }
//          }
//          finally
//          {
//            handled.Set();
//            Core.UnregisterServerMessageCallback(0x21, respondedCallback);
//            Core.UnregisterServerMessageCallback(0x22, respondedCallback);
//          }
//        }

//      }

//      UO.Wait(400);
//      return result;
//    }

//    #endregion
//  }
//}
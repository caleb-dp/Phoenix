
 
 /////////////////////////////////////////////////////////////////////////
 //
 //     www.ultima.smoce.net
 //     Name: Spiritual
 //
 /////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using Phoenix.Collections;
using System.Security.Permissions;
using System.IO;

namespace Phoenix.Scripts
{



    public class Spirits
    {

        /*
         * Step master for Lilith na Stone Human 
         * Autor:Smoce
         */
        string[] dirf = { "North East", "South East", "South West", "North West", "North", "East", "South", "West", };

        bool ignoreCase = true;
        int timeout = 10000;


        [Command("Lilith")]
        [BlockMultipleExecutions("Lilith")]
        public void Lilithstep()
        {
            bool ends = false;
            UO.Print(0x00B3, "Start detection directory - Activated (Lilith na Stone Human)");
            UO.DeleteJournal();
            do
            {
                UO.DeleteJournal();
                bool sa = funcas();
                if (sa == true)
                {
                    UO.Print(0x00B3, "OK - Detect successful");
                }
                else
                {
                    ends = false;
                    UO.Print("FAIL- Detect successful");
                }
                UO.Wait(50);
            } while (!ends);
        }

        public bool funcas()
        {
            UO.DeleteJournal();
            int select = 0;
            using (JournalEventWaiter obj = new JournalEventWaiter(ignoreCase, dirf))
            {
                obj.Wait(timeout);
            }

            foreach (string value in dirf)
            {
                if (Journal.Contains(value))
                {
                    MakeStep((byte)select);
                    UO.Print("KROK!");
                    return true;
                }
                select++;
            }
            return false;
        }



        #region Movement

        private MessageCallback RegisterMessageCallback(bool client, MessageCallback callback, params byte[] opcodes)
        {
            foreach (byte opcode in opcodes)
                if (client)
                    Core.RegisterClientMessageCallback(opcode, callback);
                else
                    Core.RegisterServerMessageCallback(opcode, callback);

            return callback;
        }

        private bool MakeStep(byte direction)
        {
            return MakeStep(direction, 0);
        }
        private bool MakeStep(byte direction, int depth)
        {
            if (depth >= 8)
                return false;

            while (!Step(direction))
                MakeStep((byte)((direction + 1) % 8), ++depth);

            return true;
        }

        public bool Step(byte direction)
        {
            Keys[] directions = {

                    Keys.Right,    // 1
                    Keys.Down,     // 3
                    Keys.Left,     // 5
                    Keys.Up,       // 7           
                    Keys.PageUp,   // 0 
                    Keys.PageDown, // 2 
                    Keys.End,      // 4 
                    Keys.Home,     // 6
                   
                };

            Keys key = directions[direction];
            int cost = 0;
            switch (World.Player.Direction)
            {

                case 0:
                    cost = 4;
                    break;
                case 1:
                    cost = 0;
                    break;
                case 2:
                    cost = 5;
                    break;
                case 3:
                    cost = 1;
                    break;
                case 4:
                    cost = 6;
                    break;
                case 5:
                    cost = 2;
                    break;
                case 6:
                    cost = 7;
                    break;
                case 7:
                    cost = 3;
                    break;
                default:
                    cost = 0;
                    break;
            }

            if (cost != Convert.ToInt32(direction))
            {
                Step(key);
            }
            Step(key);
            return true;
        }

        public bool Step(Keys key)
        {
            UO.Press(key);
            return true;

            /*
             * Slouží k zabezpečení proti problémům, u tohoto scriptu nevyžaduje
            bool result = false;
 
            using (ManualResetEvent handled = new ManualResetEvent(false))
            {
                using (ManualResetEvent requested = new ManualResetEvent(false))
                {
                    MessageCallback requestedCallback = RegisterMessageCallback(true, (d, p) =>
                    {
                        requested.Set();
                        handled.WaitOne(200);
                        return p;
                    }, 0x02);
                    try
                    {
                        UO.Press(key);
 
                        if (!requested.WaitOne(500))
                            return false;
                    }
                    finally
                    {
                        handled.Set();
                        Core.UnregisterClientMessageCallback(0x02, requestedCallback);
                    }
                }
 
                handled.Reset();
 
                using (ManualResetEvent responded = new ManualResetEvent(false))
                {
                    MessageCallback respondedCallback = RegisterMessageCallback(false, (d, p) =>
                    {
                        result = d[0] == 0x22;
                        responded.Set();
                        handled.WaitOne(500);
                        return p;
                    }, 0x21, 0x22);
                    try
                    {
                        if (!responded.WaitOne(40000))
                        {
                            UO.PrintWarning("Walk response timeout");
                            return false;
                        }
                    }
                    finally
                    {
                        handled.Set();
                        Core.UnregisterServerMessageCallback(0x21, respondedCallback);
                        Core.UnregisterServerMessageCallback(0x22, respondedCallback);
                    }
                }
 
            }
 
            UO.Wait(400);
            return result;
            */
        }

        #endregion
    }
} 
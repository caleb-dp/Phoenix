using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;

namespace CalExtension.UOExtensions
{
  [RuntimeObject]
  public class CharactersHits
  {

    [ServerMessageHandler(0xa1)]
    public CallbackResult HitsChanged(byte[] data, CallbackResult prevResult)
    {
      UOCharacter character = new UOCharacter(ByteConverter.BigEndian.ToUInt32(data, 1));

      decimal maxHits = (ushort)character.MaxHits;
      ushort hits = ByteConverter.BigEndian.ToUInt16(data, 7);

      if (character.Hits != hits)
      {
        ushort color = 0x059;
        string name = "";
        decimal perc = 0;
        bool print = false;
        bool onlyDamage = false;
        short dmg = (short)(hits - character.Hits);
        bool alie = false;

        decimal percHit = (decimal)((decimal)hits / (decimal)character.MaxHits) * 100m;// / (decimal)character.MaxHits) * 100.0m) - 100;
        decimal damagePerc = ((decimal)(Math.Abs(dmg)) / (decimal)character.MaxHits) * 100.0m;

        if (maxHits > 0)
        {
          perc = ((hits / maxHits) * 100.0m);

          if (character.Serial == World.Player.Serial)
          {
            if (Math.Abs(character.Hits - hits) < 3)
              print = false;
            else
            {
              name = hits + "";
              print = true;
            }
          }
          else if (Game.MergeLists<UOCharacter>(Game.CurrentGame.Alies, Game.CurrentGame.HealAlies).Where(item => item.Serial == character.Serial).Count() > 0)
          {
            alie = print = true;
          }
          else if (character.Renamable)//Summoni
          {
            print = true;
            color = 0x0159;
            print = onlyDamage = true;
          }
          else if (character.Notoriety == Notoriety.Murderer || character.Notoriety == Notoriety.Enemy)
          {
            name = character.Name;
            color = 0x0021;
            print = onlyDamage = true;
          }
          else if (character.Notoriety == Notoriety.Guild)
          {
            name = character.Name;
            color = 0x003F;
            print = onlyDamage = true;
          }
          else if (character.Notoriety == Notoriety.Criminal || character.Notoriety == Notoriety.Unknown || character.Notoriety == Notoriety.Neutral)
          {
            name = character.Name;
            color = 0x03B7;
            print = onlyDamage = true;
          }
        }

        if (print)
        {
          if (alie)
          {
            if (CalebConfig.FriendHitsMessageType != MessagePrintType.None)
            {
              if (dmg < 0)
                color = 0x002a;
              else
                color = 0x003F;


              if (Math.Abs(dmg) != 1)
              {
                if (CalebConfig.FriendHitsMessageType == MessagePrintType.Default)
                  character.PrintMessage((dmg < 0 ? "" : "+") + (dmg), color);
                else
                  Game.PrintMessage(character.Name + ": " + (dmg < 0 ? "" : "+") + (dmg), color);
              }
            }
          }
          else
          {
            if (onlyDamage)
            {
              if (dmg < 0 && CalebConfig.EnemyHitsMessageType != MessagePrintType.None)
              {
                if (CalebConfig.EnemyHitsMessageType == MessagePrintType.Default)
                  character.PrintMessage("" + (dmg), color);
                else
                  Game.PrintMessage(character.Name + ": " + (dmg), color);
              }
            }
            else//toto je hrac??
            {
              if (CalebConfig.PlayerHitsMessageType != MessagePrintType.None)
              {
                string sufix = "";
                if (dmg < 0 && (damagePerc > 30 || percHit <= 50))
                {
                  sufix = " Critical!";
                  color = 0x0149;
                }

                if (CalebConfig.PlayerHitsMessageType == MessagePrintType.Default)
                  character.PrintMessage(name + " [" + (dmg < 0 ? "" : "+") + dmg + "]" + sufix, color);
                else
                  Game.PrintMessage("Player: " + name + " [" + (dmg < 0 ? "" : "+") + dmg + "]" + sufix, color);
              }

              if (dmg < 0)
                this.OnDamage(character);
            }
          }
        }
      }
      return 0;
    }

    protected void OnDamage(UOCharacter ch)
    {
      //TODO to co mela amontka textura
    }
  }
}

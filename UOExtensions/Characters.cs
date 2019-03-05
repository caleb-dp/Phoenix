using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using Caleb.Library.CAL.Business;

namespace CalExtension.UOExtensions
{
  public class Characters
  {
    //---------------------------------------------------------------------------------------------

    public static int EnemyDistance = 20;

    public static readonly Notoriety[] EnemyFilter = new Notoriety[] {
            Notoriety.Murderer,
            Notoriety.Enemy,
            //Notoriety.Criminal,
            //Notoriety.Neutral
        };

    //---------------------------------------------------------------------------------------------

    public static List<UOCharacter> CharactersByDistance
    {
      get
      {
        List<UOCharacter> characters = new List<UOCharacter>();
        characters.AddRange(World.Characters);
        characters.Sort(delegate(UOCharacter char1, UOCharacter char2)
        {
          return char1.Distance.CompareTo(char2.Distance);
        });

        return characters;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsSummon(UOCharacter ch)
    {
      bool result = false;

      if (ch.Renamable || Game.IsMob(ch.Serial) || Rename.IsMobRenamed(ch.Serial))
        result = true;

      if (!result)
      {
        foreach (IUOItemType itemType in ItemLibrary.PlayerSummons)
        {
          if (itemType.Graphic == ch.Model && itemType.Color == ch.Color)
          {

            if (ItemLibrary.IsMonsterConflictSummon(ch))
            {
              UOItemType conflictType = ItemLibrary.GetMonsterConflictSummon(ch);
              string safeName = ch.Name + String.Empty;
              string defaultName = (conflictType.Name + String.Empty).Replace(" ", "").ToLower();
              string compareName = (ch.Name + String.Empty).Replace(" ", "").ToLower();


              if (!String.IsNullOrEmpty(compareName))
              {
                if (conflictType.Names.Count > 0)
                {
                  foreach (string nameAlter in conflictType.Names)
                  {
                    defaultName = (nameAlter + String.Empty).Replace(" ", "").ToLower();
                    if (compareName.Contains(defaultName))
                      return false;
                  }
                }
                else 
                  result = !compareName.Contains(defaultName);
              }
            }
            else
            {
              result = true;
            }

            break;
          }
        }
      }

      return result;
    }

    //---------------------------------------------------------------------------------------------

    public static List<UOCharacter> GetEnemiesByDistance()
    {
      return GetEnemiesByDistance(EnemyDistance, false, null);
    }

    //---------------------------------------------------------------------------------------------

    public static List<UOCharacter> GetEnemiesByDistance(int maxDistance, IUOPosition pos)
    {
      return GetEnemiesByDistance(maxDistance, false, pos);
    }

    //---------------------------------------------------------------------------------------------

    public static List<UOCharacter> GetEnemiesByDistance(int maxDistance, bool useDistanceCircles, IUOPosition pos)
    {
      List<UOCharacter> enemies = new List<UOCharacter>();

      if (pos == null)
        pos = UOPositionBase.PlayerPosition;

      int distance5 = 0;
      int distance10 = 0;
      int enemyDistance = maxDistance;

      List<UOCharacter> chars = CharactersByDistance;
      foreach (UOCharacter character in chars)
      {
        if (
  character.Serial != World.Player.Serial &&
            Robot.GetRelativeVectorLength(pos, UOPositionBase.CharacterPosition(character)) <= maxDistance &&
 // character.Distance < maxDistance &&
  Array.IndexOf(EnemyFilter, character.Notoriety) >= 0 &&
  !Game.CurrentGame.IsAlie(character.Serial) &&
  !Game.CurrentGame.IsHealAlie(character.Serial) &&
  !IsSummon(character)
            )
            
        {
          if (character.Distance <= 7)
            distance5++;
          else if (character.Distance <= 12)
            distance10++;
        }
      }

      if (useDistanceCircles)
      {
        if (distance5 > 0)
          enemyDistance = 8;
        else if (distance10 > 0)
          enemyDistance = 13;
      }

      foreach (UOCharacter character in chars)
      {
        if (
          character.Serial != World.Player.Serial &&
         // character.Distance < enemyDistance && 

            Robot.GetRelativeVectorLength(pos, UOPositionBase.CharacterPosition(character)) <= maxDistance &&
          Array.IndexOf(EnemyFilter, character.Notoriety) >= 0 &&
            !Game.CurrentGame.IsAlie(character.Serial) &&
          !Game.CurrentGame.IsHealAlie(character.Serial) &&
          !IsSummon(character))
          enemies.Add(character);
      }

      return enemies;
    }

    //---------------------------------------------------------------------------------------------

    public static bool ExistEnemy()
    {
      return GetEnemiesByDistance().Count > 0; 
    }

    //---------------------------------------------------------------------------------------------

    public static UOCharacter GetNearestEnemy()
    {
      List<UOCharacter> enemies = GetEnemiesByDistance();

      if (enemies.Count > 0)
        return enemies[0];

      return new UOCharacter(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------

    public static UOCharacter GetWeakestEnemy()
    {
      List<UOCharacter> enemies = new List<UOCharacter>();
      enemies.AddRange(GetEnemiesByDistance());
      enemies.Sort(delegate(UOCharacter char1, UOCharacter char2)
      {
        return char1.MaxHits.CompareTo(char2.MaxHits);
      });
      if (enemies.Count > 0) return enemies[enemies.Count - 1];

      return new UOCharacter(Serial.Invalid);
    }

    //---------------------------------------------------------------------------------------------
    public static List<EnemyInfo> GetEnemyList(List<UOCharacter> chars)
    {
      return GetEnemyList(chars, UOPositionBase.PlayerPosition);
    }

    public static List<EnemyInfo> GetEnemyList(List<UOCharacter> chars, IUOPosition center)
    {
      List<EnemyInfo> enemyList = new List<EnemyInfo>();

      foreach (UOCharacter ch in chars)//World.Characters)
      {
        if (Game.CurrentGame.IsAlie(ch.Serial) || Game.CurrentGame.IsHealAlie(ch.Serial))
          continue;

        EnemyInfo enemy = new EnemyInfo();
        enemy.Char = ch;
        enemy.Damage = (ch.MaxHits - ch.Hits);
        enemy.Perc = (((decimal)(ch.MaxHits - ch.Hits) / (decimal)ch.MaxHits) * 100.0m) / (decimal)ch.MaxHits;
        enemy.DamagePerc = ((decimal)(ch.MaxHits - ch.Hits) / (decimal)ch.MaxHits) * 100.0m;
        enemy.Distance = Robot.GetRelativeVectorLength(center, UOPositionBase.CharacterPosition(ch));

        if (enemy.Distance <= 5)
          enemy.Zone = 0;
        else if (enemy.Distance <= 10)
          enemy.Zone = 1;
        else if (enemy.Distance <= 15)
          enemy.Zone = 2;
        else
          enemy.Zone = 3;

        if (String.IsNullOrEmpty(ch.Name))
        {
          ch.Click();
          UO.Wait(100);
        }

        if ((String.Empty + ch.Name).ToLower().Contains("mirror") && ch.MaxHits >= 255)
          enemy.Priority = 100;
        else if ((String.Empty + ch.Name).ToLower().Contains("chameleon"))
          enemy.Priority = ch.MaxHits;
        else if ((String.Empty + ch.Name).ToLower().Contains("lethargy"))
          enemy.Priority = ch.MaxHits;
        else if ((String.Empty + ch.Name).ToLower().Contains("lethargy"))
          enemy.Priority = ch.MaxHits;
        else if ((String.Empty + ch.Name).ToLower().Contains("acid ooze"))
          enemy.Priority = ch.MaxHits;
        else if ((String.Empty + ch.Name).ToLower().Contains("bird"))
          enemy.Priority = ch.MaxHits - 100;

        if (enemy.Char.Notoriety == Notoriety.Enemy || enemy.Char.Notoriety == Notoriety.Murderer)
        {
          enemy.Priority += 1000;
        }

        enemyList.Add(enemy);
      }
      return enemyList;
    }

    //---------------------------------------------------------------------------------------------
  }

  //---------------------------------------------------------------------------------------------

  public class EnemyInfo
  {
    public UOCharacter Char;
    public decimal Perc;
    public int Zone = 0;
    public int Priority = 0;
    public int Damage = 0;
    public decimal DamagePerc = 0;
    public double Distance = 0;
  }
}

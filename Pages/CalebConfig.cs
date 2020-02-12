using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using Phoenix.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Caleb.Library;
using System.Xml;

namespace CalExtension
{
  [RuntimeObject]
  public class CalebConfig
  {
    private static bool autoHeal;
    private static bool healMoby;
    private static bool destroySpiderWeb;
    private static bool handleFrozen;
    private static RenameType rename;
    private static int healMinDamagePerc;
    private static int attackDelay;
    private static bool useWatcher;

    private static MessagePrintType friendHitsMessageType;
    private static MessagePrintType enemyHitsMessageType;
    private static MessagePrintType castMessageType;
    private static MessagePrintType playerHitsMessageType; 

    private static bool useWallTime;

    private static string lootItemTypes;
    private static string playerAliases;
    private static string globalCalebConfig;
    private static string bakGlobalCalebConfig;

    private static AutoHealType healType;
    private static LootType loot;

    private static bool trainPoison;

    private static string salatAlertHp;


    private static readonly DefaultPublicEvent propertyChanged = new DefaultPublicEvent();


    //private SynchronizedSettings globalsettings;
    /// <summary>
    /// Initializes a new instance of the <see cref="T:CorpsesAutoopen"/> class.
    /// </summary>
    public CalebConfig()
    {
      // Pozn: Snazim se mit staticky jen uplne minimum, protoze co je v instanci sezere pri rekompilaci GC
      // globalsettings = new SynchronizedSettings("Phoenix");

      // Core.Directory;

      autoHeal = Config.Profile.UserSettings.GetAttribute(true, "Value", "AutoHeal");
      healMoby = Config.Profile.UserSettings.GetAttribute(true, "Value", "HealMoby");
      destroySpiderWeb = Config.Profile.UserSettings.GetAttribute(true, "Value", "DestroySpiderWeb");
      handleFrozen = Config.Profile.UserSettings.GetAttribute(true, "Value", "HandleFrozen");
      rename = (RenameType)Config.Profile.UserSettings.GetAttribute((int)RenameType.OnAppeared, "Value", "Rename");
      healMinDamagePerc = Config.Profile.UserSettings.GetAttribute(0, "Value", "HealMinDamagePerc");
      attackDelay = Config.Profile.UserSettings.GetAttribute(1500, "Value", "AttackDelay");
      useWatcher = Config.Profile.UserSettings.GetAttribute(false, "Value", "UseWatcher");

      healType = (AutoHealType)Config.Profile.UserSettings.GetAttribute((int)AutoHealType.Automatic, "Value", "HealType");
      loot = (LootType)Config.Profile.UserSettings.GetAttribute((int)LootType.OpenCorpse, "Value", "Loot");

      trainPoison = Config.Profile.UserSettings.GetAttribute(true, "Value", "TrainPoison");

      friendHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "FriendHitsMessageType");
      enemyHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "EnemyHitsMessageType");
      castMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "CastMessageType");
      playerHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "PlayerHitsMessageType");

      useWallTime = Config.Profile.UserSettings.GetAttribute(true, "Value", "UseWallTime");

      lootItemTypes = Config.Profile.UserSettings.GetAttribute(CalExtension.UOExtensions.Loot.LootDefaultItemTypes, "Value", "LootItemTypes");
      playerAliases = GetGlobalFile("PlayerAliases.txt", "");// Config.Profile.UserSettings.GetAttribute(String.Empty, "Value", "PlayerAliases");

      salatAlertHp = Config.Profile.UserSettings.GetAttribute("33%", "Value", "SalatAlertHp");

      bakGlobalCalebConfig = globalCalebConfig = GetGlobalFile("globalCalebConfig.xml", "<Config></Config>");// Config.Profile.UserSettings.GetAttribute(String.Empty, "Value", "PlayerAliases");

      Config.Profile.UserSettings.Loaded += new EventHandler(UserSettings_Loaded);
      Config.Profile.UserSettings.Saving += new EventHandler(UserSettings_Saving);
      globalDocument = null;
      //globalsettings.Loaded += Globalsettings_Loaded;
      //globalsettings.Saving += Globalsettings_Saving;
    }


    #region Configuration

    void UserSettings_Loaded(object sender, EventArgs e)
    {
      autoHeal = Config.Profile.UserSettings.GetAttribute(true, "Value", "AutoHeal");
      healMoby = Config.Profile.UserSettings.GetAttribute(true, "Value", "HealMoby");
      destroySpiderWeb = Config.Profile.UserSettings.GetAttribute(true, "Value", "DestroySpiderWeb");
      handleFrozen = Config.Profile.UserSettings.GetAttribute(true, "Value", "HandleFrozen");
      rename = (RenameType)Config.Profile.UserSettings.GetAttribute((int)RenameType.OnAppeared, "Value", "Rename");

      healMinDamagePerc = Config.Profile.UserSettings.GetAttribute(0, "Value", "HealMinDamagePerc");
      attackDelay = Config.Profile.UserSettings.GetAttribute(1500, "Value", "AttackDelay");
      useWatcher = Config.Profile.UserSettings.GetAttribute(false, "Value", "UseWatcher");

      healType = (AutoHealType)Config.Profile.UserSettings.GetAttribute((int)AutoHealType.Automatic, "Value", "HealType");
      loot = (LootType)Config.Profile.UserSettings.GetAttribute((int)LootType.OpenCorpse, "Value", "Loot");

      trainPoison = Config.Profile.UserSettings.GetAttribute(true, "Value", "TrainPoison");

      friendHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "FriendHitsMessageType");
      enemyHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "EnemyHitsMessageType");
      castMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "CastMessageType");
      playerHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "PlayerHitsMessageType");

      useWallTime = Config.Profile.UserSettings.GetAttribute(true, "Value", "UseWallTime");


      salatAlertHp = Config.Profile.UserSettings.GetAttribute("33%", "Value", "SalatAlertHp");

      lootItemTypes = Config.Profile.UserSettings.GetAttribute(CalExtension.UOExtensions.Loot.LootDefaultItemTypes, "Value", "LootItemTypes");
      // playerAliases = Config.Profile.UserSettings.GetAttribute(String.Empty, "Value", "PlayerAliases");
      playerAliases = GetGlobalFile("PlayerAliases.txt", "");
      bakGlobalCalebConfig = globalCalebConfig = GetGlobalFile("globalCalebConfig.xml", "<Config></Config>");
      globalDocument = null;
      OnPropertyChanged(EventArgs.Empty);
    }

    void UserSettings_Saving(object sender, EventArgs e)
    {
      Config.Profile.UserSettings.SetAttribute(autoHeal, "Value", "AutoHeal");
      Config.Profile.UserSettings.SetAttribute(healMoby, "Value", "HealMoby");
      Config.Profile.UserSettings.SetAttribute(destroySpiderWeb, "Value", "DestroySpiderWeb");
      Config.Profile.UserSettings.SetAttribute(handleFrozen, "Value", "HandleFrozen");
      Config.Profile.UserSettings.SetAttribute((int)rename, "Value", "Rename");

      Config.Profile.UserSettings.SetAttribute((int)healMinDamagePerc, "Value", "HealMinDamagePerc");
      Config.Profile.UserSettings.SetAttribute((int)attackDelay, "Value", "AttackDelay");
      Config.Profile.UserSettings.SetAttribute(useWatcher, "Value", "UseWatcher");


      Config.Profile.UserSettings.SetAttribute((int)healType, "Value", "HealType");
      Config.Profile.UserSettings.SetAttribute((int)loot, "Value", "Loot");

      Config.Profile.UserSettings.SetAttribute(trainPoison, "Value", "TrainPoison");

      Config.Profile.UserSettings.SetAttribute((int)friendHitsMessageType, "Value", "FriendHitsMessageType");
      Config.Profile.UserSettings.SetAttribute((int)enemyHitsMessageType, "Value", "EnemyHitsMessageType");
      Config.Profile.UserSettings.SetAttribute((int)castMessageType, "Value", "CastMessageType");
      Config.Profile.UserSettings.SetAttribute((int)playerHitsMessageType, "Value", "PlayerHitsMessageType");

      Config.Profile.UserSettings.SetAttribute(useWallTime, "Value", "UseWallTime");

      Config.Profile.UserSettings.SetAttribute(lootItemTypes, "Value", "LootItemTypes");
      //Config.Profile.UserSettings.SetAttribute(playerAliases, "Value", "PlayerAliases");

      Config.Profile.UserSettings.SetAttribute(salatAlertHp, "Value", "SalatAlertHp");

      SaveGlobalFile("PlayerAliases.txt", playerAliases);
      try
      {
        SaveGlobalFile("globalCalebConfig.xml", CalExtension.UOExtensions.Utils.FormatXml(GlobalDocument.DocumentElement));
      }
      catch
      {
        SaveGlobalFile("globalCalebConfig.xml", globalCalebConfig);
      }
      globalDocument = null;
      Cal.Engine.SaveDataBase();
    }

    public static bool AutoHeal
    {
      get { return autoHeal; }
      set
      {
        if (value != autoHeal)
        {
          autoHeal = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool HealMoby
    {
      get { return healMoby; }
      set
      {
        if (value != healMoby)
        {
          healMoby = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool DestroySpiderWeb
    {
      get { return destroySpiderWeb; }
      set
      {
        if (value != destroySpiderWeb)
        {
          destroySpiderWeb = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }


    public static bool HandleFrozen
    {
      get { return handleFrozen; }
      set
      {
        if (value != handleFrozen)
        {
          handleFrozen = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }


    public static RenameType Rename
    {
      get { return rename; }
      set
      {
        if (value != rename)
        {
          rename = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static LootType Loot
    {
      get { return loot; }
      set
      {
        if (value != loot)
        {
          loot = value;
          OnPropertyChanged(EventArgs.Empty);
          if (LootChanged != null)
            LootChanged(value, EventArgs.Empty);
        }
      }
    }

    public static AutoHealType HealType
    {
      get { return healType; }
      set
      {
        if (value != healType)
        {
          healType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static int HealMinDamagePerc
    {
      get { return healMinDamagePerc; }
      set
      {
        if (value != healMinDamagePerc)
        {
          healMinDamagePerc = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static int AttackDelay
    {
      get { return attackDelay; }
      set
      {
        if (value != attackDelay)
        {
          attackDelay = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool UseWatcher
    {
      get { return useWatcher; }
      set
      {
        if (value != useWatcher)
        {
          useWatcher = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool TrainPoison
    {
      get { return trainPoison; }
      set
      {
        if (value != trainPoison)
        {
          trainPoison = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType CastMessageType
    {
      get { return castMessageType; }
      set
      {
        if (value != castMessageType)
        {
          castMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType FriendHitsMessageType
    {
      get { return friendHitsMessageType; }
      set
      {
        if (value != friendHitsMessageType)
        {
          friendHitsMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType EnemyHitsMessageType
    {
      get { return enemyHitsMessageType; }
      set
      {
        if (value != enemyHitsMessageType)
        {
          enemyHitsMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType PlayerHitsMessageType
    {
      get { return playerHitsMessageType; }
      set
      {
        if (value != playerHitsMessageType)
        {
          playerHitsMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool UseWallTime
    {
      get { return useWallTime; }
      set
      {
        if (value != useWallTime)
        {
          useWallTime = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static string LootItemTypes
    {
      get { return lootItemTypes; }
      set
      {
        if (value != lootItemTypes)
        {
          lootItemTypes = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static string SalatAlertHp
    {
      get { return salatAlertHp; }
      set
      {
        if (value != salatAlertHp)
        {
          salatAlertHp = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static int  SalatAlertHpValue
    {
      get
      {
        string val = SalatAlertHp;
        int outVal = 0;

        if (!Int32.TryParse((val + String.Empty).Replace("%", "").Trim(), out outVal))
          outVal = 0;

        return outVal;
      }
    }

    public static string SalatAlertHpKind
    {
      get
      {
        string val = SalatAlertHp;
        if (val.Contains("%"))
          return "perc";
        else
          return "abs";
      }
    }

    public static string PlayerAliases
    {
      get { return playerAliases; }
      set
      {
        if (value != playerAliases)
        {
          playerAliases = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static string GlobalCalebConfig
    {
      get { return globalCalebConfig; }
      set
      {
        if (value != globalCalebConfig)
        {
          globalDocument = null;
          globalCalebConfig = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    private static XmlDocument globalDocument;
    public static XmlDocument GlobalDocument
    {
      get
      {
        if (globalDocument == null)
        {
          XmlDocument doc = new XmlDocument();
          try
          {
            doc.LoadXml(GlobalCalebConfig);
          }
          catch (Exception ex)
          {
            if (Game.Debug)
              Notepad.WriteLine("GlobalDocument 1" + ex.Message);

            try
            {
              doc.LoadXml(bakGlobalCalebConfig);
            }
            catch (Exception exx)
            {
              if (Game.Debug)
                Notepad.WriteLine("GlobalDocument 2" + exx.Message );
              doc.LoadXml("<Config></Config>");
            }
          }
          globalDocument = doc;
        }
        return globalDocument;
      }
    }

    public static event EventHandler LootChanged;

    public static event EventHandler PropertyChanged
    {
      add { propertyChanged.AddHandler(value); }
      remove { propertyChanged.RemoveHandler(value); }
    }

    protected static void OnPropertyChanged(EventArgs e)
    {
      propertyChanged.InvokeAsync(null, e);
    }



    #endregion

    public static IEnumerable<KeyValuePair<int, string>> Of<T>()
    {
      return Enum.GetValues(typeof(T))
          .Cast<T>()
          .Select(p => new KeyValuePair<int, string>(Convert.ToInt32(p), p.ToString()))
          .ToList();
    }

    //---------------------------------------------------------------------------------------------

    public static string GetPlayerAlias(UOCharacter ch)
    {
      Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
      string result = null;

      string p = PlayerAliases + String.Empty;
      string chName = ch.Name;
      if (String.IsNullOrEmpty(ch.Name))
      {
        ch.Click();
        Game.Wait(100);
      }

      chName = ch.Name;

      if (!String.IsNullOrEmpty(chName))
      {
        string[] lines = p.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
          string[] lineSplit = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
          if (lineSplit.Length > 1)
          {
            string playerName = lineSplit[0];
            string chars = lineSplit[1];

            foreach (string c in chars.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
              if (chName.ToLower() == c.ToLower().Trim())
                return playerName;
            }
          }
        }
      }
      return result;
    }

    //---------------------------------------------------------------------------------------------

    protected static string GetGlobalFile(string fileName, string defaultValue)
    {
      string result = defaultValue;
      try
      {
        string filePath = Path.Combine(Core.Directory, fileName);
        if (File.Exists(filePath))
        {
          result = Encoding.UTF8.GetString(File.ReadAllBytes(Path.Combine(Core.Directory, fileName)));

          string bkpPath = Path.Combine(Core.Directory, fileName + "_" + String.Format("{0:yyyyMMdd}", DateTime.Now) + ".backup");

          if (!File.Exists(bkpPath))
            File.WriteAllBytes(bkpPath, File.ReadAllBytes(filePath));
        }
      }
      catch 
      {
        result = defaultValue;
      }
      return result;
    }

    //---------------------------------------------------------------------------------------------

    protected static void SaveGlobalFile(string fileName, string data)
    {
      try
      {
        string filePath = Path.Combine(Core.Directory, fileName);
        string bkpPath = Path.Combine(Core.Directory, fileName + ".backup");
        if (File.Exists(filePath))
        {
          if (File.Exists(bkpPath))
          {
            File.Delete(bkpPath);
            File.WriteAllBytes(bkpPath, File.ReadAllBytes(filePath));
          }

          File.Delete(filePath);
        }

        File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(data));
      }
      catch { }
    }

    //---------------------------------------------------------------------------------------------
    public static string GetStringOrDefault(string code)
    {
      return GetStringOrDefault(code, null);
    }

    public static string GetStringOrDefault(string code, string defaultValue)
    {
      return GetGlobalConfigValueOrDefault("GlobalVariables/" + code, "Value", defaultValue);
    }

    //---------------------------------------------------------------------------------------------

    public static UOColor GetUOColorOrDefault(string code)
    {
      return GetUOColorOrDefault(code, UOColor.Invariant);
    }

    public static UOColor GetUOColorOrDefault(string code, UOColor defaultValue)
    {
      return GetGlobalConfigValueOrDefault("GlobalVariables/" + code, "Value", defaultValue);
    }

    //---------------------------------------------------------------------------------------------

    public static Graphic GetGraphicOrDefault(string code)
    {
      return GetGraphicOrDefault(code, Graphic.Invariant);
    }

    public static Graphic GetGraphicOrDefault(string code, Graphic defaultValue)
    {
      return GetGlobalConfigValueOrDefault("GlobalVariables/" + code, "Value", defaultValue);
    }

    //---------------------------------------------------------------------------------------------

    public static Serial GetSerialOrDefault(string code)
    {
      return GetSerialOrDefault(code, Serial.Invalid);
    }

    public static Serial GetSerialOrDefault(string code, Serial defaultValue)
    {
      return GetGlobalConfigValueOrDefault("GlobalVariables/" + code, "Value", defaultValue);
    }

    //---------------------------------------------------------------------------------------------

    public static Int32 GetInt32OrDefault(string code)
    {
      return GetInt32OrDefault(code, 0);
    }

    public static Int32 GetInt32OrDefault(string code, Int32 defaultValue)
    {
      return GetGlobalConfigValueOrDefault("GlobalVariables/" + code, "Value", defaultValue);
    }


    //---------------------------------------------------------------------------------------------

    public static bool GetBooleanOrDefault(string code)
    {
      return GetBooleanOrDefault(code, true);
    }

    public static bool GetBooleanOrDefault(string code, bool defaultValue)
    {
      return GetGlobalConfigValueOrDefault("GlobalVariables/" + code, "Value", defaultValue);
    }


    //---------------------------------------------------------------------------------------------

    public static string GetGlobalConfigValue(string xpath, string attrName)
    {
      return GetGlobalConfigValueOrDefault(xpath, attrName, null);
    }


    //---------------------------------------------------------------------------------------------

    public static string GetGlobalConfigValueOrDefault(string xpath, string attrName, string defaultValue)
    {
      XmlDocument doc = CalebConfig.GlobalDocument;

      if (doc != null)
      {
        XmlElement itm = doc.DocumentElement.SelectSingleNode(xpath) as XmlElement;
        if (itm != null && itm.HasAttribute(attrName))
        {
          try
          {
            string strval = itm.GetAttribute(attrName);
            return strval;
          }
          catch (Exception ex)
          {
            if (Game.Debug)
              Notepad.WriteLine("GetGlobalConfigValueOrDefault ex: " + ex.Message);
          }
        }
        else
        {
          if (Game.Debug)
            Notepad.WriteLine("GetGlobalConfigValueOrDefault !itm: " + xpath + " HasAttribute: " + attrName);
        }
      }
      return defaultValue;
    }

    //---------------------------------------------------------------------------------------------

    public static UOColor GetGlobalConfigValueOrDefault(string xpath, string attrName, UOColor defaultValue)
    {
      string val = GetGlobalConfigValue(xpath, attrName);
      
      if (val != null)
      {
        try
        {
          UOColor c = UOColor.Parse(val);
          return c;
        }
        catch {  }
      }
      return defaultValue;
    }

    //---------------------------------------------------------------------------------------------

    public static Graphic GetGlobalConfigValueOrDefault(string xpath, string attrName, Graphic defaultValue)
    {
      string val = GetGlobalConfigValue(xpath, attrName);

      if (val != null)
      {
        try
        {
          Graphic c = Graphic.Parse(val);
          return c;
        }
        catch { }
      }
      return defaultValue;
    }

    //---------------------------------------------------------------------------------------------

    public static Serial GetGlobalConfigValueOrDefault(string xpath, string attrName, Serial defaultValue)
    {
      string val = GetGlobalConfigValue(xpath, attrName);

      if (val != null)
      {
        try
        {
          Serial c = Serial.Parse(val);
          return c;
        }
        catch { }
      }
      return defaultValue;
    }

    //---------------------------------------------------------------------------------------------

    public static Int32 GetGlobalConfigValueOrDefault(string xpath, string attrName, Int32 defaultValue)
    {
      string val = GetGlobalConfigValue(xpath, attrName);

      if (val != null)
      {
        try
        {
          Int32 c = Int32.Parse(val);
          return c;
        }
        catch { }
      }
      return defaultValue;
    }

    //---------------------------------------------------------------------------------------------

    public static bool GetGlobalConfigValueOrDefault(string xpath, string attrName, bool defaultValue)
    {
      string val = GetGlobalConfigValue(xpath, attrName);

      if (val != null)
      {
        try
        {
          bool c = bool.Parse(val);
          return c;
        }
        catch { }
      }
      return defaultValue;
    }

    //---------------------------------------------------------------------------------------------

  }
}






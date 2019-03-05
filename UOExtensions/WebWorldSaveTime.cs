using Phoenix;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace CalExtension.UOExtensions
{
  public class WebWorldSaveTime
  {
    //---------------------------------------------------------------------------------------------

    public static bool IsValid(DateTime wsTime)
    {
      return Math.Abs( (DateTime.Now - wsTime).TotalHours) < 24;
    }

    //---------------------------------------------------------------------------------------------

    public static DateTime GetTimeFromUrl()
    {
      return GetTimeFromUrl(null);
    }

    //---------------------------------------------------------------------------------------------

    public static DateTime GetTimeFromUrl(string url)
    {
      ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
      ServicePointManager.Expect100Continue = true;
      ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;


      if (String.IsNullOrEmpty(url))
        url = "https://www.darkparadise.eu/api.php?lastsave";

      string data = String.Empty;

      using (WebClient client = new WebClient()) 
      {
        try
        {
          
          data = client.DownloadString(url);
          UO.Wait(300);
        }
        catch (Exception ex)
        {
           System.Diagnostics.Debug.WriteLine("Ex" + ex.Message);
          System.Diagnostics.Debug.WriteLine("" + ex.Message);
          System.Diagnostics.Debug.WriteLine("" + ex.StackTrace);
          Notepad.Write(ex.StackTrace);

        }
      }
   
      var matches = Regex.Matches(data, "\"(?<name>[a-z]+)\": \"(?<value>\\d+)\"");

      int hour = 0;
      int minute = 0;
      int day = 1;
      int month = 1;
      int year = 1;

      string shour = "";
      string sminute = "";
      string sday = "";
      string smonth = "";
      string syear = "";

      foreach (Match m in matches)
      {
        string name = m.Groups["name"].Value;
        string value = m.Groups["value"].Value;

        switch (name)
        {
          case "hour":
            shour = m.Groups["value"].Value;
            hour = Int32.Parse(m.Groups["value"].Value);
            break;
          case "minute":
            sminute = m.Groups["value"].Value;
            minute = Int32.Parse(m.Groups["value"].Value);
            break;
          case "day":
            sday = m.Groups["value"].Value;
            day = Int32.Parse(m.Groups["value"].Value);
            break;
          case "month":
            smonth = m.Groups["value"].Value;
            month = Int32.Parse(m.Groups["value"].Value);
            break;
          case "year":
            syear = m.Groups["value"].Value;
            year = Int32.Parse(m.Groups["value"].Value);
            break;
        }
      }

      System.Diagnostics.Debug.WriteLine(String.Format("GetTimeFromUrl: " + url + " {0:g}", new DateTime(year, month, day, hour, minute, 0)));
      System.Diagnostics.Debug.WriteLine(String.Format("GetTimeFromUrl RAW data: " + url + " {0}", data));
      System.Diagnostics.Debug.WriteLine(String.Format("GetTimeFromUrl RAW: " + url + " {0}, {1}, {2}, {3}, {4}", shour, sminute, sday, smonth, syear));

      return new DateTime(year, month, day, hour, minute, 0);
    }

    //---------------------------------------------------------------------------------------------

    [Command("plwsifw")]
    public static void PrintLastWorldSaveInfoFromWeb()
    {
      PrintLastWorldSaveInfoFromWeb(0);
    }

    //---------------------------------------------------------------------------------------------

    [Command("plwsifw")]
    public static void PrintLastWorldSaveInfoFromWeb(int timeCorrection)
    {
      WebWorldSaveInfo info = GetInfo(GetTimeFromUrl(), timeCorrection);

      UO.Print(0x0053, "WS pred: {0}", info.LastTimeStr);
      UO.Print(0x0053, "WS   za: {0}", info.NextTimeStr);
    }

    //---------------------------------------------------------------------------------------------

    public static WebWorldSaveInfo GetInfo(DateTime lastTime, int timeCorrection)
    {
      WebWorldSaveInfo info = new WebWorldSaveInfo();

      info.LastTime = lastTime;
      info.NextTime = info.LastTime.AddMinutes(WsPeriodMinutes - timeCorrection);
      info.LastTimeSpan = DateTime.Now - info.LastTime;
      info.NextTimeSpan = DateTime.Now - info.NextTime;

      try
      {
        string tsStr = info.LastTimeSpan.ToString();
        info.LastTimeStr = tsStr.Remove(tsStr.IndexOf('.'), tsStr.Length - tsStr.IndexOf('.'));

        string ts2Str = info.NextTimeSpan.ToString();
        info.NextTimeStr = ts2Str.Remove(ts2Str.IndexOf('.'), ts2Str.Length - ts2Str.IndexOf('.'));
      }
      catch
      {
        info.LastTimeStr = "Error";
        info.NextTimeStr = "Error";
      }

      return info;
    }

    //---------------------------------------------------------------------------------------------

    public static int WsPeriodMinutes = 132;//?? pravdepodobne 
    //Poslední save: 11:23 10.04.2018 ??
    //Poslední save: 13:35 10.04.2018
    //Poslední save: 15:46 10.04.2018

  }

  public class WebWorldSaveInfo
  {
    public string LastTimeStr;
    public string NextTimeStr;
    public TimeSpan LastTimeSpan;
    public TimeSpan NextTimeSpan;
    public DateTime LastTime;
    public DateTime NextTime;

  }

}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class TimeUtils
{
    private static DateTime _lastFetchedTime = DateTime.MinValue;
    private static DateTime _systemTimeWhenFetched;

    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);
    private static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime Now => DateTime.Now;

    public static void Initialize(MonoBehaviour coroutineStarter, Action successCallback, Action errorCallback)
    {
        coroutineStarter.StartCoroutine(FetchTimeFromWorldTimeAPI(successCallback, errorCallback));
    }

    private static IEnumerator FetchTimeFromWorldTimeAPI(Action successCallback, Action errorCallback)
    {
        using var www = UnityWebRequest.Get("http://worldtimeapi.org/api/timezone/UTC");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var jsonResponse = JsonUtility.FromJson<WorldTimeResponse>(www.downloadHandler.text);
            if (DateTime.TryParse(jsonResponse.utc_datetime, out DateTime serverTime))
            {
                _lastFetchedTime = serverTime;
                _systemTimeWhenFetched = DateTime.Now;

                if (Mathf.Abs((float)(DateTimeToSeconds(_lastFetchedTime) - DateTimeToSeconds(_systemTimeWhenFetched))) < 3600.0f)
                {
                    successCallback?.Invoke();
                }
                else
                {
                    errorCallback?.Invoke();
                }
            }
            else
            {
                errorCallback?.Invoke();
                Debug.LogError("Error parsing time from WorldTimeAPI response");
            }
        }
        else
        {
            errorCallback?.Invoke();
            Debug.LogError("Error fetching time from WorldTimeAPI: " + www.error);
        }
    }

    [Serializable]
    private class WorldTimeResponse
    {
        public string utc_datetime;
    }

    // public static long CurrentTicks => DateTime.Now.Ticks - Epoch.Ticks;
    public static long CurrentTicks => Now.Ticks - Epoch.Ticks;
    public static long CurrentTicksUtc => DateTime.UtcNow.Ticks - EpochUtc.Ticks;
    public static long CurrentDays => CurrentTicks / TimeSpan.TicksPerDay;
    // public static double CurrentSeconds => TicksToSeconds(CurrentTicks);
    // public static double MaxSeconds => DateTime.MaxValue.TotalSeconds();
    public static double CurrentSecondsUtc => TicksToSeconds(CurrentTicksUtc);
    // public static int CurrentDayOfWeek => ((int)DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
    public static int CurrentDayOfWeek => ((int)Now.DayOfWeek == 0) ? 7 : (int)Now.DayOfWeek;

    public static DateTime TicksToDateTime(long ticks)
    {
        var date = Epoch + TimeSpan.FromTicks(ticks);
        return date.ToLocalTime();
    }

    public static double DateTimeToSeconds(DateTime dateTime)
    {
        return TicksToSeconds(dateTime.Ticks - Epoch.Ticks);
    }

    public static DateTime SecondsToDateTime(double seconds)
    {
        var date = Epoch + TimeSpan.FromSeconds(seconds);
        return date;
    }

    public static double TimespanSeconds(long ticks, long lastTicks)
    {
        return TimeSpan.FromTicks(ticks - lastTicks).TotalSeconds;
    }

    public static double TimespanHours(long ticks, long lastTicks)
    {
        return TimeSpan.FromTicks(ticks - lastTicks).TotalHours;
    }

    public static long SecondsToTicks(double seconds)
    {
        return (long)(seconds * TimeSpan.TicksPerSecond);
    }

    public static double TicksToSeconds(long ticks)
    {
        return (double)ticks / TimeSpan.TicksPerSecond;
    }

    public static long SecondsToDays(double seconds)
    {
        return SecondsToTicks(seconds) / TimeSpan.TicksPerDay;
    }

    public static double DaysToSeconds(long days)
    {
        return TimeSpan.FromDays(days).TotalSeconds;
    }

    public static string FormatTimeSpan(double seconds)
    {
        var span = new TimeSpan(SecondsToTicks(seconds));
        return span.Days > 0 ? $"{span.Days}d:{span.Hours:00}h:{span.Minutes:00}m" :
            span.Hours > 0 ? $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}" :
            $"{span.Minutes:00}:{span.Seconds:00}";
    }

    public static string FormatTimeSpan4(double seconds)
    {
        var span = new TimeSpan(SecondsToTicks(seconds));
        return span.Days > 0
            ? $"{span.Days:00}d {span.Hours:00}h"
            : $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
    }

    public static string FormatTimeSpanExcludeSecond(double seconds)
    {
        var span = new TimeSpan(SecondsToTicks(seconds));
        return span.Hours > 0 ? $"{span.Hours:00}h:{span.Minutes:00}min" : $"{span.Minutes:00}min";
    }

    public static float TargetTimeScale { get; set; } = 1;
}
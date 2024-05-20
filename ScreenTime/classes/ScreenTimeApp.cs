using Newtonsoft.Json;
using ScreenTime.utils;

namespace ScreenTime.classes
{
    enum SortMode
    {
        SECONDS_IN_FOCUS,
        SECONDS_IN_BACKGROUND,
        TIMES_FOCUSED,
        TIMES_OPENED
    }

    internal class ScreenTimeApp
    {
        // Path, ScreenTimeApp
        public static Dictionary<string, ScreenTimeApp> screenTimeApps = [];

        public string Name { get; private set; }
        public string Path { get; private set; }
        // Date (eg 20.05.2024), Seconds
        public Dictionary<string, uint> SecondsInFocus { get; private set; }
        public Dictionary<string, uint> SecondsInBackground { get; private set; }
        public Dictionary<string, uint> TimesFocused { get; private set; }
        public Dictionary<string, uint> TimesOpened { get; private set; }

        [JsonConstructor]
        private ScreenTimeApp(string name, string path, Dictionary<string, uint> secondsInFocus, Dictionary<string, uint> secondsInBackground, Dictionary<string, uint> timesFocused, Dictionary<string, uint> timesOpened)
        {
            Name = name;
            Path = path;
            SecondsInFocus = secondsInFocus;
            SecondsInBackground = secondsInBackground;
            TimesFocused = timesFocused;
            TimesOpened = timesOpened;

            screenTimeApps.Add(Path, this);
        }

        public static ScreenTimeApp CreateOrGetScreenTimeApp(string name, string path)
        {
            if (screenTimeApps.TryGetValue(path, out ScreenTimeApp? screenTimeApp) && screenTimeApp != null)
            {
                return screenTimeApp;
            }
            return new ScreenTimeApp(name, path, [], [], [], []);
        }

        public static List<ScreenTimeApp> GetScreenTimeAppsByDate(string date)
        {
            List<ScreenTimeApp> appsForDate = [];

            foreach (ScreenTimeApp screenTimeApp in screenTimeApps.Values)
            {
                if (screenTimeApp.SecondsInFocus.ContainsKey(date) ||
                    screenTimeApp.SecondsInBackground.ContainsKey(date) ||
                    screenTimeApp.TimesFocused.ContainsKey(date) ||
                    screenTimeApp.TimesOpened.ContainsKey(date))
                {
                    appsForDate.Add(screenTimeApp);
                }
            }

            return appsForDate;
        }

        // Sorted, longest time
        public static List<ScreenTimeApp> GetScreenTimeAppsByDateSorted(string date, SortMode sortMode, bool reversed)
        {
            List<ScreenTimeApp> apps = GetScreenTimeAppsByDate(date);

            switch (sortMode)
            {
                case SortMode.SECONDS_IN_FOCUS:
                    if (reversed)
                        apps.Sort((x, y) => y.SecondsInFocus.GetValueOrDefault(date).CompareTo(x.SecondsInFocus.GetValueOrDefault(date)));
                    else
                        apps.Sort((x, y) => x.SecondsInFocus.GetValueOrDefault(date).CompareTo(y.SecondsInFocus.GetValueOrDefault(date)));
                    break;
                
                case SortMode.SECONDS_IN_BACKGROUND:
                    if (reversed)
                        apps.Sort((x, y) => y.SecondsInBackground.GetValueOrDefault(date).CompareTo(x.SecondsInBackground.GetValueOrDefault(date)));
                    else
                        apps.Sort((x, y) => x.SecondsInBackground.GetValueOrDefault(date).CompareTo(y.SecondsInBackground.GetValueOrDefault(date)));
                    break;

                case SortMode.TIMES_FOCUSED:
                    if (reversed)
                        apps.Sort((x, y) => y.TimesFocused.GetValueOrDefault(date).CompareTo(x.TimesFocused.GetValueOrDefault(date)));
                    else
                        apps.Sort((x, y) => x.TimesFocused.GetValueOrDefault(date).CompareTo(y.TimesFocused.GetValueOrDefault(date)));
                    break;

                case SortMode.TIMES_OPENED:
                    if (reversed)
                        apps.Sort((x, y) => y.TimesOpened.GetValueOrDefault(date).CompareTo(x.TimesOpened.GetValueOrDefault(date)));
                    else
                        apps.Sort((x, y) => x.TimesOpened.GetValueOrDefault(date).CompareTo(y.TimesOpened.GetValueOrDefault(date)));
                    break;
            }

            return apps;
        }

        public void IncreaseSecondsInFocus(uint seconds)
        {
            if (SecondsInFocus.ContainsKey(DateTimeUtils.CurrentDate()))
            {
                SecondsInFocus[DateTimeUtils.CurrentDate()] += seconds;
            }
            else
            {
                SecondsInFocus.Add(DateTimeUtils.CurrentDate(), seconds);
            }
        }

        public void IncreaseSecondsInBackground(uint seconds)
        {
            if (SecondsInBackground.ContainsKey(DateTimeUtils.CurrentDate()))
            {
                SecondsInBackground[DateTimeUtils.CurrentDate()] += seconds;
            }
            else
            {
                SecondsInBackground.Add(DateTimeUtils.CurrentDate(), seconds);
            }
        }

        public void IncreaseTimesFocused(uint seconds)
        {
            if (TimesFocused.ContainsKey(DateTimeUtils.CurrentDate()))
            {
                TimesFocused[DateTimeUtils.CurrentDate()] += seconds;
            }
            else
            {
                TimesFocused.Add(DateTimeUtils.CurrentDate(), seconds);
            }
        }

        public void IncreaseTimesOpened(uint seconds)
        {
            if (TimesOpened.ContainsKey(DateTimeUtils.CurrentDate()))
            {
                TimesOpened[DateTimeUtils.CurrentDate()] += seconds;
            }
            else
            {
                TimesOpened.Add(DateTimeUtils.CurrentDate(), seconds);
            }
        }
    }
}

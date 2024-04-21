using Newtonsoft.Json;
using ScreenTime.utils;

namespace ScreenTime.classes
{
    internal class ScreenTimeApp
    {
        public static Dictionary<string, ScreenTimeApp> screenTimeApps = [];

        public string Name { get; private set; }
        public string Path { get; private set; }
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

        public void IncreaseTimesFocused()
        {
            if (TimesFocused.ContainsKey(DateTimeUtils.CurrentDate()))
            {
                TimesFocused[DateTimeUtils.CurrentDate()] += 1;
            }
            else
            {
                TimesFocused.Add(DateTimeUtils.CurrentDate(), 1);
            }
        }

        public void IncreaseTimesOpened()
        {
            if (TimesOpened.ContainsKey(DateTimeUtils.CurrentDate()))
            {
                TimesOpened[DateTimeUtils.CurrentDate()] += 1;
            }
            else
            {
                TimesOpened.Add(DateTimeUtils.CurrentDate(), 1);
            }
        }
    }
}

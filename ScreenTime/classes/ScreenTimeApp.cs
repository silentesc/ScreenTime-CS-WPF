using Newtonsoft.Json;
using ScreenTime.utils;

namespace ScreenTime.classes
{
    internal class ScreenTimeApp
    {
        public static List<ScreenTimeApp> screenTimeApps = [];

        public string Name { get; private set; }
        public string Path { get; private set; }
        public Dictionary<string, uint> SecondsInFocus { get; private set; }
        public Dictionary<string, uint> TimesFocused { get; private set; }
        public Dictionary<string, uint> TimesOpened { get; private set; }
        public Dictionary<string, uint> TimesClosed { get; private set; }

        [JsonConstructor]
        private ScreenTimeApp(string name, string path, Dictionary<string, uint> secondsInFocus, Dictionary<string, uint> timesFocused, Dictionary<string, uint> timesOpened, Dictionary<string, uint> timesClosed)
        {
            Name = name;
            Path = path;
            SecondsInFocus = secondsInFocus;
            TimesFocused = timesFocused;
            TimesOpened = timesOpened;
            TimesClosed = timesClosed;

            screenTimeApps.Add(this);
        }

        public static ScreenTimeApp CreateOrGetScreenTimeApp(string name, string path)
        {
            foreach (var app in screenTimeApps)
            {
                if (app.Path == path)
                {
                    return app;
                }
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

        public void IncreaseTimesClosed()
        {
            if (TimesClosed.ContainsKey(DateTimeUtils.CurrentDate()))
            {
                TimesClosed[DateTimeUtils.CurrentDate()] += 1;
            }
            else
            {
                TimesClosed.Add(DateTimeUtils.CurrentDate(), 1);
            }
        }
    }
}

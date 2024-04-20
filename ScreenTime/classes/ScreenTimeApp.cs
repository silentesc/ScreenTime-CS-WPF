using Newtonsoft.Json;

namespace ScreenTime.classes
{
    internal class ScreenTimeApp
    {
        public static List<ScreenTimeApp> screenTimeApps = [];

        public string Name { get; private set; }
        public string Path { get; private set; }
        public uint SecondsInFocus { get; private set; }
        public uint TimesFocused { get; private set; }
        /* TODO
         * how long in focus a day (like in iOS settings when you can skip through days)
         * how often focused a day (like in iOS settings when you can skip through days)
         */

        [JsonConstructor]
        private ScreenTimeApp(string name, string path, uint secondsInFocus, uint timesFocused)
        {
            Name = name;
            Path = path;
            SecondsInFocus = secondsInFocus;
            TimesFocused = timesFocused;

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
            return new ScreenTimeApp(name, path, 0, 0);
        }

        public void IncreaseFocusSeconds(uint seconds)
        {
            SecondsInFocus += seconds;
        }

        public void IncreaseFocusCount()
        {
            TimesFocused += 1;
        }
    }
}

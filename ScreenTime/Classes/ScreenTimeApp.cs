using Newtonsoft.Json;

namespace ScreenTime.Classes
{
    internal class ScreenTimeApp
    {
        public readonly static List<ScreenTimeApp> apps = [];

        public string Name { get; }
        public string Path { get; }
        public ulong SecondsInForeground { get; private set; }
        public ulong SecondsInBackground { get; private set; }

        [JsonConstructor]
        internal ScreenTimeApp(string name, string path, ulong secondsInForeground, ulong secondsInBackground)
        {
            Name = name;
            Path = path;
            SecondsInForeground = secondsInForeground;
            SecondsInBackground = secondsInBackground;

            apps.Add(this);
        }

        public static ScreenTimeApp Create(string name, string path, ulong secondsInForeground, ulong secondsInBackground)
        {
            foreach (ScreenTimeApp app in apps)
            {
                if (app.Path == path) return app;
            }

            return new ScreenTimeApp(name, path, secondsInForeground, secondsInBackground);
        }

        public void IncreaseSecondsInForeground(ulong seconds)
        {
            SecondsInForeground += seconds;
        }

        public void IncreaseSecondsInBackground(ulong seconds)
        {
            SecondsInBackground += seconds;
        }
    }
}

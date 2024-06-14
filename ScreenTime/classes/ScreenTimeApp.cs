using Newtonsoft.Json;
using ScreenTime.utils;
using System.Diagnostics;

namespace ScreenTime.classes
{
    internal class ScreenTimeApp
    {
        // Path, ScreenTimeApp
        public static readonly Dictionary<string, ScreenTimeApp> screenTimeApps = [];

        // Name is changable by the user, at default it's the process name
        // You can get the filename/processname via. the Path
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
            if ((screenTimeApps.TryGetValue(name, out ScreenTimeApp? screenTimeApp) || screenTimeApps.TryGetValue(path, out screenTimeApp)) &&
                screenTimeApp != null)
            {
                return screenTimeApp;
            }
            return new ScreenTimeApp(name, path, [], [], [], []);
        }

        /*
         * Takes the newer apps path and merges the rest if there are duplicates
         */
        public static void MergePossibleNameConflicts()
        {
            /*
             * For each app
             *   if app is already in mergedApps
             *     add seconds/count
             *     update path if different (the later the app has been added the newer the path is -> maybe path change due to update, e.g. discord)
             *   else
             *     add it to the list
             */
            List<ScreenTimeApp> mergedApps = [];

            foreach (ScreenTimeApp screenTimeApp in screenTimeApps.Values)
            {
                /*
                 * Trys to get the first ScreenTimeApp that has been already added to the mergedApps list
                 * If none already exist, it adds it to the mergedApps list
                 * That means the next one is a duplicate and the logic will be applied
                 */
                ScreenTimeApp? mergedApp = mergedApps.Where(item => item.Name == screenTimeApp.Name).ToList().FirstOrDefault();

                if (mergedApp == null)
                {
                    mergedApps.Add(screenTimeApp);
                    continue;
                }

                mergedApp.Path = screenTimeApp.Path;

                /*
                 * Instead of looping through all attributes in seperate loops we just put that in a dictionary
                 * where the key is the mergedApp (already existing app) variable and the value is the screenTimeApp (thats the newer version) variable
                 * New variables just have to be added to the dictionary and will be included
                 */
                Dictionary<Dictionary<string, uint>, Dictionary<string, uint>> variables = new()
                {
                    {mergedApp.SecondsInFocus, screenTimeApp.SecondsInFocus },
                    {mergedApp.SecondsInBackground, screenTimeApp.SecondsInBackground },
                    {mergedApp.TimesFocused, screenTimeApp.TimesFocused },
                    {mergedApp.TimesOpened, screenTimeApp.TimesOpened }
                };

                foreach (KeyValuePair<Dictionary<string, uint>, Dictionary<string, uint>> variable in variables)
                {
                    foreach (KeyValuePair<string, uint> keyValuePair in variable.Value)
                    {
                        if (variable.Key.ContainsKey(keyValuePair.Key))
                        {
                            variable.Key[keyValuePair.Key] += keyValuePair.Value;
                        }
                        else
                        {
                            variable.Key[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }
                }
            }

            // Save apps in list
            screenTimeApps.Clear();
            foreach (ScreenTimeApp mergedApp in mergedApps)
            {
                screenTimeApps.Add(mergedApp.Path, mergedApp);
            }
        }

        public static List<ScreenTimeApp> GetScreenTimeAppsByName(string name)
        {
            List<ScreenTimeApp> apps = [];

            foreach (ScreenTimeApp screenTimeApp in screenTimeApps.Values)
            {
                if (string.Equals(screenTimeApp.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    apps.Add(screenTimeApp);
                }
            }

            return apps;
        }

        // Sorted, longest time
        public static List<ScreenTimeApp> GetScreenTimeAppsByDateSorted(string date, SortMode sortMode, bool reversed)
        {
            List<ScreenTimeApp> appsForDate = [];

            foreach (ScreenTimeApp screenTimeApp in screenTimeApps.Values)
            {
                if (screenTimeApp.SecondsInFocus.ContainsKey(date) && sortMode == SortMode.SECONDS_IN_FOCUS ||
                    screenTimeApp.SecondsInBackground.ContainsKey(date) && sortMode == SortMode.SECONDS_IN_BACKGROUND ||
                    screenTimeApp.TimesFocused.ContainsKey(date) && sortMode == SortMode.TIMES_FOCUSED ||
                    screenTimeApp.TimesOpened.ContainsKey(date) && sortMode == SortMode.TIMES_OPENED)
                {
                    appsForDate.Add(screenTimeApp);
                }
            }

            switch (sortMode)
            {
                case SortMode.SECONDS_IN_FOCUS:
                    if (reversed)
                        appsForDate.Sort((x, y) => y.SecondsInFocus.GetValueOrDefault(date).CompareTo(x.SecondsInFocus.GetValueOrDefault(date)));
                    else
                        appsForDate.Sort((x, y) => x.SecondsInFocus.GetValueOrDefault(date).CompareTo(y.SecondsInFocus.GetValueOrDefault(date)));
                    break;

                case SortMode.SECONDS_IN_BACKGROUND:
                    if (reversed)
                        appsForDate.Sort((x, y) => y.SecondsInBackground.GetValueOrDefault(date).CompareTo(x.SecondsInBackground.GetValueOrDefault(date)));
                    else
                        appsForDate.Sort((x, y) => x.SecondsInBackground.GetValueOrDefault(date).CompareTo(y.SecondsInBackground.GetValueOrDefault(date)));
                    break;

                case SortMode.TIMES_FOCUSED:
                    if (reversed)
                        appsForDate.Sort((x, y) => y.TimesFocused.GetValueOrDefault(date).CompareTo(x.TimesFocused.GetValueOrDefault(date)));
                    else
                        appsForDate.Sort((x, y) => x.TimesFocused.GetValueOrDefault(date).CompareTo(y.TimesFocused.GetValueOrDefault(date)));
                    break;

                case SortMode.TIMES_OPENED:
                    if (reversed)
                        appsForDate.Sort((x, y) => y.TimesOpened.GetValueOrDefault(date).CompareTo(x.TimesOpened.GetValueOrDefault(date)));
                    else
                        appsForDate.Sort((x, y) => x.TimesOpened.GetValueOrDefault(date).CompareTo(y.TimesOpened.GetValueOrDefault(date)));
                    break;
            }

            return appsForDate;
        }

        public bool IsAppRunning()
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(Path);
            Process[] processes = Process.GetProcessesByName(filename);

            foreach (Process process in processes)
            {
                try
                {
                    ProcessModule? mainModule = process.MainModule;
                    if (mainModule == null) continue;

                    if (string.Equals(mainModule.FileName, Path, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Access is denied to the process. Can happen for system processes.
                    continue;
                }
                catch (InvalidOperationException)
                {
                    // Process has existed.
                    continue;
                }
            }
            return false;
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

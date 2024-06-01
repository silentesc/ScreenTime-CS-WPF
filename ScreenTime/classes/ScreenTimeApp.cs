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
            // TODO Check for conflict in name and merge

            if (screenTimeApps.TryGetValue(path, out ScreenTimeApp? screenTimeApp) && screenTimeApp != null)
            {
                return screenTimeApp;
            }
            return new ScreenTimeApp(name, path, [], [], [], []);
        }

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


            // FIXME bugged
            List<ScreenTimeApp> mergedApps = [];
            bool conflictFound = true;

            while (conflictFound)
            {
                for (int i = 0; i < screenTimeApps.Count; i++)
                {
                    // Get all apps with same name
                    string appName = screenTimeApps.Values.ToList()[i].Name;
                    List<ScreenTimeApp> appsForName = GetScreenTimeAppsByName(appName);
                    
                    // Remove apps with name
                    appsForName.ForEach(app => screenTimeApps.Remove(app.Path));

                    // Check if conflict found
                    if (appsForName.Count == 0)
                    {
                        Debug.WriteLine("0 APPS ????????????");
                        conflictFound = false;
                        continue;
                    }
                    if (appsForName.Count == 1)
                    {
                        mergedApps.Add(appsForName[0]);
                        conflictFound = false;
                        continue;
                    }

                    // Copy values
                    string path = appsForName.Last().Path;
                    Dictionary<string, uint> secondsInFocus = [];
                    Dictionary<string, uint> secondsInBackground = [];
                    Dictionary<string, uint> timesFocused = [];
                    Dictionary<string, uint> timesOpened = [];

                    foreach (ScreenTimeApp app in appsForName)
                    {
                        Dictionary<Dictionary<string, uint>, Dictionary<string, uint>> variables = new()
                        {
                            { secondsInFocus, app.SecondsInFocus },
                            { secondsInBackground, app.SecondsInBackground },
                            { timesFocused, app.TimesFocused },
                            { timesOpened, app.TimesOpened }
                        };

                        // This uses the "variables" variable to not do the same thing 4 times
                        // It adds the seconds/count to the date if there is one or creates the date with the seconds if not
                        foreach (KeyValuePair<Dictionary<string, uint>, Dictionary<string, uint>> variable in variables)
                        {
                            foreach (KeyValuePair<string, uint> keyValuePair in variable.Value)
                            {
                                if (variable.Key.TryGetValue(keyValuePair.Key, out uint output))
                                {
                                    variable.Key[keyValuePair.Key] += output;
                                }
                                else
                                {
                                    variable.Key[keyValuePair.Key] = output;
                                }
                            }
                        }
                    }

                    //Make new app and add to list
                    ScreenTimeApp mergedApp = new(appName, path, secondsInFocus, secondsInBackground, timesFocused, timesOpened);
                    mergedApps.Add(mergedApp);

                    // Break to start checking for conflicts from the beginning again
                    break;
                }
            }

            mergedApps.ForEach(app =>
            {
                Debug.WriteLine($"{app.Name} | {app.Path}");
            });

            // Save apps
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

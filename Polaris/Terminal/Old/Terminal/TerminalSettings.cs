namespace Polaris.Debug.Terminal
{
    public class TerminalSettings
    { 
        // When true, all logs are saved in a log file.
        public bool OutputToFile { get; set; }

        // When true, the terminal also outputs all of
        // UnityEngine.Debug logs.
        public bool IncludeUnityLogs { get; set; }

        // When true, the terminal saves the user's entered
        // commands in a stack, which can then be accessed.
        public bool EnablePersistentHistory { get; set; }

        // The search mode that the terminal will use when looking
        // for terminal interfaces.
        public SearchMode SearchMode { get; set; }

        // When true, all logs are saved in a list, which can then
        // be filtered. This enhances debugging; uses more memory
        // and the terminal will remove older logs to conserve memory.
        public bool EnableFiltering { get; set; }

        public int MaxTerminals { get; set; }

        public TerminalSettings(bool outputToFile, bool includeUnityLogs, bool enableHistory, SearchMode searchMode, bool enableFiltering, int maxTerminals)
        {
            OutputToFile = outputToFile;
            IncludeUnityLogs = includeUnityLogs;
            EnablePersistentHistory = enableHistory;
            SearchMode = searchMode;
            EnableFiltering = enableFiltering;
            MaxTerminals = maxTerminals;
        }
    }
}

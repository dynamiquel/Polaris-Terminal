namespace Polaris.Debug
{
    // This class contains data that is general for debugging.
    public static class Settings
    {
        public static PermissionLevel CurrentPermissionLevel { get; set; }
        const PermissionLevel DefaultPermissionLevel = PermissionLevel.Developer;
        public const string TerminalTag = "Terminal";

        static Settings()
        {
            CurrentPermissionLevel = DefaultPermissionLevel;
        }
    }
}

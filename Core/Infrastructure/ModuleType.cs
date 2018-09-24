namespace Core.Infrastructure
{
    // This ordering determines how it is displayed in application
    public enum ModuleType
    {
        // Modules for a specific collection
        Books,
        Shelves,
        Series,
        Notes,
        // Plugin modules (these are also specific to a collection
        Plugin,
        // Generic modules
        Collections,
        Modules,
        Logs,
        Settings
    };
}

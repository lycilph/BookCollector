using Core.Infrastructure;
using System.Collections.Generic;

namespace Core
{
    public static class Constants
    {
        public static readonly string SettingsFilename = "settings.json";
        public static readonly string CollectionExtension = ".bcdb"; // bcdb = Book Collector DataBase
        public static readonly string CollectionDialogFilter = $"Collection file (*{CollectionExtension})|*{CollectionExtension}";

        // Module categories
        public static readonly List<ModuleType> CollectionModules = new List<ModuleType> { ModuleType.Books, ModuleType.Shelves, ModuleType.Series, ModuleType.Notes };
        public static readonly List<ModuleType> PluginModules = new List<ModuleType> { ModuleType.Plugin };
        public static readonly List<ModuleType> GenericModules = new List<ModuleType> { ModuleType.Collections, ModuleType.Modules, ModuleType.Logs, ModuleType.Settings };
    }
}

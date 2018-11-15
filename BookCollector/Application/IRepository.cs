using BookCollector.Data;

namespace BookCollector.Application
{
    public interface IRepository
    {
        Settings LoadSettings();
        void SaveSettings(Settings settings);

        Collection LoadCollection(string filename);
        void SaveCollection(Collection collection);
        Collection CreateCollection(string filename);
        void DeleteCollection(string filename);
    }
}
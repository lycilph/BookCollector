using Core.Data;

namespace Core.Application
{
    public interface IRepository
    {
        Settings LoadSettings();
        void SaveSettings(Settings settings);

        Collection LoadCollection(string filename);
        void SaveCollection(Collection collection);
        Collection CreateCollection(string collection_name);
        void DeleteCollection(string filename);
    }
}

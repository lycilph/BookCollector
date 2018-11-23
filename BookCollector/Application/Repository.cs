using BookCollector.Data;
using NLog;
using Panda.Utils;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BookCollector.Application
{
    public class Repository : IRepository
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string app_path;
        private readonly string settings_path;

        public Repository()
        {
            var app_filename = Assembly.GetExecutingAssembly().Location;
            app_path = Path.GetDirectoryName(app_filename);
            settings_path = Path.Combine(app_path, Constants.SettingsFilename);
        }

        public Settings LoadSettings()
        {
            if (File.Exists(settings_path))
            {
                logger.Trace("Loading settings");
                return JsonUtils.ReadFromFile<Settings>(settings_path);
            }
            else
            {
                logger.Trace("No settings file found, creating new one");
                return new Settings();
            }
        }

        public void SaveSettings(Settings settings)
        {
            logger.Trace("Saving settings");

            JsonUtils.WriteToFile(settings_path, settings);
        }

        public Collection LoadCollection(string filename)
        {
            if (!File.Exists(filename))
            {
                logger.Trace($"Could not find file {filename}");
                return null;
            }

            Collection collection = null;
            try
            {
                logger.Trace($"Loading collection {filename}");

                collection = JsonUtils.ReadFromFile<Collection>(filename);
                UpdateShelves(collection);
                collection.Filename = filename;
                collection.IsDirty = false;
            }
            catch (Exception)
            {
                logger.Trace($"Could not load file {filename}");
            }

            return collection;
        }

        public void SaveCollection(Collection collection)
        {
            if (collection == null)
            {
                logger.Trace("Collection is null, skipping saving it");
                return;
            }

            // If the collection doesn't have a filename, create one
            if (string.IsNullOrWhiteSpace(collection.Filename))
            {
                var filename = string.Empty;
                if (!string.IsNullOrWhiteSpace(collection.Name))
                {
                    filename = collection.Name.MakeFilenameSafe();
                }
                else
                {
                    var guid = Guid.NewGuid().ToString();
                    collection.Name = guid;
                    filename = guid;
                }
                filename += Constants.CollectionExtension;

                collection.Filename = Path.Combine(app_path, filename);
            }

            if (File.Exists(collection.Filename) && !collection.IsDirty)
            {
                logger.Trace("Collection has not been changed, skipping save");
                return;
            }

            logger.Trace($"Saving collection {collection.Name} as [{collection.Filename}]");

            JsonUtils.WriteToFile(collection.Filename, collection);
            collection.IsDirty = false;
        }

        public Collection CreateCollection(string filename)
        {
            var collection = new Collection
            {
                Name = Path.GetFileNameWithoutExtension(filename),
                Filename = filename
            };
            SaveCollection(collection);
            return collection;
        }

        public void DeleteCollection(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
            {
                logger.Trace($"Could not delete the collection {filename}");
                return;
            }

            logger.Trace($"Deleting collection {filename}");
            File.Delete(filename);
        }

        private void UpdateShelves(Collection collection)
        {
            foreach (var shelf in collection.Shelves)
                shelf.Books = collection.Books.Where(b => b.Shelf == shelf).ToList();
        }
    }
}

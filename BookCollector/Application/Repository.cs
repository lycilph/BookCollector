using Core;
using Core.Application;
using Core.Data;
using NLog;
using Panda.Utils;
using System;
using System.IO;
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
                collection.Filename = filename;
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

            logger.Trace($"Saving collection {collection.Name}");

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

            JsonUtils.WriteToFile(collection.Filename, collection);
        }

        public Collection CreateCollection(string collection_name = "")
        {
            var collection = new Collection { Name = collection_name };
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
    }
}

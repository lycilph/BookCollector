using BookCollector.Data;
using Panda.Utils;
using System.Collections.Generic;
using System.Linq;

namespace BookCollector.Application.Controllers
{
    public class ImportController : IImportController
    {
        private IStateManager state_manager;

        public ImportController(IStateManager state_manager)
        {
            this.state_manager = state_manager;
        }

        public void Import(List<Book> books)
        {
            if (state_manager.Settings.EnableShelfMatching)
            {
                var map = CreateShelvesMap(books);
                books.Apply(b => b.Shelf = map[b.Metadata["ExclusiveShelf"]]);
            }
            else
            {
                var default_shelf = state_manager.CurrentCollection.GetDefaultshelf();
                books.Apply(b => b.Shelf = default_shelf);
            }

            state_manager.CurrentCollection.Add(books);
        }

        private Dictionary<string, Shelf> CreateShelvesMap(List<Book> books)
        {
            var shelf_mapping = new Dictionary<string, Shelf>();
            var goodreads_shelves = books.Select(b => b.Metadata["ExclusiveShelf"])
                                         .Distinct()
                                         .ToList();

            foreach (var shelf in goodreads_shelves)
            {
                var edit_distances = state_manager.CurrentCollection.Shelves
                                                  .Select(s => new { Shelf = s, EditDistance = StringMetrics.EditDistance(s.Name, shelf) })
                                                  .OrderBy(x => x.EditDistance);
                var closest_match = edit_distances.First();

                // Check if we found a match
                if (closest_match.EditDistance <= state_manager.Settings.MaxEditDistanceForShelfMatching)
                {
                    shelf_mapping.Add(shelf, closest_match.Shelf);
                }
                else 
                {
                    // If not match found, then create one
                    if (state_manager.Settings.CreateUnmatchedShelves)
                    {
                        var new_shelf = state_manager.CurrentCollection.AddShelf(shelf);
                        shelf_mapping.Add(shelf, new_shelf);
                    }
                    else // Or use the default one
                    {
                        shelf_mapping.Add(shelf, state_manager.CurrentCollection.GetDefaultshelf());
                    }
                }
            }

            return shelf_mapping;
        }
    }
}

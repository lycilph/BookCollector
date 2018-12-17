using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;

namespace BookCollector.Application.Controllers
{
    public class ThemeController : IThemeController
    {
        private IStateManager state_manager;
        private PaletteHelper palette_helper = new PaletteHelper();
        private SwatchesProvider swatches_provider = new SwatchesProvider();

        public ThemeController(IStateManager state_manager)
        {
            this.state_manager = state_manager;
        }

        public void Initialize()
        {
            Set(state_manager.Settings.PrimaryColorName, state_manager.Settings.AccentColorName);
        }

        public List<Swatch> GetPrimaryColors()
        {
            return swatches_provider.Swatches.OrderBy(s => s.Name).ToList();
        }

        public List<Swatch> GetAccentColors()
        {
            return swatches_provider.Swatches.Where(s => s.IsAccented).OrderBy(s => s.Name).ToList();
        }

        public void SetBase(bool is_dark)
        {
            palette_helper.SetLightDark(is_dark);
        }

        public void Set(string primary_color, string accent_color)
        {
            palette_helper.ReplacePrimaryColor(primary_color);
            palette_helper.ReplaceAccentColor(accent_color);
        }

        public void SetPrimary(string color)
        {
            palette_helper.ReplacePrimaryColor(color);
        }

        public void SetAccent(string color)
        {
            palette_helper.ReplaceAccentColor(color);
        }
    }
}

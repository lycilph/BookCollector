using MaterialDesignColors;
using System.Collections.Generic;

namespace BookCollector.Application.Controllers
{
    public interface IThemeController
    {
        void Initialize();
        List<Swatch> GetPrimaryColors();
        List<Swatch> GetAccentColors();
        void SetBase(bool is_dark);
        void Set(string primary_color, string accent_color);
        void SetPrimary(string color);
        void SetAccent(string color);
    }
}
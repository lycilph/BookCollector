using NHunspell;
using Panda.UI.AvalonEdit;

namespace BookCollector.Screens.Notes
{
    public partial class NotesView
    {
        public NotesView()
        {
            InitializeComponent();

            if (SpellChecker.Default.HunspellInstance == null)
                SpellChecker.Default.HunspellInstance = new Hunspell(@".\Content\en_US.aff", @".\Content\en_US.dic");

            editor.TextArea.TextView.LineTransformers.Add(new SpellCheckerColorizer());
        }
    }
}

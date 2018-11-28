using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows;
using System.Windows.Media;

namespace BookCollector.Screens.Notes.AvalonEdit
{
    public class SpellCheckerColorizer : DocumentColorizingTransformer
    {
        private readonly TextDecorationCollection decorations;

        public SpellCheckerColorizer()
        {
            decorations = new TextDecorationCollection
            {
                new TextDecoration()
                {
                    Pen = new Pen
                    {
                        Thickness = 2,
                        DashStyle = DashStyles.Solid,
                        Brush = new SolidColorBrush(Colors.Red)
                    },
                    PenThicknessUnit = TextDecorationUnit.FontRecommended
                }
            };
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            var lineText = CurrentContext.Document.Text.Substring(line.Offset, line.Length);
            foreach (var error in SpellChecker.Default.FindSpellingErrors(lineText))
            {
                var start_offset = line.Offset + error.Index;
                var end_offset = line.Offset + error.Index + error.Value.Length;
                void action(VisualLineElement e) => e.TextRunProperties.SetTextDecorations(decorations);

                ChangeLinePart(start_offset, end_offset, action);
            }
        }
    }
}

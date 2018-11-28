using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BookCollector.Screens.Notes.AvalonEdit
{
    public class SpellCheckerBehavior : Behavior<TextEditor>
    {
        private TextEditor text_editor;
        private List<Control> original_items;
        private SpellCheckerColorizer colorizer = new SpellCheckerColorizer();

        protected override void OnAttached()
        {
            text_editor = AssociatedObject;
            if (text_editor != null)
            {
                text_editor.ContextMenuOpening += TextEditorContextMenuOpening;
                text_editor.TextArea.MouseRightButtonDown += TextAreaMouseRightButtonDown;

                text_editor.TextArea.TextView.LineTransformers.Add(colorizer);

                if (text_editor.ContextMenu == null)
                    text_editor.ContextMenu = new ContextMenu();
                original_items = text_editor.ContextMenu.Items.OfType<Control>().ToList();
            }
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (text_editor != null)
            {
                text_editor.ContextMenuOpening -= TextEditorContextMenuOpening;
                text_editor.TextArea.MouseRightButtonDown -= TextAreaMouseRightButtonDown;

                text_editor.TextArea.TextView.LineTransformers.Remove(colorizer);

                original_items = null;
                text_editor = null;
            }
            base.OnDetaching();
        }

        public void TextAreaMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = text_editor.GetPositionFromPoint(e.GetPosition(text_editor));
            if (position.HasValue)
            {
                text_editor.TextArea.Caret.Position = position.Value;
            }
        }

        public void TextEditorContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Remove old entries in the menu
            foreach (Control item in text_editor.ContextMenu.Items.OfType<Control>().ToList())
            {
                if (original_items.Contains(item)) { continue; }
                text_editor.ContextMenu.Items.Remove(item);
            }

            var position = text_editor.TextArea.Caret.Position;
            Match word = null;
            Regex r = new Regex(@"\w+");
            var line = text_editor.Document.GetText(text_editor.Document.GetLineByNumber(position.Line));
            foreach (Match m in r.Matches(line))
            {
                if (m.Index >= position.VisualColumn) { break; }
                word = m;
            }
            if (null == word ||
                position.Column > word.Index + word.Value.Length ||
                SpellChecker.Default.Spell(word.Value))
            {
                e.Handled = true;
                return;
            }

            if (original_items.Any())
                text_editor.ContextMenu.Items.Insert(0, new Separator());

            // If no suggestions were found, add a disabled header
            var suggestions = SpellChecker.Default.Suggest(word.Value);
            if (!suggestions.Any())
            {
                text_editor.ContextMenu.Items.Insert(0,
                    new MenuItem() { Header = "<No suggestions found>", IsEnabled = false });
                return;
            }

            // Add new suggestions to the menu
            foreach (string suggestion in suggestions)
            {
                var item = new MenuItem { Header = suggestion, FontWeight = FontWeights.Bold };
                item.Tag =
                    new Tuple<int, int>(
                        text_editor.Document.GetOffset(position.Line, word.Index + 1),
                        word.Value.Length);
                item.Click += ItemClick;
                text_editor.ContextMenu.Items.Insert(0, item);
            }
        }

        private void ItemClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem item))
                return;
            if (!(item.Tag is Tuple<int, int> segment))
                return;
            text_editor.Document.Replace(segment.Item1, segment.Item2, item.Header.ToString());
        }
    }
}

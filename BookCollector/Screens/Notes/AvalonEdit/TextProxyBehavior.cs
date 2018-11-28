using ICSharpCode.AvalonEdit;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace BookCollector.Screens.Notes.AvalonEdit
{
    public sealed class TextProxyBehavior : Behavior<TextEditor>
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextProxyBehavior), 
                new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextChanged));

        private static void TextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var behavior = obj as TextProxyBehavior;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject;
                if (editor.Document != null)
                {
                    if (args.NewValue == null)
                        editor.Document.Text = string.Empty;
                    else
                        editor.Document.Text = args.NewValue.ToString();
                }
                editor.Focus();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged += AssociatedObjectTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectTextChanged;
        }

        private void AssociatedObjectTextChanged(object sender, EventArgs e)
        {
            if (sender is TextEditor text_editor)
            {
                if (text_editor.Document != null)
                    Text = text_editor.Document.Text;
            }
        }
    }
}

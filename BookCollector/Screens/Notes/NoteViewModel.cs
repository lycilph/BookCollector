using Core.Data;
using Panda.Infrastructure;

namespace BookCollector.Screens.Notes
{
    public class NoteViewModel : ItemViewModel<Note>
    {
        public string Name { get => Obj.Name; set => Obj.Name = value; }
        public string Text { get => Obj.Text; set => Obj.Text = value; }

        public NoteViewModel(Note obj) : base(obj) {}
    }
}

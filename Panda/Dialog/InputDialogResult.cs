namespace Panda.Dialog
{
    public class InputDialogResult : DialogResult
    {
        public string Input { get; set; }

        public InputDialogResult() { }
        public InputDialogResult(bool? result, string input) : base(result)
        {
            Input = input;
        }
    }
}

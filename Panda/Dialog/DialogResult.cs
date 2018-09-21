namespace Panda.Dialog
{
    public class DialogResult
    {
        public bool? Result { get; set; }

        public DialogResult() { }
        public DialogResult(bool? result)
        {
            Result = result;
        }
    }
}

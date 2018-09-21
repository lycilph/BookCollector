namespace Panda.Dialog
{
    public class FileDialogResult : DialogResult
    {
        public string Fullpath { get; set; }
        public string Filename { get; set; }

        public FileDialogResult() { }
        public FileDialogResult(bool? result, string fullpath, string filename) : base(result)
        {
            Fullpath = fullpath;
            Filename = filename;
        }
    }
}

namespace MySimpleNotepad.Models
{
    public enum FileTypes { BackFolder, Document, Folder, Hardrive, SystemHardrive }

    public class FileItem
    {
        public FileTypes Type { get; }
        public string Name { get; }

        public FileItem(FileTypes type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}

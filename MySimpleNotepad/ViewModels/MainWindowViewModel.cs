using MySimpleNotepad.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Path = System.IO.Path;
using static MySimpleNotepad.Models.FileTypes;

namespace MySimpleNotepad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ObservableCollection<FileItem> fileList = new();
        public ObservableCollection<FileItem> FileList { get => fileList; }

        private string currentDirectory = "";

        private void LoadDisks()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string? systemDirectory = Path.GetPathRoot(system);

            fileList.Clear();

            foreach (DriveInfo drive in drives)
                fileList.Add(new FileItem(drive.Name == systemDirectory ? SystemHardrive : Hardrive, drive.Name));
        }

        private void LoadDirectory()
        {
            DirectoryInfo directory = new(currentDirectory);
            NaturalComparer naturalComparer = new();

            List<string> directories = new();
            foreach (var file  in directory.GetDirectories())
                directories.Add(file.Name);

            directories.Sort(naturalComparer);

            List<string> files = new();
            foreach (var file in directory.GetFiles()) 
                files.Add(file.Name);

            files.Sort(naturalComparer);

            fileList.Clear();
            fileList.Add(new FileItem(BackFolder, "..."));

            foreach (var name in directories)
                fileList.Add(new FileItem(Folder, name));

            foreach (var name in files)
                fileList.Add(new FileItem(Document, name));
        }
        private void Loader(bool start = false)
        {
            FileBox = "";

            if (start) currentDirectory = Directory.GetCurrentDirectory();

            if (currentDirectory == "") LoadDisks();
            else LoadDirectory();
        }

        private void UpdateButtonMode()
        {
            if (openMode) return;

            string path = Path.Combine(currentDirectory, fileBox);

            if (!File.Exists(path))
            {
                ButtonMode = "Открыть";
                return;
            } 

            var attributes = File.GetAttributes(path);
            bool isFile = (attributes & FileAttributes.Archive) != 0;
            ButtonMode = isFile ? "Сохранить" : "Открыть";
        }

        private string buttonMode = "";
        private bool openMode = false;
        private bool explorerMode = false;

        public bool ExplorerMode
        {
            get => explorerMode;
            set => this.RaiseAndSetIfChanged(ref explorerMode, value);
        }

        public string ButtonMode
        {
            get => buttonMode;
            set => this.RaiseAndSetIfChanged(ref buttonMode, value);
        }

        private void FunctionOpen()
        {
            if (explorerMode) return;
            ExplorerMode = true;
            Loader(true);
            ButtonMode = "Открыть";
            openMode = true;
        }

        private void FunctionSave()
        {
            if (explorerMode) return;
            ExplorerMode = true;
            Loader(true);
            ButtonMode = "Открыть";
            openMode = false;
        }

        private void FunctionOk() 
        {
            DoubleTap(); 
        }

        private void FunctionCancel()
        {
            if (!explorerMode) return;
            ExplorerMode = false;
        }

        private void SelectItem(FileItem item)
        {
            if (item == null) return;
            FileBox = item.Name;
            UpdateButtonMode();
        }

        private void Message(string message)
        {
            if (!fileBox.StartsWith(message)) 
                FileBox = message + fileBox;
        }

        public void DoubleTap()
        {
            if (!explorerMode) return;

            if (fileBox == "...")
            {
                var parentDirectory = Directory.GetParent(currentDirectory);
                currentDirectory = parentDirectory == null ? "" : parentDirectory.FullName;
                Loader();
                return;
            }

            if (currentDirectory == "")
            {
                if (Directory.Exists(fileBox))
                {
                    currentDirectory = fileBox;
                    Loader();
                }
                else 
                    Message("Не существует такого диска: ");
                return;
            }

            string path = Path.Combine(currentDirectory, fileBox);
            FileAttributes attributes;

            try
            {
                attributes = File.GetAttributes(path);
            }
            catch (IOException)
            {
                if (openMode) 
                    Message("Не существует папки/файла: ");
                else
                {
                    File.WriteAllText(path, contentBox);
                    ExplorerMode = false;
                }
                return;
            }

            bool isDirectory = (attributes & FileAttributes.Directory) != 0;
            bool isFile = (attributes & FileAttributes.Archive) != 0;

            if (isDirectory)
            {
                currentDirectory = path;
                Loader();
            }
            else if (isFile)
            {
                if (openMode)
                {
                    ContentBox = File.ReadAllText(path);
                    ExplorerMode = false;
                }
                else
                {
                    File.WriteAllText(path, contentBox);
                    ExplorerMode = false;
                }
            }
        }

        string contentBox = "";
        string fileBox = "";
        FileItem selectedItem = new(Document, "?");

        public string ContentBox
        {
            get => contentBox;
            set => this.RaiseAndSetIfChanged(ref contentBox, value);
        }

        public string FileBox
        {
            get => fileBox;
            set => this.RaiseAndSetIfChanged(ref fileBox, value);
        }

        public FileItem SelectedItem
        {
            get => selectedItem;
            set { selectedItem = value; SelectItem(value); }
        }

        public MainWindowViewModel() 
        {
            Open = ReactiveCommand.Create<Unit, Unit>(_ => { FunctionOpen(); return new Unit(); });
            Save = ReactiveCommand.Create<Unit, Unit>(_ => { FunctionSave(); return new Unit(); });
            Ok = ReactiveCommand.Create<Unit, Unit>(_ => { FunctionOk(); return new Unit(); });
            Cancel = ReactiveCommand.Create<Unit, Unit>(_ => { FunctionCancel(); return new Unit(); });
        }

        public ReactiveCommand<Unit, Unit> Open { get; }
        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}

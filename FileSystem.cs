using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileManager
{
    class FileSystem
    {
        private int selectedSystem;
        private string previousOperation; // variable for "cut" and "copy"
        private string fullPathOfFile;
        private string fileName;
        private bool IsFile = false;
        private readonly int countOfFiles;
        private readonly int countOfDisks;

        private readonly List<ListView> listViews = new List<ListView>();

        private readonly string[] paths;

        public FileSystem(int countOfFiles, params string[] paths)
        {
            this.countOfFiles = countOfFiles;
            this.paths = paths;
            countOfDisks = paths.Length;

            InitialRender();
        }

        private void InitialRender()
        {
            Rendering.InitialRender(countOfDisks, countOfFiles);

            int widthSecondColumn = (int)Math.Round(((Console.WindowWidth) / countOfDisks) * 0.2, 0);
            int widthFirstСolumn = (Console.WindowWidth / countOfDisks) - widthSecondColumn * 2 - 3;

            for (int i = 0; i < countOfDisks; i++)
            {
                listViews.Add(new ListView((Console.WindowWidth / countOfDisks) * i + 1, 2, countOfFiles)
                {
                    ColumnsWidth = new List<int> { widthFirstСolumn, widthSecondColumn, widthSecondColumn },
                    Items = GetItems(paths[i]),
                    Path = paths[i]
                });

                listViews[i].Selected += View_Selected;
                listViews[i].Parent += View_Parent;
                listViews[i].Copy += Copy;
                listViews[i].Paste += Paste;
                listViews[i].Cut += Cut;
                listViews[i].Properties += Properties;
                listViews[i].Rename += Rename;
                listViews[i].CreateFolder += CreateFolder;
                listViews[i].RenderPath(paths[i], countOfDisks);
            }

            listViews[0].OnFocused = true;

            for (int i = 0; i < countOfDisks; i++)
            {
                listViews[i].Render();
            }
        }

        public void Render()
        {
            while (true)
            {
                var key = Console.ReadKey();
                Update(key);
                listViews[selectedSystem].Update(key);
                listViews[selectedSystem].Render();
            }
        }
        
        private void Update(ConsoleKeyInfo key)
        {
            if(key.Key == ConsoleKey.F5)
            {
                ListOfDisks();
            }

            if (key.Key == ConsoleKey.RightArrow && selectedSystem + 1 < countOfDisks)
            {
                selectedSystem++;

                RenderAfterChangeSystem();
            }
            else if (key.Key == ConsoleKey.LeftArrow && selectedSystem - 1 >= 0)
            {
                selectedSystem--;

                RenderAfterChangeSystem();
            }
        }

        private void RenderAfterChangeSystem()
        {
            for (int i = 0; i < listViews.Count(); i++)
            {
                if (i != selectedSystem)
                {
                    listViews[i].OnFocused = false;
                    listViews[i].Clean();
                    listViews[i].Render();
                }
            }
            listViews[selectedSystem].OnFocused = true;
            listViews[selectedSystem].Clean();
            listViews[selectedSystem].Render();
        }

        private void ListOfDisks()
        {
            ListOfDisks listOfDisks = new ListOfDisks(countOfFiles, paths);
            listOfDisks.Selected += ListOfDisks_Selected;

            Rendering.Properties(countOfFiles, paths);
            listOfDisks.Render();
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    listOfDisks.Update(key);
                    RenderAfterChangeSystem();
                    break;
                }
                else
                {
                    listOfDisks.Update(key);
                    listOfDisks.Render();
                }
            }
        }

        private void ListOfDisks_Selected(object sender, EventArgs e)
        {
            var view = (ListOfDisks)sender;
            var info = view.SelectedItem;

            for (int i = 0; i < view.GetItems.Length; i++)
            {
                if (view.GetItems[i].Equals(info))
                {
                    selectedSystem = i;
                    break;
                }
            }
            view.Clean(countOfFiles, paths.Length);
        }

        private List<ListViewItem> GetItems(string path)
        {
            return new DirectoryInfo(path).GetFileSystemInfos().
                Select(f => new ListViewItem(
                    f,
                    f.Name,
                    f is DirectoryInfo directory ? "<dir>" : f.Extension,
                    f is FileInfo file ? Math.Round(file.Length*0.001).ToString() + " MB" : ""
                    )).ToList();
        }

        private void View_Selected(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            var info = view.SelectedItem.State;

            if (info is FileInfo file)
            {
                Process.Start(file.FullName);
            }
            else if (info is DirectoryInfo directory)
            {
                FileAttributes attributes = File.GetAttributes(directory.FullName);
                if(!(attributes.HasFlag(FileAttributes.Hidden)))
                {
                    this.fullPathOfFile = directory.FullName;
                    listViews[selectedSystem].RenderPath(fullPathOfFile, countOfDisks);

                    view.Clean();
                    view.Items = GetItems(directory.FullName);
                    view.PrevSelectedItem = directory.Parent.FullName;
                }
                else
                {
                    AccessDenied();
                }
            }
        }

        private void View_Parent(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            if (view.Items.Count != 0)
            {
                var info = view.SelectedItem.State;

                if (info is FileInfo file)
                {
                    if (file.Directory.Parent != null)
                    {
                        view.Clean();
                        view.Items = GetItems(file.Directory.Parent.FullName);
                        this.fullPathOfFile = file.Directory.Parent.FullName;
                        listViews[selectedSystem].RenderPath(fullPathOfFile, countOfDisks);
                    }
                }
                else if (info is DirectoryInfo directory)
                {
                    if(!string.IsNullOrEmpty(directory.Parent.ToString()))
                    {
                        view.Clean();
                        view.Items = GetItems(directory.Parent.Parent.FullName);
                        this.fullPathOfFile = directory.Parent.Parent.FullName;
                        listViews[selectedSystem].RenderPath(fullPathOfFile, countOfDisks);
                    }
                }
            }
            else
            {
                view.Clean();
                view.Items = GetItems(view.PrevSelectedItem);
                this.fullPathOfFile = view.PrevSelectedItem;
                listViews[selectedSystem].RenderPath(fullPathOfFile, countOfDisks);
            }

        }

        private void CopyCut(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            var info = view.SelectedItem.State;

            if (info is FileInfo file)
            {
                fullPathOfFile = file.FullName;
                this.fileName = file.Name;
                IsFile = true;
            }
            else if (info is DirectoryInfo directory)
            {
                fullPathOfFile = directory.FullName;
                this.fileName = directory.Name;
                IsFile = false;
            }
        }

        private void Copy(object sender, EventArgs e)
        {
            CopyCut(sender, e);
            previousOperation = "copy";
        }

        private void Cut(object sender, EventArgs e)
        {
            CopyCut(sender, e);
            previousOperation = "cut";
        }

        private void Paste(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.fileName))
            {
                var view = (ListView)sender;
                var info = view.SelectedItem.State;

                string newpath = null;

                if (IsFile)
                {
                    if (info is FileInfo file)
                    {
                        newpath = file.Directory.FullName;
                    }
                    else if (info is DirectoryInfo directory)
                    {
                        newpath = directory.FullName;
                    }

                    newpath += $"\\{this.fileName}";

                    try
                    {
                        if (previousOperation.Equals("copy"))
                            File.Copy(fullPathOfFile, newpath, true);
                        else if (previousOperation.Equals("cut"))
                            File.Move(fullPathOfFile, newpath);
                    }
                    catch(UnauthorizedAccessException)
                    {
                        AccessDenied();
                    }
                }
                else
                {
                    DirectoryInfo directory = null;

                    if (info is FileInfo file)
                    {
                        directory = file.Directory;
                    }
                    else if (info is DirectoryInfo dir)
                    {
                        directory = dir;
                    }

                    try
                    {
                        PasteInFolder(directory.FullName, this.fileName, this.fullPathOfFile);

                        if (previousOperation.Equals("cut"))
                        {
                            Directory.Delete(fullPathOfFile, true);
                        }
                    }
                    catch(UnauthorizedAccessException)
                    {
                        AccessDenied();
                    }
                }
            }
        }

        private void PasteInFolder(string directoryFullName, string destName, string destFullName)
        {
            DirectoryInfo di = new DirectoryInfo(directoryFullName);

            di.CreateSubdirectory(destName);

            string[] files = Directory.GetFiles(destFullName);
            
            foreach (string s in files)
            {
                string fileName = Path.GetFileName(s);
                string newpath = $"{directoryFullName}\\{destName}\\{fileName}";
                File.Copy(s, newpath, true);
            }

            if (Directory.GetDirectories(destFullName).Length > 0)
            {
                string[] directoies = Directory.GetDirectories(destFullName);

                foreach (var item in directoies)
                {
                    string itempath = item.Substring(item.LastIndexOf('\\') + 1);

                    PasteInFolder($"{directoryFullName}\\{destName}", itempath, item);
                }                
            }
        }

        private void Properties(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            object info = null;
            try
            {
                info = view.SelectedItem.State;
            }
            catch (ArgumentOutOfRangeException)
            {
                AccessDenied();
            }
            
            if (info is FileInfo file)
            {
                try
                {
                    Rendering.Properties(
                        countOfFiles,
                        $"Name: {file.Name}",
                        $"Parent directory: {file.Directory.FullName}",
                        $"Root directory: {file.Directory.Root.Name}",
                        $"Is read only: " + (file.IsReadOnly ? "True" : "False"),
                        $"Last read time: {File.GetLastAccessTime(file.FullName)}",
                        $"Last write time: {File.GetLastWriteTime(file.FullName)}",
                        $"Size: {file.Length.ToString()} bytes");
                }
                catch (UnauthorizedAccessException)
                {
                    AccessDenied();
                }
            }
            else if (info is DirectoryInfo directory)
            {
                try
                {
                    Rendering.Properties(
                        countOfFiles,
                        $"Name: {directory.Name}",
                        $"Root drectory: {directory.Root.FullName}",
                        $"Parent directory: {directory.Parent.FullName}",
                        $"Last read time: {Directory.GetLastAccessTime(directory.FullName)}",
                        $"Last write time: {Directory.GetLastWriteTime(directory.FullName)}",
                        $"Size: {SizeInBytes(directory.FullName).ToString()} bytes",
                        $"Files: {CountOfFiles(directory.FullName).ToString()}",
                        $"Folders: {directory.GetDirectories().Count().ToString()}");
                }
                catch (UnauthorizedAccessException)
                {
                    AccessDenied();
                }
            }            
        }

        private int CountOfFiles(string directory)
        {
            int a = Directory.GetFiles(directory, "*.*").Length;

            if (Directory.GetDirectories(directory).Length > 0)
            {
                string[] directoies = Directory.GetDirectories(directory);

                foreach (var item in directoies)
                {
                    a += CountOfFiles(item);
                }
            }

            return a;
        }

        private long SizeInBytes(string directory)
        {
            string[] a = Directory.GetFiles(directory, "*.*");

            long b = 0;
            foreach (string name in a)
            {
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }

            if (Directory.GetDirectories(directory).Length > 0)
            {
                string[] directoies = Directory.GetDirectories(directory);

                foreach (var item in directoies)
                {
                    b += SizeInBytes(item);
                }
            }

            return b;
        }

        private void Rename(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            var info = view.SelectedItem.State;

            if (info is FileInfo file)
            {
                this.fullPathOfFile = file.FullName;

                var newname = Rendering.Rename(countOfFiles, file.FullName);

                if (!string.IsNullOrEmpty(newname))
                {
                    try
                    {
                        string newpath = $"{file.Directory.FullName}\\{newname}{Path.GetExtension(file.FullName)}";
                        File.Move(this.fullPathOfFile, newpath);
                        listViews[selectedSystem].Items = GetItems(paths[selectedSystem]);
                        listViews[selectedSystem].Clean();
                    }
                    catch (IOException)
                    {
                        AccessDenied();
                    }
                }
            }
            else if (info is DirectoryInfo directory)
            {
                this.fullPathOfFile = directory.FullName;

                var newname = Rendering.Rename(countOfFiles, directory.FullName);

                if (!string.IsNullOrEmpty(newname))
                {
                    try
                    {
                        string newpath = $"{directory.Parent.FullName}{newname}";
                        Directory.Move(this.fullPathOfFile, newpath);
                        listViews[selectedSystem].Items = GetItems(paths[selectedSystem]);
                        listViews[selectedSystem].Clean();
                    }
                    catch (IOException)
                    {
                        AccessDenied();
                        this.fullPathOfFile = view.PrevSelectedItem;
                        listViews[selectedSystem].RenderPath(this.fullPathOfFile, countOfDisks);
                    }
                }
            }
        }
        
        private void CreateFolder(object sender, EventArgs e)
        {
            var view = (ListView)sender;
            var info = view.SelectedItem.State;

            string newname = null;

            if (info is FileInfo file)
            {
                this.fullPathOfFile = file.Directory.FullName;

                newname = Rendering.Rename(countOfFiles, file.Directory.FullName);
            }
            else if (info is DirectoryInfo directory)
            {
                this.fullPathOfFile = directory.Parent.FullName;

                newname = Rendering.Rename(countOfFiles, directory.FullName);
            }
            
            if (!string.IsNullOrEmpty(newname))
            {
                Directory.CreateDirectory($"{this.fullPathOfFile}\\{newname}");
            }
        }

        private void AccessDenied()
        {
            Rendering.Properties(countOfFiles, "Access denied");
        }
    }
}

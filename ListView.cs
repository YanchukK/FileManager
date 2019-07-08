using System;
using System.Collections.Generic;

namespace FileManager
{
    class ListView
    {
        private int prevSelectedIndex;
        private int selectedIndex;
        private bool wasPainted;
        private int scroll;
        public bool OnFocused = false;
        private readonly int x;
        private readonly int y;
        private readonly int height;

        public string Path { get; set; }
        public string PrevSelectedItem { get; set; } = "C:\\";
        public ListViewItem SelectedItem => Items[selectedIndex];
        public List<int> ColumnsWidth { get; set; }

        private List<ListViewItem> ListListViewItem;

        public List<ListViewItem> Items
        {
            get => ListListViewItem;
            set
            {
                scroll = 0;
                ListListViewItem = value;
            }
        }

        public ListView(int x, int y, int height)
        {
            this.x = x;
            this.y = y;
            this.height = height;
        }

        public void Clean()
        {
            selectedIndex = prevSelectedIndex = 0;

            wasPainted = false;
            for (int i = 0; i < Math.Min(height, Items.Count); i++) // min
            {
                Console.CursorLeft = x;
                Console.CursorTop = i + y;
                Items[i].Clean(ColumnsWidth, i, x, y);
            }
        }

        public void RenderPath(string path, int countOfDisks)
        {
            int maxLength = (Console.WindowWidth / countOfDisks) - 2;
            Console.CursorLeft = x;
            Console.CursorTop = 1;
            Console.Write(new string(' ', maxLength));
            Console.CursorLeft = x;
            Console.CursorTop = 1;
            if (path.Length < maxLength)
                Console.Write(path);
            else
            {
                path = path.Substring(0, maxLength - 5) + "[...]";
                Console.Write(path);
            }
        }

        public void Render()
        {
            var foreground = ConsoleColor.DarkGray;

            if (OnFocused)
            {
                foreground = ConsoleColor.White;
            }

            var background = Console.BackgroundColor;

            for (int i = 0; i < Math.Min(height, Items.Count); i++)
            {
                int elementIndex = i + scroll;

                if (wasPainted)
                {
                    if (elementIndex != selectedIndex && elementIndex != prevSelectedIndex)
                        continue;
                }
                               
                if (elementIndex == selectedIndex)
                {                    
                    Console.ForegroundColor = ConsoleColor.Black;
                    if(OnFocused)
                        Console.BackgroundColor = ConsoleColor.White;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                }

                Console.CursorLeft = x;
                Console.CursorTop = i;
                
                Items[elementIndex].Render(ColumnsWidth, i, x, y);

                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
            }

            wasPainted = true;
        }

        public void Update(ConsoleKeyInfo key)
        {
            prevSelectedIndex = selectedIndex;

            if (key.Key == ConsoleKey.DownArrow && selectedIndex + 1 < Items.Count)
                selectedIndex++;
            else if (key.Key == ConsoleKey.UpArrow && selectedIndex - 1 >= 0)
                selectedIndex--;
            else if (key.Key == ConsoleKey.Enter)
            {
                try
                {
                    Selected(this, EventArgs.Empty);
                    scroll = 0;
                }
                catch (UnauthorizedAccessException)
                {  }
            }
            else if (key.Key == ConsoleKey.F1)
            {
                Copy(this, EventArgs.Empty);
            }
            else if (key.Key == ConsoleKey.F2)
            {
                Cut(this, EventArgs.Empty);
            }
            else if (key.Key == ConsoleKey.F3)
            {
                Paste(this, EventArgs.Empty);
            }
            else if(key.Key == ConsoleKey.F4)
            {
                Parent(this, EventArgs.Empty);
            }
            else if(key.Key == ConsoleKey.F6)
            {
                Properties(this, EventArgs.Empty);
            }
            else if(key.Key == ConsoleKey.F7)
            {
                Rename(this, EventArgs.Empty);
            }
            else if(key.Key == ConsoleKey.F9)
            {
                CreateFolder(this, EventArgs.Empty);
            }
            
            if (selectedIndex >= height + scroll)
            {
                scroll++;
                wasPainted = false;
            }
            else if (selectedIndex < scroll)
            {
                scroll--;
                wasPainted = false;
            }
        }


        public event EventHandler Selected;
        public event EventHandler Parent;
        public event EventHandler Copy;
        public event EventHandler Paste;
        public event EventHandler Cut;
        public event EventHandler Properties;
        public event EventHandler Rename;
        public event EventHandler CreateFolder;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class ListOfDisks
    {
        private int prevSelectedIndex;
        private int selectedIndex;
        private bool wasPainted;
        private int scroll;
        private readonly int x = 1;
        private readonly int height;
        public string SelectedItem => paths[selectedIndex];

        private readonly string[] paths;

        public string[] GetItems
        {
            get => paths;
        }

        public ListOfDisks(int height, params string[] paths)
        {
            this.paths = paths;
            this.height = height + 4;
        }

        internal void Render()
        {
            Console.CursorLeft = x;
            Console.CursorTop = height;

            var foreground = Console.ForegroundColor;
            var background = Console.BackgroundColor;

            for (int i = 0; i < paths.Length; i++)
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
                    Console.BackgroundColor = ConsoleColor.White;
                }

                Console.CursorLeft = x;
                Console.CursorTop = height + i;

                Console.Write(paths[elementIndex]);

                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;
            }

            wasPainted = true;
        }

        public void Update(ConsoleKeyInfo key)
        {
            prevSelectedIndex = selectedIndex;

            if (key.Key == ConsoleKey.DownArrow && selectedIndex + 1 < paths.Length)
                selectedIndex++;
            else if (key.Key == ConsoleKey.UpArrow && selectedIndex - 1 >= 0)
                selectedIndex--;
            else if(key.Key == ConsoleKey.Enter)
            {
                Selected(this, EventArgs.Empty);
                scroll = 0;
            }

            if (selectedIndex >= paths.Length + scroll)
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
               
        internal void Clean(int y, int columnsLength)
        {
            y += 3;
            Rendering.Clean(y, columnsLength);
        }

        public event EventHandler Selected;
    }
}

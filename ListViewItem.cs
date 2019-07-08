using System;
using System.Collections.Generic;
using System.Linq;

namespace FileManager
{
    public class ListViewItem
    {
        private readonly string[] columns;
        public object State { get; }

        public ListViewItem(object state, params string[] columns)
        {
            this.State = state;
            this.columns = columns;
        }

        internal void Render(List<int> columnsWidth, int elementIndex, int listViewX, int listViewY)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                Console.CursorTop = elementIndex + listViewY;
                Console.CursorLeft = listViewX + columnsWidth.Take(i).Sum();
                Console.Write(GetStringWithLength(columns[i], columnsWidth[i]));
            }
        }

        private string GetStringWithLength(string v, int maxLength)
        {
            if (v.Length < maxLength)
                return v.PadRight(maxLength, ' ');
            else
                return v.Substring(0, maxLength - 5) + "[...]";
        }

        internal void Clean(List<int> columnsWidth, int i, int x, int y)
        {
            Console.CursorTop = i + y;
            Console.CursorLeft = x;
            Console.Write(new string(' ', columnsWidth.Sum()));
        }
    }
}
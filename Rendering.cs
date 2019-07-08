using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    static class Rendering
    {
        private const char horiz = (char)9552;
        private const char vert = (char)9553;
        private const char topleft = (char)9556;
        private const char bottomleft = (char)9562;
        private const char topright = (char)9559;
        private const char bottomright = (char)9565;

        public static void InitialRender(int countOfDisks, int countOfFiles)
        {
            for (int i = 0; i < countOfDisks; i++)
            {
                Console.Write(
                    topleft +
                    new string(horiz, (Console.WindowWidth - (countOfDisks + 1)) / countOfDisks) +
                    topright);
            }
            
            for (int i = 0; i < countOfDisks; i++)
            {
                for (int j = 0; j <= countOfFiles; j++)
                {
                    Console.Write(
                        vert +
                        new string(' ', (Console.WindowWidth - (countOfDisks + 1)) / countOfDisks) +
                        vert);
                }
            }

            for (int i = 0; i < countOfDisks; i++)
            {
                Console.Write(
                    bottomleft +
                    new string(horiz, (Console.WindowWidth - (countOfDisks + 1)) / countOfDisks) +
                    bottomright);
            }

            Console.CursorTop = Console.WindowHeight - 2;
            Console.CursorLeft = 0;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Yellow;
            const string menu = "F1 - copy | F2 - cut | F3 - paste | F4 - parent directory | " +
                "F5 - list of disks | F6 - properties | F7 - rename | F9 - new folder";
            Console.Write(menu.PadRight(Console.WindowWidth, ' '));
            Console.ResetColor();
        }

        private static void Ok(int x, int y, int length, int columnsLength)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
            Console.Write(topleft + new string(horiz, length) + topright);

            y++;
            Console.CursorLeft = x;
            Console.CursorTop = y;

            for (int i = 0; i < columnsLength; i++)
            {
                Console.Write(vert + new string(' ', length) + vert);
                y++;
            }

            Console.Write(vert);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write($"{new string(' ', (length - "Ok".Length) / 2)}Ok{new string(' ', (length - "Ok".Length) / 2)}");
            Console.ResetColor();
            Console.Write(vert);
            y++;

            Console.CursorLeft = x;
            Console.CursorTop = y;
            Console.Write(bottomleft + new string(horiz, length) + bottomright);
        }

        public static void Properties(int y, params string[] columns)
        {
            int x = 0;
            y += 3;
            int length = Console.WindowWidth - 2;
            Ok(x, y, length, columns.Length);

            x++;
            for (int i = 0; i < columns.Length; i++)
            {
                Console.CursorLeft = x;
                Console.CursorTop = i + 1 + y;
                Console.Write(columns[i]);
            }

            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                Clean(y, columns.Length);
            }
        }

        public static string Rename(int y, string path)
        {
            int x = 0;
            y += 3;

            var length = Console.WindowWidth - 2;
            Ok(x, y, length, 2);

            x++;
            Console.CursorLeft = x;
            Console.CursorTop = y + 1;
            Console.WriteLine(path);

            string newNameString = "New name: ";
            Console.CursorLeft = x;
            Console.CursorTop = y + 2;
            Console.Write(newNameString);

            Console.CursorLeft = x + newNameString.Length;
            Console.CursorTop = y + 2;
            string newname = Console.ReadLine();

            if (string.IsNullOrEmpty(newname))
            {
                Clean(y, 3);
                x = 0;
                Ok(x, y, length, 2);

                x++;
                y++;
                Console.CursorLeft = x;
                Console.CursorTop = y;
                Console.Write("Name cannot be empty");

                y--; ;
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    Clean(y, 3);
                }
            }
            else
            {
                Clean(y, 3);
            }

            return newname;
        }

        public static void Clean(int y, int columnsLength)
        {
            int x = 0;

            var length = Console.WindowWidth;

            for (int i = 0; i < columnsLength + 3; i++) // 3 - one for top border,
                                                         //    one for bottom border,
                                                         //    one for "Ok"
            {
                Console.CursorTop = i + y;
                Console.CursorLeft = x;
                Console.WriteLine(new string(' ', length));
            }
        }
    }
}

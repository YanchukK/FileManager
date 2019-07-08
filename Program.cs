using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            int countOfFiles = 20; // max 24

            Console.CursorVisible = false;
            Console.BufferHeight = Console.WindowHeight = 20 + countOfFiles;
            Console.BufferWidth = Console.WindowWidth = 130;

            FileSystem fileSystem = new FileSystem(countOfFiles, "C:\\", "D:\\", "E:\\");

            fileSystem.Render();
        }
    }
} 

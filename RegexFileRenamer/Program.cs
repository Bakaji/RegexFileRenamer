using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegexFileRenamer
{
    internal class Program
    {
        public static string Path = null, SelectedRegex = null, NewValue = null;

        private static void Main(string[] args)
        {
            args = new string[] { "--path", @"D:\Mp3\Music\Anime\Gintama", "--regex", @"\d{2,}", "--replace", "newv" };
            string path = LookFor("--path", args);
            string regex = LookFor("--regex", args);
            string newv = LookFor("--replace", args);
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path)) Path = path;
            else PrintError("path is invalide " + path);
            if (!string.IsNullOrWhiteSpace(regex) && IsValidRegex(regex)) SelectedRegex = regex;
            else PrintError("regex is invalide " + regex);
            NewValue = newv ?? "";
            while (true)
            {
                int i = PrintMainScreen();
                switch (i)
                {
                    case 1:
                        {
                            ChangeRegex();
                            break;
                        }
                    case 2:
                        {
                            ChangePath();
                            break;
                        }
                    case 3:
                        {
                            ChangeNewValue();
                            break;
                        }
                    case 4:
                        {
                            Console.Clear();
                            ShowMatches();
                            break;
                        }
                    case 5:
                        {
                            Console.Clear();
                            break;
                        }

                    case 6:
                        {
                            ExitApp();
                            break;
                        }

                    default:
                        break;
                }
            }
        }

        private static string LookFor(string what, string[] args)
        {
            string value = null;
            if (args.Length > 0)
            {
                int index = -1;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == what)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1 && index < args.Length - 1)
                {
                    string path = args[index + 1];
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        value = path;
                    }
                    else
                    {
                        Console.Error.WriteLine("Empty value");
                    }
                }
            }
            return value;
        }

        private static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        private static void PrintError(string q)
        {
            PrintWithColor(q, ConsoleColor.Red);
        }

        private static int PrintMainScreen()
        {
            Console.Clear();
            Console.WriteLine("**********************************************************");
            Console.WriteLine("**********************************************************");
            Console.WriteLine();

            if (!string.IsNullOrWhiteSpace(SelectedRegex))
            {
                PrintWithColor(" Selected Regex  :  ", ConsoleColor.Red);
                PrintWithColor('"' + SelectedRegex + '"', ConsoleColor.Cyan);
                Console.WriteLine();
            }
            else
            {
                PrintWithColor("Selected Regex not set", ConsoleColor.Red);
                Console.WriteLine();
            }
            Console.WriteLine();
            if (!string.IsNullOrWhiteSpace(NewValue))
            {
                PrintWithColor(" Selected New Value  :  ", ConsoleColor.Red);
                PrintWithColor('"' + NewValue + '"', ConsoleColor.Cyan);
                Console.WriteLine();
            }
            else
            {
                PrintWithColor("Selected New Value not set", ConsoleColor.Red);
                Console.WriteLine();
            }
            Console.WriteLine();
            if (!string.IsNullOrWhiteSpace(Path))
            {
                PrintWithColor(" Selected Path   :  ", ConsoleColor.Red);
                PrintWithColor('"' + Path + '"', ConsoleColor.Cyan);
                Console.WriteLine();
            }
            else
            {
                PrintWithColor("Selected Path not set", ConsoleColor.Red);
                Console.WriteLine();
            }
            Console.WriteLine();
            PrintWithColor("1) -", ConsoleColor.Cyan); Console.WriteLine(" Change regex");
            PrintWithColor("2) -", ConsoleColor.Cyan); Console.WriteLine(" Change path ");
            PrintWithColor("3) -", ConsoleColor.Cyan); Console.WriteLine(" Change value");
            PrintWithColor("4) -", ConsoleColor.Cyan); Console.WriteLine(" Show matches and submit");
            PrintWithColor("5) -", ConsoleColor.Cyan); Console.WriteLine(" Clear screen");
            PrintWithColor("6) -", ConsoleColor.Cyan); Console.WriteLine(" Exit");
            Console.WriteLine();
            Console.WriteLine("**********************************************************");
            Console.WriteLine("**********************************************************");
            Console.Write("choose from 1 to 6\t:\t");
            string s = Console.ReadLine();
            int i = -1;
            int.TryParse(s, out i);
            return i;
        }

        private static void ChangeRegex()
        {
        a:
            Console.Clear();
            Console.Write("Type new regex\t");
            string s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s) && IsValidRegex(s)) SelectedRegex = s;
            else
            {
                PrintError("regex is invalide " + s);
                goto a;
            }
        }

        private static void ChangePath()
        {
        a:
            Console.Clear();
            Console.Write("Type new path\t");
            string s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s) && Directory.Exists(s)) Path = s;
            else
            {
                PrintError("path is invalide " + s);
                goto a;
            }
        }

        private static void ChangeNewValue()
        {
        a:
            Console.Clear();
            Console.Write("Type new value\t");
            string s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) NewValue = s;
            else
            {
                PrintError("path is invalide " + s);
                goto a;
            }
        }

        private static void ShowMatches()
        {
            string[] list = Directory.GetFiles(Path);
            var sr = new Regex(SelectedRegex);
            var files = new List<FileToChange>();
            foreach (string f in list)
            {
                FileToChange file = new FileToChange(f);
                files.Add(file);
                sr.PrintResult(file);
            }
            Console.Write("\n\n   - change matches with " + NewValue + " ?[y/n]");
            string s = Console.ReadLine().Trim().ToLower();
            if (s == "y")
                foreach (FileToChange item in files)
                    item.SubmitChanges(sr, NewValue);
        }

        private static void ExitApp()
        {
            Environment.Exit(0);
        }

        public static void PrintWithColor(string v, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Error.Write(v);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    internal static class StaticMethod
    {
        public static void PrintResult(this Regex r, FileToChange s)
        {
            int i = 0, j = 0;
            Console.Write(s.base_Path);
            MatchCollection coll = r.Matches(s.OldName);
            List<string> whites = r.Split(s.OldName).ToList();
            if (coll.Count > 0 && !string.IsNullOrWhiteSpace(s.OldName.Substring(0, coll[0].Index)))
            {
                Console.Write(whites[0]);
                j++;
            }
            if (string.IsNullOrEmpty(whites[0])) whites.RemoveAt(0);
            while (i < coll.Count || j < whites.Count)
            {
                if (i < coll.Count) Program.PrintWithColor(coll[i].Value, ConsoleColor.Red);
                if (j < whites.Count) Console.Write(whites[j]);

                i++; j++;
            }
            Console.Write(s.Extension);
            Console.WriteLine();
        }
    }

    internal class FileToChange
    {
        public readonly string base_Path, OldName, Extension;

        public FileToChange(string full_path)
        {
            OldName = Path.GetFileNameWithoutExtension(full_path);
            Extension = Path.GetExtension(full_path);
            base_Path = full_path.Substring(0, full_path.Length - OldName.Length - Extension.Length);
        }

        public string NewName(Regex regex, string value)
        {
            return base_Path + regex.Replace(OldName, value) + Extension;
        }

        public void SubmitChanges(Regex regex, string value)
        {
            try
            {
                string old = base_Path + OldName + Extension;
                File.Move(old, base_Path + NewName(regex, value) + Extension);
            }
            catch
            {
                Program.PrintWithColor("Something went wrong verify permissions and path existance\n", ConsoleColor.Red);
            }
        }
    }
}
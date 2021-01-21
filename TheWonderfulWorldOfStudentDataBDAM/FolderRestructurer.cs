using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheWonderfulWorldOfStudentDataBDAM
{
    public class FolderRestructurer : IExecutor
    {
        private string _workpath;

        public FolderRestructurer(string workpath)
        {
            _workpath = workpath;

            if (string.IsNullOrWhiteSpace(workpath))
            {
                _workpath = Directory.GetCurrentDirectory();
            }
        }

        public void Run()
        {
            Console.WriteLine("Welcome to the import data checker!");
            var folder = new DirectoryInfo(_workpath);
            var successFolder = folder.Parent.CreateSubdirectory("Success");
            Console.WriteLine("We will now process each folder to check if they contain the correct file!");
            var requiredFiles = new string[] { "amusement.csv", "ov.csv", "stad.csv", "twitter.csv", "weer.csv" };
            DirectoryInfo[] directories = folder.GetDirectories();
            var dirLength = directories.Length - 1;
            List<DirectoryInfo> directoriesFailed = new List<DirectoryInfo>();

            for (int i = 0; i <= dirLength; i++)
            {
                DirectoryInfo item = directories[i];
                if (requiredFiles.All(x => item.GetFiles().Any(c => x == c.Name.ToLower())))
                {
                    if (!Directory.Exists($@"{successFolder}\{item.Name}"))
                        Directory.CreateDirectory($@"{successFolder}\{item.Name}");

                    foreach (var reqFile in requiredFiles)
                    {
                        var f = item.GetFiles().Where(c => c.Name.ToLower() == reqFile).FirstOrDefault();
                        f.MoveTo($@"{successFolder}\{item.Name}\{reqFile}");
                    }

                    Console.WriteLine($"[{i}/{dirLength}]: {item.Name} Has been found correctly.");
                }
                else
                {
                    directoriesFailed.Add(item);
                    Console.WriteLine($"[{i}/{dirLength}]: {item.Name} Has been found incorrectly and has been moved to the next round!.");
                }
            }

            Console.WriteLine("Next round! begin!");
            Console.WriteLine("We will browse the subfolders too to find the correct files!");
            dirLength = directoriesFailed.Count - 1;
            List<DirectoryInfo> directoriesFailedTwice = new List<DirectoryInfo>();

            for (int i = 0; i <= dirLength; i++)
            {
                DirectoryInfo item = directoriesFailed[i];
                var allFiles = item.GetFiles().ToList();
                allFiles.AddRange(recursiveFolderLoop(item));

                if (requiredFiles.All(x => allFiles.Any(c => x == c.Name.ToLower())))
                {
                    if (!Directory.Exists($@"{successFolder}\{item.Name}"))
                        Directory.CreateDirectory($@"{successFolder}\{item.Name}");

                    foreach (var reqFile in requiredFiles)
                    {
                        var f = allFiles.Where(c => c.Name.ToLower() == reqFile).FirstOrDefault();
                        f.MoveTo($@"{successFolder}\{item.Name}\{reqFile}");
                    }

                    Console.WriteLine($"[{i}/{dirLength}]: {item.Name} Has been found correctly.");
                }
                else
                {
                    directoriesFailedTwice.Add(item);
                    Console.WriteLine($"[{i}/{dirLength}]: {item.Name} Has been found incorrectly and has been moved to the next round!.");
                }
            }

            Console.WriteLine("Next round! begin!");
            Console.WriteLine("We will browse the subfolders too to find the correct files AND check if it's part of the name!");
            dirLength = directoriesFailedTwice.Count - 1;
            List<DirectoryInfo> directoriesFailedThrice = new List<DirectoryInfo>();
            requiredFiles = requiredFiles.Select(c => c.Substring(0, c.IndexOf('.'))).ToArray();
            for (int i = 0; i <= dirLength; i++)
            {
                DirectoryInfo item = directoriesFailed[i];
                var allFiles = item.GetFiles().ToList();
                allFiles.AddRange(recursiveFolderLoop(item));

                if (requiredFiles.All(x => allFiles.Any(c => c.Name.ToLower().Contains(x))))
                {
                    if (!Directory.Exists($@"{successFolder}\{item.Name}"))
                        Directory.CreateDirectory($@"{successFolder}\{item.Name}");

                    foreach (var reqFile in requiredFiles)
                    {
                        var f = allFiles.Where(c => c.Name.ToLower().Contains(reqFile)).FirstOrDefault();
                        f.MoveTo($@"{successFolder}\{item.Name}\{reqFile}.csv");
                    }

                    Console.WriteLine($"[{i}/{dirLength}]: {item.Name} Has been found correctly.");
                }
                else
                {
                    directoriesFailedThrice.Add(item);
                    Console.WriteLine($"[{i}/{dirLength}]: {item.Name} Has been found incorrectly and has been moved to the next round!.");
                }
            }

            Console.WriteLine("Well... that was a rollercoaster.");
            Console.WriteLine($"Here are the {directoriesFailedThrice.Count - 1} failing failing the three tests...");
            var resultInspect = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<DirectoryInfo>()
                        .Title("Please select the one you want to inspect.")
                        .NotRequired()
                        .PageSize(10)
                        .AddChoices(directoriesFailedThrice.ToArray())
                        .UseConverter(c => c.Name));

            foreach (var item in resultInspect)
            {
                Console.WriteLine($"{item.Name}:");
                var allFiles = recursiveFolderLoop(item);

                foreach (var file in allFiles)
                {
                    Console.WriteLine(file.Name);
                }
            }
        }


        public static List<FileInfo> recursiveFolderLoop(DirectoryInfo directory)
        {
            var resultSet = new List<FileInfo>();
            resultSet.AddRange(directory.GetFiles());
            if (directory.GetDirectories().Length > 0)
            {
                foreach (var item in directory.GetDirectories())
                {
                    resultSet.AddRange(recursiveFolderLoop(item));
                }
            }

            return resultSet;
        }
    }
}

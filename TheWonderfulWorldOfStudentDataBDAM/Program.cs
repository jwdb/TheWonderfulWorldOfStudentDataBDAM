using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheWonderfulWorldOfStudentDataBDAM
{
    class Program
    {
        public static IReadOnlyDictionary<string, Func<IExecutor>> methods;
        static void Main(string[] args)
        {
            var WorkPath = "";
            if (args.Length > 0 && Directory.Exists(args[0]))
            {
                WorkPath = args[0];
                Console.WriteLine($"Current WorkPath: {WorkPath}");
            }

            methods = new Dictionary<string, Func<IExecutor>>
            {
                {nameof(FolderRestructurer), () => new FolderRestructurer(WorkPath) },
                {nameof(SanitySanitizer), () => new SanitySanitizer(WorkPath) }
            };

            var resultInspect = AnsiConsole.Prompt(
                  new MultiSelectionPrompt<KeyValuePair<string, Func<IExecutor>>>()
                      .Title("Please select the Method you want to run")
                      .PageSize(10)
                      .AddChoices(methods.ToArray())
                      .UseConverter(c => c.Key));

            foreach (var item in resultInspect)
            {
                Console.WriteLine($"{item.Key}");
                item.Value().Run();
            }
        }

    }
}

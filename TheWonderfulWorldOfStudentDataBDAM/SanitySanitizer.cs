using FileHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TheWonderfulWorldOfStudentDataBDAM
{
    public class SanitySanitizer : IExecutor
    {
        private string _workpath;

        public SanitySanitizer(string workpath)
        {
            _workpath = workpath;
            if (string.IsNullOrWhiteSpace(workpath))
            {
                _workpath = Directory.GetCurrentDirectory();
            }
        }

        public void Run()
        {
            Console.WriteLine("Welcome to the Sane Sanity creator!");

            var folder = new DirectoryInfo(_workpath);
            var requiredFiles = new string[] { "amusement.csv", "ov.csv", "stad.csv", "twitter.csv", "weer.csv" };
            DirectoryInfo[] directories = folder.GetDirectories();
            var dirLength = directories.Length - 1;
            var cities = new List<Models.CompleteCity>();
            for (int i = 0; i <= dirLength; i++)
            {
                var foldername = directories[i].Name;
                var files = directories[i].GetFiles();

                var completeCity = new Models.CompleteCity();

                var Groepnr = foldername.Split('_')[0];
                var StadNaam = foldername.Split('_')[1];
                var stadId = $"{Groepnr}_{StadNaam}";
                foreach (var item in files)
                {
                    if (!item.Name.EndsWith(".csv"))
                        break;
                    
                    var lines = File.ReadAllLines(item.FullName);
                    var linesList = lines.ToList();
                    var indexofHeader = linesList.FindIndex(c => c.ToLower().Contains("stad_naam") || c.ToLower().Contains("id"));
                    if (indexofHeader < 0)
                        continue;

                    linesList.RemoveRange(0, indexofHeader);
                    lines = linesList.ToArray();
                    var isComma = lines[lines.Length > 1 ? 1 : 0].Count(c => c == ',') > lines[lines.Length > 1 ? 1 : 0].Count(c => c == ';');

                    var parser = new CsvParser(isComma ? ',' : ';');
                    for (int li = 0; li < lines.Length; li++)
                    {
                        var line = lines[li];

                        // Is is comma or dotcomma
                        var lineIsComma = line.Count(c => c == ',') > line.Count(c => c == ';');

                        if (!lineIsComma)
                        {
                            if (line.Contains(','))
                                line = line.Replace(',', '.');

                            line = line.Replace(';', ',');
                        }

                        lines[li] = line;
                    }

                    File.WriteAllLines(item.FullName, lines);

                    try
                    {
                        switch (item.Name)
                        {
                            case "amusement.csv":
                                var xAmu = parser.Parse<Models.Amusement>(item.FullName);
                                var AmusementList = xAmu.ToList();
                                for (int aI = 0; aI < AmusementList.Count; aI++)
                                {
                                    var aItem = AmusementList[aI];
                                    aItem.id = $"{Groepnr}{aI.ToString().PadLeft(4, '0')}";
                                    aItem.STAD_NAAM = stadId;
                                    AmusementList[aI] = aItem;
                                }
                                completeCity.amusement = AmusementList;
                                break;
                            case "ov.csv":
                                var xOv = parser.Parse<Models.OV>(item.FullName);
                                var OvList = xOv.ToList();
                                for (int aI = 0; aI < OvList.Count; aI++)
                                {
                                    var aItem = OvList[aI];
                                    aItem.id = $"{Groepnr}{aI.ToString().PadLeft(4, '0')}";
                                    aItem.STAD_naam = stadId;
                                    OvList[aI] = aItem;
                                }
                                completeCity.ov = OvList;

                                break;
                            case "stad.csv":
                                var xStad = parser.Parse<Models.Stad>(item.FullName);
                                var StadList = xStad.ToList();
                                for (int aI = 0; aI < StadList.Count; aI++)
                                {
                                    var aItem = StadList[aI];
                                    aItem.naam = stadId;
                                    StadList[aI] = aItem;
                                }
                                completeCity.stad = StadList;

                                break;
                            case "twitter.csv":
                                try
                                {
                                    var xTwitter = parser.Parse<Models.Twitter>(item.FullName);
                                    var TwitterList = xTwitter.ToList();
                                    for (int aI = 0; aI < TwitterList.Count; aI++)
                                    {
                                        var aItem = TwitterList[aI];
                                        aItem.id = $"{Groepnr}{aI.ToString().PadLeft(4, '0')}";
                                        aItem.STAD_naam = stadId;
                                        TwitterList[aI] = aItem;
                                    }
                                    completeCity.twitter = TwitterList;

                                }
                                catch (Exception e)
                                {
                                }
                                break;
                            case "weer.csv":
                                var xWeer = parser.Parse<Models.Weer>(item.FullName);
                                var WeerList = xWeer.ToList();
                                for (int aI = 0; aI < WeerList.Count; aI++)
                                {
                                    var aItem = WeerList[aI];
                                    aItem.ID = $"{Groepnr}{aI.ToString().PadLeft(4, '0')}";
                                    aItem.STAD_naam = stadId;
                                    WeerList[aI] = aItem;
                                }
                                completeCity.weer = WeerList;

                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        
                    }
                }

                cities.Add(completeCity);
            }

            List<string> statements = new List<string>();
            foreach (var item in cities)
            {
                ObjectToSQLInsert objectToSQL = new ObjectToSQLInsert();

                File.AppendAllLines("stad.sql", item.stad?.Select(objectToSQL.Process) ?? new string[] { });
                File.AppendAllLines("amusement.sql", item.amusement?.Select(objectToSQL.Process) ?? new string[] { });
                File.AppendAllLines("ov.sql", item.ov?.Select(objectToSQL.Process) ?? new string[] { });
                File.AppendAllLines("twitter.sql", item.twitter?.Select(objectToSQL.Process) ?? new string[] { });
                File.AppendAllLines("weer.sql", item.weer?.Select(objectToSQL.Process) ?? new string[] { });
            }
            //File.WriteAllText("out.sql", string.Join(Environment.NewLine, statements));
        }
    }
}

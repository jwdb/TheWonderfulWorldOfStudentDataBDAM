using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;

namespace TheWonderfulWorldOfStudentDataBDAM
{
    public class CsvParser
    {
        public char FieldSeperator { get; set; }
        public string LineSeperator { get; set; }
        public char BlockFieldSeperator { get; set; }

        public CsvParser(char Seperator)
        {
            this.FieldSeperator = Seperator;
            LineSeperator = Environment.NewLine;
            BlockFieldSeperator = '\"';
        }

        public T[] Parse<T>(string inputFile) where T : class, new()
        {
            var returnList = new List<T>();
            var parsed = Parse(inputFile);

            var headerRow = parsed[0];
            var TypeProperties = typeof(T).GetProperties();
            var PropertyMapping = new Dictionary<PropertyInfo, int>();

            foreach (var item in TypeProperties)
            {
                // Perfect match first
                var hId = headerRow.FirstOrDefault(c => c.Value == item.Name);

                if (hId.Value == null)
                    hId = headerRow.FirstOrDefault(c => c.Value.ToUpperInvariant() == item.Name.ToUpperInvariant());

                if (hId.Value == null)
                    hId = headerRow.FirstOrDefault(c => item.Name.ToUpperInvariant().Contains(c.Value.ToUpperInvariant()));
                if (hId.Value == null)
                    hId = headerRow.FirstOrDefault(c => RemoveSpecialCharacters(item.Name).ToUpperInvariant().Contains(RemoveSpecialCharacters(c.Value).ToUpperInvariant()));

                if (hId.Value != null)
                    PropertyMapping.Add(item, hId.Key);
            }

            foreach (var item in parsed.Skip(1))
            {
                var newObj = new T();

                foreach (var property in PropertyMapping)
                {
                    if (item.ContainsKey(property.Value))
                        property.Key.SetValue(newObj, item[property.Value]);
                }

                returnList.Add(newObj);
            }

            return returnList.ToArray();
        }

        public Dictionary<int, string>[] Parse(string inputFile)
        {
            var returnList = new List<Dictionary<int, string>>();

            try
            {
                using var file = File.OpenRead(inputFile);
                StringWriter sWriter = new StringWriter();
                int currentByte = -1;
                var iFieldSeperator = (int)FieldSeperator;
                var iBlockFieldSeperator = (int)BlockFieldSeperator;
                var iStartLineSeperator = (int)LineSeperator[0];
                var currentRow = new Dictionary<int, string>();
                do
                {
                    currentByte = file.ReadByte();
                    if (currentByte == iBlockFieldSeperator)
                    {
                        do
                        {
                            currentByte = file.ReadByte();
                            if (currentByte == -1)
                                throw new Exception($"Not ending block character in: {inputFile} at Line: {returnList.Count}");
                            sWriter.Write((char)currentByte);
                        } while (currentByte != iBlockFieldSeperator);
                    }
                    else if (
                    currentByte == iStartLineSeperator
                    && (LineSeperator.Length == 1
                    || file.ReadByte() == (char)LineSeperator[1]))
                    {
                        currentRow.Add(currentRow.Count, sWriter.ToString());
                        returnList.Add(currentRow);
                        currentRow = new Dictionary<int, string>();
                        sWriter = new StringWriter();

                    }
                    else if (currentByte == iFieldSeperator)
                    {
                        currentRow.Add(currentRow.Count, sWriter.ToString());
                        sWriter = new StringWriter();
                    }
                    else
                    {
                        sWriter.Write((char)currentByte);
                    }

                } while (currentByte != -1);

            }
            catch (Exception)
            {

                throw;
            }

            return returnList.ToArray();
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9')
                    || (str[i] >= 'A' && str[i] <= 'z'
                        || (str[i] == '.' || str[i] == '_')))
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheWonderfulWorldOfStudentDataBDAM
{
    public class ObjectToSQLInsert
    {
        public ObjectToSQLInsert()
        {

        }

        public string Process<T>(T obj)
        {
            var type = typeof(T);
            var Properties = type.GetProperties();
            var PropertyValues = Properties.Select(prop => "\'" + prop.GetValue(obj)?.ToString()?.Replace('\'',' ').Replace(Environment.NewLine, "") + "\'");
            var sql = $"INSERT INTO {type.Name} ({string.Join(',', Properties.Select(c => c.Name))}) VALUES ({string.Join(',', PropertyValues)});";

            return sql;
        }
    }
}

using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheWonderfulWorldOfStudentDataBDAM.Models
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class Twitter
    {
        public string id { get; set; }
        [FieldQuoted]
        public string tekst { get; set; }
        public string accountnaam { get; set; }
        public string timestamp { get; set; }
        public string STAD_naam { get; set; }
    }
}

using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheWonderfulWorldOfStudentDataBDAM.Models
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class OV
    {
        public string id { get; set; }
        public string aantalPerUur { get; set; }
        public string reizigersPerUur { get; set; }
        public string soortOV { get; set; }
        public string STAD_naam { get; set; }
    }
}

using FileHelpers;

namespace TheWonderfulWorldOfStudentDataBDAM.Models
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class Amusement
    {
        public string id { get; set; }
        [FieldQuoted]
        public string soort { get; set; }
        public string aantalPerSoort { get; set; }
        public string STAD_NAAM { get; set; }
    }
}

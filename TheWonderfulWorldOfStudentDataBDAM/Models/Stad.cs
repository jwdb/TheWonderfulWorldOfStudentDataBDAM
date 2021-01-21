using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheWonderfulWorldOfStudentDataBDAM.Models
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class Stad
    {
        public string naam { get; set; }
        public string aantalInwoners { get; set; }
        public string aantalStudenten { get; set; }
        public string veiligheidsGevoel { get; set; }
        public string gemiddeldeHuurprijs { get; set; }
        public string totaalAantalMisdrijven { get; set; }
        public string oppervlakte { get; set; }
    }
}

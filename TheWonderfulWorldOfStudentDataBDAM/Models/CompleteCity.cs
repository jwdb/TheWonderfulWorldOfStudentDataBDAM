using System.Collections.Generic;
using System.Linq;

namespace TheWonderfulWorldOfStudentDataBDAM.Models
{
    public class CompleteCity
    {
        public List<Amusement> amusement { get; set; }
        public List<OV> ov { get; set; }
        public List<Stad> stad { get; set; }
        public List<Twitter> twitter { get; set; }
        public List<Weer> weer { get; set; }

        public override string ToString()
        {
            return stad?.FirstOrDefault()?.naam ?? base.ToString();
        }
    }
}

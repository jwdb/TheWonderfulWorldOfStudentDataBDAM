using FileHelpers;
using System;

namespace TheWonderfulWorldOfStudentDataBDAM.Models
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class Weer
    {
        private string neerslagInMillimeter;
        private string temperatuur;
        private string wINDKRACHT;

        public string ID { get; set; }
        public string TIMESTAMP { get; set; }
        public string NeerslagInMillimeter
        {
            get
            {
                decimal.TryParse(neerslagInMillimeter, out var res);
                decimal fraction = res - Math.Floor(res);

                if (fraction != 0 && res < 10)
                {
                    res *= 10;
                    return ((int)Math.Round(res)).ToString();
                }

                return string.IsNullOrWhiteSpace(neerslagInMillimeter) ? "0" : neerslagInMillimeter;
            }
            set => neerslagInMillimeter = value;
        }
        public string Temperatuur
        {
            get
            {
                decimal.TryParse(temperatuur, out var res);
                decimal fraction = res - Math.Floor(res);

                if (fraction != 0 && res < 10)
                {
                    res *= 10;
                    return ((int)Math.Round(res)).ToString();
                }

                return temperatuur;
            }

            set => temperatuur = value;
        }
        public string WINDKRACHT
        {
            get
            {
                decimal.TryParse(wINDKRACHT, out var res);
                decimal fraction = res - Math.Floor(res);

                if (fraction != 0 && res < 10)
                {
                    res *= 10;
                    return ((int)Math.Round(res)).ToString();
                }

                return wINDKRACHT;
            }
            set => wINDKRACHT = value;
        }
        public string STAD_naam { get; set; }
    }
}

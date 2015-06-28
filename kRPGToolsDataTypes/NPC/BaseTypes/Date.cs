using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kRPGToolsDataTypes.Interfaces;

namespace kRPGToolsDataTypes.NPC.BaseTypes
{
    public class Date : IDate
    {
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int Day { get; private set; }

        public Date(int year, int month = 1, int day = 1)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public static Date Random (Random random, int yearStart = 1, int yearEnd = 21)
        {
            Random r = random ?? new Random();
            int year = yearStart >= yearEnd ? yearStart : r.Next(yearStart, yearEnd);
            return new Date(year, r.Next(1, 13), r.Next(1, 31));
        }

    }
}

using System;
using kRPGToolsDataTypes.Interfaces;

namespace kRPGToolsDataTypes.NPC.BaseTypes
{
    public class Sex : ISex
    {
        private static readonly Lazy<Sex> _male = new Lazy<Sex>(() => new Sex());
        public static Sex Male
        {
            get { return _male.Value; }
        }
        private static readonly Lazy<Sex> _female = new Lazy<Sex>(() => new Sex());
        public static Sex Female
        {
            get { return _female.Value; }
        }


        public static Sex Random(Random random = null)
        {
            return (random ?? new Random()).NextDouble() > 0.5 ? Female : Male;
        }
    }
    
}
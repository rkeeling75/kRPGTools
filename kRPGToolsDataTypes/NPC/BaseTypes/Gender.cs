using System;
using kRPGToolsDataTypes.Interfaces;

namespace kRPGToolsDataTypes.NPC.BaseTypes
{
    public class Gender :IGender
    {

        private static readonly Lazy<Gender> _male = new Lazy<Gender>(() => new Gender());
        public static Gender Male
        {
            get { return _male.Value; }
        }
        private static readonly Lazy<Gender> _female = new Lazy<Gender>(() => new Gender());
        public static Gender Female
        {
            get { return _female.Value; }
        }


        public static Gender Random(Random random = null)
        {
            return (random ?? new Random()).NextDouble() > 0.5 ? Female : Male;
        }
    }
}
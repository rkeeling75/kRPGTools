using System;
using kRPGToolsDataTypes.Interfaces;

namespace kRPGToolsDataTypes.NPC.BaseTypes
{
    public class Race : IRace
    {
        private static readonly Lazy<Race> _dwarf = new Lazy<Race>(() => new Race());
        public static Race Dwarf
        {
            get { return _dwarf.Value; }
        }

        private static readonly Lazy<Race> _elf = new Lazy<Race>(() => new Race());
        public static Race Elf
        {
            get { return _elf.Value; }
        }

        private static readonly Lazy<Race> _human = new Lazy<Race>(() => new Race());
        public static Race Human
        {
            get { return _human.Value; }
        }

        private static readonly Lazy<Race> _gnome = new Lazy<Race>(() => new Race());
        public static Race Gnome
        {
            get { return _gnome.Value; }
        }

        private static readonly Lazy<Race> _halfling = new Lazy<Race>(() => new Race());
        public static Race Halfling
        {
            get { return _halfling.Value; }
        }

        public static Race Random(Random random)
        {
            Random r = random ?? new Random();
            switch (r.Next(0, 5))
            {
                case 0:
                    return Dwarf;
                case 1:
                    return Elf;
                case 2:
                    return Human;
                case 3:
                    return Gnome;
                case 4:
                    return Halfling;
            }
            return Human;
        }
    }
}
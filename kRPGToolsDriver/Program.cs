using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using kRPGToolsDataTypes.Interfaces;
using kRPGToolsDataTypes.NPC;
using kRPGToolsDataTypes.NPC.BaseTypes;

namespace kRPGToolsDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            Test01();
        }

        private static void Test01()
        {
            Random random = new Random();
            List<Person> people = new List<Person>();
            for (int i = 0; i < 100; i++)
            {
                people.Add(new Person(random));
            }
            for (int y = 20; y < 200; y++)
            {
                Date thisYear = new Date(y);
                
                //Look for mate
                foreach (Person person in people)
                {
                    person.TryToWed(people, thisYear);
                }
                
                
                //If has mate, possible procreate
                List<Person> seen = new List<Person>();
                List<Person> newPeople = new List<Person>();
                foreach (var person in people.Where(p => p.CanProcreate(thisYear)))
                {
                    if (!seen.Contains(person))
                    {
                        IEnumerable<Person> children = person.TryToProcrate(thisYear);
                        bool hadChildren = false;
                        foreach (var child in children)
                        {
                            hadChildren = true;
                            newPeople.Add(child);
                        }
                        if (hadChildren)
                        {
                            seen.Add(person.Spouse);
                        }
                    }
                    seen.Add(person);
                }
                people.AddRange(newPeople);

                foreach (var person in people)
                {
                    person.TryToDie(thisYear);
                }
                
            }
        }


    }
}

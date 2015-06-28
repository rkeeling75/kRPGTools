using System;
using kRPGToolsDataTypes.Interfaces;

namespace kRPGToolsDataTypes.NPC.BaseTypes
{
    public class Relationship : IRelationship
    {
        public RelationshipType Type { get; private set; }
        public Person To { get; private set; }
        public bool Active { get; set; }
        public Relationship(Person to, RelationshipType type, bool active = true)
        {
            To = to;
            Type = type;
            Active = active;
        }
    }

    public enum RelationshipType
    {
        Husband,
        Wife,
        Father,
        Mother,
        Son,
        Daughter,
        Brother,
        Sister,
        Grandfather,
        Grandmother,
        Grandson,
        Granddaugther,
        Uncle,
        Aunt,
        Niece,
        Nephew,
        Cousin,
        Friend,
        Enemy,
    }
}
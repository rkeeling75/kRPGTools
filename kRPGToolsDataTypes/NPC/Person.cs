using System;
using System.Collections.Generic;
using System.Linq;
using kRPGToolsDataTypes.Interfaces;
using kRPGToolsDataTypes.NPC.BaseTypes;

namespace kRPGToolsDataTypes.NPC
{
    public class Person : IEntity
    {
        private const double InfertilityRate = 0.06;
        private const int MinimumBredingAge = 16;
        private const int MaximumBredingAge = 40;

        private const int MinimumMarryingAge = 20;
        private const int MaximumMarryingAge = 60;

        private const int MaximumProcreationAttempts = 52;
        private const double ProcreationSuccesRate = 0.025;

        private const double InfantMortality = .2;

        private const double MarriageRate = .5;
        private const double DivorceRate = .5;

        private const int MaximumAge = 80;
        
        public string Name { get; private set; }

        public IRace Race
        {
            get { return _race; }
        }

        public IGender Gender
        {
            get
            {
                if (_gender == null)
                {
                    _gender = _sex == BaseTypes.Sex.Male ? BaseTypes.Gender.Male : BaseTypes.Gender.Female;
                }
                return _gender;
            }
        }

        public ISex Sex
        {
            get { return _sex; }
        }

        public IDate BirthDate
        {
            get { return _birthDate; }
        }

        public IEnumerable<IRelationship> Relationships { get { return _relationships; } }
        public IEnumerable<IHealthStatus> HealthStatuses { get { return _healthStatuses; } }

        public Person Spouse
        {
            get
            {
                foreach (var r in _relationships)
                {
                    if ((r.Type == RelationshipType.Husband || r.Type == RelationshipType.Wife) && r.Active)
                    {
                        return r.To;
                    }
                }
                return null;
            }
        }


        private readonly List<Health> _healthStatuses = new List<Health>();
        private readonly List<Relationship> _relationships = new List<Relationship>();
        private readonly Random _random;
        private readonly Race _race;
        private Gender _gender;
        private readonly Sex _sex;
        private readonly Date _birthDate;

        public Person(string name, Race race, Sex sex, Date birthdate = null, Gender gender = null)
        {
            _random = new Random();
            _sex = sex;
            _gender = gender;
            _race = race;
            Name = name;
            _birthDate = birthdate;
            _healthStatuses.Add(Health.Alive);
            _healthStatuses.Add(DetermineIfInfertile(_random));
        }

        public Person(Random random)
        {
            _random = random ?? new Random();
            _sex = BaseTypes.Sex.Random(_random);
            _gender = BaseTypes.Gender.Random(_random);
            _race = BaseTypes.Race.Random(_random);
            _birthDate = BaseTypes.Date.Random(_random);
            _healthStatuses.Add(Health.Alive);
            _healthStatuses.Add(DetermineIfInfertile(_random));
        }

        public Person(Person parent1, Person parent2, Date birthDate):this(null)
        {
            _birthDate = birthDate;
            if (_random.NextDouble() < InfantMortality)
            {
                _healthStatuses.Clear();
                _healthStatuses.Add(Health.Dead);
            }
            parent1.HaveChild(this);
            parent2.HaveChild(this);
        }

        private void HaveChild(Person child)
        {
            RelationshipType myStatus = Gender == BaseTypes.Gender.Female
                ? RelationshipType.Mother
                : RelationshipType.Father;
            RelationshipType childStatus = child.Gender == BaseTypes.Gender.Female
                ? RelationshipType.Daughter
                : RelationshipType.Son;

            foreach (var p in GetChildren)
            {
                p.SetSibling(child);
            }
            foreach (var p in GetParents)
            {
                p.SetGrandchild(child);
            }
            foreach (var p in GetSiblings)
            {
                p.SetAuntUncle(child);
                foreach (var pc in p.GetChildren)
                {
                    pc._relationships.Add(new Relationship(child, RelationshipType.Cousin));
                    child._relationships.Add(new Relationship(pc, RelationshipType.Cousin));
                }
            }

            _relationships.Add(new Relationship(child, myStatus));
            child._relationships.Add(new Relationship(this, childStatus));
        }

        private void SetSibling(Person child)
        {
            RelationshipType myStatus = Gender == BaseTypes.Gender.Female
                ? RelationshipType.Sister
                : RelationshipType.Brother;
            RelationshipType childStatus = child.Gender == BaseTypes.Gender.Female
                ? RelationshipType.Sister
                : RelationshipType.Brother;
            _relationships.Add(new Relationship(child, myStatus));
            child._relationships.Add(new Relationship(this, childStatus));
        }
        private void SetAuntUncle(Person child)
        {
            RelationshipType myStatus = Gender == BaseTypes.Gender.Female
                ? RelationshipType.Aunt
                : RelationshipType.Uncle;
            RelationshipType childStatus = child.Gender == BaseTypes.Gender.Female
                ? RelationshipType.Niece
                : RelationshipType.Nephew;
            _relationships.Add(new Relationship(child, myStatus));
            child._relationships.Add(new Relationship(this, childStatus));
        }
        public IEnumerable<Person> GetSiblings
        {
            get
            {
                foreach (var r in _relationships)
                {
                    if (r.Type == RelationshipType.Brother || r.Type == RelationshipType.Sister)
                    {
                        yield return r.To;
                    }
                }
            }
        }

        public IEnumerable<Person> GetChildren
        {
            get
            {
                foreach (var r in _relationships)
                {
                    if (r.Type == RelationshipType.Father || r.Type == RelationshipType.Mother)
                    {
                        yield return r.To;
                    }
                }
            }
        }

        private void SetGrandchild(Person child)
        {
            RelationshipType myStatus = Gender == BaseTypes.Gender.Female
                ? RelationshipType.Grandmother
                : RelationshipType.Grandfather;
            RelationshipType childStatus = child.Gender == BaseTypes.Gender.Female
                ? RelationshipType.Granddaugther
                : RelationshipType.Grandson;
            _relationships.Add(new Relationship(child, myStatus));
            child._relationships.Add(new Relationship(this, childStatus));
        }

        public IEnumerable<Person> GetParents
        {
            get
            {
                foreach (var r in _relationships)
                {
                    if (r.Type == RelationshipType.Daughter || r.Type == RelationshipType.Son)
                    {
                        yield return r.To;
                    }
                }
            }
        }

        public bool CanMarry(Date when)
        {
            if (_relationships.Any(r => (r.Type == RelationshipType.Husband || r.Type == RelationshipType.Wife) && r.Active))
            {
                return false;
            }
            return Age(when) >= MinimumMarryingAge && Age(when) <= MaximumMarryingAge;
        }

        public bool CanProcreate(Date when)
        {
                if (_healthStatuses.Contains(Health.Dead) || _healthStatuses.Contains(Health.Barren))
                {
                    return false;
                }
            return Age(when) >= MinimumBredingAge && Age(when) <= MaximumBredingAge;
        }

        public int Age(Date when)
        {
            return when.Year - _birthDate.Year;
        }

        private static Health DetermineIfInfertile(Random random)
        {
            Random r = random ?? new Random();
            return r.NextDouble() < InfertilityRate ? Health.Barren : (Health.Fertile);
        }

        public void TryToWed(IEnumerable<Person> people, Date when)
        {
            if (CanMarry(when))
            {
                foreach (var person in people.Where(p => p.CanMarry(when)))
                {
                    if (person != this && person.Gender != Gender && !person.IsEnemyOf(this) && !IsEnemyOf(person) && !AreRelated(person))
                    {
                        double marriageRate = MarriageRate;
                        if (person.IsFriendOf(this) && IsFriendOf(person))
                        {
                            marriageRate *= 1.5;
                        }
                        if (_random.NextDouble() < marriageRate && person._random.NextDouble() < marriageRate)
                        {
                            _relationships.Add(new Relationship(person,
                                Gender == BaseTypes.Gender.Female ? RelationshipType.Wife : RelationshipType.Husband));
                            person._relationships.Add(new Relationship(this,
                                person.Gender == BaseTypes.Gender.Female
                                    ? RelationshipType.Wife
                                    : RelationshipType.Husband));
                            return;
                        }
                    }
                }
            }
        }

        private bool IsEnemyOf(Person person)
        {
            return _relationships.Any(r => r.Type == RelationshipType.Enemy && r.To == person && r.Active);
        }
        private bool IsFriendOf(Person person)
        {
            return _relationships.Any(r => r.Type == RelationshipType.Friend && r.To == person && r.Active);
        }

        public IEnumerable<Person> TryToProcrate(Date when)
        {
            Person spouse = Spouse;
            if (spouse != null && CanProcreate(when) && spouse.CanProcreate(when))
            {
                int attempts = MaximumProcreationAttempts - (NumberOfChildrenHad*5);
                for (int a = 0; a < attempts; a++)
                {
                    if (_random.NextDouble() <= ProcreationSuccesRate)
                    {
                        a = MaximumProcreationAttempts + 1;
                        double countRate = spouse._random.NextDouble();
                        int numberOfChildren = 1;
                        if (countRate < 0.0014)
                        {
                            numberOfChildren = 3;
                        }
                        else if (countRate < 0.033)
                        {
                            numberOfChildren = 2;
                        }
                        Date childrenBirthdate = Date.Random(null, when.Year);
                       
                        for (int i = 0; i < numberOfChildren; i++)
                        {
                            yield return new Person(this, spouse, childrenBirthdate);
                        }
                    }
                }
            }
        }

        public bool AreRelated(Person person)
        {
            foreach (var r in _relationships)
            {
                switch (r.Type)
                {
                    case RelationshipType.Father:
                    case RelationshipType.Mother:
                    case RelationshipType.Son:
                    case RelationshipType.Daughter:
                    case RelationshipType.Brother:
                    case RelationshipType.Sister:
                    case RelationshipType.Grandfather:
                    case RelationshipType.Grandmother:
                    case RelationshipType.Grandson:
                    case RelationshipType.Granddaugther:
                    case RelationshipType.Uncle:
                    case RelationshipType.Aunt:
                    case RelationshipType.Niece:
                    case RelationshipType.Nephew:
                    case RelationshipType.Cousin:
                        return true;

                }
            }
            return false;
        }
        public int NumberOfChildrenHad
        {
            get
            {
                return _relationships.Count(r => r.Type == RelationshipType.Father || r.Type == RelationshipType.Mother);
            }
        }

        public void TryToDie(Date when)
        {
            double yearsOld = Age(when);
            double percentToDie = yearsOld/ MaximumAge;
            if (_random.NextDouble() <= percentToDie)
            {
                Die();
            }
        }

        private void Die()
        {
            _healthStatuses.RemoveAll(h => h == Health.Alive);
            _healthStatuses.Add(Health.Dead);
            Person spouse = Spouse;
            if (spouse != null)
            {
                spouse.Divorce();
                Divorce();
            }
        }

        private void Divorce()
        {
            foreach (var r in _relationships)
            {
                if (r.Type == RelationshipType.Husband || r.Type == RelationshipType.Wife)
                {
                    r.Active = false;
                }
            }
        }
    }
}

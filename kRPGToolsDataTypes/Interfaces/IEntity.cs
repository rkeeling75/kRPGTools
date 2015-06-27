using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kRPGToolsDataTypes.Interfaces
{
    /// <summary>
    /// The type for all other entities in the system. This includes city dwellers, monsters, gods, etc.
    /// </summary>
    interface IEntity
    {
        /// <summary>
        /// The identifier by which the entity is known by
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The race or species that the entity belongs to
        /// </summary>
        IRace Race { get; }
        /// <summary>
        /// The gender, if any that the entity identifies as. If gender is undefined then sex is assumed
        /// </summary>
        IGender Gender { get; }
        /// <summary>
        /// The biological sex, if any, of the entity
        /// </summary>
        ISex Sex { get; }
        /// <summary>
        /// Number of relationships the entity is involved in. Husband, enemy, lord, daughter, landlord, etc.
        /// </summary>
        IEnumerable<IRelationship> Relationships { get; }
    }
}

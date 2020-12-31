using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class PharmacyEntity : EntityBase
    {

        public PharmacyEntity()
        {
            this.EntityPluralName = "new_pharmacies";
            this.EntitySingularName = "new_pharmacy";
        }

    }


    public static class PharmacyFields
    {
        public const string Name = "new_pharmacy";
        public const string EntityId = "new_pharmacyid";
        public const string Id = "new_idpharmacy";
    }
}

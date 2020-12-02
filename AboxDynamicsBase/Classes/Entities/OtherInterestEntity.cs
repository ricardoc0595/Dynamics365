using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class OtherInterestEntity : EntityBase

    {


        public OtherInterestEntity()
        {
            this.EntityPluralName = "new_otherinterests";
            this.EntitySingularName = "new_otherinterest";
        }

    }
    public static class OtherInterestFields
    {
        public const string Country = "new_interestcountry";
        public const string Id = "new_idotherinterest";

    }

    public static class OtherInterestSchemas
    {
    }

}


using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ActivationKey: PersistentObject,IDeletableObject
    {
        public virtual User User { get; set; }
        public virtual string Key { get; set; }
        public virtual KeyType Type { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ActivatedDate { get; set; }
        public virtual bool Deleted { get; set; }


    }
}

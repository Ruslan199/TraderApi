using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;



namespace Domain
{
    public class Pair : PersistentObject,IDeletableObject
    {
        public virtual string Value { get; set; }
        public virtual bool Deleted { get; set; }
    }
}

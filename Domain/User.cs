using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class User : PersistentObject, IDeletableObject
    {
        public virtual string  UserName { get; set; }
        public virtual string Mail { get; set; }
        public virtual string Password { get; set; }
        public virtual bool Deleted { get; set; }
    }
}

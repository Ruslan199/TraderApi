using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public interface IDeletableObject
    {
        bool Deleted { get; set; }
    }
}

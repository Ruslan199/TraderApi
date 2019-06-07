using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Interfaces
{
    public interface ISessionStorage
    {
        ISession Session { get; }
    }
}

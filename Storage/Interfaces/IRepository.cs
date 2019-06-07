using Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage.Interfaces
{
    public interface IRepository<T> where T : PersistentObject
    {
        T Load(object nodeId);

        T Get(long id);

        IQueryable<T> GetAll();

        ISQLQuery ExecuteSqlQuery(string sqlQuery);

        void Add(T entity);

        void AddArray(List<T> array);

        void Update(T entity);

        void Remove(T entity);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Interfaces
{
    public interface IBaseCrudService<T>
    {
        IQueryable<T> GetAll();
        T Get(long id);
        void Create(T item);
        void Update(T item);
        void Delete(T item);
        void Delete(long id);
        void CreateArray(List<T> array);
    }
}

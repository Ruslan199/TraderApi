﻿using Domain;
using NHibernate;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Storage.Repositories
{
    public class Repository<T> : IRepository<T> where T : PersistentObject, new()
    {
        private readonly ISessionStorage _sessionStorage;

        public Repository(ISessionStorage ss)
        {
            _sessionStorage = ss;
        }

        public Type ElementType => OpenSession().Query<T>().ElementType;

        public IQueryProvider Provider => OpenSession().Query<T>().Provider;

        public T Load(object nodeId)
        {
            return !CheckId(nodeId) ? null : OpenSession().Load<T>(nodeId);
        }

        public ISQLQuery ExecuteSqlQuery(string sqlQuery)
        {
                var session = OpenSession();
            
                Logger.WriteLogg("NHibernate", sqlQuery, "info");
                return session.CreateSQLQuery(sqlQuery);
            
        }

        public T Get(long id)
        {
                var session = OpenSession();
            
                Logger.NhibernateSelect<T>(id.ToString());
                var returnVal = session.Get<T>(id);

                return returnVal;
            
        }

        public IQueryable<T> GetAll()
        {
            var session = OpenSession();
            {
                Logger.NhibernateSelect<T>("");
                return session.Query<T>();
            }
        }

        public void Add(T entity)
        {
            var session = OpenSession();
            //Logger.NhibernateInsert(entity, "ID");
            using (var transaction = BeginTransaction(session))
            {
                session.Save(entity);
                transaction.Commit();
                session.Flush();
            }
        }

        public void Update(T entity)
        {
            using (var session = OpenSession())
            {
                Logger.NhibernateUpdate(entity, "ID");
                session.Update(entity);
                session.Flush();
            }
        }

        public void AddArray(List<T> array)
        {
                var session = OpenSession();
            
                //Logger.Logger.NhibernateInsertArray(array); -TO-DO
                using (var transaction = BeginTransaction(session))
                {
                  foreach (var item in array)
                    session.Save(item);
                    transaction.Commit();
                    session.Flush();
                }
            
        }

        public void Remove(T entity)
        {
            using (var session = OpenSession())
            {
                Logger.NhibernateDelete(entity);
                using (var transaction = BeginTransaction(session))
                {
                    session.Delete(entity);
                    transaction.Commit();
                }
            }
        }

        public bool CheckId(object id)
        {
            var objectId = id as int?;
            return objectId.HasValue && objectId.Value != 0;
        }

        protected ISession OpenSession()
        {
            return _sessionStorage.Session;
        }

        protected ITransaction BeginTransaction(ISession session)
        {
            return session.BeginTransaction();
        }
    }
}

using System;
using System.Collections.Generic;
using CoreMvcPoc.Entities;

namespace CoreMvcPoc.DAL
{
    public interface IRepository<T> where T : BaseModel
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Predicate<T> predicate);
        T Get(long id);
        T Get(Predicate<T> predicate);
        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool Remove(T entity);
        void SaveChanges();
    }
}
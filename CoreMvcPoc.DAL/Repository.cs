using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CoreMvcPoc.Entities;

namespace CoreMvcPoc.DAL
{
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        private readonly ApplicationDbContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }
        public bool Delete(T entity)
        {           
            if (entity == null)
            {
                return false;
            }
            entities.Remove(entity);
            context.SaveChanges();
            return true;           
        }

        public T Get(long id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public T Get(Predicate<T> predicate)
        {
            return entities.FirstOrDefault(new Func<T, bool>(predicate));
        }

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public IEnumerable<T> GetAll(Predicate<T> predicate)
        {
            return entities.Where(new Func<T, bool>(predicate));
        }

        public bool Insert(T entity)
        {
            if (entity == null)
            {
                return false;
            }
            entities.Add(entity);
            context.SaveChanges();
            return true;
        }

        public bool Remove(T entity)
        {
            if (entity == null)
            {
                return false;
            }
            entities.Remove(entity);     
            return true;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public bool Update(T entity)
        {
            if (entity == null)
            {
               return false;
            }
            entities.Update(entity);
            context.SaveChanges();
            return true;
        }
    }
}
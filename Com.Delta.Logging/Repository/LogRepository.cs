using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace Com.Delta.Logging
{
    public class LogRepository<TType, TContext> : ILogRepository<TType>
        where TContext : DbContext, new()
        where TType : class
    {
        private readonly TContext _entity = new TContext();

        protected TContext Context
        {
            get { return this._entity; }
        }

        //protected string GetEntitySetName(string entityTypeName)
        //{           

        //    return this._entity.MetadataWorkspace
        //                        .GetEntityContainer(this._entity.DefaultContainerName, DataSpace.CSpace)
        //                        .BaseEntitySets
        //                        .Single(x => x.ElementType.Name.Equals(entityTypeName)).Name;
        //}

        public virtual TType Add(TType entity)
        {
            this._entity.Entry(entity).State = EntityState.Added;
            this._entity.SaveChanges();
            return entity;
        }

        public virtual bool Update(TType entity)
        {
            this._entity.Entry(entity).State = EntityState.Modified;            
            this._entity.SaveChanges();
            return true;
        }

        public virtual bool Delete(TType entity)
        {
            this._entity.Entry(entity).State = EntityState.Deleted;          
            this._entity.SaveChanges();
            return true;
        }
       

        public TType Get(Expression<Func<TType, bool>> filter)
        {
            return this._entity.Set<TType>().SingleOrDefault(filter);
           // return this._entity.CreateObjectSet<TType>().SingleOrDefault(filter);
        }

        public IQueryable<TType> GetBySearch(Expression<Func<TType, bool>> filter)
        {
            return this._entity.Set<TType>().Where(filter);
            // return this._entity.CreateObjectSet<TType>().SingleOrDefault(filter);
        }
        public IQueryable<TType> GetBySearchKit(Expression<Func<TType, bool>> filter)
        {
            return this._entity.Set<TType>().Where(filter.Expand());
            // return this._entity.CreateObjectSet<TType>().SingleOrDefault(filter);
        }
        public ICollection<TType> GetAll()
        {
            return this._entity.Set<TType>().ToList();
            //return this._entity.CreateObjectSet<TType>().ToList();
        }
    }
}

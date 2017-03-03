using DBRepoProvider.Core.Observer;
using System.Data.Entity;
using System.Linq;

namespace DBRepoProvider.Core
{
    public class GenericRepository<TEntity> : Subject where TEntity : class
    {
        private readonly DbContext context;

        internal GenericRepository(DbContext context)
        {
            this.context = context;
        }

        public TEntity GetByID(decimal id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public TEntity Add(TEntity model)
        {
            var enitity = context.Set<TEntity>().Add(model);
            NotifyObjects();
            return enitity;
        }

        public TEntity Remove(TEntity model)
        {
            var entity = context.Set<TEntity>().Remove(model);
            NotifyObjects();
            return entity;
        }

        public TEntity Edit(TEntity model)
        {
            var entity = context.Set<TEntity>().Attach(model);
            context.Entry(model).State = EntityState.Modified;
            NotifyObjects();
            return entity;
        }

        public IQueryable<TEntity> List()
        {
            return context.Set<TEntity>().AsQueryable();
        }


    }
}

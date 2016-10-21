using Dawn.DbContextScope.Interfaces;
using Dawn.Domain;
using Dawn.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Repository.EF
{
    public abstract class BaseRepository<TDbContext, TAggregateRoot> : IRepository<TAggregateRoot>
       where TAggregateRoot : class, IAggregateRoot where TDbContext : DbContext
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;
        private IQueryable<TAggregateRoot> _entities;

        public BaseRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            _ambientDbContextLocator = ambientDbContextLocator;
        }


        private TDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<TDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException("No ambient DbContext of type TDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");

                return dbContext;
            }
        }

        protected Database Database
        {
            get
            {
                return DbContext.Database;
            }
        }

        protected IQueryable<TAggregateRoot> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = DbContext.Set<TAggregateRoot>().AsNoTracking();
                return _entities;
            }
        }

        public IQueryable<TAggregateRoot> Get(int id)
        {
            return Entities.Where(t => t.Id == id);
        }

        public IQueryable<TAggregateRoot> GetAll()
        {
            return Entities;
        }



    }
}

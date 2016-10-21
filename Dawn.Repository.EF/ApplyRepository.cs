using Dawn.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawn.DbContextScope.Interfaces;
using Dawn.Repository.Interfaces;
using Dawn.Domain.ValueObjects;

namespace Dawn.Repository.EF
{
    public abstract class ApplyRepository<TDbContext, TApplyAggregateRoot> : BaseRepository<TDbContext, TApplyAggregateRoot>, IApplyRepository<TApplyAggregateRoot>
        where TApplyAggregateRoot : ApplyAggregateRoot where TDbContext : DbContext
    {
        public ApplyRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {

        }

        public IQueryable<TApplyAggregateRoot> GetByUserId(int userId)
        {
            return Entities.Where(x => x.User.Id == userId && x.IsActive);
        }

        public IQueryable<TApplyAggregateRoot> GetWaiting(int userId)
        {
            return Entities.Where(x => x.User.Id == userId && x.Status == Status.Wait && x.IsActive);
        }

        public IQueryable<TApplyAggregateRoot> GetWaiting()
        {
            return Entities.Where(x => x.Status == Status.Wait && x.IsActive);
        }
    }
}

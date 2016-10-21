using Dawn.Domain.Entity;
using Dawn.Repository.Interfaces;
using System.Linq;
using Dawn.DbContextScope.Interfaces;
using Dawn.Domain.ValueObjects;

namespace Dawn.Repository.EF
{
    public class BlogChangeApplyRepository : ApplyRepository<ApplyDbContext, BlogChangeApply>, IBlogChangeApplyRepository
    {
        public BlogChangeApplyRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {

        }

        public IQueryable<BlogChangeApply> GetByTargetAliasWithWait(string targetBlogApp)
        {
            return Entities.Where(x => x.TargetBlogApp == targetBlogApp && x.Status == Status.Wait && x.IsActive);
        }
    }
}

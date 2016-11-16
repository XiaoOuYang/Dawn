using Dawn.Application.Interfaces;
using Dawn.DbContextScope.Interfaces;
using Dawn.Domain.DomainServices;
using Dawn.Domain.Entity;
using Dawn.Domain.ValueObjects;
using Dawn.Repository.EF;
using Dawn.Repository.Interfaces;
using Dawn.ServiceAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Application.Services
{
    public class BlogChangeApplyService : IBlogChangeApplyService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IBlogChangeApplyRepository _blogChangeApplyRepository;
        private readonly IApplyAuthenticationService _applyAuthenticationService;

        public BlogChangeApplyService(IDbContextScopeFactory dbContextScopeFactory,
            IBlogChangeApplyRepository blogChangeApplyRepository,
            IApplyAuthenticationService applyAuthenticationService)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _blogChangeApplyRepository = blogChangeApplyRepository;
            _applyAuthenticationService = applyAuthenticationService;

        }
        public SubmitResult Apply(string targetBlogApp, string reason, string userLoginName, string ip)
        {
            var user = UserService.GetUserByLoginName(userLoginName).Result;

            
            using (var dbScope = _dbContextScopeFactory.Create())
            {

                var verfiyResult = _applyAuthenticationService.VerfiyForBlogChange(user, targetBlogApp);
                if (!string.IsNullOrEmpty(verfiyResult))
                {
                    return new SubmitResult { IsSucceed = false, Message = verfiyResult };
                }

                try
                {
                    var blogChangeApply = new BlogChangeApply(targetBlogApp, reason, user, ip);
                    dbScope.RegisterNew<ApplyDbContext, BlogChangeApply>(blogChangeApply);
                    return new SubmitResult { IsSucceed = dbScope.SaveChanges() > 0 };
                }
                catch (ArgumentException ae)
                {
                    return new SubmitResult { IsSucceed = false, Message = ae.Message };
                }
                catch (Exception ex)
                {
                    return new SubmitResult { IsSucceed = false, Message = ex.Message };
                }
            }
        }



        public Status GetStatus(string userLoginName)
        {
            var user = UserService.GetUserByLoginName(userLoginName).Result;
            if (user == null || user?.Id == 0)
            {
                return Status.None;
            }

            using (var dbScope = _dbContextScopeFactory.CreateReadOnly())
            {
                var apply = _blogChangeApplyRepository.GetByUserId(user.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                if (apply == null)
                {
                    return Status.None;
                }
                return apply.GetStatus();
            }

        }
    }
}

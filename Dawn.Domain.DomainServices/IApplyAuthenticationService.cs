using Dawn.Domain.ValueObjects;

namespace Dawn.Domain.DomainServices
{
    public interface IApplyAuthenticationService
    {
        string VerfiyForJsPermission(User user);

        string VerfiyForBlogChange(User user, string targetBlogApp);
    }
}

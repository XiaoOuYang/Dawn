using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dawn.Application.Interfaces;

namespace Dawn.Tests.Application
{
    [TestClass]
    public class BlogChangeApplyServiceTest
    {
        private IBlogChangeApplyService _blogChangeApplyService;
        public BlogChangeApplyServiceTest()
        {

        }

        [TestMethod]
        public void ApplyTest()
        {
            var userLoginName = "userName";
            var targetBlogApp = "blogdizhi";
            var result =  _blogChangeApplyService.Apply(targetBlogApp, "我想要修改博客地址", userLoginName, "");
            Console.WriteLine(result.Message);
            Assert.IsTrue(result.IsSucceed);

        }
    }
}

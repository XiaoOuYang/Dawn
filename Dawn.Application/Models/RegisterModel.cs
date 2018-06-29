using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Application.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "请输入账号。")]
        public string Account { get; set; }
        [Required(ErrorMessage = "请输入密码。")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
    public class RegisterModel
    {
        [Display(Name = "账号")]
        [Required(ErrorMessage = "请输入账号。")]
        public string UserName { get; set; }
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码。")]
        public string Password { get; set; }

        [Display(Name = "确认密码")]
        [Required(ErrorMessage = "请输入密码确认。")]
        [Compare("Password", ErrorMessage = "密码输入不一致。")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入名称。")]
        public string NickName { get; set; }
        [Display(Name = "手机号")]
        public string MobilePhone { get; set; }
        [Display(Name = "邮箱")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string Email { get; set; }

        [Display(Name = "角色")]
        public int RoleId { get; set; }

        [Display(Name = "工号id")]
        public string WorkId { get; set; }
        [Display(Name = "工号")]
        public string WorkNum { get; set; }
    }
}

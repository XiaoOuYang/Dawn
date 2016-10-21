using Dawn.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dawn.ServiceAgent
{
    public class UserService
    {
        private static string _userHost = "";

        public static bool IsAdmin(string loginName)
        {
            using (var httpCilent = new HttpClient())
            {
                httpCilent.BaseAddress = new System.Uri(_userHost);
                var requestMessage = new HttpRequestMessage(HttpMethod.Head, $"/users?loginName={loginName}&&roleName=网站管理员");
                var response = httpCilent.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    IEnumerable<string> outValues;
                    if (response.Headers.TryGetValues("X-IsUserRole", out outValues))
                    {
                        return outValues.First().Equals("1") ? true : false;
                    }
                }
                else
                {
                    //Logging.Logger.Default.Info("UserService.IsAdmin",
                    //    $"{response.StatusCode}: {response.Content.ReadAsStringAsync().Result}");
                }
                return false;
            }
        }

        public static async Task<User> GetUserByLoginName(string loginName)
        {
            using (var httpCilent = new HttpClient())
            {
                httpCilent.BaseAddress = new System.Uri(_userHost);
                var response = await httpCilent.GetAsync($"/users?loginName={Uri.EscapeDataString(loginName)}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    //Logging.Logger.Default.Info("UserService.GetUserByLoginName",
                    //    $"{response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                }
                return null;
            }
        }
    }
}

using ImitateLogin;
using System.Web.Http;

namespace MailNotication.Controllers
{
    [Authorize]
    public class MailController : ApiController
    {
        /// <summary>
        /// Send 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string SendMail(LoginSite loginSite, string imageUrl)
        {
            if (MailHelper.SendEmail(loginSite.ToString() + "登录中，请求协助！", string.Format("<img src=\"{0}\"><br /> {0}", imageUrl)))
                return "success";
            else
                return "";
        }
    }
}

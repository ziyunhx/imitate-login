using ImitateLogin;
using System.Web.Http;

namespace MailNotication.Controllers
{
    public class MailController : ApiController
    {
        /// <summary>
        /// Send 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string SendMail(LoginSite loginSite, string imageUrl)
        {
            if (MailHelper.SendEmail(loginSite.ToString() + "Logining, please help me!", string.Format("<img src=\"{0}\"><br /> {0}", imageUrl)))
                return "success";
            else
                return null;
        }
    }
}

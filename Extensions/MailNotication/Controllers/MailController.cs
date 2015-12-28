using ImitateLogin;
using System.Web.Http;

namespace MailNotication.Controllers
{
    [Authorize]
    public class MailController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string SendMail(LoginSite loginSite, string imageUrl)
        {
            return "success";
        }

        [HttpPost]
        public string SendMail(OperationObj operationObj)
        {
            return "success";
        }
    }
}

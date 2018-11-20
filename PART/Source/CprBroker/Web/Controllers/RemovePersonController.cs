using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CprBroker.Web.Controllers
{
    [RoutePrefix("Pages/RemovePerson")]
    public class RemovePersonController : Controller
    {
        // GET: RemovePerson
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
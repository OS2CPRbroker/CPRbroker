using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CprBroker.Web.Controllers
{
    [RoutePrefix("Pages/RemovePerson")]
    public class RemovePersonController : Controller
    {
        // HTTP GET
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("")]
        public ActionResult RemovePerson()
        {
            string response = null;
            try
            {
                string cprnr = Request["cprNrInputField"].ToString();
                if (Regex.IsMatch(cprnr, @"^\d{10}$")) {
                    // 1) Call Part.GetUuid()
                    // 2) Verify that everything went well.
                    // 3) Call Admin.RemovePerson()
                    // 4) Verify that everything went well.
                    response = string.Format("{0} was succesfully delivered to RemovePersonController.", cprnr);
                }
                else
                {
                    response = "Please enter a valid 10 digit number.";
                }
            }
            catch (NullReferenceException ex)
            {
                response = ex.ToString(); // Remove and log exception to CPR Broker before release.
            }             
            return Content(response);
        }
    }
}
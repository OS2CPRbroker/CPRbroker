using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CprBroker.PartInterface;
using CprBroker.Slet;
using CprBroker.Schemas.Part;

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
            try
            {
                if(Request["cprNrInputField"] == null)
                {
                    return Content("Please enter a valid 10 digit number.");
                }
                string cprnr = Request["cprNrInputField"].ToString();
                if (Regex.IsMatch(cprnr, @"^\d{10}$")) {
                    // 1) Call Part.GetUuid()
                    PartManager partManager = new PartManager();
                    GetUuidOutputType UUIDOutput = partManager.GetUuid(this.HttpContext.User.Identity.Name, 
                        CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), 
                        cprnr);

                    // 2) Verify that everything went well.
                    if (StandardReturType.IsSucceeded(UUIDOutput.StandardRetur)){

                        // 3) Call Admin.RemovePerson()
                        RemovePersonManager rmPersonManager = new RemovePersonManager();
                        BasicOutputType<bool> removeOutputType = rmPersonManager.RemovePerson(this.HttpContext.User.Identity.Name,
                            CprBroker.Utilities.Constants.BaseApplicationToken.ToString(),
                            Guid.Parse(UUIDOutput.UUID));

                        // 4) Verify that everything went well.
                        if (StandardReturType.IsSucceeded(removeOutputType.StandardRetur))
                        {
                            if (removeOutputType.Item)
                            {
                                return Content("Person removed.");
                            } 
                            else
                            {
                                return Content("Something went wrong - Check the CprBroker log for details.");
                            }
                        } 
                        else
                        {
                            return Content("Something went wrong - Check the CprBroker log for details.");
                        }
                    }
                    else
                    {
                        return Content("Could not find Person with cpr number: "+cprnr);
                    }

                }
                else
                {
                    return Content("Please enter a valid 10 digit number.");
                }
            }
            catch (Exception ex)
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), CprBroker.Utilities.Constants.UserToken);
                Engine.Local.Admin.LogException(ex);
                return Content("Something went wrong - Check the CprBroker log for details");
            }
        }
    }
}
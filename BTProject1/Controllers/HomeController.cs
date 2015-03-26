using BTProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BTProject1.Controllers
{

    public class HomeController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        
       
    }
}
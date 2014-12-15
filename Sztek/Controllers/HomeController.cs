using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sztek.Models;


namespace Sztek.Controllers
{
    public class HomeController : Controller
    {
        
        
        public ActionResult Index()
        {
            
            ViewBag.Message = "";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Az oldal készítésének célja.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }
    }
}

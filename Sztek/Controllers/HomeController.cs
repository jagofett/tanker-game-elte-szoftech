using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sztek.Models;
using WebMatrix.WebData;


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

        public ActionResult NotLoggedIn()
        {
            return View();
        }

        
        public ActionResult Lobby()
        {
            if (Request.IsAuthenticated)
            {
                var database = new DatabaseEntities();

                return View(database);
            }
            else
            {
                return RedirectToAction("NotLoggedIn", "Home");
            }
        }

        [HttpPost]
        public ActionResult JoinLobby()
        {


            // If we got this far, something failed, redisplay form
            return RedirectToAction("Lobby", "Home");
        }
    }
}

using System.Security.Principal;
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
        private DatabaseEntities _entities = new DatabaseEntities();
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

        [Authorize]
        public ActionResult Lobby()
        {
            if (Request.IsAuthenticated)
            {
                var current = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
                if (current == null)
                    return RedirectToAction("Index");

                ViewBag.Message = current.in_lobby.GetValueOrDefault()
                    ? "Kilépés"
                    : "Csatlakozás";

                return View(_entities);
            }
            else
            {
                return RedirectToAction("NotLoggedIn", "Home");
            }
        }
        [Authorize]
        [HttpPost]
        public ActionResult JoinLobby()
        {
            var current = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
            if (current == null)
                return RedirectToAction("Index");

            if (current.in_lobby.GetValueOrDefault())
                current.in_lobby = false;
            else
                current.in_lobby = true;

            _entities.SaveChanges();

            ViewBag.Message = current.in_lobby.GetValueOrDefault()
                ? "Kilépés"
                : "Csatlakozás";

            return RedirectToAction("Lobby", "Home");
        }
    }
}

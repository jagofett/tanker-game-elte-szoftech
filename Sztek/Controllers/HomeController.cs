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

        public ActionResult JoinedToLobby()
        {
            if (Request.IsAuthenticated)
            {
                var current = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
                if (current == null)
                    return RedirectToAction("Index");

                ViewBag.Message = current.in_lobby.GetValueOrDefault()
                    ? "Kilépés"
                    : "Csatlakozás";

                return PartialView(_entities);
            }
            else
            {
                return RedirectToAction("NotLoggedIn", "Home");
            }
        }

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

            current.in_lobby = !current.in_lobby.GetValueOrDefault();
            _entities.SaveChanges();

            var inLobby = _entities.Users.Where(x => x.in_lobby != null && (bool) x.in_lobby).ToList();
            if (inLobby.Count() >= 4)
            {
                var newGame = new games() {max_player = 4, users = inLobby, status = true};
                _entities.Games.Add(newGame);
                inLobby.ForEach(x => { x.in_lobby = false;
                                         x.game = newGame;
                });
                //egyéb game  indítás...
            }

            _entities.SaveChanges();

            ViewBag.Message = current.in_lobby.GetValueOrDefault()
                ? "Kilépés"
                : "Csatlakozás";

            return RedirectToAction("Lobby", "Home");
        }
    }
}

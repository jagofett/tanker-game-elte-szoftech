using System.Security.Principal;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Sztek.Models;
using WebMatrix.WebData;
using SignalRChatApp.Hubs;
//using Sztek.SztekWebServiceReference;

namespace Sztek.Controllers
{
    public class HomeController : Controller
    {       
        private readonly DatabaseEntities _entities = new DatabaseEntities();
        

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


        public JsonResult EndGameResult(int gameId, int winnerId)
        {
            var game = _entities.Games.FirstOrDefault(i => i.id == gameId);
            var winner = _entities.Users.FirstOrDefault(us => us.id == winnerId);
            if (game == null || winner == null || !game.status)
            {
                return Json(new {error = true, message = "Hiányzó adat!"}, JsonRequestBehavior.AllowGet);
            }
            game.status = false;
            var result = new results {games = game, users = winner, score = 1};
            _entities.Results.Add(result);
            game.users.ToList().ForEach(us => { us.game = null; });

            _entities.SaveChanges();

            return Json(new {error = false}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EndGameSample()
        {
            return View();
        }
    }


}

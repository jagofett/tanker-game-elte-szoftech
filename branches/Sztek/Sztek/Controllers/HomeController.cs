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

namespace Sztek.Controllers
{
    public class HomeController : Controller
    {       
        private readonly DatabaseEntities _entities = new DatabaseEntities();
        private readonly HubHandler _hubHandler;

        public HomeController() : this(HubHandler.Instance) { }

        public HomeController(HubHandler hubHandler)
        {
            _hubHandler = hubHandler;
        }
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

        public void GetLobbyList()
        {
            if (Request.IsAuthenticated)
            {
                _hubHandler.LobbyList(LobbyUsers());
            }
        }



        private Object LobbyUsers()
        {
            return _entities.Users.Where(user => user.in_lobby != null && (bool)user.in_lobby)
                        .OrderBy(us => us.username)
                        .Select(x => new { x.username, x.id})
                        .ToList();
        }

        //todo exit lobby when leaving!
        public ActionResult Lobby(int? startId)
        {
            if (Request.IsAuthenticated)
            {
                var current = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
                if (current == null)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.Message = current.in_lobby.GetValueOrDefault()
                    ? "Kilépés"
                    : "Csatlakozás";

                ViewBag.UserName = current.username;
                ViewBag.Id = current.id;

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
                return null;
            int? id = null;
            var error = true;
            string btnString = "Csatlakozás";
            if (current.game == null)
            {


                current.in_lobby = !current.in_lobby.GetValueOrDefault();
                _entities.SaveChanges();
                //send the new lobby to users
                GetLobbyList();
                //if the lobby is full, start the game - todo new method!!
                var inLobby = _entities.Users.Where(x => x.in_lobby != null && (bool) x.in_lobby).ToList();
                if (inLobby.Count() >= 2)
                {
                    var newGame = new games() {max_player = 4, users = inLobby, status = true};
                    var gid = _entities.Games.Add(newGame);
                    inLobby.ForEach(x =>
                    {
                        x.in_lobby = false;
                        x.game = newGame;
                    });
                    //egyéb game  indítás...
                    _entities.SaveChanges();
                    var userList = inLobby.Select(x => new { x.username, userId = x.id, gameId = newGame.id });
                    _hubHandler.StartGame(userList);
                    GetLobbyList();
                    id = newGame.id;
                }
                error = false;

                btnString = current.in_lobby.GetValueOrDefault()
                    ? "Kilépés"
                    : "Csatlakozás";
            }

            return Json(new {error = error, btnStr = btnString}, JsonRequestBehavior.AllowGet);
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

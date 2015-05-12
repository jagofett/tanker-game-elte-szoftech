using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalRChatApp.Hubs;
using Sztek.Models;
using System.Web.Script.Serialization;

namespace Sztek.Controllers
{
    public class LobbyController : Controller
    {
        private readonly DatabaseEntities _entities = new DatabaseEntities();
        private readonly HubHandler _hubHandler;

        public LobbyController() : this(HubHandler.Instance) { }

        private LobbyController(HubHandler hubHandler)
        {
            _hubHandler = hubHandler;
            _hubHandler.ActiveGamesList(ActiveGamesList());
        }

        [Authorize]
        [HttpPost]
        public ActionResult JoinGameLobby(int gameId, int teamNr)
        {
            var currentUser = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
            if (currentUser == null)
                return null;

            var currentGame = _entities.Games.FirstOrDefault(g => g.id == gameId);
            if (currentGame == null)
                return null;

            var error = true;

            if (currentUser.game == null && currentUser.inLobby == false)
            {
                var userGame = new userGame()
                {
                    game = currentGame,
                    user = currentUser,
                    team = teamNr
                };
                _entities.UserGames.Add(userGame);
                _entities.SaveChanges();
                
                currentUser.inLobby = !currentUser.inLobby.GetValueOrDefault();
                _entities.SaveChanges();

                error = false;
            }
            //send the new lobby to users
            GetActiveGamesList();

            return Json(new { error = error }, JsonRequestBehavior.AllowGet);
        }

        public void CreateNewGame(int typeNr)
        {
            var newGame = new games()
            {
                gameType = typeNr,
                status = true
            };
            var gameEntity = _entities.Games.Add(newGame);
            gameEntity.gameName = typeNr == 0
                ? "DeathMatch" + _entities.Games.Count()
                : "TeamDeathMatch" + _entities.Games.Count();
            _entities.SaveChanges();
        }

        public void GetActiveGamesList()
        {
            if (Request.IsAuthenticated)
            {
                _hubHandler.ActiveGamesList(ActiveGamesList());
            }
        }

        private Object LobbyUsers()
        {
            return _entities.Users.Where(user => user.inLobby != null && (bool)user.inLobby)
                        .OrderBy(us => us.username)
                        .Select(x => new { x.username, x.id })
                        .ToList();
        }

        private Object ActiveGamesList()
        {
            var serializer = new JavaScriptSerializer(); 

            var actives = _entities.Games.Where(game => game.status != null && (bool)game.status)
                        .OrderBy(game => game.id)
                        .Select(x => new { x.id, x.gameName, x.gameType })
                        .ToList();

            List<object> act = new List<object>();
            foreach (var a in actives)
            {
                var users = _entities.UserGames.Where(g => g.game.id == a.id).ToArray();
                act.Add(new { a.gameName, a.gameType, a.id, users });
            }

            return act;
        }

        public int GetCurrentUserId()
        {
            return _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name).id;
        }

        public ActionResult GetUserId()
        {
            var user = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name).id;
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Lobby()
        {
            if (Request.IsAuthenticated)
            {
                var current = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
                if (current == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Message = current.inLobby.GetValueOrDefault()
                    ? "Kilépés"
                    : "Csatlakozás";

                return View(_entities);
            }
            else
            {
                return RedirectToAction("NotLoggedIn", "Home");
            }
        }
    }
}

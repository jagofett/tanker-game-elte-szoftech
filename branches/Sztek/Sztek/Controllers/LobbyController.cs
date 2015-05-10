using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalRChatApp.Hubs;
using Sztek.Models;

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
            _hubHandler.LobbyList(LobbyUsers());
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

            foreach (var g in _entities.Games)
            {
                if (g.gameName == null)
                {
                    g.gameName = g.gameType == 0
                    ? "DeathMatch" + g.id
                    : "TeamDeathMatch" + g.id;
                }
            }
            _entities.SaveChanges();

            if (current.game == null)
            {
                current.inLobby = !current.inLobby.GetValueOrDefault();
                _entities.SaveChanges();
                //send the new lobby to users
                GetLobbyList();
                //if the lobby is full, start the game - todo new method!!
                var inLobby = _entities.Users.Where(x => x.inLobby != null && (bool)x.inLobby).ToList();
                if (inLobby.Count() >= 2)
                {
                    var newGame = new games() { 
                        gameType = 0,
                        users = inLobby, 
                        status = true 
                    };
                    var gid = _entities.Games.Add(newGame);
                    inLobby.ForEach(x =>
                    {
                        x.inLobby = false;
                        x.game = newGame;
                    });
                    
                    _entities.Games.First(x => x.id == gid.id).gameName = gid.gameType == 0
                        ? "DeathMatch" + gid.id
                        : "TeamDeathMatch" + gid.id;

                    //egyéb game  indítás...
                    _entities.SaveChanges();

                    //soap hívás, server instance létrehozása
                    var port = (newGame.id % 10000) + 10000;
                    var users = inLobby.Select(users1 => users1.id.ToString()).ToArray();
                    //var proxy = new MainClient();
                    //proxy.startGameServer(port.ToString(), users[0],users[1], users[2], users[3]);

                    var userList = inLobby.Select(x => new { x.username, userId = x.id, gameId = newGame.id });
                    _hubHandler.StartGame(userList);
                    GetLobbyList();
                    id = newGame.id;
                }
                error = false;

                btnString = current.inLobby.GetValueOrDefault()
                    ? "Kilépés"
                    : "Csatlakozás";
            }

            return Json(new { error = error, btnStr = btnString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult JoinLobby(int gameId)
        {
            var current = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
            if (current == null)
                return null;

            int? id = null;
            var error = true;
            string btnString = "Csatlakozás";

            if (current.game == null)
            {
                current.inLobby = !current.inLobby.GetValueOrDefault();
                _entities.SaveChanges();
                //send the new lobby to users
                GetLobbyList();
                //if the lobby is full, start the game - todo new method!!
                var inLobby = _entities.Users.Where(x => x.inLobby != null && (bool)x.inLobby).ToList();
                /*if (inLobby.Count() >= 2)
                {
                    var gid = _entities.Games.Add(newGame);
                    inLobby.ForEach(x =>
                    {
                        x.inLobby = false;
                        x.game = newGame;
                    });

                    _entities.Games.First(x => x.id == gid.id).gameName = gid.gameType == 0
                        ? "DeathMatch" + gid.id
                        : "TeamDeathMatch" + gid.id;

                    //egyéb game  indítás...
                    _entities.SaveChanges();

                    //soap hívás, server instance létrehozása
                    var port = (newGame.id % 10000) + 10000;
                    var users = inLobby.Select(users1 => users1.id.ToString()).ToArray();
                    //var proxy = new MainClient();
                    //proxy.startGameServer(port.ToString(), users[0],users[1], users[2], users[3]);

                    var userList = inLobby.Select(x => new { x.username, userId = x.id, gameId = newGame.id });
                    _hubHandler.StartGame(userList);
                    GetLobbyList();
                    id = newGame.id;
                }*/
                error = false;

                btnString = current.inLobby.GetValueOrDefault()
                    ? "Kilépés"
                    : "Csatlakozás";
            }

            return Json(new { error = error, btnStr = btnString }, JsonRequestBehavior.AllowGet);
        }

        public void GetLobbyList()
        {
            if (Request.IsAuthenticated)
            {
                _hubHandler.LobbyList(LobbyUsers());
                _hubHandler.ActiveGamesList(ActiveGamesList());
            }
        }

        public JsonResult JoinedToLobby()
        {
            return new JsonResult();
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
            var actives = _entities.Games.Where(game => game.status != null && (bool)game.status)
                        .OrderBy(game => game.id)
                        .Select(x => new { x.id, x.gameName, x.gameType, 
                            hasCurrentUser = x.users.Contains(_entities.Users.FirstOrDefault(us => us.username == User.Identity.Name))
                        })
                        .ToList();

            return actives;
        }

        public int GetCurrentUserId()
        {
            return _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name).id;
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

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


                current.inLobby = !current.inLobby.GetValueOrDefault();
                _entities.SaveChanges();
                //send the new lobby to users
                GetLobbyList();
                //if the lobby is full, start the game - todo new method!!
                var inLobby = _entities.Users.Where(x => x.inLobby != null && (bool)x.inLobby).ToList();
                if (inLobby.Count() >= 2)
                {
                    var newGame = new games() { users = inLobby, status = true };
                    var gid = _entities.Games.Add(newGame);
                    inLobby.ForEach(x =>
                    {
                        x.inLobby = false;
                        x.game = newGame;
                    });
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

        public void GetLobbyList()
        {
            if (Request.IsAuthenticated)
            {
                _hubHandler.LobbyList(LobbyUsers());
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

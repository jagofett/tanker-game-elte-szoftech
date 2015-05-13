using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalRChatApp.Hubs;
using Sztek.Models;
using System.Web.Script.Serialization;
using Sztek.SztekServiceReference;

namespace Sztek.Controllers
{
    public class LobbyController : Controller
    {
        private readonly DatabaseEntities _entities = new DatabaseEntities();
        private readonly HubHandler _hubHandler;
        private readonly MainClient _proxy = new MainClient();

        public LobbyController() : this(HubHandler.Instance) { }

        private LobbyController(HubHandler hubHandler)
        {
            _hubHandler = hubHandler;
            _hubHandler.ActiveGamesList(ActiveGamesList());
        }
        //use port 12345 and 12346
        private bool StartGameServer(int port, List<int> players, int gameId, int gameType)
        {
            var type = gameType == 1 ? "tdm" : "ffa";
            if (players.Count() != 4)
            {
                return false;
            }
            try
            {
                _proxy.startGameServer(port.ToString(), players[0].ToString(), players[1].ToString(), players[2].ToString(),
                    players[3].ToString(), gameId.ToString(), type);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
                //throw;
            }
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

                //// Játék indítás ////
                var userGamesList = _entities.UserGames.Where(g => g.game.id == gameId).ToList();
                var started = false;
                if (userGamesList.Count >= 4)
                {
                    // Indítsd a játékot!
                    if (userGamesList[0].game.gameType == 1)
                    {
                        // TeamDeathMatch
                        var port = 12345; //debug only
                        var team1 = userGamesList.Where(us => us.team == 0).Select(x => x.user.id).ToList();
                        var team2 = userGamesList.Where(us => us.team == 1).Select(x => x.user.id).ToList();
                        started = StartGameServer(port, team1.Concat(team2).ToList(), gameId, 1);
                    }
                    else
                    {
                        // DeathMatch
                        var port = 12346; //debug only
                        var users = userGamesList.Select(us => us.user.id).ToList();
                        started = StartGameServer(port, users, gameId, 0);

                    }
                    if (started)
                    {
                        userGamesList.ToList().ForEach(us => us.user.inLobby = false);
                        //for debug, remove connected players.
                        _entities.UserGames.RemoveRange(userGamesList);
                        _entities.SaveChanges();
                        //and the started game.
                        _entities.Games.Remove(currentGame);
                        _entities.SaveChanges();
                    }
                }
                //////////////////////

                error = false;
            }
            //send the new lobby to users
            RefreshActiveGamesList();
            RefreshGameMembers(gameId);

            return Json(new { error = error }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateNewGame(int typeNr, string name)
        {
            var error = false;
            var message = "";

            if (!string.IsNullOrEmpty(name))
            {
                var numOfSameName = _entities.Games.Where(x => x.gameName == name && x.status).Count();
                if (numOfSameName > 0)
                {
                    error = true;
                    message = "Már van ilyen nevű aktív játék";
                }
            }

            if (!error)
            {
                var newGame = new games()
                {
                    gameType = typeNr,
                    status = true
                };
                var gameEntity = _entities.Games.Add(newGame);
                gameEntity.gameName = !string.IsNullOrEmpty(name)
                ? name
                : (typeNr == 0
                    ? "DeathMatch" + _entities.Games.Count()
                    : "TeamDeathMatch" + _entities.Games.Count());
                _entities.SaveChanges();

                RefreshActiveGamesList();
            }

            return Json(new { err = error, errMessage = message }, JsonRequestBehavior.AllowGet);
        }

        public void LeaveGame(int gameId)
        {
            var currentUser = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
            var userGame = _entities.UserGames.First(e => e.game.id == gameId && e.user.id == currentUser.id);
            _entities.UserGames.Remove(userGame);
            currentUser.inLobby = false;
            _entities.SaveChanges();

            RefreshGameMembers(gameId);
        }

        public void RefreshGameMembers(int gameId)
        {
            if (Request.IsAuthenticated)
            {
                _hubHandler.GameMemberList(UsersInGame(gameId));
            }
        }

        public ActionResult UsersInGame(int gameId)
        {
            var user = _entities.UserGames.Where(x => x.game.id == gameId).Select(e => new { e.user.id, e.user.username, e.team, e.game.gameType }).ToArray();
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGameDetails(int gameId)
        {
            var isPlayer = _entities.UserGames.Where(x => x.game.id == gameId && x.user.username == User.Identity.Name).Count() > 0
                ? true
                : false;
            var game = _entities.Games.Where(x => x.id == gameId).Select(e => new { e.id, e.gameName, e.gameType, isUserPlayer = isPlayer }).First();
            return Json(game, JsonRequestBehavior.AllowGet);
        }

        #region Active games list
        public void RefreshActiveGamesList()
        {
            if (Request.IsAuthenticated)
            {
                _hubHandler.ActiveGamesList(ActiveGamesList());
            }
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
                var users = _entities.UserGames.Where(g => g.game.id == a.id).Select(u => u.user.username).ToArray();
                act.Add(new { a.gameName, a.gameType, a.id, users });
            }

            return act;
        }
        #endregion

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
                RefreshActiveGamesList();
                return View(_entities);
            }
            else
            {
                return RedirectToAction("NotLoggedIn", "Home");
            }
        }
    }
}

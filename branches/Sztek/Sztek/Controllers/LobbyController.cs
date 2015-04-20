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

        public ActionResult JoinLobby()
        {
            return View();
        }
        public void GetLobbyList()
        {
            
        }


        public JsonResult JoinedToLobby()
        {
            return new JsonResult();
        }

        private Object LobbyUsers()
        {
            return null;
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
    }
}

using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Sztek.Models;
using Sztek.SztekServiceReference;

namespace Sztek.Controllers
{
    public class ToplistController : Controller
    {
        private MainClient _proxy;
        //
        // GET: /TopList/
        private readonly DatabaseEntities _entities = new DatabaseEntities();
        public ActionResult Index()
        {
            _proxy = new MainClient();
            var current = _entities.Users.FirstOrDefault(us => us.username == User.Identity.Name);
            if (current == null)
                return RedirectToAction("NotLoggedIn", "Home");

            var users = _entities.Users.Include("results").ToList();
            var toplists = (from user in users
                let score = user.results.Sum(x => x.score)
                select new toplistModel
                {
                    Name = user.username,
                    Country = user.country,
                    Id = user.id,
                    Score = score
                }).OrderByDescending(x => x.Score).ThenBy(x => x.Name).ToList();

            for (var i = 0; i < toplists.Count; i++)
            {
                toplists[i].Place = i + 1;
            }
            //_proxy.startGameServer("12346", "1", "2", "3", "4", "1516", "ffa");
            //_proxy.startGameServer("12345", "1", "2", "3", "4", "1515", "tdm");
            return View(toplists);
        }
    }
}

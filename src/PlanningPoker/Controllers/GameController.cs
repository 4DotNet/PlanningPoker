using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ScrumPlanningPoker.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ScrumMaster(string naam, ICollection<int> cards)
        {
            var rnd = new Random();
            var key = rnd.Next(1000, 9999);
            
            ViewBag.Naam = naam;
            ViewBag.UniqueKey = key;
            ViewBag.AvailableCards = cards;
            return View(key);
        }

        public ActionResult Developer(string naam, int uniqueKey)
        {
            ViewBag.Naam = naam;
            ViewBag.UniqueKey = uniqueKey;
            return View();
        }

        public ActionResult Hoofdscherm(int uniqueKey)
        {
            return View();
        }
    }
}
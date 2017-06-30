using System;
using ScrumPlanningPoker.Helpers;
using System.Web.Mvc;

namespace ScrumPlanningPoker.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ScrumMaster()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Help()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ScrumMaster(string naam)
        {
            if (string.IsNullOrWhiteSpace(naam))
            {
                ViewBag.ErrorMessage = "U dient een naam in te vullen.";

                return View();
            }

            var redirectUrl = "/Game/ScrumMaster?naam=" + naam;

            // Checkbox waardes komen een beetje raar door, dit werkt.i
            if (!string.IsNullOrWhiteSpace(Request["generateQR"]))
                redirectUrl += "&generateQR=1";

            if (!string.IsNullOrWhiteSpace(Request["ShowHistory"]))
                redirectUrl += "&showHistory=1";

            // kijken of key valid is
            if (naam != string.Empty)
                return Redirect(redirectUrl);

            return View();
        }

        [HttpGet]
        public ActionResult Developer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Developer(string naam, string uniqueKey)
        {
            int gameId;

            if (string.IsNullOrWhiteSpace(naam))
            {
                ViewBag.ErrorMessage = "Please enter your name.";

                return View();
            }

            // Naam wordt gebruikt om div id's te matchen, werkt niet met spaties.
            if (naam.Contains(" "))
            {
                ViewBag.ErrorMessage = "Your name cannot contain spaces.";

                return View();
            }

            // Naam (voorlopig) gelimiteerd op 10 tekens voor optimale weergave
            if (naam.Length > 10)
            {
                ViewBag.ErrorMessage = "Your name can be a maximum of ten characters.";

                return View();
            }

            if (!int.TryParse(uniqueKey, out gameId))
            {
                ViewBag.ErrorMessage = "The key needs to be numeric.";

                return View();
            }

            if (!GameManager.Instance.GameExists(gameId))
            {
                ViewBag.ErrorMessage = "No game was found with the key you have provided.";

                return View();
            }

            var player = GameManager.Instance.GetPlayerByName(gameId, naam);
            if ((player != null) && (DateTime.Now.Ticks - player.LastPingResponse) <= 80000000)
            {
                ViewBag.ErrorMessage =
                    "There already is a player with the name you have chosen. Please choose a different name.";

                return View();
            }

            if (string.IsNullOrWhiteSpace(ViewBag.ErrorMessage))
                return Redirect("/Game/Developer?naam=" + naam + "&uniqueKey=" + uniqueKey);
            
            return View();
        }

        [HttpGet]
        public ActionResult Hoofdscherm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Hoofdscherm(int uniqueKey)
        {
            return Redirect("/Game/Hoofdscherm?uniqueKey="+ uniqueKey);
        }
    }
}